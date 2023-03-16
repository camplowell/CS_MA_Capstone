struct SphericalGaussian
{
    float3 Amplitude;
    float3 Axis;
    float3 Sharpness;
};

float3 ApproximateSGIntegral(in SphericalGaussian sg)
{
    return (sg.Amplitude / sg.Sharpness) * 6.28318530718;
}

SphericalGaussian MakeNormalizedSG(float3 axis, float3 sharpness) {
    SphericalGaussian sg;
    sg.Amplitude = 1;
    sg.Axis = axis;
    sg.Sharpness = sharpness;

    sg.Amplitude /= ApproximateSGIntegral(sg);

    return sg;
}

float3 SGIrradianceFitted(in SphericalGaussian sg, in float3 normal)
{
    const float muDotN = dot(sg.Axis, normal);
    const float3 lambda = sg.Sharpness;

    const float c0 = 0.36f;
    const float c1 = 1.0f / (4.0f * c0);

    float3 eml = exp(-lambda);
    float3 em2l = eml * eml;
    float3 rl = rcp(lambda);

    float3 scale = 1.0f + 2.0f * em2l - rl;
    float3 bias = (eml - em2l) * rl - em2l;

    float3 x = sqrt(1.0f - scale);
    float x0 = c0 * muDotN;
    float3 x1 = x * c1;

    float3 n = x1 + x0;

    float3 y = saturate(muDotN);
    y = lerp(y, n * n / x, float3(
        abs(x0) <= x1.x ? 1 : 0,
        abs(x0) <= x1.y ? 1 : 0,
        abs(x0) <= x1.z ? 1 : 0
    ));

    float3 result = scale * y + bias;

    return result * ApproximateSGIntegral(sg);
}

float3 SoftenSG(float3 lightDir, float3 scattering, float3 normal) {
    SphericalGaussian sg = MakeNormalizedSG(lightDir, 1.0f / max(sqrt(scattering), 0.0001));

    return SGIrradianceFitted(sg, normal);
}