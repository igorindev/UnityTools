using UnityEngine;

public partial class RadialMenu : MonoBehaviour
{
    [SerializeField] RingPiece ringPiecePrefab;
    [SerializeField] int numOfPieces;
    [SerializeField] float radius = 100;

    RingPiece[] ringPieces;
    float degreesPerPiece;

    void Start()
    {
        degreesPerPiece = 360f / numOfPieces;
        ringPieces = new RingPiece[numOfPieces];

        for (int i = 0; i < numOfPieces; i++)
        {
            ringPieces[i] = Instantiate(ringPiecePrefab, PlaceVectorsInCircle(i, transform.position, degreesPerPiece, radius), Quaternion.identity, transform);
        }
    }

    void Update()
    {
        int activeElement = GetActiveElement();
        HighlightActiveElement(activeElement);
    }

    Vector3 PlaceVectorsInCircle(int i, Vector3 center, float angleIncrement, float radius)
    {
        float radians = i * Mathf.Deg2Rad * angleIncrement;
        Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians));
        return center + offset * radius;
    }

    int GetActiveElement()
    {
        Vector2 normalizedMousePosition = new Vector2(Input.mousePosition.x - transform.position.x, Input.mousePosition.y - transform.position.y);
        float currentAngle = Mathf.Atan2(normalizedMousePosition.y, normalizedMousePosition.x) * Mathf.Rad2Deg + degreesPerPiece * 0.5f;
        currentAngle = (currentAngle + 360) % 360;
        int result = (int)(currentAngle / degreesPerPiece);

        return result;
    }

    void HighlightActiveElement(int activeElement)
    {
        for (int i = 0; i < ringPieces.Length; i++)
        {
            if (i == activeElement)
            {
                ringPieces[i].icon.color = Color.black;
            }
            else
            {
                ringPieces[i].icon.color = Color.white;
            }
        }
    }
}