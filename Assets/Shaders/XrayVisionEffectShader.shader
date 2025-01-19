Shader "Hidden/XrayVisionEffectShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DepthTex ("Depth Texture", 2D) = "white" {}
        _XrayColor ("X-ray Color", Color) = (0, 1, 1, 1) // Default cyan
        _Intensity ("X-ray Intensity", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DepthTex;
            float4 _XrayColor;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

          fixed4 frag(v2f i) : SV_Target
            {
                float depth = tex2D(_DepthTex, i.uv).r;

                float xrayEffect = 1.0 - depth;

                xrayEffect = xrayEffect * _Intensity; 

                fixed4 col = tex2D(_MainTex, i.uv);

                fixed3 xrayColor = _XrayColor.rgb * xrayEffect;

                fixed4 finalColor = fixed4(lerp(xrayColor, col.rgb, 0.5), 1.0);

                finalColor.rgb = max(finalColor.rgb, col.rgb); 

                return finalColor;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}
