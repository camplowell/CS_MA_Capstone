using UnityEngine;

public class CapstoneSolidGUI : CapstoneShaderGUI
{
    bool showAdvanced;
    protected override void Draw()
    {
        Header("Main maps");
        TextureAndProp("_BaseMap", "_BaseColor");
        TextureAndProp("_SpecGlossMap", "_IsRoughMap");
        Indent(() => {
            ShaderProperty("_Roughness");
            ShaderProperty("_Specular");
        });
        TextureAndHDR("_EmissionMap", "_Emission");
        
        Header("Detail");
        TextureAndProp("_NormalMap", "_NormalStrength");
        TextureAndProp("_ParallaxMap", "_Parallax");

        Foldout(ref showAdvanced, "Advanced", () => {
            ShaderProperty("_ReceiveShadows");
            ShaderProperty("_Culling");
            ShaderProperty("_InteriorGlow");
            InstancingProperties();
        });
    }

    protected override void MaterialChanged(Material mat)
    {
        SetupTextureKeywords(mat, new string[] {
            "_SpecGlossMap",
            "_EmissionMap",
            "_NormalMap",
            "_ParallaxMap",
        });
    }
}