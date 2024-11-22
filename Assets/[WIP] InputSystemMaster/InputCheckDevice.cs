using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputSchemes { Control, Key }

public class InputCheckDevice : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] TMP_SpriteAsset spriteAsset;
    [SerializeField] Sprite[] xboxSprite = new Sprite[0];
    [SerializeField] Sprite[] pcSprite = new Sprite[0];
    
    public List<InputIcon> inputIcons = new List<InputIcon>();
    public List<string> names = new List<string>();
    private InputSchemes currentScheme;

    void Start()
    {
        foreach (var item in spriteAsset.spriteCharacterTable)
        {
            names.Add(item.name);
        }

        playerInput.onDeviceRegained += OnControlsChanged;
        playerInput.onControlsChanged += OnControlsChanged;
        currentScheme = (InputSchemes)Enum.Parse(typeof(InputSchemes), PlayerInputController.Instance.playerInput.currentControlScheme);
        SetDeviceChangesCallback();
    }

    public void OnControlsChanged(PlayerInput value)
    {
        currentScheme = (InputSchemes)Enum.Parse(typeof(InputSchemes), value.currentControlScheme);
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
        inputIcon.inputBinding = PlayerInputController.Instance.playerInput.actions.FindAction(inputIcon.inputActionName).bindings.ToList();
        //Debug.Log(InputControlPath.ToHumanReadableString(PlayerInputController.Instance.PlayerInput.actions.actionMaps[0].actions[0].bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice));

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
            }
        };
    }

    Sprite GetSprite(InputBinding[] bindings)
    {
        //Discover what key of the action correnspont to the current Scheme
        int id = -1;
        for (int i = 0; i < bindings.Length; i++)
        {
            if (Enum.TryParse(bindings[i].groups, out InputSchemes s))
            {
                if (s == currentScheme)
                {
                    id = i;
                    break;
                }
            }
        }

        //Get the active scheme and search in the icons correspondents
        return currentScheme switch
        {
            InputSchemes.Control => SearchSprite(xboxSprite, RemoveSpecialCharacters(bindings[id].effectivePath)),
            InputSchemes.Key => SearchSprite(pcSprite, RemoveSpecialCharacters(bindings[id].effectivePath)),
            _ => SearchSprite(xboxSprite, RemoveSpecialCharacters(bindings[id].effectivePath)),
        };
    }

    Sprite SearchSprite(Sprite[] sprites, string path)
    {
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
        StringBuilder sb = new();
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
