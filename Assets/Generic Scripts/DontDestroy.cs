using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyThis(gameObject);
    }

    public void DontDestroyThis(GameObject gameObject)
    {
        DontDestroyOnLoad(gameObject);
    }
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
