using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFade : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] bool playOnStart = true;
    [SerializeField] bool isTimeDependent = true;
    [SerializeField] float startFadeInDelay = 0;
    [SerializeField] float startFadeOutDelay = 0;
    [SerializeField] FADETYPE type = 0;

    [Header("Durations")]
    float InDuration = 2f;
    float WaitDuration = 2f;
    float OutDuration = 2f;

    CanvasGroup canvasGroup;
    Coroutine active;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        //canvasGroup.alpha = 0;
        if (playOnStart)
        {
            StartFade();
        }
    }

    [ContextMenu("StartFade")]
    public void StartFade()
    {
        StopFade();
        if (type == FADETYPE.OnlyIn || type == FADETYPE.InOut)
        {
            active = StartCoroutine(FadeIn());
        }
        else
        {
            active = StartCoroutine(FadeOut());
        }
    }

    public void StarFade(FADETYPE fadeType, float inDuration = 1, float outDuration = 1)
    {
        StopFade();
        InDuration = inDuration;
        OutDuration = outDuration;
        type = fadeType;

        if (type == FADETYPE.OnlyIn || type == FADETYPE.InOut)
        {
            active = StartCoroutine(FadeIn());
        }
        else
        {
            active = StartCoroutine(FadeOut());
        }
    }

    void StarFadeIn()
    {
        StopFade();
        active = StartCoroutine(FadeIn());
    }
    void StarFadeOut()
    {
        StopFade();
        active = StartCoroutine(FadeOut());
    }

    public void StopFade()
    {
        if (active != null)
        {
            StopCoroutine(active);
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(startFadeInDelay);

        float count = 0;
        float value;
        while (count < InDuration)
        {
            value = isTimeDependent ? Time.deltaTime : Time.unscaledDeltaTime;
            count += value;
            canvasGroup.alpha = count / InDuration;

            yield return null;
        }
        canvasGroup.alpha = 1;

        if (type == FADETYPE.InOut)
        {
            if (isTimeDependent)
            {
                yield return new WaitForSeconds(WaitDuration);
            }
            else
            {
                yield return new WaitForSecondsRealtime(WaitDuration);
            }

            StarFadeOut();
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(startFadeOutDelay);

        float count = 0;
        float value;
        while (count < OutDuration)
        {
            value = isTimeDependent ? Time.deltaTime : Time.unscaledDeltaTime;
            count += value;
            canvasGroup.alpha = (OutDuration - count) / OutDuration;

            yield return null;
        }
        canvasGroup.alpha = 0;

        if (type == FADETYPE.OutIn)
        {
            if (isTimeDependent)
            {
                yield return new WaitForSeconds(WaitDuration);
            }
            else
            {
                yield return new WaitForSecondsRealtime(WaitDuration);
            }

            StarFadeIn();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIFade))]
    public class UIFadeEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = target as UIFade;
            EditorGUILayout.Space(5);
            if (myScript.type == FADETYPE.OnlyIn)
            {
                myScript.InDuration = EditorGUILayout.FloatField("In Duration", myScript.InDuration);
            }

            if (myScript.type == FADETYPE.OnlyOut)
            {
                myScript.OutDuration = EditorGUILayout.FloatField("Out Duration", myScript.OutDuration);
            }

            if (myScript.type == FADETYPE.InOut || myScript.type == FADETYPE.OutIn)
            {
                myScript.InDuration = EditorGUILayout.FloatField("In Duration", myScript.InDuration);
                myScript.WaitDuration = EditorGUILayout.FloatField("Wait Duration", myScript.WaitDuration);
                myScript.OutDuration = EditorGUILayout.FloatField("Out Duration", myScript.OutDuration);
            }
        }
    }
#endif
}
public enum FADETYPE
{
    OnlyIn,
    OnlyOut,
    InOut,
    OutIn,
}
