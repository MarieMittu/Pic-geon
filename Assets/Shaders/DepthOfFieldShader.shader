Shader "Hidden/DepthOfFieldShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridSize("Grid Size", Integer) = 1
        _FocusDistance("Focus Distance", Float) = 2
        _DepthOfFieldSize("Depth Of Field Size", Float) = 1
        _TransitionSize("Transition Size", Float) = 0.1
        _PeripheryBlurRadius("Periphery Blur Radius", Range(0.0, 1.0)) = 0.1
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
            sampler2D _CameraDepthTexture;
            int _GridSize;
            float _FocusDistance;
            float _DepthOfFieldSize;
            float _TransitionSize;
            float _PeripheryBlurRadius;

            float getBlurStrength(float distance)
            {
                // create this shape with 2 clamped linear functions
                // 1 ________	     ________
	            // 0         \______/
                //           ^      ^
                //         near    far    - blur transition
                float nearTransition = _FocusDistance - _DepthOfFieldSize / 2;
                float farTransition  = _FocusDistance + _DepthOfFieldSize / 2;
                float falling = (nearTransition - distance) / _TransitionSize + 0.5;
                float rising  = (-farTransition + distance) / _TransitionSize + 0.5;
                return clamp(falling, 0, 1) + clamp(rising, 0, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                //linear depth between camera and far clipping plane
                depth = Linear01Depth(depth);
                //depth as distance from camera in units
                depth = depth * _ProjectionParams.z;

                float blurStrength = getBlurStrength(depth);

                // blur periphery
                float aspectRatio = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
                float2 normalizedUV = i.uv - float2(.5,.5);
                normalizedUV.x *= aspectRatio;
                float distFromCenter = distance(float2(0,0), normalizedUV);
                float radialBlurStrength = clamp((distFromCenter - _PeripheryBlurRadius) * 5, 0, 1);
                blurStrength = clamp(blurStrength + radialBlurStrength, 0, 1);

                fixed4 col = tex2D(_MainTex, i.uv);

                // blur in a Gridzize * Gridzize square around this texel
                float3 blurredColor = float3(0,0,0);
                int gridHelp = ((_GridSize - 1) / 2);
                float ignorance = 0; // how many pixels are (partially) ignored
                for (int x = -gridHelp; x <= gridHelp; x++)
                {
                    for (int y = -gridHelp; y <= gridHelp; y++)
                    {
                        float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
                        // circular blur shape
                        float ignoreFactor = clamp(2 * length(float2(x,y)) / _GridSize, 0, 1);
                        ignorance += ignoreFactor;
                        blurredColor += tex2D(_MainTex, uv).xyz * (1-ignoreFactor);
                    }
                }
                blurredColor /= _GridSize * _GridSize - ignorance;

                


                return float4(blurredColor, 1.0) * blurStrength + col * (1-blurStrength);
            }
            ENDCG
        }
    }
}
