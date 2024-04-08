#ifndef CAPSTONE_PASS
#define CAPSTONE_PASS

#include "GraphicsUtils.hlsl"
#include "../util/Common.hlsl"
#include "../util/Transition.hlsl"

// Copied from Grass.compute
struct DrawVertex {
    float3 positionWS;
    float height;
};

struct DrawTriangle {
    float3 lightingNormalWS;
    DrawVertex vertices[3];
};

// Contains the generated mesh
StructuredBuffer<DrawTriangle> _DrawTriangles;

struct Varyings {
    float height : TEXCOORD0; // The "height" of the vertex along the grass
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float3 viewDirWS : TEXCOORD3;

    float4 positionCS : SV_POSITION;
};

Varyings vert(uint vertexID : SV_VERTEXID) {
    DrawTriangle tri = _DrawTriangles[vertexID / 3];
    DrawVertex vert = tri.vertices[vertexID % 3];

    Varyings OUT;
    OUT.positionWS = vert.positionWS;
    OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionWS);
    OUT.normalWS = tri.lightingNormalWS;
    OUT.height = vert.height;

    OUT.positionCS = TransformWorldToHClip(OUT.positionWS);

    return OUT;
}

#include "../util/Shading/Shading.hlsl"

float4 frag(Varyings IN) : SV_TARGET {
    float4 color = lerp(_BaseColor, _TipColor, IN.height);

    Surface surface;
    surface.positionWS = IN.positionWS;
    surface.normal = normalize(IN.normalWS);
    surface.incoming = normalize(IN.viewDirWS);
    surface.depth = length(TransformWorldToView(IN.positionWS));
    surface.pixelPos = GetPixelPos(IN.positionCS);

    surface.albedo = color.rgb;
    surface.alpha = color.a;
    surface.specular = 0;
    surface.roughness = 1;
    surface.emission = 0;

    return Shade(surface);
}

#endif // End of Code