﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Linq;
using UnityEngine.Events;
using System.Reflection;

namespace UnityEventReferenceViewer
{
    public class UnityEventReferenceViewerWindow : EditorWindow
    {
        readonly int mainSpacing = 20;
        readonly int tabulation = 30;
        readonly float leftColoumnRelativeWidth = 0.6f;

        static List<EventReferenceInfo> dependencies;
        string searchString = "";

        Rect drawableRect;

        Vector2 scroll = Vector2.zero;

        [MenuItem("Tools/UnityEvent Reference Viewer")]
        public static void OpenWindow()
        {
            UnityEventReferenceViewerWindow window = CreateWindow<UnityEventReferenceViewerWindow>();
            window.titleContent = new GUIContent("UnityEvent Reference Viewer");
            window.minSize = new Vector2(250, 100);
        }

        [DidReloadScripts]
        static void RefreshDependencies()
        {
            FindDependencies();
        }

        void OnFocus()
        {
            RefreshDependencies();
        }

        void OnEnable()
        {
            FindDependencies();
        }

        void OnDisable()
        {
            dependencies = null;
        }

        void OnGUI()
        {
            DrawWindow();
        }

        void DrawWindow()
        {
            var drawnVertically = 0;
            drawableRect = GetDrawableRect();

            EditorGUI.LabelField(new Rect(drawableRect.position, new Vector2(100f, 16f)), "Search method");
            var newString = EditorGUI.TextField(new Rect(drawableRect.position + Vector2.right * 100f, new Vector2(drawableRect.width - 100, 16f)), searchString);

            if (newString != searchString)
            {
                searchString = newString;

                if (searchString != "")
                {
                    FindDependencies(searchString);
                }
                else
                {
                    FindDependencies();
                }
            }

            if (dependencies != null)
            {
                GUILayout.Space(80);
                scroll = GUILayout.BeginScrollView(scroll, false, false);

                int i = 0;
                foreach (var d in dependencies)
                {
                    var verticalSpace = (d.Listeners.Count + 1) * 16 + mainSpacing;
                    var position = new Vector2(drawableRect.position.x, 0f) + Vector2.up * drawnVertically;

                    DrawDependencies(d, position);

                    drawnVertically += verticalSpace;
                    i++;
                }

                GUILayout.Space(drawnVertically + 20);

                GUILayout.EndScrollView();
            }
        }

        void DrawDependencies(EventReferenceInfo dependency, Vector2 position)
        {
            float width = drawableRect.width * leftColoumnRelativeWidth;

            EditorGUI.ObjectField(new Rect(position, new Vector2(width - tabulation, 16f)), dependency.Owner, typeof(MonoBehaviour), true);

            for (int i = 0; i < dependency.Listeners.Count; i++)
            {
                Vector2 positionRoot = position + Vector2.up * 16 + Vector2.up * 16 * i;
                EditorGUI.ObjectField(new Rect(positionRoot + Vector2.right * tabulation, new Vector2(width - tabulation, 16f)), dependency.Listeners[i], typeof(MonoBehaviour), true);

                Vector2 labelPosition = new Vector2(drawableRect.width * leftColoumnRelativeWidth + tabulation * 1.5f, positionRoot.y);
                EditorGUI.LabelField(new Rect(labelPosition, new Vector2(drawableRect.width * (1 - leftColoumnRelativeWidth) - tabulation / 1.5f, 16f)), dependency.MethodNames[i]);
            }
        }

        static void FindDependencies(string methodName)
        {
            var depens = UnityEventReferenceFinder.FindAllUnityEventsReferences();
            var onlyWithName = new List<EventReferenceInfo>();

            foreach (var d in depens)
            {
                if (d.MethodNames.Where(m => m.ToLower().Contains(methodName.ToLower())).Count() > 0)
                {
                    var indexes = d.MethodNames.Where(n => n.ToLower().Contains(methodName.ToLower())).Select(n => d.MethodNames.IndexOf(n)).ToArray();

                    var info = new EventReferenceInfo();
                    info.Owner = d.Owner;

                    foreach (var i in indexes)
                    {
                        info.Listeners.Add(d.Listeners[i]);
                        info.MethodNames.Add(d.MethodNames[i]);
                    }

                    onlyWithName.Add(info);
                }
            }

            dependencies = onlyWithName.Count > 0 ? onlyWithName : depens;
        }

        static void FindDependencies()
        {
            dependencies = UnityEventReferenceFinder.FindAllUnityEventsReferences();
        }

        Rect GetDrawableRect()
        {
            return new Rect(Vector2.one * 30f, position.size - Vector2.one * 60f);
        }
    }

    public class EventReferenceInfo
    {
        public Object Owner { get; set; }
        public List<Object> Listeners { get; set; } = new List<Object>();
        public List<string> MethodNames { get; set; } = new List<string>();
    }

    public class UnityEventReferenceFinder
    {
        public static List<EventReferenceInfo> FindAllUnityEventsReferences()
        {
            List<EventReferenceInfo> infos = new List<EventReferenceInfo>();
            foreach (Component behaviour in GameObject.FindObjectsOfType<Component>())
            {
                List<FieldInfo> events = behaviour.GetType().GetTypeInfo().DeclaredFields.Where(f => f.FieldType.IsSubclassOf(typeof(UnityEventBase))).ToList();
                foreach (var e in events)
                {
                    UnityEventBase eventValue = e.GetValue(behaviour) as UnityEventBase;
                    int count = eventValue.GetPersistentEventCount();
                    EventReferenceInfo info = new EventReferenceInfo();
                    info.Owner = behaviour;

                    for (int i = 0; i < count; i++)
                    {
                        Object obj = eventValue.GetPersistentTarget(i);
                        string method = eventValue.GetPersistentMethodName(i);

                        info.Listeners.Add(obj);
                        info.MethodNames.Add(obj.GetType().Name.ToString() + "." + method);
                    }

                    infos.Add(info);
                }
            }
            return infos;
        }
    }
}