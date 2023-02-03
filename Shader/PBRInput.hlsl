#ifndef __PBR_INPUT__
#define __PBR_INPUT__

// ---------------------------------------------------------------------------
// Includes
// ---------------------------------------------------------------------------

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// ---------------------------------------------------------------------------
// InputData
// ---------------------------------------------------------------------------

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData) {
	inputData = (InputData)0; // avoids "not completely initalized" errors

	inputData.positionWS = input.positionWS;

	#if defined(_NORMALMAP) && !defined(_DBK) // calculate world space noraml of brick is not here 
		half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w); // viewDir has been stored in w components of these in vertex shader
		inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
	#else
		half3 viewDirWS = GetWorldSpaceNormalizeViewDir(inputData.positionWS);
		inputData.normalWS = input.normalWS;
	#endif

	inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);

	viewDirWS = SafeNormalize(viewDirWS);
	inputData.viewDirectionWS = viewDirWS;

	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
		inputData.shadowCoord = input.shadowCoord;
	#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
		inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
	#else
		inputData.shadowCoord = float4(0, 0, 0, 0);
	#endif

	// Fog
	#ifdef _ADDITIONAL_LIGHTS_VERTEX
		inputData.fogCoord = input.fogFactorAndVertexLight.x;
		inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
	#else
		inputData.fogCoord = input.fogFactorAndScreenUV.x;
		inputData.vertexLighting = half3(0, 0, 0);
	#endif

	inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
	inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
	inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
}

#endif