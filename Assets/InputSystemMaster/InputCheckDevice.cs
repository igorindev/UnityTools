using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
public enum SCHEMES { Control, Key }

public class InputCheckDevice : MonoBehaviour
{
    [SerializeField] Sprite[] xboxSprite = new Sprite[0];
    [SerializeField] Sprite[] pcSprite = new Sprite[0];
    
    public List<InputIcon> inputIcons = new List<InputIcon>();
    SCHEMES currentScheme;

    void Start()
    {
        currentScheme = (SCHEMES)Enum.Parse(typeof(SCHEMES), PlayerInputController.instance.PlayerInput.currentControlScheme);
        SetDeviceChangesCallback();
    }

    public void OnControlsChanged(PlayerInput value)
    {
        currentScheme = (SCHEMES)Enum.Parse(typeof(SCHEMES), value.currentControlScheme);
        UpdateAllIcons();
    }

    public void AddIconToList(InputIcon inputIcon)
    {
        UpdatePath(inputIcon);

        UpdateIcon(inputIcons.Count - 1);
    }

    public void UpdateAllPaths()
    {
        for (int i = 0; i < inputIcons.Count; i++)
        {
            UpdatePath(inputIcons[i]);
        }

        UpdateAllIcons();
    }

    public void UpdatePath(InputIcon inputIcon)
    {
        inputIcon.inputBinding = PlayerInputController.instance.PlayerInput.actions.FindAction(inputIcon.inputActionName).bindings.ToList();
        Debug.Log(InputControlPath.ToHumanReadableString(PlayerInputController.instance.PlayerInput.actions.actionMaps[0].actions[0].bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice));

        if (!inputIcons.Contains(inputIcon))
        {
            inputIcons.Add(inputIcon);
        }
    }
    public void UpdateAllIcons()
    {
        for (int i = 0; i < inputIcons.Count; i++)
        {
            inputIcons[i].Icon.sprite = GetSprite(inputIcons[i].inputBinding.ToArray());
        }
    }
    public void UpdateIcon(int id)
    {
        inputIcons[id].Icon.sprite = GetSprite(inputIcons[id].inputBinding.ToArray());
    }

    void SetDeviceChangesCallback()
    {
        InputSystem.onDeviceChange +=
        (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    break;
                case InputDeviceChange.Removed:
                    break;
                case InputDeviceChange.Disconnected:
                    break;
                case InputDeviceChange.Reconnected:
                    break;
                case InputDeviceChange.Enabled:
                    break;
                case InputDeviceChange.Disabled:
                    break;
                case InputDeviceChange.UsageChanged:
                    break;
                case InputDeviceChange.ConfigurationChanged:
                    break;
                case InputDeviceChange.Destroyed:
                    break;
            }
        };
    }

    Sprite GetSprite(InputBinding[] bindings)
    {
        //Discover what key of the action correnspont to the current Scheme
        int id = -1;
        for (int i = 0; i < bindings.Length; i++)
        {
            if (Enum.TryParse(bindings[i].groups, out SCHEMES s))
            {
                if (s == currentScheme)
                {
                    id = i;
                    break;
                }
            }
        }

        //Get the active scheme and search in the icons correspondents
        switch (currentScheme)
        {
            case SCHEMES.Control:
                return SearchSprite(xboxSprite, RemoveSpecialCharacters(bindings[id].effectivePath));
            case SCHEMES.Key:
                return SearchSprite(pcSprite, RemoveSpecialCharacters(bindings[id].effectivePath));
            default:
                return SearchSprite(xboxSprite, RemoveSpecialCharacters(bindings[id].effectivePath));
        }
    }
    Sprite SearchSprite(Sprite[] sprites, string path)
    {
        Debug.Log(path);
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == path)
            {
                return sprites[i];
            }
        }

        return null;
    }

    public string RemoveSpecialCharacters(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
