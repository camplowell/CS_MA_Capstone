Shader "Capstone/Terrain"
{
    Properties
    {
        _HeightTransition ("Height Transition", Range(0.00001, 1)) = 0

        _MaskTex ("Mask (RGBA)", 2D) = "red" {}

        _BaseMap0 ("Albedo", 2D) = "white" {}
        _BaseColor0 ("Tint", Color) = (1, 1, 1)
        _SpecGlossMap0 ("Specular / Roughness", 2D) = "white" {}
        _Specular0 ("Specular", Color) = (0.2, 0.2, 0.2)
        _Roughness0 ("Roughness", Range(0, 1)) = 0.6
        [Toggle(_ROUGH_MAP0)] _IsRoughMap0 ("Roughnes Only", Float) = 0
        _NormalMap0 ("Normal", 2D) = "bump" {}
        _NormalStrength0 ("Strength", Range(0, 1)) = 1
        _HeightMap0 ("Height", 2D) = "grey" {}

        _BaseMap1 ("Albedo", 2D) = "white" {}
        _BaseColor1 ("Tint", Color) = (1, 1, 1)
        _SpecGlossMap1 ("Specular / Roughness", 2D) = "white" {}
        _Specular1 ("Specular", Color) = (0.2, 0.2, 0.2)
        _Roughness1 ("Roughness", Range(0, 1)) = 0.6
        [Toggle(_ROUGH_MAP1)] _IsRoughMap1 ("Roughnes Only", Float) = 0
        _NormalMap1 ("Normal", 2D) = "bump" {}
        _NormalStrength1 ("Strength", Range(0, 1)) = 1
        _HeightMap1 ("Height", 2D) = "grey" {}
        
        _BaseMap2 ("Albedo", 2D) = "white" {}
        _BaseColor2 ("Tint", Color) = (1, 1, 1)
        _SpecGlossMap2 ("Specular / Roughness", 2D) = "white" {}
        _Specular2 ("Specular", Color) = (0.2, 0.2, 0.2)
        _Roughness2 ("Roughness", Range(0, 1)) = 0.6
        [Toggle(_ROUGH_MAP2)] _IsRoughMap2 ("Roughnes Only", Float) = 0
        _NormalMap2 ("Normal", 2D) = "bump" {}
        _NormalStrength2 ("Strength", Range(0, 1)) = 1
        _HeightMap2 ("Height", 2D) = "grey" {}

        _BaseMap3 ("Albedo", 2D) = "white" {}
        _BaseColor3 ("Tint", Color) = (1, 1, 1)
        _SpecGlossMap3 ("Specular / Roughness", 2D) = "white" {}
        _Specular3 ("Specular", Color) = (0.2, 0.2, 0.2)
        _Roughness3 ("Roughness", Range(0, 1)) = 0.6
        [Toggle(_ROUGH_MAP3)] _IsRoughMap3 ("Roughnes Only", Float) = 0
        _NormalMap3 ("Normal", 2D) = "bump" {}
        _NormalStrength3 ("Strength", Range(0, 1)) = 1
        _HeightMap3 ("Height", 2D) = "grey" {}

        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 2
        [Toggle(_RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1
        [Toggle] [PerRendererData] _ShowInside("float", Float) = 1
        [Toggle] [PerRendererData] _ShowOutside("float", Float) = 1
    }
    SubShader
    {
        Tags {
            "SplatCount" = "4"
            "Queue" = "Geometry-100" 
            "RenderType"="Opaque" 
        }
        LOD 100

        Pass
        {
            Name "BaseMapShader"
            Tags { "LightMode" = "CapstoneUnlit" }
            Cull [_Culling]

            HLSLPROGRAM
            
            #pragma target 3.5
            
            #pragma shader_feature_local_fragment _RECEIVE_SHADOWS
            #pragma shader_feature_local_fragment _SPECGLOSSMapMAP0
            #pragma shader_feature_local_fragment _SPECGLOSSMAP1
            #pragma shader_feature_local_fragment _SPECGLOSSMAP2
            #pragma shader_feature_local_fragment _SPECGLOSSMAP3
            #pragma shader_feature_local_fragment _NORMALMAP0
            #pragma shader_feature_local_fragment _NORMALMAP1
            #pragma shader_feature_local_fragment _NORMALMAP2
            #pragma shader_feature_local_fragment _NORMALMAP3
            #pragma shader_feature_local_fragment _HEIGHTMAP0
            #pragma shader_feature_local_fragment _HEIGHTMAP1
            #pragma shader_feature_local_fragment _HEIGHTMAP2
            #pragma shader_feature_local_fragment _HEIGHTMAP3
            #pragma shader_feature_local_fragment _ROUGH_MAP0
            #pragma shader_feature_local_fragment _ROUGH_MAP1
            #pragma shader_feature_local_fragment _ROUGH_MAP2
            #pragma shader_feature_local_fragment _ROUGH_MAP3

            #pragma vertex vert
            #pragma fragment frag

            #include "./Input.hlsl"
            #include "./ForwardPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }

            BlendOp Max
            ZWrite Off
            Cull [_Culling]

            HLSLPROGRAM
            #pragma target 3.5
            #pragma multi_compile_vertex _ _CASTING_DIRECTIONAL_LIGHT_SHADOW

            #pragma vertex vert
            #pragma fragment frag

            #include "./Input.hlsl"
            #include "./ShadowPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "CapstoneTerrainGUI"
}
