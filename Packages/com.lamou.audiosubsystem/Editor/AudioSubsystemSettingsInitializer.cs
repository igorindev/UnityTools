using AudioSubsystem;
using UnityEditor;
using UnityEngine;

public class AudioSubsystemSettingsInitializer : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        AudioSubsystemSettings audioSettings = Resources.Load<AudioSubsystemSettings>("AudioSubsystem/AudioSubsystemSettings");
        if (audioSettings == null)
        {
            audioSettings = ScriptableObject.CreateInstance<AudioSubsystemSettings>();

            if (!AssetDatabase.IsValidFolder("Assets/Resources/AudioSubsystem"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                AssetDatabase.CreateFolder("Assets/Resources", "AudioSubsystem");
            }

            AssetDatabase.CreateAsset(audioSettings, "Assets/Resources/AudioSubsystem/AudioSubsystemSettings.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
