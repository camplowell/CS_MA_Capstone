using UnityEngine;
using UnityEditor;

class StylizedStandardGUI : ShaderGUI {
    MaterialEditor editor;
    MaterialProperty[] properties;

    static bool showMainMaps = true;

    public virtual void OnOpenGUI(Material material, MaterialEditor materialEditor) {
        MaterialChanged(material);
    }
    public override void OnGUI(
        MaterialEditor editor, MaterialProperty[] properties
    ) {
        this.editor = editor;
        this.properties = properties;

        EditorGUI.BeginChangeCheck();

        ShowMainMaps();

        if (EditorGUI.EndChangeCheck()) {
            foreach (var obj in editor.targets) {
                MaterialChanged((Material)obj);
            }
        }
    }

    void ShowMainMaps() {
        if (!EditorGUILayout.Foldout(showMainMaps, "Main maps")) return;
        GUILayout.Label("Main maps", EditorStyles.boldLabel);
        TextureAndProperty("_BaseMap", "_BaseColor", "Albedo (RGBA)");

        TextureAndProperty("_TranslucencyMap", "_TranslucencyColor", "Scattering radius (RGB)");
        EditorGUI.indentLevel += 2;
        ShaderProperty("_LightWrap", "Scale");
        EditorGUI.indentLevel -= 2;

        TextureAndProperty("_SpecGlossMap", "_Specular", "f0 (RGB) and roughness (alpha)");
        EditorGUI.indentLevel += 2;
        ShaderProperty("_Roughness");
        EditorGUI.indentLevel -= 2;

        
        TextureAndProperty("_BumpMap", "_BumpScale", "Tangent-space normal map");

        TextureAndHDR("_EmissionMap", "_Emission");

        TextureSingleLine("_ParallaxMap");
        EditorGUI.indentLevel += 2;
        ShaderProperty("_Parallax");
        EditorGUI.indentLevel -= 2;
    }

    void TextureAndProperty(string textureProperty, string colorProperty, string tooltip = "") {
        MaterialProperty tex = FindProperty(textureProperty, this.properties);
        GUIContent label = new GUIContent(tex.displayName, tooltip);
        MaterialProperty tint = FindProperty(colorProperty, this.properties);

		editor.TexturePropertySingleLine(label, tex, tint);
    }

    void TextureAndHDR(string textureProperty, string colorProperty, string tooltip = "") {
        MaterialProperty tex = FindProperty(textureProperty, this.properties);
        GUIContent label = new GUIContent(tex.displayName, tooltip);
        MaterialProperty tint = FindProperty(colorProperty, this.properties);

		editor.TexturePropertyWithHDRColor(label, tex, tint, false);
    }

    void TextureSingleLine(string textureProperty, string tooltip = "") {
        MaterialProperty tex = FindProperty(textureProperty, this.properties);
        GUIContent label = new GUIContent(tex.displayName, tooltip);
        editor.TexturePropertySingleLine(label, tex);
    }

    void ShaderProperty(string propertyName, string tooltip = "") {
        MaterialProperty property = FindProperty(propertyName, this.properties);
        GUIContent label = new GUIContent(property.displayName, tooltip);
        editor.ShaderProperty(property, label);
    }

    void SetupMaterialKeywords(Material material) {
        SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
        SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap"));
        SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
    }

    void MaterialChanged(Material material) {
        SetupMaterialKeywords(material);
    }

    static void SetKeyword(Material material, string keyword, bool state) {
        if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
    }
}