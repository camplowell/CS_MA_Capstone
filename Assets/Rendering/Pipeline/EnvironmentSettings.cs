using UnityEngine;

[System.Serializable]
public class EnvironmentSettings {
    
    public Color ambientLight = new Color(0.05f, 0.2f, 0.3f);

    public TransitionSettings transition = default;


    [System.Serializable]
    public class TransitionSettings {
        public Vector2 position = Vector2.zero;
        public float radius = 4.0f;

        [ColorUsage(false, true)]
        public Color glow = Color.cyan * 2;
        public float glowExtent = 2;
        public float glowFalloff = 2;
    }
    
}