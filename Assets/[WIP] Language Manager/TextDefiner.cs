using TMPro;
using UnityEditor;
using UnityEngine;

namespace LanguageController.Text
{
    [DisallowMultipleComponent]
    public class TextDefiner : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [Space(10)]
        [SerializeField] TextIDType indexType;
        [SerializeField] [Min(1)] int textNumIndex = 1;
        [SerializeField] string textNameIndex = "context_name_value";

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        public int TextNumIndex { get => textNumIndex; }
        public string TextNameIndex { get => textNameIndex; }
        public string TextComponent { get => text.text; set => text.text = value; }
        public TextIDType IndexType { get => indexType; set => indexType = value; }

        public int SetTextNumIndex { get; set; }
        public string SetTextNameIndex { get; set; }
    }

    public enum TextIDType
    {
        IndexNumber,
        IndexName,
    }
}