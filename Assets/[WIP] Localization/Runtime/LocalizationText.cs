using TMPro;
using UnityEngine;

namespace Localization
{
    [DisallowMultipleComponent]
    public class LocalizationText : Localization
    {
        public enum TextIDType { IndexNumber, IndexName }

        [SerializeField] TextIDType indexType;
        [SerializeField] [Min(1)] int index = 1;
        [SerializeField] string contextIndex = "context_name_value";

        TMP_Text text;

        protected override void Initialize()
        {
            text = GetComponent<TMP_Text>();
        }

        public override void UpdateLocalization()
        {
            if (indexType == TextIDType.IndexNumber) 
            {
                text.text = LocalizationManager.instance.GetContentAtIndex(index, gameObject);
            }
            else
            {
                text.text = LocalizationManager.instance.GetContentAtIndex(contextIndex, gameObject);
            }            
        }
    }


}

