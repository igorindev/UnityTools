using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAudio : MonoBehaviour
{
    public int band;
    public float minIntensity;
    public float maxIntensity;
    public Light lightRef;

    // Update is called once per frame
    void Update()
    {
        lightRef.intensity = (AudioVisualizer.audioBandBuffer[band] * (maxIntensity - minIntensity)) + minIntensity;
    }
}
