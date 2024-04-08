#ifndef CAPSTONE_TERRAIN_INPUT
#define CAPSTONE_TERRAIN_INPUT

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

#if defined(_NORMALMAP0) || defined(_NORMALMAP1) || defined(_NORMALMAP2) || defined(_NORMALMAP3)
#define _NEEDS_TANGENT 1
#endif

CBUFFER_START(UnityPerMaterial)
    float _HeightTransition;

    float4 _MaskTex_ST;

    float4 _BaseMap0_ST;
	float4 _BaseColor0;
    float3 _Specular0;
    float _Roughness0;


    float4 _BaseMap1_ST;
	float4 _BaseColor1;
    float3 _Specular1;
    float _Roughness1;

    float4 _BaseMap2_ST;
	float4 _BaseColor2;
    float3 _Specular2;
    float _Roughness2;

    float4 _BaseMap3_ST;
	float4 _BaseColor3;
    float3 _Specular3;
    float _Roughness3;

    float _ShowInside;
    float _ShowOutside;

#if _NORMALMAP0
	float _NormalStrength0;
#endif
#if _NORMALMAP1
	float _NormalStrength1;
#endif
#if _NORMALMAP2
	float _NormalStrength2;
#endif
#if _NORMALMAP3
	float _NormalStrength3;
#endif

CBUFFER_END

#define GET_PROP(prop) prop

#define TEXTURE2D_SAMPLER(_mapName, _samplerName) \
TEXTURE2D(_mapName); \
SAMPLER(_samplerName)

TEXTURE2D_SAMPLER(_MaskTex, sampler_MaskTex);

TEXTURE2D_SAMPLER(_BaseMap0, sampler_BaseMap0);
TEXTURE2D_SAMPLER(_BaseMap1, sampler_BaseMap1);
TEXTURE2D_SAMPLER(_BaseMap2, sampler_BaseMap2);
TEXTURE2D_SAMPLER(_BaseMap3, sampler_BaseMap3);

#if _SPECGLOSSMAP0
    TEXTURE2D_SAMPLER(_SpecGlossMap0, sampler_SpecGlossMap0);
#endif
#if _SPECGLOSSMAP1
    TEXTURE2D_SAMPLER(_SpecGlossMap1, sampler_SpecGlossMap1);
#endif
#if _SPECGLOSSMAP2
    TEXTURE2D_SAMPLER(_SpecGlossMap2, sampler_SpecGlossMap2);
#endif
#if _SPECGLOSSMAP3
    TEXTURE2D_SAMPLER(_SpecGlossMap3, sampler_SpecGlossMap3);
#endif

#if _NORMALMAP0
TEXTURE2D_SAMPLER(_NormalMap0, sampler_NormalMap0);
#endif
#if _NORMALMAP1
TEXTURE2D_SAMPLER(_NormalMap1, sampler_NormalMap1);
#endif
#if _NORMALMAP2
TEXTURE2D_SAMPLER(_NormalMap2, sampler_NormalMap2);
#endif
#if _NORMALMAP3
TEXTURE2D_SAMPLER(_NormalMap3, sampler_NormalMap3);
#endif

#if _HEIGHTMAP0
TEXTURE2D_SAMPLER(_HeightMap0, sampler_HeightMap0);
#endif
#if _HEIGHTMAP1
TEXTURE2D_SAMPLER(_HeightMap1, sampler_HeightMap1);
#endif
#if _HEIGHTMAP2
TEXTURE2D_SAMPLER(_HeightMap2, sampler_HeightMap2);
#endif
#if _HEIGHTMAP3
TEXTURE2D_SAMPLER(_HeightMap3, sampler_HeightMap3);
#endif

#endif // End of File