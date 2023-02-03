#ifndef __DBK_INCLUDE__
#define __DBK_INCLUDE__

#if defined(_BRICK) || defined(_RUBBLE) || defined(_RUBBLE_TILE) || defined(_CARPET) || defined(_CINDER_BLOCK) || defined(_CINDER_BLOCK_RUBBLE) || defined(_CINDER_BLOCK_RUBBLE_TILE) || defined(_CONCRETE) || defined(_REBAR) || defined(_DOOR) || defined(_DRYWALL) || defined(_GLASS) || defined(_GLASS_INTERIOR) || defined(_INSULATION) || defined(_PLYWOOD) || defined(_TILE) || defined(_TRIM) || defined(_WALLPAPER) || defined(_WOOD_FLOOR) || defined(_CONCRETE_RUBBLE)
	#define _DBK 1
#endif

#if defined(_BRICK)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Brick
#elif defined(_RUBBLE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Rubble
#elif defined(_RUBBLE_TILE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_RubbleTile
#elif defined(_CARPET)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Carpet
#elif defined(_CINDER_BLOCK)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_CinderBlock
#elif defined(_CINDER_BLOCK_RUBBLE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_CinderBlockRubble
#elif defined(_CINDER_BLOCK_RUBBLE_TILE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_CinderBlockRubbleTile
#elif defined(_CONCRETE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Concrete
#elif defined(_REBAR)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Rebar
#elif defined(_DOOR)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Door
#elif defined(_DRYWALL)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Drywall
#elif defined(_GLASS)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Glass
#elif defined(_GLASS_INTERIOR)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_GlassInterior
#elif defined(_INSULATION)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Insulation
#elif defined(_PLYWOOD)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Plywood
#elif defined(_TILE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Tile
#elif defined(_TRIM)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Trim
#elif defined(_WALLPAPER)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_Wallpaper
#elif defined(_WOOD_FLOOR)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_WoodFloor
#elif defined(_CONCRETE_RUBBLE)
	#define SampleToonAlbedoAlpha_DBK SampleToonAlbedoAlpha_ConcreteRubble
#else
	#define SampleToonAlbedoAlpha_DBK 0
#endif

#define CUSTOM_COLOR unity_DynamicLightmapST.x
#define WALLPAPER_NUMBER unity_DynamicLightmapST.y
#define WALLPAPER_ROW unity_DynamicLightmapST.z

half4 SampleToonAlbedoAlpha_Brick(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_BRICK)
    float staticSwitch676 = (float)CUSTOM_COLOR;
	float temp_output_659_0 = ( staticSwitch676 * 0.015625 );
	float2 appendResult667 = (float2(temp_output_659_0 , -0.75));
	float2 uv3666 = IN.uv23.zw * float2( 1,1 ) + appendResult667;
	float4 BricksColorUV3668 = SAMPLE_TEXTURE2D( _BK_MainTex, sampler_BK_MainTex, uv3666 );
	float2 uv_RGBAMaskC = IN.uv.xy * _BK_RGBAMaskC_ST.xy + _BK_RGBAMaskC_ST.zw;
	float4 tex2DNode606 = SAMPLE_TEXTURE2D( _BK_RGBAMaskC, sampler_BK_RGBAMaskC, uv_RGBAMaskC );
	float4 temp_cast_2 = (pow( tex2DNode606.r , _BK_CementBrightness )).xxxx;
	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _BK_RGBAMaskB_ST.xy + _BK_RGBAMaskB_ST.zw;
	float4 tex2DNode561 = SAMPLE_TEXTURE2D( _BK_RGBAMaskB, sampler_BK_RGBAMaskB, uv_RGBAMaskB );
	float4 lerpResult23 = lerp( BricksColorUV3668 , temp_cast_2 , tex2DNode561.r);
	float4 BricksLerp578 = lerpResult23;
	float2 uv3656 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + ( half2( 0.015625,0 ) * staticSwitch676 );
	float4 PaintColorUV3658 = SAMPLE_TEXTURE2D( _BK_MainTex, sampler_BK_MainTex, uv3656 );
	float BricksMask_Blue573 = tex2DNode561.b;
	float3 temp_cast_3 = (( 1.0 - tex2DNode606.b )).xxx;
	float VertexBlue593 = IN.color.b;
	float BricksMask_Alpha604 = tex2DNode561.a;
	float3 temp_cast_4 = (( 1.0 - ( ( 1.0 - VertexBlue593 ) * BricksMask_Alpha604 ) )).xxx;
	float PaintEdges471 = saturate( ( 1.0 - ( ( distance( temp_cast_3 , temp_cast_4 ) - _BK_WhiteBrickRange ) / max( _BK_WhiteBrickSmooth , 1E-05 ) ) ) );
	float PaintEdgesSelection476 = step( 0.1 , ( ( 1.0 - PaintEdges471 ) * step( _BK_DarkBrickAmount , PaintEdges471 ) ) );
	float2 uv_RGBA_Mask_A = float4(IN.uv.xy,0,0).xy * _BK_RGBA_Mask_A_ST.xy + _BK_RGBA_Mask_A_ST.zw;
	float4 tex2DNode371 = SAMPLE_TEXTURE2D( _BK_RGBA_Mask_A, sampler_BK_RGBA_Mask_A, uv_RGBA_Mask_A );
	float BricksDirtBlue598 = tex2DNode371.b;
	float BricksDirtRed597 = tex2DNode371.r;
	float HeightMask6 = saturate(pow(((BricksDirtBlue598*( _BK_DamageAmount * ( IN.color.g * BricksDirtRed597 ) ))*4)+(( _BK_DamageAmount * ( IN.color.g * BricksDirtRed597 ) )*2),_BK_DamageSmoothness));
	float HeightBricks385 = HeightMask6;
	float4 lerpResult10 = lerp( BricksLerp578 , ( ( ( PaintColorUV3658 + ( ( 1.0 - BricksMask_Blue573 ) * _BK_BrickOverlay ) ) + ( PaintEdges471 * _BK_WhiteBrick ) ) * ( 1.0 - ( ( PaintEdgesSelection476 * ( 1.0 - PaintEdges471 ) ) * _BK_DarkBrick ) ) ) , HeightBricks385);
	float4 BricksBrightDarkLerp581 = lerpResult10;
	float BricksHeightBlue601 = tex2DNode606.a;
	float2 appendResult649 = (float2(( staticSwitch676 * 0.015625 ) , -0.25));
	float2 uv3650 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult649;
	float4 TransitionUV3654 = SAMPLE_TEXTURE2D( _BK_MainTex, sampler_BK_MainTex, uv3650 );
	float4 TransitionColor562 = ( BricksHeightBlue601 * TransitionUV3654 );
	float BricksStep406 = (0.0 + (step( 0.1 , ( ( 1.0 - HeightBricks385 ) * step( _BK_TransitionScale , HeightBricks385 ) ) ) - 0.0) * (1.0 - 0.0) / (1.0 - 0.0));
	float4 lerpResult124 = lerp( BricksBrightDarkLerp581 , TransitionColor562 , BricksStep406);
	float4 LerpTransition584 = lerpResult124;
	float4 temp_cast_5 = (pow( tex2DNode606.g , _BK_CementBrightness )).xxxx;
	float4 lerpResult544 = lerp( BricksColorUV3668 , temp_cast_5 , tex2DNode561.g);
	float4 UniqueBricksLerp547 = lerpResult544;
	float VertexAlpha521 = IN.color.a;
	float4 lerpResult548 = lerp( LerpTransition584 , UniqueBricksLerp547 , VertexAlpha521);
	float2 appendResult660 = (float2(temp_output_659_0 , -0.5));
	float2 uv3661 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult660;
	float4 DirtColorUV3663 = SAMPLE_TEXTURE2D( _BK_MainTex, sampler_BK_MainTex, uv3661 );
	float3 temp_cast_6 = (tex2DNode371.g).xxx;
	float VertexRed592 = IN.color.r;
	float3 temp_cast_7 = (( 1.0 - VertexRed592 )).xxx;
	float temp_output_387_0 = ( tex2DNode371.a * saturate( ( 1.0 - ( ( distance( temp_cast_6 , temp_cast_7 ) - _BK_DirtRange ) / max( _BK_DirtSmooth , 1E-05 ) ) ) ) );
	float4 Albedo372 = lerp( lerpResult548 , ( BricksHeightBlue601 * DirtColorUV3663 ) , temp_output_387_0);

	float2 uv_TextureSample3 = float4(IN.uv.xy,0,0).xy * _BK_TextureSample3_ST.xy + _BK_TextureSample3_ST.zw;
	float2 uv_DBK_Brick_NM = float4(IN.uv.xy,0,0).xy * _BK_DBK_Brick_NM_ST.xy + _BK_DBK_Brick_NM_ST.zw;
	float3 lerpResult689 = lerp( SampleNormal(uv_TextureSample3, TEXTURE2D_ARGS(_BK_TextureSample3, sampler_BK_TextureSample3), _BK_BrickScale) , SampleNormal(uv_DBK_Brick_NM, TEXTURE2D_ARGS(_BK_DBK_Brick_NM, sampler_BK_DBK_Brick_NM), _BK_PaintedBrickScale) , HeightBricks385);
	float2 uv_BrickUniqueNM = float4(IN.uv.xy,0,0).xy * _BK_BrickUniqueNM_ST.xy + _BK_BrickUniqueNM_ST.zw;
	float3 lerpResult520 = lerp( lerpResult689 , SampleNormal(uv_BrickUniqueNM, TEXTURE2D_ARGS(_BK_BrickUniqueNM, sampler_BK_BrickUniqueNM), _BK_BrickUniqueScale) , VertexAlpha521);
	float3 Normals446 = lerpResult520;
	normalTS = Normals446;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float BricksTileSelection609 = tex2DNode606.r;
		float BricksDirt_Alpha614 = tex2DNode371.a;
		float lerpResult410 = lerp( ( BricksTileSelection609 * _BK_BrickBareSmoothness ) , ( _BK_BrickPaintSmoothness * ( 1.0 - ( BricksDirt_Alpha614 * _BK_BrickDirtSmoothness ) ) * ( 1.0 - ( PaintEdges471 * _BK_BrickDarkSmoothness ) ) ) , HeightBricks385);
		float BricksMainDirt625 = temp_output_387_0;
		float BricksUniqueSelection634 = tex2DNode606.g;
		float lerpResult632 = lerp( ( lerpResult410 * ( 1.0 - ( _BK_BrickDirtMainSmoothness * BricksMainDirt625 ) ) ) , ( _BK_BrickBareSmoothness * BricksUniqueSelection634 ) , VertexAlpha521);
		float Smoothness638 = lerpResult632;
		smoothness = Smoothness638;
	#endif

	half4 albedo = Albedo372;
	return albedo;
#else
    return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Rubble(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_RUBBLE)
	float4 tex2DNode133 = SAMPLE_TEXTURE2D( _RB_MainTex, sampler_RB_MainTex, IN.uv.xy );
	float Mask_A_Red134 = tex2DNode133.r;
	float4 temp_cast_0 = (Mask_A_Red134).xxxx;
	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch72 = (float)CUSTOM_COLOR;
	float2 temp_output_74_0 = ( half2( 0.015625,0 ) * staticSwitch72 );
	float2 appendResult79 = (float2(temp_output_74_0.x , 0.19));
	float2 uv080 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult79;
	float4 Color383 = SAMPLE_TEXTURE2D( _RB_ColorTheme, sampler_RB_ColorTheme, uv080 );
	float2 uv077 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + temp_output_74_0;
	float4 Color1UV381 = SAMPLE_TEXTURE2D( _RB_ColorTheme, sampler_RB_ColorTheme, uv077 );
	float2 temp_cast_4 = (_RB_DetailsTiling).xx;
	float2 uv089 = float4(IN.uv.xy,0,0).xy * temp_cast_4 + float2( 0,0 );
	float4 tex2DNode55 = SAMPLE_TEXTURE2D( _RB_MaskB, sampler_RB_MaskB, uv089 );
	float Mask_B_Red56 = tex2DNode55.r;
	float3 temp_cast_5 = (( 1.0 - Mask_B_Red56 )).xxx;
	float Mask_B_Green57 = tex2DNode55.g;
	float VertexGreen63 = IN.color.g;
	float3 temp_cast_6 = (( Mask_B_Green57 * VertexGreen63 )).xxx;
	float Mask_A_Green137 = tex2DNode133.g;
	float PaintSelection119 = ( saturate( ( 1.0 - ( ( distance( temp_cast_5 , temp_cast_6 ) - _RB_PaintRange ) / max( _RB_PaintSmooth , 1E-05 ) ) ) ) * Mask_A_Green137 );
	float4 lerpResult126 = lerp( Color383 , Color1UV381 , PaintSelection119);
	float Mask_A_Blue135 = tex2DNode133.b;
	float4 lerpResult1 = lerp( temp_cast_0 , lerpResult126 , ( 1.0 - Mask_A_Blue135 ));
	float2 appendResult87 = (float2(temp_output_74_0.x , 0.5));
	float2 uv085 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult87;
	float4 Color2UV386 = SAMPLE_TEXTURE2D( _RB_ColorTheme, sampler_RB_ColorTheme, uv085 );
	float Mask_B_Alpha59 = tex2DNode55.a;
	float VertexRed62 = IN.color.r;
	float3 temp_cast_8 = (( VertexRed62 * Mask_B_Alpha59 )).xxx;
	float3 temp_cast_9 = (( 1.0 - Mask_B_Red56 )).xxx;
	float clampResult51 = clamp( ( saturate( ( 1.0 - ( ( distance( temp_cast_8 , temp_cast_9 ) - _RB_DirtRange ) / max( _RB_DirtSmooth , 1E-05 ) ) ) ) * _RB_DirtContrast ) , 0.0 , 1.0 );
	float DirtSelection52 = clampResult51;
	float4 lerpResult68 = lerp( lerpResult1 , ( ( Color2UV386 * Mask_B_Alpha59 ) * _RB_DirtBrightness ) , DirtSelection52);
	float4 Albedo143 = ( lerpResult68 * Mask_B_Alpha59 );

	float2 uv_BakedNM = float4(IN.uv.xy,0,0).xy * _RB_BakedNM_ST.xy + _RB_BakedNM_ST.zw;
	float2 TilingAmount91 = uv089;
	float3 Normals94 = BlendNormal( SampleNormal(uv_BakedNM, TEXTURE2D_ARGS(_RB_BakedNM, sampler_RB_BakedNM), _RB_BakedNMScale) , SampleNormal(TilingAmount91, TEXTURE2D_ARGS(_RB_DetailsNM, sampler_RB_DetailsNM), _RB_DetailsNMScale) );
	normalTS = Normals94;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float Smoothnes163 = ( ( ( Mask_A_Red134 * _RB_MainSmoothness ) + ( _RB_PaintSmoothness * PaintSelection119 ) ) * ( 1.0 - DirtSelection52 ) );
		smoothness = Smoothnes163;
	#endif

	half4 albedo = Albedo143;
	albedo.a = tex2DNode133.a;
	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_RubbleTile(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_RUBBLE_TILE)
	float2 temp_cast_0 = (_RBT_MainTiling).xx;
	float2 uv0218 = IN.uv.xy * temp_cast_0 + float2( 0,0 );
	float2 Tiling220 = uv0218;
	float4 tex2DNode133 = SAMPLE_TEXTURE2D( _RBT_MainTex1, sampler_RBT_MainTex1, Tiling220 );
	float Mask_A_Red134 = tex2DNode133.r;
	float4 temp_cast_1 = (Mask_A_Red134).xxxx;
	half2 _ColorsNumber = half2(0,0.02);
	float staticSwitch72 = (float)CUSTOM_COLOR;
	float2 temp_output_74_0 = ( half2( 0.015625,0 ) * staticSwitch72 );
	float2 appendResult79 = (float2(temp_output_74_0.x , -0.86));
	float2 uv280 = IN.uv23.xy * _ColorsNumber + appendResult79;
	float4 Color383 = SAMPLE_TEXTURE2D( _RBT_MainTex, sampler_RBT_MainTex, uv280 );
	float2 uv277 = float4(IN.uv23.xy,0,0).xy * _ColorsNumber + temp_output_74_0;
	float4 Color1UV381 = SAMPLE_TEXTURE2D( _RBT_MainTex, sampler_RBT_MainTex, uv277 );
	float2 temp_cast_5 = (_RBT_DetailsTiling).xx;
	float2 uv089 = float4(IN.uv.xy,0,0).xy * temp_cast_5 + float2( 0,0 );
	float4 tex2DNode55 = SAMPLE_TEXTURE2D( _RBT_MaskB, sampler_RBT_MaskB, uv089 );
	float Mask_B_Red56 = tex2DNode55.r;
	float3 temp_cast_6 = (( 1.0 - Mask_B_Red56 )).xxx;
	float Mask_B_Green57 = tex2DNode55.g;
	float VertexGreen63 = IN.color.g;
	float3 temp_cast_7 = (( Mask_B_Green57 * VertexGreen63 )).xxx;
	float Mask_A_Green137 = tex2DNode133.g;
	float PaintSelection119 = ( saturate( ( 1.0 - ( ( distance( temp_cast_6 , temp_cast_7 ) - _RBT_PaintRange ) / max( _RBT_PaintSmooth , 1E-05 ) ) ) ) * Mask_A_Green137 );
	float4 lerpResult126 = lerp( Color383 , Color1UV381 , PaintSelection119);
	float Mask_A_Blue135 = tex2DNode133.b;
	float4 lerpResult1 = lerp( temp_cast_1 , lerpResult126 , ( 1.0 - Mask_A_Blue135 ));
	float2 appendResult87 = (float2(temp_output_74_0.x , -0.57));
	float2 uv285 = float4(IN.uv23.xy,0,0).xy * _ColorsNumber + appendResult87;
	float4 Color2UV386 = SAMPLE_TEXTURE2D( _RBT_MainTex, sampler_RBT_MainTex, uv285 );
	float Mask_B_Alpha59 = tex2DNode55.a;
	float VertexRed62 = IN.color.r;
	float3 temp_cast_9 = (( VertexRed62 * Mask_B_Alpha59 )).xxx;
	float3 temp_cast_10 = (( 1.0 - Mask_B_Red56 )).xxx;
	float clampResult51 = clamp( ( saturate( ( 1.0 - ( ( distance( temp_cast_9 , temp_cast_10 ) - _RBT_DirtRange ) / max( _RBT_DirtSmooth , 1E-05 ) ) ) ) * _RBT_DirtContrast ) , 0.0 , 1.0 );
	float DirtSelection52 = clampResult51;
	float4 lerpResult68 = lerp( lerpResult1 , ( ( Color2UV386 * Mask_B_Alpha59 ) * _RBT_DirtBrightness ) , DirtSelection52);
	float4 Albedo143 = ( lerpResult68 * Mask_B_Alpha59 );

	float2 TilingAmount91 = uv089;
	float3 Normals94 = BlendNormal( SampleNormal(Tiling220, TEXTURE2D_ARGS(_RBT_BakedNM, sampler_RBT_BakedNM), _RBT_BakedNMScale) , SampleNormal(TilingAmount91, TEXTURE2D_ARGS(_RBT_DetailsNM, sampler_RBT_DetailsNM), _RBT_DetailsNMScale) );
	normalTS = Normals94;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float Smoothnes163 = ( ( ( Mask_A_Red134 * _RBT_MainSmoothness ) + ( _RBT_PaintSmoothness * PaintSelection119 ) ) * ( 1.0 - DirtSelection52 ) );
		smoothness = Smoothnes163;
	#endif

	half4 albedo = Albedo143;
	albedo.a = CutoutAlpha_RubbleTile(IN.color, Mask_B_Red56);
	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Carpet(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_CARPET)
	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch168 = (float)CUSTOM_COLOR;
	float2 temp_output_154_0 = ( half2( 0.015625,0 ) * staticSwitch168 );
	float2 uv0158 = IN.uv.xy * _ColorsNumber + temp_output_154_0;
	float4 PrimaryColor146 = SAMPLE_TEXTURE2D( _CP_MainTex, sampler_CP_MainTex, uv0158 );
	float2 uv_RGBAMaskA = float4(IN.uv.xy,0,0).xy * _CP_RGBAMaskA_ST.xy + _CP_RGBAMaskA_ST.zw;
	float4 tex2DNode115 = SAMPLE_TEXTURE2D( _CP_RGBAMaskA, sampler_CP_RGBAMaskA, uv_RGBAMaskA );
	float2 appendResult155 = (float2(temp_output_154_0.x , 0.5));
	float2 uv0157 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult155;
	float4 UndersideColorUV3145 = SAMPLE_TEXTURE2D( _CP_MainTex, sampler_CP_MainTex, uv0157 );
	float2 uv4_RGBAMaskA = IN.uv23.zw * _CP_RGBAMaskA_ST.xy + _CP_RGBAMaskA_ST.zw;
	float4 tex2DNode39 = SAMPLE_TEXTURE2D( _CP_RGBAMaskA, sampler_CP_RGBAMaskA, uv4_RGBAMaskA );
	float3 temp_cast_3 = (pow( ( 1.0 - tex2DNode39.b ) , _CP_Edges )).xxx;
	float3 desaturateInitialColor43 = temp_cast_3;
	float desaturateDot43 = dot( desaturateInitialColor43, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar43 = lerp( desaturateInitialColor43, desaturateDot43.xxx, 1.0 );
	float3 CutoutEdges44 = desaturateVar43;
	float4 lerpResult47 = lerp( ( ( PrimaryColor146 * tex2DNode115.r ) * ( IN.color.g + _CP_Brightness ) * _CP_Darkness ) , ( UndersideColorUV3145 * tex2DNode115.a ) , float4( ( ( ( 1.0 - IN.color.b ) * CutoutEdges44 ) + IN.color.b ) , 0.0 ));
	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _CP_RGBAMaskB_ST.xy + _CP_RGBAMaskB_ST.zw;
	float4 tex2DNode102 = SAMPLE_TEXTURE2D( _CP_RGBAMaskB, sampler_CP_RGBAMaskB, uv_RGBAMaskB );
	float CarpetMaskAlpha108 = tex2DNode102.a;
	float2 appendResult161 = (float2(temp_output_154_0.x , 0.1));
	float2 uv0163 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult161;
	float4 DirtColorUV3147 = SAMPLE_TEXTURE2D( _CP_MainTex, sampler_CP_MainTex, uv0163 );
	float CarpetMaskBlue106 = tex2DNode102.b;
	float3 temp_cast_6 = (( tex2DNode102.g * _CP_DirtMultiplier )).xxx;
	float VertexRed124 = IN.color.r;
	float3 temp_cast_7 = (( 1.0 - ( VertexRed124 * tex2DNode102.r ) )).xxx;
	float DirtHeight17 = saturate( ( 1.0 - ( ( distance( temp_cast_6 , temp_cast_7 ) - _CP_DirtRange ) / max( _CP_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult31 = lerp( lerpResult47 , ( ( CarpetMaskAlpha108 * DirtColorUV3147 ) * IN.color.g * ( CarpetMaskBlue106 + tex2DNode115.a ) ) , ( DirtHeight17 * _CP_DirtOpacity ));
	float4 Albedo99 = lerpResult31;

	clip( tex2DNode115.a - ( IN.color.g * _CP_Threshold )); // layers culling

	normalTS = float3(0, 0, 1);

	half4 albedo = Albedo99;
	albedo.a = tex2DNode39.b;
	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_CinderBlock(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_CINDER_BLOCK)
	float staticSwitch562 = (float)CUSTOM_COLOR;
	float2 uv3444 = IN.uv23.zw * float2( 1,1 ) + ( half2( 0.015625,0 ) * staticSwitch562 );
	float4 PaintColorUV3447 = SAMPLE_TEXTURE2D( _CB_MainTex, sampler_CB_MainTex, uv3444 );
	float2 uv_RGBAMaskA = IN.uv.xy * _CB_RGBAMaskA_ST.xy + _CB_RGBAMaskA_ST.zw;
	float4 tex2DNode356 = SAMPLE_TEXTURE2D( _CB_RGBAMaskA, sampler_CB_RGBAMaskA, uv_RGBAMaskA );
	float temp_output_38_0 = pow( tex2DNode356.r , _CB_EdgesMultiply );
	float2 uv_RGBAMaskC = float4(IN.uv.xy,0,0).xy * _CB_RGBAMaskC_ST.xy + _CB_RGBAMaskC_ST.zw;
	float4 tex2DNode556 = SAMPLE_TEXTURE2D( _CB_RGBAMaskC, sampler_CB_RGBAMaskC, uv_RGBAMaskC );
	float RGBA_Mask_C_Blue557 = tex2DNode556.b;
	float temp_output_442_0 = ( staticSwitch562 * 0.015625 );
	float2 appendResult445 = (float2(temp_output_442_0 , -0.75));
	float2 uv3448 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult445;
	float4 BricksColorUV3450 = SAMPLE_TEXTURE2D( _CB_MainTex, sampler_CB_MainTex, uv3448 );
	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _CB_RGBAMaskB_ST.xy + _CB_RGBAMaskB_ST.zw;
	float4 tex2DNode351 = SAMPLE_TEXTURE2D( _CB_RGBAMaskB, sampler_CB_RGBAMaskB, uv_RGBAMaskB );
	float HeightMask10 = saturate(pow(((( 1.0 - tex2DNode351.g )*( ( ( tex2DNode351.r * _CB_DamageMultiplier ) * ( 1.0 - IN.color.g ) ) * _CB_DamageAmount ))*4)+(( ( ( tex2DNode351.r * _CB_DamageMultiplier ) * ( 1.0 - IN.color.g ) ) * _CB_DamageAmount )*2),_CB_DamageSmoothness));
	float Heightmap11 = HeightMask10;
	float MainShapeSelect298 = step( 0.06 , Heightmap11 );
	float4 lerpResult14 = lerp( ( ( PaintColorUV3447 * temp_output_38_0 ) + ( tex2DNode356.g * _CB_EdgesOverlay * PaintColorUV3447 ) ) , ( ( RGBA_Mask_C_Blue557 * BricksColorUV3450 ) * temp_output_38_0 ) , MainShapeSelect298);
	float2 appendResult436 = (float2(( staticSwitch562 * 0.015625 ) , -0.25));
	float2 uv3438 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult436;
	float4 TransitionUV3443 = SAMPLE_TEXTURE2D( _CB_MainTex, sampler_CB_MainTex, uv3438 );
	float RGBA_Mask_A_Alpha549 = tex2DNode356.a;
	float3 temp_cast_2 = (( 1.0 - SAMPLE_TEXTURE2D( _CB_RGBAMaskC, sampler_CB_RGBAMaskC, uv_RGBAMaskC ).r )).xxx;
	float VertexBlue365 = IN.color.b;
	float3 temp_cast_3 = (( 1.0 - ( ( 1.0 - VertexBlue365 ) * tex2DNode556.g ) )).xxx;
	float temp_output_482_0 = saturate( ( 1.0 - ( ( distance( temp_cast_2 , temp_cast_3 ) - _CB_PaintBareRange ) / max( _CB_PaintBareSmooth , 1E-05 ) ) ) );
	float PaintConcreteSelection512 = temp_output_482_0;
	float temp_output_517_0 = ( PaintConcreteSelection512 * ( 1.0 - MainShapeSelect298 ) );
	float temp_output_537_0 = step( 0.1 , temp_output_517_0 );
	float4 lerpResult493 = lerp( lerpResult14 , ( TransitionUV3443 * RGBA_Mask_A_Alpha549 * _CB_ChippedPaintBrightness ) , temp_output_537_0);
	float PaintEdges496 = temp_output_482_0;
	float PaintEdgesSelection501 = step( 0.1 , ( ( 1.0 - PaintEdges496 ) * step( _CB_PaintSharpness , PaintEdges496 ) ) );
	float RGBA_Mask_C_Alpha559 = tex2DNode556.a;
	float HeightDamage403 = Heightmap11;
	float clampResult411 = clamp( step( 0.1 , ( ( 1.0 - HeightDamage403 ) * step( 0.06 , HeightDamage403 ) ) ) , 0.0 , 1.0 );
	float TransitionSelect234 = clampResult411;
	float4 lerpResult428 = lerp( ( lerpResult493 + ( lerpResult493 * ( PaintEdgesSelection501 * _CB_PaintEdgesMultiply ) ) ) , ( TransitionUV3443 * RGBA_Mask_C_Alpha559 * _CB_TransitionBrighthness ) , TransitionSelect234);
	float2 appendResult451 = (float2(temp_output_442_0 , -0.5));
	float2 uv3452 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult451;
	float4 DirtColorUV3454 = SAMPLE_TEXTURE2D( _CB_MainTex, sampler_CB_MainTex, uv3452 );
	float RGBA_Mask_A_Blue357 = tex2DNode356.b;
	float3 temp_cast_4 = (( tex2DNode351.b * _CB_DirtMultiplier )).xxx;
	float3 temp_cast_5 = (( 1.0 - IN.color.r )).xxx;
	float DirtHeight74 = saturate( ( 1.0 - ( ( distance( temp_cast_4 , temp_cast_5 ) - _CB_DirtRange ) / max( _CB_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult30 = lerp( lerpResult428 , ( DirtColorUV3454 * RGBA_Mask_A_Blue357 ) , ( DirtHeight74 * _CB_DirtOpacity ));
	float4 Albedo103 = lerpResult30;
	half4 albedo = Albedo103;

	float2 uv_PaintNM = float4(IN.uv.xy,0,0).xy * _CB_PaintNM_ST.xy + _CB_PaintNM_ST.zw;
	float2 uv_PaintDamageNM = float4(IN.uv.xy,0,0).xy * _CB_PaintDamageNM_ST.xy + _CB_PaintDamageNM_ST.zw;
	float ChippedPaintSelection542 = temp_output_537_0;
	float3 lerpResult419 = lerp( SampleNormal(uv_PaintNM, TEXTURE2D_ARGS(_CB_PaintNM, sampler_CB_PaintNM), _CB_PaintNMScale) , SampleNormal(uv_PaintDamageNM, TEXTURE2D_ARGS(_CB_PaintDamageNM, sampler_CB_PaintDamageNM), _CB_PaintDamageNMScale) , ( MainShapeSelect298 + ( ChippedPaintSelection542 * _CB_ChippedPaintScale ) ));
	float2 uv_TransitionNM = float4(IN.uv.xy,0,0).xy * _CB_TransitionNM_ST.xy + _CB_TransitionNM_ST.zw;
	float3 lerpResult421 = lerp( lerpResult419 , SampleNormal(uv_TransitionNM, TEXTURE2D_ARGS(_CB_TransitionNM, sampler_CB_TransitionNM), _CB_TransitionNMScale) , TransitionSelect234);
	float3 Normals92 = lerpResult421;
	normalTS = Normals92;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float RedChannelRGBA109 = tex2DNode351.r;
		float RGBA_Mask_B_Blue123 = tex2DNode351.b;
		float Smoothness117 = ( ( _CB_SmoothnessMain * RedChannelRGBA109 * ( 1.0 - RGBA_Mask_B_Blue123 ) ) * ( 1.0 - ( _CB_SmoothnessDamage * Heightmap11 ) ) * ( 1.0 - ( _CB_SmoothnessDirt * DirtHeight74 ) ) * ( 1.0 - ( _CB_PaintSmoothness * ChippedPaintSelection542 ) ) );
		smoothness = Smoothness117;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_CinderBlockRubble(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_CINDER_BLOCK_RUBBLE)
	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch72 = (float)CUSTOM_COLOR;
	float2 temp_output_74_0 = ( half2( 0.015625,0 ) * staticSwitch72 );
	float2 appendResult79 = (float2(temp_output_74_0.x , 0.2));
	float2 uv080 = IN.uv.xy * _ColorsNumber + appendResult79;
	float4 Color383 = SAMPLE_TEXTURE2D( _CBR_ColorTheme, sampler_CBR_ColorTheme, uv080 );
	float2 uv_MainTex = float4(IN.uv.xy,0,0).xy * _CBR_MainTex_ST.xy + _CBR_MainTex_ST.zw;
	float4 tex2DNode133 = SAMPLE_TEXTURE2D( _CBR_MainTex, sampler_CBR_MainTex, uv_MainTex );
	float Mask_A_Red134 = tex2DNode133.r;
	float2 uv077 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + temp_output_74_0;
	float4 Color1UV381 = SAMPLE_TEXTURE2D( _CBR_ColorTheme, sampler_CBR_ColorTheme, uv077 );
	float2 temp_cast_3 = (_CBR_DetailsTiling).xx;
	float2 uv089 = float4(IN.uv.xy,0,0).xy * temp_cast_3 + float2( 0,0 );
	float4 tex2DNode55 = SAMPLE_TEXTURE2D( _CBR_MaskB, sampler_CBR_MaskB, uv089 );
	float Mask_B_Red56 = tex2DNode55.r;
	float3 temp_cast_4 = (( 1.0 - Mask_B_Red56 )).xxx;
	float Mask_B_Green57 = tex2DNode55.g;
	float VertexGreen63 = IN.color.g;
	float3 temp_cast_5 = (( Mask_B_Green57 * VertexGreen63 )).xxx;
	float Mask_A_Green137 = tex2DNode133.g;
	float PaintSelection119 = ( saturate( ( 1.0 - ( ( distance( temp_cast_4 , temp_cast_5 ) - _CBR_PaintRange ) / max( _CBR_PaintSmooth , 1E-05 ) ) ) ) * Mask_A_Green137 );
	float4 lerpResult126 = lerp( ( Color383 * Mask_A_Red134 ) , Color1UV381 , PaintSelection119);
	float2 appendResult87 = (float2(temp_output_74_0.x , 0.5));
	float2 uv085 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult87;
	float4 Color2UV386 = SAMPLE_TEXTURE2D( _CBR_ColorTheme, sampler_CBR_ColorTheme, uv085 );
	float Mask_B_Alpha59 = tex2DNode55.a;
	float VertexRed62 = IN.color.r;
	float3 temp_cast_7 = (( VertexRed62 * Mask_B_Alpha59 )).xxx;
	float3 temp_cast_8 = (( 1.0 - Mask_B_Red56 )).xxx;
	float clampResult51 = clamp( ( saturate( ( 1.0 - ( ( distance( temp_cast_7 , temp_cast_8 ) - _CBR_DirtRange ) / max( _CBR_DirtSmooth , 1E-05 ) ) ) ) * _CBR_DirtContrast ) , 0.0 , 1.0 );
	float DirtSelection52 = clampResult51;
	float4 lerpResult68 = lerp( lerpResult126 , ( ( Color2UV386 * Mask_B_Alpha59 ) * _CBR_DirtBrightness ) , DirtSelection52);
	float Mask_A_Blue135 = tex2DNode133.b;
	float4 Albedo143 = ( ( lerpResult68 * Mask_B_Alpha59 ) * ( 1.0 - ( Mask_A_Blue135 * _CBR_AODirt ) ) );
	half4 albedo = Albedo143;
	albedo.a = tex2DNode133.a;

	float2 uv_BakedNM = float4(IN.uv.xy,0,0).xy * _CBR_BakedNM_ST.xy + _CBR_BakedNM_ST.zw;
	float2 TilingAmount91 = uv089;
	float3 Normals94 = BlendNormal( SampleNormal(uv_BakedNM, TEXTURE2D_ARGS(_CBR_BakedNM, sampler_CBR_BakedNM), 1.0f) , SampleNormal(TilingAmount91, TEXTURE2D_ARGS(_CBR_DetailsNM, sampler_CBR_DetailsNM), _CBR_DetailsNMScale) );
	normalTS = Normals94;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float Smoothnes163 = ( ( ( Mask_A_Red134 * _CBR_MainSmoothness ) + ( _CBR_PaintSmoothness * PaintSelection119 ) ) * ( 1.0 - DirtSelection52 ) );
		smoothness = Smoothnes163;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_CinderBlockRubbleTile(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_CINDER_BLOCK_RUBBLE_TILE)
	float staticSwitch72 = (float)CUSTOM_COLOR;
	float2 temp_output_74_0 = ( half2( 0.015625,0 ) * staticSwitch72 );
	float2 appendResult79 = (float2(temp_output_74_0.x , 0.1));
	float2 uv380 = IN.uv23.zw * _CBRT_ColorsNumber + appendResult79;
	float4 Color383 = SAMPLE_TEXTURE2D( _CBRT_MainTex, sampler_CBRT_MainTex, uv380 );
	float2 uv_MainTex1 = IN.uv.xy * _CBRT_MainTex1_ST.xy + _CBRT_MainTex1_ST.zw;
	float4 tex2DNode133 = SAMPLE_TEXTURE2D( _CBRT_MainTex1, sampler_CBRT_MainTex1, uv_MainTex1 );
	float Mask_A_Red134 = tex2DNode133.r;
	float2 uv377 = float4(IN.uv23.zw,0,0).xy * _CBRT_ColorsNumber + temp_output_74_0;
	float4 Color1UV381 = SAMPLE_TEXTURE2D( _CBRT_MainTex, sampler_CBRT_MainTex, uv377 );
	float2 temp_cast_3 = (_CBRT_DetailsTiling).xx;
	float2 uv089 = float4(IN.uv.xy,0,0).xy * temp_cast_3 + float2( 0,0 );
	float4 tex2DNode55 = SAMPLE_TEXTURE2D( _CBRT_MaskB, sampler_CBRT_MaskB, uv089 );
	float Mask_B_Red56 = tex2DNode55.r;
	float3 temp_cast_4 = (( 1.0 - Mask_B_Red56 )).xxx;
	float Mask_B_Green57 = tex2DNode55.g;
	float VertexGreen63 = IN.color.g;
	float3 temp_cast_5 = (( Mask_B_Green57 * VertexGreen63 )).xxx;
	float Mask_A_Green137 = tex2DNode133.g;
	float PaintSelection119 = ( saturate( ( 1.0 - ( ( distance( temp_cast_4 , temp_cast_5 ) - _CBRT_PaintRange ) / max( _CBRT_PaintSmooth , 1E-05 ) ) ) ) * Mask_A_Green137 );
	float4 lerpResult126 = lerp( ( Color383 * Mask_A_Red134 ) , Color1UV381 , PaintSelection119);
	float2 appendResult87 = (float2(temp_output_74_0.x , 0.5));
	float2 uv385 = float4(IN.uv23.zw,0,0).xy * _CBRT_ColorsNumber + appendResult87;
	float4 Color2UV386 = SAMPLE_TEXTURE2D( _CBRT_MainTex, sampler_CBRT_MainTex, uv385 );
	float Mask_B_Alpha59 = tex2DNode55.a;
	float VertexRed62 = IN.color.r;
	float3 temp_cast_7 = (( VertexRed62 * Mask_B_Alpha59 )).xxx;
	float3 temp_cast_8 = (( 1.0 - Mask_B_Red56 )).xxx;
	float clampResult51 = clamp( ( saturate( ( 1.0 - ( ( distance( temp_cast_7 , temp_cast_8 ) - _CBRT_DirtRange ) / max( _CBRT_DirtSmooth , 1E-05 ) ) ) ) * _CBRT_DirtContrast ) , 0.0 , 1.0 );
	float DirtSelection52 = clampResult51;
	float4 lerpResult68 = lerp( lerpResult126 , ( ( Color2UV386 * Mask_B_Alpha59 ) * _CBRT_DirtBrightness ) , DirtSelection52);
	float4 Albedo143 = ( lerpResult68 * Mask_B_Alpha59 );
	half4 albedo = Albedo143;

	float2 uv_BrickTileNM = float4(IN.uv.xy,0,0).xy * _CBRT_BrickTileNM_ST.xy + _CBRT_BrickTileNM_ST.zw;
	float2 TilingAmount91 = uv089;
	float3 Normals94 = BlendNormal( SampleNormal(uv_BrickTileNM, TEXTURE2D_ARGS(_CBRT_BrickTileNM, sampler_CBRT_BrickTileNM), _CBRT_BrickNMScale) , SampleNormal(TilingAmount91, TEXTURE2D_ARGS(_CBRT_DetailsNM, sampler_CBRT_DetailsNM), _CBRT_DetailsNMScale) );
	normalTS = Normals94;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float Smoothnes163 = ( ( ( Mask_A_Red134 * _CBRT_MainSmoothness ) + ( _CBRT_PaintSmoothness * PaintSelection119 ) ) * ( 1.0 - DirtSelection52 ) );
		smoothness = Smoothnes163;
	#endif

	albedo.a = CutoutAlpha_CinderBlockRubbleTile(IN.color, Mask_B_Red56);
	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Concrete(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_CONCRETE)
	float2 temp_cast_0 = (_CC_ConcreteDamageTiling).xx;
	float2 uv0385 = IN.uv.xy * temp_cast_0 + float2( 0,0 );
	float2 ConcreteTileUV386 = uv0385;
	float4 tex2DNode185 = SAMPLE_TEXTURE2D( _CC_DamageColor, sampler_CC_DamageColor, ConcreteTileUV386 );
	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _CC_RGBAMaskB_ST.xy + _CC_RGBAMaskB_ST.zw;
	float4 tex2DNode636 = SAMPLE_TEXTURE2D( _CC_RGBAMaskB, sampler_CC_RGBAMaskB, uv_RGBAMaskB );
	float ConcreteNewAlpha640 = tex2DNode636.a;
	float staticSwitch635 = (float)CUSTOM_COLOR;
	float2 appendResult613 = (float2(( staticSwitch635 * 0.015625 ) , -0.33));
	float2 uv3611 = IN.uv23.zw * float2( 1,1 ) + appendResult613;
	float4 BareConcrete622 = SAMPLE_TEXTURE2D( _CC_MainTex, sampler_CC_MainTex, uv3611 );
	float4 temp_output_559_0 = ( ConcreteNewAlpha640 * BareConcrete622 * _CC_BareBrightness );
	float HeightMask58 = saturate(pow(((( 1.0 - tex2DNode636.r )*( _CC_DamageAmount * ( ( IN.color.g * _CC_DamageMultiplier ) * tex2DNode636.g ) ))*4)+(( _CC_DamageAmount * ( ( IN.color.g * _CC_DamageMultiplier ) * tex2DNode636.g ) )*2),_CC_DamageAmountSmooth));
	float HeightBricks203 = HeightMask58;
	float BricksStep210 = step( 0.1 , ( ( 1.0 - HeightBricks203 ) * step( _CC_TransitionAmount , HeightBricks203 ) ) );
	float4 lerpResult459 = lerp( ( tex2DNode185 * _CC_DamageBrightness ) , ( ( temp_output_559_0 + tex2DNode185 ) * _CC_TransitionBrightness * ConcreteNewAlpha640 ) , BricksStep210);
	float2 uv3612 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + ( half2( 0.015625,0 ) * staticSwitch635 );
	float4 PaintColor619 = SAMPLE_TEXTURE2D( _CC_MainTex, sampler_CC_MainTex, uv3612 );
	float HeightGreen564 = tex2DNode636.b;
	float3 temp_cast_3 = (( 1.0 - HeightGreen564 )).xxx;
	float VertexBlue392 = IN.color.b;
	float2 uv_RGBAMaskA = float4(IN.uv.xy,0,0).xy * _CC_RGBAMaskA_ST.xy + _CC_RGBAMaskA_ST.zw;
	float4 tex2DNode325 = SAMPLE_TEXTURE2D( _CC_RGBAMaskA, sampler_CC_RGBAMaskA, uv_RGBAMaskA );
	float DirtMaskBlue413 = tex2DNode325.b;
	float3 temp_cast_4 = (( 1.0 - ( ( 1.0 - VertexBlue392 ) * DirtMaskBlue413 ) )).xxx;
	float temp_output_426_0 = saturate( ( 1.0 - ( ( distance( temp_cast_3 , temp_cast_4 ) - _CC_ConcreteBareRange ) / max( _CC_ConcreteBareSmooth , 1E-05 ) ) ) );
	float PaintEdges465 = temp_output_426_0;
	float PaintEdgesSelection471 = step( 0.1 , ( ( 1.0 - PaintEdges465 ) * step( 0.0 , PaintEdges465 ) ) );
	float4 lerpResult397 = lerp( PaintColor619 , ( temp_output_559_0 * ( 1.0 - ( PaintEdgesSelection471 * _CC_PaintEdgesMultiply ) ) ) , temp_output_426_0);
	float temp_output_228_0 = pow( tex2DNode325.r , _CC_DirtOverlay );
	float PaintSelection313 = ( ( 1.0 - HeightBricks203 ) * ( 1.0 - BricksStep210 ) );
	float4 lerpResult117 = lerp( lerpResult459 , ( ( lerpResult397 + ( lerpResult397 * _CC_EdgesOverlay * tex2DNode325.g ) ) * temp_output_228_0 ) , PaintSelection313);
	float2 appendResult628 = (float2(( staticSwitch635 * 0.015625 ) , -0.66));
	float2 uv3627 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult628;
	float4 DirtColor630 = SAMPLE_TEXTURE2D( _CC_MainTex, sampler_CC_MainTex, uv3627 );
	float VertexRed336 = IN.color.r;
	float3 temp_cast_5 = (( VertexRed336 * ( 1.0 - tex2DNode325.g ) )).xxx;
	float3 temp_cast_6 = (( 1.0 - HeightGreen564 )).xxx;
	float temp_output_309_0 = ( ( tex2DNode325.a * saturate( ( 1.0 - ( ( distance( temp_cast_5 , temp_cast_6 ) - _CC_DirtRange ) / max( _CC_DirtSmooth , 1E-05 ) ) ) ) ) * _CC_DirtContrast );
	float4 lerpResult308 = lerp( lerpResult117 , ( DirtColor630 * ConcreteNewAlpha640 ) , temp_output_309_0);
	float DamageAlpha599 = tex2DNode185.a;
	float lerpResult596 = lerp( ConcreteNewAlpha640 , DamageAlpha599 , PaintSelection313);
	float4 Albedo323 = ( lerpResult308 * lerpResult596 );
	half4 albedo = Albedo323;

	float2 uv_ConcreteNM = float4(IN.uv.xy,0,0).xy * _CC_ConcreteNM_ST.xy + _CC_ConcreteNM_ST.zw;
	float2 uv_TransitionNM = float4(IN.uv.xy,0,0).xy * _CC_TransitionNM_ST.xy + _CC_TransitionNM_ST.zw;
	float3 lerpResult1 = lerp( SampleNormal(uv_ConcreteNM, TEXTURE2D_ARGS(_CC_ConcreteNM, sampler_CC_ConcreteNM), 1.0f) , SampleNormal(uv_TransitionNM, TEXTURE2D_ARGS(_CC_TransitionNM, sampler_CC_TransitionNM), _CC_TransitionScale)  , BricksStep210);
	float3 lerpResult68 = lerp( lerpResult1 , SampleNormal(ConcreteTileUV386, TEXTURE2D_ARGS(_CC_DamageNM, sampler_CC_DamageNM), _CC_ConcreteDamageScale) , HeightBricks203);
	float3 Normals320 = lerpResult68;
	normalTS = Normals320;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float PaintSmoothness351 = temp_output_228_0;
		float DirtSmothness366 = temp_output_309_0;
		float ConcreteDamage375 = tex2DNode185.r;
		float lerpResult349 = lerp( ( ( PaintSmoothness351 * _CC_MainSmoothness ) * ( 1.0 - ( DirtSmothness366 * _CC_DirtSmoothness ) ) * ( 1.0 - ( PaintEdges465 * _CC_BareSmoothness ) ) ) , ( ConcreteDamage375 * _CC_DamageSmoothness ) , ( 1.0 - PaintSelection313 ));
		float Smoothness359 = lerpResult349;
		smoothness = Smoothness359;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Rebar(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_REBAR)
	float2 uv_MainTex = IN.uv.xy * _RBR_MainTex_ST.xy + _RBR_MainTex_ST.zw;
	float4 tex2DNode1 = SAMPLE_TEXTURE2D( _RBR_MainTex, sampler_RBR_MainTex, uv_MainTex );
	half4 albedo = tex2DNode1;

	float2 uv_Normals = float4(IN.uv.xy,0,0).xy * _RBR_Normals_ST.xy + _RBR_Normals_ST.zw;
	float3 Normal001 = SampleNormal(uv_Normals, TEXTURE2D_ARGS(_RBR_Normals, sampler_RBR_Normals), _RBR_NormalsScale);
	normalTS = Normal001;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float2 uv_Smoothness = float4(IN.uv.xy,0,0).xy * _RBR_Smoothness_ST.xy + _RBR_Smoothness_ST.zw;
		float Smoothness = SAMPLE_TEXTURE2D( _RBR_Smoothness, sampler_RBR_Smoothness, uv_Smoothness ).r;
		smoothness = Smoothness;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Door(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_DOOR)
	float2 uv_DoorsColor = IN.uv.xy * _DR_DoorsColor_ST.xy + _DR_DoorsColor_ST.zw;
	float4 tex2DNode150 = SAMPLE_TEXTURE2D( _DR_DoorsColor, sampler_DR_DoorsColor, uv_DoorsColor );
	float staticSwitch376 = (float)CUSTOM_COLOR;
	float2 appendResult378 = (float2(( staticSwitch376 * 0.015625 ) , -0.33333));
	float2 uv3380 = IN.uv23.zw * float2( 1,1 ) + appendResult378;
	float4 Color2UV3325 = SAMPLE_TEXTURE2D( _DR_MainTex, sampler_DR_MainTex, uv3380 );
	float2 uv_RGBAMaskA = IN.uv.xy * _DR_RGBAMaskA_ST.xy + _DR_RGBAMaskA_ST.zw;
	float4 tex2DNode219 = SAMPLE_TEXTURE2D( _DR_RGBAMaskA, sampler_DR_RGBAMaskA, uv_RGBAMaskA );
	float MaskBlueChannel221 = tex2DNode219.b;
	float3 temp_cast_2 = (( 1.0 - MaskBlueChannel221 )).xxx;
	float VertexBlue308 = IN.color.b;
	float3 temp_cast_3 = (( 1.0 - VertexBlue308 )).xxx;
	float2 uv5_RGBAMaskB = IN.uv.zw * _DR_RGBAMaskB_ST.xy + _DR_RGBAMaskB_ST.zw;
	float4 tex2DNode271 = SAMPLE_TEXTURE2D( _DR_RGBAMaskB, sampler_DR_RGBAMaskB, uv5_RGBAMaskB );
	float3 temp_cast_4 = (pow( ( 1.0 - tex2DNode271.g ) , _DR_TransitionEdgeAmount )).xxx;
	float3 desaturateInitialColor295 = temp_cast_4;
	float desaturateDot295 = dot( desaturateInitialColor295, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar295 = lerp( desaturateInitialColor295, desaturateDot295.xxx, 1.0 );
	float3 CutoutEdges296 = desaturateVar295;
	float4 lerpResult288 = lerp( tex2DNode150 , Color2UV3325 , float4( ( ( 1.0 - saturate( ( 1.0 - ( ( distance( temp_cast_2 , temp_cast_3 ) - _DR_TransitionAmount ) / max( 0.8514541 , 1E-05 ) ) ) ) ) + CutoutEdges296 ) , 0.0 ));
	float2 uv3385 = IN.uv23.zw * float2( 1,1 ) + ( half2( 0.015625,0 ) * staticSwitch376 );
	float4 Color1UV3327 = SAMPLE_TEXTURE2D( _DR_MainTex, sampler_DR_MainTex, uv3385 );
	float MaskRedChannelX369 = tex2DNode219.a;
	float clampResult393 = clamp( ( MaskRedChannelX369 + _DR_WoodGrain ) , 0.0 , 1.0 );
	float4 lerpResult147 = lerp( ( Color1UV3327 * clampResult393 ) , tex2DNode150 , pow( tex2DNode150.a , _DR_MaskOverlay ));
	float saferPower56 = max( tex2DNode219.b , 0.0001 );
	float HeightMask8 = saturate(pow(((tex2DNode219.g*( ( pow( saferPower56 , _DR_DamagePower ) * ( ( 1.0 - IN.color.g ) + ( 1.0 - _DR_DamageMultiply ) ) ) * ( 1.0 - _DR_DamageAmount ) ))*4)+(( ( pow( saferPower56 , _DR_DamagePower ) * ( ( 1.0 - IN.color.g ) + ( 1.0 - _DR_DamageMultiply ) ) ) * ( 1.0 - _DR_DamageAmount ) )*2),_DR_DamageSmooth));
	float DamageSelection212 = ( 1.0 - HeightMask8 );
	float4 lerpResult149 = lerp( lerpResult288 , lerpResult147 , ( 1.0 - DamageSelection212 ));
	float MaskAlphaChannelX227 = tex2DNode219.r;
	float3 temp_cast_6 = (MaskBlueChannel221).xxx;
	float VertexRed234 = IN.color.r;
	float3 temp_cast_7 = (( 1.0 - VertexRed234 )).xxx;
	float temp_output_190_0 = ( ( MaskAlphaChannelX227 * saturate( ( 1.0 - ( ( distance( temp_cast_6 , temp_cast_7 ) - _DR_DirtRange ) / max( _DR_DirtSmooth , 1E-05 ) ) ) ) ) * _DR_DirtContrast );
	float4 lerpResult194 = lerp( lerpResult149 , ( Color2UV3325 * MaskAlphaChannelX227 * _DR_DirtBrightness ) , temp_output_190_0);
	float4 Albedo210 = lerpResult194;
	half4 albedo = Albedo210;

	float2 uv_DoorNM = IN.uv.xy * _DR_DoorNM_ST.xy + _DR_DoorNM_ST.zw;
	float3 unpack4 = SampleNormal(uv_DoorNM, TEXTURE2D_ARGS(_DR_DoorNM, sampler_DR_DoorNM), _DR_DoorsNMScale);
	unpack4.z = lerp( 1, unpack4.z, saturate(_DR_DoorsNMScale) );
	float2 uv_DoorDamageNM = IN.uv.xy * _DR_DoorDamageNM_ST.xy + _DR_DoorDamageNM_ST.zw;
	float3 unpack2 = SampleNormal(uv_DoorDamageNM, TEXTURE2D_ARGS(_DR_DoorDamageNM, sampler_DR_DoorDamageNM), _DR_DoorsDamageNMScale);
	unpack2.z = lerp( 1, unpack2.z, saturate(_DR_DoorsDamageNMScale) );
	float3 lerpResult17 = lerp( unpack4 , unpack2 , DamageSelection212);
	float3 Normals208 = lerpResult17;
	normalTS = Normals208;

	float2 uv_RGBAMaskB = IN.uv.xy * _DR_RGBAMaskB_ST.xy + _DR_RGBAMaskB_ST.zw;
	float4 tex2DNode306 = SAMPLE_TEXTURE2D( _DR_RGBAMaskB, sampler_DR_RGBAMaskB, uv_RGBAMaskB );
	float Metallic352 = tex2DNode306.a;

	float OpacityMask303 = ( tex2DNode271.g * tex2DNode306.b );
	albedo.a = OpacityMask303;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float DirtSelection248 = temp_output_190_0;
		float Smoothness260 = ( pow( tex2DNode306.r , _DR_MainSmoothness ) * ( 1.0 - ( MaskBlueChannel221 * _DR_SmoothnessOverlay ) ) * ( 1.0 - ( _DR_SmoothnessDirt * DirtSelection248 ) ) * ( 1.0 - ( _DR_SmoothnessDamage * DamageSelection212 ) ) );
		smoothness = Smoothness260;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Drywall(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_DRYWALL)
	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch415 = (float)CUSTOM_COLOR;
	float2 temp_output_404_0 = ( half2( 0.015625,0 ) * staticSwitch415 );
	float2 uv0406 = IN.uv.xy * _ColorsNumber + temp_output_404_0;
	float4 PaintColorUV3389 = SAMPLE_TEXTURE2D( _DW_ColorTheme, sampler_DW_ColorTheme, uv0406 );
	float2 uv4_MainTex = IN.uv23.zw * _DW_MainTex_ST.xy + _DW_MainTex_ST.zw;
	float4 tex2DNode356 = SAMPLE_TEXTURE2D( _DW_MainTex, sampler_DW_MainTex, uv4_MainTex );
	float temp_output_38_0 = pow( tex2DNode356.r , _DW_EdgesMultiply );
	float2 appendResult405 = (float2(temp_output_404_0.x , 0.5));
	float2 uv0410 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult405;
	float4 PaperColorUV3388 = SAMPLE_TEXTURE2D( _DW_ColorTheme, sampler_DW_ColorTheme, uv0410 );
	float2 uv4_RGBAMaskA = float4(IN.uv23.zw,0,0).xy * _DW_RGBAMaskA_ST.xy + _DW_RGBAMaskA_ST.zw;
	float4 tex2DNode351 = SAMPLE_TEXTURE2D( _DW_RGBAMaskA, sampler_DW_RGBAMaskA, uv4_RGBAMaskA );
	float HeightMask10 = saturate(pow(((( 1.0 - tex2DNode351.g )*( ( ( tex2DNode351.r * _DW_DamageMultiplier ) * IN.color.g ) * _DW_DamageAmount ))*4)+(( ( ( tex2DNode351.r * _DW_DamageMultiplier ) * IN.color.g ) * _DW_DamageAmount )*2),_DW_DamageSmoothness));
	float Heightmap11 = HeightMask10;
	float temp_output_311_0 = step( 0.1 , Heightmap11 );
	float CombinedWhiteEdges298 = temp_output_311_0;
	float4 lerpResult14 = lerp( ( ( PaintColorUV3389 * temp_output_38_0 ) + ( tex2DNode356.g * _DW_EdgesOverlay * PaintColorUV3389 ) ) , ( PaperColorUV3388 * temp_output_38_0 ) , CombinedWhiteEdges298);
	float HeightBricks227 = Heightmap11;
	float BricksStep234 = ( temp_output_311_0 * ( 1.0 - step( 0.1 , ( ( 1.0 - HeightBricks227 ) * step( _DW_TransitionAmount , HeightBricks227 ) ) ) ) );
	float RedChannelRGBA109 = tex2DNode351.r;
	float3 temp_cast_3 = (pow( RedChannelRGBA109 , _DW_ExtraTransition )).xxx;
	float VertexBlue365 = IN.color.b;
	float3 temp_cast_4 = (( 1.0 - VertexBlue365 )).xxx;
	float2 uv_MainTex = float4(IN.uv.xy,0,0).xy * _DW_MainTex_ST.xy + _DW_MainTex_ST.zw;
	float4 tex2DNode363 = SAMPLE_TEXTURE2D( _DW_MainTex, sampler_DW_MainTex, uv_MainTex );
	float3 temp_cast_5 = (pow( ( 1.0 - tex2DNode363.a ) , _DW_EdgeBrightness )).xxx;
	float3 desaturateInitialColor341 = temp_cast_5;
	float desaturateDot341 = dot( desaturateInitialColor341, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar341 = lerp( desaturateInitialColor341, desaturateDot341.xxx, 1.0 );
	float3 CutoutEdges342 = desaturateVar341;
	float3 clampResult346 = clamp( CutoutEdges342 , float3( 0,0,0 ) , float3( 1,1,1 ) );
	float3 temp_output_327_0 = ( BricksStep234 * ( ( 1.0 - step( 0.1 , ( 1.0 - saturate( ( 1.0 - ( ( distance( temp_cast_3 , temp_cast_4 ) - _DW_ExtraTransitionRange ) / max( 1.0 , 1E-05 ) ) ) ) ) ) ) + clampResult346 ) );
	float2 appendResult408 = (float2(temp_output_404_0.x , 0.1));
	float2 uv0409 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult408;
	float4 DirtColorUV3390 = SAMPLE_TEXTURE2D( _DW_ColorTheme, sampler_DW_ColorTheme, uv0409 );
	float MasksBlueChannel357 = tex2DNode356.b;
	float3 temp_cast_8 = (( tex2DNode351.b * _DW_DirtMultiplier )).xxx;
	float3 temp_cast_9 = (( 1.0 - IN.color.r )).xxx;
	float DirtHeight74 = saturate( ( 1.0 - ( ( distance( temp_cast_8 , temp_cast_9 ) - _DW_DirtRange ) / max( _DW_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult30 = lerp( ( lerpResult14 + float4( ( _DW_TransitionBrightness * temp_output_327_0 ) , 0.0 ) ) , ( DirtColorUV3390 * _DW_DirtBrightness * MasksBlueChannel357 ) , ( DirtHeight74 * _DW_DirtOpacity ));
	float VertexAlpha394 = IN.color.a;
	float4 Albedo103 = ( lerpResult30 + ( _DW_InsideBrightness * VertexAlpha394 ) );
	float OpacityMask348 = tex2DNode363.a;
	half4 albedo = Albedo103;
	albedo.a = OpacityMask348;

	float2 uv4_DrywallNM = float4(IN.uv23.zw,0,0).xy * _DW_DrywallNM_ST.xy + _DW_DrywallNM_ST.zw;
	float2 uv4_DrywallDamageNM = float4(IN.uv23.zw,0,0).xy * _DW_DrywallDamageNM_ST.xy + _DW_DrywallDamageNM_ST.zw;
	float3 EdgesDamages199 = temp_output_327_0;
	float3 lerpResult21 = lerp( SampleNormal(uv4_DrywallNM, TEXTURE2D_ARGS(_DW_DrywallNM, sampler_DW_DrywallNM), _DW_DrywallNmScale) , SampleNormal(uv4_DrywallDamageNM, TEXTURE2D_ARGS(_DW_DrywallDamageNM, sampler_DW_DrywallDamageNM), _DW_DamageNMScale) , EdgesDamages199);
	float3 Normals92 = lerpResult21;
	normalTS = Normals92;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float GreenChannelRGBA123 = tex2DNode351.b;
		float Smoothness117 = ( ( _DW_SmoothnessMain * RedChannelRGBA109 * ( 1.0 - GreenChannelRGBA123 ) ) * ( 1.0 - ( _DW_SmoothnessDamage * Heightmap11 ) ) * ( 1.0 - ( _DW_SmoothnessDirt * DirtHeight74 ) ) );
		smoothness = Smoothness117;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Glass(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_GLASS)
	float2 uv_MainTex = IN.uv.xy * _GL_MainTex_ST.xy + _GL_MainTex_ST.zw;
	float4 tex2DNode65 = SAMPLE_TEXTURE2D( _GL_MainTex, sampler_GL_MainTex, uv_MainTex );
	float2 uv055 = IN.uv.xy * half2( 0,-0.1 ) + ( half2( 0.015625,0 ) * CUSTOM_COLOR );
	float4 ColorSelection63 = SAMPLE_TEXTURE2D( _GL_ColorTheme, sampler_GL_ColorTheme, uv055 );
	half4 albedo = ( tex2DNode65.r * ColorSelection63 * _GL_GlassBrigthness );
	half Alpha = ( tex2DNode65.b * ( 1.0 - ( tex2DNode65.r * _GL_DirtOpacity ) ) );
	albedo.a = Alpha;

	float2 uv_Glass_Normals = IN.uv.xy * _GL_Glass_Normals_ST.xy + _GL_Glass_Normals_ST.zw;
	normalTS = SampleNormal(uv_Glass_Normals, TEXTURE2D_ARGS(_GL_Glass_Normals, sampler_GL_Glass_Normals), 1.0f);

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		smoothness = 0.5;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_GlassInterior(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_GLASS_INTERIOR)

	float2 uv_WallTex = IN.uv.xy * _GLI_WallTex_ST.xy + _GLI_WallTex_ST.zw;
	float2 uv_MainTex149 = float4(IN.uv.xy,0,0).xy;
	float4 tex2DNode149 = SAMPLE_TEXTURE2D( _GLI_MainTex, sampler_GLI_MainTex, uv_MainTex149 );
	float2 uv0164 = float4(IN.uv.xy,0,0).xy * half2( 0,-0.1 ) + ( half2( 0.015625,0 ) * CUSTOM_COLOR );
	float4 lerpResult152 = lerp( ( _GLI_WallBrightness * SAMPLE_TEXTURE2D( _GLI_WallTex, sampler_GLI_WallTex, uv_WallTex ) ) , ( tex2DNode149.r * SAMPLE_TEXTURE2D( _GLI_ColorTheme, sampler_GLI_ColorTheme, uv0164 ) ) , tex2DNode149.g);
	half4 albedo = lerpResult152;

	float2 uv_GlassNM = float4(IN.uv.xy,0,0).xy * _GLI_GlassNM_ST.xy + _GLI_GlassNM_ST.zw;
	normalTS = SampleNormal(uv_GlassNM, TEXTURE2D_ARGS( _GLI_GlassNM, sampler_GLI_GlassNM), 1.0f );

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		smoothness = ( tex2DNode149.r * _GLI_SmoothnessValue );
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Insulation(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_INSULATION)
	float2 uv_InsulationMask = IN.uv.xy * _INS_InsulationMask_ST.xy + _INS_InsulationMask_ST.zw;
	float4 tex2DNode7 = SAMPLE_TEXTURE2D( _INS_InsulationMask, sampler_INS_InsulationMask, uv_InsulationMask );
	float MaskBlue70 = tex2DNode7.b;
	float2 uv382 = IN.uv23.zw * float2( 1,1 ) + ( half2( 0.015625,0 ) * CUSTOM_COLOR );
	float4 ColorUV396 = SAMPLE_TEXTURE2D( _INS_MainTex, sampler_INS_MainTex, uv382 );
	float2 appendResult88 = (float2((float)( CUSTOM_COLOR * 0 ) , -0.5));
	float2 uv389 = float4(IN.uv23.zw,0,0).xy * float2( 1,1 ) + appendResult88;
	float4 DirtUV395 = SAMPLE_TEXTURE2D( _INS_MainTex, sampler_INS_MainTex, uv389 );
	float3 temp_cast_1 = (( tex2DNode7.g * _INS_DirtMultiplier )).xxx;
	float3 temp_cast_2 = (( 1.0 - IN.color.r )).xxx;
	float DirtHeight14 = saturate( ( 1.0 - ( ( distance( temp_cast_1 , temp_cast_2 ) - _INS_DirtRange ) / max( _INS_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult23 = lerp( ( MaskBlue70 * ( 1.0 - ( _INS_InsulationDarkness * ( 1.0 - IN.color.g ) ) ) * ColorUV396 ) , ( DirtUV395 * tex2DNode7.r ) , ( DirtHeight14 * _INS_DirtOpacity ));
	float4 Albedo64 = lerpResult23;
	half4 albedo = Albedo64;
	albedo.a = 1;

	float2 uv_Normals = float4(IN.uv.xy,0,0).xy * _INS_Normals_ST.xy + _INS_Normals_ST.zw;
	float3 Normals59 = SampleNormal(uv_Normals, TEXTURE2D_ARGS( _INS_Normals, sampler_INS_Normals), _INS_NormalsScale );
	normalTS = Normals59;

	float VertexGreen67 = IN.color.g;
	clip( tex2DNode7.b - ( VertexGreen67 * _INS_Mask ));

	return albedo;

#else
	return 0;
#endif

}

half4 SampleToonAlbedoAlpha_Plywood(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_PLYWOOD)
	half2 _ColorsNumber = half2(0,-0.1);
	float2 temp_output_274_0 = ( half2( 0.015625,0 ) * CUSTOM_COLOR );
	float2 uv0278 = IN.uv.xy * _ColorsNumber + temp_output_274_0;
	float4 Color1UV3259 = SAMPLE_TEXTURE2D( _PLYW_ColorTheme, sampler_PLYW_ColorTheme, uv0278 );
	float2 uv4_MainTex = IN.uv23.zw * _PLYW_MainTex_ST.xy + _PLYW_MainTex_ST.zw;
	float4 tex2DNode229 = SAMPLE_TEXTURE2D( _PLYW_MainTex, sampler_PLYW_MainTex, uv4_MainTex );
	float2 appendResult275 = (float2(temp_output_274_0.x , 0.5));
	float2 uv0277 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult275;
	float4 Color2UV3258 = SAMPLE_TEXTURE2D( _PLYW_ColorTheme, sampler_PLYW_ColorTheme, uv0277 );
	float2 uv4_RGBAMaskB = float4(IN.uv23.zw,0,0).xy * _PLYW_RGBAMaskB_ST.xy + _PLYW_RGBAMaskB_ST.zw;
	float4 tex2DNode1 = SAMPLE_TEXTURE2D( _PLYW_RGBAMaskB, sampler_PLYW_RGBAMaskB, uv4_RGBAMaskB );
	float HeightMask10 = saturate(pow(((( 1.0 - tex2DNode1.a )*( ( ( tex2DNode1.r * _PLYW_DamageMultiplier ) * IN.color.g ) * _PLYW_DamageAmount ))*4)+(( ( ( tex2DNode1.r * _PLYW_DamageMultiplier ) * IN.color.g ) * _PLYW_DamageAmount )*2),_PLYW_DamageSmooth));
	float2 uv_MainTex = float4(IN.uv.xy,0,0).xy * _PLYW_MainTex_ST.xy + _PLYW_MainTex_ST.zw;
	float4 tex2DNode236 = SAMPLE_TEXTURE2D( _PLYW_MainTex, sampler_PLYW_MainTex, uv_MainTex );
	float3 temp_cast_1 = (pow( ( 1.0 - tex2DNode236.a ) , 0.46 )).xxx;
	float3 desaturateInitialColor181 = temp_cast_1;
	float desaturateDot181 = dot( desaturateInitialColor181, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar181 = lerp( desaturateInitialColor181, desaturateDot181.xxx, 1.0 );
	float3 CutoutEdges182 = desaturateVar181;
	float3 Heightmap11 = ( HeightMask10 + CutoutEdges182 );
	float3 clampResult221 = clamp( ( Heightmap11 + pow( ( tex2DNode229.g + ( tex2DNode229.g * _PLYW_EdgesOverlayMultiply ) ) , _PLYW_EdgesOverlayPower ) ) , float3( 0,0,0 ) , float3( 1,1,1 ) );
	float4 lerpResult14 = lerp( ( Color1UV3259 * pow( tex2DNode229.r , _PLYW_DirtOverlay ) ) , ( Color2UV3258 * pow( tex2DNode229.b , _PLYW_SplintersOverlay ) ) , float4( clampResult221 , 0.0 ));
	float2 appendResult284 = (float2(temp_output_274_0.x , 0.1));
	float2 uv0285 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult284;
	float4 DirtColorUV3260 = SAMPLE_TEXTURE2D( _PLYW_ColorTheme, sampler_PLYW_ColorTheme, uv0285 );
	float HeightMaskBlue232 = tex2DNode1.b;
	float3 temp_cast_5 = (( tex2DNode1.g * _PLYW_DirtMultiplier )).xxx;
	float3 temp_cast_6 = (( 1.0 - ( IN.color.r * tex2DNode1.r ) )).xxx;
	float DirtHeight74 = saturate( ( 1.0 - ( ( distance( temp_cast_5 , temp_cast_6 ) - _PLYW_DirtRange ) / max( _PLYW_DirtSmooth , 1E-05 ) ) ) );
	float clampResult270 = clamp( ( DirtHeight74 * _PLYW_DirtOpacity ) , 0.0 , 1.0 );
	float4 lerpResult30 = lerp( ( lerpResult14 + ( IN.color.a * _PLYW_BrightAreas * lerpResult14 ) + ( float4( CutoutEdges182 , 0.0 ) * _PLYW_EdgesBrightness * lerpResult14 ) ) , ( DirtColorUV3260 * HeightMaskBlue232 ) , clampResult270);
	float VertexAlpha265 = IN.color.a;
	float4 Albedo103 = ( lerpResult30 + ( VertexAlpha265 * _PLYW_InsideBrightness ) );
	half4 albedo = Albedo103;

	float2 uv4_PlywoodNM = float4(IN.uv23.zw,0,0).xy * _PLYW_PlywoodNM_ST.xy + _PLYW_PlywoodNM_ST.zw;
	float2 uv4_PlywoodDamageNM = float4(IN.uv23.zw,0,0).xy * _PLYW_PlywoodDamageNM_ST.xy + _PLYW_PlywoodDamageNM_ST.zw;
	float3 lerpResult21 = lerp( SampleNormal(uv4_PlywoodNM, TEXTURE2D_ARGS( _PLYW_PlywoodNM, sampler_PLYW_PlywoodNM), _PLYW_PlywoodNMScale ) , SampleNormal(uv4_PlywoodDamageNM, TEXTURE2D_ARGS( _PLYW_PlywoodDamageNM, sampler_PLYW_PlywoodDamageNM), _PLYW_PlywoodDamgeNMScale ) , Heightmap11);
	float3 Normals92 = lerpResult21;
	normalTS = Normals92;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float RedChannelRGBA109 = tex2DNode1.r;
		float GreenChannelRGBA123 = tex2DNode1.g;
		float Splinters119 = tex2DNode229.b;
		float3 Smoothness117 = ( ( _PLYW_SmoothnessWood * RedChannelRGBA109 * ( 1.0 - GreenChannelRGBA123 ) ) * ( 1.0 - ( _PLYW_SmoothnessDamage * ( 1.0 - Splinters119 ) * Heightmap11 ) ) * ( 1.0 - ( _PLYW_SmoothnessDirt * DirtHeight74 ) ) );
		smoothness = Smoothness117;
	#endif

	float OpacityMask225 = tex2DNode236.a;
	albedo.a = OpacityMask225;

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Tile(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_TILE)
	float2 appendResult363 = (float2(( WALLPAPER_NUMBER * 0.25 ) , ( 0.25 * WALLPAPER_ROW )));
	float2 uv3364 = IN.uv23.zw * float2( 1,1 ) + appendResult363;
	float4 tex2DNode320 = SAMPLE_TEXTURE2D( _TILE_Pattern, sampler_TILE_Pattern, uv3364 );
	float staticSwitch429 = (float)CUSTOM_COLOR;

	float2 temp_output_420_0 = ( half2( 0.015625,0 ) * staticSwitch429 );
	float2 uv0412 = IN.uv.xy * half2( 0,-0.5 ) + temp_output_420_0;
	float4 Color1_UV3398 = SAMPLE_TEXTURE2D( _TILE_MainTex, sampler_TILE_MainTex, uv0412 );
	float4 lerpResult314 = lerp( tex2DNode320 , Color1_UV3398 , ( 1.0 - tex2DNode320.a ));
	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _TILE_RGBAMaskB_ST.xy + _TILE_RGBAMaskB_ST.zw;
	float4 tex2DNode294 = SAMPLE_TEXTURE2D( _TILE_RGBAMaskB, sampler_TILE_RGBAMaskB, uv_RGBAMaskB );
	float clampResult225 = clamp( pow( tex2DNode294.r , _TILE_DirtOverlay ) , 0.0 , 1.0 );
	float4 temp_output_33_0 = ( lerpResult314 * clampResult225 );
	float clampResult226 = clamp( pow( ( tex2DNode294.g + ( tex2DNode294.g * _TILE_EdgesOverlayMultiply ) ) , _TILE_EdgesOverlayPower ) , 0.0 , 1.0 );
	float2 uv_RGBAMaskC = float4(IN.uv.xy,0,0).xy * _TILE_RGBAMaskC_ST.xy + _TILE_RGBAMaskC_ST.zw;
	float4 tex2DNode238 = SAMPLE_TEXTURE2D( _TILE_RGBAMaskC, sampler_TILE_RGBAMaskC, uv_RGBAMaskC );
	float4 temp_cast_2 = (tex2DNode238.g).xxxx;
	float2 uv_RGBAMaskA = float4(IN.uv.xy,0,0).xy * _TILE_RGBAMaskA_ST.xy + _TILE_RGBAMaskA_ST.zw;
	float4 tex2DNode290 = SAMPLE_TEXTURE2D( _TILE_RGBAMaskA, sampler_TILE_RGBAMaskA, uv_RGBAMaskA );
	float TilesMaskBlue291 = tex2DNode290.b;
	float4 lerpResult230 = lerp( lerpResult314 , temp_cast_2 , ( 1.0 - TilesMaskBlue291 ));
	float4 temp_output_68_0 = ( lerpResult230 * pow( tex2DNode294.a , _TILE_DirtDamageOverlay ) );
	float3 temp_cast_3 = (( 1.0 - ( IN.color.g * ( 1.0 - tex2DNode290.g ) ) )).xxx;
	float Heightmap11 = saturate( ( 1.0 - ( ( distance( temp_cast_3 , float3( 0,0,0 ) ) - _TILE_DamageAmount ) / max( 0.0 , 1E-05 ) ) ) );
	float4 lerpResult14 = lerp( ( temp_output_33_0 + ( clampResult226 * temp_output_33_0 ) ) , ( temp_output_68_0 + ( pow( ( tex2DNode294.b + ( tex2DNode294.b * _TILE_EdgesDamageMultiply ) ) , _TILE_EdgesDamagePower ) * temp_output_68_0 ) ) , Heightmap11);
	float2 uv0426 = float4(IN.uv.xy,0,0).xy * half2( 0,0 ) + temp_output_420_0;
	float4 Color2_UV3397 = SAMPLE_TEXTURE2D( _TILE_MainTex, sampler_TILE_MainTex, uv0426 );
	float GreenChannelRGBA123 = tex2DNode290.a;
	float3 temp_cast_4 = (( GreenChannelRGBA123 * _TILE_DirtMultiplier )).xxx;
	float3 temp_cast_5 = (( 1.0 - ( IN.color.r * pow( GreenChannelRGBA123 , _TILE_DirtPower ) ) )).xxx;
	float DirtHeight74 = saturate( ( 1.0 - ( ( distance( temp_cast_4 , temp_cast_5 ) - _TILE_DirtRange ) / max( _TILE_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult30 = lerp( lerpResult14 , ( tex2DNode238.b * Color2_UV3397 ) , ( DirtHeight74 * _TILE_DirtOpacity ));
	float4 Albedo103 = lerpResult30;
	half4 albedo = Albedo103;

	float2 uv_NormalGood = float4(IN.uv.xy,0,0).xy * _TILE_NormalGood_ST.xy + _TILE_NormalGood_ST.zw;
	float2 uv_NormalDamage = float4(IN.uv.xy,0,0).xy * _TILE_NormalDamage_ST.xy + _TILE_NormalDamage_ST.zw;
	float3 lerpResult21 = lerp( SampleNormal(uv_NormalGood, TEXTURE2D_ARGS( _TILE_NormalGood, sampler_TILE_NormalGood), _TILE_NormalScale ) , SampleNormal(uv_NormalDamage, TEXTURE2D_ARGS( _TILE_NormalDamage, sampler_TILE_NormalDamage), _TILE_NormalDamageScale ) , Heightmap11);
	float3 Normals92 = lerpResult21;
	normalTS = Normals92;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float temp_output_102_0 = ( _TILE_SmoothnessMain * ( _TILE_SmoothnessMain * ( 1.0 - ( GreenChannelRGBA123 * _TILE_SmoothnessOverlayDirt ) ) ) );
		float DirtDamage119 = tex2DNode294.a;
		float lerpResult268 = lerp( temp_output_102_0 , ( temp_output_102_0 * DirtDamage119 * TilesMaskBlue291 ) , Heightmap11);
		float DirtOpacity308 = _TILE_DirtOpacity;
		float Smoothness117 = ( lerpResult268 * ( 1.0 - ( ( _TILE_SmoothnessDirt * DirtHeight74 ) * DirtOpacity308 ) ) );
		smoothness = Smoothness117;
	#endif

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Trim(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_TRIM)
	float2 uv_TrimColor = IN.uv.xy * _TRIM_TrimColor_ST.xy + _TRIM_TrimColor_ST.zw;
	float4 tex2DNode150 = SAMPLE_TEXTURE2D( _TRIM_TrimColor, sampler_TRIM_TrimColor, uv_TrimColor );
	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch350 = (float)CUSTOM_COLOR;
	float2 temp_output_336_0 = ( half2( 0.015625,0 ) * staticSwitch350 );
	float2 appendResult337 = (float2(temp_output_336_0.x , 0.5));
	float2 uv0339 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult337;
	float4 Color2UV3325 = SAMPLE_TEXTURE2D( _TRIM_MainTex, sampler_TRIM_MainTex, uv0339 );
	float2 uv_RGBAMaskA = float4(IN.uv.xy,0,0).xy * _TRIM_RGBAMaskA_ST.xy + _TRIM_RGBAMaskA_ST.zw;
	float4 tex2DNode219 = SAMPLE_TEXTURE2D( _TRIM_RGBAMaskA, sampler_TRIM_RGBAMaskA, uv_RGBAMaskA );
	float MaskRedChannel290 = tex2DNode219.b;
	float3 temp_cast_3 = (( 1.0 - MaskRedChannel290 )).xxx;
	float VertexBlue308 = IN.color.b;
	float3 temp_cast_4 = (( 1.0 - VertexBlue308 )).xxx;
	float2 uv4_RGBAMaskB = IN.uv23.zw * _TRIM_RGBAMaskB_ST.xy + _TRIM_RGBAMaskB_ST.zw;
	float4 tex2DNode271 = SAMPLE_TEXTURE2D( _TRIM_RGBAMaskB, sampler_TRIM_RGBAMaskB, uv4_RGBAMaskB );
	float3 temp_cast_5 = (pow( ( 1.0 - tex2DNode271.g ) , _TRIM_TransitionEdgeAmount )).xxx;
	float3 desaturateInitialColor295 = temp_cast_5;
	float desaturateDot295 = dot( desaturateInitialColor295, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar295 = lerp( desaturateInitialColor295, desaturateDot295.xxx, 1.0 );
	float3 CutoutEdges296 = desaturateVar295;
	float4 lerpResult288 = lerp( tex2DNode150 , Color2UV3325 , float4( ( ( 1.0 - saturate( ( 1.0 - ( ( distance( temp_cast_3 , temp_cast_4 ) - _TRIM_TransitionAmount ) / max( 1.0 , 1E-05 ) ) ) ) ) + CutoutEdges296 ) , 0.0 ));
	float2 uv0340 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + temp_output_336_0;
	float4 Color1UV3327 = SAMPLE_TEXTURE2D( _TRIM_MainTex, sampler_TRIM_MainTex, uv0340 );
	float4 lerpResult147 = lerp( Color1UV3327 , tex2DNode150 , pow( tex2DNode150.a , _TRIM_MaskOverlay ));
	float HeightMask8 = saturate(pow(((tex2DNode219.r*( ( pow( tex2DNode219.g , _TRIM_DamagePower ) * ( ( 1.0 - IN.color.g ) + ( 1.0 - _TRIM_DamageMultiply ) ) ) * ( 1.0 - _TRIM_DamageAmount ) ))*4)+(( ( pow( tex2DNode219.g , _TRIM_DamagePower ) * ( ( 1.0 - IN.color.g ) + ( 1.0 - _TRIM_DamageMultiply ) ) ) * ( 1.0 - _TRIM_DamageAmount ) )*2),_TRIM_DamageSmooth));
	float DamageSelection212 = ( 1.0 - HeightMask8 );
	float4 lerpResult149 = lerp( lerpResult288 , lerpResult147 , ( 1.0 - DamageSelection212 ));
	float2 appendResult346 = (float2(temp_output_336_0.x , 0.1));
	float2 uv0347 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult346;
	float4 DirtColorUV3329 = SAMPLE_TEXTURE2D( _TRIM_MainTex, sampler_TRIM_MainTex, uv0347 );
	float MaskAlphaChannel227 = tex2DNode219.a;
	float MaskBlueChannel221 = tex2DNode219.b;
	float3 temp_cast_8 = (MaskBlueChannel221).xxx;
	float VertexRed234 = IN.color.r;
	float3 temp_cast_9 = (( 1.0 - VertexRed234 )).xxx;
	float temp_output_190_0 = ( ( MaskAlphaChannel227 * saturate( ( 1.0 - ( ( distance( temp_cast_8 , temp_cast_9 ) - _TRIM_DirtRange ) / max( _TRIM_DirtSmooth , 1E-05 ) ) ) ) ) * _TRIM_DirtContrast );
	float4 lerpResult194 = lerp( lerpResult149 , ( DirtColorUV3329 * MaskAlphaChannel227 * _TRIM_DirtBrightness ) , temp_output_190_0);
	float4 Albedo210 = lerpResult194;
	half4 albedo = Albedo210;

	float2 uv_TrimNM = float4(IN.uv.xy,0,0).xy * _TRIM_TrimNM_ST.xy + _TRIM_TrimNM_ST.zw;
	float2 uv_TrimDamageNM = float4(IN.uv.xy,0,0).xy * _TRIM_TrimDamageNM_ST.xy + _TRIM_TrimDamageNM_ST.zw;
	float3 lerpResult17 = lerp( SampleNormal(uv_TrimNM, TEXTURE2D_ARGS( _TRIM_TrimNM, sampler_TRIM_TrimNM), _TRIM_TrimNMScale ) , SampleNormal(uv_TrimDamageNM, TEXTURE2D_ARGS( _TRIM_TrimDamageNM, sampler_TRIM_TrimDamageNM), _TRIM_TrimDamageNMScale ) , DamageSelection212);
	float3 Normals208 = lerpResult17;
	normalTS = Normals208;

	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _TRIM_RGBAMaskB_ST.xy + _TRIM_RGBAMaskB_ST.zw;
	float4 tex2DNode306 = SAMPLE_TEXTURE2D( _TRIM_RGBAMaskB, sampler_TRIM_RGBAMaskB, uv_RGBAMaskB );
	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float DirtSelection248 = temp_output_190_0;
		float Smoothness260 = ( pow( tex2DNode306.r , _TRIM_MainSmoothness ) * ( 1.0 - ( MaskBlueChannel221 * _TRIM_SmoothnessOverlay ) ) * ( 1.0 - ( _TRIM_SmoothnessDirt * DirtSelection248 ) ) * ( 1.0 - ( _TRIM_SmoothnessDamage * DamageSelection212 ) ) );
		smoothness = Smoothness260;
	#endif

	float OpacityMask303 = ( tex2DNode271.g * tex2DNode306.b );
	albedo.a = OpacityMask303;

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_Wallpaper(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_WALLPAPER)
	half2 _ColorsNumber = half2(0,-0.07);
	float staticSwitch325 = (float)CUSTOM_COLOR;
	float2 temp_output_298_0 = ( half2( 0.015625,0 ) * staticSwitch325 );
	float2 uv0301 = IN.uv.xy * _ColorsNumber + temp_output_298_0;
	float4 ColorSelection1306 = SAMPLE_TEXTURE2D( _WP_ColorTheme, sampler_WP_ColorTheme, uv0301 );
	float2 appendResult283 = (float2(( WALLPAPER_NUMBER * 0.25 ) , ( 0.25 * WALLPAPER_ROW )));
	float2 uv3282 = IN.uv23.zw * float2( 1,1 ) + appendResult283;
	float4 tex2DNode167 = SAMPLE_TEXTURE2D( _WP_PatternTexture, sampler_WP_PatternTexture, uv3282 );
	float4 lerpResult170 = lerp( ColorSelection1306 , tex2DNode167 , ( tex2DNode167.a * _WP_PatternStrenght ));
	float2 uv3_RGBA_Mask_B = IN.uv23.xy * _WP_RGBA_Mask_B_ST.xy + _WP_RGBA_Mask_B_ST.zw;
	float4 tex2DNode236 = SAMPLE_TEXTURE2D( _WP_RGBA_Mask_B, sampler_WP_RGBA_Mask_B, uv3_RGBA_Mask_B );
	float4 temp_output_33_0 = ( lerpResult170 * pow( tex2DNode236.r , _WP_DirtOverlay ) );
	float2 appendResult320 = (float2(temp_output_298_0.x , 0.5));
	float2 uv0304 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult320;
	float4 ColorSelection2307 = SAMPLE_TEXTURE2D( _WP_ColorTheme, sampler_WP_ColorTheme, uv0304 );
	float EdgesNoNormals220 = ( IN.color.b * _WP_EdgesNormals );
	float4 temp_output_68_0 = ( ColorSelection2307 * pow( ( tex2DNode236.b + EdgesNoNormals220 ) , _WP_DirtDamageOverlay ) );
	float2 uv3_RGBA_Mask_A = float4(IN.uv23.xy,0,0).xy * _WP_RGBA_Mask_A_ST.xy + _WP_RGBA_Mask_A_ST.zw;
	float4 tex2DNode232 = SAMPLE_TEXTURE2D( _WP_RGBA_Mask_A, sampler_WP_RGBA_Mask_A, uv3_RGBA_Mask_A );
	float HeightMask10 = saturate(pow(((tex2DNode232.g*( ( ( tex2DNode232.r * _WP_DamageMultiplier ) * IN.color.g ) * _WP_DamageAmount ))*4)+(( ( ( tex2DNode232.r * _WP_DamageMultiplier ) * IN.color.g ) * _WP_DamageAmount )*2),_WP_DamageSmooth));
	float Heightmap11 = HeightMask10;
	float4 lerpResult14 = lerp( ( temp_output_33_0 + ( pow( ( tex2DNode236.g + ( tex2DNode236.g * _WP_EdgesOverlayMultiply ) ) , _WP_EdgesOverlayPower ) * temp_output_33_0 ) ) , ( temp_output_68_0 + ( tex2DNode236.a * temp_output_68_0 * _WP_PaperBrightness ) ) , Heightmap11);
	float HeightBricks172 = Heightmap11;
	float BricksStep179 = step( 0.1 , ( ( 1.0 - HeightBricks172 ) * step( _WP_TransitionAmount , HeightBricks172 ) ) );
	float2 uv_MainTex = float4(IN.uv.xy,0,0).xy * _WP_MainTex_ST.xy + _WP_MainTex_ST.zw;
	float4 tex2DNode205 = SAMPLE_TEXTURE2D( _WP_MainTex, sampler_WP_MainTex, uv_MainTex );
	float4 temp_cast_3 = (_WP_EdgesAmount).xxxx;
	float3 desaturateInitialColor215 = pow( ( 1.0 - tex2DNode205 ) , temp_cast_3 ).rgb;
	float desaturateDot215 = dot( desaturateInitialColor215, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar215 = lerp( desaturateInitialColor215, desaturateDot215.xxx, 1.0 );
	float3 CutoutEdges216 = desaturateVar215;
	float2 appendResult323 = (float2(temp_output_298_0.x , 0.1));
	float2 uv0321 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult323;
	float4 ColorSelection3312 = SAMPLE_TEXTURE2D( _WP_ColorTheme, sampler_WP_ColorTheme, uv0321 );
	float Mask1_Alpha242 = tex2DNode232.a;
	float3 temp_cast_7 = (( tex2DNode232.b * _WP_DirtMultiplier )).xxx;
	float3 temp_cast_8 = (( 1.0 - IN.color.r )).xxx;
	float DirtHeight74 = saturate( ( 1.0 - ( ( distance( temp_cast_7 , temp_cast_8 ) - _WP_DirtRange ) / max( _WP_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult30 = lerp( ( lerpResult14 + float4( ( ( BricksStep179 + CutoutEdges216 ) * _WP_EdgesBrigthness ) , 0.0 ) ) , ( ColorSelection3312 * Mask1_Alpha242 ) , ( DirtHeight74 * _WP_DirtOpacity ));
	float4 Albedo103 = lerpResult30;
	half4 albedo = Albedo103;

	float2 uv3_WallpaperNM = float4(IN.uv23.xy,0,0).xy * _WP_WallpaperNM_ST.xy + _WP_WallpaperNM_ST.zw;
	float2 uv3_WallpaperDamageNM = float4(IN.uv23.xy,0,0).xy * _WP_WallpaperDamageNM_ST.xy + _WP_WallpaperDamageNM_ST.zw;
	float3 lerpResult21 = lerp( SampleNormal(uv3_WallpaperNM, TEXTURE2D_ARGS( _WP_WallpaperNM, sampler_WP_WallpaperNM), _WP_WallpaperNMScale ) , SampleNormal(uv3_WallpaperDamageNM, TEXTURE2D_ARGS( _WP_WallpaperDamageNM, sampler_WP_WallpaperDamageNM), _WP_WallpaperDamageNMScale ) , ( ( Heightmap11 * ( 1.0 - EdgesNoNormals220 ) ) + ( IN.color.r * _WP_DamageNormalsExtra ) + CutoutEdges216 ));
	float3 Normals92 = lerpResult21;
	normalTS = Normals92;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float RedChannelRGBA109 = tex2DNode232.r;
		float GreenChannelRGBA123 = tex2DNode232.b;
		float Smoothness117 = ( ( _WP_SmoothnessMain * RedChannelRGBA109 * ( 1.0 - GreenChannelRGBA123 ) ) * ( 1.0 - ( _WP_SmoothnessDamage * Heightmap11 ) ) * ( 1.0 - ( _WP_SmoothnessDirt * DirtHeight74 ) ) );
		smoothness = Smoothness117;
	#endif

	float4 OpacityMask244 = tex2DNode205;
	albedo.a = OpacityMask244.r;
	
	return albedo;
#else
	return 1;
#endif
}

half4 SampleToonAlbedoAlpha_WoodFloor(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_WOOD_FLOOR)

	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch338 = (float)CUSTOM_COLOR;

	float2 temp_output_324_0 = ( half2( 0.015625,0 ) * staticSwitch338 );
	float2 uv0328 = IN.uv.xy * _ColorsNumber + temp_output_324_0;
	float4 Color1UV3309 = SAMPLE_TEXTURE2D( _WF_MainTex, sampler_WF_MainTex, uv0328 );
	float2 uv_RGBAMaskB = float4(IN.uv.xy,0,0).xy * _WF_RGBAMaskB_ST.xy + _WF_RGBAMaskB_ST.zw;
	float4 tex2DNode268 = SAMPLE_TEXTURE2D( _WF_RGBAMaskB, sampler_WF_RGBAMaskB, uv_RGBAMaskB );
	float4 temp_output_33_0 = ( Color1UV3309 * ( 1.0 - ( ( 1.0 - tex2DNode268.r ) * _WF_DirtOverlay ) ) );
	float2 appendResult325 = (float2(temp_output_324_0.x , 0.5));
	float2 uv0327 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult325;
	float4 Color2UV3307 = SAMPLE_TEXTURE2D( _WF_MainTex, sampler_WF_MainTex, uv0327 );
	float2 uv_RGBAMaskA = float4(IN.uv.xy,0,0).xy * _WF_RGBAMaskA_ST.xy + _WF_RGBAMaskA_ST.zw;
	float4 tex2DNode178 = SAMPLE_TEXTURE2D( _WF_RGBAMaskA, sampler_WF_RGBAMaskA, uv_RGBAMaskA );
	float HeightMask10 = saturate(pow(((tex2DNode178.b*( ( ( ( 1.0 - tex2DNode178.g ) * _WF_DamageMultiplier ) * IN.color.g ) * _WF_DamageAmount ))*4)+(( ( ( ( 1.0 - tex2DNode178.g ) * _WF_DamageMultiplier ) * IN.color.g ) * _WF_DamageAmount )*2),_WF_DamageSmooth));
	float Heightmap11 = HeightMask10;
	float HeightBricks256 = Heightmap11;
	float clampResult264 = clamp( step( 0.1 , ( ( 1.0 - HeightBricks256 ) * step( _WF_TransitionAmount , HeightBricks256 ) ) ) , 0.0 , 1.0 );
	float BricksStep262 = clampResult264;
	float clampResult314 = clamp( ( Heightmap11 + BricksStep262 ) , 0.0 , 1.0 );
	float4 lerpResult14 = lerp( ( temp_output_33_0 + ( ( tex2DNode268.g * _WF_EdgesOverlay ) * temp_output_33_0 ) ) , ( ( Color2UV3307 * pow( tex2DNode268.b , _WF_DirtDamageOverlay ) ) + ( ( ( tex2DNode268.a + ( BricksStep262 * _WF_EdgeBrighntess ) ) * _WF_EdgesDamageOverlay ) * Color2UV3307 ) ) , clampResult314);
	float2 appendResult331 = (float2(temp_output_324_0.x , 0.1));
	float2 uv0333 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult331;
	float4 DirtColorUV3311 = SAMPLE_TEXTURE2D( _WF_MainTex, sampler_WF_MainTex, uv0333 );
	float2 uv_RGBAMaskC = float4(IN.uv.xy,0,0).xy * _WF_RGBAMaskC_ST.xy + _WF_RGBAMaskC_ST.zw;
	float3 temp_cast_4 = (( tex2DNode178.r * _WF_DirtMultiplier )).xxx;
	float3 temp_cast_5 = (( 1.0 - ( IN.color.r * tex2DNode178.g ) )).xxx;
	float DirtHeight74 = saturate( ( 1.0 - ( ( distance( temp_cast_4 , temp_cast_5 ) - _WF_DirtRange ) / max( _WF_DirtSmooth , 1E-05 ) ) ) );
	float4 lerpResult30 = lerp( lerpResult14 , ( DirtColorUV3311 * SAMPLE_TEXTURE2D( _WF_RGBAMaskC, sampler_WF_RGBAMaskC, uv_RGBAMaskC ).r ) , ( DirtHeight74 * _WF_DirtOpacity ));
	float4 Albedo103 = lerpResult30;
	half4 albedo = Albedo103;

	float2 uv_NormalGood = float4(IN.uv.xy,0,0).xy * _WF_NormalGood_ST.xy + _WF_NormalGood_ST.zw;
	float2 uv_NormalDamage = float4(IN.uv.xy,0,0).xy * _WF_NormalDamage_ST.xy + _WF_NormalDamage_ST.zw;
	float3 lerpResult21 = lerp( SampleNormal(uv_NormalGood, TEXTURE2D_ARGS( _WF_NormalGood, sampler_WF_NormalGood), _WF_NormalGoodScale ) , SampleNormal(uv_NormalDamage, TEXTURE2D_ARGS( _WF_NormalDamage, sampler_WF_NormalDamage), _WF_NormalDamageScale ) , Heightmap11);
	float3 Normals92 = lerpResult21;
	normalTS = Normals92;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float GreenChannelRGBA123 = tex2DNode178.r;
		float RedChannelRGBA109 = tex2DNode178.g;
		float Smoothness117 = ( ( _WF_SmoothnessMain * ( 1.0 - ( GreenChannelRGBA123 * _WF_SmoothnessMainDirt ) ) * RedChannelRGBA109 ) * ( 1.0 - ( _WF_SmoothnessDamage * Heightmap11 ) ) * ( 1.0 - ( _WF_SmoothnessDirt * DirtHeight74 ) ) );
		smoothness = Smoothness117;
	#endif

	float2 uv4_RGBAMaskC = IN.uv23.zw * _WF_RGBAMaskC_ST.xy + _WF_RGBAMaskC_ST.zw;
	float OpacityMask276 = SAMPLE_TEXTURE2D( _WF_RGBAMaskC, sampler_WF_RGBAMaskC, uv4_RGBAMaskC ).g;
	albedo.a = OpacityMask276;

	return albedo;
#else
	return 0;
#endif
}

half4 SampleToonAlbedoAlpha_ConcreteRubble(Varyings IN, out float3 normalTS, out float smoothness)
{
	normalTS = 0;
	smoothness = 0;

#if defined(_CONCRETE_RUBBLE)
	float2 uv_ColorMap = IN.uv.xy * _CCB_ColorMap_ST.xy + _CCB_ColorMap_ST.zw;
	float4 tex2DNode250 = SAMPLE_TEXTURE2D( _CCB_ColorMap, sampler_CCB_ColorMap, uv_ColorMap );
	float2 temp_cast_0 = (_CCB_DetailNMTiling).xx;
	float2 uv018 = float4(IN.uv.xy,0,0).xy * temp_cast_0 + float2( 0,0 );
	float2 ConcreteUVTiling73 = uv018;
	float4 tex2DNode85 = SAMPLE_TEXTURE2D( _CCB_MaskB, sampler_CCB_MaskB, ConcreteUVTiling73 );
	float Mask_B_Alpha89 = tex2DNode85.a;
	float2 uv_MainTex = float4(IN.uv.xy,0,0).xy * _CCB_MainTex_ST.xy + _CCB_MainTex_ST.zw;
	float4 tex2DNode75 = SAMPLE_TEXTURE2D( _CCB_MainTex, sampler_CCB_MainTex, uv_MainTex );
	float MaskABlue76 = tex2DNode75.b;
	float MaskARed84 = tex2DNode75.r;
	float3 desaturateInitialColor247 = ( ( ( tex2DNode250 * Mask_B_Alpha89 ) + ( ( 1.0 - MaskABlue76 ) * _CCB_ConcreteBrightness ) ) * ( 1.0 - ( MaskARed84 * _CCB_AODirt ) ) ).rgb;
	float desaturateDot247 = dot( desaturateInitialColor247, float3( 0.299, 0.587, 0.114 ));
	float3 desaturateVar247 = lerp( desaturateInitialColor247, desaturateDot247.xxx, ( 1.0 - MaskABlue76 ) );
	half2 _ColorsNumber = half2(0,-0.1);
	float staticSwitch94 = (float)CUSTOM_COLOR;
	float2 temp_output_96_0 = ( half2( 0.015625,0 ) * staticSwitch94 );
	float2 uv098 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + temp_output_96_0;
	float4 Color1UV3105 = SAMPLE_TEXTURE2D( _CCB_MainTex1, sampler_CCB_MainTex1, uv098 );
	float Mask_B_Blue88 = tex2DNode85.b;
	float Mask_B_Red86 = tex2DNode85.r;
	float3 temp_cast_5 = (( 1.0 - Mask_B_Red86 )).xxx;
	float Mask_B_Green87 = tex2DNode85.g;
	float VertexGreen112 = IN.color.g;
	float3 temp_cast_6 = (( Mask_B_Green87 * VertexGreen112 )).xxx;
	float PaintSelection123 = ( saturate( ( 1.0 - ( ( distance( temp_cast_5 , temp_cast_6 ) - _CCB_PaintRange ) / max( _CCB_PaintSmooth , 1E-05 ) ) ) ) * ( 1.0 - MaskABlue76 ) );
	float4 lerpResult37 = lerp( float4( desaturateVar247 , 0.0 ) , ( Color1UV3105 + ( Mask_B_Blue88 * Color1UV3105 ) ) , PaintSelection123);
	float2 appendResult103 = (float2(temp_output_96_0.x , 0.1));
	float2 uv0107 = float4(IN.uv.xy,0,0).xy * _ColorsNumber + appendResult103;
	float4 DirtColorUV3109 = SAMPLE_TEXTURE2D( _CCB_MainTex1, sampler_CCB_MainTex1, uv0107 );
	float VertexRed125 = IN.color.r;
	float3 temp_cast_8 = (( VertexRed125 * Mask_B_Alpha89 )).xxx;
	float3 temp_cast_9 = (( ( 1.0 - Mask_B_Red86 ) + ( ( 1.0 - MaskARed84 ) * _CCB_DirtInsideMultiplier ) )).xxx;
	float clampResult63 = clamp( ( saturate( ( 1.0 - ( ( distance( temp_cast_8 , temp_cast_9 ) - _CCB_DirtRange ) / max( _CCB_DirtSmooth , 1E-05 ) ) ) ) * _CCB_DirtContrast ) , 0.0 , 1.0 );
	float DirtSelection136 = clampResult63;
	float4 lerpResult194 = lerp( lerpResult37 , ( DirtColorUV3109 * Mask_B_Alpha89 ) , DirtSelection136);
	float ColorAlpha259 = tex2DNode250.a;
	float4 Albedo223 = ( lerpResult194 + ( _CCB_EdgesAdd * ColorAlpha259 ) );
	half4 albedo = Albedo223;

	float2 uv_MainNM = float4(IN.uv.xy,0,0).xy * _CCB_MainNM_ST.xy + _CCB_MainNM_ST.zw;
	float3 tex2DNode2 = SampleNormal(uv_MainNM, TEXTURE2D_ARGS( _CCB_MainNM, sampler_CCB_MainNM), 1.0f );
	float2 temp_cast_11 = (_CCB_DamageNMTiling).xx;
	float2 uv011 = float4(IN.uv.xy,0,0).xy * temp_cast_11 + float2( 0,0 );
	float3 lerpResult10 = lerp( BlendNormal( tex2DNode2 , SampleNormal(uv018, TEXTURE2D_ARGS( _CCB_DetailNM, sampler_CCB_DetailNM), _CCB_DetailScale ) ), BlendNormal( tex2DNode2 , SampleNormal(uv011, TEXTURE2D_ARGS( _CCB_DamageNM, sampler_CCB_DamageNM), _CCB_DamageScale ) ) , MaskABlue76);
	float3 Normals71 = lerpResult10;
	normalTS = Normals71;

	#if !defined(_SPECULARHIGHLIGHTS_OFF)
		float Smoothnes244 = ( ( Mask_B_Alpha89 * _CCB_MainSmoothness * ( 1.0 - MaskARed84 ) * ( 1.0 - ( DirtSelection136 * _CCB_DirtSmoothness ) ) ) + ( _CCB_PaintSmoothness * PaintSelection123 ) );
		smoothness = Smoothnes244;
	#endif

	float MaskAAlpha78 = tex2DNode75.a;
	albedo.a = MaskAAlpha78;

	return albedo;
#else
	return 0;
#endif
}

#endif