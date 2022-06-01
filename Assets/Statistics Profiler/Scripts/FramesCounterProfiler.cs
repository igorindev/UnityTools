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
    
    public int FPS { get; private set; }
    public int AverageFPS { get; private set; }
    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }

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
    }

    void Display(TextMeshProUGUI label, int fps, string prefix)
    {
        if (fps < 30)
        {
            label.text = prefix + "<color=yellow>" + fps + " FPS </color>";
        }
        else
        {
            if (fps < 10)
            {
                label.text = prefix + "<color=red>" + fps + " FPS </color>";
            }
            else
            {
                label.text = prefix + "<color=white>" + fps + " FPS </color>";
            }
        }
    }
}
