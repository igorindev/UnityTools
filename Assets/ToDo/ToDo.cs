using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ToDo : MonoBehaviour
{
    List<Tasks> tasks;

    public void AddTask()
    {

    }
    public void RemoveTask()
    {

    }

    void OnDrawGizmos()
    {
        transform.LookAt(SceneView.lastActiveSceneView.camera.transform.position);
        transform.Rotate(0, 180, 0);
    }
}
