using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class InputToRawImage : InputTo<RawImage>
{ 
    protected override void HandleInputUpdate(string oldScheme, string newScheme)
    {
        //Conversion.texture = InputToSprite.GetIconSprite(_inputAction);
    }
}
