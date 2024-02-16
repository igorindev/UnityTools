using UnityEditor;

#if UNITY_2021_2_OR_NEWER
using UnityEditor.Overlays;
#endif

namespace SceneLoader
{
#if UNITY_2021_2_OR_NEWER
    [Overlay(typeof(SceneView), "Scene Loader")]
    public class SceneLoaderToolBar : ToolbarOverlay
    {
        SceneLoaderToolBar() : base(SceneLoaderOverlay.id) { }
    }
#endif
}