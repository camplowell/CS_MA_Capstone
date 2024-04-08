#ifndef CAPSTONE_PASS
#define CAPSTONE_PASS

#include "../util/Common.hlsl"
#include "../util/Lighting/Caster.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float4 normalOS : NORMAL;
};

struct Varyings {
    float4 positionCS : SV_POSITION;

    float3 positionWS : TEXCOORD1;
    float3 viewDirWS : TEXCOORD2;
};

Varyings vert(Attributes IN)
{
    Varyings OUT;

    float3 normalWS = TransformObjectToWorldNormal(IN.normalOS.xyz);

    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS = TransformWorldToHClip(ApplyCasterBias(OUT.positionWS.xyz, normalWS));
    OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionWS.xyz);

    return OUT;
}

float4 frag(Varyings IN) : SV_TARGET
{
    return TransitionShadow(
        IN.positionCS, IN.positionWS, normalize(IN.viewDirWS),
        _ShowInside, _ShowOutside
    );
}

#endif // End of File