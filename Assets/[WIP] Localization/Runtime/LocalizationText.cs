using TMPro;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace Localization
{
    [DisallowMultipleComponent]
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