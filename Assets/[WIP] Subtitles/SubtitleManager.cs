using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class SubtitleManager : MonoBehaviour
{
    [SerializeField] Subtitle[] subtitles;
    [SerializeField] int simultaneous = 3;
    [SerializeField] float fadeInSpeed = 1;
    [SerializeField] float fadeOutSpeed = 1;
    [SerializeField] float moveUpSpeed = 1;
    [SerializeField] bool alertIfToLong;

    bool coroutineStarted = false;
    List<int> subtitlesQueued = new List<int>();

    string[] testPhrases = { "This is a test",
                             "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit...",
                             "Where does it come from?",
                             "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
                             "Where is this?",
                             "Get over here",
                             "Over",
                             "Kill me NOW",
    };

    void OnValidate()
    {
        subtitles = new Subtitle[transform.GetChild(0).childCount];

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            Debug.Log(transform.GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>());
            subtitles[i].text = transform.GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            subtitles[i].background = transform.GetChild(0).GetChild(i).GetComponent<RectTransform>();
            subtitles[i].canvasGroup = transform.GetChild(0).GetChild(i).GetComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].text.verticalAlignment = VerticalAlignmentOptions.Middle;
            subtitles[i].text.horizontalAlignment = HorizontalAlignmentOptions.Center;

            subtitles[i].canvasGroup.alpha = 0;
            subtitles[i].background.anchoredPosition = new Vector2(0, -subtitles[i].Height);
        }
    }

    [ContextMenu("Test")]
    void Test()
    {
        //need to add offst
        SetText(testPhrases.GetRandom(),Random.Range(5f, 5f));
    }

    public void UpdateTextsSize(float size)
    {

    }

    public void SetText(string text, float duration = 5f, bool priority = false)
    {
        //if duration is too short, increase to 1.5f sec fixed
        duration = duration < 1.5f ? 1.5f : duration;

        int toUse = -1;
        //allocate subtitle that is not in use
        for (int i = 0; i < subtitles.Length; i++)
        {
            if (subtitles[i].InUse == false)
            {
                toUse = i;
                subtitles[toUse].text.text = text;
                subtitles[toUse].Duration = duration;
                subtitles[toUse].InUse = true;
                subtitles[toUse].Ready = false;
                subtitles[toUse].Priority = priority;
                subtitles[toUse].canvasGroup.alpha = 0;
                break;
            }
        }

        //if there is none availble, get the first, that is not priority
        if (toUse == -1) //Not found
        {
            // for (int i = 0; i < subtitles.Length; i++)
            // {
            //     if (subtitles[i].Priority == false)
            //     {
            //         toUse = i;
            //         subtitles[toUse].Duration = duration;
            //         subtitles[toUse].InUse = true;
            //         break;
            //     }
            // }
            toUse = subtitlesQueued[0];
            subtitles[toUse].text.text = text;
            subtitles[toUse].Duration = duration;
            subtitles[toUse].InUse = true;
            subtitles[toUse].Ready = false;
            subtitles[toUse].Priority = priority;
            subtitles[toUse].canvasGroup.alpha = 0;
        }

        //override a priority with less duration remaining if other priority need, and all texts are already priority

        //Update size of background
        StartCoroutine(WaitFrameToDrawText(toUse));
    }

    IEnumerator WaitFrameToDrawText(int index)
    {
        yield return null;
        yield return null;

        if (subtitlesQueued.Contains(index))
        {
            subtitlesQueued.Remove(index);
        }

        subtitles[index].background.sizeDelta = new(subtitles[index].text.renderedWidth + 20, subtitles[index].text.renderedHeight + 20);
        for (int i = 0; i < subtitlesQueued.Count; i++)
        {
            subtitles[subtitlesQueued[i]].Height += subtitles[index].background.sizeDelta.y;
        }

        subtitles[index].background.anchoredPosition = subtitlesQueued.Count < 1 ? Vector2.zero : Vector2.down * subtitles[index].background.sizeDelta.y;
        subtitles[index].Height = 0;
        subtitles[index].Ready = true;
        subtitlesQueued.Add(index);

        if (subtitlesQueued.Count > simultaneous)
        {
            subtitles[subtitlesQueued[0]].Duration = subtitles[subtitlesQueued[0]].Duration < 0 ? subtitles[subtitlesQueued[0]].Duration : 0;
            subtitlesQueued.RemoveAt(0);
        }

        //Start counter to disapear text based on duration
        if (coroutineStarted == false)
        {
            StartCoroutine(SubtitlesCounter());
        }
    }

    //if none in use, disable counter
    IEnumerator SubtitlesCounter()
    {
        coroutineStarted = true;
        bool thereIsSuntitleInUse = true;
        while (thereIsSuntitleInUse)
        {
            thereIsSuntitleInUse = false;

            for (int i = 0; i < subtitles.Length; i++)
            {
                if (subtitles[i].InUse && subtitles[i].Ready)
                {
                    subtitles[i].Duration -= Time.deltaTime;

                    //Move up
                    if (subtitles[i].background.anchoredPosition.y < subtitles[i].Height)
                    {
                        subtitles[i].background.anchoredPosition += moveUpSpeed * 100 * Time.deltaTime * Vector2.up;
                        if (subtitles[i].background.anchoredPosition.y >= subtitles[i].Height)
                        {
                            subtitles[i].background.anchoredPosition = new Vector2(subtitles[i].background.anchoredPosition.x, subtitles[i].Height);
                        }
                    }

                    if (subtitles[i].Duration <= 0)
                    {
                        if (subtitles[i].canvasGroup.alpha > 0)
                        {
                            //fade out
                            subtitles[i].canvasGroup.alpha -= fadeOutSpeed * Time.deltaTime;
                        }
                        else
                        {
                            subtitles[i].canvasGroup.alpha = 0;
                            subtitles[i].InUse = false;
                            subtitles[i].Ready = false;
                            subtitlesQueued.Remove(i);
                        }
                    }
                    else
                    {
                        if (subtitles[i].canvasGroup.alpha < 1)
                        {
                            //fade in
                            subtitles[i].canvasGroup.alpha += fadeInSpeed * Time.deltaTime;
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
    public struct Subtitle
    {
        public TextMeshProUGUI text;
        public RectTransform background;
        public CanvasGroup canvasGroup;
        public bool InUse { get; set; }
        public bool Ready { get; set; }
        public bool Priority { get; set; }
        public float Duration { get; set; }
        public float Height { get; set; }
    }
}