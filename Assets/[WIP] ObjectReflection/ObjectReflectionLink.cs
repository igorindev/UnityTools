using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ObjectReflection
{
    [ExecuteInEditMode]
    public class ObjectReflectionLink : MonoBehaviour
    {
        public GameObject reflectionLinked;
        public Transform reflectionPoint;
        List<GameObject> toLink = new List<GameObject>();

        void Update()
        {
            if (Time.frameCount % 10 == 0)
            {
                if (transform.hasChanged)
                {
                    Debug.Log($"Update {gameObject.name} Reflection");
                    transform.hasChanged = false;

                    Reload();
                    return;
                }

                toLink.Clear();
                foreach (Transform t in transform)
                {
                    toLink.Add(t.gameObject);
                }
                ObjectReflection.GetAllChildren(toLink);

                for (int i = 0; i < toLink.Count; i++)
                {
                    GameObject t = toLink[i];
                    if (t.transform.hasChanged)
                    {
                        Debug.Log($"Update {gameObject.name} Reflection");
                        t.transform.hasChanged = false;

                        Reload();
                    }
                }
            }
        }

        [ContextMenu("Reload")]
        void Reload()
        {
            DestroyImmediate(reflectionLinked);
            reflectionLinked = new GameObject("ReflectionLinked");

            toLink.Clear();
            foreach (Transform t in transform)
            {
                toLink.Add(t.gameObject);
            }
            ObjectReflection.GetAllChildren(toLink);

            for (int i = 0; i < toLink.Count; i++)
            {
                Vector3 pos = ObjectReflection.ReflectPosition(toLink[i].transform.position, reflectionPoint.position, reflectionPoint.forward);
                Quaternion rot = ObjectReflection.ReflectRotation(toLink[i].transform.rotation, reflectionPoint.forward);

                Instantiate(toLink[i], pos, rot, reflectionLinked.transform).transform.DestroyImmediateAllChildren();
            }
        }
        [ContextMenu("Clear")]
        void Clear()
        {
            DestroyImmediate(reflectionLinked);
            reflectionLinked = null;
        }
    }
}