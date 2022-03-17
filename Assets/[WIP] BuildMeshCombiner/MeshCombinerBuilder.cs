using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[System.Serializable]
public struct Chuncks
{
    public List<MeshFilter> meshFilters;
}

public class MeshCombinerBuilder : MonoBehaviour
{
    [SerializeField] float gridSize = 32;
    [SerializeField] List<Chuncks> chuncks = new List<Chuncks>();
    [SerializeField] List<MeshFilter> foundMeshes = new List<MeshFilter>();
    [SerializeField] List<MeshFilter> remainMeshes = new List<MeshFilter>();
    [SerializeField] List<GameObject> traverseList = new List<GameObject>();
    int forwardCount = 0;
    int rightCount = 0;
    int upCount = 0;

    int forwards = 0;
    int rights = 0;
    int ups = 0;

    void StartCombine()
    {
        string[] scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        //Open all scenes sequentiali and combine meshes
        for (int i = 0; i < scenes.Length; i++)
        {
            EditorSceneManager.LoadScene(scenes[i]);

            Combine();
        }

        //Real build
    }

    [ContextMenu("Build")]
    void Combine()
    {
        //Start Combine
        //Find all assets in hierarchy
        traverseList = StartTraverse();

        foundMeshes = new List<MeshFilter>();

        for (int i = 0; i < traverseList.Count; i++)
        {
            if (traverseList[i].TryGetComponent(out MeshFilter meshFilter))
            {
                foundMeshes.Add(meshFilter);
            }
        }

        //Find the world bounds
        Bounds worldBounds = WorldBounds();
        Vector3 min = worldBounds.min;

        remainMeshes = foundMeshes.GetRange(0, foundMeshes.Count);

        chuncks = new List<Chuncks>();

        //Look for close meshes (inside chunk range?)

        //Num of chunks
        forwardCount = 0;
        rightCount = 0;
        upCount = 0;
        Bounds currentBounds;
        do
        {
            do
            {
                do
                {
                    Chuncks newChunck = new Chuncks();
                    newChunck.meshFilters = new List<MeshFilter>();

                    currentBounds = new Bounds(min + (gridSize * forwardCount * Vector3.forward) + (gridSize * upCount * Vector3.up) + (gridSize * rightCount * Vector3.right), (Vector3.one * gridSize));

                    for (int i = remainMeshes.Count - 1; i >= 0; i--)
                    {
                        if (currentBounds.Contains(remainMeshes[i].transform.position))
                        {
                            newChunck.meshFilters.Add(remainMeshes[i]);
                            remainMeshes.Remove(remainMeshes[i]);
                        }
                    }

                    chuncks.Add(newChunck);
                    forwardCount += 1;
                }
                while (currentBounds.max.z <= worldBounds.max.z);

                forwards = forwardCount;
                forwardCount = 0;
                upCount += 1;
            }
            while (currentBounds.max.y <= worldBounds.max.y);

            ups = upCount;
            upCount = 0;

            rightCount += 1;
            rights = rightCount;
        }
        while (currentBounds.max.x <= worldBounds.max.x);
        //Ignore child meshes and include in same group of parent (simply check in same pos of the root that has mesh filter)

        //Look for same materials

        //Look for num of verices and separete group in small groups

        //Combine each group

        //Delete old GameObjects but preserve colliders

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
        Gizmos.color = Color.blue;
        int count = 0;
        for (int i = 0; i < rights; i++)
        {
            for (int j = 0; j < ups; j++)
            {
                for (int k = 0; k < forwards; k++)
                {
                    if (chuncks[count].meshFilters.Count > 0) 
                    {
                        Gizmos.DrawWireCube(bounds.min + (gridSize * k * Vector3.forward) + (gridSize * j * Vector3.up) + (gridSize * i * Vector3.right), (Vector3.one * gridSize));
                    }
                    count += 1;
                }
            }
        }
    }
}