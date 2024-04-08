Shader "Hidden/Column"
{
    HLSLINCLUDE
      #include "Column.hlsl"
  ENDHLSL
  
  SubShader
  {
      Cull Off ZWrite Off ZTest Always
      Pass
      {
          HLSLPROGRAM
              #pragma vertex Vert
              #pragma fragment Frag
          ENDHLSL
      }
  }
}
