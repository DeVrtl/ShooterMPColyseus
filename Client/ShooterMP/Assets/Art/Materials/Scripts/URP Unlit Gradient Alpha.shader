Shader "Custom/URP Unlit Gradient Alpha"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1, 1, 1, 1)
        _BottomColor ("Bottom Color", Color) = (1, 1, 1, 0)
        _GradientOffset ("Gradient Offset", Range(-1, 1)) = 0
        _GradientScale ("Gradient Scale", Range(0.1, 5)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha  // Стандартное смешивание для прозрачности
        ZWrite Off                       // Отключаем запись в Z-буфер (для корректной прозрачности)
        Cull Off                         // Рендерим обе стороны (если нужно — можно включить Back)

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _TopColor;
                float4 _BottomColor;
                float _GradientOffset;
                float _GradientScale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // UV.y идёт от 0 (снизу) до 1 (сверху)
                float t = saturate((IN.uv.y + _GradientOffset) * _GradientScale);
                
                // Лерпируем цвет и альфу одновременно
                half4 col = lerp(_BottomColor, _TopColor, t);
                
                return col;
            }
            ENDHLSL
        }
    }
    
    // Fallback для безопасности
    FallBack "Universal Render Pipeline/Unlit"
}