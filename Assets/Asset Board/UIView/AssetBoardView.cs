using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetBoardView : GraphView
{
    public new class UxmlFactory : UxmlFactory<AssetBoardView, UxmlTraits> { }

    public Action<AssetNodeView> OnNodeSelected;

    [SerializeField] bool locked = true;

    AssetBoard assetBoard;
    ContentZoomer contentZoomer = new ContentZoomer();
    ContentDragger contentDragger = new ContentDragger();
    SelectionDragger selectionDragger = new SelectionDragger();
    RectangleSelector rectangleSelector = new RectangleSelector();
    DragAndDropManipulator dragAndDrop;
    public GridBackground grid = new GridBackground();

    AssetNodeView currentNodeView;

    public AssetBoardView()
    {
        Insert(0, grid = new GridBackground());

        VisualElement visualElement = new VisualElement()
        {
            name = "drop-area",
        };
        visualElement.style.flexGrow = 1;
        visualElement.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
        visualElement.pickingMode = PickingMode.Ignore;

        Label label = new Label("Drag your assets here")
        {
            name = "drop-area__label",
        };
        label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
        label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        label.style.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        label.style.flexGrow = 1;
        label.style.fontSize = 24;
        label.pickingMode = PickingMode.Ignore;

        visualElement.Insert(0, label);

        Insert(1, visualElement);

        this.AddManipulator(contentZoomer);
        this.AddManipulator(contentDragger);

        //Toggle toggle = this.Q<Toggle>();
        //toggle.RegisterValueChangedCallback(OnToggleLock);

        static StyleSheet FindSettings()
        {
            var guids = AssetDatabase.FindAssets("AssetBoardEditorStyle");
            if (guids.Length > 1)
            {
                Debug.LogWarning($"Found multiple settings files, using the first.");
            }

            switch (guids.Length)
            {
                case 0:
                    return null;
                default:
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            }
        }

        StyleSheet AssetBoardStyle = FindSettings();
        StyleSheet styleSheet = AssetBoardStyle;
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;

        //toggle.value = true;
        LockNodes(null);
    }

    private void OnToggleLock(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
            LockNodes(null);
        else
            UnlockNodes(null);
    }

    public static StyleSheet FindStyle()
    {
        var guids = AssetDatabase.FindAssets("AssetBoardEditorStyle");
        if (guids.Length > 1)
        {
            Debug.LogWarning($"Found multiple settings files, using the first.");
        }

        switch (guids.Length)
        {
            case 0:
                return null;
            default:
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
        }
    }

    public static VisualTreeAsset FindUXML()
    {
        var guids = AssetDatabase.FindAssets("AssetBoardUxml");
        if (guids.Length > 1)
        {
            Debug.LogWarning($"Found multiple settings files, using the first.");
        }

        switch (guids.Length)
        {
            case 0:
                return null;
            default:
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        }
    }

    public static VisualTreeAsset FindNodeUXML()
    {
        var guids = AssetDatabase.FindAssets("AssetNodeViewUxml");
        if (guids.Length > 1)
        {
            Debug.LogWarning($"Found multiple settings files, using the first.");
        }

        switch (guids.Length)
        {
            case 0:
                return null;
            default:
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);

        if (locked)
            evt.menu.AppendAction($"Unlock Nodes", UnlockNodes);
        else
            evt.menu.AppendAction($"Lock Nodes", LockNodes);
        evt.menu.AppendSeparator();
    }

    public void CreateNode(AssetNode node)
    {
        CreateNodeView(node);
    }

    internal void PopulateView(AssetBoard assetBoard)
    {
        this.assetBoard = assetBoard;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChanged;

        if (dragAndDrop != null && dragAndDrop.target == this)
            return;

        this.AddManipulator(dragAndDrop = new DragAndDropManipulator(this, assetBoard));
        dragAndDrop.OnDropInside += OnDrop;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                AssetNodeView nodeView = elem as AssetNodeView;
                if (nodeView != null)
                {
                    assetBoard.DeleteNode(nodeView.node);
                }
            });
        }

        return graphViewChange;
    }

    void DragNodeWhenLocked(AssetNodeView nodeView)
    {
        // Clear existing data in DragAndDrop class.
        DragAndDrop.PrepareStartDrag();

        // Store reference to object and path to object in DragAndDrop static fields.
        DragAndDrop.objectReferences = new[] { nodeView.node.reference };

        // Start a drag.
        DragAndDrop.StartDrag(string.Empty);

        if (locked)
            nodeView.Unselect(this);
    }

    void OnUndoRedo()
    {
        //PopulateView(tree);
        //AssetDatabase.SaveAssets();
    }

    void CreateNodeView(AssetNode node)
    {
        AssetNodeView nodeView = new AssetNodeView(node)
        {
            OnNodeSelected = NodeSelected,
        };
        AddElement(nodeView);

        if (locked)
            nodeView.capabilities &= ~Capabilities.Movable;
        else
            nodeView.capabilities |= Capabilities.Movable;
    }

    void NodeSelected(AssetNodeView nodeView)
    {
        OnNodeSelected?.Invoke(nodeView);

        if (locked)
            DragNodeWhenLocked(nodeView);
    }

    void LockNodes(DropdownMenuAction action)
    {
        locked = true;
        foreach (Node item in nodes)
        {
            item.capabilities &= ~Capabilities.Movable;
        }

        this.RemoveManipulator(selectionDragger);
        this.RemoveManipulator(rectangleSelector);
    }

    void UnlockNodes(DropdownMenuAction action)
    {
        locked = false;
        foreach (Node item in nodes)
        {
            item.capabilities |= Capabilities.Movable;
        }

        this.AddManipulator(selectionDragger);
        this.AddManipulator(rectangleSelector);
    }

    void OnDrop(Vector2 mousePos, UnityEngine.Object reference)
    {
        CreateNode(assetBoard.CreateNode(typeof(AssetNode), mousePos, reference));
    }
}