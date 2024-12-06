using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ControlSchemeBindingsPreset", menuName = "Input Bindings/Bindings Preset")]
public class ControlSchemeBindingsPreset : ScriptableObject
{
    [SerializeField]
    private List<string> _bindings = new()
            {
                "0", "1","2", "3", "4", "5", "6", "7", "8", "9",
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "=", "+", "[", "{", "]", "}", "|", "\\", ";", ":", "\"", "\"", ",", "<", ".", ">", "/", "?",
                "spacebar",
                "backspace",
                "tab",
                "enter",
                "esc",
                "delete"
            };

    public IReadOnlyList<string> Bindings => _bindings;
}
