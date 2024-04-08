#ifndef CAPSTONE_UTIL_COMMON
#define CAPSTONE_UTIL_COMMON

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

CBUFFER_START(UnityPerDraw)
    float4x4 unity_ObjectToWorld;
    float4x4 unity_WorldToObject;
    real4 unity_WorldTransformParams;

    float4x4 unity_MatrixPreviousM;
    float4x4 unity_MatrixPreviousMI;
CBUFFER_END

float4x4 unity_MatrixVP;
float4x4 unity_MatrixV;
float4x4 glstate_matrix_projection;

float3 _WorldSpaceCameraPos;
float4 unity_OrthoParams;
float4 _ScreenParams;

#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection
#define UNITY_PREV_MATRIX_M unity_MatrixPreviousM
#define UNITY_PREV_MATRIX_I_M unity_MatrixPreviousMI


#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

//#include "./Transition.hlsl"

float3 GetWorldSpaceViewDir(float3 positionWS) {
    if (unity_OrthoParams.w == 0) {
        return _WorldSpaceCameraPos - positionWS;
    } else {
        // Orthographic
        return UNITY_MATRIX_V[2].xyz;
    }
}

float2 GetPixelPos(float4 positionCS) {
    float2 pixelPos = positionCS.xy / positionCS.w;
    pixelPos *= _ScreenParams.xy;

    return pixelPos;
}


float3 NormalMap(
    float3 tangent,
    float3 bitangent,
    float3 normal,
    float3 normalMapTS
) {
    float3 normalTS = normalMapTS * 2 - 1;
    return normalize(
        tangent * normalTS.x +
        bitangent * normalTS.y +
        normal * normalTS.z
    );
}




#endif // End of File