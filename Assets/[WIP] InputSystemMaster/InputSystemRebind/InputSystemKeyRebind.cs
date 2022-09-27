using System;
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
}

public class InputSystemKeyRebind : MonoBehaviour
{
    [UnityEngine.InputSystem.Layouts.InputControl(layout = "Button")]
    [SerializeField] string keyboardCancelInput;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] InputActionReference[] inputActions;
    [SerializeField] Button[] inputs;

    InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    public RebindHolder currentRebinded;

    void Start()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            RebindHolder rebindHolder = new RebindHolder
            {
                inputAction = playerInput.actions[inputActions[i].ToInputAction().name],
                button = inputs[i],
                bindText = inputs[i].GetComponentInChildren<TextMeshProUGUI>()
            };

            inputs[i].onClick.AddListener(() => StartRebindingAction(rebindHolder));

            int bindingIndex = rebindHolder.inputAction.GetBindingIndexForControl(rebindHolder.inputAction.controls[0]);
            rebindHolder.bindText.text = ConvertToTextSprite(InputControlPath.ToHumanReadableString(rebindHolder.inputAction.bindings[bindingIndex].effectivePath, 
                                                                                                    InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames));
        }
    }
    public void StartRebindingAction(RebindHolder action)
    {
        action.bindText.text = "Waiting Input";

        playerInput.SwitchCurrentActionMap("Menu");
        action.inputAction.Disable();
        currentRebinded = action;
        rebindingOperation = action.inputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithControlsExcluding("Joystick")
            .WithControlsExcluding(keyboardCancelInput)
            .WithCancelingThrough(keyboardCancelInput)
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(RebindComplete)
            .OnComplete(RebindComplete)
            .Start();

        action.inputAction.Enable();
    }
    void RebindComplete(InputActionRebindingExtensions.RebindingOperation obj)
    {
        int bindingIndex = currentRebinded.inputAction.GetBindingIndexForControl(currentRebinded.inputAction.controls[0]);
        currentRebinded.bindText.text = ConvertToTextSprite(InputControlPath.ToHumanReadableString(currentRebinded.inputAction.bindings[bindingIndex].effectivePath,
                                                                                                   InputControlPath.HumanReadableStringOptions.OmitDevice | InputControlPath.HumanReadableStringOptions.UseShortNames));
        rebindingOperation.Dispose();

        currentRebinded = new RebindHolder();

        playerInput.SwitchCurrentActionMap("Player");
        PlayerInputController.Instance.InputSystemCheckDevice.UpdateAllPaths();
    }

    string ConvertToTextSprite(string path)
    {
        return "<sprite name=" + path + ">";
    }
}
