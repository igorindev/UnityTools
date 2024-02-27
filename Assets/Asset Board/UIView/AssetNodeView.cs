using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetNodeView : Node
{
    public Action<AssetNodeView> OnNodeSelected;
    public AssetNode node;

    public AssetNodeView(AssetNode node) : base(AssetDatabase.GetAssetPath(AssetBoardView.FindNodeUXML()))
    {
        this.node = node;
        this.node.name = node.reference.name;
        title = node.name.Replace("(Clone)", "").Replace("Node", "").Replace("Reference", "");
        viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;

        Texture2D thumb = AssetPreview.GetAssetPreview(node.reference);
        VisualElement ve = this.Q<VisualElement>("previewImage");

        if (thumb == null)
            thumb = AssetPreview.GetMiniThumbnail(node.reference);

        ve.style.backgroundImage = new StyleBackground(thumb);

        int id = node.reference.GetInstanceID();
        if (AssetPreview.IsLoadingAssetPreview(id))
        {
            EditorCoroutineUtility.StartCoroutine(WaitPreviewLoading(node.reference, id), this);
        }
    }

    IEnumerator WaitPreviewLoading(UnityEngine.Object asset, int id)
    {
        while (AssetPreview.IsLoadingAssetPreview(id))
            yield return null;

        VisualElement ve = this.Q<VisualElement>("previewImage");
        Texture2D thumb = AssetPreview.GetAssetPreview(asset);
        if (thumb)
            ve.style.backgroundImage = new StyleBackground(thumb);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Asset Node (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
    }
}