using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class AntiAliasingCameraComponent : MonoBehaviour
{
    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        UniversalAdditionalCameraData additionalCameraData = cam.GetUniversalAdditionalCameraData();
        UniversalRenderPipelineAsset renderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        //Get defined infos from video settings

        int msaaSmaples = 1;
        AntialiasingMode antialiasingMode = (AntialiasingMode)1;
        AntialiasingQuality antialiasingQuality = (AntialiasingQuality)1;
        TemporalAAQuality taaQuality = (TemporalAAQuality)1;
        int taaSharpening = 1;

        additionalCameraData.antialiasing = antialiasingMode;
        additionalCameraData.antialiasingQuality = antialiasingQuality;

        if (antialiasingMode == AntialiasingMode.TemporalAntiAliasing)
        {
            //TAA - Quality: 0, 1, 2, 3, 4 - Contrast = 0 - 1
            additionalCameraData.taaSettings.quality = taaQuality;
            additionalCameraData.taaSettings.contrastAdaptiveSharpening = taaSharpening;

            msaaSmaples = 1;
        }

        cam.allowMSAA = msaaSmaples > 1;
        renderPipelineAsset.msaaSampleCount = msaaSmaples;
    }
}
