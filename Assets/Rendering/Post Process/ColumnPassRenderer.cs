using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ColumnPassRenderer : PostProcessEffectRenderer<ColumnPassSettings>
{
    public static float distortionMultiplier = 1;
    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Column"));
        sheet.properties.SetFloat("_Strength", distortionMultiplier * settings.distortionStrength / 100f);
        sheet.properties.SetFloat("_TimeScale", settings.speed);

        float size = Shader.GetGlobalFloat("_TransitionRadius") / settings.distortionSize;
        float baseY = 1 / (Mathf.PI * settings.distortionSize * settings.distortionStretch);
        // Octave; minimum of 0
        float octave = Mathf.Log(Mathf.Max(size, 1.0f / 4), 2);
        int floorOctave = Mathf.FloorToInt(octave);
        int nextOctave = floorOctave + 1;

        // Subdivisions X
        Vector4 scale = Vector4.one;

        scale.x = Mathf.Pow(2, Mathf.Max(floorOctave, 0));
        scale.z = Mathf.Pow(2, Mathf.Max(nextOctave, 0));

        scale.y = baseY * Mathf.Pow(2, Mathf.Max(settings.thinOctave - floorOctave, 0));
        scale.w = baseY * Mathf.Pow(2, Mathf.Max(settings.thinOctave - nextOctave, 0));

        sheet.properties.SetVector("_Scale", scale);

        sheet.properties.SetFloat("_ScaleMix", octave - floorOctave);

        sheet.properties.SetInteger("_SmallBuffers", Mathf.Clamp(settings.thinOctave - floorOctave, 0, 2));

        sheet.properties.SetFloat("_DensityFacing", Mathf.Lerp(settings.thinDensity, settings.densityFacing, size / Mathf.Pow(2, settings.thinOctave)));
        
        SetupCameraProps(context, sheet);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }

    void SetupCameraProps(PostProcessRenderContext context, PropertySheet sheet) {
        // NOTE: code was ported from: https://gamedev.stackexchange.com/questions/131978/shader-reconstructing-position-from-depth-in-vr-through-projection-matrix
        var camera = context.camera;
        var p = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false);
        p[2, 3] = p[3, 2] = 0.0f;
        p[3, 3] = 1.0f;
        var clipToWorld = Matrix4x4.Inverse(p * camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2,2]), Quaternion.identity, Vector3.one);
        sheet.properties.SetMatrix("_ClipToWorld", clipToWorld);
        sheet.properties.SetMatrix("_InverseView", camera.cameraToWorldMatrix);

        sheet.properties.SetVector("_CameraForward", camera.transform.forward);
    }
}
