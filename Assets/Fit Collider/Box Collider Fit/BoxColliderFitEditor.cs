using UnityEngine;
using UnityEditor;

namespace FBC
{
	[CustomEditor(typeof(BoxColliderFit))]
	public class BoxColliderFitEditor : Editor
	{
		private BoxColliderFit fit;

		void OnEnable()
        {
			fit = target as BoxColliderFit;
            if (fit.state != FitState.Done)
			{
				fit.state = FitState.Done;
				fit.FitColliderFromScratch();
				fit.ApplyScale();
			}
		}

		public override void OnInspectorGUI()
		{
			GUI.changed = false;

			Undo.RecordObject(fit, "BoxColliderFit");

			Color defaultGuiColor = GUI.color;
			//float defaultLabelWidth = EditorGUIUtility.labelWidth;

			GUIStyle bigButtonStyle = new GUIStyle("button");
			bigButtonStyle.fontSize = 12;

			EditorGUILayout.Space();

			if (!fit.HasBoxCollider())
			{
				EditorGUILayout.HelpBox("No BoxCollider", MessageType.Error);

				if (GUILayout.Button("Add a BoxCollider", bigButtonStyle, GUILayout.Height(22)))
					fit.gameObject.AddComponent<BoxCollider>();

				EditorGUILayout.Space();
				return;
			}

			if (fit.HasSkinnedMeshRenderer())
            {
				EditorGUI.BeginChangeCheck();
				fit.isBestFit = EditorGUILayout.Toggle(new GUIContent("Best Fit",
					"turns on/off 'Update When Offscreen' of every SkinnedMeshRenderer"), fit.isBestFit);
				if (EditorGUI.EndChangeCheck())
				{
					fit.ApplyUpdateWhenOffscreen();
					fit.state = FitState.Need;
				}

				EditorGUILayout.Space();
			}

			if (fit.state == FitState.Not)
				GUI.color = Color.red;
			else if (fit.state == FitState.Need)
				GUI.color = Color.yellow;
			if (GUILayout.Button(new GUIContent("Fit", "fits the collider to the object"), bigButtonStyle, GUILayout.Height(22)))
			{
				fit.state = FitState.Done;
				fit.FitColliderFromScratch();
				fit.ApplyScale();
            }
			GUI.color = defaultGuiColor;

			if (fit.state == FitState.Not)
			{
				EditorGUILayout.Space();
				return;
			}

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			fit.isDynamic = EditorGUILayout.Toggle(new GUIContent("Dynamic",
				"resizes and relocate the collider every update"), fit.isDynamic);
			if (EditorGUI.EndChangeCheck())
            {
				fit.FitColliderFromScratch();
				fit.ApplyScale();
			}

			EditorGUILayout.Space();

			GUI.enabled = false;
			EditorGUILayout.Vector3Field(new GUIContent("Base Size", "The calculated fit size of the collider"), fit.size);
			GUI.enabled = true;

			EditorGUI.BeginChangeCheck();
			fit.scale = EditorGUILayout.Vector3Field(new GUIContent("Scale (0.1 ~)",
				"scales the box collider based on 'Base Size'"), fit.scale);
			if (fit.scale.x < 0.1f)
				fit.scale.x = 0.1f;
			if (fit.scale.y < 0.1f)
				fit.scale.y = 0.1f;
			if (fit.scale.z < 0.1f)
				fit.scale.z = 0.1f;
			if (EditorGUI.EndChangeCheck())
				fit.ApplyScale();

			EditorGUILayout.Space();
		}
	}
}
