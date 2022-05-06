using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class Folders : MonoBehaviour
{
    public bool open;
    public GameObject[] folderItens;

    [MenuItem("GameObject/Folder", false, 12)]
    static void CreateFolder()
    {
        GameObject folderObj = new GameObject("Folder");
        folderObj.AddComponent<Folders>();
        folderObj.tag = "EditorOnly";
    }

    void Reset()
    {  //
       // folderItens = EditorSceneManager.GetActiveScene().GetRootGameObjects();
       // int count = -1;
       // for (int i = 0; i < folderItens.Length; i++)
       // {
       //     folderItens[i].hideFlags = HideFlags.None;
       //     if (folderItens[i].TryGetComponent(out Folders internalFolder))
       //     {
       //         count += 1;
       //     }
       // }
       //
       // name += " (" + count + ") ";
       //
       // if (endFolder == null)
       // {
       //     endFolder = new GameObject("EndFolder (" + count + ") ")
       //     {
       //         tag = "EditorOnly"
       //     };
       // }
       // Selection.activeGameObject = endFolder;
       // EditorApplication.DirtyHierarchyWindowSorting();
       // Selection.activeGameObject = gameObject;
    }

    [ExecuteInEditMode]
    void OnDestroy()
    {
       // if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
       // {
       //     GameObject[] toHide = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
       //
       //     for (int i = 0; i < toHide.Length; i++)
       //     {
       //         toHide[i].hideFlags = HideFlags.None;
       //     }
       //
       //     EditorApplication.DirtyHierarchyWindowSorting();
       //
       //     if (endFolder != null)
       //     {
       //         DestroyImmediate(endFolder);
       //     }
       //
       //     EditorApplication.DirtyHierarchyWindowSorting();
       // }
    }

    void Hide()
    {
        if (transform.parent == null)
        {
            folderItens = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        }
        else
        {
            int childCount = transform.root.childCount;
            folderItens = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                folderItens[i] = transform.root.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < folderItens.Length; i++)
        {
            if (i <= transform.GetSiblingIndex()) { continue; }
            if (folderItens[i] != gameObject)
                folderItens[i].hideFlags = HideFlags.HideInHierarchy;
        }

        EditorApplication.DirtyHierarchyWindowSorting();
        Selection.activeGameObject = gameObject;
    }
    void Show()
    {
        if (transform.parent == null)
        {
            folderItens = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        }
        else
        {
            int childCount = transform.parent.childCount;
            folderItens = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                folderItens[i] = transform.parent.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < folderItens.Length; i++)
        {
            if (i <= transform.GetSiblingIndex()) { continue; }
            if (folderItens[i] != gameObject)
                folderItens[i].hideFlags = HideFlags.None;
        }

        EditorApplication.DirtyHierarchyWindowSorting();
        Selection.activeGameObject = gameObject;
    }

    [ContextMenu("ExpandAll")]
    void ShowAll()
    {
        GameObject[] toHide = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].hideFlags = HideFlags.None;
        }

        EditorApplication.DirtyHierarchyWindowSorting();
    }

    public void ToggleFolder()
    {
        if (open = !open)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}