using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ObjectReflection
{
    public class ObjectReflection : EditorWindow
    {
        [SerializeField] Transform reflectionPoint;

        SerializedObject so;
        SerializedProperty reflectionPointProperty;

        [MenuItem("Tools/Object Reflection")]
        static void Init()
        {
            GetWindow<ObjectReflection>("Object Reflection");
        }
        void OnEnable()
        {
            so = new SerializedObject(this);

            reflectionPointProperty = so.FindProperty(nameof(reflectionPoint));
        }
        void OnGUI()
        {
            so.Update();
            EditorGUILayout.PropertyField(reflectionPointProperty);

            if (GUILayout.Button("Reflect"))
            {
                List<GameObject> toReflect = new List<GameObject>();

                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    if (Selection.gameObjects[i].scene.IsValid())
                    {
                        toReflect.Add(Selection.gameObjects[i]);
                    }
                }
                GetAllChildren(toReflect);

                if (toReflect.Count > 0)
                    Reflect(toReflect.ToArray());
            }

            if (GUILayout.Button("Create Reflection Link"))
            {
                List<GameObject> toLink = new List<GameObject>();
                List<GameObject> parentLink = new List<GameObject>();

                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    if (Selection.gameObjects[i].scene.IsValid())
                    {
                        toLink.Add(Selection.gameObjects[i]);
                        parentLink.Add(Selection.gameObjects[i]);
                    }
                }

                GetAllChildren(toLink);

                if (toLink.Count > 0)
                    Link(toLink.ToArray(), parentLink.ToArray());

            }

            so.ApplyModifiedProperties();
        }

        [ContextMenu("Reflect")]
        void Reflect(GameObject[] toReflect)
        {
            GameObject reflection = new GameObject("Reflection");

            for (int i = 0; i < toReflect.Length; i++)
            {
                Vector3 pos = ReflectPosition(toReflect[i].transform.position, reflectionPoint.position, reflectionPoint.forward);
                Quaternion rot = ReflectRotation(toReflect[i].transform.rotation, reflectionPoint.forward);

                Instantiate(toReflect[i], pos, rot, reflection.transform).transform.DestroyImmediateAllChildren();
            }
        }

        void Link(GameObject[] toLink, GameObject[] parentLink)
        {
            if (reflectionPoint == null)
            {
                Debug.Log("Reflection Point was not assigned");
                return;
            }

            GameObject link = new GameObject("ReflectionLink");
            GameObject reflectionLinked = new GameObject("ReflectionLinked");

            ObjectReflectionLink objectReflectionLink = link.AddComponent<ObjectReflectionLink>();
            objectReflectionLink.reflectionLinked = reflectionLinked;
            objectReflectionLink.reflectionPoint = reflectionPoint;

            foreach (var item in parentLink)
            {
                item.transform.SetParent(link.transform);
            }

            for (int i = 0; i < toLink.Length; i++)
            {
                Vector3 pos = ReflectPosition(toLink[i].transform.position, reflectionPoint.position, reflectionPoint.forward);
                Quaternion rot = ReflectRotation(toLink[i].transform.rotation, reflectionPoint.forward);

                Instantiate(toLink[i], pos, rot, reflectionLinked.transform).transform.DestroyImmediateAllChildren();
            }
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
            t.AddUnique(obj);
            foreach (Transform child in obj.transform)
            {
                Traverse(child.gameObject, ref t);
            }
        }
    }
}