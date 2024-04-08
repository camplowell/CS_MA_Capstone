uniform float _TransitionRadius;
uniform float2 _TransitionPos;
uniform float3 _TransitionGlow;
uniform float _TransitionExtent;
uniform float _TransitionFalloff;

bool TransitionHit(
    float3 originWS, float3 nearPlaneRWS, inout float z_depth, 
    out float3 hitPos, out float2 hitNormal, out float hitDot) {
    // Default hit position is positionWS
    hitPos = originWS + nearPlaneRWS * z_depth;
    hitNormal = 0;
    hitDot = 0;

    // Define the ray
    [branch] if (_TransitionRadius <= 0) return false;
    float horizontalness = length(nearPlaneRWS.xz);
    float3 rayDir3 = nearPlaneRWS / horizontalness;
    float2 rayDir = rayDir3.xz;

    float2 startPos = originWS.xz - _TransitionPos;

    float t_center = dot(-startPos, rayDir);
    float d_center = length(startPos + rayDir * t_center);

    [branch] if (d_center > _TransitionRadius) return false;

    float t_width = sqrt(_TransitionRadius * _TransitionRadius - d_center * d_center);
    float t_hit = t_center - t_width;

    [branch] if (t_hit < 0 && unity_OrthoParams.w == 0) return false;

    float z_hit = t_hit / horizontalness;
    if (z_hit > z_depth) return false;
    
    z_depth = z_hit;
    hitPos = originWS + nearPlaneRWS * z_hit;
    hitNormal = (hitPos.xz - _TransitionPos) / _TransitionRadius;

    hitDot = dot(-rayDir, hitNormal);
    return true;
}