using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using TMPro;

public class StatusProfiler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI profiler;

    float updateInterval = 1.0f;
    float lastInterval; // Last interval end time
    float frames = 0; // Frames over current interval

    float framesavtick = 0;
    float framesav = 0.0f;

    // Use this for initialization
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        framesav = 0;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        ++frames;

        var timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval)
        {
            float fps = frames / (timeNow - lastInterval);
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

            ++framesavtick;
            framesav += fps;
            float fpsav = framesav / framesavtick;

            profiler.text = "Time : " + ms.ToString("f1") + "ms   " + "Current FPS : " + fps.ToString("f2") + "   AvgFps : " + fpsav.ToString("f2") +
            '\n';
            profiler.text += '\n' + "GPU memory : " + SystemInfo.graphicsMemorySize + "   Sys Memory : " + SystemInfo.systemMemorySize;

            profiler.text += '\n' + "TotalAllocatedMemory : " + Profiler.GetTotalAllocatedMemoryLong() / 1048576 + "mb" + "   " +
                "TotalReservedMemory : " + Profiler.GetTotalReservedMemoryLong() / 1048576 + "mb" + "   " +
                "TotalUnusedReservedMemory : " + Profiler.GetTotalUnusedReservedMemoryLong() / 1048576 + "mb";
            
            #if UNITY_EDITOR
            profiler.text += "\nDrawCalls : " + UnityStats.drawCalls +
            '\n' +
            "Used Texture Memory : " + UnityStats.usedTextureMemorySize / 1024 / 1024 + "mb" +
            '\n' +
            "RenderedTextureCount : " + UnityStats.usedTextureCount;
            #endif

            frames = 0;
            lastInterval = timeNow;
        }

    }
}


