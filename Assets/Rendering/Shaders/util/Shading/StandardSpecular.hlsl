#ifndef CAPSTONE_SPECULAR
#define CAPSTONE_SPECULAR

// Physically-based GGX specular highlights
//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/BSDF.hlsl"

// Reference: Filament https://google.github.io/filament/Filament.html#materialsystem/specularbrdf

// GGX normal distribution
float D_GGX(float NoH, float roughness) {
    float a = NoH * roughness;
    float k = roughness / max(1 - NoH * NoH + a * a, 0.0001);
    return k * k * INV_PI;
}

// Gometric shadowing for GGX (Smith)
float V_SmithGGX_Appx(float NoV, float NoL, float roughness) {
    float a = roughness;
    float GGXV = NoL * (NoV * (1.0 - a) + a);
    float GGXL = NoV * (NoL * (1.0 - a) + a);
    return 0.5 / max(GGXV + GGXL, 0.0001);
}


float3 GetSpecular(Surface surface, ShadingLight light)
{
    return D_GGX(light.NoH, surface.roughness)
        * V_SmithGGX_Appx(light.NoV, light.NoL, surface.roughness)
        * light.NoL;
}

#endif // End of File