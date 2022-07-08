#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RemoveOnPlatformsProcessor : IProcessSceneWithReport
{
    public int callbackOrder => 0;
    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report == null) { return; }
        
        //Get root objects in scene
        GameObject[] rootObjects = scene.GetRootGameObjects();

        // iterate root objects and do something
        RemoveOnPlatforms[] allObjects = StartTraverse(rootObjects);

        Debug.Log("Assets to Remove in scene " + scene.name + ": " + allObjects.Length);

        for (int i = 0; i < allObjects.Length; i++)
        {
            Debug.Log("Found object to Remove" + allObjects[i].gameObject.name);
            Handle(allObjects[i]);
        }
    }

    void Handle(RemoveOnPlatforms rp)
    {
        Debug.Log(rp);
        Debug.Log(rp.platforms);

        for (int i = 0; i < rp.platforms.Length; i++)
        {
            if (rp.platforms[i] == EditorUserBuildSettings.activeBuildTarget)
            {
                Debug.Log("Preparing to Remove object " + rp.gameObject.name);
                Object.DestroyImmediate(rp.gameObject);
                Debug.Log("Removed");
                return;
            }
        }

        Object.DestroyImmediate(rp);
    }
    RemoveOnPlatforms[] StartTraverse(GameObject[] toTraverse)
    {
        List<GameObject> traverse = new List<GameObject>();
        List<RemoveOnPlatforms> traversed = new List<RemoveOnPlatforms>();
        foreach (GameObject obj in toTraverse)
        {
            Traverse(obj, ref traverse);
        }

        for (int i = 0; i < traverse.Count; i++)
        {
            if (traverse[i].TryGetComponent(out RemoveOnPlatforms c))
                traversed.Add(c);
        }

        return traversed.ToArray();
    }
    void Traverse(GameObject obj, ref List<GameObject> t)
    {
        t.Add(obj);
        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject, ref t);
        }
    }
}
#endif