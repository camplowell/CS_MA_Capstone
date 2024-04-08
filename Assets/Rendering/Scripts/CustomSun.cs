using UnityEngine;

public class CustomSun : MonoBehaviour
{
    static int
        sunCastsShadows = Shader.PropertyToID("_SunShadowStrength"),
        sunColorId = Shader.PropertyToID("_SunColor"),
        sunDirectionId = Shader.PropertyToID("_SunDirection"),
        sunViewProjId = Shader.PropertyToID("_SunViewProjection");
    
    static CustomSun instance;
    [ColorUsage(false, true)] public Color color = Color.white;

    private new Camera camera;

    void Update()
    {
        Shader.SetGlobalFloat("_SunShadowStrength", camera == null ? 0 : 1);
        Shader.SetGlobalVector("_SunDirection", transform.forward);
        Shader.SetGlobalColor("_SunColor", color);
        if (camera != null) {
            Shader.SetGlobalMatrix("_SunViewProjectionMatrix", camera.worldToCameraMatrix * camera.projectionMatrix);
        }
    }

    void UpdateCamera() {
        bool enabled = (camera != null && camera.isActiveAndEnabled);
        if (enabled) {
            camera = GetComponent<Camera>();
        } else {
            camera = null;
            Debug.LogWarning("Custom sun will not cast shadows");
        }
    }

    void OnEnable()
    {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("Multiple Custom Suns enabled; disabling");
            this.enabled = false;
        }
        UpdateCamera();
    }

    void OnDisable()
    {
        if (instance == this) instance = null;
    }

    void OnValidate()
    {
        if (enabled) UpdateCamera();
    }


}