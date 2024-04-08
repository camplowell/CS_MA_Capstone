Shader "Capstone/Solid"
{
    Properties
    {
        [MainTexture] _BaseMap ("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1)

        _SpecGlossMap ("Specular / Roughness", 2D) = "white" {}
        _Roughness ("Roughness", Range(0, 1)) = 0.6
        _Specular ("Specular", Color) = (0.2, 0.2, 0.2)
        [Toggle(_ROUGH_MAP)] _IsRoughMap ("Roughnes Only", Float) = 0

        [HDR] _Emission ("Color", Color) = (0, 0, 0)
        _EmissionMap ("Emission", 2D) = "white" {}

        _NormalMap ("Normal", 2D) = "bump" {}
        _NormalStrength ("Strength", Range(0, 1)) = 1

        _ParallaxMap ("Displacement", 2D) = "white" {}
        _Parallax ("Strength", Range(0.005, 0.08)) = 0.005

        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 2
        [Toggle(_INTERIORGLOW)] _InteriorGlow ("Interior Glow", Float) = 1
        [Toggle(_RECEIVE_SHADOWS)] _ReceiveShadows ("Receive Shadows", Float) = 1

        
        [Toggle] [PerRendererData] _ShowInside("Show Inside", Float) = 1
        [Toggle] [PerRendererData] _ShowOutside("Show Outside", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "Forward Lit"
            Tags { "LightMode"="CapstoneUnlit" }
            Cull [_Culling]
            
            HLSLPROGRAM

            #pragma target 3.5
            #pragma multi_compile_instancing

            #pragma shader_feature_local_fragment _SPECGLOSSMAP
            #pragma shader_feature_local_fragment _ROUGH_MAP
            #pragma shader_feature_local_fragment _NORMALMAP
            #pragma shader_feature_local_fragment _PARALLAXMAP
            #pragma shader_feature_local_fragment _EMISSIONMAP
            #pragma shader_feature_local_fragment _RECEIVE_SHADOWS
            #pragma shader_feature_local_fragment _INTERIORGLOW

            #pragma vertex vert
            #pragma fragment frag

            #include "./Input.hlsl"
            #include "./ForwardPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode"="ShadowCaster" }

            BlendOp Max
            ZWrite Off
            Cull [_Culling]

            HLSLPROGRAM
            #pragma target 3.5

            #pragma multi_compile_instancing

            #pragma multi_compile_vertex _ _CASTING_DIRECTIONAL_LIGHT_SHADOW

            #pragma vertex vert
            #pragma fragment frag

            #include "./Input.hlsl"
            #include "./ShadowPass.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "CapstoneSolidGUI"
}
