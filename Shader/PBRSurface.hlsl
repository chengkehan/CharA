#ifndef __PBR_SURFACE__
#define __PBR_SURFACE__

// ---------------------------------------------------------------------------
// Includes
// ---------------------------------------------------------------------------

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/GameRes/Shader/PBRAlphaTest.hlsl"
#include "Assets/GameRes/Shader/GetLightData.hlsl"
#include "Assets/GameRes/Shader/DBK.hlsl"

// ---------------------------------------------------------------------------
// Uniforms
// ---------------------------------------------------------------------------
uniform float3 _CubemapLighting_BBoxMin;
uniform float3 _CubemapLighting_BBoxMax;
uniform float3 _CubemapLighting_CenterPos;
uniform float3 _CubemapLighting_LightPos;
uniform float4 _CubemapLighting_DataBundle;
uniform samplerCUBE _CubemapLightingTex;
uniform sampler2D _CubemapLighting_DynamicShadowTex;
uniform float4x4 _CubemapLighting_DynamicShadowVP;
uniform float4 _CubemapLighting_Role;

uniform float _DayNightProgress;

#define _CubemapLighting_LodFactor _CubemapLighting_DataBundle.x
#define _CubemapLighting_Shadowed _CubemapLighting_DataBundle.y
#define _CubemapLighting_ShadowedRamp _CubemapLighting_DataBundle.z
#define _CubemapLighting_LightingRange _CubemapLighting_DataBundle.w

// ---------------------------------------------------------------------------
// Functions
// ---------------------------------------------------------------------------

half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha) {
	half4 specGloss;
	#ifdef _METALLICSPECGLOSSMAP
		specGloss = SAMPLE_TEXTURE2D(_MetallicSpecGlossMap, sampler_MetallicSpecGlossMap, uv);
		specGloss.rgb *= _SpecColor.rgb;
		#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			specGloss.a = albedoAlpha * _Smoothness;
		#else
			specGloss.a *= _Smoothness;
		#endif
	#else // _METALLICSPECGLOSSMAP
		#if _SPECULAR_SETUP
			specGloss.rgb = _SpecColor.rgb;
		#else
			specGloss.rgb = _Metallic.rrr;
		#endif

		#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			specGloss.a = albedoAlpha * _Smoothness;
		#else
			specGloss.a = _Smoothness;
		#endif
	#endif
	return specGloss;
}

half SampleOcclusion(float2 uv) {
	#ifdef _OCCLUSIONMAP
	#if defined(SHADER_API_GLES)
		return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
	#else
		half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
		return LerpWhiteTo(occ, _OcclusionStrength);
	#endif
	#else
		return 1.0;
	#endif
}

// ---------------------------------------------------------------------------
// SurfaceData
// ---------------------------------------------------------------------------

void FresnelEffect(float3 NormalWS, float3 ViewDirWS, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(NormalWS), normalize(ViewDirWS)))), Power);
}

void ToonShading_float(in float3 Normal, in float ToonSmoothness, in float3 WorldPos, in float4 ToonTinting,
in float ToonOffset, in float Occlusion, in float Occlusion2, in float OcclusionOffset, in float OcclusionSmoothness, in float4 inShadowTint, in float4 outShadowTint,
in float4 SSS, in float cubemapLighting, 
out float3 ToonRampOutput, out float Shadowed)
{
	// grab the shadow coordinates
	#if SHADOWS_SCREEN
		half4 shadowCoord = ComputeScreenPos(TransformWorldToHClip(WorldPos));
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
	#endif 

	// grab the main light
	#if _MAIN_LIGHT_SHADOWS_CASCADE || _MAIN_LIGHT_SHADOWS
		Light light = GetMainLight(shadowCoord);
	#else
		Light light = GetMainLight();
	#endif

	// dot product for toonramp
	half d = dot(Normal, light.direction) * 0.5 + 0.5;
		
	// toonramp in a smoothstep
	half toonRamp = smoothstep(ToonOffset, ToonOffset+ ToonSmoothness, d );
	float shadowAttenuation = light.shadowAttenuation;
	shadowAttenuation = shadowAttenuation * Occlusion;
	shadowAttenuation = smoothstep(OcclusionOffset, OcclusionOffset+ OcclusionSmoothness, shadowAttenuation );
	float shadowAttenuation2 = (1 - shadowAttenuation) * Occlusion2;
	shadowAttenuation2 = smoothstep(OcclusionOffset, OcclusionOffset+ OcclusionSmoothness, shadowAttenuation2 );
	// multiply with shadows;
	toonRamp *= shadowAttenuation;
	toonRamp = saturate(toonRamp + cubemapLighting);
	Shadowed = toonRamp;
	// add in lights and extra tinting
	float3 outShadowC = toonRamp * outShadowTint.rgb * outShadowTint.a;
	float3 inShadowC = (1 - toonRamp) * (inShadowTint.rgb * inShadowTint.a - (1 - shadowAttenuation2)*_ToonInShadowOcclusionTintOffset.rgb);
	float3 sssC = toonRamp * (1 - toonRamp) * SSS.rgb * SSS.a;
	ToonRampOutput = light.color * (toonRamp + ToonTinting.rgb + outShadowC + inShadowC + sssC) ;

}

// Rotate around a pivot point and an axis
float3 Rotate(float3 pivot, float3 position, float3 rotationAxis, float angle)
{
    rotationAxis = normalize(rotationAxis);
    float3 cpa = pivot + rotationAxis * dot(rotationAxis, position - pivot);
    return cpa + ((position - cpa) * cos(angle) + cross(rotationAxis, (position - cpa)) * sin(angle));
}

half3 AdditionalToonLighting(in Light light)
{
	half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation * _AdditionalLightRangeScale;
	half3 radiance = light.color * lightAttenuation;
	return radiance;
}

float IsInCubemapLightingBounds(float3 wPos)
{
	/*
	Optimize instructions with step function
	return wPos.x > _CubemapLighting_BBoxMin.x && wPos.y > _CubemapLighting_BBoxMin.y && wPos.z > _CubemapLighting_BBoxMin.z &&
			wPos.x < _CubemapLighting_BBoxMax.x && wPos.y < _CubemapLighting_BBoxMax.y && wPos.z < _CubemapLighting_BBoxMax.z;
	*/
	float3 cpMin = step(_CubemapLighting_BBoxMin, wPos);
	float3 cpMax = step(wPos, _CubemapLighting_BBoxMax);
	float result = dot(cpMin, cpMax);
	return step(2.5, result);
}

// Apply local correction
float3 LocalCorrect(float3 origVec, float3 bboxMin, float3 bboxMax, float3 vertexPos, float3 cubemapPos)
{
    // Local-correction code
    // Find the ray intersection with box plane
    float3 invOrigVec = float3(1.0,1.0,1.0)/origVec;
    float3 FirstPlaneIntersect = (bboxMax - vertexPos) * invOrigVec;
    float3 SecondPlaneIntersect = (bboxMin - vertexPos) * invOrigVec;
    
    // Get the furthest of these intersections along the ray
    float3 FurthestPlane = max(FirstPlaneIntersect, SecondPlaneIntersect);

    // Find the closest far intersection
    float Distance = min(min(FurthestPlane.x, FurthestPlane.y), FurthestPlane.z);

    // Get the intersection position
    float3 IntersectPositionWS = vertexPos + origVec * Distance;
    // Get corrected vector
    float3 localCorrectedVec = IntersectPositionWS - cubemapPos;

	return localCorrectedVec;
}

half3 ToonCharacter(Varyings IN, half3 diffuse, in InputData inputData)
{
	half3 o_diffuse = diffuse * _ToonIntensity;

	float3 mainLightDir = GetMainLightDir(inputData.positionWS);
	float3 mainLightDir2 = Rotate(float3(0, 0, 0), mainLightDir, float3(0, 1, 0), PI);
	float i_occlusion = saturate(dot(inputData.normalWS, mainLightDir));
	float i_occlusion2 = saturate(dot(inputData.normalWS, mainLightDir2));
	float4 i_inShadowTint = _ToonInShadowTint;
	i_inShadowTint.a *= 3;
	float4 i_outShadowTint = _ToonOutShadowTint;
	i_outShadowTint.a *= 3;
	float4 i_sssTine = _ToonSSSTint;
	i_sssTine.a *= 3;

	float additionalLightsScale = 1;

	float cubemapLighting = 0;
#ifdef _CUBEMAP_LIGHTING
	// Check if this pixel could be affected by shadows from light source
	float3 vertexToLightWS = normalize(_CubemapLighting_LightPos.xyz - inputData.positionWS);
	if (_CubemapLighting_Role.x)
	{
		vertexToLightWS.xyz *= _CubemapLighting_Role.yzw;
		vertexToLightWS = normalize(vertexToLightWS);
    }
	float3 normalWS = inputData.normalWS;
	float dotProd = dot(normalWS, vertexToLightWS);
	float isInCubemapLightingBounds = IsInCubemapLightingBounds(IN.positionWS);
	float shadowRampMax = 10;
	float shadowRamp = _CubemapLighting_ShadowedRamp;
	float shadowRampScaled = shadowRamp * shadowRampMax;
	/*
	Optimize instructions with step function
	dotProd > 0 && isInCubemapLightingBounds > 0.5
	*/
	float isFaceToLight = step(0, dotProd);
	float isInBounds = step(0.5, isInCubemapLightingBounds);
	float isConditionPasses = step(1.5, isFaceToLight + isInBounds);
	float shadowColor = 0;
	additionalLightsScale *= isInBounds;
	UNITY_BRANCH
	if (isConditionPasses)
	{
		// Apply local correction to vertex-light vector
		float3 correctVec = LocalCorrect(vertexToLightWS, _CubemapLighting_BBoxMin, _CubemapLighting_BBoxMax, inputData.positionWS, _CubemapLighting_CenterPos);

		float4 tempVec = float4(correctVec,  _CubemapLighting_LodFactor);
		// Fetch the color at a given LOD
		float4 tempCol = texCUBElod(_CubemapLightingTex, tempVec);
#ifdef _CUBEMAP_LIGHTING_DYNAMIC_SHADOW // Is Building
		tempCol.a = saturate(tempCol.a + (1 - _DayNightProgress));
#else // Is Role
		tempCol.a = saturate(tempCol.a * _DayNightProgress);
#endif

#ifdef _CUBEMAP_LIGHTING_DYNAMIC_SHADOW
		// Dynamic Shadow
		float2 screenUV = IN.fogFactorAndScreenUV.yz * IN.fogFactorAndScreenUV.w;
		if (_ProjectionParams.x < 0.0)
		{
			screenUV.y = 1.0 - screenUV.y;
		}
		
		half dynamicShadowed = tex2D(_CubemapLighting_DynamicShadowTex, screenUV).r;
		dynamicShadowed += step(screenUV.x, 0.05);
		dynamicShadowed += step(screenUV.y, 0.05);
		dynamicShadowed += step(0.95, screenUV.x);
		dynamicShadowed += step(0.95, screenUV.y);
		dynamicShadowed = saturate(dynamicShadowed);
		tempCol.a = max(1 - dynamicShadowed, tempCol.a);
#endif

		float shadowedRamp = (1 - saturate(dotProd)) * shadowRampScaled + 1;
		tempCol.a = saturate(tempCol.a * _CubemapLighting_Shadowed * shadowedRamp);
		// The shadow color will be the alpha component.
		shadowColor = (1.0 - tempCol.a);
    }
	else
	{
		float attenuation = saturate(length(IN.positionWS - _CubemapLighting_LightPos) / _CubemapLighting_LightingRange);
		// Set a color for those ones that back to light
		shadowColor = (1 - saturate(_CubemapLighting_Shadowed + shadowRamp)) * isInBounds * -dotProd * (1 - attenuation);
    }
	cubemapLighting = shadowColor;
#endif

	half3 radiance = half3(0, 0, 0);
#ifdef _ADDITIONAL_LIGHTS
	uint pixelLightCount = GetAdditionalLightsCount();
	for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
    {
        Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
		radiance += AdditionalToonLighting(light) * _AdditionalLightIntensity;
    }
#endif
	
#ifdef _ADDITIONAL_LIGHTS_VERTEX
	radiance = inputData.vertexLighting * _AdditionalLightIntensity;
#endif

	radiance *= additionalLightsScale;
	cubemapLighting = saturate(cubemapLighting + length(radiance));

	float3 o_ToonRampOutput;
	float o_Shadowed;
	ToonShading_float(inputData.normalWS, _ToonSmoothness, inputData.positionWS, _ToonTint, _ToonOffset, i_occlusion, i_occlusion2, _ToonOcclusionOffset, _ToonOcclusionSmoothness, i_inShadowTint, i_outShadowTint, i_sssTine, cubemapLighting, o_ToonRampOutput, o_Shadowed);

	float3 o_rim = 0;
#ifdef _TOON_RIM
	float o_fresnelEffect;
	FresnelEffect(inputData.normalWS, inputData.viewDirectionWS, _ToonRimPower, o_fresnelEffect);
	o_fresnelEffect *= _ToonRimIntensity * o_Shadowed;
	o_rim = o_fresnelEffect * _ToonRimColor.rgb;
#endif

	half contrast = ((o_Shadowed * 2) - 1) * _Contrast + 1;
	half3 o_color = (o_diffuse * contrast + o_rim) * o_ToonRampOutput;



	half stylizedColor = _StylizedColored + step(o_Shadowed, 0.5) * _StylizedColoredInShadow;
	half gray = dot(o_color.rgb, half3(0.2126, 0.7152, 0.0722));
    o_color.rgb = o_color.rgb * stylizedColor + gray * (1 - stylizedColor);

	half3 hatchColor = half3(1, 1, 1);
#ifdef _TOON_HATCH
	half2 hatchUV = half2(0, 0);
	#ifdef _HATCH_UV_DEFAULT
		hatchUV = IN.uv.xy * _HatchScale;
	#endif
	#ifdef _HATCH_UV_SECOND
		hatchUV = IN.uv.zw * _HatchScale;
	#endif
	#ifdef _HATCH_UV_THIRD
		hatchUV = IN.uv23.zw * _HatchScale;
	#endif

	half hatchRotationSin = 0;
	half hatchRotationCos = 0;
	sincos(_HatchRotation, hatchRotationSin, hatchRotationCos);
	float2x2 hatchRotationMatrix = float2x2( hatchRotationCos, -hatchRotationSin, hatchRotationSin, hatchRotationCos);
	hatchUV = mul ( hatchUV, hatchRotationMatrix );

	half4 hatch = SAMPLE_TEXTURE2D(_HatchTex, sampler_HatchTex, hatchUV);

	half hatchLevel = saturate(o_Shadowed + radiance * _AdditionalLightInHatch) * 4;
	half hatchLevelCheck1 = step(hatchLevel, 1.0);
	half hatchLevelCheck1Inv = 1 - hatchLevelCheck1;
	half hatchLevelCheck2 = step(hatchLevel, 2.0);
	half hatchLevelCheck2Inv = 1 - hatchLevelCheck2;
	half hatchLevelCheck3 = step(hatchLevel, 3.0);
	half hatchLevelCheck3Inv = 1 - hatchLevelCheck3;
	hatchColor =
		hatchLevelCheck1 * hatch.rrr +
		hatchLevelCheck1Inv * hatchLevelCheck2 * hatch.ggg +
		hatchLevelCheck1Inv * hatchLevelCheck2Inv * hatchLevelCheck3 * hatch.bbb +
		hatchLevelCheck1Inv * hatchLevelCheck2Inv * hatchLevelCheck3Inv * hatch.aaa;
	hatchColor = saturate(hatchColor + radiance * _AdditionalLightInHatch2 + _HatchLighting);
#endif
	o_color = (o_color * _DayNightProgress + radiance * o_color) * hatchColor; 

	return o_color;
}

half3 InitizliedNormalTS(Varyings IN)
{
#if defined(_DBK) // brick material sample normal is not in normalmap
	return half3(0.0h, 0.0h, 1.0h);
#else
	half3 normalTS = SampleNormal(IN.uv.xy, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
	return normalTS;
#endif
}

#define ONE_DIVIDE_255 0.003921568627451

half SoftLight(half a, half b)
{
	if(b <= 0.5)
	{
		return 2 * a * b + a * a * (1 - 2 * b);
    }
	else
	{
		return 2 * a * (1 - b) + (2 * b - 1) * sqrt(a);
    }
}

half4 SampleToonAlbedoAlpha(Varyings IN, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap), out float3 normalTS, out float smoothness)
{
	half4 albedo = 0;
	normalTS = 0;
	smoothness = 0;

#if defined(_DBK)
	albedo = SampleToonAlbedoAlpha_DBK(IN, normalTS, smoothness);
#else
	albedo = SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, IN.uv.xy);
#endif
	half4 result = albedo;

#ifdef _TOON_DETAIL
	half4 detail = SAMPLE_TEXTURE2D(_ToonDetailMap, sampler_ToonDetailMap, IN.uv.xy);
	half detailGray = dot(detail.rgb, half3(1, 1, 1)) * 0.333;
	detail.rgb = (detail.rgb - _ToonDetailLightingOffset) * saturate(pow(1-detailGray, _ToonDetailLighting)) + _ToonDetailLightingOffset;
	detail.rgb = lerp(_ToonDetailShadowInput, _ToonDetailHighlightInput, detail.rgb);
	detail.rgb = pow(detail.rgb, _ToonDetailMidtoneInput);
	detail.rgb = lerp(_ToonDetailShadowOutput, _ToonDetailHighlightOutput, detail.rgb * ONE_DIVIDE_255) * ONE_DIVIDE_255;
	result = half4(SoftLight(albedo.r, detail.r), SoftLight(albedo.g, detail.g), SoftLight(albedo.b, detail.b), albedo.a);
#endif

	return result;
}

uniform sampler2D _GridFadeTex;
uniform float _GridFadeAlpha;

void InitializeSurfaceData(Varyings IN, half3 normalTS, inout InputData inputData, out SurfaceData surfaceData)
{
	surfaceData = (SurfaceData)0;

#ifdef _GRID_FADE
	float2 uv = IN.fogFactorAndScreenUV.yz;
	uv *= _ScreenParams.xy * 0.125;/* divide by 8*/
	uv *= IN.fogFactorAndScreenUV.w;
	float gridAlpha = tex2D(_GridFadeTex, uv).a;
	clip(_GridFadeAlpha - gridAlpha);
#endif

	surfaceData.normalTS = normalTS;

	float3 _normalTS = 0;
	float _smoothness = 0;
	half4 albedoAlpha = SampleToonAlbedoAlpha(IN, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap), _normalTS, _smoothness);
	surfaceData.alpha = Alpha_float(albedoAlpha.a, _BaseColor, _Cutoff, IN.uv.xy);
	#if defined(_DBK) && defined(_NORMALMAP) // calculate world space normal of brick is here
		inputData.normalWS = TransformTangentToWorld(_normalTS, half3x3(IN.tangentWS.xyz, IN.bitangentWS.xyz, IN.normalWS.xyz));
		inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
		surfaceData.normalTS = _normalTS;
	#endif
	#ifdef _TOON
		surfaceData.albedo = _BaseColor.rgb;
	#else
		surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
	#endif

	half3 emission = SampleEmission(IN.uv.xy, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
	#ifdef _TOON
		surfaceData.emission = ToonCharacter(IN, albedoAlpha.rgb, inputData) + emission;
	#else
		surfaceData.emission = emission;
	#endif
	#ifdef _OCCLUSIONMAP
		surfaceData.occlusion = SampleOcclusion(IN.uv.xy);
	#else
		surfaceData.occlusion = _OcclusionStrength;
	#endif

	half4 specGloss = SampleMetallicSpecGloss(IN.uv.xy, albedoAlpha.a);
	#if _SPECULAR_SETUP
		surfaceData.metallic = 1.0h;
		surfaceData.specular = specGloss.rgb;
	#else
		surfaceData.metallic = specGloss.r;
		surfaceData.specular = half3(0.0h, 0.0h, 0.0h);
	#endif
	#if defined(_DBK)
		specGloss.a *= _smoothness;
	#endif
	surfaceData.smoothness = specGloss.a;

#ifdef _GRID_FADE
	surfaceData.emission *= pow(_GridFadeAlpha, 5);
#endif
}

#endif