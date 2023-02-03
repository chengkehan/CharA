Shader "Custom/TinyOpaque"
{
    Properties
    {
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        _Intensity("Intensity", Range(0, 3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}
        LOD 100

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Assets/GameRes/Shader/TinyCommon.hlsl"

            ENDHLSL
        }

        // Outline
		Pass
		{
			Name "Outline"
			Tags { "LightMode"="Outline" }

			Cull Back
			ZWrite Off
			ZTest LEqual
			Lighting Off

			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma multi_compile_instancing
			#pragma vertex VertOutline
			#pragma fragment FragOutline

			struct Attributes
			{
				float4 positionOS   : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS   : SV_POSITION;
			};

			Varyings VertOutline(Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);

				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				return output;
			}

			half4 FragOutline(Varyings input) : SV_Target
			{
				return 1;
			}

			ENDHLSL
		}
    }
}
