#include "../util/Lighting.hlsl"
#include "../util/ParallaxOcclusion.hlsl"

struct Attributes
{
    float4 objPos   : POSITION;
    float2 uv : TEXCOORD0;
    float4 normal : NORMAL;
    float4 texcoord1 : TEXCOORD1;
#ifdef _NORMALMAP
    float4 tangent : TANGENT;
#endif
};

struct Varyings
{
    float4 clipPos  : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    float3 normal : TEXCOORD2;
    float3 incoming : TEXCOORD3;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
#if defined(_NORMALMAP) || defined(_PARALLAXMAP)
    float4 tangent : TEXCOORD5;
    float3 bitangent : TEXCOORD6;
#endif
};

float3 NormalMap(Varyings IN) {
#ifdef _NORMALMAP
    float3 tangentNormals = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv), _BumpScale);
    return normalize(
        IN.tangent.xyz * tangentNormals.x + 
        IN.bitangent * tangentNormals.y + 
        IN.normal * tangentNormals.z);
#else
    return normalize(IN.normal);
#endif
}

InputData InitializeInputData(Varyings IN) {
    InputData inputData = (InputData)0;
    inputData.positionWS = IN.worldPos;
    inputData.positionCS = IN.clipPos;
    inputData.normalWS = NormalMap(IN);
    inputData.viewDirectionWS = normalize(IN.incoming);
    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS + _ShadowBias.y * normalize(IN.normal));
    // fogCoord
    // vertexLighting
    inputData.bakedGI = SAMPLE_GI( IN.lightmapUV, IN.vertexSH, inputData.normalWS );
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
    // shadowMask
    // tangentToWorld
    return inputData;
}

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.worldPos = TransformObjectToWorld(IN.objPos.xyz);
    OUT.clipPos = TransformWorldToHClip(OUT.worldPos.xyz);
    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

    OUT.normal = TransformObjectToWorldNormal(IN.normal.xyz);
#if defined(_NORMALMAP) || defined(_PARALLAXMAP)
    OUT.tangent.xyz = TransformObjectToWorldDir(IN.tangent.xyz);
    OUT.tangent.w = IN.tangent.w;
    OUT.bitangent = float4(cross(OUT.normal, OUT.tangent.xyz) * OUT.tangent.w, 1.0);
#endif
    OUT.incoming = _WorldSpaceCameraPos - OUT.worldPos;

    OUTPUT_LIGHTMAP_UV( IN.texcoord1, unity_LightmapST, OUT.lightmapUV );
    OUTPUT_SH(OUT.normal.xyz, OUT.vertexSH);

    return OUT;
}

// The fragment shader definition.
float4 frag(Varyings IN) : SV_Target
{
#if _PARALLAXMAP
    float3 viewDirTS = CalculateViewDirTS(IN.tangent, IN.bitangent, IN.normal, IN.incoming);
    ApplyParallax(viewDirTS, IN.uv);
#endif
    float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
    clip(albedo.a - _Cutoff);

    InputData inputData = InitializeInputData(IN);
    
    #ifdef _SPECGLOSSMAP
        float4 specData = SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, IN.uv);
        specData.a *= _Roughness;
    #else
        float4 specData = float4(_Specular.rgb, _Roughness);
    #endif

    MySurface surface;
    surface.albedo = albedo;
    surface.f0 = specData.rgb * specData.rgb;
    surface.roughness = specData.a;
    surface.emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, IN.uv).rgb * _Emission.rgb;
    surface.translucency = SAMPLE_TEXTURE2D(_TranslucencyMap, sampler_TranslucencyMap, IN.uv).rgb * _TranslucencyColor.rgb;
    surface.lightWrap = _LightWrap;
    surface.ao = 1;
    
    return UniversalFragmentStylized(inputData, surface);
}