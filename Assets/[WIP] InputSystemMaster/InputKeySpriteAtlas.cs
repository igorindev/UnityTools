using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputMap
{
    [CreateAssetMenu(fileName = "InputKeySpriteAtlas", menuName = "Input Bindings/Binding Sprites")]
    public class InputKeySpriteAtlas : ScriptableObject
    {
        [Serializable]
        public class InputMap
        {
            public string DeviceLayoutName;
            public Texture2D SpriteAtlas;
            public TMP_SpriteAsset GeneratedSpriteAsset;
            public string[] InputNames;
        }

        public InputMap[] _inputMaps;

        public string[] keyboard =
            {
                "0", "1","2", "3", "4", "5", "6", "7", "8", "9",
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "ç",
                "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "=", "+", "[", "{", "]", "}", "|", "\\", ";", ":", "\"", "\"", ",", "<", ".", ">", "/", "?",
                "spacebar",
                "backspace",
                "tab",
                "enter",
                "esc",
                "delete"
            };

        public string[] gamepad =
            {
            "buttonSouth",
            "buttonNorth",
            "buttonEast",
            "buttonWest",
            "start",
            "select",
            "leftTrigger",
            "rightTrigger",
            "leftShoulder",
            "rightShoulder",
            "dpad" ,
            "dpad/up" ,
            "dpad/down" ,
            "dpad/left" ,
            "dpad/right",
            "leftStick" ,
            "rightStick",
            "leftStickPress",
            "rightStickPress",
        };

        public static string Get(InputAction inputAction, InputBindingData inputBindingData)
        {
            string displayString = inputAction.GetBindingDisplayString(inputBindingData.bindingIndex, out string deviceLayoutName, out string controlPath, 0);
            return $"<sprite=\"SpriteAsset_{deviceLayoutName}\" name=\"{controlPath}\">";
        }

        [Serializable]
        public class GamepadIcons
        {
            public string buttonSouth = "buttonSouth";
            public string buttonNorth = "buttonNorth";
            public string buttonEast = "buttonEast";
            public string buttonWest = "buttonWest";
            public string startButton = "start";
            public string selectButton = "select";
            public string leftTrigger = "leftTrigger";
            public string rightTrigger = "rightTrigger";
            public string leftShoulder = "leftShoulder";
            public string rightShoulder = "rightShoulder";
            public string dpad = "dpad";
            public string dpadUp = "dpad/up";
            public string dpadDown = "dpad/down";
            public string dpadLeft = "dpad/left";
            public string dpadRight = "dpad/right";
            public string leftStick = "leftStick";
            public string rightStick = "rightStick";
            public string leftStickPress = "leftStickPress";
            public string rightStickPress = "rightStickPress";
        }
    }
}
