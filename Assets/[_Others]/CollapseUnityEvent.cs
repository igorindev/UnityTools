using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(UnityEventBase), true)]
public class CollapsableEventDrawer : UnityEventDrawer
{
    private VisualElement header;
    private ListView list;

    private bool IsCollapsed
    {
        get => list.style.display == DisplayStyle.None;
        set
        {
            if (value)
                list.style.display = DisplayStyle.None;
            else
                list.style.display = DisplayStyle.Flex;
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement drawer = base.CreatePropertyGUI(property);
        drawer.style.marginBottom = 4f;

        header = drawer.Q<VisualElement>(className: "unity-list-view__header");
        header.RegisterCallback<ClickEvent>(x => IsCollapsed = !IsCollapsed);
        list = drawer.Q<ListView>();

        IsCollapsed = true;
        return drawer;
    }
}