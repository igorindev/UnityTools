using System;
using UnityEngine;

public static class Audio 
{
    private static AudioSubsystem AudioSubsystem;

    public static AudioPlayer Play(string clipName) 
    {
        return AudioSubsystem.Play(clipName);
    }

    public static AudioPlayer PlayAtPosition(string clipName, Vector3 worldPosition) 
    {
        return AudioSubsystem.PlayAtPosition(clipName, worldPosition, null);
    }

    public static AudioPlayer PlayAttached(string clipName, Transform parent, Vector3 localPosition = default)
    {
        return AudioSubsystem.PlayAtPosition(clipName, localPosition, parent);
    }

    public static void Stop(AudioPlayer audioPlayer)
    {
        AudioSubsystem.Stop(audioPlayer);
    }

    internal static bool RegisterAudioSubsystem(AudioSubsystem audioSubsystem)
    {
        if (AudioSubsystem)
        {
            return false;
        }

        AudioSubsystem = audioSubsystem;
        return true;
    }
}