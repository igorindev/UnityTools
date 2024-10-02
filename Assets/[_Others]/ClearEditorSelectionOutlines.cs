using System.Reflection;
using UnityEditor;

public class ClearEditorSelectionOutlines
{
    // theoretically should work from Unity 2022.1 to 6000.0 (I only tested in 2022.3)
#if UNITY_2023_3_OR_NEWER
    const string FIELD_PARENTS = "s_CachedParentRenderersForOutlining";
    const string FIELD_CHILDREN = "s_CachedChildRenderersForOutlining";
#elif UNITY_2022_2_OR_NEWER
    const string FIELD_PARENTS = "s_CachedParentRenderersFromSelection";
    const string FIELD_CHILDREN = "s_CachedChildRenderersFromSelection";
#elif UNITY_2022_1_OR_NEWER
    const string FIELD_PARENTS = "m_CachedParentRenderersFromSelection";
    const string FIELD_CHILDREN = "m_CachedChildRenderersFromSelection";
#endif

    const BindingFlags bfs = BindingFlags.Static | BindingFlags.NonPublic;
    static readonly FieldInfo sceneOutlineParents = typeof(SceneView).GetField(FIELD_PARENTS, bfs);
    static readonly FieldInfo sceneOutlineChildren = typeof(SceneView).GetField(FIELD_CHILDREN, bfs);

    // <summary>Hides selection outlines until a new selection is made,
    // bypassing the user scene view setting</summary>
    [MenuItem("Tools/Hide Selected Outlines")]
    public static void HideSelectionOutlinesTemporarily()
    {
#if !UNITY_2022_1_OR_NEWER
    throw new NotImplementedException($"Outline hiding not implemented");
#endif
        sceneOutlineParents.SetValue(null, new int[] { });
        sceneOutlineChildren.SetValue(null, new int[] { });
        SceneView.RepaintAll();
    }
}
