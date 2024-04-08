using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Capstone Render Pipeline")]
public class CapstonePipeline : RenderPipelineAsset
{
    public static Pipeline activeInstance;
    public EnvironmentSettings environment = default;
    public ShadowSettings shadow = default;
    public BatchingSettings batching = default;

    protected override RenderPipeline CreatePipeline() {
        return new CapstonePipeline.Pipeline(
            environment,
            shadow,
            batching
        );
    }

    public class Pipeline : RenderPipeline {
        bool useDynamicBatching, useGPUInstancing;
        private SceneCameraRenderer cameraRenderer = new SceneCameraRenderer();
        public ShadowSettings shadow;
        public EnvironmentSettings environment;

        public Pipeline(
            EnvironmentSettings environmentSettings,
            ShadowSettings shadowSettings,
            BatchingSettings batchingSettings
        ) {
            CapstonePipeline.activeInstance = this;
            this.useDynamicBatching = batchingSettings.useDynamicBatching;
            this.useGPUInstancing = batchingSettings.useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = batchingSettings.useSRPBatcher;
            GraphicsSettings.lightsUseLinearIntensity = true;
            this.shadow = shadowSettings;
            this.environment = environmentSettings;
        }
        protected override void Render(
            ScriptableRenderContext context,
            Camera[] cameras
        ) {
            CapstonePipeline.activeInstance = this;
            SetGlobals();

            foreach (Camera camera in cameras) {
                cameraRenderer.Render(
                    context, 
                    camera, 
                    useDynamicBatching, 
                    useGPUInstancing,
                    shadow
                );
            }
        }

        protected void SetGlobals() {
            Shader.SetGlobalFloat("_DepthBias", shadow.depthBias * 0.1f);
            Shader.SetGlobalFloat("_NormalBias", shadow.normalBias * 0.01f);
            Shader.SetGlobalColor("_AmbientLight", environment.ambientLight.linear);
            
            EnvironmentSettings.TransitionSettings transitionSettings = environment.transition;
            Shader.SetGlobalVector("_TransitionPos", transitionSettings.position);
            Shader.SetGlobalFloat("_TransitionRadius", transitionSettings.radius);
            Shader.SetGlobalColor("_TransitionGlow", transitionSettings.glow.linear);
            Shader.SetGlobalFloat("_TransitionExtent", transitionSettings.glowExtent);
            float falloff2 = transitionSettings.glowFalloff * transitionSettings.glowFalloff;
            Shader.SetGlobalFloat("_TransitionFalloff", falloff2);

        }
    }
}