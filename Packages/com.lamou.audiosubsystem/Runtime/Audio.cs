using System;
using System.Text;
using UnityEngine;

namespace AudioSubsystem
{
    public static class Audio
    {
        private static AudioSubsystem AudioSubsystem;

        public static AudioPlayer Play(string clipName, uint layer = 0, int listenPriority = 0)
        {
            return AudioSubsystem.Play(clipName, layer, listenPriority);
        }

        public static AudioPlayer PlayAtPosition(string clipName, Vector3 worldPosition, uint layer = 0, int listenPriority = 0)
        {
            return AudioSubsystem.PlayAtPosition(clipName, worldPosition, null, layer, listenPriority);
        }

        public static AudioPlayer PlayAttached(string clipName, Transform parent, Vector3 localPosition = default, uint layer = 0, int listenPriority = 0)
        {
            return AudioSubsystem.PlayAtPosition(clipName, localPosition, parent, layer, listenPriority);
        }

        public static void Stop(AudioPlayer audioPlayer)
        {
            AudioSubsystem.Stop(audioPlayer);
        }

        public static void PauseGroup(string groupName)
        {
            AudioSubsystem.PauseGroup(groupName);
        }

        public static void UnPauseGroup(string groupName)
        {
            AudioSubsystem.UnPauseGroup(groupName);
        }

        public static uint StringToHash(string value)
        {
            return StringToCrc32Converter.StringToCrc32(value);
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
}