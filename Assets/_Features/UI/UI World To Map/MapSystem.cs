using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    public static MapSystem map;

    [Header("Map Data")]
    [SerializeField] Vector2 UIMapSize = new Vector2(400, 400);
    [SerializeField] Vector2 WorldSize = new Vector2(1000, 1000);
    [Space(7)]
    [SerializeField] bool isRealTime;

    [Header("Icons Instances")]
    [SerializeField] Transform iconsParent;
    [SerializeField] Image iconsPrefab;

    private List<Transform> addedEntities = new();
    private List<Transform> iconReferenceToEntities = new();

    private Vector2 worldSize;
    private float mapScale;

    private void Start()
    {
        map = this;

        Setup(WorldSize);
    }

    public void Setup(Vector2 WorldSize)
    {
        WorldSize = worldSize;

        CalculateMapScale();

        UpdateIconsTransform();
    }

    private void Update()
    {
        if (isRealTime)
        {
            UpdateIconsTransform();
        }
    }

    private void UpdateIconsTransform()
    {
        for (int i = 0; i < addedEntities.Count; i++)
        {
            SetIconPosition(addedEntities[i], iconReferenceToEntities[i]);
        }
    }

    public void AddEntity(Transform transform, Sprite icon)
    {
        addedEntities.Add(transform);
        AddIcon(transform, icon);
    }

    public void RemoveEntity()
    {
        int index = addedEntities.IndexOf(transform);
        addedEntities.RemoveAt(index);

        Destroy(iconReferenceToEntities[index].gameObject);
        iconReferenceToEntities.RemoveAt(index);
    }

    private void CalculateMapScale()
    {
        mapScale = Mathf.Abs(WorldSize.x / UIMapSize.x);
    }

    private void SetIconPosition(Transform worldTransform, Transform mapTransform)
    {
        mapTransform.localPosition = new Vector2(worldTransform.position.x, worldTransform.position.z) / mapScale;

        Vector3 newRot = worldTransform.eulerAngles;
        newRot = new Vector3(0, 0, 360 - newRot.y);
        mapTransform.rotation = Quaternion.Euler(newRot);
    }

    private void AddIcon(Transform pos, Sprite icon)
    {
        Image inst = Instantiate(iconsPrefab, iconsParent);
        //inst.sprite = icon;
        iconReferenceToEntities.Add(inst.transform);
    }
}
