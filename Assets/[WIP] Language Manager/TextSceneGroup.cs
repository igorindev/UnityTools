using LanguageController.Text;
using UnityEngine;

namespace LanguageController.Group
{
    public class TextSceneGroup : MonoBehaviour
    {
        [SerializeField] TextDefiner[] textDefinersInScene;

        void Start()
        {
            LanguageManager.instance.SetSceneTexts(textDefinersInScene);
        }
    }
}