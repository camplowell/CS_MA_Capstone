#ifndef CAPSTONE_UTIL_SHADING
#define CAPSTONE_UTIL_SHADING

#ifndef CAPSTONE_DIFFUSE
#include "StandardDiffuse.hlsl"
#endif

#ifndef CAPSTONE_SPECULAR
#include "StandardSpecular.hlsl"
#endif

#ifndef CAPSTONE_DIFFSPEC_MIX
#include "Fresnel.hlsl"
#endif

#include "../Lighting/Lights.hlsl"
#include "Surface.hlsl"
#include "../Transition.hlsl"

uniform float3 _AmbientLight;

float4 Shade(Surface surface, bool isFrontFace = true) {
#if _INTERIORGLOW
    if (!isFrontFace) {
        return InteriorTransition(
            surface
        );
    }
#endif

    float inside;
    float3 glow = SurfaceTransition(surface, inside);

    // Convert to perceptual specular / roughness
    surface.specular *= surface.specular;
    surface.roughness *= surface.roughness;

    float3 diffuse = _AmbientLight;
    float3 specular = 0;
    for (int i = 0; i < DIR_LIGHT_COUNT; i++) {
        ShadingLight light = MakeShadingLight(i, surface, inside);
        float3 mixFac = GetMixFac(surface, light);
        diffuse += GetDiffuse(surface, light) * (1 - mixFac);
        specular += GetSpecular(surface, light) * mixFac;
    }

    float3 color = surface.emission;
    color += diffuse * surface.albedo;
    color += specular;
    
    return float4(max(color, glow), surface.alpha);
}

#endif // End of FIle