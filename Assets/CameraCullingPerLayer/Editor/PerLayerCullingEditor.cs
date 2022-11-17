using UnityEngine;
using UnityEditor;

namespace UmbraEvolution.PerLayerCameraCulling
{ 
    [CustomEditor(typeof(PerLayerCulling))]
    public class PerLayerCullingEditor : Editor
    {
        // Reference to the PerLayerCulling script that this editor script is attached to
        private PerLayerCulling perLayerCulling;

        private void OnEnable()
        {
            // Collect a reference to the script so that we can modify values if necessary
            perLayerCulling = (PerLayerCulling)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    perLayerCulling.MyCamera.farClipPlane = perLayerCulling.defaultCullDistance = EditorGUILayout.FloatField("Default Cull Distance ", perLayerCulling.defaultCullDistance);
                    GUILayout.Space(10);
                    perLayerCulling.UseSphericalCulling = EditorGUILayout.Toggle("Sphere Culling", perLayerCulling.UseSphericalCulling);
                    
                    if (perLayerCulling.defaultCullDistance < perLayerCulling.MinClip)
                    {
                        perLayerCulling.defaultCullDistance = perLayerCulling.MinClip;
                    }

                    perLayerCulling.TriggerUpdate();
                }
            }

            for (int i = 0; i < PerLayerCulling.NUMBER_OF_UNITY_LAYERS; i++)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i))) continue;

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(i + ". " + LayerMask.LayerToName(i));

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            perLayerCulling.layerCullInfoArray[i].useDefaultCull = EditorGUILayout.ToggleLeft("Use Default", perLayerCulling.layerCullInfoArray[i].useDefaultCull);

                            if (perLayerCulling.layerCullInfoArray[i].useDefaultCull)
                            {
                                GUI.enabled = false;
                            }
                            perLayerCulling.layerCullInfoArray[i].cullDistance = EditorGUILayout.Slider(perLayerCulling.layerCullInfoArray[i].cullDistance, perLayerCulling.MinClip, perLayerCulling.defaultCullDistance);
                        }

                        GUILayout.Space(10);
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.VerticalScope())
                        {
                            perLayerCulling.layerCullInfoArray[i].showGizmo = EditorGUILayout.ToggleLeft("Show Gizmo", perLayerCulling.layerCullInfoArray[i].showGizmo, GUILayout.MaxWidth(100));
                            perLayerCulling.layerCullInfoArray[i].gizmoColor = EditorGUILayout.ColorField(perLayerCulling.layerCullInfoArray[i].gizmoColor, GUILayout.MaxWidth(100));
                        }

                        GUI.enabled = true;
                    }
                }
            }

            // create a button that, when pressed, triggers a method in the script that resets all layers to the current default
            GUIContent resetDefaultsButton = new GUIContent("Reset All to Default", "Press this button to reset all layers to the default culling distance");
            if (GUILayout.Button(resetDefaultsButton))
            {
                // record this action so that the scene is marked as dirty and we can use an undo operation on it
                Undo.RecordObject(perLayerCulling, "Reset All to Default");
                perLayerCulling.ResetAllToDefault();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
