// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
TEXTURE2D_SAMPLER2D(_CameraDepthMap, sampler_CameraDepthMap);

float4x4 _ClipToWorld;
float4x4 _InverseView;
float3 _CameraForward;

float _TimeScale;
float4 _Scale;
float _ScaleMix;
int _SmallBuffers;
float _DensityFacing;

float _Strength;

struct Attributes
{
    float3 vertex : POSITION;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 texcoord : TEXCOORD0;

    float3 nearPlaneRWS : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
};

Varyings Vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionCS = float4(IN.vertex.xy, 0.0, 1.0);
    OUT.texcoord = TransformTriangleVertexToUV(IN.vertex.xy);

    if (unity_OrthoParams.w == 0) {
        float4 clip = float4((OUT.texcoord.xy * 2.0f - 1.0f) * float2(1, -1), 0.0f, 1.0f);
        OUT.nearPlaneRWS = (mul(_ClipToWorld, clip) -_WorldSpaceCameraPos).xyz;
    } else {
        OUT.nearPlaneRWS = _CameraForward;
    }

#if UNITY_UV_STARTS_AT_TOP
    OUT.texcoord = OUT.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif

    return OUT;
}

float3 GetPositionWS(float2 texcoord, float3 nearPlaneRWS) {
    const float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    const float2 p13_31 = float2(unity_CameraProjection._13, unity_CameraProjection._23);
    const float near = _ProjectionParams.y;
    const float far = _ProjectionParams.z;

    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthMap, sampler_CameraDepthMap, texcoord);

    if (unity_OrthoParams.w == 0) {
        depth = LinearEyeDepth(depth);
        return nearPlaneRWS * depth + _WorldSpaceCameraPos;
    } else {

#if defined(UNITY_REVERSED_Z)
        depth = 1 - depth;
#endif
        float zOrtho = lerp(near, far, depth);
        float3 vPos = float3((texcoord * 2 - 1 - p13_31) / p11_22, -zOrtho);
        return mul(_InverseView, float4(vPos, 1)).xyz;
    }
}

float3 GetOriginWS(float2 texcoord) {
    const float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    const float2 p13_31 = float2(unity_CameraProjection._13, unity_CameraProjection._23);
    const float near = _ProjectionParams.y;
    const float far = _ProjectionParams.z;

    if (unity_OrthoParams.w == 0) {
        return _WorldSpaceCameraPos;
    } else {
        float3 vPos = float3((texcoord * 2 - 1 - p13_31) / p11_22, -near);
        return mul(_InverseView, float4(vPos, 1)).xyz;
    }
}

float GetActualDepth(float2 texcoord) {
    const float near = _ProjectionParams.y;
    const float far = _ProjectionParams.z;

    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthMap, sampler_CameraDepthMap, texcoord);
    if (unity_OrthoParams.w == 0) {
        return LinearEyeDepth(depth);
    } else {
#if defined(UNITY_REVERSED_Z)
        depth = 1 - depth;
#endif
        return depth * (far - near);
    }
}

#include "./ColumnHit.hlsl"
#include "../util/NoisyNodes/ClassicNoise3D.hlsl"
#include "../util/NoisyNodes/ClassicNoise2D.hlsl"

float4 Frag(Varyings IN) : SV_Target
{
    float3 originWS = GetOriginWS(IN.texcoord);
    float z_depth = GetActualDepth(IN.texcoord);
    float3 incoming = IN.nearPlaneRWS;

    float3 hitPos;
    float2 hitNormal;
    float hitDot;
    [branch] if (!TransitionHit(originWS, IN.nearPlaneRWS, z_depth, hitPos, hitNormal, hitDot)) {
        return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
    }

    float3 hitUV = float3(
        atan(hitNormal.x / hitNormal.y) / 3.14159265359 + 0.5,
        hitPos.y,
        _Time.x
    );
    
    float fresnel = 1 - hitDot;
    fresnel = fresnel * fresnel;
    float density_fresnel = lerp(_DensityFacing, 1, fresnel);

    float3 scaledUV = hitUV * float3(_Scale.xy, _TimeScale);
    float density;
    float noise;

    if (_SmallBuffers > 0) {
        PerlinNoise2D_float(scaledUV.yz, noise);
        density = _DensityFacing;
    } else {
        PerlinNoise3DPeriodic_float(scaledUV, float3(_Scale.x, 128, 128), noise);
        density = density_fresnel;
    }
    
    scaledUV = hitUV * float3(_Scale.zw, _TimeScale);
    float noise_larger;
    float density_larger;
    if (_SmallBuffers > 1) {
        PerlinNoise2D_float(scaledUV.yz, noise_larger);
        density_larger = _DensityFacing;
    } else {
        PerlinNoise3DPeriodic_float(scaledUV, float3(_Scale.z, 128, 128), noise_larger);
        density_larger = density_fresnel;
    }
    noise = lerp(noise, noise_larger, _ScaleMix);
    density = lerp(density, density_larger, _ScaleMix);
    
    float2 refraction = _Strength * noise;
    refraction.x *= hitDot;
    refraction += IN.texcoord;

    float refractedDepth = GetActualDepth(refraction);
    if (refractedDepth < z_depth) refraction = IN.texcoord;
    
    noise = step(noise * 0.5 + 0.5, density);

    float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, refraction);
    sceneColor.rgb = max(sceneColor.rgb, _TransitionGlow * noise);

    return sceneColor;
}