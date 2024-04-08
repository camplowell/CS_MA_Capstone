#ifndef CAPSTONE_UTIL_PARALLAX
#define CAPSTONE_UTIL_PARALLAX

#ifdef _PARALLAXMAP

#define MIN_STEPS 2
#define MAX_STEPS 30
#define REFINE_STEPS 4


float3 CalculateViewDirTS(
    float3 tangent,
    float3 bitangent,
    float3 normal,
    float3 viewDirWS
) {
    float3x3 objectToTangent = float3x3(
			normalize(tangent),
			normalize(bitangent),
			normalize(normal)
		);
		return mul(objectToTangent, viewDirWS) * float3(_Parallax, _Parallax, 1);
}

void ApplyParallax(
    float3 tangent,
    float3 bitangent,
    float3 normal,
    float3 viewDirWS, 
    inout float2 uv
) {
    float3 viewDirTS = CalculateViewDirTS(tangent, bitangent, normal, viewDirWS);
    int steps = lerp(MAX_STEPS, MIN_STEPS, saturate(-viewDirTS.z));

    float3 rayStep = normalize(viewDirTS);
    rayStep /= -rayStep.z * steps;

    float3 pos = float3(uv, 1.0);

    int i;
    [loop] for (i = 0; i < steps; i++) {
        float mapHeight = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, pos.xy).x;
        if (mapHeight > pos.z) {
            rayStep *= 0.5;
            pos -= rayStep;
            i = steps + 1;
        } else {
            pos += rayStep;
        }
    }
    for (i = 0; i < REFINE_STEPS; i++) {
        float mapHeight = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, pos.xy).x;
        pos += mapHeight > pos.z ? -rayStep : rayStep;
        rayStep *= 0.5;
    }
    
    uv = pos.xy;
}

#endif

#endif