using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpatialHash;
using static UnityEditor.ShaderData;

public class TestSpatialHash : MonoBehaviour
{
    SpatialHash grid;

    [SerializeField] int gridSize = 1;
    [SerializeField] Transform target;
    [SerializeField] Transform[] Transform = new Transform[0];
    ArrayList results = new ArrayList();

    List<HashObject> hashs = new();

    [ContextMenu("Execute")]
    private void Start()
    {
        hashs = new List<HashObject>();
        results = new ArrayList();
        grid = new SpatialHash(gridSize);

        foreach (var item in Transform)
        {
            var i = new HashObject(item);
            hashs.Add(i);
            grid.Insert(i.insertPos, i);
        }

        results = grid.Query(target.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdatePos();
            results = grid.Query(target.position);
        }
    }

    [ContextMenu(nameof(UpdatePos))]
    void UpdatePos()
    {
        foreach (var item in hashs)
        {
            grid.UpdatePosition(item);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var item in hashs)
        {
            Gizmos.DrawSphere(item.insertPos, 0.3f);
        }

        Gizmos.color = Color.green;
        foreach (HashObject item in results)
        {
            Debug.Log(item.currentTransform.name, item.currentTransform.gameObject);
            Gizmos.DrawSphere(item.insertPos, 0.4f);
        }
    }
}
