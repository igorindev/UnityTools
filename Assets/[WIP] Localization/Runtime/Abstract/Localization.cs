using System;
using UnityEngine;

namespace Localization
{
    [System.Serializable]
    public abstract class Localization : MonoBehaviour
    {
        void Start()
        {
            if (LocalizationManager.instance)
            {
                LocalizationManager.instance.AddLocalization(this, Initialize);
            }
            else
            {
                Debug.LogError("There is no LocalizationManager created");
            }
        }
        public abstract void UpdateLocalization();
        protected abstract void Initialize();

        void OnDestroy()
        {
            if (LocalizationManager.instance)
            {
                LocalizationManager.instance.Localization.Remove(this);
            }
            else
            {
                if (Application.isPlaying == false)
                    Debug.LogError("There is no LocalizationManager created");
            }
        }
    }
}
