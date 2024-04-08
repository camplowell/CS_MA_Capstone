#ifndef CAPSTONE_PASS
#define CAPSTONE_PASS

#include "../util/Common.hlsl"
#include "../util/Lighting/Caster.hlsl"

struct Attributes {
    float4 positionOS : POSITION;
    float4 normalOS : NORMAL;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings {
    float4 positionCS : SV_POSITION;

    float3 positionWS : TEXCOORD1;
    float3 viewDirWS : TEXCOORD2;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings vert(Attributes IN)
{
    Varyings OUT;

    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

    float3 normalWS = TransformObjectToWorldNormal(IN.normalOS.xyz);

    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
    OUT.positionCS = TransformWorldToHClip(ApplyCasterBias(OUT.positionWS.xyz, normalWS));
    OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionWS.xyz);

    return OUT;
}

float4 frag(Varyings IN) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(IN);
    
    return TransitionShadow(
        IN.positionCS, IN.positionWS, normalize(IN.viewDirWS),
        GET_PROP(_ShowInside), 
        GET_PROP(_ShowOutside)
    );
}

#endif // End of File