using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EckTechGames
{
	[InitializeOnLoad]
	public class AutoSaveExtension
	{
		// Static constructor that gets called when unity fires up.
		static AutoSaveExtension()
		{
			EditorApplication.pauseStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PauseState pauseState)
		{
			// If we're about to run the scene...
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				// Save the scene and the assets.
				EditorSceneManager.SaveOpenScenes();
				AssetDatabase.SaveAssets();
			}
		}
	}
}