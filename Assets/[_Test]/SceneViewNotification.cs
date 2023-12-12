using UnityEditor;
using UnityEngine;
public static class NotifyMe
{
    [MenuItem("Test/Notify Me")]
    public static void NotifyMeMenuItem()
    {
        GUIContent content = new GUIContent("Say Yes To Notifications");
        SceneView.lastActiveSceneView.ShowNotification(content, 2);
    }
}