using System.Collections.Generic;
using System.Text;
using UnityEngine.LowLevel;

#if UNITY_EDITOR
#endif

namespace PlayerLoopInjectionSystem
{
    public class PlayerLoopUtility
    {
        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem toInsert, int index)
        {
            if (loop.type != typeof(T))
            {
                return HandleSubSystemLoop<T>(ref loop, toInsert, index);
            }

            var playerLoopSystemList = new List<PlayerLoopSystem>();
            if (loop.subSystemList != null)
            {
                playerLoopSystemList.AddRange(loop.subSystemList);
            }

            playerLoopSystemList.Insert(index, toInsert);

            loop.subSystemList = playerLoopSystemList.ToArray();

            return true;
        }

        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem toRemove)
        {
            if (loop.subSystemList == null)
            {
                return;
            }

            var playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (int i = 0; i < playerLoopSystemList.Count; i++)
            {
                if (playerLoopSystemList[i].type == toRemove.type && playerLoopSystemList[i].updateDelegate == toRemove.updateDelegate)
                {
                    playerLoopSystemList.RemoveAt(i);
                    loop.subSystemList = playerLoopSystemList.ToArray();
                }
            }

            HandleSubSystemLoopForRemoval<T>(ref loop, toRemove);
        }

        private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, PlayerLoopSystem toRemove)
        {
            if (loop.subSystemList == null)
            {
                return;
            }

            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                RemoveSystem<T>(ref loop.subSystemList[i], toRemove);
            }
        }

        private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, PlayerLoopSystem toInsert, int index)
        {
            if (loop.subSystemList == null)
            {
                return false;
            }

            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                if (!InsertSystem<T>(ref loop.subSystemList[i], in toInsert, index))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public static void PrintPlayerLoop(PlayerLoopSystem loop)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unity Player Loop");
            foreach (PlayerLoopSystem subSystem in loop.subSystemList)
            {
                PrintSubsystem(subSystem, stringBuilder, 0);
            }
        }

        private static void PrintSubsystem(PlayerLoopSystem system, StringBuilder stringBuilder, int level)
        {
            stringBuilder.Append(' ', level * 2).AppendLine(system.type.ToString());
            if (system.subSystemList == null || system.subSystemList.Length == 0)
            {
                return;
            }

            foreach (PlayerLoopSystem subSystem in system.subSystemList)
            {
                PrintSubsystem(subSystem, stringBuilder, level + 1);
            }
        }
    }
}
