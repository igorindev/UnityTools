using UnityEngine;
using UnityEditor;

public class PlataformActiveSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct GOPlataform
    {
        public GameObject gameObject;
        public BuildTarget[] platforms;
    }

    [SerializeField] GOPlataform[] gameObjectsOnPlataform;

    public void Switch(BuildTarget target)
    {
        transform.DestroyImmediateAllChildren();

        for (int i = 0; i < gameObjectsOnPlataform.Length; i++)
        {
            for (int j = 0; j < gameObjectsOnPlataform[i].platforms.Length; j++)
            {
                if (gameObjectsOnPlataform[i].platforms[j] == target)
                {
                    Instantiate(gameObjectsOnPlataform[i].gameObject, transform);
                    return;
                }
            }
        }
    }
}
