using UnityEngine;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    [Header("Map Data")]
    [SerializeField] Vector2 UIMapSize = new Vector2(400, 400);
    [SerializeField] Vector2 WorldSize = new Vector2(1000, 1000);
    [Space(7)]
    [SerializeField] bool isRealTime;

    [Header("Characters")]
    [SerializeField] Transform[] charactersReference;
    [SerializeField] Transform[] charactersUIIcon;

    [Header("Icons Instances")]
    [SerializeField] Transform iconsParent;
    [SerializeField] Image iconsPrefab;

    Vector2 worldSize;
    float division;

    void Awake()
    {
        CalculateMapScale();

        for (int i = 0; i < charactersReference.Length; i++)
        {
            CalculateCharsPosInMap(charactersReference[i], charactersUIIcon[i]);
        }
    }
    void Update()
    {
        if (isRealTime)
        {
            for (int i = 0; i < charactersReference.Length; i++)
            {
                CalculateCharsPosInMap(charactersReference[i], charactersUIIcon[i]);
            }
        }
    }

    void OnEnable()
    {
        if (isRealTime)
        {
            return;
        }

        for (int i = 0; i < charactersReference.Length; i++)
        {
            CalculateCharsPosInMap(charactersReference[i], charactersUIIcon[i]);
        }
    }

    public void CalculateMapScale()
    {
        division = Mathf.Abs(WorldSize.x / UIMapSize.x);
    }

    public void CalculateCharsPosInMap(Transform characterRef, Transform mapIcon)
    {
        mapIcon.localPosition = new Vector2(characterRef.position.x, characterRef.position.z) / division;

        //Handle rotation
        Vector3 newRot = characterRef.eulerAngles;
        newRot.x = 0;
        newRot.z = 360 - newRot.y;
        newRot.y = 0;
        mapIcon.rotation = Quaternion.Euler(newRot);
    }

    public void SetIconInMap(Transform pos, Sprite icon)
    {
        Image inst = Instantiate(iconsPrefab, iconsParent);
        inst.sprite = icon;
        MapPoint(pos, inst.transform);
    }
    void MapPoint(Transform pos, Transform icon)
    {
        division = worldSize.x / UIMapSize.x;
        if (division < 0) { division *= -1; }
        Vector2 mapLocation = new Vector2(pos.position.x, pos.position.z) / division;

        icon.localPosition = mapLocation;
    }
}
