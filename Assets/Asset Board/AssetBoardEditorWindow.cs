using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetBoardEditorWindow : EditorWindow
{
    AssetBoardView treeView;
    static AssetBoard assetBoard;
    Toggle toggle;

    Button createNewTreeButton;
    VisualElement overlay;

    VisualTreeAsset uxml;
    StyleSheet style;


    [MenuItem("Tools/Asset Board...")]
    public static void OpenWindow()
    {
        AssetBoardEditorWindow wnd = GetWindow<AssetBoardEditorWindow>();
        wnd.titleContent = new GUIContent("Asset Board");
        wnd.minSize = new Vector2(100, 100);

        if (assetBoard == null)
            assetBoard = LoadAssets<AssetBoard>()[0];
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is AssetBoard)
        {
            assetBoard = Selection.activeObject as AssetBoard;
            OpenWindow();
            return true;
        }
        return false;
    }

    static List<T> LoadAssets<T>() where T : Object
    {
        string[] assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        List<T> assets = new List<T>();
        foreach (var assetId in assetIds)
        {
            string path = AssetDatabase.GUIDToAssetPath(assetId);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            assets.Add(asset);
        }
        return assets;
    }

    public void CreateGUI()
    {
        StyleSheet style = AssetBoardView.FindStyle();

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        (uxml = AssetBoardView.FindUXML()).CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        StyleSheet styleSheet = style;
        root.styleSheets.Add(styleSheet);

        // Main treeView
        treeView = root.Q<AssetBoardView>();
        treeView.OnNodeSelected += NodeSelected;
        
        if (assetBoard == null)
            assetBoard = LoadAssets<AssetBoard>()[0];

        if (assetBoard)
        {
            treeView.PopulateView(assetBoard);
            foreach (AssetNode item in assetBoard.nodes)
            {
                treeView.CreateNode(item);
            }
        }
    }

    private void NodeSelected(AssetNodeView view) { }
}
