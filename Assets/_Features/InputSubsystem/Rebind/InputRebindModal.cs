using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputRebindModal : MonoBehaviour
{
    [InputControl(layout = "Button")]
    [SerializeField] string keyboardCancelInput;
    [InputControl(layout = "Button")]
    [SerializeField] string controllerCancelInput;

    private InputBindingData _currentBinding;
    private RebindingOperation _rebindingOperation;
    private InputActionAsset _inputActionAsset;
    private string _controlScheme;

    public delegate void HandleInputRebindInit(ReadOnlyArray<InputControlScheme> controlSchemes, string cancelInput);
    public delegate void HandleInputRebindBegin(RebindingOperation rebindingOperation, InputBindingData inputBindingData);
    public delegate void HandleInputRebindEnd(bool success, InputBindingData inputBindingData);

    public event HandleInputRebindInit onInitialize;
    public event HandleInputRebindBegin onBegin;
    public event HandleInputRebindEnd OnRebindEnd;

    public event Action<string> OnSchemeUpdated;
    public event Action<InputBindingData, Action<InputBindingData>> OnCreateInputBindingData;
    public event Action<InputActionMap> OnBeginNewInputMapSection;
    public event Action<string, InputBindingData> OnRebindComplete;

    private bool IsRebindingOperationActive() => _currentBinding != null;

    private void Start()
    {
        _inputActionAsset = PlayerInputController.Instance.PlayerInput.actions;
        _controlScheme = PlayerInputController.Instance.PlayerInputUpdateHandler.ActiveScheme.name;

        onInitialize?.Invoke(_inputActionAsset.controlSchemes, keyboardCancelInput);
        ReadBindings();
        PlayerInputController.Instance.PlayerInputUpdateHandler.RegisterCallback(HandleControlSchemaUpdate);
    }

    private void OnDisable()
    {
        PlayerInputController.Instance.PlayerInputUpdateHandler.RemoveCallback(HandleControlSchemaUpdate);
        _rebindingOperation?.Cancel();
    }

    public void Setup(string inputControlScheme)
    {
        _controlScheme = inputControlScheme;
        OnSchemeUpdated?.Invoke(inputControlScheme);
    }

    private void HandleControlSchemaUpdate(string oldScheme, string newScheme)
    {
        _rebindingOperation?.Cancel();
    }

    private void ReadBindings()
    {
        int compositeCount = 0;
        foreach (InputActionMap inputActionMap in _inputActionAsset.actionMaps)
        {
            OnBeginNewInputMapSection?.Invoke(inputActionMap);

            foreach (InputAction inputAction in inputActionMap.actions)
            {
                foreach (InputBinding binding in inputAction.bindings)
                {
                    bool isStick = !string.IsNullOrEmpty(binding.effectiveProcessors) && binding.effectiveProcessors.Contains("StickDeadzone");
                    if (binding.groups != _controlScheme
                        || string.IsNullOrEmpty(binding.effectivePath)
                        || binding.isComposite
                        || isStick)
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

        OnCreateInputBindingData?.Invoke(bindingData, StartRebindingAction);
    }

    private void StartRebindingAction(InputBindingData inputBindingData)
    {
        if (IsRebindingOperationActive())
        {
            return;
        }

        _currentBinding = inputBindingData;
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

        _rebindingOperation.Start();

        onBegin?.Invoke(_rebindingOperation, _currentBinding);
    }

    private void RebindCompleted(RebindingOperation rebindingOperation)
    {
        string oldKey = _currentBinding.inputEffectivePath;
        int bindingIndex = _currentBinding.compositionIndex > 0 ? _currentBinding.compositionIndex : _currentBinding.bindingIndex;
        _currentBinding.inputEffectivePath = rebindingOperation.action.bindings[bindingIndex].effectivePath;

        OnRebindComplete?.Invoke(oldKey, _currentBinding);

        RebindDispose(false);
    }

    private void RebindCanceled(RebindingOperation rebindingOperation)
    {
        RebindDispose(true);
    }

    private void RebindDispose(bool canceled)
    {
        OnRebindEnd?.Invoke(!canceled, _currentBinding);

        _rebindingOperation.Dispose();

        _currentBinding.inputAction.Enable();
        _currentBinding = null;

        PlayerInputController.Instance.PlayerInputUpdateHandler.UpdateAllSubscribers();
    }
}
