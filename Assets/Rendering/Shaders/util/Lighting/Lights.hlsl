#ifndef CAPSTONE_UTIL_LIGHTS
#define CAPSTONE_UTIL_LIGHTS

#include "Input.hlsl"
#include "Shadows.hlsl"

Light GetDirectionalLight(
    int index, 
    float3 positionWS, 
    float3 normalWS,
    float depth,
    float inside
) {
    Light light;

    light.color = _DirectionalLightColors[index].rgb;
    light.direction = _DirectionalLightDirections[index].xyz;
    DirectionalShadowData shadowData = GetDirectionalShadowData(index, depth);
    light.attenuation = GetDirectionalShadowAttenuation(shadowData, positionWS, normalWS, inside);

    return light;
}

#endif // End of File