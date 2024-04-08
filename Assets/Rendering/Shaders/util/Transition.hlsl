#ifndef CAPSTONE_UTIL_TRANSITION
#define CAPSTONE_UTIL_TRANSITION

#include "Common.hlsl"

uniform float _TransitionRadius;
uniform float2 _TransitionPos;
uniform float3 _TransitionGlow;
uniform float _TransitionExtent;
uniform float _TransitionFalloff;

void TransitionCore(float3 worldPos, float3 viewDir, out bool inside, out float glowDist) {
    inside = false;
    glowDist = 0;
    // Define the ray
    float2 startPos = worldPos.xz - _TransitionPos;
    float startDist = length(startPos);

    [branch] if (_TransitionRadius <= 0) {
        inside = false;
        glowDist = startDist - _TransitionRadius;
        return;
    }

    float2 rayDir = normalize(viewDir.xz);

    float t_center = dot(-startPos, rayDir);
    float d_center = length(startPos + rayDir * t_center);

    glowDist = abs(startDist - _TransitionRadius);
    inside = (startDist < _TransitionRadius);
    if (t_center > 0) {
        inside = (d_center < _TransitionRadius);

        if (inside) {
            float d_lateral = abs(_TransitionRadius - d_center);
            glowDist = sqrt(0.25 * t_center * t_center + d_lateral * d_lateral);
        }
    }
}

#include "./NoisyNodes/SimplexNoise3D.hlsl"
#include "Shading/Surface.hlsl"

float GetGlowNoise(float3 worldPos) {
    float OUT;
    SimplexNoise3D_float(worldPos * float3(1, 1, .1), OUT);
    return OUT;
}

float GlowFac(float3 positionWS, float glowDist) {
    float distNorm = saturate(glowDist / _TransitionExtent);
    float dist2Inv = 1 - distNorm * distNorm;
    return  (dist2Inv * dist2Inv) / (1 + _TransitionFalloff * distNorm);
}

float4 InteriorTransition(
    Surface surface
) {
    bool isInside;
    float glowDist;
    TransitionCore(surface.positionWS, surface.incoming, isInside, glowDist);

    clip((isInside ? GET_PROP(_ShowInside) : GET_PROP(_ShowOutside)) - 0.5);

    return float4(_TransitionGlow, surface.alpha);
}

float4 UnlitTransition(
    float4 color,
    float3 positionWS, float3 viewDir
) {
    bool isInside;
    float glowDist;
    TransitionCore(positionWS, viewDir, isInside, glowDist);

    clip((isInside ? GET_PROP(_ShowInside) : GET_PROP(_ShowOutside)) - 0.5);

    return color + float4(_TransitionGlow * GlowFac(positionWS, glowDist), color.a);
}

float3 SurfaceTransition(
     float glowDist,
     inout Surface surface
) {
    /*
    float glowFac = GlowFac(surface.positionWS, glowDist);
    
    surface.emission = lerp(
        surface.emission, 
        _TransitionGlow,
        glowFac);
    surface.albedo *= 1 - glowFac;
    surface.specular *= 1 - glowFac;
    */
    return _TransitionGlow * GlowFac(surface.positionWS, glowDist);
}

float3 SurfaceTransition(
    inout Surface surface,
    out float inside
) {
    bool isInside;
    float glowDist;
    TransitionCore(surface.positionWS, surface.incoming, isInside, glowDist);

    clip((isInside ? GET_PROP(_ShowInside) : GET_PROP(_ShowOutside)) - 0.5);

    inside = isInside ? 1.0 : 0.0;

    return SurfaceTransition(glowDist, surface);
}



#endif // End of File