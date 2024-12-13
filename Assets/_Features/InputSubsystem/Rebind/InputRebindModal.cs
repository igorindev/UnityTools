using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class InputRebindModal : MonoBehaviour
{
    [InputControl(layout = "Button")]
    [SerializeField] string keyboardCancelInput;
    [InputControl(layout = "Button")]
    [SerializeField] string controllerCancelInput;

    private InputBindingData _currentBindingTryingRebind;
    private InputActionAsset _inputActionAsset;
    private InputRebindOperation _inputRebindOperation;
    private string _controlScheme;

    public delegate void HandleInputRebindInit(string controlScheme, string cancelInput);
    public delegate void HandleInputRebindBegin(InputBindingData inputBindingData);
    public delegate void HandleInputRebindCanceled(InputBindingData inputBindingData);

    public event HandleInputRebindInit onInitialize;
    public event HandleInputRebindBegin onRebindStart;
    public event HandleInputRebindCanceled OnRebindCancel;

    public event Action<string, InputBindingData, Action<InputBindingData>> OnCreateInputBindingData;
    public event Action<InputActionMap> OnBeginNewInputMapSection;
    public event Action<string, InputBindingData> OnRebindComplete;

    public bool IsRebindingOperationActive => _inputRebindOperation.IsRebinding;

    private void Start()
    {
        _inputRebindOperation = new();
        _inputRebindOperation.OnRebindCompleted += RebindCompleted;
        _inputRebindOperation.OnRebindCanceled += RebindCanceled;

        _inputActionAsset = PlayerInputController.Instance.PlayerInput.actions;
        _controlScheme = PlayerInputController.Instance.PlayerInputUpdateHandler.ActiveScheme.name;

        Setup(_controlScheme);
    }

    private void OnDestroy()
    {
        _inputRebindOperation.OnRebindCompleted -= RebindCompleted;
        _inputRebindOperation.OnRebindCanceled -= RebindCanceled;
        _inputRebindOperation.Dispose();
    }

    public void Setup(string inputControlScheme)
    {
        _controlScheme = inputControlScheme;

        onInitialize?.Invoke(_controlScheme, keyboardCancelInput);
        ReadBindings();
    }

    private void ReadBindings()
    {
        int compositeCount = 0;
        foreach (InputActionMap inputActionMap in _inputActionAsset.actionMaps)
        {
            InputControlScheme scheme = inputActionMap.controlSchemes.FirstOrDefault(scheme => scheme.name == _controlScheme);
            if (scheme == default)
            {
                continue;
            }

            OnBeginNewInputMapSection?.Invoke(inputActionMap);

            foreach (InputAction inputAction in inputActionMap.actions)
            {
                foreach (InputBinding binding in inputAction.bindings)
                {
                    bool isStick = !string.IsNullOrEmpty(binding.effectiveProcessors) && binding.effectiveProcessors.Contains("StickDeadzone");
                    if (string.IsNullOrEmpty(binding.effectivePath) || binding.isComposite || isStick)
                    {
                        continue;
                    }

                    compositeCount = binding.isPartOfComposite ? compositeCount + 1 : 0;
                    CreateBindingReference(inputAction, binding, compositeCount);
                }
            }
        };
    }

    private void CreateBindingReference(InputAction inputAction, InputBinding binding, int compositeCount)
    {
        int bindIndex = inputAction.bindings.IndexOf(x => x.id == binding.id);
        string actionName = binding.isPartOfComposite ? binding.name : inputAction.name;

        var bindingData = new InputBindingData()
        {
            inputAction = inputAction,
            name = actionName,
            scheme = binding.groups,
            inputEffectivePath = binding.effectivePath,
            compositionIndex = compositeCount,
            bindingIndex = bindIndex,
        };

        OnCreateInputBindingData?.Invoke(binding.groups, bindingData, StartRebindingAction);
    }

    private void StartRebindingAction(InputBindingData inputBindingData)
    {
        if (IsRebindingOperationActive)
        {
            return;
        }

        _inputRebindOperation.StartRebindingAction(inputBindingData, keyboardCancelInput);

        onRebindStart?.Invoke(_currentBindingTryingRebind);
    }

    private void RebindCompleted(InputAction modifiedAction)
    {
        string oldKey = _currentBindingTryingRebind.inputEffectivePath;
        int bindingIndex = _currentBindingTryingRebind.compositionIndex > 0 ? _currentBindingTryingRebind.compositionIndex : _currentBindingTryingRebind.bindingIndex;
        _currentBindingTryingRebind.inputEffectivePath = modifiedAction.bindings[bindingIndex].effectivePath;

        OnRebindComplete?.Invoke(oldKey, _currentBindingTryingRebind);

        RebindDispose();
    }

    private void RebindCanceled()
    {
        OnRebindCancel?.Invoke(_currentBindingTryingRebind);

        RebindDispose();
    }

    private void RebindDispose()
    {
        _currentBindingTryingRebind = null;
        PlayerInputController.Instance.PlayerInputUpdateHandler.UpdateAllSubscribers();
    }
}
