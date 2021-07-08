using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapSystem : MonoBehaviour
{
    public static MapSystem instance;
    public bool realTime;
    public GameObject player;
    public Transform icon;
    [Space(5)]
    public List<Transform> referencePoints;

    public Vector2 mapSize;

    public Transform iconsInstances;
    public Image defaultImage;

    Vector2 worldSize;
    Vector2 playerMapLocation;
    float division;

    [Header("")]
    public MapType usingMap = MapType.terrain;
    [HideInInspector] public Terrain terrain;
    [HideInInspector] public Transform planeOrQuad;
    [HideInInspector] public Vector2 size;

    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Já há um MapSystem declarado");
            return;
        }
        instance = this;
    }

    public enum MapType
    {
        terrain,
        plane,
        quad,
        size
    }

    private void Update()
    {
        if (realTime)
        {
            LoadMap();
        }
    }

    private void OnEnable()
    {
        LoadMap();
    }

    public void LoadMap()
    {
        if (usingMap == MapType.terrain)
        {
            worldSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        }
        else if (usingMap == MapType.plane)
        {
            worldSize = new Vector2(planeOrQuad.localScale.x, planeOrQuad.localScale.z) * 10;
        }
        else if (usingMap == MapType.quad)
        {
            worldSize = new Vector2(planeOrQuad.localScale.x, planeOrQuad.localScale.z);
        }
        else if (usingMap == MapType.size)
        {
            worldSize = new Vector2(size.x, size.y);
        }

        division = worldSize.x / mapSize.x;
        if (division < 0) { division *= -1; }
        playerMapLocation = new Vector2(player.transform.position.x, player.transform.position.z) / division;

        icon.localPosition = playerMapLocation;

        Vector3 newRot = player.transform.eulerAngles;
        newRot.x = 0;
        newRot.z = 360 - newRot.y;
        newRot.y = 0;
        icon.transform.rotation = Quaternion.Euler(newRot);
    }

    void MapPoint(Transform pos, Transform icon)
    {
        division = worldSize.x / mapSize.x;
        if (division < 0) { division *= -1; }
        Vector2 mapLocation = new Vector2(pos.position.x, pos.position.z) / division;

        icon.localPosition = mapLocation;
    }

    public void SetMapIcon(Transform pos, Sprite icon)
    {
        Image inst = Instantiate(defaultImage, iconsInstances);
        inst.sprite = icon;
        MapPoint(pos, inst.transform);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapSystem))]
public class MyScriptEditor : Editor
{
    override public void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Choose the method to calculate the size of the world. Depending on the chosen one, the calculation changes, which makes the right choice necessary.", MessageType.Info);
        DrawDefaultInspector();

        var myScript = target as MapSystem;

        if (myScript.usingMap == MapSystem.MapType.terrain)
            myScript.terrain = EditorGUILayout.ObjectField("Terrain", myScript.terrain, typeof(Terrain), true) as Terrain;

        if (myScript.usingMap == MapSystem.MapType.plane)
            myScript.planeOrQuad = EditorGUILayout.ObjectField("Plane", myScript.planeOrQuad, typeof(Transform), true) as Transform;

        if (myScript.usingMap == MapSystem.MapType.quad)
            myScript.planeOrQuad = EditorGUILayout.ObjectField("Quad", myScript.planeOrQuad, typeof(Transform), true) as Transform;

        if (myScript.usingMap == MapSystem.MapType.size)
            myScript.size = EditorGUILayout.Vector2Field("Size", myScript.size);
    }
}
#endif
