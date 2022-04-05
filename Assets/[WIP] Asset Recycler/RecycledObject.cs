using UnityEngine;

public class RecycledObject : MonoBehaviour
{
    [SerializeField] GameObject recycleObj = null;
    [SerializeField] string key;

    [ContextMenu("aa")]
    void Start()
    {
        if (recycleObj != null)
        {
            Debug.Log(name + " | " + recycleObj);
            Recycler.AddToRecycler(key, recycleObj);
            recycleObj = null;
            DontDestroyOnLoad(recycleObj);
        }
        else
        {
            Debug.Log("Exists" + " | " + recycleObj);
            GameObject recycled = Recycler.Recycle(key);
            recycled.transform.SetPositionAndRotation(transform.position, transform.localRotation);
        }
    }
}