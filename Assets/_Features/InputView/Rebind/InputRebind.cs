using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

[Serializable]
public class InputBindingsSchema
{
    public string Schema;
    public List<InputBindingData> Bindings;
    public Dictionary<string, List<InputRebindView>> InputKeysBindings;
}

[Serializable]
public class InputBindingData
{
    public string inputActionName;

    public string schema;
    public string inputkey;
    public int bindingIndex;
    public int compositionIndex;

    public InputAction inputAction;
}

public class InputRebind : MonoBehaviour
{
    public PlayerInput playerInput;

    [InputControl(layout = "Button")]
    [SerializeField] string keyboardCancelInput;
    [InputControl(layout = "Button")]
    [SerializeField] string controllerCancelInput;

    [SerializeField] Transform bindsScrollViewHolder;
    [SerializeField] InputRebindView rebindViewPrefab;

    [Header("Warnings")]
    [SerializeField] TextMeshProUGUI cancelWarning;
    [SerializeField] TextMeshProUGUI duplicateWarning;

    public Dictionary<string, InputBindingsSchema> Schemas = new();

    private InputRebindView _currentRebindingView;
    private InputBindingData _currentBinding;
    private RebindingOperation _rebindingOperation;

    private void Start()
    {
        ReadBindings();
    }

    private void OnDisable()
    {
        _rebindingOperation?.Cancel();
    }

    private void ReadBindings()
    {
        if (!playerInput || !playerInput.actions)
        {
            return;
        }

        ReadOnlyArray<InputControlScheme> controlSchemes = playerInput.actions.controlSchemes;
        Schemas = new Dictionary<string, InputBindingsSchema>();

        for (int i = 0; i < controlSchemes.Count; i++)
        {
            Schemas.Add(controlSchemes[i].name, new InputBindingsSchema()
            {
                Schema = controlSchemes[i].name,
                Bindings = new List<InputBindingData>(),
                InputKeysBindings = new Dictionary<string, List<InputRebindView>>()
            });
        }

        List<InputBinding> allInputBindings = new List<InputBinding>();
        foreach (InputBinding item in playerInput.actions.bindings)
        {
            bool isStick = !string.IsNullOrEmpty(item.effectiveProcessors) && item.effectiveProcessors.Contains("StickDeadzone");
            if (string.IsNullOrEmpty(item.effectivePath) || item.isComposite || isStick)
            {
                continue;
            }

            allInputBindings.Add(item);
        };

        allInputBindings = allInputBindings.OrderBy(x => x.groups).ToList();

        int compositeCount = 0;
        int bindIndex = 0;
        string lastAction = string.Empty;

        foreach (InputBinding bind in allInputBindings)
        {
            compositeCount = bind.isPartOfComposite ? compositeCount + 1 : 0;

            InputAction inputAction = playerInput.actions[bind.action];

            bindIndex = inputAction.bindings.IndexOf(x => x.id == bind.id);

            lastAction = bind.action;

            InputRebindView rebindView = Instantiate(rebindViewPrefab, bindsScrollViewHolder);

            if (bind.isPartOfComposite)
            {
                lastAction += compositeCount;
            }

            var bindingData = new InputBindingData()
            {
                inputActionName = lastAction,
                inputkey = bind.effectivePath,
                schema = bind.groups,
                compositionIndex = compositeCount,
                bindingIndex = bindIndex,

                inputAction = inputAction,
            };

            rebindView.Setup(bindingData, StartRebindingAction);

            AddToInputSchema(bindingData, bind, rebindView);
        }
    }

    private void AddToInputSchema(InputBindingData binding, InputBinding bind, InputRebindView inputRebindView)
    {
        foreach (InputBindingsSchema schema in Schemas.Values)
        {
            if (schema.Schema == bind.groups) //Which schema this input belongs
            {
                schema.Bindings.Add(binding);
                if (schema.InputKeysBindings.TryGetValue(binding.inputkey, out List<InputRebindView> list))
                {

                    list.Add(inputRebindView);
                }
                else
                {
                    schema.InputKeysBindings.Add(binding.inputkey, new List<InputRebindView>() { inputRebindView });
                }

                return;
            }
        }
    }

    [ContextMenu("Save Bindings")]
    public void SaveUserRebinds()
    {
        string rebinds = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    [ContextMenu("Load Bindings")]
    public void LoadUserRebinds()
    {
        string rebinds = PlayerPrefs.GetString("rebinds");
        playerInput.actions.LoadBindingOverridesFromJson(rebinds);
    }

    [ContextMenu("Reset Bindings")]
    public void ResetUserRebinds()
    {
        playerInput.actions.RemoveAllBindingOverrides();

        ReadBindings();
    }

    private void StartRebindingAction(InputRebindView rebindView)
    {
        //Is already Rebinding?
        if (_currentBinding != null)
        {
            return;
        }

        _currentBinding = rebindView.Binding;
        _currentRebindingView = rebindView;

        _currentBinding.inputAction.Disable();

        _rebindingOperation = _currentBinding.compositionIndex > 0
            ? _currentBinding.inputAction.PerformInteractiveRebinding(_currentBinding.compositionIndex)
            : _currentBinding.inputAction.PerformInteractiveRebinding(_currentBinding.bindingIndex);

        _rebindingOperation.WithControlsExcluding("Mouse")
            .WithControlsExcluding("Gamepad")
            .WithControlsExcluding(keyboardCancelInput)
            .WithCancelingThrough(keyboardCancelInput)
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(RebindCanceled)
            .OnComplete(RebindCompleted);

        _currentRebindingView.SetAsRebinding();

        _rebindingOperation.Start();

        SetCancelWarningActive(true);
    }

    private void RebindCompleted(RebindingOperation rebindingOperation)
    {
        InputBindingsSchema schema = Schemas[_currentBinding.schema];
        Dictionary<string, List<InputRebindView>> rebindViews = schema.InputKeysBindings;

        rebindViews[_currentRebindingView.Binding.inputkey].Remove(_currentRebindingView);
        CheckDuplicateInput(rebindViews[_currentRebindingView.Binding.inputkey]);

        int bindingIndex = _currentBinding.compositionIndex > 0 ? _currentBinding.compositionIndex : _currentBinding.bindingIndex;
        _currentBinding.inputkey = rebindingOperation.action.bindings[bindingIndex].effectivePath;
        _currentRebindingView.UpdateBindData(_currentBinding);

        if (rebindViews.TryGetValue(_currentBinding.inputkey, out List<InputRebindView> list))
        {
            list.Add(_currentRebindingView);
        }
        else
        {
            rebindViews.Add(_currentBinding.inputkey, new List<InputRebindView>() { _currentRebindingView });
        }

        CheckDuplicateInput(rebindViews[_currentBinding.inputkey]);

        RebindCanceled(rebindingOperation);
    }

    private void CheckDuplicateInput(List<InputRebindView> inputRebindViews)
    {
        bool hasDuplicate = inputRebindViews.Count > 1;

        foreach (InputRebindView view in inputRebindViews)
        {
            view.SetAsDuplicated(hasDuplicate);
        }
    }

    private void RebindCanceled(RebindingOperation rebindingOperation)
    {
        _currentRebindingView.UpdateBindData(_currentBinding);

        SetCancelWarningActive(false);
        _rebindingOperation.Dispose();

        _currentBinding.inputAction.Enable();
        _currentBinding = null;

        PlayerInputController.Instance.PlayerInputUpdateHandler.UpdateAllSubscribers();
    }

    private void SetCancelWarningActive(bool value)
    {
        cancelWarning.text = $"Cancel Rebind {keyboardCancelInput}";
        cancelWarning.gameObject.SetActive(value);
    }
}
