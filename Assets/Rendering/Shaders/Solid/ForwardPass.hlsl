#ifndef CAPSTONE_PASS
#define CAPSTONE_PASS

#include "../util/Common.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float4 normalOS : NORMAL;
    float2 uv : TEXCOORD0;

#if _NEEDS_TANGENT
    float4 tangent : TANGENT;
#endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings {
    float4 positionCS : SV_POSITION;
    float2 texcoord : TEXCOORD0;

    float3 positionWS : TEXCOORD1;
    float3 viewDirWS : TEXCOORD2;
    float3 normalWS : TEXCOORD3;

#if _NEEDS_TANGENT
    float3 tangent : TEXCOORD4;
    float3 bitangent : TEXCOORD5;
#endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings vert(Attributes IN)
{
    Varyings OUT;

    UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    

    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS = TransformWorldToHClip(OUT.positionWS.xyz);
    OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionWS);
    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS.xyz);

    float4 baseST = GET_PROP(_BaseMap_ST);
    OUT.texcoord = IN.uv * baseST.xy + baseST.zw;

#if _NEEDS_TANGENT
    OUT.tangent = normalize(TransformObjectToWorldDir(IN.tangent.xyz));
    float3 bitangentOS = cross(IN.normalOS, IN.tangent.xyz) * IN.tangent.w;
    OUT.bitangent = normalize(TransformObjectToWorldDir(bitangentOS));
#endif

    return OUT;
}

#include "../util/Shading/Shading.hlsl"
#include "../util/ParallaxOcclusion.hlsl"
#include "../util/Shading/SpecGlossMap.hlsl"

void PreprocessInput(inout Varyings IN) {
    IN.viewDirWS = normalize(IN.viewDirWS);
    IN.normalWS = normalize(IN.normalWS);

#if _NEEDS_TANGENT
    IN.tangent = normalize(IN.tangent);
    IN.bitangent = normalize(IN.bitangent);
#endif

#if _PARALLAXMAP
    ApplyParallax(
        IN.tangent.xyz,
        IN.bitangent,
        IN.normalWS,
        IN.viewDirWS,
        IN.texcoord
    );
#endif

#if defined(_NORMALMAP)
    IN.normalWS = normalize(lerp(IN.normalWS, 
    NormalMap(
        IN.tangent.xyz, IN.bitangent, IN.normalWS,
        SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.texcoord)
    ), GET_PROP(_NormalStrength)));
#endif
}

Surface DefineSurface(Varyings IN) {
    Surface surface;
    surface.positionWS = IN.positionWS;
    surface.normal = IN.normalWS;
    surface.incoming = IN.viewDirWS;

    float4 albedo = GET_PROP(_BaseColor) * SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.texcoord);

    surface.albedo = albedo.rgb;
    surface.alpha = albedo.a;
    clip(surface.alpha - 0.01);

    ReadSpecGloss(IN.texcoord, surface.specular, surface.roughness);

    surface.emission = GET_PROP(_Emission);
#if _EMISSIONMAP
    surface.emission *= SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, IN.texcoord);
#endif

    surface.depth = length(TransformWorldToView(IN.positionWS));
    surface.pixelPos = GetPixelPos(IN.positionCS);

    return surface;
}

float4 frag(Varyings IN, bool isFrontFace : SV_ISFRONTFACE) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(IN);

    PreprocessInput(IN);
    Surface surface = DefineSurface(IN);

    return Shade(surface, isFrontFace);
}

#endif // End of File