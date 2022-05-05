using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceEntity : MonoBehaviour
{
    public AudioSource AudioSource { get; set; }
    public bool InUse { get; set; }
    public Transform AudioParent { get; set; }

    public void ResetPoolObject(float time) 
    {
        StartCoroutine(WaitEndOfAudio(time));
    }
    IEnumerator WaitEndOfAudio(float time) 
    {
        yield return new WaitForSeconds(time);

        InUse = false;
        AudioSource.Stop();
        transform.parent = AudioParent;
    }

    public void Initialize()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.playOnAwake = false;
        AudioParent = transform.parent;
    }
}