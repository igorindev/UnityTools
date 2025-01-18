using UnityEngine;

namespace AnimatorLODSystem
{
    public class AnimatorLODSettings : ScriptableObject
    {
        public AnimatorLODData[] animatorLODDatas = new AnimatorLODData[1] { AnimatorLODData.Default };

        private void OnValidate()
        {
            if (animatorLODDatas.Length == 0)
            {
                animatorLODDatas = new[] { AnimatorLODData.Default };
            }
        }
    }
}
