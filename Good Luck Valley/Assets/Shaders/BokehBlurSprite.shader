Shader "Custom/BokehBlurSprite"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _BlurRadius("Blur Radius", Range(0, 20)) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _BlurRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.position = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 texelSize = 1.0 / _ScreenParams.xy;
                float2 offsets[6] = {
                    float2(-0.5, -0.5), float2(0.5, -0.5),
                    float2(-1.0, 0.0), float2(1.0, 0.0),
                    float2(-0.5, 0.5), float2(0.5, 0.5)
                };
                
                float4 col = float4(0,0,0,0);
                float totalWeight = 0.0;
                
                for (int j = 0; j < 6; j++)
                {
                    float2 offset = offsets[j] * _BlurRadius * texelSize;
                    float4 sampleColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset);
                    col += sampleColor;
                    totalWeight += 1.0;
                }
                
                col /= totalWeight;
                return col;
            }
            ENDHLSL
        }
    }
}
