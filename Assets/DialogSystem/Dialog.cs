using System.Collections;
using UnityEngine;

namespace DialogSystem
{
    public class Dialog : MonoBehaviour
    {
        [Header("Dialogs")]
        [SerializeField] DialogObj[] npcDialogs = { };

        AudioSource audioSource;

        public Speech[] speechs { get; set; } = { };
        public DialogObj[] NPCDialogs { get => npcDialogs; set => npcDialogs = value; }

        [ContextMenu("Play")]
        public void PlayRandomDialog()
        {
            speechs = npcDialogs[Random.Range(0, npcDialogs.Length)].dialogs;
            DialogSubtitleSystem.instance.SetSpeaker(this);
            DialogSubtitleSystem.instance.PlayDialog(speechs, audioSource);
        }

        public void PlayDialog(int index)
        {
            speechs = npcDialogs[index].dialogs;
            DialogSubtitleSystem.instance.SetSpeaker(this);
            DialogSubtitleSystem.instance.PlayDialog(speechs, audioSource);
        }
    }

    [System.Serializable]
    public struct Speech
    {
        [SerializeField] DialogNpcIdentity npc;

        [Header("Dialog")]
        [TextArea] [SerializeField] string dialogIndex;
        [SerializeField] DialogAudioClip audioClip;

        [Header("Timers")]
        [SerializeField] float clipDuration;
        [SerializeField] float clipExtraDuration;
        [SerializeField] float waitUntilNextClip;

        public string Dialog => dialogIndex;

        public AudioClip[] AudioClips { get => audioClip.Clips; }
        public float ClipDuration { get => clipDuration; set => clipDuration = value; }
        public float WaitUntilNextClip { get => waitUntilNextClip; set => waitUntilNextClip = value; }
        public float ClipExtraDuration { get => clipExtraDuration; set => clipExtraDuration = value; }
        public DialogNpcIdentity Npc { get => npc; set => npc = value; }
    }
}