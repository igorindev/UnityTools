﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NineRealmsTools
{
    static public class CustomMenuItens
    {
        //[MenuItem("GameObject/Folder", false, 12)]
        static void CreateFolder()
        {
            GameObject _object = new GameObject("Folder");
            _object.AddComponent<Folders>();
            _object.tag = "EditorOnly";
        }
        [MenuItem("GameObject/Divisor", false, 12)]
        static void CreateDivisior()
        {
            GameObject _object = new GameObject("_________________________________________________________________________________________");
            _object.tag = "EditorOnly";
        }
        [MenuItem("GameObject/Trigger/TriggerBox", false, 0)]
        static void CreateTriggerBox()
        {
            GameObject _object = new GameObject("Trigger Box");
            _object.AddComponent<BoxCollider>();
            _object.AddComponent<TriggerCaller>();
        }
        [MenuItem("GameObject/Trigger/TriggerSphere", false, 0)]
        static void CreateTriggerSphere()
        {
            GameObject _object = new GameObject("Trigger Sphere");
            _object.AddComponent<SphereCollider>();
            _object.AddComponent<TriggerCaller>();
        }
        [MenuItem("GameObject/Trigger/TriggerCapsule", false, 0)]
        static void CreateTriggerCapsule()
        {
            GameObject _object = new GameObject("Trigger Capsule");
            _object.AddComponent<CapsuleCollider>();
            _object.AddComponent<TriggerCaller>();
        }

        [MenuItem("GameObject/UI/Page", false, 0)]
        static void CreatePage()
        {
            GameObject inst = Object.Instantiate(Resources.Load("Page/PageUI", typeof(GameObject))) as GameObject;
            inst.name = "Page";

            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.TryGetComponent(out Canvas c))
                {
                    inst.transform.SetParent(c.transform);
                }
                else
                {
                    if (Object.FindObjectOfType<Canvas>() != null)
                    {
                        inst.transform.SetParent(Object.FindObjectOfType<Canvas>().transform);
                    }
                    else
                    {
                        GameObject _object = new GameObject("Canvas");
                        _object.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                        _object.AddComponent<CanvasScaler>();
                        _object.AddComponent<GraphicRaycaster>();

                        inst.transform.SetParent(_object.transform);
                    }
                }
            }
            else if (Object.FindObjectOfType<Canvas>() != null)
            {
                inst.transform.SetParent(Object.FindObjectOfType<Canvas>().transform);
            }
            else
            {
                GameObject _object = new GameObject("Canvas");
                _object.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                _object.AddComponent<CanvasScaler>();
                _object.AddComponent<GraphicRaycaster>();

                inst.transform.SetParent(_object.transform);
            }

            inst.transform.localPosition = Vector3.zero;
        }

        [MenuItem("GameObject/Custom/Console", false, 0)]
        static void CreateConsole()
        {
            GameObject inst = Object.Instantiate(Resources.Load("Console/DebugConsole", typeof(GameObject))) as GameObject;
            inst.name = "DebugConsole";
        }

        [MenuItem("GameObject/UI/Horizontal List", false, 0)]
        static void CreateHL()
        {
            GameObject inst = Object.Instantiate(Resources.Load("HorizontalList/Horizontal List", typeof(GameObject))) as GameObject;
            inst.name = "Horizontal List";
        }
    }
}
#endif

