#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CommentHierarchyIcon
{
    private bool IconCondition = true;
    private Texture2D _openFolderTexture;
    private Texture2D _closedFolderTexture;

#if UNITY_2020_1_OR_NEWER
    private const string _openedFolderPrefix = "FolderOpened";
#else
    private const string _openedFolderPrefix = "OpenedFolder";
#endif
    private const string _closedFolderPrefix = "Folder";

    public CommentHierarchyIcon()
    {
        _openFolderTexture = (Texture2D)EditorGUIUtility.IconContent($"{_openedFolderPrefix} Icon").image;
        _closedFolderTexture = (Texture2D)EditorGUIUtility.IconContent($"{_closedFolderPrefix} Icon").image;

        EditorApplication.hierarchyWindowItemOnGUI += DrawIconOnWindowItem;
    }

    public void ChangeIcon(bool value)
    {
        if (value)
        {
            IconCondition = value;
        }
        else
        {
            IconCondition = value;
        }

        EditorApplication.hierarchyWindowItemOnGUI += DrawIconOnWindowItem;
    }

    private void DrawIconOnWindowItem(int instanceID, Rect rect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject == null)
        {
            return;
        }

        var comment = gameObject.GetComponent<Folders>();

        if (comment != null)
        {
            float iconWidth = 18;
            EditorGUIUtility.SetIconSize(new Vector2(iconWidth, iconWidth));
            var padding = new Vector2(0, 5);
            var iconDrawRect = new Rect(rect.xMax - (iconWidth + padding.x), rect.yMin, rect.width, rect.height);
            iconDrawRect.x = 35;

            Texture2D Icon;
            if (IconCondition)
            {
                Icon = _openFolderTexture;
            }
            else
            {
                Icon = _closedFolderTexture;
            }

            var iconGUIContent = new GUIContent(Icon);
            EditorGUI.LabelField(iconDrawRect, iconGUIContent);
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }
    }
}
#endif