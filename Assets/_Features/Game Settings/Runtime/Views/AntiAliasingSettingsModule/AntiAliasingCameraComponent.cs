using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class AntiAliasingCameraComponent : MonoBehaviour
{
    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        UniversalAdditionalCameraData additionalCameraData = cam.GetUniversalAdditionalCameraData();
        UniversalRenderPipelineAsset renderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

        //Get defined infos from video settings
        AntiAliasing aa = VideoSettings.Get<AntiAliasing>();

        //int msaaSmaples = 1;
        AntialiasingMode antialiasingMode = aa.GetAAMode();
        AntialiasingQuality antialiasingQuality = aa.GetSMAAQuality();
        TemporalAAQuality taaQuality = aa.GetTAAQuality();
        float taaSharpening = aa.GetTAASharpening();

        additionalCameraData.antialiasing = antialiasingMode;
        additionalCameraData.antialiasingQuality = antialiasingQuality;

        if (antialiasingMode == AntialiasingMode.TemporalAntiAliasing)
        {
            //TAA - Quality: 0, 1, 2, 3, 4 - Contrast = 0 - 1
            additionalCameraData.taaSettings.quality = taaQuality;
            additionalCameraData.taaSettings.contrastAdaptiveSharpening = taaSharpening;

            //msaaSmaples = 1;
        }

        //cam.allowMSAA = msaaSmaples > 1;
        //renderPipelineAsset.msaaSampleCount = msaaSmaples;
    }
}
