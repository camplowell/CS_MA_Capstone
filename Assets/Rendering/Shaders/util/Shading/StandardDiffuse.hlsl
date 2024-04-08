#ifndef CAPSTONE_DIFFUSE
#define CAPSTONE_DIFFUSE

#include "Surface.hlsl"

float3 GetDiffuse(Surface surface, ShadingLight light) {
    return saturate(light.NoL) * light.color * light.attenuation;
}

#endif // End of File