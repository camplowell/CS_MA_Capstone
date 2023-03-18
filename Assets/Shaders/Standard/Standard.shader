Shader "Stylized/Standard"
{
    Properties
    {
        _BaseMap ("Albedo", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1, 1, 1, 1)

        _LightWrap ("Scale", Range(0.0, 1.0)) = 0.0
        _TranslucencyMap ("Subsurface Scattering", 2D) = "white" {}
        _TranslucencyColor ("Translucency", Color) = (0, 0, 0)

        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Roughness ("Roughness", Range(0.0, 1.0)) = 0.5
        _Specular ("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossMap("Specular / Roughness", 2D) = "white" {}

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        [HDR] _Emission ("Color", Color) = (0, 0, 0)
        _EmissionMap ("Emission", 2D) = "white" {}

        _Parallax("Scale", Range(0.005, 0.08)) = 0.005
        _ParallaxMap ("Parallax", 2D) = "white" {}

        _Cull("__cull", Float) = 2.0
        [ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
    }

    SubShader
    {
        Tags {"RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            // Shader features
            #pragma shader_feature_local_fragment _SPECGLOSSMAP
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

            // URP Stuff
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

            #pragma vertex vert
            #pragma fragment frag

            #include "./Input.hlsl"
            #include "./ForwardLit.hlsl"
            ENDHLSL
        }

        Pass {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Front

            HLSLPROGRAM
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Universal Pipeline keywords

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex vert
            #pragma fragment frag

            #include "./Input.hlsl"
            #include "./ShadowCaster.hlsl"
            ENDHLSL
        }
    }
    CustomEditor "StylizedStandardGUI"
}
