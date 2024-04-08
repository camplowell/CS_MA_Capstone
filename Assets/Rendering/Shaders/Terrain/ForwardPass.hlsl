#ifndef CAPSTONE_PASS
#define CAPSTONE_PASS

#include "../util/Common.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float4 normalOS : NORMAL;
    float2 uv_Mask : TEXCOORD0;

#if _NEEDS_TANGENT
    float4 tangent : TANGENT;
#endif
};

struct Varyings {
    float4 positionCS : SV_POSITION;
    float2 uv_Mask : TEXCOORD0;

    float3 positionWS : TEXCOORD1;
    float3 viewDirWS : TEXCOORD2;
    float3 normalWS : TEXCOORD3;

#if _NEEDS_TANGENT
    float3 tangentWS : TEXCOORD4;
    float3 bitangentWS : TEXCOORD5;
#endif

};

Varyings vert(Attributes IN)
{
    Varyings OUT;
    

    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS = TransformWorldToHClip(OUT.positionWS.xyz);
    OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionWS);
    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS.xyz);

    OUT.uv_Mask = IN.uv_Mask * _MaskTex_ST.xy + _MaskTex_ST.zw;

#if _NEEDS_TANGENT
    OUT.tangentWS = normalize(TransformObjectToWorldDir(IN.tangent.xyz));
    float3 bitangentOS = cross(IN.normalOS, IN.tangent.xyz) * IN.tangent.w;
    OUT.bitangentWS = normalize(TransformObjectToWorldDir(bitangentOS));
#endif

    return OUT;
}

#include "../util/Shading/Shading.hlsl"

void PreprocessInput(inout Varyings IN) {
    IN.viewDirWS = normalize(IN.viewDirWS);
    IN.normalWS = normalize(IN.normalWS);

#if _NEEDS_TANGENT
    IN.tangentWS = normalize(IN.tangentWS);
    IN.bitangentWS = normalize(IN.bitangentWS);
#endif
}

float3 weight3(float3 v0, float3 v1, float3 v2, float3 v3, float4 weights) {
    return (weights.x * v0) + (weights.y * v1) + (weights.z * v2) + (weights.w * v3);
}

float4 getHeights(float2 uv0, float2 uv1, float2 uv2, float2 uv3) {
    float4 heights = 0.5;
#if _HEIGHTMAP0
    heights.x = SAMPLE_TEXTURE2D(_HeightMap0, sampler_HeightMap0, uv0).x;
#endif
#if _HEIGHTMAP1
    heights.y = SAMPLE_TEXTURE2D(_HeightMap1, sampler_HeightMap1, uv1).x;
#endif
#if _HEIGHTMAP2
    heights.z = SAMPLE_TEXTURE2D(_HeightMap2, sampler_HeightMap2, uv2).x;
#endif
#if _HEIGHTMAP3
    heights.w = SAMPLE_TEXTURE2D(_HeightMap3, sampler_HeightMap3, uv3).x;
#endif
    return heights;
}

float4 getWeights(float2 uv_Mask, float4 heights) {
    float4 weights = heights;
    weights += SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv_Mask);
    weights.w = 0;

    float maxWeight = max(max(weights.x, weights.y), max(weights.z, weights.w));

    weights = max(weights + _HeightTransition - maxWeight, 0);
    float totalWeight = weights.x + weights.y + weights.z + weights.w;
    
    return weights / totalWeight;
}

float4 frag(Varyings IN, bool isFrontFace : SV_ISFRONTFACE) : SV_TARGET
{
    PreprocessInput(IN);

    float2 uv0 = IN.positionWS.xz / _BaseMap0_ST.xy + _BaseMap0_ST.zw;
    float2 uv1 = IN.positionWS.xz / _BaseMap1_ST.xy + _BaseMap1_ST.zw;
    float2 uv2 = IN.positionWS.xz / _BaseMap2_ST.xy + _BaseMap2_ST.zw;
    float2 uv3 = IN.positionWS.xz / _BaseMap3_ST.xy + _BaseMap3_ST.zw;

    float4 weights = getWeights(IN.uv_Mask, getHeights(uv0, uv1, uv2, uv3));

    Surface surface0;
    Surface surface1;
    Surface surface2;
    Surface surface3;

    surface0.positionWS = IN.positionWS;
    surface1.positionWS = IN.positionWS;
    surface2.positionWS = IN.positionWS;
    surface3.positionWS = IN.positionWS;

#if _NORMALMAP0
    surface0.normal = NormalMap(
        IN.tangentWS, IN.bitangentWS, IN.normalWS,
        lerp(float3(.5, .5, 1), SAMPLE_TEXTURE2D(_NormalMap0, sampler_NormalMap0, uv0), _NormalStrength0)
    );
#else
    surface0.normal = IN.normalWS;
#endif
#if _NORMALMAP1
    surface1.normal = NormalMap(
        IN.tangentWS, IN.bitangentWS, IN.normalWS,
        lerp(float3(.5, .5, 1), SAMPLE_TEXTURE2D(_NormalMap1, sampler_NormalMap1, uv1), _NormalStrength1)
    );
#else
    surface1.normal = IN.normalWS;
#endif
#if _NORMALMAP2
    surface2.normal = NormalMap(
        IN.tangentWS, IN.bitangentWS, IN.normalWS,
        lerp(float3(.5, .5, 1), SAMPLE_TEXTURE2D(_NormalMap2, sampler_NormalMap2, uv2), _NormalStrength2)
    );
#else
    surface2.normal = IN.normalWS;
#endif
#if _NORMALMAP3
    surface3.normal = NormalMap(
        IN.tangentWS, IN.bitangentWS, IN.normalWS,
        lerp(float3(.5, .5, 1), SAMPLE_TEXTURE2D(_NormalMap3, sampler_NormalMap3, uv3), _NormalStrength3)
    );
#else
    surface3.normal = IN.normalWS;
#endif

    surface0.incoming = IN.viewDirWS;
    surface1.incoming = IN.viewDirWS;
    surface2.incoming = IN.viewDirWS;
    surface3.incoming = IN.viewDirWS;

    surface0.depth = length(TransformWorldToView(IN.positionWS));
    surface1.depth = surface0.depth;
    surface2.depth = surface0.depth;
    surface3.depth = surface0.depth;

    surface0.pixelPos = GetPixelPos(IN.positionCS);
    surface1.pixelPos = surface0.pixelPos;
    surface2.pixelPos = surface0.pixelPos;
    surface3.pixelPos = surface0.pixelPos;

    float4 color0 = _BaseColor0 * SAMPLE_TEXTURE2D(_BaseMap0, sampler_BaseMap0, uv0);
    surface0.albedo = color0.rgb;
    surface0.alpha = color0.a;
    float4 color1 = _BaseColor1 * SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, uv1);
    surface1.albedo = color1.rgb;
    surface1.alpha = color1.a;
    float4 color2 = _BaseColor2 * SAMPLE_TEXTURE2D(_BaseMap2, sampler_BaseMap2, uv2);
    surface2.albedo = color2.rgb;
    surface2.alpha = color2.a;
    float4 color3 = _BaseColor3 * SAMPLE_TEXTURE2D(_BaseMap3, sampler_BaseMap3, uv3);
    surface3.albedo = color3.rgb;
    surface3.alpha = color3.a;

#if _SPECGLOSSMAP0
    float4 specGlossTex0 = SAMPLE_TEXTURE2D(_SpecGlossMap0, sampler_SpecGlossMap0, uv0);
    surface0.roughness = specGlossTex0.w;
    #if _ROUGH_MAP0
        surface0.specular = GET_PROP(_Specular0.xyz);
    #else
        surface0.specular = specGlossTex0.xyz;
    #endif
#else
    surface0.specular = GET_PROP(_Specular0.xyz);
    surface0.roughness = GET_PROP(_Roughness0);
#endif
#if _SPECGLOSSMAP1
    float4 specGlossTex1 = SAMPLE_TEXTURE2D(_SpecGlossMap1, sampler_SpecGlossMap1, uv1);
    surface1.roughness = specGlossTex1.w;
    #if _ROUGH_MAP1
        surface1.specular = GET_PROP(_Specular1.xyz);
    #else
        surface1.specular = specGlossTex1.xyz;
    #endif
#else
    surface1.specular = GET_PROP(_Specular1.xyz);
    surface1.roughness = GET_PROP(_Roughness1);
#endif
#if _SPECGLOSSMAP2
    float4 specGlossTex2 = SAMPLE_TEXTURE2D(_SpecGlossMap2, sampler_SpecGlossMap2, uv2);
    surface2.roughness = specGlossTex2.w;
    #if _ROUGH_MAP2
        surface2.specular = GET_PROP(_Specular2.xyz);
    #else
        surface2.specular = specGlossTex2.xyz;
    #endif
#else
    surface2.specular = GET_PROP(_Specular2.xyz);
    surface2.roughness = GET_PROP(_Roughness2);
#endif
#if _SPECGLOSSMAP3
    float4 specGlossTex3 = SAMPLE_TEXTURE2D(_SpecGlossMap3, sampler_SpecGlossMap3, uv3);
    surface3.roughness = specGlossTex3.w;
    #if _ROUGH_MAP3
        surface3.specular = GET_PROP(_Specular3.xyz);
    #else
        surface3.specular = specGlossTex3.xyz;
    #endif
#else
    surface3.specular = GET_PROP(_Specular3.xyz);
    surface3.roughness = GET_PROP(_Roughness3);
#endif




    surface0.emission = 0;
    surface1.emission = 0;
    surface2.emission = 0;
    surface3.emission = 0;


    return (weights.x * Shade(surface0, isFrontFace))
         + (weights.y * Shade(surface1, isFrontFace))
         + (weights.z * Shade(surface2, isFrontFace))
         + (weights.w * Shade(surface3, isFrontFace));
}

#endif // End of File