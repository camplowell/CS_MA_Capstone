using UnityEngine;

[ExecuteInEditMode]
public class TransitionMarker : MonoBehaviour
{
    void LateUpdate()
    {
        UpdateShaders();
    }

    public void UpdateShaders() {
        if (CapstonePipeline.activeInstance == null) return;
        var transition = CapstonePipeline.activeInstance.environment.transition;
        transition.radius = transform.localScale.x;
        transition.position = new Vector2(transform.position.x, transform.position.z);
    }
}
