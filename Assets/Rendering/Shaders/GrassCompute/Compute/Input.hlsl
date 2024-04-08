#ifndef CAPSTONE_GRASS_COMPUTE_INPUT
#define CAPSTONE_GRASS_COMPUTE_INPUT

struct DrawVert {
    float3 positionWS;
    float height;
};

struct DrawTri {
    float3 shadingNormalWS;
    DrawVert verts[3];
};

struct IndirectArgs {
    uint numVerticesPerInstance;
    uint numInstances;
    uint startVertexIndex;
    uint startInstanceIndex;
};

AppendStructuredBuffer<DrawTri> _DrawTriangles;
RWStructuredBuffer<IndirectArgs> _IndirectArgsBuffer;



static int numVerts;

void BeginOutput() {
    numVerts = 0;
}

void AppendTri(DrawVert a, DrawVert b, DrawVert c, float3 normal) {
    DrawTri tri;

    tri.shadingNormalWS = normal;
    tri.verts[0] = a;
    tri.verts[1] = b;
    tri.verts[2] = c;

    _DrawTriangles.Append(tri);

    numVerts += 3;
}

void ApplyOutput() {
    InterlockedAdd(_IndirectArgsBuffer[0].numVerticesPerInstance, numVerts);
}

#endif // End of File