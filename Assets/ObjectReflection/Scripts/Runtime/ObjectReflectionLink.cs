using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectReflection
{
    [ExecuteInEditMode]
    public class ObjectReflectionLink : MonoBehaviour
    {
#if UNITY_EDITOR
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
                GetAllChildren(toLink);

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
            GetAllChildren(toLink);

            for (int i = 0; i < toLink.Count; i++)
            {
                Vector3 pos = ReflectPosition(toLink[i].transform.position, reflectionPoint.position, reflectionPoint.forward);
                Quaternion rot = ReflectRotation(toLink[i].transform.rotation, reflectionPoint.forward);

                DestroyImmediateAllChildren(Instantiate(toLink[i], pos, rot, reflectionLinked.transform).transform);
            }
        }
        [ContextMenu("Clear")]
        void Clear()
        {
            DestroyImmediate(reflectionLinked);
            reflectionLinked = null;
        }
        public static Vector3 ReflectPosition(Vector3 sourcePosition, Vector3 reflectionPosition, Vector3 reflectionDirection)
        {
            Vector3 reflectionDir = sourcePosition - reflectionPosition;
            return reflectionPosition + Vector3.Reflect(reflectionDir, reflectionDirection);
        }
        public static Quaternion ReflectRotation(Quaternion sourceRotation, Vector3 mirrorNormal)
        {
            return Quaternion.LookRotation(Vector3.Reflect(sourceRotation * Vector3.forward, mirrorNormal), Vector3.Reflect(sourceRotation * Vector3.up, mirrorNormal));
        }
        public static void GetAllChildren(List<GameObject> list)
        {
            foreach (GameObject obj in list.OrderBy(m => m.transform.GetSiblingIndex()).ToArray())
            {
                Traverse(obj, ref list);
            }
        }
        static void Traverse(GameObject obj, ref List<GameObject> t)
        {
            if (!t.Contains(obj))
                t.Add(obj);
            foreach (Transform child in obj.transform)
            {
                Traverse(child.gameObject, ref t);
            }
        }
        public static void DestroyImmediateAllChildren(Transform transform)
        {
            while (transform.childCount > 0)
            {
                UnityEngine.Object.DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
#endif
    }
}