using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyWindowGameObjectIcon
{
    const string IgnoreIcons = "GameObject Icon, Prefab Icon, d_GameObject Icon, d_Prefab Icon";
    static Dictionary<int, string> instanceToBtnStr = new Dictionary<int, string>();

    static bool IconCondition = true;
    static Texture2D openFolderTexture;
    static Texture2D closedFolderTexture;

#if UNITY_2020_1_OR_NEWER
    private const string openedFolderPrefix = "FolderOpened";
#else
    private const string _openedFolderPrefix = "OpenedFolder";
#endif
    private const string closedFolderPrefix = "Folder";

    public static void ChangeIcon(bool value)
    {
        if (value)
        {
            IconCondition = value;
        }
        else
        {
            IconCondition = value;
        }
    }

    static HierarchyWindowGameObjectIcon()
    {
        return;
        openFolderTexture = (Texture2D)EditorGUIUtility.IconContent($"{openedFolderPrefix} Icon").image;
        closedFolderTexture = (Texture2D)EditorGUIUtility.IconContent($"{closedFolderPrefix} Icon").image;

        //EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

    static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GUIContent content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(instanceID), null);
        Rect rect = new Rect(selectionRect.xMax - 16, selectionRect.yMin, 16, 16);

        if (content.image && !IgnoreIcons.Contains(content.image.name))
        {
            GUI.DrawTexture(rect, content.image);
        }    
    }

    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
        Rect rect = new Rect(selectionRect.xMax - 15, selectionRect.yMin, 16, 16);

        if (go)
        {
            if (GUI.Toggle(rect, go.activeSelf, "") != go.activeSelf)
            {
                go.SetActive(!go.activeSelf);
            }
        }
    }
}