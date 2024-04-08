Shader "Capstone/Grass"
{
    Properties
    {
        _BaseColor("Base color", Color) = (0, 0.5, 0, 1)
        _TipColor("Tip color", Color) = (0, 1, 0, 1)

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
            Cull Off // Grass must be double-sided

            HLSLPROGRAM

            #pragma shader_feature_local_fragment _RECEIVE_SHADOWS

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            #pragma vertex vert
            #pragma fragment frag

            #include "Input.hlsl"
            #include "ForwardPass.hlsl"
            ENDHLSL
        }
    }
}
