using UnityEditor;
using UnityEngine;

public class AnimationPreviewer : EditorWindow
{
    static AnimationPreviewer window;

    [SerializeField] Animator animatorReference;
    [SerializeField] AnimationClip[] clips = new AnimationClip[0];
    int currentClip;
    float percentage;
    float playbackSpeed = 1;
    float editorLastUpdate;
    float editorDelta;
    bool autoPlay;
    bool playing;

    SerializedObject serializedObject;

    GUIStyle playStyle;

    [MenuItem("Tools/Animation Previewer...")]
    static void Create()
    {
        window = GetWindow<AnimationPreviewer>();
        window.position = new Rect(0, 0, 250, 200);
        window.titleContent = new GUIContent("Animation Previewer", EditorGUIUtility.IconContent("UnityEditor.ProfilerWindow").image, "Animation Previewer");
        window.Show();
    }

    void OnEnable()
    {
        ScriptableObject target = this;
        serializedObject = new SerializedObject(target);

        EditorApplication.update += OnEditorUpdate;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        editorLastUpdate = Time.realtimeSinceStartup;
    }

    void OnDestroy()
    {
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        StopAnimation();
    }

    void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        StopAnimation();
    }

    void OnEditorUpdate()
    {
        if (playing && autoPlay)
        {
            percentage += (editorDelta * 100 * playbackSpeed) / clips[currentClip].length;
            Repaint();
            if (percentage >= 100)
            {
                percentage = 0;
            }
            else if (percentage <= 0)
            {
                percentage = 100;
            }

            editorDelta = Time.realtimeSinceStartup - editorLastUpdate;
            editorLastUpdate = Time.realtimeSinceStartup;
        }
    }

    void OnGUI()
    {
        playStyle = new GUIStyle("Button");
        GUILayout.Label("");

        serializedObject.Update();

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            GUILayout.Label("Disabled during PlayMode");
            return;
        }

        GUILayout.Label("Preview Animations from an animator");

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(15);
            using (new EditorGUILayout.VerticalScope())
            {
                animatorReference = EditorGUILayout.ObjectField(animatorReference, typeof(Animator), true) as Animator;
                if (!animatorReference) return;

                StartAnimation();

                percentage = EditorGUILayout.Slider("Sequence", percentage, 0, 100);
                playbackSpeed = EditorGUILayout.Slider("Playback Speed", playbackSpeed, -2, 2);
                autoPlay = GUILayout.Toggle(autoPlay, "Play", playStyle, GUILayout.Width(60));

                if (autoPlay)
                {
                    PlayAuto();
                }
                else
                {
                    if (playing)
                    {
                        playing = false;
                    }
                }
            }
        }
        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Prefab"))
            {
                CreatePrefabWithPose();
            }
        }

        GUILayout.Label("Animation Clips");
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(15);
            using (new EditorGUILayout.VerticalScope())
            {
                SerializedProperty clipsProperty = serializedObject.FindProperty("clips");

                currentClip = EditorGUILayout.IntField("Current Clip", currentClip);

                EditorGUILayout.PropertyField(clipsProperty, true);
            }
        }

        if (animatorReference)
        {
            if (clips.Length > 0)
            {
                currentClip = Mathf.Clamp(currentClip, 0, clips.Length - 1);

                if (clips[currentClip])
                {
                    float value = clips[currentClip].length * (percentage / 100f);
                    AnimationMode.SampleAnimationClip(animatorReference.gameObject, clips[currentClip], value);
                }
            }
            else
            {
                currentClip = 0;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void PlayAuto()
    {
        if (clips.Length == 0)
        {
            Debug.LogWarning("There is no clip.");
            if (autoPlay)
            {
                autoPlay = false;
                if (playing)
                {
                    playing = false;
                }
            }
            return;
        }

        if (playing == false)
        {
            playing = true;
        }
    }

    public void CreatePrefabWithPose()
    {
        PrefabUtility.SaveAsPrefabAsset(animatorReference.transform.root.gameObject, "Assets/" + animatorReference.gameObject.name + ".prefab", out bool success);
        if (success)
        {
            Debug.Log("Prefab created at: " + "Assets/" + animatorReference.gameObject.name + ".prefab");
        }
        else
        {
            Debug.LogError("Failed creating prefab.");
        }

        AssetDatabase.Refresh();
    }

    static void StartAnimation()
    {
        if (!AnimationMode.InAnimationMode())
        {
            AnimationMode.StartAnimationMode();
            AnimationMode.BeginSampling();
        }
    }

    static void StopAnimation()
    {
        if (AnimationMode.InAnimationMode())
        {
            AnimationMode.EndSampling();
            AnimationMode.StopAnimationMode();
        }
    }

}
