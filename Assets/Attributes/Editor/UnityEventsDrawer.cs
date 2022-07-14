using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(UnityEventBase), true)]
public class UnityEventsDrawer : UnityEventDrawer
{
    protected override void SetupReorderableList(ReorderableList list)
    {
        base.SetupReorderableList(list);

        list.draggable = true;
    }
}