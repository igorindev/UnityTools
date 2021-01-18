using System.Collections;
using UnityEditor;
using UnityEngine;

namespace NineRealmsTools
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeAnimation : MonoBehaviour
    {
        [Header("Fade")]
        [SerializeField] bool isTimeDependent = true;
        [SerializeField] FadeType type = 0;

        [Header("Durations")]
        float InDuration = 2f;
        float WaitDuration = 2f;
        float OutDuration = 2f;

        CanvasGroup canvasGroup;
        Coroutine active;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            StarFade();
        }

        [ContextMenu("StartFade")]
        public void StarFade()
        {
            StopFade();
            if (type == FadeType.OnlyIn || type == FadeType.InOut)
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

            if (type == FadeType.InOut)
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

            if (type == FadeType.OutIn)
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

        enum FadeType
        {
            OnlyIn,
            OnlyOut,
            InOut,
            OutIn,
        }

        [CustomEditor(typeof(FadeAnimation))]
        public class FadeAnimationEditor : Editor
        {
            override public void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var myScript = target as FadeAnimation;
                EditorGUILayout.Space(5);
                if (myScript.type == FadeType.OnlyIn)
                {
                    myScript.InDuration = EditorGUILayout.FloatField("In Duration", myScript.InDuration);
                }

                if (myScript.type == FadeType.OnlyOut)
                {
                    myScript.OutDuration = EditorGUILayout.FloatField("Out Duration", myScript.OutDuration);
                }

                if (myScript.type == FadeType.InOut || myScript.type == FadeType.OutIn)
                {
                    myScript.InDuration = EditorGUILayout.FloatField("In Duration", myScript.InDuration);
                    myScript.WaitDuration = EditorGUILayout.FloatField("Wait Duration", myScript.WaitDuration);
                    myScript.OutDuration = EditorGUILayout.FloatField("Out Duration", myScript.OutDuration);
                }
            }
        }
    }
}