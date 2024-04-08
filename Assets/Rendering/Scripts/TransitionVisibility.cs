using UnityEngine;

public class TransitionVisibility : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [field: SerializeField] public bool showInside { get; private set; } = true;
    [field: SerializeField] public bool showOutside { get; private set; } = true;

    static MaterialPropertyBlock inside;
    static MaterialPropertyBlock outside;
    static MaterialPropertyBlock both;
    static MaterialPropertyBlock neither;
    void Awake() {
        if (renderers == null) {
            GetComponentsInChildren<Renderer>();
        }
        if (Application.isPlaying) {
            SetTransitionVisibility(true, false);
        } else {
            UpdateRenderers();
        }
    }
    void OnValidate()
    {
        renderers = GetComponentsInChildren<Renderer>();
        UpdateRenderers();
    }

    public void SetTransitionVisibility(bool inside, bool outside) {
        showInside = inside;
        showOutside = outside;
        UpdateRenderers();
    }

    private void UpdateRenderers() {
        EnsureBlocksInitialized();

        if (showInside && showOutside) {
            foreach (var renderer in renderers) {
                renderer.SetPropertyBlock(both);
            }
        } else if (showInside) {
            foreach (var renderer in renderers) {
                renderer.SetPropertyBlock(inside);
            }
        } else if (showOutside) {
            foreach (var renderer in renderers) {
                renderer.SetPropertyBlock(outside);
            }
        } else {
            foreach (var renderer in renderers) {
                renderer.SetPropertyBlock(neither);
            }
        }
    }

    void EnsureBlocksInitialized() {
        if (inside == null) {
            inside = new MaterialPropertyBlock();
            inside.SetFloat("_ShowInside", 1);
            inside.SetFloat("_ShowOutside", 0);
        }
        if (outside == null) {
            outside = new MaterialPropertyBlock();
            outside.SetFloat("_ShowInside", 0);
            outside.SetFloat("_ShowOutside", 1);
        }
        if (both == null) {
            both = new MaterialPropertyBlock();
            both.SetFloat("_ShowInside", 1);
            both.SetFloat("_ShowOutside", 1);
        }
        if (neither == null) {
            neither = new MaterialPropertyBlock();
            neither.SetFloat("_ShowInside", 0);
            neither.SetFloat("_ShowOutside", 0);
        }
    }
}