using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public struct RebindHolder
{
    public InputAction inputAction;
    public TextMeshProUGUI bindText;
    public Button button;
    public int bindIndex;
    public int compositionIndex;
    public string path;
}

public class InputSystemKeyRebind : MonoBehaviour
{
    [UnityEngine.InputSystem.Layouts.InputControl(layout = "Button")]
    [SerializeField] string keyboardCancelInput;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] Button[] inputs;

    InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    public RebindHolder currentRebinding;
    public List<InputBinding> b = new List<InputBinding>();
    void Start()
    {
        foreach (var item in playerInput.actions.bindings)
        {
            //Debug.Log(item.effectivePath + " | " + item.groups + " | " + item.isComposite + " | " + item.isPartOfComposite);
            if (string.IsNullOrEmpty(item.effectivePath) == false && item.isComposite == false)
            {
                b.Add(item);
            }
        };

        int compositeCount = 0;
        int bindCount = 0;
        string lastAction = "";
        for (int i = 0; i < b.Count; i++)
        {
            Debug.Log(i);
            if (b[i].isPartOfComposite)
            {
                compositeCount++;
            }
            else
            {
                compositeCount = 0;
            }

            if (b[i].action == lastAction && b[i].isPartOfComposite == false)
            {
                bindCount++;
            }
            else
            {
                bindCount = 0;
            }

            lastAction = b[i].action;

            RebindHolder rebindHolder = new RebindHolder
            {
                inputAction = playerInput.actions[b[i].action],
                path = b[i].effectivePath, //probably need to be the overrided value when loading
                compositionIndex =  compositeCount,
                bindIndex = bindCount,
                button = inputs[i],
                bindText = inputs[i].GetComponentInChildren<TextMeshProUGUI>()
            };

            inputs[i].onClick.AddListener(() => StartRebindingAction(rebindHolder));

            rebindHolder.bindText.text = ConvertToTextSprite(InputControlPath.ToHumanReadableString(rebindHolder.path, 
                                                                                                    InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames));
        }
    }
    public void StartRebindingAction(RebindHolder action)
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

        currentRebinding.bindText.text = ConvertToTextSprite(InputControlPath.ToHumanReadableString(currentRebinding.inputAction.bindings[bindingIndex].effectivePath,
                                                                                                   InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames));

        currentRebinding = new RebindHolder();

        PlayerInputController.Instance.InputSystemCheckDevice.UpdateAllPaths();
    }

    string ConvertToTextSprite(string path)
    {
        return "<sprite name=" + path + ">";
    }
}
