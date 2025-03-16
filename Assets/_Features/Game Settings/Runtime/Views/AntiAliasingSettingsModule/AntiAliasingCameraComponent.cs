using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class AntiAliasingCameraComponent : MonoBehaviour
{
    private AntiAliasing _aa;

    private Camera _camera;
    private UniversalAdditionalCameraData _cameraData;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _cameraData = _camera.GetUniversalAdditionalCameraData();
        //UniversalRenderPipelineAsset renderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

        _aa = VideoSettings.Get<AntiAliasing>();
        _aa.AddCamera(_camera);

        ApplyAA();
    }

    private void OnDestroy()
    {
        _aa.RemoveCamera(_camera);
    }

    private void ApplyAA()
    {
        //int msaaSmaples = 1;
        AntialiasingMode antialiasingMode = _aa.GetAAMode();
        AntialiasingQuality antialiasingQuality = _aa.GetSMAAQuality();
        TemporalAAQuality taaQuality = _aa.GetTAAQuality();
        float taaSharpening = _aa.GetTAASharpening();

        _cameraData.antialiasing = antialiasingMode;
        _cameraData.antialiasingQuality = antialiasingQuality;

        if (antialiasingMode == AntialiasingMode.TemporalAntiAliasing)
        {
            //TAA - Quality: 0, 1, 2, 3, 4 - Contrast = 0 - 1
            _cameraData.taaSettings.quality = taaQuality;
            _cameraData.taaSettings.contrastAdaptiveSharpening = taaSharpening;

            //msaaSmaples = 1;
        }

        //cam.allowMSAA = msaaSmaples > 1;
        //renderPipelineAsset.msaaSampleCount = msaaSmaples;
    }
}
