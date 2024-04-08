#ifndef CAPSTONE_UTIL_SHADING_SURFACE
#define CAPSTONE_UTIL_SHADING_SURFACE

struct Surface {
    float3 positionWS;
    float3 normal;
    float3 incoming;
    float depth;
    float2 pixelPos;
    
    float3 albedo;
    float3 specular;
    float  roughness;
    float3 emission;
    float  alpha;
};

struct ShadingLight {
    float NoV;
    float NoL;
    float NoH;
    float LoH;

    float3 color;
    float3 direction;
    float attenuation;
};

#include "../Lighting/Lights.hlsl"

ShadingLight MakeShadingLight(int lightIndex, Surface surface, float inside) {
    Light light = GetDirectionalLight(
        lightIndex, 
        surface.positionWS, 
        surface.normal,
        surface.depth,
        inside
    );
    ShadingLight OUT;
    float3 H = light.direction + surface.incoming;
    H /= max(0.00001, length(H));

    OUT.NoV = saturate(dot(surface.normal, surface.incoming));
    OUT.NoL = saturate(dot(surface.normal, light.direction));
    OUT.NoH = saturate(dot(surface.normal, H));
    OUT.LoH = saturate(dot(light.direction, H));

    OUT.color = light.color;
    OUT.direction = light.direction;
    OUT.attenuation = light.attenuation;

    return OUT;
}

#endif // End of File