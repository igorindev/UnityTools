using UnityEngine;

[CreateAssetMenu(menuName = "Variable/List/Game Object")]
public class GameObjectListVariable : ListVariable<GameObject>
{
    public GameObject Instance
    {
        get { return value.Count > 0 ? value[0] : null; }
    }

    public void InstantiateAll(Transform transform)
    {
        foreach (var item in value)
        {
            Instantiate(item, transform);
        }
    }

    public void DestroyAll()
    {
        foreach (var item in value)
        {
            onListValueChange?.Invoke(item);
            Destroy(item);
        }
        value.Clear();
    }
}