using UnityEngine;

public class OBBCollisionTest : MonoBehaviour
{
    [SerializeField] OBB obbA;
    [SerializeField] OBB obbB;

    void Update()
    {
        bool isIntersects = obbA.Intersects(obbB);
        if (isIntersects)
        {
            obbA.gizmosColor = Color.red;
            obbB.gizmosColor = Color.red;
        }
        else
        {
            obbA.gizmosColor = Color.white;
            obbB.gizmosColor = Color.white;
        }
    }
}
