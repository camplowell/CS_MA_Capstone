using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderCameraDepth : MonoBehaviour
{
    public CapstonePipeline pipeline;
    // Start is called before the first frame update
    new Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
    }

    void Update() {
        camera.backgroundColor = pipeline.environment.transition.glow;
    }
}
