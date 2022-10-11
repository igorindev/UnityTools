using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Layouts;

[Serializable]
public struct RebindHolder
{
    public InputAction inputAction;
    public TextMeshProUGUI bindText;
    public TextMeshProUGUI inputNameText;
    public Button button;
    public int bindIndex;
    public int compositionIndex;
    public string path;
}

public class InputSystemKeyRebind : MonoBehaviour
{
    [InputControl(layout = "Button")]
    [SerializeField] string keyboardCancelInput;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] Button[] inputs;

    public RebindHolder currentRebinding;
    public RebindHolder[] bindingHolders;
    public List<InputBinding> allInputs = new List<InputBinding>();
    public List<InputBinding> allInputsNotCleaned = new List<InputBinding>();

    public string SAVE_BINDINGS_PATH;

    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        foreach (InputBinding item in playerInput.actions.bindings)
        {
            allInputsNotCleaned.Add(item);
            if (string.IsNullOrEmpty(item.effectivePath) == false && item.isComposite == false)
            {
                allInputs.Add(item);
            }
        };

        int compositeCount = 0;
        int bindCount = 0;
        string lastAction = "";

        bindingHolders = new RebindHolder[allInputs.Count];

        for (int i = 0; i < allInputs.Count; i++)
        {
            if (allInputs[i].isPartOfComposite)
            {
                compositeCount++;
            }
            else
            {
                compositeCount = 0;
            }

            if (allInputs[i].action == lastAction && allInputs[i].isPartOfComposite == false)
            {
                bindCount++;
            }
            else
            {
                bindCount = 0;
            }

            lastAction = allInputs[i].action;

            RebindHolder rebindHolder = new RebindHolder
            {
                inputAction = playerInput.actions[allInputs[i].action],
                path = allInputs[i].effectivePath, //probably need to be the overrided value when loading
                compositionIndex = compositeCount,
                bindIndex = bindCount,
                button = inputs[i],
                bindText = inputs[i].GetComponentsInChildren<TextMeshProUGUI>()[0],
                inputNameText = inputs[i].GetComponentsInChildren<TextMeshProUGUI>()[1]
            };

            inputs[i].onClick.AddListener(() => StartRebindingAction(rebindHolder));

            rebindHolder.bindText.text = ConvertToTextSprite(rebindHolder.path);
            rebindHolder.inputNameText.text = allInputs[i].isPartOfComposite ? allInputs[i].name : allInputs[i].action;

            bindingHolders[i] = rebindHolder;
        }
    }

    [ContextMenu("Save Bindings")]
    public void SaveUserRebinds()
    {
        var rebinds = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
    [ContextMenu("Load Bindings")]
    public void LoadUserRebinds()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        playerInput.actions.LoadBindingOverridesFromJson(rebinds);
    }
    [ContextMenu("Reset Bindings")]
    public void ResetUserRebinds()
    {
        playerInput.actions.RemoveAllBindingOverrides();

        ShowBindingsInText();
    }
    void ShowBindingsInText()
    {
        for (int i = 0; i < bindingHolders.Length; i++)
        {
            bindingHolders[i].bindText.text = ConvertToTextSprite(bindingHolders[i].path);
        }
    }

    void StartRebindingAction(RebindHolder action)
    {
        if (currentRebinding.button != null) return;

        action.bindText.text = "Waiting Input";

        action.inputAction.Disable();

        currentRebinding = action;

        //NOT ALLOWING SMAE KEY AT 2 PLACES

        if (action.compositionIndex > 0)
            rebindingOperation = action.inputAction.PerformInteractiveRebinding(action.compositionIndex);
        else
            rebindingOperation = action.inputAction.PerformInteractiveRebinding(action.bindIndex);

        rebindingOperation.WithControlsExcluding("Mouse")
            .WithControlsExcluding("Gamepad")
            .WithControlsExcluding(keyboardCancelInput)
            .WithCancelingThrough(keyboardCancelInput)
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(RebindEndedAsCompletedOrCanceled)
            .OnComplete(RebindEndedAsCompletedOrCanceled)
            .Start();

        action.inputAction.Enable();
    }
    void RebindEndedAsCompletedOrCanceled(InputActionRebindingExtensions.RebindingOperation obj)
    {
        rebindingOperation.Dispose();
        currentRebinding.inputAction.Enable();

        int bindingIndex;
        if (currentRebinding.compositionIndex > 0)
            bindingIndex = currentRebinding.compositionIndex;
        else
            bindingIndex = currentRebinding.bindIndex;

        currentRebinding.bindText.text = ConvertToTextSprite(currentRebinding.inputAction.bindings[bindingIndex].effectivePath);

        currentRebinding = new RebindHolder();

        PlayerInputController.Instance.inputSystemCheckDevice.UpdateAllPaths();
    }

    string ConvertToTextSprite(string path)
    {
        path = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames);
        return "<sprite name=" + path + ">";
    }
}
