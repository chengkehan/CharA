#ifndef __PBR_TEX_PROPS__
#define __PBR_TEX_PROPS__

// ---------------------------------------------------------------------------
// Textures, Samplers & Global Properties
// ---------------------------------------------------------------------------
// (note, BaseMap, BumpMap and EmissionMap is being defined by the SurfaceInput.hlsl include)

TEXTURE2D(_MetallicSpecGlossMap); 	SAMPLER(sampler_MetallicSpecGlossMap);
TEXTURE2D(_OcclusionMap); 			SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_AlphaTestMaskTex); 			SAMPLER(sampler_AlphaTestMaskTex);
TEXTURE2D(_ToonDetailMap); 			SAMPLER(sampler_ToonDetailMap);
TEXTURE2D(_HatchTex); 			SAMPLER(sampler_HatchTex);

TEXTURE2D(_BK_MainTex); 			SAMPLER(sampler_BK_MainTex);
TEXTURE2D(_BK_TextureSample3); 			SAMPLER(sampler_BK_TextureSample3);
TEXTURE2D(_BK_RGBA_Mask_A); 			SAMPLER(sampler_BK_RGBA_Mask_A);
TEXTURE2D(_BK_BrickUniqueNM); 			SAMPLER(sampler_BK_BrickUniqueNM);
TEXTURE2D(_BK_RGBAMaskB); 			SAMPLER(sampler_BK_RGBAMaskB);
TEXTURE2D(_BK_RGBAMaskC); 			SAMPLER(sampler_BK_RGBAMaskC);
TEXTURE2D(_BK_DBK_Brick_NM); 			SAMPLER(sampler_BK_DBK_Brick_NM);

TEXTURE2D(_RB_ColorTheme); 			SAMPLER(sampler_RB_ColorTheme);
TEXTURE2D(_RB_BakedNM); 			SAMPLER(sampler_RB_BakedNM);
TEXTURE2D(_RB_MainTex); 			SAMPLER(sampler_RB_MainTex);
TEXTURE2D(_RB_DetailsNM); 			SAMPLER(sampler_RB_DetailsNM);
TEXTURE2D(_RB_MaskB); 			SAMPLER(sampler_RB_MaskB);

TEXTURE2D(_RBT_MainTex); 			SAMPLER(sampler_RBT_MainTex);
TEXTURE2D(_RBT_DetailsNM); 			SAMPLER(sampler_RBT_DetailsNM);
TEXTURE2D(_RBT_BakedNM); 			SAMPLER(sampler_RBT_BakedNM);
TEXTURE2D(_RBT_MainTex1); 			SAMPLER(sampler_RBT_MainTex1);
TEXTURE2D(_RBT_MaskB); 			SAMPLER(sampler_RBT_MaskB);

TEXTURE2D(_CP_MainTex); 			SAMPLER(sampler_CP_MainTex);
TEXTURE2D(_CP_RGBAMaskB); 			SAMPLER(sampler_CP_RGBAMaskB);
TEXTURE2D(_CP_RGBAMaskA); 			SAMPLER(sampler_CP_RGBAMaskA);

TEXTURE2D(_CB_MainTex); 			SAMPLER(sampler_CB_MainTex);
TEXTURE2D(_CB_PaintNM); 			SAMPLER(sampler_CB_PaintNM);
TEXTURE2D(_CB_TransitionNM); 			SAMPLER(sampler_CB_TransitionNM);
TEXTURE2D(_CB_PaintDamageNM); 			SAMPLER(sampler_CB_PaintDamageNM);
TEXTURE2D(_CB_RGBAMaskA); 			SAMPLER(sampler_CB_RGBAMaskA);
TEXTURE2D(_CB_RGBAMaskB); 			SAMPLER(sampler_CB_RGBAMaskB);
TEXTURE2D(_CB_RGBAMaskC); 			SAMPLER(sampler_CB_RGBAMaskC);

TEXTURE2D(_CBR_ColorTheme); 			SAMPLER(sampler_CBR_ColorTheme);
TEXTURE2D(_CBR_BakedNM); 			SAMPLER(sampler_CBR_BakedNM);
TEXTURE2D(_CBR_DetailsNM); 			SAMPLER(sampler_CBR_DetailsNM);
TEXTURE2D(_CBR_MainTex); 			SAMPLER(sampler_CBR_MainTex);
TEXTURE2D(_CBR_MaskB); 			SAMPLER(sampler_CBR_MaskB);

TEXTURE2D(_CBRT_MainTex); 			SAMPLER(sampler_CBRT_MainTex);
TEXTURE2D(_CBRT_DetailsNM); 			SAMPLER(sampler_CBRT_DetailsNM);
TEXTURE2D(_CBRT_MainTex1); 			SAMPLER(sampler_CBRT_MainTex1);
TEXTURE2D(_CBRT_MaskB); 			SAMPLER(sampler_CBRT_MaskB);
TEXTURE2D(_CBRT_BrickTileNM); 			SAMPLER(sampler_CBRT_BrickTileNM);

TEXTURE2D(_CC_MainTex); 			SAMPLER(sampler_CC_MainTex);
TEXTURE2D(_CC_DamageNM); 			SAMPLER(sampler_CC_DamageNM);
TEXTURE2D(_CC_DamageColor); 			SAMPLER(sampler_CC_DamageColor);
TEXTURE2D(_CC_ConcreteNM); 			SAMPLER(sampler_CC_ConcreteNM);
TEXTURE2D(_CC_TransitionNM); 			SAMPLER(sampler_CC_TransitionNM);
TEXTURE2D(_CC_RGBAMaskA); 			SAMPLER(sampler_CC_RGBAMaskA);
TEXTURE2D(_CC_RGBAMaskB); 			SAMPLER(sampler_CC_RGBAMaskB);

TEXTURE2D(_RBR_MainTex); 			SAMPLER(sampler_RBR_MainTex);
TEXTURE2D(_RBR_Smoothness); 			SAMPLER(sampler_RBR_Smoothness);
TEXTURE2D(_RBR_Normals); 			SAMPLER(sampler_RBR_Normals);

TEXTURE2D(_DR_MainTex); 			SAMPLER(sampler_DR_MainTex);
TEXTURE2D(_DR_DoorsColor); 			SAMPLER(sampler_DR_DoorsColor);
TEXTURE2D(_DR_DoorNM); 			SAMPLER(sampler_DR_DoorNM);
TEXTURE2D(_DR_DoorDamageNM); 			SAMPLER(sampler_DR_DoorDamageNM);
TEXTURE2D(_DR_RGBAMaskA); 			SAMPLER(sampler_DR_RGBAMaskA);
TEXTURE2D(_DR_RGBAMaskB); 			SAMPLER(sampler_DR_RGBAMaskB);

TEXTURE2D(_DW_ColorTheme); 			SAMPLER(sampler_DW_ColorTheme);
TEXTURE2D(_DW_DrywallNM); 			SAMPLER(sampler_DW_DrywallNM);
TEXTURE2D(_DW_DrywallDamageNM); 			SAMPLER(sampler_DW_DrywallDamageNM);
TEXTURE2D(_DW_RGBAMaskA); 			SAMPLER(sampler_DW_RGBAMaskA);
TEXTURE2D(_DW_MainTex); 			SAMPLER(sampler_DW_MainTex);

TEXTURE2D(_GL_ColorTheme); 			SAMPLER(sampler_GL_ColorTheme);
TEXTURE2D(_GL_Glass_Normals); 			SAMPLER(sampler_GL_Glass_Normals);
TEXTURE2D(_GL_MainTex); 			SAMPLER(sampler_GL_MainTex);

TEXTURE2D(_GLI_MainTex); 			SAMPLER(sampler_GLI_MainTex);
TEXTURE2D(_GLI_ColorTheme); 			SAMPLER(sampler_GLI_ColorTheme);
TEXTURE2D(_GLI_GlassNM); 			SAMPLER(sampler_GLI_GlassNM);
TEXTURE2D(_GLI_WallTex); 			SAMPLER(sampler_GLI_WallTex);

TEXTURE2D(_INS_Normals); 			SAMPLER(sampler_INS_Normals);
TEXTURE2D(_INS_InsulationMask); 			SAMPLER(sampler_INS_InsulationMask);
TEXTURE2D(_INS_MainTex); 			SAMPLER(sampler_INS_MainTex);

TEXTURE2D(_PLYW_ColorTheme); 			SAMPLER(sampler_PLYW_ColorTheme);
TEXTURE2D(_PLYW_PlywoodDamageNM); 			SAMPLER(sampler_PLYW_PlywoodDamageNM);
TEXTURE2D(_PLYW_PlywoodNM); 			SAMPLER(sampler_PLYW_PlywoodNM);
TEXTURE2D(_PLYW_MainTex); 			SAMPLER(sampler_PLYW_MainTex);
TEXTURE2D(_PLYW_RGBAMaskB); 			SAMPLER(sampler_PLYW_RGBAMaskB);

TEXTURE2D(_TILE_MainTex); 			SAMPLER(sampler_TILE_MainTex);
TEXTURE2D(_TILE_NormalDamage); 			SAMPLER(sampler_TILE_NormalDamage);
TEXTURE2D(_TILE_NormalGood); 			SAMPLER(sampler_TILE_NormalGood);
TEXTURE2D(_TILE_RGBAMaskA); 			SAMPLER(sampler_TILE_RGBAMaskA);
TEXTURE2D(_TILE_RGBAMaskB); 			SAMPLER(sampler_TILE_RGBAMaskB);
TEXTURE2D(_TILE_RGBAMaskC); 			SAMPLER(sampler_TILE_RGBAMaskC);
TEXTURE2D(_TILE_Pattern); 			SAMPLER(sampler_TILE_Pattern);

TEXTURE2D(_TRIM_MainTex); 			SAMPLER(sampler_TRIM_MainTex);
TEXTURE2D(_TRIM_TrimColor); 			SAMPLER(sampler_TRIM_TrimColor);
TEXTURE2D(_TRIM_TrimNM); 			SAMPLER(sampler_TRIM_TrimNM);
TEXTURE2D(_TRIM_TrimDamageNM); 			SAMPLER(sampler_TRIM_TrimDamageNM);
TEXTURE2D(_TRIM_RGBAMaskA); 			SAMPLER(sampler_TRIM_RGBAMaskA);
TEXTURE2D(_TRIM_RGBAMaskB); 			SAMPLER(sampler_TRIM_RGBAMaskB);

TEXTURE2D(_WP_ColorTheme); 			SAMPLER(sampler_WP_ColorTheme);
TEXTURE2D(_WP_WallpaperNM); 			SAMPLER(sampler_WP_WallpaperNM);
TEXTURE2D(_WP_WallpaperDamageNM); 			SAMPLER(sampler_WP_WallpaperDamageNM);
TEXTURE2D(_WP_RGBA_Mask_A); 			SAMPLER(sampler_WP_RGBA_Mask_A);
TEXTURE2D(_WP_RGBA_Mask_B); 			SAMPLER(sampler_WP_RGBA_Mask_B);
TEXTURE2D(_WP_MainTex); 			SAMPLER(sampler_WP_MainTex);
TEXTURE2D(_WP_PatternTexture); 			SAMPLER(sampler_WP_PatternTexture);

TEXTURE2D(_WF_MainTex); 			SAMPLER(sampler_WF_MainTex);
TEXTURE2D(_WF_NormalDamage); 			SAMPLER(sampler_WF_NormalDamage);
TEXTURE2D(_WF_NormalGood); 			SAMPLER(sampler_WF_NormalGood);
TEXTURE2D(_WF_RGBAMaskA); 			SAMPLER(sampler_WF_RGBAMaskA);
TEXTURE2D(_WF_RGBAMaskB); 			SAMPLER(sampler_WF_RGBAMaskB);
TEXTURE2D(_WF_RGBAMaskC); 			SAMPLER(sampler_WF_RGBAMaskC);

TEXTURE2D(_CCB_MainTex1); 			SAMPLER(sampler_CCB_MainTex1);
TEXTURE2D(_CCB_ColorMap); 			SAMPLER(sampler_CCB_ColorMap);
TEXTURE2D(_CCB_MainNM); 			SAMPLER(sampler_CCB_MainNM);
TEXTURE2D(_CCB_DamageNM); 			SAMPLER(sampler_CCB_DamageNM);
TEXTURE2D(_CCB_DetailNM); 			SAMPLER(sampler_CCB_DetailNM);
TEXTURE2D(_CCB_MaskB); 			SAMPLER(sampler_CCB_MaskB);
TEXTURE2D(_CCB_MainTex); 			SAMPLER(sampler_CCB_MainTex);

#endif