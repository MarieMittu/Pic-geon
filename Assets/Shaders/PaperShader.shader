// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/PaperShader"
{
   Properties
   {
      _MainTex ("Source", 2D) = "white" {}
      _Intensity ("Intensity", Range(0.0, 10.0)) = 0.5
      _PaperTex ("Paper Texture", 2D) = "white" {}
   }
   SubShader
   {
      Cull Off 
      ZWrite Off 
      ZTest Always

      Pass
      {
         CGPROGRAM
         #pragma vertex vertexShader
         #pragma fragment fragmentShader
			
         #include "UnityCG.cginc"

         struct vertexInput
         {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
         };

         struct vertexOutput
         {
            float2 texcoord : TEXCOORD0;
            float4 position : SV_POSITION;
         };

         vertexOutput vertexShader(vertexInput i)
         {
            vertexOutput o;
            o.position = UnityObjectToClipPos(i.vertex);
            o.texcoord = i.texcoord;
            return o;
         }
			
         sampler2D _MainTex;
         float4 _MainTex_ST;
         float _Intensity;
         sampler2D _PaperTex;
         float4 _PaperTex_ST;

         float4 fragmentShader(vertexOutput i) : COLOR
         {
            float4 color = tex2D(_MainTex, 
               UnityStereoScreenSpaceUVAdjust(
               i.texcoord, _MainTex_ST));

            float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.position.xyz);
            //float3 viewDir = WorldSpaceViewDir(i.position);

            float4 paperColor = tex2D(_PaperTex, 
               UnityStereoScreenSpaceUVAdjust(
               i.texcoord + viewDir.xy*1, _PaperTex_ST));
            // for (int i = 0; i < 3, i++)
            // {
            //     paperColor[i] = (paperColor[i] + _Intensity) / (1.0 + _Intensity);
            // }
            //paperColor = paperColor * _Intensity + color * (1.0 - _Intensity)
            paperColor = (paperColor + _Intensity) / (1.0 + _Intensity);
            return color * paperColor;
         }
         ENDCG
      }
   }
   Fallback Off
}