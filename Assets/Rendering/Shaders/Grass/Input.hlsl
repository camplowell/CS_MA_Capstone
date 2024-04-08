#ifndef CAPSTONE_INPUT
#define CAPSTONE_INPUT

#include "../util/Common.hlsl"

CBUFFER_START(UnityPerMaterial)

    float4 _BaseColor;
    float4 _TipColor;

    float _ShowInside;
    float _ShowOutside;

CBUFFER_END

#define GET_PROP(prop) prop

#endif