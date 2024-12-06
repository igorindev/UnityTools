using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ControlSchemeAtrribute))]
public class ControlSchemeAtrributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ControlSchemeAtrribute controlSchemeattribute = (ControlSchemeAtrribute)attribute;
        SerializedPropertyType propertyType = property.propertyType;

        var scheme = controlSchemeattribute.Scheme;

        label.tooltip = "Select Control Scheme to Show";

        //PrefixLabel returns the rect of the right part of the control. It leaves out the label section. We don't have to worry about it. Nice!
        Rect controlRect = EditorGUI.PrefixLabel(position, label);

        Rect[] splittedRect = SplitRect(controlRect, 3);

        if (propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 vector = property.vector2Value;
            float minVal = vector.x;
            float maxVal = vector.y;

            //F2 limits the float to two decimal places (0.00).
            minVal = EditorGUI.FloatField(splittedRect[0], float.Parse(minVal.ToString("F3")));
            maxVal = EditorGUI.FloatField(splittedRect[2], float.Parse(maxVal.ToString("F3")));

            //EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal, controlSchemeattribute.min, controlSchemeattribute.max);
            //
            //if (minVal < controlSchemeattribute.min)
            //{
            //    minVal = controlSchemeattribute.min;
            //}
            //
            //if (maxVal > controlSchemeattribute.max)
            //{
            //    maxVal = controlSchemeattribute.max;
            //}

            vector = new Vector2(minVal > maxVal ? maxVal : minVal, maxVal);

            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = vector;
            }

        }
    }

    Rect[] SplitRect(Rect rectToSplit, int n)
    {
        Rect[] rects = new Rect[n];

        for (int i = 0; i < n; i++)
        {

            rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);

        }

        int padding = (int)rects[0].width - 50;
        int space = 5;

        rects[0].width -= padding + space;
        rects[2].width -= padding + space;

        rects[1].x -= padding;
        rects[1].width += padding * 2;

        rects[2].x += padding + space;

        return rects;
    }
}
