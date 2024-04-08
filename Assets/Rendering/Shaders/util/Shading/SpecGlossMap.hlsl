#ifndef CAPSTONE_UTIL_SHADING_SPECGLOSSMAP
#define CAPSTONE_UTIL_SHADING_SPECGLOSSMAP

void ReadSpecGloss(float2 texcoord, out float3 specular, out float roughness) {
#if _SPECGLOSSMAP

    #if _ROUGH_MAP
        specular = GET_PROP(_Specular.xyz);
        roughness = SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, texcoord).x;
    #else
        float4 specGlossTex = SAMPLE_TEXTURE2D(_SpecGlossMap, sampler_SpecGlossMap, texcoord);
        specular = specGlossTex.xyz;
        roughness = specGlossTex.w;
    #endif

#else
    specular = GET_PROP(_Specular.xyz);
    roughness = GET_PROP(_Roughness);
#endif
}


#endif // End of File