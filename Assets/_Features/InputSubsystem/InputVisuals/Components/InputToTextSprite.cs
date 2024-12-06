using TMPro;

public class InputToTextSprite : InputTo<TMP_Text>
{
    protected override void HandleInputUpdate(string oldScheme, string newScheme)
    {
        string oldValue = $"<sprite=\"SpriteAsset_{oldScheme}";
        if (oldScheme != newScheme && Conversion.text.Contains(oldValue))
        {
            string newValue = $"<sprite=\"SpriteAsset_{newScheme}";
            Conversion.text.Replace(oldScheme, newValue);
        }
        else
        {
            string[] splits = Conversion.text.Split("<");
            string finalString = splits[0];
            for (int i = 1; i < splits.Length; i++)
            {
                string item = splits[i];
                string[] subSplit = item.Split(">");
                string iconString = InputToSpriteUtility.GetIconString(_inputAction);

                string s = string.Empty;
                if (subSplit.Length != 1)
                {
                    s = subSplit[1];
                }

                finalString += iconString + s;
            }

            Conversion.text = finalString;
        }
    }
}
