#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "./SphericalGaussians.hlsl"

struct MyLightingData {
    float3 primary;
    float3 accent;
    float3 specular;
};

struct MySurface {
    float4 albedo;
    float3 f0;
    float  roughness;
    float3 emission;
    float3 translucency;
    float  lightWrap;
    float  ao;
};

float lum(float3 color) {
    return dot(color, float3(0.2126, 0.7152, 0.0722)); //Max3(color.r, color.g, color.b);
}

float4 MixLighting(MySurface surface, MyLightingData lighting) {
    float3 diffuse = lighting.primary + lighting.accent;
    return (surface.albedo * float4(diffuse, 1.0)) + float4(lighting.specular + surface.emission, 0.0);
}

BRDFData _InitializeBRDFData(inout MySurface surface) {
    half reflectivity = ReflectivitySpecular(surface.f0);
    half oneMinusReflectivity = half(1.0) - reflectivity;
    surface.albedo.rgb *= (1.0 - Max3(surface.f0.r, surface.f0.g, surface.f0.b));
    half3 brdfDiffuse = surface.albedo.rgb;
    half3 brdfSpecular = half3(surface.f0);
    BRDFData brdfData;
    InitializeBRDFDataDirect(
        surface.albedo.rgb,
        brdfDiffuse,
        brdfSpecular,
        reflectivity,
        oneMinusReflectivity,
        1.0 - surface.roughness,
        surface.albedo.a,
        brdfData
    );
    return brdfData;
}

void AddStylizedLight(
    Light light, MySurface surface, BRDFData brdfData, 
    float3 normalWS, float3 viewDirectionWS, float fresnelTerm,
    inout MyLightingData lighting, bool specularHighlightsOff
) {
    float attenuation = (light.distanceAttenuation * light.shadowAttenuation);

    float NdotL = dot(light.direction, normalWS);
    float3 radiance = light.color * attenuation;

    float3 radius = surface.lightWrap * surface.lightWrap * surface.translucency;
    float3 scattering = SoftenSG(light.direction, radius, normalWS);
    lighting.accent += radiance * scattering * surface.albedo.rgb * (1 - fresnelTerm);

    radiance *= saturate(NdotL);

#ifndef _SPECULARHIGHLIGHTS_OFF
    [branch] if (!specularHighlightsOff) {
        lighting.specular += radiance * brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, light.direction, viewDirectionWS);
    }
#endif // _SPECULARHIGHLIGHTS_OFF
}

void AddEnvironmentLight(
    MySurface surface, BRDFData brdfData, float3 positionWS, 
    float3 bakedGI, float3 reflectVector, float fresnelTerm,
    inout MyLightingData lighting, bool environmentReflectionsOff
) {
    lighting.primary += bakedGI;
#ifndef _ENVIRONMENTREFLECTIONS_OFF
    [branch] if (!environmentReflectionsOff) {
        float3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, positionWS, brdfData.perceptualRoughness, 1.0h);
        lighting.specular += indirectSpecular * EnvironmentBRDFSpecular(brdfData, fresnelTerm);
    }
#endif
}

void MixMainLight(inout MyLightingData lighting) {
    float brightness = lum(lighting.accent) + lum(lighting.primary);
    float fac = lum(lighting.accent) / max(lum(lighting.primary), 0.0001);
    fac = fac / (1 + fac);
    lighting.primary = lerp(lighting.primary, lighting.accent, fac);
    lighting.primary *= brightness / max(lum(lighting.primary), 0.01);
    lighting.accent = float3(0, 0, 0);
}

float4 UniversalFragmentStylized(InputData inputData, MySurface surface) {
    BRDFData brdfData = _InitializeBRDFData(surface);
    float NoV = dot(inputData.normalWS, inputData.viewDirectionWS);
    half3 reflectVector = reflect(-inputData.viewDirectionWS, inputData.normalWS);
    half fresnelTerm = saturate(Pow4(1.0 - lerp(NoV, 1, surface.roughness)));

    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData.normalizedScreenSpaceUV, surface.ao);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    MyLightingData lighting = (MyLightingData)0;

    // Main and environment lighting
    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);

    AddEnvironmentLight(
        surface, brdfData, inputData.positionWS,
        inputData.bakedGI, reflectVector, fresnelTerm,
        lighting, false
    );
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers)) {
        AddStylizedLight(
            mainLight, surface, brdfData, 
            inputData.normalWS, inputData.viewDirectionWS, fresnelTerm,
            lighting, false
        );
    }
    MixMainLight(lighting);
    
    // Additional lights
    #if defined(_ADDITIONAL_LIGHTS)
    uint pixelLightCount = GetAdditionalLightsCount();

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            AddStylizedLight(
            light, surface, brdfData, 
            inputData.normalWS, inputData.viewDirectionWS, fresnelTerm,
            lighting, false
        );
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            AddStylizedLight(
            light, surface, brdfData, 
            inputData.normalWS, inputData.viewDirectionWS, fresnelTerm,
            lighting, false
        );
        }
    LIGHT_LOOP_END
    #endif

    
    return MixLighting(surface, lighting);
}