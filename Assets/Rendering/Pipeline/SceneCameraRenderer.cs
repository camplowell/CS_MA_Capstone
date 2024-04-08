using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public partial class SceneCameraRenderer : CameraRenderer {
    static ShaderTagId[] supportedTags = {
        new ShaderTagId("CapstoneUnlit"),
        new ShaderTagId("CapstoneLit"),
        new ShaderTagId("CapstoneTransparent")
    };
    string bufferName = "Render Scene";
    CommandBuffer buffer = new CommandBuffer();

    Lighting lighting = new Lighting();

    static int renderTargetId = Shader.PropertyToID("_CameraColorMap");
    static RenderTargetIdentifier renderTargetIdendifier = new RenderTargetIdentifier(renderTargetId);
    static int depthTargetId = Shader.PropertyToID("_CameraDepthMap");
    static RenderTargetIdentifier depthTargetIdendifier = new RenderTargetIdentifier(depthTargetId);

    PostProcessLayer postProcessLayer;
    PostProcessRenderContext postProcess;

    protected override void Setup() {
        context.SetupCameraProperties(camera);
        SetupLighting();

        context.SetupCameraProperties(camera);


        postProcessLayer = camera.GetComponent<PostProcessLayer>();
        if (RuntimeUtilities.IsPostProcessingActive(postProcessLayer)) {
            postProcessLayer.UpdateVolumeSystem(camera, buffer);
            postProcess = new PostProcessRenderContext();
        }

        buffer.GetTemporaryRT(
            renderTargetId, camera.pixelWidth, camera.pixelHeight,
            24, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf
        );
        buffer.GetTemporaryRT(
            depthTargetId, camera.pixelWidth, camera.pixelHeight,
            24, FilterMode.Bilinear, RenderTextureFormat.Depth
        );
        buffer.SetRenderTarget(
            renderTargetId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
            depthTargetId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
        );

        SetupBuffer(buffer, bufferName, camera.clearFlags);
    }

    void SetupLighting() {
        buffer.BeginSample(bufferName);
        Execute(buffer);

        lighting.Setup(context, cullingResults, shadowSettings);

        buffer.EndSample(bufferName);
    }

    protected override void Draw() {
        InitSettings(supportedTags, SortingCriteria.CommonOpaque, RenderQueueRange.opaque);
        DrawScene();

        context.DrawSkybox(camera);

        UpdateSettings(SortingCriteria.CommonTransparent, RenderQueueRange.transparent);
        DrawScene();
        Execute(buffer);
    }

    protected override void DrawEffects()
    {
        if (
            RuntimeUtilities.IsPostProcessingActive(postProcessLayer)
        ) {
            RenderPostProcessing();
        }
    }

    protected void RenderPostProcessing() {

        postProcess.Reset();
        postProcess.camera = camera;
        postProcess.source = renderTargetIdendifier;
        postProcess.sourceFormat = RenderTextureFormat.ARGBHalf;
        postProcess.destination = new RenderTargetIdentifier(camera.targetTexture);
        postProcess.command = buffer;
        postProcess.flip = true;
        
        postProcessLayer.Render(postProcess);

        Execute(buffer);
    }

    protected override void Submit() {
        lighting.Cleanup();
        buffer.ReleaseTemporaryRT(renderTargetId);
        buffer.ReleaseTemporaryRT(depthTargetId);

        buffer.EndSample(bufferName);
         
        Execute(buffer);

        context.Submit();
    }



}