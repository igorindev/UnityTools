using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    [SerializeField] Subtitle[] substitles;
    [SerializeField] int simultaneous = 3;

    bool coroutineStarted = false;

    void Start()
    {
        for (int i = 0; i < substitles.Length; i++)
        {
            substitles[i].text.verticalAlignment = VerticalAlignmentOptions.Middle;
            substitles[i].text.horizontalAlignment = HorizontalAlignmentOptions.Center;

            substitles[i].canvasGroup.alpha = 0;
        }
    }
   

    [ContextMenu("a")]
    void Test()
    {
        //need to add offst
        SetText("Meu cu Meu cu Meu cuMeu cuMeu cu Meu cu");
    }

    void UpdateTextsSize()
    {

    }

    public void SetText(string text, float duration = 3f, bool priority = false)
    {
        //if duration is too short, increase to 1.5f sec fixed
        duration = duration < 1.5f ? 1.5f : duration;

        int toUse = -1;
        //allocate subtitle that is not in use
        for (int i = 0; i < substitles.Length; i++)
        {
            if (substitles[i].InUse == false)
            {
                toUse = i;
                substitles[toUse].text.text = text;
                substitles[toUse].Duration = duration;
                substitles[toUse].InUse = true;
                substitles[toUse].Priority = priority;
                break;
            }
        }

        //if there is none availble, get the first, that is not priority
        if (toUse == -1) //Not found
        {
            for (int i = 0; i < substitles.Length; i++)
            {
                if (substitles[i].Priority == false)
                {
                    toUse = i;
                    substitles[toUse].Duration = duration;
                    substitles[toUse].InUse = true;
                    break;
                }
            }
        }

        //override a priority with less duration remaining if other priority need, and all texts are already priority

        //Update size of background
        StartCoroutine(WaitFrameToDrawText(toUse));

        //Move old up - DO NOT move if is a priority

        //Start counter to disapear text based on duration
        if (coroutineStarted == false)
        {
            StartCoroutine(SubtitlesCounter());
        }
    }

    IEnumerator WaitFrameToDrawText(int i)
    {
        yield return null;
        yield return null;

        substitles[i].background.sizeDelta = new(substitles[i].text.renderedWidth + 20, substitles[i].text.renderedHeight + 20);
    }

    //if none in use, disable counter
    IEnumerator SubtitlesCounter()
    {
        coroutineStarted = true;
        bool thereIsSuntitleInUse = true;
        while (thereIsSuntitleInUse)
        {
            thereIsSuntitleInUse = false;

            for (int i = 0; i < substitles.Length; i++)
            {
                if (substitles[i].InUse)
                {
                    substitles[i].Duration -= Time.deltaTime;
                    if (substitles[i].Duration <= 0)
                    {
                        if (substitles[i].canvasGroup.alpha > 0)
                        {
                            //fade out
                            substitles[i].canvasGroup.alpha -= Time.deltaTime;
                        }
                        else
                        {
                            substitles[i].canvasGroup.alpha = 0;
                            substitles[i].InUse = false;
                        }
                    }
                    else
                    {
                        if (substitles[i].canvasGroup.alpha < 1)
                        {
                            //fade in
                            substitles[i].canvasGroup.alpha += Time.deltaTime;
                        }
                    }
                    thereIsSuntitleInUse = true; //there is one in use
                }
            }

            yield return null;
        }
        coroutineStarted = false;
    }

    [System.Serializable]
    public class Subtitle
    {
        [Header("Subtitle")]
        public TextMeshProUGUI text;
        public RectTransform background;
        public CanvasGroup canvasGroup;
        public bool InUse { get; set; }
        public bool Priority { get; set; }
        public float Duration { get; set; }
    }
}
