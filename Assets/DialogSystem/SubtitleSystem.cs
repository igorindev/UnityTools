using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DialogSystem
{
    public class SubtitleSystem : MonoBehaviour
    {
        public static SubtitleSystem instance;

        [SerializeField] GameObject subtitlePanel;
        [SerializeField] TextMeshProUGUI subtitleText;
        [SerializeField] LayoutElement layout;

        [Header("Condition")]
        [SerializeField] bool isSubstitlesOn;
        [SerializeField] bool playWhenPause = true;

        Coroutine active;

        public Dialog Speaker { get; set; }
        public bool IsSubstitlesOn { get => isSubstitlesOn; set => isSubstitlesOn = value; }

        void Awake()
        {
            if (instance != null)
            {
                Debug.Log("Ja existe um Level manager");
                Destroy(gameObject);
            }
            instance = this;
        }

        public void SetSpeaker(Dialog who)
        {
            Speaker = who;
        }

        void WriteSubtitle(Speech[] speeches, int count)
        {
            StartCoroutine(WriteText(speeches, count));
        }
        IEnumerator WriteText(Speech[] speeches, int count)
        {
            subtitleText.color = speeches[count].Npc.WhoIsTalkingColor;
            subtitleText.text = speeches[count].Npc.WhoIsTalking + ": " + speeches[count].Dialog;

            yield return null;
            yield return new WaitForEndOfFrame();
            if (subtitleText.rectTransform.rect.width >= 1650)
            {
                layout.enabled = true;
            }
            else
            {
                layout.enabled = false;
            }

            if (IsSubstitlesOn)
            {
                subtitlePanel.SetActive(true);
            }
        }

        public void PlayDialog(Speech[] speeches, AudioSource audioSource, int count = 0)
        {
            if (count >= speeches.Length)
            {
                EndDialog();
            }
            else
            {
                if (active != null)
                {
                    StopCoroutine(active);
                }

                WriteSubtitle(speeches, count);
                active = StartCoroutine(Auto(speeches, audioSource, count));
            }
        }
        IEnumerator Auto(Speech[] s, AudioSource audioSource, int count)
        {
            float duration = s[count].ClipDuration + s[count].ClipExtraDuration;
            if (s[count].AudioClip != null)
            {
                if (audioSource != null)
                {
                    audioSource.Stop();

                    audioSource.clip = s[count].AudioClip;
                    audioSource.Play();
                }
            }

            if (playWhenPause)
            {
                yield return new WaitForSecondsRealtime(duration);
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            subtitlePanel.SetActive(false);

            if (playWhenPause)
            {
                yield return new WaitForSecondsRealtime(s[count].WaitUntilNextClip);
            }
            else
            {
                yield return new WaitForSeconds(s[count].WaitUntilNextClip);
            }

            PlayDialog(s, audioSource, count + 1);
        }

        public void EndDialog()
        {
            layout.enabled = false;
            Speaker = null;
            subtitlePanel.SetActive(false);
            subtitleText.text = "";
        }
    }
}