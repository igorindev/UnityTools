using UnityEngine;
using UnityEngine.UI;

namespace MobileController
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIInteractable : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }

        /// Probably not necessary since the chain of calls `Rebuild()`->`UpdateGeometry()`->`DoMeshGeneration()`->`OnPopulateMesh()` won't happen; so here really just as a fail-safe.
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects, UnityEditor.CustomEditor(typeof(UIInteractable), false)]
    public class UIInteractableEditor : UnityEditor.UI.GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            GUI.enabled = false;
            UnityEditor.EditorGUILayout.PropertyField(m_Script, new GUILayoutOption[0]);
            GUI.enabled = true;

            // skipping AppearanceControlsGUI
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}