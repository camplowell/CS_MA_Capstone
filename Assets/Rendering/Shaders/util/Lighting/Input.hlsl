#ifndef CAPSTONE_UTIL_LIGHTING_INPUT
#define CAPSTONE_UTIL_LIGHTING_INPUT

// Lights ----------------------------------------------------------------------------------------

#define MAX_DIRECTIONAL_LIGHT_COUNT 2

CBUFFER_START(_CapstoneLight)
    int _DirectionalLightCount;
    float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
    float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
    float4 _DirectionalLightShadowData[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

#define DIR_LIGHT_COUNT min(_DirectionalLightCount, MAX_DIRECTIONAL_LIGHT_COUNT)

struct Light {
    float3 color;
    float3 direction;
    float attenuation;
};

// Shadows ---------------------------------------------------------------------------------------

#define MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT 1

TEXTURE2D(_DirectionalShadowAtlas);
#define SHADOW_SAMPLER sampler_point_clamp
SAMPLER(SHADOW_SAMPLER);
float4 _DirectionalShadowAtlas_TexelSize;

CBUFFER_START(_CapstoneShadows)
    float4x4 _DirectionalShadowMatrices[MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

uniform float _NormalBias;
uniform float _DepthBias;

struct DirectionalShadowData {
    float strength;
    int tileIndex;
};

#endif // End of File