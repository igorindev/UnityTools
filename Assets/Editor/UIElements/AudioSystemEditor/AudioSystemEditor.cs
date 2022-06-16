using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class AudioSystemEditor : EditorWindow
{
    [MenuItem("UI Toolkit/AudioSystemEditor")]
    public static void ShowExample()
    {
        AudioSystemEditor wnd = GetWindow<AudioSystemEditor>();
        wnd.titleContent = new GUIContent("AudioSystemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIElements/AudioSystemEditor/AudioSystemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        var visualTree2 = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIElements/AudioSystemEditor/ToggleTemplate.uxml");
        VisualElement labelFromUXML2 = visualTree2.Instantiate();
        root.Add(labelFromUXML2);
        var visualTree3 = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIElements/AudioSystemEditor/ToggleTemplate 1.uxml");
        VisualElement labelFromUXML3 = visualTree3.Instantiate();
        root.Add(labelFromUXML3);
        var scrollView = labelFromUXML2.Q<ScrollView>("Scroll");

        VisualElement ll = new Label("Hello World! From C#");
        scrollView.Add(ll);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UIElements/AudioSystemEditor/AudioSystemEditor.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
    }

    //public void OnEnable()
    //{
    //    // Create UI
    //    var mainTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ScrollViewTest.uxml");
    //    var ui = mainTemplate.Instantiate();
    //    rootVisualElement.Add(ui);
    //    // Support dynamic items
    //    itemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ScrollViewTestItem.uxml");
    //    itemContainer = ui.Q<ScrollView>("itemContainer");
    //    var addItemBtn = ui.Q<Button>("addItemBtn");
    //    addItemBtn.clicked += AddListItem;
    //}
    //
    //void AddListItem()
    //{
    //    var itemUi = itemTemplate.Instantiate();
    //    var itemIdLbl = itemUi.Q<Label>("itemIdLbl");
    //    itemIdLbl.text = System.Guid.NewGuid().ToString();
    //    var deleteItemBtn = itemUi.Q<Button>("deleteItemBtn");
    //    deleteItemBtn.clicked += () => itemUi.RemoveFromHierarchy();
    //    itemContainer.Add(itemUi);
    //}

}