using UnityEngine;

public class SelfDelete : MonoBehaviour
{
    public float timeToDestroy = 3f;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}