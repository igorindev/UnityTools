using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Hierarchy window game object icon.
/// http://diegogiacomelli.com.br/unitytips-hierarchy-window-gameobject-icon/
/// </summary>
[InitializeOnLoad]
public static class HierarchyWindowGameObjectIcon
{
    const string IgnoreIcons = "GameObject Icon, Prefab Icon";

    static HierarchyWindowGameObjectIcon()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

    static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(instanceID), null);

        if (content.image != null && !IgnoreIcons.Contains(content.image.name))
            GUI.DrawTexture(new Rect(selectionRect.xMax - 16, selectionRect.yMin, 16, 16), content.image);

    }

    static Dictionary<int, string> instanceToBtnStr = new Dictionary<int, string>();

    private static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        Debug.Log("Over");


        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
        Rect rect = new Rect(selectionRect);
        rect.x = rect.width - 30;


        if (go != null)
        {
            //Add key if it is not in the Dictionary yet
            if (!instanceToBtnStr.ContainsKey(instanceID))
            {
                //Add with Default On Key
                instanceToBtnStr.Add(instanceID, go.activeSelf ? "On" : "Off");
            }

            GUI.skin.button.fixedWidth = 30;

            //Initialize btnStr from Dictionary
            string btnStr = instanceToBtnStr[instanceID];

            if (GUI.Button(rect, btnStr))
            {
                Debug.Log(go.name);
                if (go.activeSelf == true)
                {
                    go.SetActive(false);
                    instanceToBtnStr[instanceID] = "Off";
                }
                else
                {
                    go.SetActive(true);
                    instanceToBtnStr[instanceID] = "On";
                }
            }
        }
    }
}