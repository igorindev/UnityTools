using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] string audioReference;

    public void Play()
    {
        Audio.Play(audioReference);
    }
    public void PlayOneShot()
    {
        Audio.PlayOneShot(audioReference);
    }
}
