using UnityEngine;
using UnityEngine.Rendering;


public class Shadows {
    const string bufferName = "Shadows";
    CommandBuffer buffer = new CommandBuffer {
		name = bufferName
	};
	ShadowSettings settings;

    // Directional Shadows
    static int 
        dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas"),
        dirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices"),
        shadowDistanceFadeId = Shader.PropertyToID("_ShadowDistanceFade"); 
    const int maxDirectionalShadowCount = 1;
    ShadowedDirectionalLight[] directionalLights = 
        new ShadowedDirectionalLight[maxDirectionalShadowCount];
    Matrix4x4[] dirShadowMatrices = new Matrix4x4[maxDirectionalShadowCount];
    int directionalShadowCount;

    // Drawcall-specific
	ScriptableRenderContext context;
	CullingResults cullingResults;

	public void Setup (
		ScriptableRenderContext context, CullingResults cullingResults,
		ShadowSettings settings
	) {
		this.context = context;
		this.cullingResults = cullingResults;
		this.settings = settings;
        this.directionalShadowCount = 0;
	}

    public Vector2 ReserveDirectionalShadows(Light light, int visibleLightIndex) {
        if (
            directionalShadowCount < maxDirectionalShadowCount
            && light.shadows != LightShadows.None && light.shadowStrength > 0f
            && cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b)
        ) {
            directionalLights[directionalShadowCount] = 
                new ShadowedDirectionalLight { visibleLightIndex = visibleLightIndex };
            return new Vector2(
                light.shadowStrength, directionalShadowCount++
            );
        }
        return Vector2.zero;
    }

    public void Render() {
        RenderDirectionalShadows();
    }

    void RenderDirectionalShadows() {
        if (directionalShadowCount == 0) {
            buffer.GetTemporaryRT(
                dirShadowAtlasId, 1, 1,
                32, FilterMode.Bilinear, RenderTextureFormat.RGFloat
            );
            return;
        }

        int atlasSize = (int)settings.directional.atlasSize;
        buffer.GetTemporaryRT(
            dirShadowAtlasId, atlasSize, atlasSize,
            32, FilterMode.Bilinear, RenderTextureFormat.RGFloat
        );
        
        buffer.SetRenderTarget(
            dirShadowAtlasId,
            RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
        );
        buffer.ClearRenderTarget(true, true, Color.clear);

        buffer.BeginSample(bufferName);
        ExecuteBuffer();

        buffer.SetGlobalVector(shadowDistanceFadeId, new Vector2(1f / settings.maxDistance, 1f / settings.distanceFade));

        int split = directionalShadowCount <= 1 ? 1 
            : (Mathf.CeilToInt(Mathf.Log(directionalShadowCount, 2)) + 1);
        int tileSize = atlasSize / split;
        for (int i = 0; i < directionalShadowCount; i++) {
            RenderDirectionalShadow(i, split, tileSize);
        }
        buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);

        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }

    void RenderDirectionalShadow(int index, int split, int tileSize) {
        ShadowedDirectionalLight light = directionalLights[index];
        var shadowSettings = 
            new ShadowDrawingSettings(cullingResults, light.visibleLightIndex);
        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(
            light.visibleLightIndex, 0, 1, Vector3.zero,
            tileSize, 0f,
            out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData splitData
        );

        shadowSettings.splitData = splitData;

        dirShadowMatrices[index] = ConvertToAtlasMatrix(
            projectionMatrix * viewMatrix,
            SetTileViewport(index, split, tileSize), split
        );

        buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
        CoreUtils.SetKeyword(buffer, "_CASTING_DIRECTIONAL_LIGHT_SHADOW", true);
        ExecuteBuffer();

        context.DrawShadows(ref shadowSettings);
    }

    Matrix4x4 ConvertToAtlasMatrix(Matrix4x4 mat, Vector2 offset, int split) {
        if (SystemInfo.usesReversedZBuffer) {
            mat.m20 = -mat.m20;
			mat.m21 = -mat.m21;
			mat.m22 = -mat.m22;
			mat.m23 = -mat.m23;
        }
        float scale = 1f / split;
		mat.m00 = (0.5f * (mat.m00 + mat.m30) + offset.x * mat.m30) * scale;
		mat.m01 = (0.5f * (mat.m01 + mat.m31) + offset.x * mat.m31) * scale;
		mat.m02 = (0.5f * (mat.m02 + mat.m32) + offset.x * mat.m32) * scale;
		mat.m03 = (0.5f * (mat.m03 + mat.m33) + offset.x * mat.m33) * scale;
		mat.m10 = (0.5f * (mat.m10 + mat.m30) + offset.y * mat.m30) * scale;
		mat.m11 = (0.5f * (mat.m11 + mat.m31) + offset.y * mat.m31) * scale;
		mat.m12 = (0.5f * (mat.m12 + mat.m32) + offset.y * mat.m32) * scale;
		mat.m13 = (0.5f * (mat.m13 + mat.m33) + offset.y * mat.m33) * scale;
        return mat;
    }

    Vector2 SetTileViewport(int index, int split, float tileSize) {
        Vector2 offset = new Vector2(index % split, index / split);
        buffer.SetViewport(new Rect(
            offset.x * tileSize, offset.y * tileSize, tileSize, tileSize
        ));
        return offset;
    }

    public void Cleanup() {
		buffer.ReleaseTemporaryRT(dirShadowAtlasId);
		ExecuteBuffer();
    }

	void ExecuteBuffer () {
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}

    struct ShadowedDirectionalLight {
        public int visibleLightIndex;
    }
}