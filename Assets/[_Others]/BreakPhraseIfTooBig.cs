using System.Collections.Generic;

public class StringPhrase
{
    public static List<string> BreakPhraseIfTooBig(string phrase, int maxLetterCount)
    {
        List<string> subStrings = SplitStringByLength(phrase, maxLetterCount);
        List<string> texts = new List<string>(subStrings.Count);

        for (int i = 0; i < subStrings.Count; i++)
        {
            string text = subStrings[i];

            text += i == subStrings.Count - 1 ? "" : "[...]";
            texts.Add(text);
        }

        return texts;
    }

    public static List<string> SplitStringByLength(string input, int maxLength)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < input.Length;)
        {
            int max = maxLength;
            if (i + maxLength <= input.Length) //0 + 200 = 200
            {
                for (int j = i + maxLength - 1; j >= 0; j--)
                {
                    if (char.IsWhiteSpace(input[j]))
                    {
                        max = maxLength - (i + maxLength - j) + 1;
                        break;
                    }
                }

                result.Add(input.Substring(i, max));
            }
            else
            {
                result.Add(input.Substring(i));
            }

            i += max;
        }
        return result;
    }
}