using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Plataform/MeshSwitcher")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlataformMeshSwitcher : MonoBehaviour
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
