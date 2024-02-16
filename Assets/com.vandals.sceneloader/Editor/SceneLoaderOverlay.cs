using UnityEditor;
using UnityEngine;

#if UNITY_2021_2_OR_NEWER
using UnityEditor.Toolbars;
#endif

namespace SceneLoader
{
#if UNITY_2021_2_OR_NEWER
    [EditorToolbarElement(id, typeof(SceneView))]
    public class SceneLoaderOverlay : EditorToolbarButton, IAccessContainerWindow
    {
        public const string id = "ToolBar/SceneLoader";
        public EditorWindow containerWindow { get; set; }

        public SceneLoaderOverlay()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            if (packageInfo != null)
            {
                icon = AssetDatabase.LoadAssetAtPath($"Packages/{SceneLoaderEditor.uiPath}/Icon.png", typeof(Texture)) as Texture2D;
            }
            else
            {
                icon = AssetDatabase.LoadAssetAtPath($"Assets/{SceneLoaderEditor.uiPath}/Icon.png", typeof(Texture)) as Texture2D;
            }

            text = "";

            tooltip = "Open a scrollable menu that shows all assigned scenes, allowing to load then.";
            clicked += OnClick;
        }

        void OnClick()
        {
            SceneLoaderEditor.ShowWindow();
        }
    }
#endif
}