using UnityEngine;
using UnityEngine.UI;


namespace Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
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
            int i = LocalizationManager.instance.SelectedLanguageIndex;
            if(i < localizatedSprites.Length)
            {
                image.sprite = localizatedSprites[i];
            }
            else
            {
                Debug.LogError("The language do not have an image setted, changing to first");
                image.sprite = localizatedSprites[0];
            }
            
        }
    }
}
