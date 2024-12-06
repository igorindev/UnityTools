using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputToImage : InputTo<Image>
{
    protected override void HandleInputUpdate(string oldScheme, string newScheme)
    {
        Conversion.sprite = InputToSpriteUtility.GetIconSprite(_inputAction);
    }
}
