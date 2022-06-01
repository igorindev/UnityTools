using UnityEditor;
using UnityEngine;

public class SplineBake : MonoBehaviour
{
    [HideInInspector] public Vector3[] movePositions = new Vector3[0];
    [HideInInspector] public Vector3[] controlPositions = new Vector3[0];
    [HideInInspector] public int precision = 0;

    public void EditSpline()
    {
        SplineCreator bake = gameObject.AddComponent<SplineCreator>();
        bake.precision = precision;
        for (int i = 0; i < controlPositions.Length; i++)
        {
            bake.controlPoints.Add(new GameObject("Point(" + i + ")"));
            bake.controlPoints[i].transform.position = controlPositions[i];
            bake.controlPoints[i].transform.parent = transform;
            bake.controlPoints[i].transform.parent = transform;
        }

        DestroyImmediate(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < movePositions.Length - 1; i++)
        {
            Gizmos.DrawLine(movePositions[i], movePositions[i + 1]);
        }   
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SplineBake))]
public class SplineBakeEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = target as SplineBake;
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Edit"))
        {
            myScript.EditSpline();
        }
    }
}
#endif
