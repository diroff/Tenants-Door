Shader "FullScreen/Custom/TVNoise"
{
    Properties
    {
        _NoiseColor("Noise Color", Color) = (1, 1, 1, 1)
        _NoiseScale("Noise Scale", Vector) = (10, 10, 0, 0)
        _NoiseSpeed("Noise Speed", Float) = 10.0
        _Intensity("Intensity", Range(0.0, 1.0)) = 0.5
        _BlendMode("Blend Mode (0-Add, 1-Lerp)", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Overlay" }
        LOD 100

        Pass
        {
            Name "TV Noise Pass"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _NoiseColor;
                float2 _NoiseScale;
                float _NoiseSpeed;
                float _Intensity;
                float _BlendMode;
            CBUFFER_END

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = float4(input.vertexID == 1 ? 3.0 : -1.0, input.vertexID == 2 ? 3.0 : -1.0, 0.0, 1.0);
                output.uv = float2(input.vertexID == 1 ? 2.0 : 0.0, input.vertexID == 2 ? 2.0 : 0.0);
                
                #if UNITY_UV_STARTS_AT_TOP
                output.uv.y = 1.0 - output.uv.y;
                #endif
                
                return output;
            }

            float SimpleNoise(float2 uv, float t)
            {
                return frac(sin(dot(uv.xy + t, float2(12.9898, 78.233))) * 43758.5453123);
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 screenUV = input.uv;
                half4 sceneColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, screenUV);

                float2 noiseUV = screenUV * _NoiseScale;
                float dynamicTime = floor(_Time.y * _NoiseSpeed);
                
                float noiseVal = SimpleNoise(noiseUV, dynamicTime);
                half3 noiseFinalColor = noiseVal * _NoiseColor.rgb;

                half3 additiveMode = sceneColor.rgb + (noiseFinalColor * _Intensity);
                half3 lerpMode = lerp(sceneColor.rgb, noiseFinalColor, _Intensity);
                
                half3 resultColor = lerp(additiveMode, lerpMode, _BlendMode);

                return half4(resultColor, 1.0);
            }
            ENDHLSL
        }
    }
}