using System;
using UnityEngine.InputSystem;

[Serializable]
public class InputBindingData
{
    public InputAction inputAction;
    public string scheme;
    public string name;
    public string inputEffectivePath;
    public int bindingIndex;
    public int compositionIndex;
}
