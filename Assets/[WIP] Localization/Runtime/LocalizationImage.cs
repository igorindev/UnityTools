using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [DisallowMultipleComponent]
    public class LocalizationImage : Localization
    {
        [SerializeField] Sprite[] localizatedSprites;
        Image image;

        protected override void Initialize()
        {
            image = GetComponent<Image>();
        }

        public override void UpdateLocalization()
        {
            image.sprite = localizatedSprites[LocalizationManager.instance.SelectedLanguageIndex];
        }
    }
}
