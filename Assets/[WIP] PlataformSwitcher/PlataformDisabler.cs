using UnityEngine;

public class PlataformDisabler : MonoBehaviour
{
    [SerializeField] RuntimePlatform[] platforms;

    void OnEnable()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] == Application.platform)
            {
                gameObject.SetActive(false);
                break;
            }
        }
    }
}