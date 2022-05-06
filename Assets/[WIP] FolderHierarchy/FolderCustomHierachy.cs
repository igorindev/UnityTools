using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FolderCustomHierachy
{
    const string IgnoreIcons = "GameObject Icon, Prefab Icon, d_GameObject Icon, d_Prefab Icon";
    static Dictionary<int, string> instanceToBtnStr = new Dictionary<int, string>();

    static Texture2D openFolderTexture;
    static Texture2D closedFolderTexture;

#if UNITY_2020_1_OR_NEWER
    private const string openedFolderPrefix = "FolderOpened On";
#else
    private const string _openedFolderPrefix = "OpenedFolder";
#endif
    private const string closedFolderPrefix = "d_Folder On";
    private const string emptyFolderPrefix = "d_FolderEmpty On";

    static FolderCustomHierachy()
    {
        openFolderTexture = (Texture2D)EditorGUIUtility.IconContent($"{openedFolderPrefix} Icon").image;
        closedFolderTexture = (Texture2D)EditorGUIUtility.IconContent($"{closedFolderPrefix} Icon").image;

        EditorApplication.hierarchyWindowItemOnGUI += HierarchyUpdate;
    }

    static void HierarchyUpdate(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject == null) { return; }
        Folders folder = gameObject.GetComponent<Folders>();

        if (folder)
        {
            float iconWidth = 18;
            EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
            var padding = new Vector2(0, 5);
            var iconDrawRect = new Rect(selectionRect.xMax - (iconWidth + padding.x), selectionRect.yMin, selectionRect.width, selectionRect.height);
            iconDrawRect.x = 35;

            Texture2D Icon = folder.open ? openFolderTexture : closedFolderTexture;
            GUIContent iconGUIContent = new GUIContent(Icon);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 30,
            };

            if (GUI.Button(FolderButtonPos(selectionRect, gameObject), iconGUIContent, buttonStyle))
            {
                folder.ToggleFolder();
            }

            EditorGUI.LabelField(iconDrawRect, iconGUIContent);
            
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }
        else
        {
            float iconWidth = 18;
            EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
            var padding = new Vector2(0, 5);
            var iconDrawRect = new Rect(selectionRect.xMax - (iconWidth + padding.x), selectionRect.yMin, selectionRect.width, selectionRect.height);
            iconDrawRect.x = 35;
            EditorGUI.LabelField(iconDrawRect, "|");

            EditorGUIUtility.SetIconSize(Vector2.zero);
        }
    }

    static Rect FolderButtonPos(Rect selectionRect, GameObject hierarchyObj)
    {
        Vector2 objTextSuze = GUIStyle.none.CalcSize(new GUIContent(hierarchyObj.name));
        Vector2 position = selectionRect.position;
        position.x += objTextSuze.x + 20f;
        Vector2 size = selectionRect.size;
        size.x = 30f;
        return new Rect(position, size);
    }
}