using UnityEngine;
using UnityEditor;
using System;

public abstract class CapstoneShaderGUI: ShaderGUI {
    protected MaterialEditor editor;
    protected MaterialProperty[] properties;

    bool firstHeader;
    int foldoutLevel;

    public virtual void OnOpenGUI(Material material, MaterialEditor materialEditor) {
        MaterialChanged(material);
    }

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties) {
        this.editor = editor;
        this.properties = properties;
        this.firstHeader = true;
        this.foldoutLevel = 0;

        EditorGUI.BeginChangeCheck();

        Draw();

        if (EditorGUI.EndChangeCheck()) {
            foreach (var obj in editor.targets) {
                MaterialChanged((Material)obj);
            }
        }
    }
    /// <summary>Draw the GUI</summary>
    protected abstract void Draw();

    /// <summary>Runs when the material has been changed.</summary>
    protected virtual void MaterialChanged(Material mat) {}

    /// <summary>Indents the contained code.</summary>
    protected void Indent(Action contents) {
        EditorGUI.indentLevel += 2;
        contents();
        EditorGUI.indentLevel -= 2;
    }

    /// <summary>A foldout panel</summary>
    protected void Foldout(ref bool show, string label, Action contents) {
        if (this.foldoutLevel == 0) HLine();
        show = EditorGUILayout.Foldout(show, label);
        if (show) {
            this.foldoutLevel++;
            contents();
            HLine();
            this.foldoutLevel--;
        }
    }

    protected void Space() {
        EditorGUILayout.Space();
    }

    /// <summary>ModLunar's approximation of Unity's horizontal line separator</summary>
    protected void HLine(int height = 1) {
        GUILayout.Space(4);
 
        Rect rect = GUILayoutUtility.GetRect(10, height, GUILayout.ExpandWidth(true));
        rect.height = height;
        rect.xMin = 0;
        rect.xMax = EditorGUIUtility.currentViewWidth;
    
        Color lineColor = new Color(0.10196f, 0.10196f, 0.10196f, 1);
        EditorGUI.DrawRect(rect, lineColor);
        GUILayout.Space(4);
    }
    /// <summary>Show a label.</summary>
    protected void Label(string label, GUIStyle style = null) {
        if (style == null) style = EditorStyles.boldLabel;

        GUILayout.Label(label, style);
    }

    protected void Header(string label, bool hline = true) {
        if (hline && !firstHeader) HLine();
        Label(label);
        firstHeader = false;
    }
    protected void TextureScaleOffset(string texture) {
        MaterialProperty tex = FindProperty(texture, this.properties);
        editor.TextureScaleOffsetProperty(tex);
    }
    /// <summary>Show a texture and related property.</summary>
    protected void TextureAndProp(string texture, string property, string tooltip = "") {
        MaterialProperty tex = FindProperty(texture, this.properties);
        MaterialProperty prop = FindProperty(property, this.properties);
        GUIContent label = new GUIContent(tex.displayName, tooltip);

        editor.TexturePropertySingleLine(label, tex, prop);
    }

    /// <summary>Show a texture and HDR color.</summary>
    protected void TextureAndHDR(string texture, string color, bool alpha = false, string tooltip = "") {
        MaterialProperty tex = FindProperty(texture, this.properties);
        MaterialProperty col = FindProperty(color, this.properties);
        GUIContent label = new GUIContent(tex.displayName, tooltip);

        editor.TexturePropertyWithHDRColor(label, tex, col, alpha);
    }
    /// <summary>Show a texture as a single-line thumbnail.</summary>
    protected void TextureSingleLine(string textureProperty, string tooltip = "") {
        MaterialProperty tex = FindProperty(textureProperty, this.properties);
        GUIContent label = new GUIContent(tex.displayName, tooltip);
        editor.TexturePropertySingleLine(label, tex);
    }

    /// <summary>Display a single shader property.</summary>
    protected void ShaderProperty(string propertyName, string tooltip = "") {
        MaterialProperty property = FindProperty(propertyName, this.properties);
        GUIContent label = new GUIContent(property.displayName, tooltip);
        editor.ShaderProperty(property, label);
    }

    protected void InstancingProperties() {
        editor.EnableInstancingField();
    }

    /// <summary>Set shader keywords depending on if the given textures have been assigned. </summary>
    protected static void SetupTextureKeywords(Material mat, string[] textures) {
        foreach (string texture in textures) {
            string textureKeyword = texture.ToUpper();
            SetKeyword(mat, textureKeyword, mat.GetTexture(texture));
        }
    }
    /// <summary>Set a keyword for a given material.</summary>
    protected static void SetKeyword(Material material, string keyword, bool state) {
        if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
    }
}