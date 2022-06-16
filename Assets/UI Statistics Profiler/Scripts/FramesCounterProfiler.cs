using System.Text;
using TMPro;
using UnityEngine;

public class FramesCounterProfiler : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI realtimeFpsText;
    [SerializeField] TextMeshProUGUI highFpsText;
    [SerializeField] TextMeshProUGUI avgFpsText;
    [SerializeField] TextMeshProUGUI lowFpsText;

    [Header("Update")]
    [SerializeField, Min(0)] float updateInterval = 0.5f;
    [SerializeField, Min(1)] int frameRange = 60;

    float timeleft; // Left time for current intervalte FPSColor[] coloring; over the interval
    int frames = 0; // Frames drawn over the interval
    float accum;

    int[] fpsBuffer;
    int fpsBufferIndex;

    int FPS;
    int AverageFPS;
    int HighestFPS;
    int LowestFPS;
    float ms;

    StringBuilder sb = new StringBuilder(60, 60);

    void Update()
    {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFPS();

        DisplayFps();
    }
    void InitializeBuffer()
    {
        if (frameRange <= 0)
        {
            frameRange = 1;
        }
        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
    }
    void UpdateBuffer()
    {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
        {
            fpsBufferIndex = 0;
        }
    }
    void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        for (int i = 0; i < frameRange; i++)
        {
            int fps = fpsBuffer[i];
            sum += fps;
            if (fps > highest)
            {
                highest = fps;
            }
            if (fps < lowest)
            {
                lowest = fps;
            }
        }
        AverageFPS = sum / frameRange;
        HighestFPS = highest;
        LowestFPS = lowest;
        ms = 1000.0f / Mathf.Max(FPS, 0.00001f);
    }
    void DisplayFps()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        FPS = (int)(accum / frames);

        if (timeleft <= 0.0)
        {
            Display(highFpsText, HighestFPS, "HGH:\n");
            Display(avgFpsText, AverageFPS, "AVG: ");
            Display(lowFpsText, LowestFPS, "LOW:\n");
            Display(realtimeFpsText, FPS, "");

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    void Display(TextMeshProUGUI label, int fps, string prefix)
    {
        sb.Clear();
        if (fps < 30)
        {
            sb.Append($"{prefix} <color=yellow>{fps} FPS</color>");
        }
        else
        {
            if (fps < 10)
            {
                sb.Append($"{prefix} <color=red>{fps} FPS</color>");
            }
            else
            {
                sb.Append($"{prefix} <color=white>{fps} FPS</color>");
            }
        }
        if (prefix == "")
        {
            sb.Append($" ({ms:f1} ms)");
        }

        label.text = sb.ToString();
    }
}
