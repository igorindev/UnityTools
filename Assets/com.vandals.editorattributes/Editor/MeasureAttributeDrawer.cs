using UnityEditor;
using UnityEngine;

namespace Attribute.Measure.Editor
{
    [CustomPropertyDrawer(typeof(MeasureAttribute))]
    public class MeasureAttributeDrawer : PropertyDrawer
    {
        GUIStyle GUIStyle;
        Color defaultColor;
        void BuildGUIStyle()
        {
            GUIStyle = new GUIStyle(GUI.skin.label);
            defaultColor = GUI.color;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (GUIStyle == null)
            {
                BuildGUIStyle();
            }

            MeasureAttribute att = attribute as MeasureAttribute;

            EditorGUI.PropertyField(position, property, label);
            
            Rect pos = position;
            pos.x -= 5;
            string unit = att.type switch
            {
                MeasureType.Acceleration => "m/s²",
                MeasureType.Meter        => "m",
                MeasureType.Speed        => "m/s",
                MeasureType.Centimeter   => "cm",
                MeasureType.Hour         => "h",
                MeasureType.Minutes      => "min",
                MeasureType.Seconds      => "sec",
                MeasureType.Milliseconds => "ms",
                MeasureType.Custom       => att.CustomType,
                _ => "default",
            };

            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.label.alignment = TextAnchor.UpperRight;
            GUI.color = new Color(0.6f, 0.6f, 0.6f);
            GUI.Label(pos, unit);
            GUI.color = defaultColor;
            GUI.skin.label = GUIStyle;
        }
    }
}