#ifndef CAPSTONE_UTIL_LIGHTING_CASTER
#define CAPSTONE_UTIL_LIGHTING_CASTER

#include "../Common.hlsl"
#include "Input.hlsl"
#include "../Transition.hlsl"

float3 ApplyCasterBias(float3 positionWS, float3 normalWS) {

#if _CASTING_DIRECTIONAL_LIGHT_SHADOW
    float3 viewDirWS = UNITY_MATRIX_V[2].xyz;
#else
    float3 viewDirWS = _WorldSpaceCameraPos - positionWS;
#endif

    return positionWS - (normalWS * _NormalBias) - (viewDirWS * _DepthBias);
}

float4 TransitionShadow(
    float4 positionCS, float3 worldPos, float3 viewDir,
    float showInside, float showOutside
) {

    float depthCS = positionCS.z / positionCS.w;
    float depthSS = depthCS; // * 0.5 + 0.5;
    
    return float4(showInside * depthSS, showOutside * depthCS, 0, 1);
}

#endif // End of File