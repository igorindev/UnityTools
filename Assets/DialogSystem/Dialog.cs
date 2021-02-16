using System.Collections;
using UnityEngine;

namespace DialogSystem
{
    public class Dialog : MonoBehaviour
    {
        [Header("Dialogs")]
        [SerializeField] DialogObj[] npcDialogs = { };

        AudioSource audioSource;

        Coroutine active;
        public Speech[] speechs { get; set; } = { };
        public DialogObj[] NPCDialogs { get => npcDialogs; set => npcDialogs = value; }

        [ContextMenu("Play")]
        public void PlayRandomDialog()
        {
            speechs = npcDialogs[Random.Range(0, npcDialogs.Length)].dialogs;
            SubtitleSystem.instance.SetSpeaker(this);
            SubtitleSystem.instance.PlayDialog(speechs, audioSource);
        }

        public void PlayDialog(int index)
        {
            speechs = npcDialogs[index].dialogs;
            SubtitleSystem.instance.SetSpeaker(this);
            SubtitleSystem.instance.PlayDialog(speechs, audioSource);
        }

        private void OnValidate()
        {
            for (int i = 0; i < speechs.Length; i++)
            {
                if (speechs[i].AudioClip != null)
                {
                    speechs[i].ClipDuration = speechs[i].AudioClip.length;
                }
            }
        }
    }

    [System.Serializable]
    public struct Speech
    {
        [SerializeField] DialogNpcIdentity npc;

        [Header("Dialog")]
        [TextArea] [SerializeField] string dialogIndex;
        [SerializeField] AudioClip audioClip;

        [Header("Timers")]
        [SerializeField] float clipDuration;
        [SerializeField] float clipExtraDuration;
        [SerializeField] float waitUntilNextClip;

        public string Dialog => dialogIndex;

        public AudioClip AudioClip { get => audioClip; set => audioClip = value; }
        public float ClipDuration { get => clipDuration; set => clipDuration = value; }
        public float WaitUntilNextClip { get => waitUntilNextClip; set => waitUntilNextClip = value; }
        public float ClipExtraDuration { get => clipExtraDuration; set => clipExtraDuration = value; }
        public DialogNpcIdentity Npc { get => npc; set => npc = value; }
    }
}