float3 _LightDirection;
float3 _LightPosition;
float4 _ShadowBias; // x: depth bias, y: normal bias

struct Attributes
{
    float4 objPos   : POSITION;
    float2 uv : TEXCOORD0;
    float4 normal : NORMAL;
};

struct Varyings
{
    float4 clipPos  : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
};

Varyings vert(Attributes IN) {
#if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
#else
    float3 lightDirectionWS = _LightDirection;
#endif

    Varyings OUT;
    OUT.worldPos = TransformObjectToWorld(IN.objPos.xyz);
    float3 worldPos = OUT.worldPos;
    worldPos -= lightDirectionWS * _ShadowBias.x;
    worldPos += IN.normal.xyz * _ShadowBias.y;
    OUT.clipPos = TransformWorldToHClip(worldPos);
    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

    return OUT;
}

half4 frag(Varyings IN) : SV_TARGET {
    float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
    clip(albedo.a - _Cutoff);
    return 0;
}