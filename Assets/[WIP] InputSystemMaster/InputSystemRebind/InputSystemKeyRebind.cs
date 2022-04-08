using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InputSystemKeyRebind : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] TMP_Text bindingDisplayName;
    [SerializeField] GameObject startRebind;
    [SerializeField] GameObject waitingRebind;

    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        int bindingIndex = playerInput.actions.actionMaps[0].actions[0].GetBindingIndexForControl(playerInput.actions.actionMaps[0].actions[0].controls[0]);
        bindingDisplayName.text = InputControlPath.ToHumanReadableString(playerInput.actions.actionMaps[0].actions[0].bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
    public void StartRebinding()
    {
        startRebind.SetActive(false);
        waitingRebind.SetActive(true);

        

        playerInput.SwitchCurrentActionMap("Menu");

        rebindingOperation = playerInput.actions.actionMaps[0].actions[0].PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RevindComplete())
            .Start();
    }

    void RevindComplete()
    {
        int bindingIndex = playerInput.actions.actionMaps[0].actions[0].GetBindingIndexForControl(playerInput.actions.actionMaps[0].actions[0].controls[0]);
        bindingDisplayName.text = InputControlPath.ToHumanReadableString(playerInput.actions.actionMaps[0].actions[0].bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startRebind.SetActive(true);
        waitingRebind.SetActive(false);

        playerInput.SwitchCurrentActionMap("Player");
        PlayerInputController.Instance.InputSystemCheckDevice.UpdateAllPaths();
    }
}
