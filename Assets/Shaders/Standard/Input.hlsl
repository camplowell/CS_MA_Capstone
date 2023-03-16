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
CBUFFER_END

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

TEXTURE2D(_TranslucencyMap);
SAMPLER(sampler_TranslucencyMap);

TEXTURE2D(_SpecGlossMap);
SAMPLER(sampler_SpecGlossMap);

TEXTURE2D(_EmissionMap);
SAMPLER(sampler_EmissionMap);