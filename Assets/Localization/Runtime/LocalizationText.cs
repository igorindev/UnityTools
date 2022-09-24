using TMPro;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    [DisallowMultipleComponent]
    [CanEditMultipleObjects]
    public class LocalizationText : Localization
    {
        [SerializeField] int index = 1;
        [SerializeField] string contextIndex = "context_name_value";

        TMP_Text text;

        protected override void Initialize()
        {
            text = GetComponent<TMP_Text>();
        }

        public override void UpdateLocalization()
        {
            text.text = LocalizationManager.instance.GetContentAtIndex(index, gameObject);
        }
    }
}