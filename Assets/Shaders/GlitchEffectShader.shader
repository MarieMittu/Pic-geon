Shader "Hidden/GlitchEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Intensity;

            // i call this the marmorkuchen function because it looks a bit like marble cake when graphed
            // the purpose is to have some random looking function
            float marmorkuchen(float x)
            {
                float sinsum = sin(x*4) + sin(x*17) + sin(x*13);
                return sinsum - floor(sinsum);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float threshold = 1.0-_Intensity;

                float strength = floor(clamp(marmorkuchen(_Time)-threshold, 0, 1) * 2);

                // uv displacement is grouped into thicker lines
                // this value is in the interval [-0.15, 0.15]
                float displacement = floor(marmorkuchen(uv.y + round(_Time*100)*0.5) - 0.7) - 0.15;

                uv.x += 0.01 * displacement * strength * _Intensity;


                fixed4 col = tex2D(_MainTex, uv);
                // fixed4 colOffsetA = tex2D(_MainTex, uv + _MainTex_TexelSize.xy);
                // fixed4 colOffsetB = tex2D(_MainTex, uv - _MainTex_TexelSize.xy);
                // col.g = colOffsetA.g;
                // col.b = colOffsetB.b;

                return col;
            }
            ENDCG
        }
    }
}
