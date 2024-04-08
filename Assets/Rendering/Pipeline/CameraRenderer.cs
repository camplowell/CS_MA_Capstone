using UnityEngine;
using UnityEngine.Rendering;

public abstract class CameraRenderer {
    bool useDynamicBatching, useGPUInstancing;
    protected ScriptableRenderContext context;
    protected Camera camera;
    protected CullingResults cullingResults;
    protected SortingSettings sortingSettings;
    protected DrawingSettings drawingSettings;
    protected FilteringSettings filteringSettings;
    protected ShadowSettings shadowSettings;

    public void Render(
        ScriptableRenderContext context,
        Camera camera,
        bool useDynamicBatching,
        bool useGPUInstancing,
        ShadowSettings shadowSettings
    ) {
        this.context = context;
        this.camera = camera;
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        this.shadowSettings = shadowSettings;

        Prepare();

        if (!Cull()) return;

        Setup();

        Draw();
        DrawExtrasPreEffects();
        DrawEffects();
        DrawExtrasPostEffects();

        Submit();
    }
    protected virtual void Prepare() {}
    protected abstract void Setup();
    protected abstract void Draw();
    protected virtual void DrawExtrasPreEffects() {}
    protected virtual void DrawEffects() {}
    protected virtual void DrawExtrasPostEffects() {}
    protected abstract void Submit();

    protected bool Cull() {
        if (camera.TryGetCullingParameters(out var p)) {
            p.shadowDistance = Mathf.Min(shadowSettings.maxDistance, camera.farClipPlane);
            cullingResults = context.Cull(ref p);
            return true;
        }
        return false;
    }

    protected void SetupBuffer(
        CommandBuffer buffer, 
        string name,
        CameraClearFlags clearFlags
    ) {
        buffer.ClearRenderTarget(
            clearFlags <= CameraClearFlags.Depth,
            clearFlags == CameraClearFlags.Color,
            clearFlags == CameraClearFlags.Color ? 
                camera.backgroundColor.linear : Color.clear
        );
        buffer.BeginSample(name);
        Execute(buffer);
    }

    protected void Execute(CommandBuffer buffer) {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    protected void InitSettings(
        ShaderTagId[] shaderTags, 
        SortingCriteria sortingCriteria, 
        RenderQueueRange renderQueueRange
    ) {
        sortingSettings = new SortingSettings(camera) {
            criteria = sortingCriteria
        };
        drawingSettings = new DrawingSettings(
            shaderTags[0], sortingSettings
        ) {
            enableDynamicBatching = useDynamicBatching,
			enableInstancing = useGPUInstancing
        };
        for (int i = 1; i < shaderTags.Length; i++) {
            drawingSettings.SetShaderPassName(1, shaderTags[i]);
        }
        filteringSettings = new FilteringSettings(renderQueueRange);
    }

    protected void InitSettings(ShaderTagId[] shaderTags, SettingPreset preset) {
        switch(preset) {
            case SettingPreset.Opaque:
                InitSettings(shaderTags, SortingCriteria.CommonOpaque, RenderQueueRange.opaque);
                break;
            case SettingPreset.Transparent:
                InitSettings(shaderTags, SortingCriteria.CommonTransparent, RenderQueueRange.transparent);
                break;
            default:
                Debug.LogError("Unsupported Setting Preset: " + preset);
                break;
        }
    }

    protected void UpdateSettings(
        SortingCriteria sortingCriteria,
        RenderQueueRange renderQueueRange
    ) {
        sortingSettings.criteria = sortingCriteria;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = renderQueueRange;
    }

    protected void UpdateSettings(SettingPreset preset) {
        switch(preset) {
            case SettingPreset.Opaque:
                UpdateSettings(SortingCriteria.CommonOpaque, RenderQueueRange.opaque);
                break;
            case SettingPreset.Transparent:
                UpdateSettings(SortingCriteria.CommonTransparent, RenderQueueRange.transparent);
                break;
            default:
                Debug.LogError("Unsupported Setting Preset: " + preset);
                break;
        }
    }

    protected void DrawScene() {
        context.DrawRenderers(
            cullingResults,
            ref drawingSettings,
            ref filteringSettings
        );
    }

    protected enum SettingPreset {
        Opaque,
        Transparent
    }
}