using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Subtitles
{
    public enum IMPORTANCE
    {
        Low, //Italic with alpha
        Normal,
        High //Bold
    }

    public class SubtitleManager : MonoBehaviour
    {
        [SerializeField, Min(0)] int simultaneous = 3;

        [Header("Text")]
        [SerializeField] float textSize = 25;
        [SerializeField] float textWidth = 550;

        [Header("Effects")]
        [SerializeField] float fadeInSpeed = 1;
        [SerializeField] float fadeOutSpeed = 1;
        [SerializeField] float moveUpSpeed = 1;

        [Header("Background")]
        [SerializeField] float textOffsetWidth = 20;
        [SerializeField] float textOffsetHeight = 20;

        [Header("Allocation")]
        [SerializeField] bool instantiateIfNeeeded;

        [Header("Text Color")]
        [SerializeField] Color low = new Color(0.65f, 0.65f, 0.65f);
        [SerializeField] Color normal = Color.white;
        [SerializeField] Color high = Color.white;

        [Header("Background Color")]
        [SerializeField] Color background = new Color(0, 0, 0, 0.313f);

        [Header("Debug")]
        [SerializeField] float testSpawnTimer = 0.2f;
        [SerializeField] bool alertIfTextToLong;
        [SerializeField] int textLimitSize = 60;

        [Header("")]
        [SerializeField, ReadOnly] Subtitle[] subtitles;

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

        Vector2 startPos;
        float counter;

        StringBuilder sb = new StringBuilder();

        void OnValidate()
        {
            subtitles = new Subtitle[transform.GetChild(0).childCount];

            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                subtitles[i].text = transform.GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                subtitles[i].subtitleRect = transform.GetChild(0).GetChild(i).GetComponent<RectTransform>();
                subtitles[i].background = transform.GetChild(0).GetChild(i).GetComponent<Image>();
                subtitles[i].canvasGroup = transform.GetChild(0).GetChild(i).GetComponent<CanvasGroup>();
            }
        }

        void Start()
        {
            for (int i = 0; i < subtitles.Length; i++)
            {
                subtitles[i].text.rectTransform.sizeDelta.Set(subtitles[i].text.rectTransform.sizeDelta.x, textWidth);
                subtitles[i].text.verticalAlignment = VerticalAlignmentOptions.Middle;
                subtitles[i].text.horizontalAlignment = HorizontalAlignmentOptions.Center;

                subtitles[i].canvasGroup.alpha = 0;
                subtitles[i].subtitleRect.anchoredPosition = new Vector2(0, -subtitles[i].Height);
                subtitles[i].subtitleRect.pivot = new Vector2(0.5f, 0);
                subtitles[i].background.color = background;
            }

            UpdateTextsSize(textSize);
        }

        [ContextMenu("Test All")]
        void Test()
        {
            InvokeRepeating(nameof(InvokeTest), 1, testSpawnTimer);
        }
        [ContextMenu("Test One")]
        void InvokeTest()
        {
            AddSubstitle(testPhrases.GetRandom(), Random.Range(1f, 5f), (IMPORTANCE)Random.Range(0, 3), Random.Range(0, 2) == 0 ? true : false);
        }

        [ContextMenu("Update Text")]
        void UpdateText()
        {
            UpdateTextsSize(textSize);
        }

        public void UpdateTextsSize(float size)
        {
            for (int i = 0; i < subtitles.Length; i++)
            {
                subtitles[i].text.fontSize = size;
            }
        }
        void InstantiateIfNeeded()
        {

        }
        public void ChangeBackgroundColor(Color color)
        {
            background = color;
        }

        public void AddSubstitle(string text, float duration = 5f, IMPORTANCE importance = IMPORTANCE.Normal, bool closedCaption = false)
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
                    break;
                }
            }

            //if there is none availble, get the first, that is not priority
            if (toUse == -1) //Not found
            {
                if (instantiateIfNeeeded)
                {
                    InstantiateIfNeeded();
                }
                else
                {
                    toUse = subtitlesQueued[0];
                }
            }

            switch (importance)
            {
                case IMPORTANCE.Low:
                    subtitles[toUse].text.color = low;
                    subtitles[toUse].text.fontStyle = FontStyles.Italic;
                    break;
                case IMPORTANCE.Normal:
                    subtitles[toUse].text.color = normal;
                    subtitles[toUse].text.fontStyle = FontStyles.Normal;
                    break;
                case IMPORTANCE.High:
                    subtitles[toUse].text.color = high;
                    subtitles[toUse].text.fontStyle = FontStyles.Bold;
                    break;
            }

            sb.Clear();

            if (closedCaption)
                sb.Append("[");

            if (text.Length > textLimitSize && alertIfTextToLong)
                sb.Append("<color=red>Attention: this text is to long.</color> ");

            sb.Append(text);

            if (closedCaption)
                sb.Append("]");

            subtitles[toUse].text.text = sb.ToString();
            subtitles[toUse].Duration = duration;
            subtitles[toUse].InUse = true;
            subtitles[toUse].Ready = false;
            //subtitles[toUse].Priority = priority;
            subtitles[toUse].canvasGroup.alpha = 0;
            subtitles[toUse].background.color = background;

            //override a priority with less duration remaining if other priority need, and all texts are already priority

            //Update size of background
            StartCoroutine(WaitFrameToDrawText(toUse));
        }

        IEnumerator WaitFrameToDrawText(int index)
        {
            yield return null;
            yield return null;
            counter = 0;
            if (subtitlesQueued.Contains(index))
            {
                subtitlesQueued.Remove(index);
            }

            subtitles[index].subtitleRect.sizeDelta = new(subtitles[index].text.renderedWidth + textOffsetWidth, subtitles[index].text.renderedHeight + textOffsetHeight);

            subtitles[index].subtitleRect.anchoredPosition = subtitlesQueued.Count < 1 ? Vector2.zero :
                                                                                        (Vector2.down * subtitles[index].subtitleRect.sizeDelta.y)
                                                                                        + subtitles[subtitlesQueued[subtitlesQueued.Count - 1]].subtitleRect.anchoredPosition;
            float h = subtitles[index].subtitleRect.sizeDelta.y;
            for (int i = 0; i < subtitlesQueued.Count; i++)
            {
                subtitles[subtitlesQueued[i]].Start = subtitles[subtitlesQueued[i]].subtitleRect.anchoredPosition.y;
                subtitles[subtitlesQueued[i]].Height += h;
            }

            if (subtitlesQueued.Count > 0)
            {
                Debug.Log((Vector2.down * subtitles[index].subtitleRect.sizeDelta.y) + " | " + subtitles[subtitlesQueued[subtitlesQueued.Count - 1]].subtitleRect.anchoredPosition);
            }

            subtitles[index].Start = subtitles[index].subtitleRect.anchoredPosition.y;
            subtitles[index].Height = 0;
            subtitles[index].Ready = true;
            subtitlesQueued.Add(index);

            if (simultaneous > 0)
            {
                if (subtitlesQueued.Count > simultaneous)
                {
                    subtitles[subtitlesQueued[0]].Duration = subtitles[subtitlesQueued[0]].Duration < 0 ? subtitles[subtitlesQueued[0]].Duration : 0;
                    subtitles[subtitlesQueued[0]].canvasGroup.alpha = 0;
                }
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
                counter += Time.deltaTime * 2;
                for (int i = 0; i < subtitles.Length; i++)
                {
                    if (subtitles[i].InUse && subtitles[i].Ready)
                    {
                        subtitles[i].Duration -= Time.deltaTime;

                        //Move up
                        if (subtitles[i].subtitleRect.anchoredPosition.y < subtitles[i].Height)
                        {
                            float h = Mathf.Lerp(subtitles[i].Start, subtitles[i].Height, counter);

                            subtitles[i].subtitleRect.anchoredPosition = new Vector2(0, h);

                            if (subtitles[i].subtitleRect.anchoredPosition.y >= subtitles[i].Height)
                            {
                                subtitles[i].subtitleRect.anchoredPosition = new Vector2(subtitles[i].subtitleRect.anchoredPosition.x, subtitles[i].Height);
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
            public RectTransform subtitleRect;
            public Image background;
            public CanvasGroup canvasGroup;
            public bool InUse { get; set; }
            public bool Ready { get; set; }
            public bool Priority { get; set; }
            public float Duration { get; set; }
            public float Start { get; set; }
            public float Height { get; set; }
        }
    }
}