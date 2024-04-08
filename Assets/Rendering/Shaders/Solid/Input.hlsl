#ifndef CAPSTONE_SOLID_INPUT
#define CAPSTONE_SOLID_INPUT

#include "../util/Common.hlsl"

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)

	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)

    UNITY_DEFINE_INSTANCED_PROP(float4, _Specular)
    UNITY_DEFINE_INSTANCED_PROP(float, _Roughness)

	UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
    
	UNITY_DEFINE_INSTANCED_PROP(float, _ShowInside)
	UNITY_DEFINE_INSTANCED_PROP(float, _ShowOutside)

#ifdef _NORMALMAP
	UNITY_DEFINE_INSTANCED_PROP(float, _NormalStrength)
#endif

#ifdef _PARALLAXMAP
    UNITY_DEFINE_INSTANCED_PROP(float, _Parallax)
#endif

UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)


#define GET_PROP(prop) UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, prop)

#if defined(_NORMALMAP) || defined(_PARALLAXMAP)
#define _NEEDS_TANGENT 1
#endif

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

#if _SPECGLOSSMAP
    TEXTURE2D(_SpecGlossMap);
    SAMPLER(sampler_SpecGlossMap);
#endif

#if _NORMALMAP
    TEXTURE2D(_NormalMap);
    SAMPLER(sampler_NormalMap);
#endif

#if _PARALLAXMAP
    TEXTURE2D(_ParallaxMap);
    SAMPLER(sampler_ParallaxMap);
#endif

#if _EMISSIONMAP
    TEXTURE2D(_EmissionMap);
    SAMPLER(sampler_EmissionMap);
#endif

#endif // End of File