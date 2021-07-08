using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class TextDefiner : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] [Min(1)] int textIndex = 1;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
    }

    public int TextIndex { get => textIndex + 1;}
    public string Text { get => text.text; set => text.text = value; }
}
