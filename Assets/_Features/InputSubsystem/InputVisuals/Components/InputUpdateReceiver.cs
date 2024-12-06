using System;
using UnityEngine.InputSystem;

public class InputUpdateReceiver
{
    private InputAction _inputAction;

    public Action<string> OnUpdate { get; internal set; }

    public InputUpdateReceiver(string inputActionNameOrId)
    {
        Initialize(InputToSpriteUtility.GetInputAction(inputActionNameOrId));
    }

    public InputUpdateReceiver(InputAction inputAction)
    {
        Initialize(inputAction);
    }

    private void Initialize(InputAction inputAction)
    {
        _inputAction = inputAction;
        Register();
    }

    ~InputUpdateReceiver()
    {
        Deregister();
    }

    public void Add(Action<string> handleUpdate)
    {
        OnUpdate += handleUpdate;
        HandleInputUpdate(null, null);
    }

    public void Remove(Action<string> handleUpdate)
    {
        OnUpdate -= handleUpdate;
    }

    protected void Register()
    {
        PlayerInputController.Instance.PlayerInputUpdateHandler.RegisterCallback(HandleInputUpdate);
    }

    protected void Deregister()
    {
        PlayerInputController.Instance.PlayerInputUpdateHandler.RemoveCallback(HandleInputUpdate);
    }

    protected void HandleInputUpdate(string oldScheme, string newScheme)
    {
        OnUpdate?.Invoke(InputToSpriteUtility.GetIconString(_inputAction));
    }
}
