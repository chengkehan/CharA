#ifndef __PBR_ALPHA_TEST__
#define __PBR_ALPHA_TEST__

#include "Assets/GameRes/Shader/PBRTexProps.hlsl"


half Alpha_float(half albedoAlpha, half4 color, half cutoff, float2 uv)
{
	half alpha = color.a;
	#ifdef _ALPHA_TEST_BASE
		alpha *= albedoAlpha;
	#endif
	#ifdef _ALPHA_TEST_TEX
		alpha *= SAMPLE_TEXTURE2D(_AlphaTestMaskTex, sampler_AlphaTestMaskTex, uv).r;
	#endif

#if defined(_ALPHA_TEST_BASE) || defined(_ALPHA_TEST_TEX)
    clip(alpha - cutoff);
	return 1;
#else
    return alpha;
#endif
}

half CutoutAlpha_RubbleTile(half4 vColor, float Mask_B_Red56)
{
#if defined(_RUBBLE_TILE)
	float VertexBlue194 = vColor.b;
	float3 temp_cast_12 = (VertexBlue194).xxx;
	float3 temp_cast_13 = (( 1.0 - Mask_B_Red56 )).xxx;
	float clampResult188 = clamp( saturate( ( 1.0 - ( ( distance( temp_cast_12 , temp_cast_13 ) - _RBT_CutAlphaRange ) / max( _RBT_CutAlphaSmooth , 1E-05 ) ) ) ) , 0.0 , 1.0 );
	float AlphaSelection189 = clampResult188;
	float temp_output_202_0 = ( 1.0 - AlphaSelection189 );
	float AlphaMap208 = temp_output_202_0;
	return AlphaMap208;
#else
	return 1;
#endif
}

half CutoutAlpha_CinderBlockRubbleTile(half4 vColor, float Mask_B_Red56)
{
#if defined(_CINDER_BLOCK_RUBBLE_TILE)
	float VertexBlue194 = vColor.b;
	float3 temp_cast_10 = (VertexBlue194).xxx;
	float3 temp_cast_11 = (( 1.0 - Mask_B_Red56 )).xxx;
	float clampResult188 = clamp( saturate( ( 1.0 - ( ( distance( temp_cast_10 , temp_cast_11 ) - _CBRT_CutAlphaRange ) / max( _CBRT_CutAlphaSmooth , 1E-05 ) ) ) ) , 0.0 , 1.0 );
	float AlphaSelection189 = clampResult188;
	float temp_output_202_0 = ( 1.0 - AlphaSelection189 );
	float AlphaMap208 = temp_output_202_0;
	return AlphaMap208;
#else
	return 0;
#endif
}

half CutoutAlpha(float4 uv, float4 uv23, half4 vColor)
{
#if defined(_RUBBLE)
	return SAMPLE_TEXTURE2D( _RB_MainTex, sampler_RB_MainTex, uv.xy ).a;
#elif defined(_RUBBLE_TILE)
	float2 temp_cast_5 = (_RBT_DetailsTiling).xx;
	float2 uv089 = float4(uv.xy,0,0).xy * temp_cast_5 + float2( 0,0 );
	float4 tex2DNode55 = SAMPLE_TEXTURE2D( _RBT_MaskB, sampler_RBT_MaskB, uv089 );
	float Mask_B_Red56 = tex2DNode55.r;
	return CutoutAlpha_RubbleTile(vColor, Mask_B_Red56);
#elif defined(_CARPET)
	float2 uv_RGBAMaskA = float4(uv.xy.xy,0,0).xy * _CP_RGBAMaskA_ST.xy + _CP_RGBAMaskA_ST.zw;
	float4 tex2DNode115 = SAMPLE_TEXTURE2D( _CP_RGBAMaskA, sampler_CP_RGBAMaskA, uv_RGBAMaskA );
	clip( tex2DNode115.a - ( vColor.g * _CP_Threshold )); // layers culling

	float2 uv4_RGBAMaskA = uv23.zw * _CP_RGBAMaskA_ST.xy + _CP_RGBAMaskA_ST.zw;
	return SAMPLE_TEXTURE2D( _CP_RGBAMaskA, sampler_CP_RGBAMaskA, uv4_RGBAMaskA ).b;
#elif defined(_CINDER_BLOCK_RUBBLE)
	float2 uv_MainTex = float4(uv.xy,0,0).xy * _CBR_MainTex_ST.xy + _CBR_MainTex_ST.zw;
	return SAMPLE_TEXTURE2D( _CBR_MainTex, sampler_CBR_MainTex, uv_MainTex ).a;
#elif defined(_CINDER_BLOCK_RUBBLE_TILE)
	float2 temp_cast_3 = (_CBRT_DetailsTiling).xx;
	float2 uv089 = float4(uv.xy,0,0).xy * temp_cast_3 + float2( 0,0 );
	float4 tex2DNode55 = SAMPLE_TEXTURE2D( _CBRT_MaskB, sampler_CBRT_MaskB, uv089 );
	float Mask_B_Red56 = tex2DNode55.r;
	return CutoutAlpha_CinderBlockRubbleTile(vColor, Mask_B_Red56);
#elif defined(_REBAR)
	float2 uv_MainTex = uv.xy * _RBR_MainTex_ST.xy + _RBR_MainTex_ST.zw;
	float4 tex2DNode1 = SAMPLE_TEXTURE2D( _RBR_MainTex, sampler_RBR_MainTex, uv_MainTex );
	return tex2DNode1.a;
#elif defined(_DOOR)
	float2 uv5_RGBAMaskB = uv.zw * _DR_RGBAMaskB_ST.xy + _DR_RGBAMaskB_ST.zw;
	float4 tex2DNode271 = SAMPLE_TEXTURE2D( _DR_RGBAMaskB, sampler_DR_RGBAMaskB, uv5_RGBAMaskB );
	float2 uv_RGBAMaskB = uv.xy * _DR_RGBAMaskB_ST.xy + _DR_RGBAMaskB_ST.zw;
	float4 tex2DNode306 = SAMPLE_TEXTURE2D( _DR_RGBAMaskB, sampler_DR_RGBAMaskB, uv_RGBAMaskB );
	float OpacityMask303 = ( tex2DNode271.g * tex2DNode306.b );
	return OpacityMask303;
#elif defined(_DRYWALL)
	float2 uv_MainTex = float4(uv.xy,0,0).xy * _DW_MainTex_ST.xy + _DW_MainTex_ST.zw;
	float4 tex2DNode363 = SAMPLE_TEXTURE2D( _DW_MainTex, sampler_DW_MainTex, uv_MainTex );
	float OpacityMask348 = tex2DNode363.a;
	return OpacityMask348;
#elif defined(_GLASS)
	float2 uv_MainTex = uv.xy * _GL_MainTex_ST.xy + _GL_MainTex_ST.zw;
	float4 tex2DNode65 = SAMPLE_TEXTURE2D( _GL_MainTex, sampler_GL_MainTex, uv_MainTex );
	half Alpha = ( tex2DNode65.b * ( 1.0 - ( tex2DNode65.r * _GL_DirtOpacity ) ) );
	return Alpha;
#elif defined(_INSULATION)
	float2 uv_InsulationMask = uv.xy * _INS_InsulationMask_ST.xy + _INS_InsulationMask_ST.zw;
	float4 tex2DNode7 = SAMPLE_TEXTURE2D( _INS_InsulationMask, sampler_INS_InsulationMask, uv_InsulationMask );
	float VertexGreen67 = vColor.g;
	clip( tex2DNode7.b - ( VertexGreen67 * _INS_Mask ));
	return 1;
#elif defined(_PLYWOOD)
	float2 uv_MainTex = float4(uv.xy,0,0).xy * _PLYW_MainTex_ST.xy + _PLYW_MainTex_ST.zw;
	float4 tex2DNode236 = SAMPLE_TEXTURE2D( _PLYW_MainTex, sampler_PLYW_MainTex, uv_MainTex );
	float OpacityMask225 = tex2DNode236.a;
	return OpacityMask225;
#elif defined(_TRIM)
	float2 uv4_RGBAMaskB = uv23.zw * _TRIM_RGBAMaskB_ST.xy + _TRIM_RGBAMaskB_ST.zw;
	float4 tex2DNode271 = SAMPLE_TEXTURE2D( _TRIM_RGBAMaskB, sampler_TRIM_RGBAMaskB, uv4_RGBAMaskB );
	float2 uv_RGBAMaskB = float4(uv.xy,0,0).xy * _TRIM_RGBAMaskB_ST.xy + _TRIM_RGBAMaskB_ST.zw;
	float4 tex2DNode306 = SAMPLE_TEXTURE2D( _TRIM_RGBAMaskB, sampler_TRIM_RGBAMaskB, uv_RGBAMaskB );
	float OpacityMask303 = ( tex2DNode271.g * tex2DNode306.b );
	return OpacityMask303;
#elif defined(_WALLPAPER)
	float2 uv_MainTex = float4(uv.xy,0,0).xy * _WP_MainTex_ST.xy + _WP_MainTex_ST.zw;
	float4 tex2DNode205 = SAMPLE_TEXTURE2D( _WP_MainTex, sampler_WP_MainTex, uv_MainTex );
	float4 OpacityMask244 = tex2DNode205;
	return OpacityMask244.r;
#elif defined(_WOOD_FLOOR)
	float2 uv4_RGBAMaskC = uv23.zw * _WF_RGBAMaskC_ST.xy + _WF_RGBAMaskC_ST.zw;
	float OpacityMask276 = SAMPLE_TEXTURE2D( _WF_RGBAMaskC, sampler_WF_RGBAMaskC, uv4_RGBAMaskC ).g;
	return OpacityMask276;
#elif defined(_CONCRETE_RUBBLE)
	float2 uv_MainTex = float4(uv.xy,0,0).xy * _CCB_MainTex_ST.xy + _CCB_MainTex_ST.zw;
	float4 tex2DNode75 = SAMPLE_TEXTURE2D( _CCB_MainTex, sampler_CCB_MainTex, uv_MainTex );
	float MaskAAlpha78 = tex2DNode75.a;
	return MaskAAlpha78;
#else
	#if defined(_ALPHA_TEST_BASE)
		return SampleAlbedoAlpha(uv.xy, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a;
	#else
		return 1;
	#endif
#endif
}

#endif