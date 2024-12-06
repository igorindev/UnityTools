using UnityEngine;
using UnityEngine.InputSystem;

public class InputBindingsSaveData
{
    private PlayerInput _playerInput;

    public InputBindingsSaveData(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }

    public void SaveBindings()
    {
        string rebinds = _playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    public void LoadBindings()
    {
        string rebinds = PlayerPrefs.GetString("rebinds");
        _playerInput.actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void ResetBindings()
    {
        _playerInput.actions.RemoveAllBindingOverrides();
    }
}
