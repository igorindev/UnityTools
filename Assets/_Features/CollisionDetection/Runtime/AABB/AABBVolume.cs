using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AABBVolume : MonoBehaviour
{
    public AABBData[] aabbs = new AABBData[0];

    [SerializeField] UnityEvent<Transform> onEnter;
    [SerializeField] UnityEvent<Transform> onExit;

    [SerializeField] List<Transform> targets = new List<Transform>();

    private void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i])
            {
                if (!Inside(targets[i].position))
                {
                    onExit?.Invoke(targets[i]);
                }
                else
                {
                    onEnter?.Invoke(targets[i]);
                }
            }
        }
    }

    public bool Inside(Vector3 point)
    {
        for (int index = 0; index < aabbs.Length; index++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (point[i] >= transform.position[i] + aabbs[index].center[i] + (aabbs[index].size[i] * 0.5f)
                 || point[i] <= transform.position[i] + aabbs[index].center[i] - (aabbs[index].size[i] * 0.5f))
                {
                    break; //True if not inside, search in next
                }

                if (i >= 2)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < aabbs.Length; i++)
        {
            Gizmos.DrawWireCube(transform.position + aabbs[i].center, aabbs[i].size);
        }
    }
}
