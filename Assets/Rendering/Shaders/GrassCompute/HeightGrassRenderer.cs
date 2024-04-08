using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode, RequireComponent(typeof(TransitionVisibility))]
public class HeightGrassRenderer : MonoBehaviour
{
    [SerializeField] Shape shape = new Shape() {
        length = 0.5f,
        lengthVariation = 0.25f,
        width = 0.2f,
        widthVariation = 0.1f,
        bend = 30f
    };
    [SerializeField] Scattering scattering = new Scattering() {
        spacing = 0.1f,
        jitter = 1,
        transition = 0.05f,
        normalSampleDist = 0.05f
    };
    [SerializeField] Terrain terrain = new Terrain() {
        size = 10,
        height = 5f
    };

    [SerializeField] Texture2D blueNoise;
    [SerializeField] ComputeShader computeShader;
    [SerializeField] Material material;


    TransitionVisibility visibility;
    ComputeShader _computeShader;
    MaterialPropertyBlock propertyBlock;
    Bounds localBounds;
    Texture2D whiteTex;
    Texture2D blackTex;

    void Awake()
    {
        whiteTex = Texture2D.whiteTexture;
        blackTex = Texture2D.blackTexture;
    }


    void LateUpdate()
    {
        
        if (!Application.isPlaying) {
            OnDisable();
            OnEnable();
        }

        DrawCompute();
        UpdatePropertyBlock();

        Bounds bounds = TransformBounds(localBounds);
        bounds.Expand(shape.length);

        Graphics.DrawProceduralIndirect(
            material, bounds, MeshTopology.Triangles, argsBuffer, 0,
            null, propertyBlock, ShadowCastingMode.Off, true, gameObject.layer
        );
    }

    void UpdatePropertyBlock() {
        propertyBlock.SetFloat("_ShowInside",  visibility.showInside  ? 1 : 0);
        propertyBlock.SetFloat("_ShowOutside", visibility.showOutside ? 1 : 0);
    }

    void DrawCompute() {

        // Reset output buffers
        drawBuffer.SetCounterValue(0);
        argsBuffer.SetData(argsBufferReset); 
        // Frame-specific data
        _computeShader.SetMatrix("_Obj2World", transform.localToWorldMatrix);

        _computeShader.Dispatch(idGrassKernel, dispatchSize, 1, 1);
    }

    ComputeBuffer drawBuffer, argsBuffer;
    const int DRAW_STRIDE = sizeof(float) * (
        3 // shadingNormalWS
        + (3 + 1) * 3 // DrawVerts (positionWS, height)
    );
    const int INDIRECT_ARGS_STRIDE = sizeof(int) * 4;

    // The data to reset the args buffer with
    // 0: vertex count per draw instance (we will only use 1)
    // 1: instance count (1)
    // 2: start vertex if using a Graphics buffer
    // 3: start instance if using a Graphics buffer
    private static readonly int[] argsBufferReset = new int[] { 0, 1, 0, 0 };

    bool initialized;
    int idGrassKernel,
        dispatchSize;

    void OnEnable()
    {
        if (initialized) { OnDisable(); }
        initialized = true;

        if (terrain.heightmap == null) return;
        var material = GetComponentInChildren<Renderer>().sharedMaterial;
        if (material.HasTexture("_MaskTex")) {
            try {
                scattering.masks = (Texture2D)material.GetTexture("_MaskTex");
            } finally {}
        }

        visibility = GetComponent<TransitionVisibility>();

        int pointsPerRow = Mathf.RoundToInt(terrain.size / scattering.spacing);
        int totalPoints = pointsPerRow * pointsPerRow;

        drawBuffer = new ComputeBuffer(totalPoints * 2, DRAW_STRIDE, ComputeBufferType.Append);
        drawBuffer.SetCounterValue(0);
        argsBuffer = new ComputeBuffer(1, INDIRECT_ARGS_STRIDE, ComputeBufferType.IndirectArguments);

        _computeShader = Instantiate(computeShader);
        idGrassKernel = _computeShader.FindKernel("CSMain");
        uint threadGroupSize;
        _computeShader.GetKernelThreadGroupSizes(idGrassKernel, out threadGroupSize, out _, out _);
        dispatchSize = Mathf.CeilToInt(totalPoints / (float)threadGroupSize);

        _computeShader.SetBuffer(idGrassKernel, "_DrawTriangles", drawBuffer);
        _computeShader.SetBuffer(idGrassKernel, "_IndirectArgsBuffer", argsBuffer);

        AddTexture("_HeightMap", terrain.heightmap);
        _computeShader.SetVector("_Size", new Vector3(terrain.size, terrain.height, terrain.size));
        _computeShader.SetInt("_Resolution", pointsPerRow);
        _computeShader.SetFloat("_NormalSampleDist", scattering.normalSampleDist);

        AddTexture("_MaskMap", scattering.masks);
        _computeShader.SetInt("_MaskChannel", (int)scattering.grassChannel);
        _computeShader.SetFloat("_Transition", scattering.transition);
        AddTexture("_BlueNoiseMap", blueNoise);
        _computeShader.SetFloat("_Jitter", scattering.jitter);

        _computeShader.SetFloat("_Length", shape.length);
        _computeShader.SetFloat("_LengthVariance", shape.lengthVariation);
        _computeShader.SetFloat("_Width", shape.width);
        _computeShader.SetFloat("_WidthVariance", shape.widthVariation);

        if (propertyBlock == null) { 
            propertyBlock = new MaterialPropertyBlock(); 
        }
        propertyBlock.SetBuffer("_DrawTriangles", drawBuffer);

        localBounds = GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
    }

    void AddTexture(string id, Texture2D tex) {
        _computeShader.SetTexture(idGrassKernel, id, tex);
        _computeShader.SetVector(id + "_TexelSize", TexelSize(tex));
    }

    Vector4 TexelSize(Texture2D tex) {
        return new Vector4(
            1f / tex.width, 1f / tex.height, 
            1f * tex.width, 1f * tex.height);
    }

    void OnDisable()
    {
        if (!initialized) return;
        
        if (drawBuffer != null) { drawBuffer.Release(); }
        if (argsBuffer != null) { argsBuffer.Release(); }

        if (Application.isPlaying) {
            Destroy(_computeShader);
        } else {
            DestroyImmediate(_computeShader);
        }
    }

    private Bounds TransformBounds(Bounds localBounds)
    {
        Vector3 aWS = transform.localToWorldMatrix.MultiplyPoint(localBounds.min);
        Vector3 bWS = transform.localToWorldMatrix.MultiplyPoint(localBounds.max);
        Vector3 minWS = new Vector3(
            Mathf.Min(aWS.x, bWS.x),
            Mathf.Min(aWS.y, bWS.y),
            Mathf.Min(aWS.z, bWS.z)
        );
        Vector3 maxWS = new Vector3(
            Mathf.Max(aWS.x, bWS.x),
            Mathf.Max(aWS.y, bWS.y),
            Mathf.Max(aWS.z, bWS.z)
        );
        return new Bounds((minWS + maxWS) / 2, maxWS - minWS);
    }



    [System.Serializable]
    struct Terrain {
        public Texture2D heightmap;
        [Min(0.1f)] public float size;
        [Min(0.1f)] public float height;
    }
    [System.Serializable]
    struct Scattering {
        [Min(0.1f)] public float spacing;
        [Range(0, 1)] public float jitter;
        public Texture2D masks;
        public ColorChannel grassChannel;
        [Range(0.01f, 1)] public float transition;
        [Range(0.01f, 0.1f)] public float normalSampleDist;
    }

    [System.Serializable]
    struct Shape {
        [Min(0)] public float length;
        [Range(0, 1)] public float lengthVariation;
        [Min(0)] public float width;
        [Range(0, 1)] public float widthVariation;
        [Min(0)] public float bend;
    }


    enum ColorChannel {
        R = 0,
        G,
        B,
        A
    }
}