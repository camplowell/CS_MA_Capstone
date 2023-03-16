float transitionRamp(float3 worldPos, float3 epicenter, float radius, float transition, float t) {
    float dist = length(worldPos - epicenter);
    float r = r * t;
    float distToEdge = dist - r;

    return (distToEdge - 0.5 * transition) / transition;
}