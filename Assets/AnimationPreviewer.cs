using UnityEditor;
using UnityEngine;

public class AnimationPreviewer : EditorWindow
{
    [SerializeField] GameObject animatedGameObject;
    [SerializeField] AnimationClip[] clips = new AnimationClip[0];
    [SerializeField] int currentClip;
    [SerializeField] float time;
    [SerializeField] bool autoPlay;
    [SerializeField] float autoPlaySpeed;

    public string[] Strings = { "Larry", "Curly", "Moe" };
    [MenuItem("Tools/Animation Previewer")]
    static void Create()
    {
        var window = GetWindow<AnimationPreviewer>();
        window.position = new Rect(0, 0, 250, 200);
        window.titleContent = new GUIContent("Animation Previewer", EditorGUIUtility.IconContent("UnityEditor.ProfilerWindow").image, "Animation Previewer");
        window.Show();
    }

    void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { return; }
        if (autoPlay)
        {
            time += autoPlaySpeed * Time.deltaTime;
            float value = clips[currentClip].length * time / 100;
            if (time >= 100)
            {
                time = 0;
            }
            else if (time < 0)
            {
                time = 100;
            }
            clips[currentClip].SampleAnimation(animatedGameObject, value);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("");
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            GUILayout.Label("Disabled during PlayMode");
            return;
        }

        GUILayout.Label("Preview an Animation Clip of a player");

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(15);
            using (var m = new EditorGUILayout.VerticalScope())
            {
                animatedGameObject = EditorGUILayout.ObjectField(animatedGameObject, typeof(GameObject), true) as GameObject;
                if (!animatedGameObject)
                {
                    return;
                }

                animatedGameObject = animatedGameObject.GetComponentInChildren<Animator>().gameObject;
                time = EditorGUILayout.Slider("Sequence", time, 0, 100);
                using (new EditorGUILayout.HorizontalScope())
                {
                    autoPlay = EditorGUILayout.Toggle("Auto Play", autoPlay);
                    autoPlaySpeed = EditorGUILayout.FloatField("Auto Play Speed", autoPlaySpeed);
                }
            }
        }
        GUILayout.Space(10);
        GUILayout.Label("Animation Clips");
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(15);
            using (new EditorGUILayout.VerticalScope())
            {
                ScriptableObject target = this;
                SerializedObject so = new SerializedObject(target);
                SerializedProperty property = so.FindProperty("clips");
                currentClip = EditorGUILayout.IntField("Current Clip", currentClip);
                EditorGUILayout.PropertyField(property, true);
                so.ApplyModifiedProperties();

            }
        }

        if (animatedGameObject)
        {
            if (clips.Length > 0)
            {
                if (currentClip < 0)
                {
                    currentClip = 0;
                }
                else if (currentClip >= clips.Length)
                {
                    currentClip = clips.Length - 1;
                }

                if (clips[currentClip])
                {
                    float value = clips[currentClip].length * time / 100;
                    clips[currentClip].SampleAnimation(animatedGameObject, value);
                }
            }
            else
            {
                currentClip = 0;
            }
        }
        else
        {
            currentClip = 0;
        }
    }
}
