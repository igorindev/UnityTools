using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AnimationPreviewer : EditorWindow
{
    [SerializeField] Animator animatedGameObject;
    [SerializeField] AnimationClip[] clips = new AnimationClip[0];
    [SerializeField] int currentClip;
    [SerializeField] float time;

    static AnimationPreviewer window;

    SerializedObject so;

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
        so = new SerializedObject(target);
    }

    void OnGUI()
    {
        GUILayout.Label("");

        so.Update();

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            GUILayout.Label("Disabled during PlayMode");
            return;
        }

        GUILayout.Label("Preview Animations from animator");

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(15);
            using (new EditorGUILayout.VerticalScope())
            {
                animatedGameObject = EditorGUILayout.ObjectField(animatedGameObject, typeof(Animator), true) as Animator;
                if (!animatedGameObject)
                {
                    return;
                }

                time = EditorGUILayout.Slider("Sequence", time, 0, 100);
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
                SerializedProperty clipsProperty = so.FindProperty("clips");

                currentClip = EditorGUILayout.IntField("Current Clip", currentClip);

                EditorGUILayout.PropertyField(clipsProperty, true);
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
                    clips[currentClip].SampleAnimation(animatedGameObject.gameObject, value);
                }
            }
            else
            {
                currentClip = 0;
            }
        }

        so.ApplyModifiedProperties();
    }

    public void CreatePrefabWithPose()
    {
        PrefabUtility.SaveAsPrefabAsset(animatedGameObject.gameObject, "Assets/" + animatedGameObject.gameObject.name + ".prefab", out bool success);
        if (success)
        {
            Debug.Log("Prefab created at: " + "Assets/" + animatedGameObject.gameObject.name + ".prefab");
        }
        else
        {
            Debug.LogError("Failed creating prefab.");
        }

        AssetDatabase.Refresh();
    }
}
