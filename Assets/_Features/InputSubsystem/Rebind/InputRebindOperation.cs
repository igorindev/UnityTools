using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputRebindOperation : IDisposable
{
    private RebindingOperation _rebindingOperation = new();

    public event Action onRebindStarted;
    public event Action<InputAction> OnRebindCompleted;
    public event Action OnRebindCanceled;

    public bool IsRebinding => _rebindingOperation.started;

    public void StartRebindingAction(InputBindingData inputBindingData, string cancelInput)
    {
        if (_rebindingOperation.started)
        {
            Debug.Log("Trying to do more than one rebind at the same time.");
            return;
        }

        _rebindingOperation = inputBindingData.compositionIndex > 0
            ? inputBindingData.inputAction.PerformInteractiveRebinding(inputBindingData.compositionIndex)
            : inputBindingData.inputAction.PerformInteractiveRebinding(inputBindingData.bindingIndex);

        _rebindingOperation.action.Disable();

        _rebindingOperation.WithControlsExcluding("Mouse")
            .WithControlsExcluding("Gamepad")
            .WithControlsExcluding(cancelInput)
            .WithCancelingThrough(cancelInput)
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(RebindCanceled)
            .OnComplete(RebindCompleted);

        _rebindingOperation.Start();

        onRebindStarted?.Invoke();
    }

    public void Dispose()
    {
        if (_rebindingOperation.started)
        {
            _rebindingOperation.Cancel();
            return;
        }

        RebindDispose();
    }

    private void RebindCompleted(RebindingOperation rebindingOperation)
    {
        _rebindingOperation.action.Enable();
        OnRebindCompleted?.Invoke(rebindingOperation.action);
        RebindDispose();
    }

    private void RebindCanceled(RebindingOperation rebindingOperation)
    {
        _rebindingOperation.action.Enable();
        OnRebindCanceled?.Invoke();
        RebindDispose();
    }

    private void RebindDispose()
    {
        _rebindingOperation.Dispose();
    }
}
