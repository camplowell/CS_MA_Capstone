#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

CBUFFER_START(UnityPerMaterial)
    float4 _BaseColor;
    float4 _BaseMap_ST;

    float _LightWrap;
    float4 _TranslucencyColor;

    float _Cutoff;

    float _Roughness;
    float4 _Specular;

    float4 _Emission;

    float _BumpScale;

#ifdef _PARALLAXMAP
    float _Parallax;
#endif

CBUFFER_END

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

TEXTURE2D(_TranslucencyMap);
SAMPLER(sampler_TranslucencyMap);

#ifdef _SPECGLOSSMAP
TEXTURE2D(_SpecGlossMap);
SAMPLER(sampler_SpecGlossMap);
#endif

TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);

#ifdef _NORMALMAP
TEXTURE2D(_BumpMap);
SAMPLER(sampler_BumpMap);
#endif

#ifdef _PARALLAXMAP
TEXTURE2D(_ParallaxMap);
SAMPLER(sampler_ParallaxMap);
#endif