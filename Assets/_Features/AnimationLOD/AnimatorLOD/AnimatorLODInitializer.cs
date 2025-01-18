using UnityEditor;
using UnityEngine;

namespace AnimatorLODSystem
{
    public class AnimatorLODInitializer : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            AnimatorLODSettings animatorLODSettings = Resources.Load<AnimatorLODSettings>("AnimatorLODSettings/AnimatorLODSettings");
            if (animatorLODSettings == null)
            {
                animatorLODSettings = ScriptableObject.CreateInstance<AnimatorLODSettings>();

                if (!AssetDatabase.IsValidFolder("Assets/Resources/AnimatorLODSettings"))
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    AssetDatabase.CreateFolder("Assets/Resources", "AnimatorLODSettings");
                }

                AssetDatabase.CreateAsset(animatorLODSettings, "Assets/Resources/AnimatorLODSettings/AnimatorLODSettings.asset");
                AssetDatabase.SaveAssets();
            }
        }

    }
}
