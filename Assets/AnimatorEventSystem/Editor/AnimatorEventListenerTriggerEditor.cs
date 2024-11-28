using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static AnimatorEventListener;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

[CustomPropertyDrawer(typeof(AnimatorEventListenerTrigger))]
public class AnimatorEventListenerTriggerEditor : PropertyDrawer
{
    private GameObject go;
    private Animator animator;
    private AnimatorController animatorController;
    private List<AnimatorState> states = new();
    private CultureInfo customCulture;

    public void Initialize(SerializedProperty property)
    {
        go = go != null ? go : (property.serializedObject.targetObject as AnimatorEventListener).gameObject;
        animator = animator != null ? animator : go.GetComponent<Animator>();
        animatorController = animatorController != null ? animatorController : GetCurrentController(animator);
        states = states == null || states.Count == 0 ? GetAnimatorStateInfo(animatorController) : states;

        customCulture ??= (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var hashProperty = property.FindPropertyRelative("hash");
        var triggerTimeProperty = property.FindPropertyRelative("triggerTime");
        var triggerOnlyOnceProperty = property.FindPropertyRelative("triggerOnlyOnce");
        var onTrigger = property.FindPropertyRelative("onTrigger");

        EditorGUI.BeginProperty(rect, label, property);
        EditorGUI.BeginChangeCheck();

        /// Label Field
        rect.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUIUtility.wideMode = true;
        EditorGUI.indentLevel = 0;

        Initialize(property);

        List<string> stateNames = new List<string>();
        foreach (AnimatorState item in states)
        {
            stateNames.Add(item.name);
        }

        if (stateNames.Count == 0)
        {
            return;
        }

        int index = 0;
        for (int i = 0; i < states.Count; i++)
        {
            if (states[i].nameHash == hashProperty.intValue)
            {
                index = i;
                break;
            }
        }

        index = EditorGUI.Popup(rect, "State To Listen", index, stateNames.ToArray());
        var result = stateNames[index];
        if (!string.IsNullOrEmpty(result))
        {
            hashProperty.intValue = Animator.StringToHash(result);
        }

        EditorGUI.indentLevel = 1;

        rect.y += rect.height;
        float widthSize = rect.width / 4;
        float offsetSize = 2;

        Rect pos1 = new Rect(rect.x, rect.y, widthSize * 2 - offsetSize, rect.height);
        Rect pos2 = new Rect(rect.x + widthSize * 3, rect.y, widthSize, rect.height);

        GUI.enabled = false;
        EditorGUI.ObjectField(rect, "Animation Clip", states[index].motion, typeof(AnimationClip), false);
        GUI.enabled = true;
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleRight;
        EditorGUI.LabelField(pos2, string.Format(customCulture, $"{states[index].motion.averageDuration:F3} sec      "), style);

        rect.y += rect.height;
        triggerTimeProperty.floatValue = EditorGUI.Slider(rect, "Clip Trigger Time", triggerTimeProperty.floatValue * states[index].motion.averageDuration, 0f, states[index].motion.averageDuration);
        triggerTimeProperty.floatValue /= states[index].motion.averageDuration;

        rect.y += rect.height;
        triggerOnlyOnceProperty.boolValue = EditorGUI.Toggle(rect, "Trigger Only Once", triggerOnlyOnceProperty.boolValue);

        /// Property Field
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        rect.height = EditorGUI.GetPropertyHeight(onTrigger);
        EditorGUI.PropertyField(rect, onTrigger);

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Height of the label
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Height of the property
        var size = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onTrigger"));
        height += size;
        height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3.3f;
        return height;
    }

    private List<AnimatorState> GetAnimatorStateInfo(AnimatorController ac)
    {
        if (ac == null)
        {
            return new List<AnimatorState>();
        }

        AnimatorControllerLayer[] acLayers = ac.layers;
        List<AnimatorState> allStates = new List<AnimatorState>();
        foreach (AnimatorControllerLayer i in acLayers)
        {
            ChildAnimatorState[] animStates = i.stateMachine.states;
            foreach (ChildAnimatorState j in animStates)
            {
                if (j.state.behaviours.OfType<AnimatorEventDispatcherStateBehaviour>().Count() > 0)
                {
                    allStates.Add(j.state);
                }
            }
        }
        return allStates;
    }

    private static AnimatorController GetCurrentController(Animator animator)
    {
        string assetPath = AssetDatabase.GetAssetPath(animator.runtimeAnimatorController);
        return AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
    }
}
