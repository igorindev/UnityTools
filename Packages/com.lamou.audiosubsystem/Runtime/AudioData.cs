using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/Audio/Audio Data")]
public class AudioData : ScriptableObject
{
    [SerializeField] private AudioGroup audioGroup;
    [SerializeField] private AudioClip clip;

    [Space(5)]
    [Header("Volume")]
    [SerializeField][Range(0, 1)] private float volume = 1f;
    [SerializeField][AudioMinMaxSlider(0, 3)] private Vector2 pitchVariation = Vector2.one;

    [Header("Conditions")]
    [SerializeField] private bool loop;
    [SerializeField] private bool is2D;

    public AudioClip Clip => clip;
    public AudioGroup AudioGroup => audioGroup;
    public float Volume => volume;
    public float Pitch => GetRandomInRange(pitchVariation);
    public bool Loop => loop;
    public bool Is2D => is2D;

    public float GetRandomInRange(Vector2 range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }
}
