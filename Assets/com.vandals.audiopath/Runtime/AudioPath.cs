using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class AudioPath : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] SplineContainer splineContainer;

    [Header("Spline")]
    [SerializeField] int pathResolution = 4;

    Transform m_AudioSourceTransform;
    Transform m_Camera;

    static AudioSource mainSource;
    static readonly List<AudioSource> audioSources = new List<AudioSource>();

    void Start()
    {
        m_AudioSourceTransform = audioSource.transform;

        if (mainSource)
            audioSources.Add(audioSource);
        else
            mainSource = audioSource;
    }

    void OnDestroy()
    {
        if (mainSource == audioSource)
            mainSource = null;
        else
            audioSources.Remove(audioSource);
    }

    void Update()
    {
        if (Time.frameCount % 5 == 0)
        {
            if (mainSource == audioSource)
                SyncSources();

            Transform cam = GetCam();

            if (cam)
            {
                using NativeSpline native = new NativeSpline(splineContainer.Spline, splineContainer.transform.localToWorldMatrix);
                SplineUtility.GetNearestPoint(native, cam.position, out float3 nearest, out float normalizedTime, pathResolution);

                m_AudioSourceTransform.position = nearest;
            }
        }
    }

    Transform GetCam()
    {
        if (m_Camera)
            return m_Camera;
        else
        {
            if (Camera.main)
                return m_Camera = Camera.main.transform;
        }

        return null;
    }

    void SyncSources()
    {
        foreach (AudioSource slave in audioSources)
        {
            slave.timeSamples = mainSource.timeSamples;
        }
    }
}
