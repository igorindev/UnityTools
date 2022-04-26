using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationTween))]
public class AnimationTweenCustomInspector : Editor
{
    AnimationTween animationTween;

    public override void OnInspectorGUI()
    {
        animationTween = (AnimationTween)target;
        GUILayout.Space(5);
        animationTween.executeOnStart = EditorGUILayout.Toggle("Execute On Start", animationTween.executeOnStart);
        animationTween.loop = EditorGUILayout.Toggle("Loop", animationTween.loop);
        animationTween.testMode = EditorGUILayout.Toggle("TestMode", animationTween.testMode);
        animationTween.duration = EditorGUILayout.FloatField("Duration", animationTween.duration);
        GUILayout.Space(10);

        using (new EditorGUILayout.HorizontalScope())
        {
            animationTween.updatePosition = GUILayout.Toggle(animationTween.updatePosition, "Position", EditorStyles.miniButtonLeft);
            animationTween.updateRotation = GUILayout.Toggle(animationTween.updateRotation, "Rotation", EditorStyles.miniButtonMid);
            animationTween.updateScale = GUILayout.Toggle(animationTween.updateScale, "Scale", EditorStyles.miniButtonRight);
        }

        if (animationTween.updatePosition)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                animationTween.position.desiredVector = EditorGUILayout.Vector3Field("Target Position", animationTween.position.desiredVector);

                EditorGUILayout.LabelField("Axis", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.position.xAxisActive = GUILayout.Toggle(animationTween.position.xAxisActive, "X (" + (animationTween.position.xAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.position.xAxis = EditorGUILayout.CurveField(animationTween.position.xAxis, Color.red, new Rect(0, 0, 1, 1));
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.position.yAxisActive = GUILayout.Toggle(animationTween.position.yAxisActive, "Y (" + (animationTween.position.yAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.position.yAxis = EditorGUILayout.CurveField(animationTween.position.yAxis, Color.green, new Rect(0, 0, 1, 1));
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.position.zAxisActive = GUILayout.Toggle(animationTween.position.zAxisActive, "Z (" + (animationTween.position.zAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.position.zAxis = EditorGUILayout.CurveField(animationTween.position.zAxis, new Color(0, 0.15f, 1f), new Rect(0, 0, 1, 1));
                }
            }
            GUILayout.Space(8);
        }
        
        if (animationTween.updateRotation)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                animationTween.rotation.desiredVector = EditorGUILayout.Vector3Field("Target Rotation", animationTween.rotation.desiredVector);

                EditorGUILayout.LabelField("Axis", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.rotation.xAxisActive = GUILayout.Toggle(animationTween.rotation.xAxisActive, "X (" + (animationTween.rotation.xAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.rotation.xAxis = EditorGUILayout.CurveField(animationTween.rotation.xAxis, Color.red, new Rect(0, 0, 1, 1));
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.rotation.yAxisActive = GUILayout.Toggle(animationTween.rotation.yAxisActive, "Y (" + (animationTween.rotation.yAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.rotation.yAxis = EditorGUILayout.CurveField(animationTween.rotation.yAxis, Color.green, new Rect(0, 0, 1, 1));
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.rotation.zAxisActive = GUILayout.Toggle(animationTween.rotation.zAxisActive, "Z (" + (animationTween.rotation.zAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.rotation.zAxis = EditorGUILayout.CurveField(animationTween.rotation.zAxis, new Color(0, 0.15f, 1f), new Rect(0, 0, 1, 1));
                }
            }
            GUILayout.Space(8);
        }
        
        if (animationTween.updateScale)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                animationTween.scale.desiredVector = EditorGUILayout.Vector3Field("Target Scale", animationTween.scale.desiredVector);

                EditorGUILayout.LabelField("Axis", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.scale.xAxisActive = GUILayout.Toggle(animationTween.scale.xAxisActive, "X (" + (animationTween.scale.xAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.scale.xAxis = EditorGUILayout.CurveField(animationTween.scale.xAxis, Color.red, new Rect(0, 0, 1, 1));
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.scale.yAxisActive = GUILayout.Toggle(animationTween.scale.yAxisActive, "Y (" + (animationTween.scale.yAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.scale.yAxis = EditorGUILayout.CurveField(animationTween.scale.yAxis, Color.green, new Rect(0, 0, 1, 1));
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (animationTween.scale.zAxisActive = GUILayout.Toggle(animationTween.scale.zAxisActive, "Z (" + (animationTween.scale.zAxisActive ? "ON)" : "OFF)"), EditorStyles.miniButtonRight))
                        animationTween.scale.zAxis = EditorGUILayout.CurveField(animationTween.scale.zAxis, new Color(0, 0.15f, 1f), new Rect(0, 0, 1, 1));
                }
            }
            GUILayout.Space(8);
        }
    }
}
