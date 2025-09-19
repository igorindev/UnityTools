using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlayerLoopInjectionSystem
{
    internal static class TimerLoop
    {
        private static PlayerLoopSystem timerLoopSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            timerLoopSystem = new PlayerLoopSystem()
            {
                type = typeof(TimerManager),
                updateDelegate = TimerManager.UpdateTimers,
                subSystemList = null
            };

            if (!PlayerLoopUtility.InsertSystem<Update>(ref currentPlayerLoop, in timerLoopSystem, 0))
            {
                Debug.LogError("Error inserting Timer Manger to the player loop");
                return;
            }

            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            PlayerLoopUtility.PrintPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
#endif
        }

        private static void OnPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ResetAndClearEditorPlayerLoop();
            }
        }

        private static void ResetAndClearEditorPlayerLoop()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopUtility.RemoveSystem<Update>(ref currentPlayerLoop, in timerLoopSystem);
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);

            TimerManager.Clear();
        }
    }
}
