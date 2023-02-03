#ifndef __PBR_VALUE_PROPS__
#define __PBR_VALUE_PROPS__

#define cb_start CBUFFER_START(UnityPerMaterial)
#define cb_end CBUFFER_END

#define values_common_f4 float4 _BaseMap_ST; \
						float4 _BaseColor; \
						float4 _EmissionColor; \
						float4 _SpecColor; \
						float4 _ToonTint; \
						float4 _ToonInShadowTint; \
						float4 _ToonOutShadowTint; \
						float4 _ToonSSSTint; \
						float4 _ToonRimColor; \
						float4 _ToonInShadowOcclusionTintOffset;

#define values_common_f1 float _Metallic; \
						float _Smoothness; \
						float _OcclusionStrength; \
						float _Cutoff; \
						float _BumpScale; \
						float _ToonIntensity; \
						float _ToonSmoothness; \
						float _ToonOffset; \
						float _ToonOcclusionOffset; \
						float _ToonOcclusionSmoothness; \
						float _ToonRimPower; \
						float _ToonRimIntensity; \
						float _ToonDetailLighting; \
						float _ToonDetailLightingOffset; \
						float _ToonDetailShadowInput; \
						float _ToonDetailHighlightInput; \
						float _ToonDetailMidtoneInput; \
						float _ToonDetailShadowOutput; \
						float _ToonDetailHighlightOutput; \
						float _AdditionalLightIntensity; \
						float _AdditionalLightRangeScale; \
						float _StylizedColored; \
						float _StylizedColoredInShadow; \
						float _Outline; \
						float _Sketch; \
						float _Contrast; \
						float _HatchScale; \
						float _AdditionalLightInHatch; \
						float _AdditionalLightInHatch2; \
						float _HatchLighting; \
						float _HatchRotation;

#define values_brick_f1 float _BK_CementBrightness; \
						float _BK_BrickOverlay; \
						float _BK_WhiteBrick; \
						float _BK_WhiteBrickRange; \
						float _BK_WhiteBrickSmooth; \
						float _BK_DarkBrick; \
						float _BK_DarkBrickAmount; \
						float _BK_DamageAmount; \
						float _BK_DamageSmoothness; \
						float _BK_TransitionScale; \
						float _BK_DirtRange; \
						float _BK_DirtSmooth; \
						float _BK_BrickScale; \
						float _BK_PaintedBrickScale; \
						float _BK_BrickUniqueScale; \
						float _BK_BrickBareSmoothness; \
						float _BK_BrickPaintSmoothness; \
						float _BK_BrickDirtSmoothness; \
						float _BK_BrickDirtMainSmoothness; \
						float _BK_BrickDarkSmoothness;

#define values_brick_f4 float4 _BK_RGBAMaskC_ST; \
						float4 _BK_RGBAMaskB_ST; \
						float4 _BK_BrickUniqueNM_ST; \
						float4 _BK_DBK_Brick_NM_ST; \
						float4 _BK_TextureSample3_ST; \
						float4 _BK_RGBA_Mask_A_ST;

#define values_rubble_f1 float _RB_PaintRange; \
							float _RB_PaintSmooth; \
							float _RB_DetailsNMScale; \
							float _RB_DetailsTiling; \
							float _RB_DirtSmooth; \
							float _RB_DirtRange; \
							float _RB_DirtContrast; \
							float _RB_DirtBrightness; \
							float _RB_MainSmoothness; \
							float _RB_PaintSmoothness; \
							float _RB_BakedNMScale;

#define values_rubble_f4 float4 _RB_MainTex_ST; \
							float4 _RB_BakedNM_ST;
						

#define values_rubble_tile_f1 float _RBT_MainTiling; \
								float _RBT_PaintRange; \
								float _RBT_PaintSmooth; \
								float _RBT_DetailsNMScale; \
								float _RBT_DetailsTiling; \
								float _RBT_BakedNMScale; \
								float _RBT_DirtSmooth; \
								float _RBT_DirtRange; \
								float _RBT_DirtContrast; \
								float _RBT_DirtBrightness; \
								float _RBT_MainSmoothness; \
								float _RBT_PaintSmoothness; \
								float _RBT_CutAlphaSmooth; \
								float _RBT_CutAlphaRange;

#define values_carpet_f1 float _CP_Brightness; \
							float _CP_Darkness; \
							float _CP_Edges; \
							float _CP_Threshold; \
							float _CP_DirtOpacity; \
							float _CP_DirtSmooth; \
							float _CP_DirtMultiplier; \
							float _CP_DirtRange;

#define values_carpet_f4 float4 _CP_RGBAMaskA_ST; \
							float4 _CP_RGBAMaskB_ST;

#define values_cinder_block_f1 float _CB_EdgesOverlay; \
								float _CB_EdgesMultiply; \
								float _CB_DamageAmount; \
								float _CB_DamageSmoothness; \
								float _CB_DamageMultiplier; \
								float _CB_PaintNMScale; \
								float _CB_PaintSharpness; \
								float _CB_PaintBareSmooth; \
								float _CB_PaintBareRange; \
								float _CB_PaintEdgesMultiply; \
								float _CB_TransitionNMScale; \
								float _CB_PaintDamageNMScale; \
								float _CB_DirtOpacity; \
								float _CB_DirtRange; \
								float _CB_DirtSmooth; \
								float _CB_DirtMultiplier; \
								float _CB_TransitionBrighthness; \
								float _CB_SmoothnessMain; \
								float _CB_SmoothnessDamage; \
								float _CB_SmoothnessDirt; \
								float _CB_PaintSmoothness; \
								float _CB_ChippedPaintScale; \
								float _CB_ChippedPaintBrightness;

#define values_cinder_block_f4 float4 _CB_RGBAMaskA_ST; \
								float4 _CB_TransitionNM_ST; \
								float4 _CB_RGBAMaskC_ST; \
								float4 _CB_RGBAMaskB_ST; \
								float4 _CB_PaintDamageNM_ST; \
								float4 _CB_PaintNM_ST;

#define values_cinder_block_rubble_f1 float _CBR_PaintRange; \
										float _CBR_PaintSmooth; \
										float _CBR_DetailsNMScale; \
										float _CBR_DetailsTiling; \
										float _CBR_DirtSmooth; \
										float _CBR_DirtRange; \
										float _CBR_DirtContrast; \
										float _CBR_DirtBrightness; \
										float _CBR_MainSmoothness; \
										float _CBR_PaintSmoothness; \
										float _CBR_AODirt;

#define values_cinder_block_rubble_f4 float4 _CBR_MainTex_ST; \
										float4 _CBR_BakedNM_ST;

#define values_cinder_block_rubble_tile_f1 float _CBRT_PaintRange; \
											float _CBRT_PaintSmooth; \
											float _CBRT_DetailsNMScale; \
											float _CBRT_DetailsTiling; \
											float _CBRT_DirtSmooth; \
											float _CBRT_DirtRange; \
											float _CBRT_DirtContrast; \
											float _CBRT_DirtBrightness; \
											float _CBRT_MainSmoothness; \
											float _CBRT_PaintSmoothness; \
											float _CBRT_BrickNMScale; \
											float _CBRT_CutAlphaSmooth; \
											float _CBRT_CutAlphaRange;

#define values_cinder_block_rubble_tile_f4 float4 _CBRT_ColorsNumber; \
											float4 _CBRT_MainTex1_ST; \
											float4 _CBRT_BrickTileNM_ST;

#define values_concrete_f1 float _CC_DamageAmountSmooth; \
							float _CC_DamageAmount; \
							float _CC_DamageBrightness; \
							float _CC_DamageMultiplier; \
							float _CC_ConcreteDamageTiling; \
							float _CC_ConcreteDamageScale; \
							float _CC_PaintEdgesMultiply; \
							float _CC_ConcreteBareRange; \
							float _CC_ConcreteBareSmooth; \
							float _CC_EdgesOverlay; \
							float _CC_DirtOverlay; \
							float _CC_TransitionBrightness; \
							float _CC_TransitionScale; \
							float _CC_TransitionAmount; \
							float _CC_DirtContrast; \
							float _CC_DirtRange; \
							float _CC_DirtSmooth; \
							float _CC_MainSmoothness; \
							float _CC_DirtSmoothness; \
							float _CC_DamageSmoothness; \
							float _CC_BareSmoothness; \
							float _CC_BareBrightness;

#define values_concrete_f4 float4 _CC_RGBAMaskB_ST; \
							float4 _CC_TransitionNM_ST; \
							float4 _CC_ConcreteNM_ST; \
							float4 _CC_RGBAMaskA_ST;

#define values_rebar_f1 float _RBR_NormalsScale;

#define values_rebar_f4 float4 _RBR_MainTex_ST; \
						float4 _RBR_Normals_ST; \
						float4 _RBR_Smoothness_ST;

#define values_door_f1 float _DR_WoodGrain; \
						float _DR_TransitionAmount; \
						float _DR_TransitionEdgeAmount; \
						float _DR_MaskOverlay; \
						float _DR_DamageAmount; \
						float _DR_DamagePower; \
						float _DR_DamageSmooth; \
						float _DR_DamageMultiply; \
						float _DR_DoorsNMScale; \
						float _DR_DoorsDamageNMScale; \
						float _DR_DirtBrightness; \
						float _DR_DirtRange; \
						float _DR_MainSmoothness; \
						float _DR_DirtSmooth; \
						float _DR_DirtContrast; \
						float _DR_SmoothnessOverlay; \
						float _DR_SmoothnessDirt; \
						float _DR_SmoothnessDamage;

#define values_door_f4 float4 _DR_DoorsColor_ST; \
						float4 _DR_RGBAMaskA_ST; \
						float4 _DR_RGBAMaskB_ST; \
						float4 _DR_DoorDamageNM_ST; \
						float4 _DR_DoorNM_ST;

#define values_drywall_f1 float _DW_EdgesOverlay; \
							float _DW_EdgesMultiply; \
							float _DW_DamageAmount; \
							float _DW_DamageSmoothness; \
							float _DW_DamageMultiplier; \
							float _DW_DrywallNmScale; \
							float _DW_DamageNMScale; \
							float _DW_DirtOpacity; \
							float _DW_DirtRange; \
							float _DW_DirtSmooth; \
							float _DW_DirtMultiplier; \
							float _DW_DirtBrightness; \
							float _DW_TransitionAmount; \
							float _DW_TransitionBrightness; \
							float _DW_ExtraTransition; \
							float _DW_ExtraTransitionRange; \
							float _DW_EdgeBrightness; \
							float _DW_SmoothnessMain; \
							float _DW_SmoothnessDamage; \
							float _DW_SmoothnessDirt; \
							float _DW_InsideBrightness;

#define values_drywall_f4 float4 _DW_MainTex_ST; \
							float4 _DW_DrywallDamageNM_ST; \
							float4 _DW_RGBAMaskA_ST; \
							float4 _DW_DrywallNM_ST;

#define values_glass_f1 float _GL_DirtOpacity; \
						float _GL_GlassBrigthness;

#define values_glass_f4 float4 _GL_MainTex_ST; \
						float4 _GL_Glass_Normals_ST;

#define values_glass_interior_f1 float _GLI_SmoothnessValue; \
									float _GLI_WallBrightness;

#define values_glass_interior_f4 float4 _GLI_WallTex_ST; \
									float4 _GLI_GlassNM_ST;

#define values_insulation_f1 float _INS_InsulationDarkness; \
								float _INS_NormalsScale; \
								float _INS_DirtOpacity; \
								float _INS_DirtRange; \
								float _INS_DirtSmooth; \
								float _INS_DirtMultiplier; \
								float _INS_Mask;

#define values_insulation_f4 float4 _INS_InsulationMask_ST; \
								float4 _INS_Normals_ST;

#define values_plywood_f1 float _PLYW_DirtOverlay; \
							float _PLYW_EdgesOverlayMultiply; \
							float _PLYW_EdgesOverlayPower; \
							float _PLYW_SplintersOverlay; \
							float _PLYW_BrightAreas; \
							float _PLYW_EdgesBrightness; \
							float _PLYW_PlywoodDamgeNMScale; \
							float _PLYW_PlywoodNMScale; \
							float _PLYW_DamageAmount; \
							float _PLYW_DamageSmooth; \
							float _PLYW_DamageMultiplier; \
							float _PLYW_DirtOpacity; \
							float _PLYW_DirtRange; \
							float _PLYW_DirtSmooth; \
							float _PLYW_DirtMultiplier; \
							float _PLYW_SmoothnessDirt; \
							float _PLYW_SmoothnessWood; \
							float _PLYW_SmoothnessDamage; \
							float _PLYW_InsideBrightness;

#define values_plywood_f4 float4 _PLYW_MainTex_ST; \
							float4 _PLYW_RGBAMaskB_ST; \
							float4 _PLYW_PlywoodDamageNM_ST; \
							float4 _PLYW_PlywoodNM_ST;

#define values_tile_f1 float _TILE_DamageAmount; \
						float _TILE_DirtDamageOverlay; \
						float _TILE_DirtOverlay; \
						float _TILE_EdgesOverlayPower; \
						float _TILE_EdgesDamagePower; \
						float _TILE_EdgesDamageMultiply; \
						float _TILE_EdgesOverlayMultiply; \
						float _TILE_NormalDamageScale; \
						float _TILE_NormalScale; \
						float _TILE_DirtOpacity; \
						float _TILE_DirtSmooth; \
						float _TILE_DirtRange; \
						float _TILE_DirtMultiplier; \
						float _TILE_DirtPower; \
						float _TILE_SmoothnessMain; \
						float _TILE_SmoothnessOverlayDirt; \
						float _TILE_SmoothnessDirt;

#define values_tile_f4 float4 _TILE_NormalDamage_ST; \
						float4 _TILE_RGBAMaskB_ST; \
						float4 _TILE_NormalGood_ST; \
						float4 _TILE_RGBAMaskC_ST; \
						float4 _TILE_RGBAMaskA_ST;

#define values_trim_f1 float _TRIM_TransitionAmount; \
						float _TRIM_TransitionEdgeAmount; \
						float _TRIM_MaskOverlay; \
						float _TRIM_DamageAmount; \
						float _TRIM_DamagePower; \
						float _TRIM_DamageSmooth; \
						float _TRIM_DamageMultiply; \
						float _TRIM_TrimNMScale; \
						float _TRIM_TrimDamageNMScale; \
						float _TRIM_DirtBrightness; \
						float _TRIM_DirtRange; \
						float _TRIM_MainSmoothness; \
						float _TRIM_DirtSmooth; \
						float _TRIM_DirtContrast; \
						float _TRIM_SmoothnessOverlay; \
						float _TRIM_SmoothnessDirt; \
						float _TRIM_SmoothnessDamage;

#define values_trim_f4 float4 _TRIM_TrimColor_ST; \
						float4 _TRIM_RGBAMaskA_ST; \
						float4 _TRIM_RGBAMaskB_ST; \
						float4 _TRIM_TrimDamageNM_ST; \
						float4 _TRIM_TrimNM_ST; 

#define values_wallpaper_f1 float _WP_PaperBrightness; \
							float _WP_DirtOverlay; \
							float _WP_EdgesOverlayMultiply; \
							float _WP_EdgesOverlayPower; \
							float _WP_DirtDamageOverlay; \
							float _WP_EdgesAmount; \
							float _WP_EdgesBrigthness; \
							float _WP_TransitionAmount; \
							float _WP_DamageAmount; \
							float _WP_DamageSmooth; \
							float _WP_DamageMultiplier; \
							float _WP_WallpaperNMScale; \
							float _WP_EdgesNormals; \
							float _WP_DamageNormalsExtra; \
							float _WP_WallpaperDamageNMScale; \
							float _WP_DirtRange; \
							float _WP_DirtSmooth; \
							float _WP_DirtMultiplier; \
							float _WP_DirtOpacity; \
							float _WP_SmoothnessMain; \
							float _WP_SmoothnessDamage; \
							float _WP_SmoothnessDirt; \
							float _WP_PatternStrenght;

#define values_wallpaper_f4 float4 _WP_MainTex_ST; \
							float4 _WP_RGBA_Mask_A_ST; \
							float4 _WP_WallpaperDamageNM_ST; \
							float4 _WP_RGBA_Mask_B_ST; \
							float4 _WP_WallpaperNM_ST;

#define values_woodfloor_f1 float _WF_DirtOverlay; \
							float _WF_DirtDamageOverlay; \
							float _WF_EdgesOverlay; \
							float _WF_EdgesDamageOverlay; \
							float _WF_TransitionAmount; \
							float _WF_DamageAmount; \
							float _WF_DamageSmooth; \
							float _WF_DamageMultiplier; \
							float _WF_NormalDamageScale; \
							float _WF_NormalGoodScale; \
							float _WF_DirtOpacity; \
							float _WF_DirtRange; \
							float _WF_DirtSmooth; \
							float _WF_DirtMultiplier; \
							float _WF_SmoothnessMain; \
							float _WF_SmoothnessDamage; \
							float _WF_SmoothnessDirt; \
							float _WF_SmoothnessMainDirt; \
							float _WF_EdgeBrighntess;

#define values_woodfloor_f4	float4 _WF_RGBAMaskC_ST; \
							float4 _WF_RGBAMaskB_ST; \
							float4 _WF_NormalDamage_ST; \
							float4 _WF_RGBAMaskA_ST; \
							float4 _WF_NormalGood_ST; 

#define values_concrete_rubble_f1 float _CCB_ConcreteBrightness; \
									float _CCB_PaintRange; \
									float _CCB_PaintSmooth; \
									float _CCB_DamageNMTiling; \
									float _CCB_DamageScale; \
									float _CCB_DetailNMTiling; \
									float _CCB_DetailScale; \
									float _CCB_DirtContrast; \
									float _CCB_DirtRange; \
									float _CCB_DirtSmooth; \
									float _CCB_EdgesAdd; \
									float _CCB_DirtInsideMultiplier; \
									float _CCB_AODirt; \
									float _CCB_MainSmoothness; \
									float _CCB_PaintSmoothness; \
									float _CCB_DirtSmoothness;

#define values_concrete_rubble_f4 float4 _CCB_ColorMap_ST; \
									float4 _CCB_MainTex_ST; \
									float4 _CCB_MainNM_ST;

#endif