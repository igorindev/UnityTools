using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputUpdateHandler
{
    private event Action<string, string> _onInputSchemeUpdate;

    private PlayerInput _playerInput;
    private IReadOnlyDictionary<string, InputControlScheme> _schemes;

    Dictionary<InputAction, InputTo> a;

    public InputControlScheme ActiveScheme { get; private set; }

    public PlayerInputUpdateHandler(PlayerInput playerInput, ReadOnlyArray<InputControlScheme> schemes)
    {
        InputSystem.onDeviceChange += HandleDeviceChange;

        _playerInput = playerInput;
        _playerInput.onDeviceRegained += OnControlsChanged;
        _playerInput.onControlsChanged += OnControlsChanged;

        _schemes = schemes.ToDictionary(scheme => scheme.name, scheme => scheme);
        ActiveScheme = _schemes[_playerInput.currentControlScheme];
    }

    ~PlayerInputUpdateHandler()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;

        _playerInput.onDeviceRegained -= OnControlsChanged;
        _playerInput.onControlsChanged -= OnControlsChanged;
    }

    public void RegisterCallback(Action<string, string> updateIcon)
    {
        _onInputSchemeUpdate += updateIcon;
        updateIcon.Invoke(ActiveScheme.name, ActiveScheme.name);
    }

    public void RemoveCallback(Action<string, string> updateIcon)
    {
        _onInputSchemeUpdate -= updateIcon;
    }

    private void OnControlsChanged(PlayerInput value)
    {
        InputControlScheme old = ActiveScheme;
        ActiveScheme = _schemes[_playerInput.currentControlScheme];

        SendUpdateInputCallback(old, ActiveScheme);
    }

    private void SendUpdateInputCallback(InputControlScheme oldValue, InputControlScheme newValue)
    {
        _onInputSchemeUpdate?.Invoke(oldValue.name, newValue.name);
    }

    public void UpdateAllSubscribers()
    {
        SendUpdateInputCallback(ActiveScheme, ActiveScheme);
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                //Debug.Log("ADDED");
                break;
            case InputDeviceChange.Removed:
                //Debug.Log("REMOVED");
                break;
            case InputDeviceChange.Disconnected:
                //Debug.Log("DISCONNECTED");
                break;
            case InputDeviceChange.Reconnected:
                //Debug.Log("RECONNECTED");
                break;
            case InputDeviceChange.Enabled:
                //Debug.Log("ENABLED");
                break;
            case InputDeviceChange.Disabled:
                //Debug.Log("DISABLED");
                break;
            case InputDeviceChange.UsageChanged:
                //Debug.Log("USAGE CHANGED");
                break;
            case InputDeviceChange.ConfigurationChanged:
                //Debug.Log("CONFIGURATION CHANGED");
                break;

        };
    }
}
