using UnityEngine;

public static class Audio 
{
    public static AudioSystem audioSystem;

    public static void Play(string clipName) 
    {
        if (audioSystem != null && clipName != "") audioSystem.Play(clipName);
    }
    public static void PlayAtPosition(string clipName, Vector3 position, Transform parent = null) 
    {
        if (audioSystem != null && clipName != "") audioSystem.PlayAtPosition(clipName, position, parent);
    }
    public static void PlayMusic(string musicName) 
    {
        if (audioSystem != null && musicName != "") audioSystem.PlayMusic(musicName);
    }
}