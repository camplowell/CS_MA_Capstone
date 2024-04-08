#ifndef CAPSTONE_UTIL_SHADING_FRESNEL
#define CAPSTONE_UTIL_SHADING_FRESNEL

#include "Surface.hlsl"

float SchlickFresnel(float i) {
    float x = saturate(1.0 - i);
    float x2 = x * x;
    return x2 * x2 * x;
}

void ApplyFresnel(inout Surface surface, float LdotH) {
    float3 f0 = surface.specular;
    surface.specular = f0 + (1 - f0) * SchlickFresnel(LdotH);
}

float3 GetMixFac(Surface surface, ShadingLight light) {
    float3 f0 = surface.specular;
    return f0 + (1 - f0) * SchlickFresnel(light.LoH);
}

#endif // End of File