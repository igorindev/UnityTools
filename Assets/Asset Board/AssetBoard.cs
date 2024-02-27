using System.Collections.Generic;
using TheKiwiCoder;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class AssetBoard : ScriptableObject
{
    public List<AssetNode> nodes = new List<AssetNode>();

    public AssetNode CreateNode(System.Type type, Vector2 mousePos, Object reference)
    {
        AssetNode node = ScriptableObject.CreateInstance(type) as AssetNode;
        node.name = type.Name + "Reference";
        node.guid = GUID.Generate().ToString();
        node.position = mousePos;
        node.reference = reference;

        Undo.RecordObject(this, "AssetNode");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCreatedObjectUndo(node, "AssetNode");

        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(AssetNode node)
    {
        Undo.RecordObject(this, "AssetNode");
        nodes.Remove(node);

        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }
}
