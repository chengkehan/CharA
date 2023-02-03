Shader "Hidden/GlobalEffect"
{
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

	TEXTURE2D_X(_GlobalEffectMainTex);
	SAMPLER(sampler_GlobalEffectMainTex);

	TEXTURE2D_X(_SkyboxSnapshot);
	SAMPLER(sampler_SkyboxSnapshot);

	TEXTURE2D_X(_SkyboxOcclusion);
	SAMPLER(sampler_SkyboxOcclusion);

	TEXTURE2D_X(_SkyboxSnapshotBlur);
	SAMPLER(sampler_SkyboxSnapshotBlur);

	TEXTURE2D_X(_HeightFogNoise);
	SAMPLER(sampler_HeightFogNoise);

	TEXTURE2D_X(_SkyboxRadiusBlur);
	SAMPLER(sampler_SkyboxRadiusBlur);

	float4 _SkyboxFogParams;
	#define _SkyboxFogBlendStart _SkyboxFogParams.x
	#define _SkyboxFogBlendEnd _SkyboxFogParams.y
	#define _SkyboxFogPow _SkyboxFogParams.z
	#define _HeightFogMinAtNight _SkyboxFogParams.w

	float4 _HeightFogParams;
	#define _HeightFogStart _HeightFogParams.x
	#define _HeightFogEnd _HeightFogParams.y
	#define _HeightFogTop _HeightFogParams.z
	#define _HeightFogBottom _HeightFogParams.w
	float4 _HeightFogParams2;
	#define _HeightFogSpeed _HeightFogParams2.x
	#define _HeightFogIntensity _HeightFogParams2.y
	#define _HeightFogScale _HeightFogParams2.z
	#define _HeightFogNoiseBlend _HeightFogParams2.w

	// 6 point lights at most.
    // this amount is also fixed in script.
    // we should change it both in shader and script at the same time.
	static const int NUMBER_POINT_LIGHTS = 6;
	uniform float4 _PointLightsPositions[NUMBER_POINT_LIGHTS]; /* xyz:position, w:range */
	uniform float4 _PointLightsColor[NUMBER_POINT_LIGHTS]; /* xyz:rgb, w:intnsity */
	#define PointLightPos(__i) _PointLightsPositions[__i].xyz
	#define PointLightRange(__i) _PointLightsPositions[__i].w
	#define PointLightColor(__i) _PointLightsColor[__i].xyz
	#define PointLightIntensity(__i) _PointLightsColor[__i].w

	uniform float _DayNightProgress;
	uniform float _SunOcclusion;

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

	float m_smoothstep(float edge0, float edge1, float x) {
		x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
		return (3.0 - 2.0 * x* x* x) * x * x* x* x* x* x;
	}


	half4 Fragment(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
		half4 c = SAMPLE_TEXTURE2D_X(_GlobalEffectMainTex, sampler_GlobalEffectMainTex, uv);
		half4 skyboxC = SAMPLE_TEXTURE2D_X(_SkyboxSnapshot, sampler_SkyboxSnapshot, uv);
		half4 skyboxR = SAMPLE_TEXTURE2D_X(_SkyboxRadiusBlur, sampler_SkyboxRadiusBlur, uv);
		half4 skyboxBlur = SAMPLE_TEXTURE2D_X(_SkyboxSnapshotBlur, sampler_SkyboxSnapshotBlur, uv);
		half4 skyboxO = SAMPLE_TEXTURE2D_X(_SkyboxOcclusion, sampler_SkyboxOcclusion, uv);
		float occlusionMask = saturate(((1 - saturate(length(skyboxC.rgb))) + skyboxO.r));
		skyboxC.rgb = skyboxC.rgb * occlusionMask + skyboxBlur.rgb * (1 - occlusionMask);

		#if UNITY_REVERSED_Z
            real depth = SampleSceneDepth(uv);
        #else
            // Adjust Z to match NDC for OpenGL ([-1, 1])
            real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
        #endif
		float3 viewPos = ComputeViewSpacePosition(uv, depth, UNITY_MATRIX_I_P);

		float fogBlend = (viewPos.z - _SkyboxFogBlendStart) / (_SkyboxFogBlendEnd - _SkyboxFogBlendStart);
		fogBlend = pow(saturate(fogBlend), _SkyboxFogPow);

		// Height Fog
		float3 wPos = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
		float heightFogHeight = _HeightFogTop - _HeightFogBottom;
#if _HEIGHT_FOG_COORD
		float heightFogY = (wPos.y + wPos.z) - _HeightFogBottom;
#else
		float heightFogY = (wPos.y) - _HeightFogBottom;
#endif
		float heightFogBlendH = 1 - saturate((viewPos.z - _HeightFogStart) / (_HeightFogEnd - _HeightFogStart));
		float heightFogBlendV = 1 - saturate((heightFogY) / (heightFogHeight));
		float heightFogBlend = heightFogBlendH * heightFogBlendV;
		float heightFogLeft = 0;
		float heightFogRight = heightFogLeft + heightFogHeight;
		float2 heightFogNoiseUV = float2(
			(wPos.x - heightFogLeft) / (heightFogRight - heightFogLeft),
			(heightFogY) / (heightFogHeight)
		);
		heightFogNoiseUV *= _HeightFogScale;
		heightFogNoiseUV.x += _Time.x * _HeightFogSpeed;
		heightFogNoiseUV.x = frac(heightFogNoiseUV.x);
		float4 heightFogNoiseC = SAMPLE_TEXTURE2D_X(_HeightFogNoise, sampler_HeightFogNoise, heightFogNoiseUV);
		float heightFogNoise = 0;
#if _HEIGHT_FOG_NOISE_BLEND_12
		heightFogNoise = lerp(heightFogNoiseC.r, heightFogNoiseC.g, _HeightFogNoiseBlend);
#elif _HEIGHT_FOG_NOISE_BLEND_23
		heightFogNoise = lerp(heightFogNoiseC.g, heightFogNoiseC.b, _HeightFogNoiseBlend);
#elif _HEIGHT_FOG_NOISE_BLEND_34
		heightFogNoise = lerp(heightFogNoiseC.b, heightFogNoiseC.a, _HeightFogNoiseBlend);
#elif _HEIGHT_FOG_NOISE_BLEND_41
		heightFogNoise = lerp(heightFogNoiseC.a, heightFogNoiseC.r, _HeightFogNoiseBlend);
#else
		// Never accessed
#endif

		heightFogBlend = heightFogBlend * heightFogNoise;
		heightFogBlend = saturate(heightFogBlend * _HeightFogIntensity);

		// point lights
		float3 pointLightC = float3(0, 0, 0);
		float pointLightBlend = 0;
		for (int pointLightI = 0; pointLightI < NUMBER_POINT_LIGHTS; ++pointLightI)
		{
			float dist = distance(wPos, PointLightPos(pointLightI));
			float range = PointLightRange(pointLightI);
			float blend = m_smoothstep(0, 1, 1 - saturate(dist / range));
			blend *= PointLightIntensity(pointLightI);
			pointLightBlend += blend;
			pointLightC += blend * PointLightColor(pointLightI);
        }
		pointLightC = saturate(pointLightC);
		pointLightBlend = saturate(pointLightBlend);

		heightFogBlend *= saturate(pointLightBlend + _DayNightProgress + _HeightFogMinAtNight);
		fogBlend = saturate(fogBlend + heightFogBlend);

		// final blending
		float3 finalBlendTo = pointLightC * pointLightBlend * fogBlend + skyboxC.rgb * (1 - pointLightBlend);
		c.rgb = lerp(c.rgb, finalBlendTo, fogBlend);
		c.rgb = c.rgb + skyboxR.rgb * 0.1 * (1 - _SunOcclusion);

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
			#pragma multi_compile_local_fragment _ _HEIGHT_FOG_COORD
			#pragma multi_compile_local_fragment _ _HEIGHT_FOG_NOISE_BLEND_12 _HEIGHT_FOG_NOISE_BLEND_23 _HEIGHT_FOG_NOISE_BLEND_34 _HEIGHT_FOG_NOISE_BLEND_41

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
			#pragma multi_compile_local_fragment _ _HEIGHT_FOG_COORD
			#pragma multi_compile_local_fragment _ _HEIGHT_FOG_NOISE_BLEND_12 _HEIGHT_FOG_NOISE_BLEND_23 _HEIGHT_FOG_NOISE_BLEND_34 _HEIGHT_FOG_NOISE_BLEND_41

			ENDHLSL
		}
	}
}
