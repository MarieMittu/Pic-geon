Shader "Hidden/ThermalVisionEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            sampler2D _CameraDepthNormalsTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                int colSeg = 7;// number of color segments
                float3 colorRamp[8];// must be colSeg + 1
                colorRamp[0] = float3(0,0,0.01);
                colorRamp[1] = float3(0,0,1);
                colorRamp[2] = float3(1,0,1);
                colorRamp[3] = float3(1,0,0);
                colorRamp[4] = float3(1,0.2,0);
                colorRamp[5] = float3(1,0.5,0);
                colorRamp[6] = float3(1,1,0);
                colorRamp[7] = float3(1,1,1);

                float3 normalValues;
                float depthValue;
                //extract depth value and normal values
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depthValue, normalValues);

                float dotProduct = dot(normalValues, float3(0,0,1));
                // determine if object should have a heat signature
                fixed4 col = tex2D(_MainTex, i.uv);
                if (!(col.r > 0.8 && col.b > 0.8 && col.g < 0.5))
                {
                    dotProduct *= 0.3;
                }

                int index = dotProduct * colSeg;
                float ratio = fmod(dotProduct * colSeg, 1);

                
                // col = float4(dotProduct, dotProduct, dotProduct, 1.0);
                return float4(colorRamp[index] * (1-ratio) + colorRamp[index+1] * ratio, 1.0);
            }
            ENDCG
        }
    }
}