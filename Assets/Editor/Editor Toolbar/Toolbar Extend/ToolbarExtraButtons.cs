using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 8,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                padding = new RectOffset(2, 2, 2, 2),
                fontStyle = FontStyle.Bold,
            };
        }
    }

    [InitializeOnLoad]
    public class ToolbarExtraButtons
    {
        // Define a texture and GUIContent
        static ToolbarExtraButtons()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            ToolbarScriptableObject tasksObj = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Editor/Editor Toolbar/Toolbar.asset") as ToolbarScriptableObject;

            GUILayout.FlexibleSpace();

            GUI.backgroundColor = new Color(1, 1, 1, 0.5f);

            for (int i = 0; i < tasksObj.tools.Length; i++)
            {
                GUIContent content = EditorGUIUtility.IconContent(tasksObj.tools[i].toolIconPath);
                content.tooltip = tasksObj.tools[i].toolTip;

                if (GUILayout.Button(content, ToolbarStyles.commandButtonStyle))
                {
                    tasksObj.tools[i].tool.Perform();
                }
            }
        }
    }

    static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // need to get scene via search because the path to the scene
                // file contains the package version so it'll change over time
                string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Couldn't find scene file");
                }
                else
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    EditorApplication.isPlaying = true;
                }
            }
            sceneToOpen = null;
        }
    }

    public static class ConsoleWindowOpenner
    {
        public static bool isOpen;

        public static void UseConsole()
        {
            System.Type T = System.Type.GetType("UnityEditor.ConsoleWindow, UnityEditor");
            EditorWindow consoleWindow = EditorWindow.GetWindow(T);

            if (isOpen)
            {
                isOpen = false;
                consoleWindow.Close();
            }
            else
            {
                System.Type C = System.Type.GetType("UnityEditor.SceneView, UnityEditor");
                EditorWindow sceneWIndow = EditorWindow.GetWindow(C, false, null, false);
                isOpen = true;
                consoleWindow.position = new Rect(sceneWIndow.position.x, sceneWIndow.position.yMax - consoleWindow.position.height - 7, sceneWIndow.position.width - 10, consoleWindow.position.height);
            }
        }
    }
}