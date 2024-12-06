using System.Linq;
using Lamou.InputSystem.SpriteMap;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputToSpriteUtility
{
    private static InputSpritesMap _inputSpritesMap;
    private static PlayerInputUpdateHandler _playerInputUpdateHandler;
    private static InputActionAsset _inputActionAsset;
    private static ControlSchemeSpriteData[] _inputMaps;

    public InputToSpriteUtility(InputSpritesMap inputSpritesMap, PlayerInputUpdateHandler playerInputUpdateHandler, InputActionAsset inputActionAsset)
    {
        _inputSpritesMap = inputSpritesMap;
        _inputMaps = inputSpritesMap._inputMaps;
        _playerInputUpdateHandler = playerInputUpdateHandler;
        _inputActionAsset = inputActionAsset;
    }

    public static InputAction GetInputAction(string actionNameOrId)
    {
        InputAction actionAsset = _inputActionAsset[actionNameOrId];

        Debug.Assert(actionAsset != null, $"Could not find Input Action with name or id {actionNameOrId}");

        return actionAsset;
    }

    public static string GetIconString(string inputAction)
    {
        return GetIconString(GetInputAction(inputAction));
    }

    public static string GetIconString(InputAction inputAction)
    {
        string controlScheme = _playerInputUpdateHandler.ActiveScheme.name;
        string displayString = inputAction.GetBindingDisplayString(0, controlScheme).ToLower();
        return $"<sprite=\"SpriteAsset_{controlScheme}\" name=\"{displayString}\">";
    }

    public static string GetIconString(InputAction inputAction, int bindingIndex)
    {
        string controlScheme = _playerInputUpdateHandler.ActiveScheme.name;
        InputBinding inputBinding = inputAction.bindings[bindingIndex];
        string displayString = GetBindingDisplayStringOrCompositeName(inputAction, controlScheme, inputBinding);
        return $"<sprite=\"SpriteAsset_{controlScheme}\" name=\"{displayString}\">";
    }

    public static string GetIconNameString(InputAction inputAction, string controlScheme)
    {
        return inputAction.GetBindingDisplayString(0, controlScheme).ToLower();
    }

    public static Sprite GetIconSprite(string inputAction)
    {
        return GetIconSprite(GetInputAction(inputAction));
    }

    public static Sprite GetIconSprite(InputAction inputAction)
    {
        string controlScheme = _playerInputUpdateHandler.ActiveScheme.name;
        string displayString = inputAction.GetBindingDisplayString(0, controlScheme).ToLower();

        ControlSchemeSpriteData activeMap = _inputMaps.FirstOrDefault(x => x.DeviceLayoutName == controlScheme);
        int index = activeMap.GeneratedSpriteAsset.GetSpriteIndexFromName(displayString);

        if (index == -1)
        {
            Debug.LogError($"Control Path {displayString} from device {controlScheme} is not available. Check the SpriteAsset_{controlScheme}");
            return null;
        }

        return activeMap.GeneratedSpriteAsset.spriteGlyphTable[index].sprite;
    }

    public static Sprite GetIconSprite(InputAction inputAction, string controlScheme)
    {
        string displayString = inputAction.GetBindingDisplayString(0, controlScheme).ToLower();

        ControlSchemeSpriteData activeMap = _inputMaps.FirstOrDefault(x => x.DeviceLayoutName == controlScheme);

        int index = activeMap.GeneratedSpriteAsset.GetSpriteIndexFromName(displayString);

        if (index == -1)
        {
            Debug.LogError($"Control Path {displayString} from device {controlScheme} is not available. Check the SpriteAsset_{controlScheme}");
            return null;
        }

        return activeMap.GeneratedSpriteAsset.spriteGlyphTable[index].sprite;
    }

    public static Sprite GetIconSprite(string actionName, string controlScheme)
    {
        ControlSchemeSpriteData activeMap = _inputMaps.FirstOrDefault(x => x.DeviceLayoutName == controlScheme);

        int index = activeMap.GeneratedSpriteAsset.GetSpriteIndexFromName(actionName);
        if (index == -1)
        {
            Debug.LogError($"Action name {actionName} from scheme {controlScheme} is not available. Check the SpriteAsset_{controlScheme}");
            return null;
        }

        return activeMap.GeneratedSpriteAsset.spriteInfoList[index].sprite;
    }

    public static string GetBindingDisplayStringOrCompositeName(InputAction action, string scheme, InputBinding inputBindingBase)
    {
        bool isBindingMatchingScheme = InputBinding.MaskByGroup(scheme).Matches(inputBindingBase);
        if (isBindingMatchingScheme && !inputBindingBase.isComposite && !inputBindingBase.isPartOfComposite)
        {
            return action.GetBindingDisplayString(0, scheme).ToLower();
        }

        return inputBindingBase.ToDisplayString().ToLower();
    }

    public static string GetIconString(InputAction inputAction, bool letterAsIcon = true)
    {
        string controlScheme = _playerInputUpdateHandler.ActiveScheme.name;
        string displayString = inputAction.GetBindingDisplayString(0, controlScheme).ToLower();

        if (letterAsIcon)
        {
            return $"<sprite=\"SpriteAsset_{controlScheme}\" name=\"{displayString}\">";
        }

        ControlSchemeSpriteData inputMap = _inputSpritesMap._inputMaps.FirstOrDefault(x => x.DeviceLayoutName == "Key");
        if (inputMap != default && inputMap.BindingsPreset.Bindings.Contains(displayString))
        {
            return displayString;
        }
        else
        {
            return $"<sprite=\"SpriteAsset_{controlScheme}\" name=\"{displayString}\">";
        }
    }
}
