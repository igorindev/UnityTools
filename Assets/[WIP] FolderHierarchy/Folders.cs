#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Folders : MonoBehaviour
{
    CommentHierarchyIcon icon;
    public bool hide;
    public GameObject endFolder;
    public void Validate()
    {
        if (hide)
        {
            Hide();
        }
    }

    private void Reset()
    {
        icon = new CommentHierarchyIcon();
        GameObject[] toHide = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        int count = -1;
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].hideFlags = HideFlags.None;
            if (toHide[i].TryGetComponent(out Folders internalFolder))
            {
                count += 1;
            }
        }

        name += " (" + count + ") "; 

        if (endFolder == null)
        {
            endFolder = new GameObject("EndFolder (" + count + ") ")
            {
                tag = "EditorOnly"
            };
        }
        Selection.activeGameObject = endFolder;
        EditorApplication.DirtyHierarchyWindowSorting();
        Selection.activeGameObject = gameObject;
    }
    private void OnValidate()
    {
        Validation();
    }

    void Validation()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
        {
            icon = new CommentHierarchyIcon();
            if (hide)
            {
                Hide();
                icon.ChangeIcon(false);
            }
            else
            {
                Show();
                icon.ChangeIcon(true);
            }

            if (endFolder == null)
            {
                Reset();
            }
        }
    }

    [ExecuteInEditMode]
    private void OnDestroy()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
        {
            GameObject[] toHide = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < toHide.Length; i++)
            {
                toHide[i].hideFlags = HideFlags.None;
            }

            EditorApplication.DirtyHierarchyWindowSorting();

            if (endFolder != null)
            {
                DestroyImmediate(endFolder);
            }

            EditorApplication.DirtyHierarchyWindowSorting();
        }
    }

    private void Hide()
    {
        GameObject[] toHide = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        bool isFolder = false;
        for (int i = 0; i < toHide.Length; i++)
        {
            if (toHide[i].GetInstanceID() == Selection.activeGameObject.GetInstanceID())
            {
               string newName = "";
              
               for (int j = 0; j < toHide[i].name.Length; j++)
               {
                   if (toHide[i].name[j] == '-')
                   {
                       break;
                   }
                   else
                   {
                       newName += toHide[i].name[j];
                   }
               }
               newName += "- Closed";
               toHide[i].name = newName;
                isFolder = true;
                continue;
            }
            if (toHide[i].GetInstanceID() == endFolder.GetInstanceID() && isFolder)
            {
                toHide[i].hideFlags = HideFlags.HideInHierarchy;
                break;
            }

            if (isFolder)
            {
                toHide[i].hideFlags = HideFlags.HideInHierarchy;
            }
        }
        EditorApplication.DirtyHierarchyWindowSorting();
    }
    private void Show()
    {
        GameObject[] toHide = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        List<Folders> list = new List<Folders>();
        GameObject selected = Selection.activeGameObject;

        string newName = "";

        for (int i = 0; i < selected.name.Length; i++)
        {
            if (selected.name[i] == '-')
            {
                break;
            }
            else
            {
                newName += selected.name[i];
            }
        }
        newName += "- Open";
        selected.name = newName;
       
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].hideFlags = HideFlags.None;
            if (toHide[i].TryGetComponent(out Folders internalFolder))
            {
                list.Add(internalFolder);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            Selection.activeGameObject = list[i].gameObject;
            list[i].Validate();
        }

        Selection.activeGameObject = selected;
        EditorApplication.DirtyHierarchyWindowSorting();
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
}
#endif