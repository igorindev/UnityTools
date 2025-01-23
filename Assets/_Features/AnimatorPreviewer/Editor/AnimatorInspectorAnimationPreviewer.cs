using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

//https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/AnimatorInspector.cs

[CustomEditor(typeof(Animator), false)]
public class AnimatorInspectorAnimationPreviewer : Editor
{
    private const string AnimatorInspector = "UnityEditor.AnimatorInspector";
    private const string WarningAnimatorEmpty = "Animator Controller is empty";
    private const string WarningDisabledInPlayMode = "Disabled during play mode";

    static bool testFolderOpen;

    Animator animator;
    Editor editorInstance;
    EditorCoroutine editorCoroutine;

    void OnEnable()
    {
        Assembly ass = Assembly.GetAssembly(typeof(Editor));
        Type rtEditor = ass.GetType(AnimatorInspector);
        editorInstance = CreateEditor(target, rtEditor);

        animator = target as Animator;
    }

    public override void OnInspectorGUI()
    {
        editorInstance.OnInspectorGUI();

        if (testFolderOpen = EditorGUILayout.Foldout(testFolderOpen, "Animation Previewer"))
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                StopAnimation();
                GUILayout.Label(WarningDisabledInPlayMode);
            }
            else if (animator.runtimeAnimatorController)
            {
                List<AnimatorState> GetAnimatorStateInfo()
                {
                    AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
                    AnimatorControllerLayer[] acLayers = ac.layers;
                    List<AnimatorState> allStates = new List<AnimatorState>();
                    foreach (AnimatorControllerLayer i in acLayers)
                    {
                        ChildAnimatorState[] animStates = i.stateMachine.states;
                        foreach (ChildAnimatorState j in animStates)
                        {
                            allStates.Add(j.state);
                        }
                    }
                    return allStates;
                }

                List<AnimatorState> list = GetAnimatorStateInfo();

                foreach (AnimatorState item in list)
                {
                    GUI.enabled = item.motion != null;

                    if (GUILayout.Button(BuildButtonMessage(item)))
                    {
                        PlayAnimationDebug(item.motion as AnimationClip);
                    }
                }

                GUI.color = Color.red;
                GUI.enabled = true;
                if (GUILayout.Button("Stop"))
                {
                    StopAnimation();
                }
                GUI.color = Color.white;
            }
            else
            {
                GUILayout.Label(WarningAnimatorEmpty);
            }
        }
        else
        {
            StopAnimation();
        }
    }

    void OnDisable()
    {
        StopAnimation();

        DestroyImmediate(editorInstance);
    }

    void PlayAnimationDebug(AnimationClip clip)
    {
        StopAnimation();

        editorCoroutine = EditorCoroutineUtility.StartCoroutine(AnimationRoutine(clip), this);
    }

    IEnumerator AnimationRoutine(AnimationClip clip)
    {
        AnimationMode.StartAnimationMode();
        AnimationMode.BeginSampling();

        float value = 0;
        float editorDelta = 0;
        float editorLastUpdate = Time.realtimeSinceStartup;

        while (true)
        {
            AnimationMode.SampleAnimationClip(animator.gameObject, clip, value);

            value += editorDelta;

            if (clip.isLooping && value >= clip.length)
            {
                value -= clip.length;
            }

            yield return null;

            editorDelta = Time.realtimeSinceStartup - editorLastUpdate;
            editorLastUpdate = Time.realtimeSinceStartup;
        }
    }

    void StopAnimation()
    {
        if (editorCoroutine != null)
            EditorCoroutineUtility.StopCoroutine(editorCoroutine);

        if (AnimationMode.InAnimationMode())
        {
            AnimationMode.EndSampling();
            AnimationMode.StopAnimationMode();
        }
    }

    string BuildButtonMessage(AnimatorState state)
    {
        bool hasMotion = state.motion;

        string o1 = $"State: {state.name}";
        string o2 = $" - Clip: {(hasMotion ? state.motion.name : "Empty")}";
        string o3 = $"{(hasMotion ? ($" - Loop : {state.motion.isLooping}") : "")}";
        return o1 + o2 + o3;
    }
}