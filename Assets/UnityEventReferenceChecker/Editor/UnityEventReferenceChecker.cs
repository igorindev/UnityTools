using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace UnityEventReference
{
    public static class UnityEventReferenceChecker
    {
        static bool foundError;

        [MenuItem("Tools/UnityEvents/Check References", priority = 10000)]
        static void FindAllUnityEventsReferences()
        {
            foundError = false;
            foreach (Component behaviour in Resources.FindObjectsOfTypeAll<Component>())
            {
                List<FieldInfo> events = behaviour.GetType().GetTypeInfo().DeclaredFields.Where(f => f.FieldType.IsSubclassOf(typeof(UnityEventBase))).ToList();
                foreach (FieldInfo e in events)
                {
                    UnityEventBase eventValue = e.GetValue(behaviour) as UnityEventBase;
                    DetectUnityEventError(eventValue, behaviour);
                }
            }

            if (foundError == false)
            {
                Debug.Log("<color=cyan>UnityEventReferenceChecker:</color> <color=white>Everything is fine.</color>");
            }
        }
        static void DetectUnityEventError(UnityEventBase ue, Component behaviour)
        {
            for (int i = 0; i < ue.GetPersistentEventCount(); i++)
            {
                Object targetObj = ue.GetPersistentTarget(i);
                string methodName = ue.GetPersistentMethodName(i); ;

                //Check if the class that holds the function is null then exit if it is 
                if (targetObj == null)
                {
                    foundError = true;
                    if (behaviour.gameObject.scene.name == null)
                    {
                        string path = AssetDatabase.GetAssetPath(behaviour.gameObject);

                        Debug.LogError("<color=cyan>UnityEventReferenceChecker:</color> <color=white>UnityEvent from <color=cyan>Prefab</color> at \"" + path + "\" is missing it's object reference. Please check if this reference still exists.</color>", behaviour.gameObject);
                    }
                    else
                    {
                        Debug.LogError($"<color=cyan>UnityEventReferenceChecker:</color> <color=white>UnityEvent from <b>\"{behaviour.gameObject.name}\"</b> in <b>{behaviour.gameObject.scene.name}.scene</b> is missing it's object reference. " +
                                  $"Please check if this reference still exists.</color>", behaviour.gameObject);
                    }
                    continue;
                }

                //Get full target class name(including namespace)
                Type objectFullNameWithNamespace = targetObj.GetType();

                //Check if the class that holds the function exist then exit if it does not
                if (!ClassExist(objectFullNameWithNamespace))
                {
                    foundError = true;
                    if (behaviour.gameObject.scene.name == null)
                    {
                        string path = AssetDatabase.GetAssetPath(behaviour.gameObject);

                        Debug.LogError($"<color=cyan>UnityEventReferenceChecker:</color> <color=white>UnityEvent from <color=cyan>Prefab</color> at \"{path}\" class reference is missing. " +
                                  $"Please check if this class still exists.</color>", behaviour.gameObject);
                    }
                    else
                    {
                        Debug.LogError($"<color=cyan>UnityEventReferenceChecker:</color> <color=white>UnityEvent from <b>\"{behaviour.gameObject.name}\"</b> in <b>{behaviour.gameObject.scene.name}.scene</b> class reference is missing. " +
                                  $"Please check if this class still exists.</color>", behaviour.gameObject);
                    }
                    continue;
                }

                if (FunctionExistAsPublicInTarget(targetObj, methodName))
                {
                    continue;
                }
                else if (FunctionExistAsPrivateInTarget(targetObj, methodName))
                {
                    foundError = true;
                    Debug.LogWarning($"<color=cyan>UnityEventReferenceChecker:</color> <color=white>The registered Function {methodName}() exist as a private function. Please change it in {objectFullNameWithNamespace} script to a public</color>", behaviour.gameObject);
                }
                else
                {
                    foundError = true;
                    if (string.IsNullOrEmpty(methodName))
                    {
                        Debug.LogError($"<color=cyan>UnityEventReferenceChecker:</color> <color=white>The UnityEvent from <b>\"{behaviour.gameObject.name}\"</b> in <b>{behaviour.gameObject.scene.name}.scene</b> is not calling a method.</color>", behaviour.gameObject);
                    }
                    else
                    {
                        Debug.LogError($"<color=cyan>UnityEventReferenceChecker:</color> <color=white>The {methodName}() function does <b>NOT</b> exist in the {objectFullNameWithNamespace.FullName} script.</color>", behaviour.gameObject);
                    }
                }
            }
        }

        static bool ClassExist(Type classType) => classType != null;
        static bool FunctionExistAsPublicInTarget(Object target, string functionName) => target.GetType().GetMethod(functionName) != null;
        static bool FunctionExistAsPrivateInTarget(Object target, string functionName) => target.GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.NonPublic) != null;
    }
}