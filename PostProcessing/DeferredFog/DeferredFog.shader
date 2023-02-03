Shader "Hidden/DeferredFog"
{
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

	TEXTURE2D_X(_DeferredFogMainTex);
	SAMPLER(sampler_DeferredFogMainTex);

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

#if SHADER_TARGET < 35 || _USE_DRAWMESH

	struct Attributes
	{
		float4 positionOS : POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	Varyings Vertex(Attributes input)
	{
		Varyings output = (Varyings)0;

		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		output.positionCS = float4(input.positionOS.xy, UNITY_NEAR_CLIP_VALUE, 1);
		output.uv = ComputeScreenPos(output.positionCS).xy;

		return output;
	}

#else

	struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	Varyings Vertex(Attributes input)
	{
		Varyings output = (Varyings)0;

		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

		return output;
	}

#endif

	half4 Fragment(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
		half4 c = SAMPLE_TEXTURE2D_X(_DeferredFogMainTex, sampler_DeferredFogMainTex, uv);

		#if UNITY_REVERSED_Z
            real depth = SampleSceneDepth(uv);
        #else
            // Adjust Z to match NDC for OpenGL ([-1, 1])
            real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
        #endif
		float3 viewPos = ComputeViewSpacePosition(uv, depth, UNITY_MATRIX_I_P);

		float start = 1;
		float end = (20);
		float fogFade = (viewPos.z - start) / (end - start);
		fogFade = saturate(fogFade);
		//fogFade = pow(fogFade, 0.4);

		c.rgb = lerp(c.rgb, half3(0, 0.5, 0.9), fogFade);

		return c;
	}

	ENDHLSL

    // SM3.5+
	SubShader
	{
		Tags{ "RenderPipeline" = "UniversalPipeline" }

		Cull Off
		ZWrite Off
		ZTest Always
		Lighting Off

		Pass
		{
			HLSLPROGRAM

			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma shader_feature_local _USE_DRAWMESH
			#pragma vertex Vertex
			#pragma fragment Fragment

			ENDHLSL
		}
	}

	// SM2.0
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }

		Cull Off
		ZWrite Off
		ZTest Always
		Lighting Off

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vertex
			#pragma fragment Fragment

			ENDHLSL
		}
	}
}
