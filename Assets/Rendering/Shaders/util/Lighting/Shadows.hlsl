#ifndef CAPSTONE_UTIL_SHADOWS
#define CAPSTONE_UTIL_SHADOWS

#include "Input.hlsl"
#include "../Shading/Surface.hlsl"
#include "../Dither.hlsl"

float4 _ShadowDistanceFade;

//uniform float _NormalBias;

float FadeShadowStrength(float distance, float scale, float fade) {
    return saturate((1.0 - distance * scale) * fade);
}

DirectionalShadowData GetDirectionalShadowData(int lightIndex, float depth) {
    DirectionalShadowData data;

	data.strength = _DirectionalLightShadowData[lightIndex].x 
        * FadeShadowStrength(depth,_ShadowDistanceFade.x, _ShadowDistanceFade.y);

	data.tileIndex = _DirectionalLightShadowData[lightIndex].y;
    
	return data;
}

float2 SampleShadowAtlas(float2 uv, float2 depth) {
    float2 atlasDepth = SAMPLE_TEXTURE2D(_DirectionalShadowAtlas, SHADOW_SAMPLER, uv).xy;
    return step(atlasDepth, depth);
}

//#define SAMPLE_DIRECTIONAL_SHADOW_ATLAS(uv, depth) step(SAMPLE_TEXTURE2D(_DirectionalShadowAtlas, SHADOW_SAMPLER, uv).xy, depth)
float SampleDirectionalShadowAtlas(float3 positionSTS, float inside) {
    float2 uv = positionSTS.xy;
    float2 depth = positionSTS.z * 0.5 + 0.5;
    
    float2 uvTexel = uv * _DirectionalShadowAtlas_TexelSize.zw;
    float2 uvFloor = floor(uvTexel) * _DirectionalShadowAtlas_TexelSize.xy;
    float2 uvFrac = frac(uvTexel);
    float3 offset = float3(_DirectionalShadowAtlas_TexelSize.xy, 0);
    float2 shadow =  lerp(
        lerp(SampleShadowAtlas(uv + offset.zz, depth),
             SampleShadowAtlas(uv + offset.xz, depth), uvFrac.x),
        lerp(SampleShadowAtlas(uv + offset.zy, depth),
             SampleShadowAtlas(uv + offset.xy, depth), uvFrac.x), uvFrac.y);

    return (inside * shadow.x + (1 - inside) * shadow.y);
}

float GetDirectionalShadowAttenuation(DirectionalShadowData data, float3 positionWS, float3 normalWS, float inside) {
#if _RECEIVE_SHADOWS
    if (data.strength <= 0.0) {
        return 1.0;
    }
    float3 positionSTS = mul(
		_DirectionalShadowMatrices[data.tileIndex],
		float4(positionWS, 1.0)
	).xyz;

	float shadow = SampleDirectionalShadowAtlas(positionSTS, inside);
	return lerp(1.0, shadow, data.strength);
#else
    return 1.0;
#endif
}

#endif // End of File