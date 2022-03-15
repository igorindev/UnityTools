using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

public class MeshCombinerBuilder : MonoBehaviour
{
    [SerializeField] float gridSize = 32;
    List<GameObject> traverseList = new List<GameObject>();


    [ContextMenu("Build")]
    void Combine()
    {
        string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        //Open all scenes sequentiali and combine meshes
        for (int i = 0; i < scenes.Length; i++)
        {
            EditorSceneManager.LoadScene(scenes[i]);

            //Start Combine
            //Find all assets in hierarchy
            traverseList = StartTraverse();

            List<MeshFilter> meshes = new List<MeshFilter>();

            for (int j = 0; j < traverseList.Count; j++)
            {
                if (traverseList[j].TryGetComponent(out MeshFilter meshFilter) && traverseList[j].isStatic)
                {
                    meshes.Add(meshFilter);
                }
            }

            //Find the world bounds

            //Look for close meshes (inside chunk range?)
            //Ignore child meshes and include in same group of parent (simply check in same pos of the root that has mesh filter)

            //Look for same materials

            //Combine each group

            //Delete old GameObjects but preserve colliders
        }

        //Real build
    }

    [ContextMenu("Combine in Editor")]
    void CombineInEditor()
    {
        //Create a backup scene and store it
        if (!AssetDatabase.CopyAsset(EditorSceneManager.GetActiveScene().path, ReadSetupData() + "Backup_" + EditorSceneManager.GetActiveScene().name + ".unity"))
            Debug.LogWarning($"Failed to copy {ReadSetupData() + "Backup_" + EditorSceneManager.GetActiveScene().name + ".unity"}");

        AssetDatabase.Refresh();

        //Combibe meshes on the current scene
    }

    [ContextMenu("Uncombine")]
    void UncombineInEditor()
    {
        //Check if has uncombine option
        if (!CheckIfAssetExists()) { return; }

        //Restore the meshes from the backup scene to the original scene and

        //Remove the combined and

        //Delete the backup scene
        if (!AssetDatabase.DeleteAsset(ReadSetupData() + "Backup_" + EditorSceneManager.GetActiveScene().name + ".unity"))
        {
            Debug.LogWarning($"Failed to delete {ReadSetupData() + "Backup_" + EditorSceneManager.GetActiveScene().name + ".unity"}, the path is correct?");
        }
    }

    bool CheckIfAssetExists()
    {
        Object asset = AssetDatabase.LoadAssetAtPath(ReadSetupData() + "Backup_" + EditorSceneManager.GetActiveScene().name + ".unity", typeof(Object));
        if (asset == null)
        {
            Debug.LogWarning($"The asset at {ReadSetupData() + "Backup_" + EditorSceneManager.GetActiveScene().name + ".unity"} don't exist.");
            return false;
        }
        else
        {
            return true;
        }
    }

    [ContextMenu("Test")]
    string ReadSetupData()
    {
        MonoScript ms = MonoScript.FromMonoBehaviour(this);
        string m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

        FileInfo fi = new FileInfo(m_ScriptFilePath);
        string m_ScriptFolder = fi.Directory.ToString();
        m_ScriptFolder += "\\";
        string r = "Assets" + m_ScriptFolder.Split("Assets")[1] + "UncombinedScenes/";
        r = r.Replace('\\', '/');
        return r;
    }

    List<GameObject> StartTraverse()
    {
        List<GameObject> traverse = new List<GameObject>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>().OrderBy(m => m.transform.GetSiblingIndex()).ToArray())
        {
            if (obj.transform.parent == null)
            {
                Traverse(obj, ref traverse);
            }
        }

        return traverse;
    }
    void Traverse(GameObject obj, ref List<GameObject> t)
    {
        t.Add(obj);
        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject, ref t);
        }
    }

    Bounds WorldBounds()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero); // nothing to see here, go on

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return combinedBounds;
    }

    void OnDrawGizmos()
    {
        Bounds bounds = WorldBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
