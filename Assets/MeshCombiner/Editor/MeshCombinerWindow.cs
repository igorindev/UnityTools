#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class MeshCombinerWindow : EditorWindow
{
    private const int Mesh16BitBufferVertexLimit = 65535;
    [SerializeField] Transform transformOrigin;

    [SerializeField] int numOfVertices;

    [SerializeField] bool createMultiMaterialMesh = false, combineInactiveChildren = false, deactivateCombinedChildren = true, deactivateCombinedChildrenMeshRenderers = false, generateUVMap = false, destroyCombinedChildren = false;

    [SerializeField] string folderPath = "Meshes/mesh_Combined";

    [SerializeField] string mergeName = "Combined Mesh";

    [SerializeField] MeshFilter[] meshesToCombine = new MeshFilter[0];

    Vector2 scroll;
    Mesh mesh = null;

    public bool CreateMultiMaterialMesh { get { return createMultiMaterialMesh; } set { createMultiMaterialMesh = value; } }
    public bool CombineInactiveChildren { get { return combineInactiveChildren; } set { combineInactiveChildren = value; } }
    public bool DeactivateCombinedChildren
    {
        get { return deactivateCombinedChildren; }
        set
        {
            deactivateCombinedChildren = value;
            CheckDeactivateCombinedChildren();
        }
    }
    public bool DeactivateCombinedChildrenMeshRenderers
    {
        get { return deactivateCombinedChildrenMeshRenderers; }
        set
        {
            deactivateCombinedChildrenMeshRenderers = value;
            CheckDeactivateCombinedChildren();
        }
    }
    public bool GenerateUVMap { get { return generateUVMap; } set { generateUVMap = value; } }
    public bool DestroyCombinedChildren
    {
        get { return destroyCombinedChildren; }
        set
        {
            destroyCombinedChildren = value;
            CheckDestroyCombinedChildren();
        }
    }


    [MenuItem("Tools/Mesh/Mesh Combiner...")]
    static void CreateCombiner()
    {
        var window = GetWindow<MeshCombinerWindow>("Mesh Combiner");
        window.position = new Rect(0, 0, 400, 600);
        window.Show();
    }

    void CheckDeactivateCombinedChildren()
    {
        if (deactivateCombinedChildren || deactivateCombinedChildrenMeshRenderers)
        {
            destroyCombinedChildren = false;
        }
    }
    void CheckDestroyCombinedChildren()
    {
        if (destroyCombinedChildren)
        {
            deactivateCombinedChildren = false;
            deactivateCombinedChildrenMeshRenderers = false;
        }
    }

    /// <summary>
    /// Combine children's Meshes into one Mesh. Set 'showCreatedMeshInfo' to true if want to show info about created Mesh in the console.
    /// </summary>
    public void CombineMeshes(bool showCreatedMeshInfo)
    {
        if (transformOrigin != null)
        {
            SetAllAsChild();
        }
        else
        {
            CreateParent();
        }

        #region Save our parent scale and our Transform and reset it temporarily:
        // When we are unparenting and get parent again then sometimes scale is a little bit different so save scale before unparenting:
        Vector3 oldScaleAsChild = transformOrigin.localScale;

        // If we have parent then his scale will affect to our new combined Mesh scale so unparent us:
        int positionInParentHierarchy = transformOrigin.GetSiblingIndex();
        Transform parent = transformOrigin.parent;
        transformOrigin.parent = null;

        // Thanks to this the new combined Mesh will have same position and scale in the world space like its children:
        Quaternion oldRotation = transformOrigin.rotation;
        Vector3 oldPosition = transformOrigin.position;
        Vector3 oldScale = transformOrigin.localScale;
        transformOrigin.rotation = Quaternion.identity;
        transformOrigin.position = Vector3.zero;
        transformOrigin.localScale = Vector3.one;
        #endregion Save Transform and reset it temporarily.

        #region Combine Meshes into one Mesh:
        if (!createMultiMaterialMesh)
        {
            CombineMeshesWithSingleMaterial(showCreatedMeshInfo);
        }
        else
        {
            CombineMeshesWithMutliMaterial(showCreatedMeshInfo);
        }
        #endregion Combine Meshes into one Mesh.

        #region Set old Transform values:
        // Bring back the Transform values:
        transformOrigin.rotation = oldRotation;
        transformOrigin.position = oldPosition;
        transformOrigin.localScale = oldScale;

        // Get back parent and same hierarchy position:
        transformOrigin.parent = parent;
        transformOrigin.SetSiblingIndex(positionInParentHierarchy);

        // Set back the scale value as child:
        transformOrigin.localScale = oldScaleAsChild;
        #endregion Set old Transform values.

        MoveChilds();

        SaveCombinedMesh();

        transformOrigin = null;
        mesh = null;
    }

    MeshFilter[] GetMeshFiltersToCombine()
    {
        // Get all MeshFilters belongs to this GameObject and its children:
        List<MeshFilter> list = new List<MeshFilter>();

        if (transformOrigin != null)
        {
            list.Add(transformOrigin.GetComponent<MeshFilter>());
        }

        list.AddRange(meshesToCombine);
        MeshFilter[] meshFilters = list.ToArray();

        return meshFilters;
    }

    void SetAllAsChild()
    {
        transformOrigin.gameObject.AddComponent<MeshRenderer>();
        transformOrigin.gameObject.AddComponent<MeshFilter>();

        for (int i = 0; i < meshesToCombine.Length; i++)
        {
            meshesToCombine[i].transform.SetParent(transformOrigin);
        }
    }
    void CreateParent()
    {
        transformOrigin = new GameObject("Merge Parent").transform;
        transformOrigin.gameObject.AddComponent<MeshRenderer>();
        transformOrigin.gameObject.AddComponent<MeshFilter>();
        transformOrigin.position = Vector3.zero;
        transformOrigin.rotation = Quaternion.identity;

        for (int i = 0; i < meshesToCombine.Length; i++)
        {
            meshesToCombine[i].transform.SetParent(transformOrigin);
        }
    }

    void PreviewNumOfVertices()
    {
        MeshFilter[] meshes = meshesToCombine;

        numOfVertices = 0;

        for (int i = 0; i < meshes.Length; i++)
        {
            numOfVertices += meshes[i].sharedMesh.vertexCount;
        }
    }
    void GetSelectedGameObjectsInScene()
    {
        Transform[] group = Selection.GetTransforms(SelectionMode.Unfiltered);
        List<MeshFilter> list = new List<MeshFilter>();
        for (int i = 0; i < group.Length; i++)
        {
            if (group[i].TryGetComponent(out MeshFilter meshFilter))
            {
                list.Add(meshFilter);
            }
        }

        for (int i = 0; i < meshesToCombine.Length; i++)
        {
            int id = i;
            if (list.Contains(meshesToCombine[id]))
            {
                list.Remove(meshesToCombine[id]);
            }
        }

        list.AddRange(meshesToCombine);

        meshesToCombine = list.ToArray();
    }
    void GetSelectedGameObjectsAndChildsInScene()
    {
        Transform[] group = Selection.GetTransforms(SelectionMode.Unfiltered);
        List<MeshFilter> list = new List<MeshFilter>();
        for (int i = 0; i < group.Length; i++)
        {
            MeshFilter[] m = group[i].GetComponentsInChildren<MeshFilter>();
            list.AddRange(m);
        }

        for (int i = 0; i < meshesToCombine.Length; i++)
        {
            int id = i;
            if (list.Contains(meshesToCombine[id]))
            {
                list.Remove(meshesToCombine[id]);
            }
        }

        list.AddRange(meshesToCombine);

        meshesToCombine = list.ToArray();
    }
    void MoveChilds()
    {
        GameObject combined = new GameObject("Merged Objects");
        List<Transform> list = new List<Transform>();

        for (int i = 0; i < transformOrigin.childCount; i++)
        {
            int id = i;
            list.Add(transformOrigin.GetChild(id));
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetParent(combined.transform);
        }

        combined.SetActive(false);
    }

    void CombineMeshesWithSingleMaterial(bool showCreatedMeshInfo)
    {
        // Get all MeshFilters belongs to this GameObject and its children:
        MeshFilter[] meshFilters = GetMeshFiltersToCombine();

        // First MeshFilter belongs to this GameObject so we don't need it:
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length - 1];

        // If it will be over 65535 then use the 32 bit index buffer:
        long verticesLength = 0;

        for (int i = 0; i < meshFilters.Length - 1; i++) // Skip first MeshFilter belongs to this GameObject in this loop.
        {
            combineInstances[i].subMeshIndex = 0;
            combineInstances[i].mesh = meshFilters[i + 1].sharedMesh;
            combineInstances[i].transform = meshFilters[i + 1].transform.localToWorldMatrix;
            verticesLength += combineInstances[i].mesh.vertices.Length;
        }

        // Set Material from child:
        MeshRenderer[] meshRenderers = transformOrigin.GetComponentsInChildren<MeshRenderer>(combineInactiveChildren);
        if (meshRenderers.Length >= 2)
        {
            meshRenderers[0].sharedMaterials = new Material[1];
            meshRenderers[0].sharedMaterial = meshRenderers[1].sharedMaterial;
        }
        else
        {
            meshRenderers[0].sharedMaterials = new Material[0]; // Reset the MeshRenderer's Materials array.
        }

        // Create Mesh from combineInstances:
        Mesh combinedMesh = new Mesh
        {
            name = mergeName
        };


#if UNITY_2017_3_OR_NEWER
        if (verticesLength > Mesh16BitBufferVertexLimit)
        {
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Only works on Unity 2017.3 or higher.
        }

        combinedMesh.CombineMeshes(combineInstances);
        GenerateUV(combinedMesh);
        mesh = transformOrigin.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
        DeactivateCombinedGameObjects(meshFilters);

        if (showCreatedMeshInfo)
        {
            if (verticesLength <= Mesh16BitBufferVertexLimit)
            {
                Debug.Log("<color=#00cc00><b>Mesh \"" + mergeName + "\" was created from " + combineInstances.Length + " children meshes and has " + verticesLength
                    + " vertices.</b></color>");
            }
            else
            {
                Debug.Log("<color=#ff3300><b>Mesh \"" + mergeName + "\" was created from " + combineInstances.Length + " children meshes and has " + verticesLength
                    + " vertices. Some old devices, like Android with Mali-400 GPU, do not support over 65535 vertices.</b></color>");
            }
        }
#else
		if(verticesLength <= Mesh16BitBufferVertexLimit)
		{
			combinedMesh.CombineMeshes(combineInstances);
			GenerateUV(combinedMesh);
			mesh = transformOrigin.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
			DeactivateCombinedGameObjects(meshFilters);

			if(showCreatedMeshInfo)
			{
				Debug.Log("<color=#00cc00><b>Mesh \""+name+"\" was created from "+combineInstances.Length+" children meshes and has "+verticesLength
					+" vertices.</b></color>");
			}
		}
		else if(showCreatedMeshInfo)
		{
			Debug.Log("<color=red><b>The mesh vertex limit is 65535! The created mesh had "+verticesLength+" vertices. Upgrade Unity version to"
				+" 2017.3 or higher to avoid this limit (some old devices, like Android with Mali-400 GPU, do not support over 65535 vertices).</b></color>");
		}
#endif
    }

    void CombineMeshesWithMutliMaterial(bool showCreatedMeshInfo)
    {
        #region Get MeshFilters, MeshRenderers and unique Materials from all children:
        MeshFilter[] meshFilters = GetMeshFiltersToCombine();
        MeshRenderer[] meshRenderers = new MeshRenderer[meshFilters.Length];
        meshRenderers[0] = transformOrigin.GetComponent<MeshRenderer>(); // Our (parent) MeshRenderer.

        List<Material> uniqueMaterialsList = new List<Material>();
        for (int i = 0; i < meshFilters.Length - 1; i++)
        {
            meshRenderers[i + 1] = meshFilters[i + 1].GetComponent<MeshRenderer>();
            if (meshRenderers[i + 1] != null)
            {
                Material[] materials = meshRenderers[i + 1].sharedMaterials; // Get all Materials from child Mesh.
                for (int j = 0; j < materials.Length; j++)
                {
                    if (!uniqueMaterialsList.Contains(materials[j])) // If Material doesn't exists in the list then add it.
                    {
                        uniqueMaterialsList.Add(materials[j]);
                    }
                }
            }
        }
        #endregion Get MeshFilters, MeshRenderers and unique Materials from all children.

        #region Combine children Meshes with the same Material to create submeshes for final Mesh:
        List<CombineInstance> finalMeshCombineInstancesList = new List<CombineInstance>();

        // If it will be over 65535 then use the 32 bit index buffer:
        long verticesLength = 0;

        for (int i = 0; i < uniqueMaterialsList.Count; i++) // Create each Mesh (submesh) from Meshes with the same Material.
        {
            List<CombineInstance> submeshCombineInstancesList = new List<CombineInstance>();

            for (int j = 0; j < meshFilters.Length - 1; j++) // Get only childeren Meshes (skip our Mesh).
            {
                if (meshRenderers[j + 1] != null)
                {
                    Material[] submeshMaterials = meshRenderers[j + 1].sharedMaterials; // Get all Materials from child Mesh.

                    for (int k = 0; k < submeshMaterials.Length; k++)
                    {
                        // If Materials are equal, combine Mesh from this child:
                        if (uniqueMaterialsList[i] == submeshMaterials[k])
                        {
                            CombineInstance combineInstance = new CombineInstance();
                            combineInstance.subMeshIndex = k; // Mesh may consist of smaller parts - submeshes.
                                                              // Every part have different index. If there are 3 submeshes
                                                              // in Mesh then MeshRender needs 3 Materials to render them.
                            combineInstance.mesh = meshFilters[j + 1].sharedMesh;
                            combineInstance.transform = meshFilters[j + 1].transform.localToWorldMatrix;
                            submeshCombineInstancesList.Add(combineInstance);
                            verticesLength += combineInstance.mesh.vertices.Length;
                        }
                    }
                }
            }

            // Create new Mesh (submesh) from Meshes with the same Material:
            Mesh submesh = new Mesh();

#if UNITY_2017_3_OR_NEWER
            if (verticesLength > Mesh16BitBufferVertexLimit)
            {
                submesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Only works on Unity 2017.3 or higher.
            }

            submesh.CombineMeshes(submeshCombineInstancesList.ToArray(), true);
#else
			// Below Unity 2017.3 if vertices count is above the limit then an error appears in the console when we use the below method.
			// Anyway we don't stop the algorithm here beacuse we want to count the entire number of vertices in the children meshes:
			if(verticesLength <= Mesh16BitBufferVertexLimit)
			{
				submesh.CombineMeshes(submeshCombineInstancesList.ToArray(), true);
			}
#endif

            CombineInstance finalCombineInstance = new CombineInstance();
            finalCombineInstance.subMeshIndex = 0;
            finalCombineInstance.mesh = submesh;
            finalCombineInstance.transform = Matrix4x4.identity;
            finalMeshCombineInstancesList.Add(finalCombineInstance);
        }
        #endregion Combine submeshes (children Meshes) with the same Material.

        #region Set Materials array & combine submeshes into one multimaterial Mesh:
        meshRenderers[0].sharedMaterials = uniqueMaterialsList.ToArray();

        Mesh combinedMesh = new Mesh
        {
            name = mergeName
        };

#if UNITY_2017_3_OR_NEWER
        if (verticesLength > Mesh16BitBufferVertexLimit)
        {
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Only works on Unity 2017.3 or higher.
        }

        combinedMesh.CombineMeshes(finalMeshCombineInstancesList.ToArray(), false);
        GenerateUV(combinedMesh);
        mesh = transformOrigin.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
        DeactivateCombinedGameObjects(meshFilters);

        if (showCreatedMeshInfo)
        {
            if (verticesLength <= Mesh16BitBufferVertexLimit)
            {
                Debug.Log("<color=#00cc00><b>Mesh \"" + mergeName + "\" was created from " + (meshFilters.Length - 1) + " children meshes and has "
                    + finalMeshCombineInstancesList.Count + " submeshes, and " + verticesLength + " vertices.</b></color>");
            }
            else
            {
                Debug.Log("<color=#ff3300><b>Mesh \"" + mergeName + "\" was created from " + (meshFilters.Length - 1) + " children meshes and has "
                    + finalMeshCombineInstancesList.Count + " submeshes, and " + verticesLength
                    + " vertices. Some old devices, like Android with Mali-400 GPU, do not support over 65535 vertices.</b></color>");
            }
        }
#else
		if(verticesLength <= Mesh16BitBufferVertexLimit)
		{
			combinedMesh.CombineMeshes(finalMeshCombineInstancesList.ToArray(), false);
			GenerateUV(combinedMesh);
			meshFilters[0].sharedMesh = combinedMesh;
			DeactivateCombinedGameObjects(meshFilters);

			if(showCreatedMeshInfo)
			{
				Debug.Log("<color=#00cc00><b>Mesh \""+mergeName+"\" was created from "+(meshFilters.Length-1)+" children meshes and has "
					+finalMeshCombineInstancesList.Count+" submeshes, and "+verticesLength+" vertices.</b></color>");
			}
		}
		else if(showCreatedMeshInfo)
		{
			Debug.Log("<color=red><b>The mesh vertex limit is 65535! The created mesh had "+verticesLength+" vertices. Upgrade Unity version to"
				+" 2017.3 or higher to avoid this limit (some old devices, like Android with Mali-400 GPU, do not support over 65535 vertices).</b></color>");
		}
#endif
        #endregion Set Materials array & combine submeshes into one multimaterial Mesh.
    }

    void DeactivateCombinedGameObjects(MeshFilter[] meshFilters)
    {
        for (int i = 0; i < meshFilters.Length - 1; i++) // Skip first MeshFilter belongs to this GameObject in this loop.
        {
            if (!destroyCombinedChildren)
            {
                if (deactivateCombinedChildren)
                {
                    meshFilters[i + 1].gameObject.SetActive(false);
                }
                if (deactivateCombinedChildrenMeshRenderers)
                {
                    MeshRenderer meshRenderer = meshFilters[i + 1].gameObject.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = false;
                    }
                }
            }
            else
            {
                DestroyImmediate(meshFilters[i + 1].gameObject);
            }
        }
    }

    void GenerateUV(Mesh combinedMesh)
    {
#if UNITY_EDITOR
        if (generateUVMap)
        {
            UnityEditor.UnwrapParam unwrapParam = new UnityEditor.UnwrapParam();
            UnityEditor.UnwrapParam.SetDefaults(out unwrapParam);
            UnityEditor.Unwrapping.GenerateSecondaryUVSet(combinedMesh, unwrapParam);
        }
#endif
    }

    void OnValidate()
    {
        PreviewNumOfVertices();
    }

    void OnGUI()
    {
        MeshCombinerWindow meshCombiner = this;

        // Create style wherein text color will be red if folder path is not valid:
        GUIStyle style = new GUIStyle(EditorStyles.textField);
        bool isValidPath = IsValidPath(meshCombiner.folderPath);
        if (!isValidPath)
        {
            style.normal.textColor = Color.red;
            style.focused.textColor = Color.red;
        }

        GUILayout.Label("");
        GUILayout.Label(new GUIContent("Transform Origin: ", "The origin, if is null a new is created in world center"));

        meshCombiner.transformOrigin = EditorGUILayout.ObjectField(meshCombiner.transformOrigin, typeof(Transform), true) as Transform;

        #region Path to the folder where combined Meshes will be saved:
        // Create Labels:
        GUILayout.Label("");
        GUILayout.Label(new GUIContent("Combined Mesh Name:", "The name of the combined mesh."));

        // Create TextField with custom style:
        meshCombiner.folderPath = EditorGUILayout.TextField(meshCombiner.mergeName, style);
        #endregion Path to the folder where combined Meshes will be saved:

        GUILayout.Space(15);

        // The last (6) "Destroy Combined Children" Toggle:
        style = new GUIStyle(EditorStyles.toggle);
        if (meshCombiner.DestroyCombinedChildren)
        {
            style.onNormal.textColor = new Color(1, 0.15f, 0);
        }

        #region Button which combine Meshes into one Mesh & Toggles with combine options:
        // Toggles:
        meshCombiner.CreateMultiMaterialMesh = GUILayout.Toggle(meshCombiner.CreateMultiMaterialMesh, "Create Multi-Material Mesh");
        meshCombiner.CombineInactiveChildren = GUILayout.Toggle(meshCombiner.CombineInactiveChildren, "Combine Inactive Children");

        meshCombiner.DeactivateCombinedChildren = GUILayout.Toggle(meshCombiner.DeactivateCombinedChildren, "Deactivate Combined Children");
        meshCombiner.DeactivateCombinedChildrenMeshRenderers = GUILayout.Toggle(meshCombiner.DeactivateCombinedChildrenMeshRenderers, "Deactivate Combined Children's MeshRenderers");

        meshCombiner.GenerateUVMap = GUILayout.Toggle(meshCombiner.GenerateUVMap, new GUIContent("Generate UV Map", "It is a slow operation that " + "generates a UV map (required for the lightmap).\n\nCan be used only in the Editor."));

        meshCombiner.DestroyCombinedChildren = GUILayout.Toggle(meshCombiner.DestroyCombinedChildren,
        new GUIContent("Destroy Combined Children", "In the editor this operation can NOT be undone!\n\n" +
        "If you want to bring back destroyed GameObjects, you have to load again the scene without saving."), style);
        #endregion Button which combine Meshes into one Mesh & Toggles with combine options.

        #region Path to the folder where combined Meshes will be saved:
        // Create Labels:
        GUILayout.Label("");
        GUILayout.Label(new GUIContent("Folder path:", "Folder path to save combined Mesh."));

        // Create style wherein text color will be red if folder path is not valid:
        style = new GUIStyle(EditorStyles.textField);
        isValidPath = IsValidPath(meshCombiner.folderPath);
        if (!isValidPath)
        {
            style.normal.textColor = Color.red;
            style.focused.textColor = Color.red;
        }

        // Create TextField with custom style:
        meshCombiner.folderPath = EditorGUILayout.TextField(meshCombiner.folderPath, style);
        #endregion Path to the folder where combined Meshes will be saved.

        GUILayout.Space(15);

        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        if (GUILayout.Button("Add Selected", GUILayout.Height(30)))
        {
            GetSelectedGameObjectsInScene();
            PreviewNumOfVertices();
        }
        if (GUILayout.Button("Add Child Selected List", GUILayout.Height(30)))
        {
            GetSelectedGameObjectsAndChildsInScene();
            PreviewNumOfVertices();
        }
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear List", GUILayout.Height(30)))
        {
            meshesToCombine = new MeshFilter[0];
            PreviewNumOfVertices();
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Combine Meshes", GUILayout.Height(25)))
        {
            meshCombiner.CombineMeshes(true);
        }

        style = new GUIStyle(EditorStyles.helpBox)
        {
            fontSize = 16
        };

        #region Total of Vertices:
        GUILayout.Space(5);
        GUILayout.BeginHorizontal(style, GUILayout.Height(20));
        GUILayout.Label("Merge Total of Vertices:", style);
        GUI.contentColor = Color.green;
        GUILayout.Label(numOfVertices.ToString(), style);
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();
        #endregion Total of Vertices.
        
        GUILayout.Space(7);

        #region Meshes To Combine array:
        scroll = EditorGUILayout.BeginScrollView(scroll, false, true, GUILayout.Height(Screen.height - 450));
        EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width - 30));

        ScriptableObject target = this;
        SerializedObject s = new SerializedObject(target);
        SerializedProperty property = s.FindProperty("meshesToCombine");

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(property, true);
        if (EditorGUI.EndChangeCheck())
        {
            s.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        #endregion Meshes To Combine array.

        GUILayout.Space(10);
    }

    bool IsValidPath(string folderPath)
    {
        string pattern = "[:*?\"<>|]"; // Prohibited characters.
        Regex regex = new Regex(pattern);
        return (!regex.IsMatch(folderPath));
    }
    string SaveCombinedMesh()
    {
        bool meshIsSaved = AssetDatabase.Contains(mesh); // If is saved then only show it in the project view.

        #region Create directories if Mesh and path doesn't exists:
        folderPath = folderPath.Replace('\\', '/');
        if (!meshIsSaved && !AssetDatabase.IsValidFolder("Assets/" + folderPath))
        {
            string[] folderNames = folderPath.Split('/');
            folderNames = folderNames.Where((folderName) => !folderName.Equals("")).ToArray();
            folderNames = folderNames.Where((folderName) => !folderName.Equals(" ")).ToArray();

            folderPath = "/"; // Reset folder path.
            for (int i = 0; i < folderNames.Length; i++)
            {
                folderNames[i] = folderNames[i].Trim();
                if (!AssetDatabase.IsValidFolder("Assets" + folderPath + folderNames[i]))
                {
                    string folderPathWithoutSlash = folderPath.Substring(0, folderPath.Length - 1); // Delete last "/" character.
                    AssetDatabase.CreateFolder("Assets" + folderPathWithoutSlash, folderNames[i]);
                }
                folderPath += folderNames[i] + "/";
            }
            folderPath = folderPath.Substring(1, folderPath.Length - 2); // Delete first and last "/" character.
        }
        #endregion Create directories if Mesh and path doesn't exists.

        #region Save Mesh:
        if (!meshIsSaved)
        {
            string meshPath = "Assets/" + folderPath + "/" + mesh.name + ".asset";
            int assetNumber = 1;
            while (AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) != null) // If Mesh with same name exists, change name.
            {
                meshPath = "Assets/" + folderPath + "/" + mesh.name + " (" + assetNumber + ").asset";
                assetNumber++;
            }

            AssetDatabase.CreateAsset(mesh, meshPath);
            AssetDatabase.SaveAssets();
            Debug.Log("<color=#ff9900><b>Mesh \"" + mesh.name + "\" was saved in the \"" + folderPath + "\" folder.</b></color>"); // Show info about saved mesh.
        }
        #endregion Save Mesh.

        EditorGUIUtility.PingObject(mesh); // Show Mesh in the project view.
        return folderPath;
    }
}
#endif