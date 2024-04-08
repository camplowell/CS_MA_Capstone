using UnityEngine;

public class CapstoneTerrainGUI : CapstoneShaderGUI
{
    bool showLayer0, showLayer1, showLayer2, showLayer3, showAdvanced;
    protected override void Draw()
    {
        TextureSingleLine("_MaskTex");
        ShaderProperty("_HeightTransition");

        Foldout(ref showLayer0, "Layer 0 (R)", () => {
            ShowMaps("0");
        });

        Foldout(ref showLayer1, "Layer 1 (G)", () => {
            ShowMaps("1");
        });

        Foldout(ref showLayer2, "Layer 2 (B)", () => {
            ShowMaps("2");
        });

        Foldout(ref showLayer3, "Layer 3 (A)", () => {
            ShowMaps("3");
        });
        
        Foldout(ref showAdvanced, "Advanced", () => {
            ShaderProperty("_ReceiveShadows");
        });
    }

    protected override void MaterialChanged(Material mat)
    {
        SetupTextureKeywords(mat, new string[] {
            "_SpecGlossMap0",
            "_SpecGlossMap1",
            "_SpecGlossMap2",
            "_SpecGlossMap3",
            "_NormalMap0",
            "_NormalMap1",
            "_NormalMap2",
            "_NormalMap3",
            "_HeightMap0",
            "_HeightMap1",
            "_HeightMap2",
            "_HeightMap3"
        });
    }

    protected void ShowMaps(string suffix) {
        TextureAndProp("_BaseMap" + suffix, "_BaseColor" + suffix);
        TextureAndProp("_SpecGlossMap" + suffix, "_IsRoughMap" + suffix);
        Indent(() => {
            ShaderProperty("_Roughness" + suffix);
            ShaderProperty("_Specular" + suffix);
        });

        TextureAndProp("_NormalMap" + suffix, "_NormalStrength" + suffix);

        TextureSingleLine("_HeightMap" + suffix);

        Header("Scale and Offset");
        TextureScaleOffset("_BaseMap" + suffix);
    }
}