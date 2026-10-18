Shader "Sprites/Lit-Outline"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)

        [Header(Outline Settings)]
        [Toggle(_OUTLINE_ON)] _OutlineEnabled ("Enable Outline", Float) = 0
        [HDR] _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineThickness ("Outline Thickness (Pixels)", Range(0, 16)) = 2.0
        _AlphaThreshold ("Alpha Threshold", Range(0.001, 0.99)) = 0.1

        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent" 
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "SpriteLitOutline"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile_local _ _OUTLINE_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float2 screenUV     : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            TEXTURE2D(_ShapeLightTexture0);
            SAMPLER(sampler_ShapeLightTexture0);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _RendererColor;
                float2 _Flip;
                
                float4 _OutlineColor;
                float _OutlineThickness;
                float _AlphaThreshold;
            CBUFFER_END

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                
                float3 pos = input.positionOS;
                pos.xy *= _Flip.xy;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(pos);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.color = input.color * _Color * _RendererColor;

                float4 projPos = ComputeScreenPos(vertexInput.positionCS);
                output.screenUV = projPos.xy / projPos.w;

                return output;
            }

            float GetMaxNeighborAlpha(float2 uv, float thickness)
            {
                float maxAlpha = 0.0;
                float2 texelSize = _MainTex_TexelSize.xy * thickness;

                float2 offsets[8] = {
                    float2(-1,  0), float2(1,  0),
                    float2( 0, -1), float2( 0, 1),
                    float2(-1, -1), float2(1, -1),
                    float2(-1,  1), float2(1,  1)
                };

                for (int i = 0; i < 8; i++)
                {
                    float neighborAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offsets[i] * texelSize).a;
                    maxAlpha = max(maxAlpha, neighborAlpha);
                }

                return maxAlpha;
            }

            float4 Fragment(Varyings input) : SV_Target
            {
                float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float4 spriteColor = mainTex * input.color;

                float4 lightColor = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, input.screenUV);
                float3 litSpriteRGB = spriteColor.rgb * lightColor.rgb;

                #if defined(_OUTLINE_ON)
                if (_OutlineThickness > 0.0)
                {
                    float neighborAlpha = GetMaxNeighborAlpha(input.uv, _OutlineThickness);
                    float isOutline = step(_AlphaThreshold, neighborAlpha) * (1.0 - step(_AlphaThreshold, mainTex.a));
                    
                    float3 outlineRGB = _OutlineColor.rgb;
                    float outlineAlpha = _OutlineColor.a;

                    spriteColor.rgb = lerp(litSpriteRGB, outlineRGB, isOutline);
                    spriteColor.a = lerp(spriteColor.a, outlineAlpha, isOutline);
                }
                else
                {
                    spriteColor.rgb = litSpriteRGB;
                }
                #else
                spriteColor.rgb = litSpriteRGB;
                #endif

                spriteColor.rgb *= spriteColor.a;

                return spriteColor;
            }
            ENDHLSL
        }
    }
    Fallback "Sprites/Default"
}