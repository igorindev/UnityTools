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
        }
        else
        {
            Debug.Log("Exists" + " | " + recycleObj);
            Transform recycled = Recycler.Recycle(key).transform;
            if (ReferenceEquals(recycled, null) == false)
            {
                recycled.SetParent(transform, false);
            }
        }
    }
}