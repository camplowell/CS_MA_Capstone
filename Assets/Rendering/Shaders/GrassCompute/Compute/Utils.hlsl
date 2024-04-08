#ifndef CAPSTONE_COMPUTE_UTILS
#define CAPSTONE_COMPUTE_UTILS


float4x4 _Obj2World;

#define PI 3.14159265359 

float3 Obj2World(float3 positionOS) {
    return mul(_Obj2World, float4(positionOS, 1)).xyz;
}

void CalculateNormalAndTS2WSMatrix(
    float3 positionA, float3 positionB, float3 positionC,
    out float3 normal, out float3x3 tbnInverse
) {
    float3 tangent = normalize(positionB - positionA);
    normal = normalize(cross(tangent, normalize(positionC - positionA)));
    float3 bitangent = normalize(cross(tangent, normal));

    tbnInverse = transpose(float3x3(tangent, bitangent, normal));
}

// A function to compute an rotation matrix which rotates a point
// by angle radians around the given axis
// By Keijiro Takahashi
float3x3 AngleAxis3x3(float angle, float3 axis) {
    float c, s;
    sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float3x3(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c
        );
}

#define SAMPLE_TEXEL(tex, texel) tex.SampleLevel(sampler##tex, ((texel) + 0.5) * tex##_TexelSize.xy, 0)

#endif // End of File