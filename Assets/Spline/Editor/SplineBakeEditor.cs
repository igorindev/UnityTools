using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineBaked))]
public class SplineBakeEditor : Editor
{
    SplineBaked splineBake;
    private void OnEnable()
    {
        splineBake = target as SplineBaked;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Edit"))
        {
            EditSpline();
        }
    }

    void EditSpline()
    {
        SplineCreator creator = splineBake.gameObject.AddComponent<SplineCreator>();
        creator.precision = splineBake.Precision;
        for (int i = 0; i < splineBake.SplineControlPositions.Length; i++)
        {
            creator.controlPoints.Add(new GameObject("Point(" + i + ")"));
            creator.controlPoints[i].transform.position = splineBake.SplineControlPositions[i];
            creator.controlPoints[i].transform.parent = splineBake.transform;
            creator.controlPoints[i].transform.parent = splineBake.transform;
        }

        DestroyImmediate(splineBake);
    }
}
