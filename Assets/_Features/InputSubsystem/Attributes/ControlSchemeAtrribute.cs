using UnityEngine;

public class ControlSchemeAtrribute : PropertyAttribute
{
    public string Scheme;
    public int PlayerInput;

    public ControlSchemeAtrribute(string scheme, int playerInput)
    {
        Scheme = scheme;
        PlayerInput = playerInput;
    }
}
