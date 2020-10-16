// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

/// Channel Mapping
#if !defined (SO_LIGHTING_UNLIT) && !defined(SO_GD_UNLIT)
	#define SO_LIGHTING_ON
#endif

#if defined (SO_SF_SPECULAR_ON) && defined(SO_LIGHTING_UNLIT) && !defined(SO_GD_UNLIT)
	#define SO_SPECULAR_ONLY
	#define SO_LIGHTING_ON
	#define SO_LIGHT_NO_AFFECT
#endif

#if !defined(SO_GD_PIPELINE_UNLIT) && defined(SO_LIGHTING_UNLIT) && !defined(SO_GD_UNLIT)
#if ( defined(SO_GD_FOG_VOLUMETRIC) || defined(SO_GD_FOG_VOLUMETRIC_3D) ) && defined(SO_GD_BASE_PASS)
	#define SO_LIGHTING_ON
#ifndef SO_LIGHT_NO_AFFECT
	#define SO_LIGHT_NO_AFFECT
#endif
#endif
#endif

// VERTEX FRAG LIGHTING
#if defined(SO_LIGHTING_ON) && defined(SO_GD_FRAG_LIGHTING_ON)
	#define SO_FRAG_LIGHTING_ON
#endif

#if defined(SO_LIGHTING_ON)
#if defined (SO_MC_V_DIRECTIONAL_ON) || defined(SO_MC_V_POINT_ON) || defined(SO_MC_V_SPOT_ON)
	#define SO_VERTEX_LIGHTING_ON
#endif
#endif

// multiply RGB * alpha
#if defined(SO_BLEND_TRANSPARENT) || defined(SO_BLEND_ADDITIVE_ALPHA) || defined(SO_BLEND_ADDITIVE_BLEND) || defined(SO_BLEND_MULTIPLY)
	#define SO_PREMULTIPLY_ALPHA
#endif

// PBR Lighting or simple ?
#if defined(SO_LIGHTING_ON) && defined(SO_SF_PBR_ON)
	#define SO_PBR_LIGHTING
#endif


//=============================================================================
/// CHECK IF REFLECTION METALLIC AND SMOOHTNESS ARE NEEDED
//=============================================================================
// REFLECTION
#if ( defined (SO_SF_REFLECT_PROBE_ON) || defined (SO_SF_REFLECT_2D_ON) || defined(SO_PBR_LIGHTING) ) && defined(SO_GD_BASE_PASS)
	#define SO_REFLECT_ON
#endif

// if metallic/smoothness is on
///SCOTTFIND SIMPLIFY THIS
#if ( defined(SO_LIGHTING_ON) || defined(SO_REFLECT_ON)|| defined (SO_SF_SPECULAR_ON) )
	#define SO_SURFACE_VARS
#endif

// RIMLIGHTING ON
#if defined (SO_SF_RIMLIT_ADD) || defined (SO_SF_RIMLIT_SUBTRACT)
	#define SO_RIMLIT_ON
#endif

// GLOBAL BUMPMAP FLAG
#if ( defined(SO_SF_LAYER0_BUMP_ON) || defined(SO_SF_LAYER1_BUMP_ON) || defined(SO_SF_LAYER2_BUMP_ON) || defined(SO_SF_LAYER3_BUMP_ON) )
	#define SO_BUMP_ON
#endif

#if defined(SO_SF_MESH_TERRAIN_ON) || defined(SO_SF_UNITY_TERRAIN_ON)
	#define SO_TERRAIN_ON
#endif

#if defined(_TERRAIN_NORMAL_MAP) && defined(SO_SF_UNITY_TERRAIN_ON)
	#define CALC_TANGENT_ON
#endif

// GLOBAL FLOWMAP FLAG
#if defined(SO_SF_LAYER0_FLOWMAP_ON) || defined(SO_SF_LAYER1_FLOWMAP_ON) || defined(SO_SF_LAYER2_FLOWMAP_ON) || defined(SO_SF_LAYER3_FLOWMAP_ON)
	#define SO_FLOWMAP_ON
#endif


// LAYERS
//#if defined (SO_SF_LAYER0_ON) || defined (SO_MC_LAYER0_ON)
	#define SO_LAYER0_ON
//#endif

#if defined (SO_SF_LAYER1_ON) || defined (SO_MC_LAYER1_ON)
	#define SO_LAYER1_ON
#endif

#if defined (SO_SF_LAYER2_ON) || defined (SO_MC_LAYER2_ON)
	#define SO_LAYER2_ON
#endif

#if defined (SO_SF_LAYER3_ON) || defined (SO_MC_LAYER3_ON)
	#define SO_LAYER3_ON
#endif


// UV's for layers
#if defined(SO_LAYER0_ON) && !defined(SO_LAYER3_ON)
	#define SO_UV1_SIZE half2
	#define SO_UV_LAYER0(a) a.uv1.xy
#endif

#if defined(SO_LAYER0_ON) && defined(SO_LAYER3_ON)
	#define SO_UV1_SIZE half4
	#define SO_UV_LAYER0(a) a.uv1.xy
	#define SO_UV_LAYER3(a) a.uv1.zw
#endif

#if !defined(SO_LAYER0_ON) && defined(SO_LAYER3_ON)
	#define SO_UV1_SIZE half2
	#define SO_UV_LAYER3(a) a.uv1.xy
#endif

#if defined(SO_LAYER1_ON) && defined(SO_LAYER2_ON)
	#define SO_UV2_SIZE half4
	#define SO_UV_LAYER1(a) a.uv2.xy
	#define SO_UV_LAYER2(a) a.uv2.zw
#endif

#if defined(SO_LAYER1_ON) && !defined(SO_LAYER2_ON)
	#define SO_UV2_SIZE half2
	#define SO_UV_LAYER1(a) a.uv2.xy
#endif

#if !defined(SO_LAYER1_ON) && defined(SO_LAYER2_ON)
	#define SO_UV2_SIZE half2
	#define SO_UV_LAYER2(a) a.uv2.xy
#endif

// UV's for cell anim blend layers
#if defined(SO_LAYER0_ON) && !defined(SO_LAYER3_ON) && defined (SO_LAYER0_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_BUV1_SIZE half2
	#define SO_BUV_LAYER0(a) a.buv1.xy
#endif

#if defined(SO_LAYER0_ON) && defined(SO_LAYER3_ON) && defined (SO_LAYER0_ANIMTYPE_CELL_ANIM_BLEND) && defined (SO_LAYER3_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_BUV1_SIZE half4
	#define SO_BUV_LAYER0(a) a.buv1.xy
	#define SO_BUV_LAYER3(a) a.buv1.zw
#endif

#if !defined(SO_LAYER0_ON) && defined(SO_LAYER3_ON) && defined (SO_LAYER3_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_BUV1_SIZE half2
	#define SO_BUV_LAYER3(a) a.buv1.xy
#endif

#if defined(SO_LAYER1_ON) && defined(SO_LAYER2_ON) && defined (SO_LAYER1_ANIMTYPE_CELL_ANIM_BLEND) && defined (SO_LAYER2_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_BUV2_SIZE half4
	#define SO_BUV_LAYER1(a) a.buv2.xy
	#define SO_BUV_LAYER2(a) a.buv2.zw
#endif

#if defined(SO_LAYER1_ON) && !defined(SO_LAYER2_ON) && defined (SO_LAYER1_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_BUV2_SIZE half2
	#define SO_BUV_LAYER1(a) a.buv2.xy
#endif

#if !defined(SO_LAYER1_ON) && defined(SO_LAYER2_ON) && defined (SO_LAYER2_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_BUV2_SIZE half2
	#define SO_BUV_LAYER2(a) a.buv2.xy
#endif

#if defined (SO_LAYER0_ANIMTYPE_CELL_ANIM) || defined (SO_LAYER0_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_LAYER0_CELLANIM_ON
#endif

#if defined (SO_LAYER1_ANIMTYPE_CELL_ANIM) || defined (SO_LAYER1_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_LAYER1_CELLANIM_ON
#endif

#if defined (SO_LAYER2_ANIMTYPE_CELL_ANIM) || defined (SO_LAYER2_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_LAYER2_CELLANIM_ON
#endif

#if defined (SO_LAYER3_ANIMTYPE_CELL_ANIM) || defined (SO_LAYER3_ANIMTYPE_CELL_ANIM_BLEND)
	#define SO_LAYER3_CELLANIM_ON
#endif


// any kinda of lighting even baked
//#if defined(SO_LIGHTING_ON) || defined(SO_VERTEX_COLOR) || defined (SO_VERTEX_LIGHTING_ON) || defined (SO_LIGHT_PROBE_VERTEX) || (!defined(LIGHTMAP_OFF) || defined(DYNAMICLIGHTMAP_ON))
//	#define SO_LIGHTRGB_ON
//#endif

// is parralax on
#if defined(SO_ALPHA_MAP_PARALLAX_HEIGHT) || defined(SO_SURFACE_MAP_PARALLAX_HEIGHT) || defined(SO_SPECULAR_MAP_PARALLAX_HEIGHT)
	#define SO_PARALLAX_ON
#endif

// light probe
#if defined (SO_GD_LIGHTPROBES_ON) && ( defined(LIGHTPROBE_SH) || defined(SO_UNITY_5) ) //&& defined(SO_LIGHTING_ON)// && defined(LIGHTPROBE_SH)
    #if defined(SO_BUMP_ON)
    	#define SO_LIGHT_PROBE_FRAG
    #else
		#define SO_LIGHT_PROBE_VERTEX
    #endif
#endif

/// VARYINGS
#if defined (SO_BUMP_ON)
		#define SO_WORLDNORMAL_ON 1
#else
	#if defined(SO_SF_FRESNEL_ON) || defined(SO_SF_SPECULAR_ON) || defined(SO_LIGHTING_ON) || defined(SO_REFLECT_ON) || ( !defined(SO_GD_UV1_NORMAL) && !defined(SO_SF_UV1_NORMAL) )
		#define SO_WORLDNORMAL_ON 1
		#define SO_WORLDNORMAL_VARY_ON 1
	#endif
#endif

#if defined (SO_GD_VERTEX_LIGHTING_ON) && !defined(SO_WORLDNORMAL_ON)
		#define SO_WORLDNORMAL_ON 1
#endif

#if defined (SO_SF_DISTORT_HORZ_ON) || defined (SO_SF_DISTORT_VERT_ON) || defined (SO_SF_DISTORT_CIRCULAR_ON)
	#define SO_DISTORT_ON
#endif

#if defined (SO_DISTORT_ON) || defined (SO_SF_SCANLINE_ON)
		#define SO_CLEAN_UV_ON 1
#endif

#if defined (SO_LAYER0_ANIMTYPE_PROGRESS) || defined (SO_LAYER1_ANIMTYPE_PROGRESS) || defined (SO_LAYER2_ANIMTYPE_PROGRESS) || defined (SO_LAYER3_ANIMTYPE_PROGRESS)
	#define SO_PROGRESS_ON
#endif

#if defined(SO_GD_PIPELINE_SHADER_ONE)
	#if defined (SHADOWS_SOFT) || defined (SHADOWS_SCREEN) || defined(SHADOWS_CUBE)
		#define SO_SHADOWS
	#endif
#endif

///SCOTTFIND fog has to work with unlit
#if ( defined(SO_GD_FOG_VOLUMETRIC) || defined(SO_GD_FOG_VOLUMETRIC_3D) ) && defined(SO_GD_BASE_PASS)

	#if defined(SO_GD_FOG_VOLUME_3D)
		#define SO_FOG_VERTEX_UV
	#endif

	#if defined (SO_LIGHTING_ON)
		#define SO_FOG_LIGHT
	#endif

#endif

#if defined(SO_GD_FRESNEL_ON) || defined (SO_REFLECT_ON) || defined (SO_RIMLIT_ON) || defined (SO_SF_SPECULAR_ON) || defined (SO_PBR_LIGHTING) || defined (SO_GD_FOG_VOLUMETRIC_3D)
	#define SO_VIEWDIR_ON
#endif

// include position world in varys
#if defined (SO_REFLECT_ON) || defined (SO_LIGHTING_ON) || defined (SO_VIEWDIR_ON) || defined (SO_GD_FOG_VOLUMETRIC_3D) || defined (SO_GD_FOG_VOLUMETRIC) || ( !defined(SO_GD_UV1_NORMAL) && !defined(SO_SF_UV1_NORMAL) )
	#define SO_POSWORLD_VARY_ON 1
#endif

#if defined (SO_SF_LAYER0_SURFACE_MAP_ON) || defined (SO_SF_LAYER1_SURFACE_MAP_ON) || defined (SO_SF_LAYER2_SURFACE_MAP_ON) || defined (SO_SF_LAYER3_SURFACE_MAP_ON)
//#if !defined (SO_SURFACE_MAPR_EMPTY) || !defined (SO_SURFACE_MAPG_EMPTY) || !defined (SO_SURFACE_MAPB_EMPTY) || !defined (SO_SURFACE_MAPA_EMPTY)
	#define SO_SURFACE_MAP_ON
//#endif
#endif

#if defined (SO_SF_LAYER0_SPECULAR_MAP_ON) || defined (SO_SF_LAYER1_SPECULAR_MAP_ON) || defined (SO_SF_LAYER2_SPECULAR_MAP_ON) || defined (SO_SF_LAYER3_SPECULAR_MAP_ON)
	#define SO_SPECULAR_MAP_ON
#endif

#ifdef SO_SF_UNITY_TERRAIN_ON
	#define LAYER0_TEX_ST  _Splat0_ST
	#define LAYER1_TEX_ST  _Splat1_ST
	#define LAYER2_TEX_ST  _Splat2_ST
	#define LAYER3_TEX_ST  _Splat3_ST

	#define LAYER0_TEX    _Splat0
	#define LAYER1_TEX    _Splat1
	#define LAYER2_TEX    _Splat2
	#define LAYER3_TEX    _Splat3

	#define LAYER0_BUMP    _Normal0
	#define LAYER1_BUMP    _Normal1
	#define LAYER2_BUMP    _Normal2
	#define LAYER3_BUMP    _Normal3

	#define LAYER0_GLOSSINESS _Smoothness0
	#define LAYER1_GLOSSINESS _Smoothness1
	#define LAYER2_GLOSSINESS _Smoothness2
	#define LAYER3_GLOSSINESS _Smoothness3

	#define LAYER0_METALLIC _Metallic0
	#define LAYER1_METALLIC _Metallic1
	#define LAYER2_METALLIC _Metallic2
	#define LAYER3_METALLIC _Metallic3

   // #define LAYER_TEX(layerNum) _Splat##layerNum
   // #define LAYER_NORMAL(layerNum) _Normal##layerNum
   // #define LAYER_SMOOTHNESS(layerNum) _Smoothness##layerNum
   // #define LAYER_METALLIC(layerNum) _Metallic##layerNum
#else
	#define LAYER0_TEX_ST  _MainTex_ST
	#define LAYER1_TEX_ST  _Layer1Tex_ST
	#define LAYER2_TEX_ST  _Layer2Tex_ST
	#define LAYER3_TEX_ST  _Layer3Tex_ST

	#define LAYER0_TEX    _MainTex
	#define LAYER1_TEX    _Layer1Tex
	#define LAYER2_TEX    _Layer2Tex
	#define LAYER3_TEX    _Layer3Tex

	#define LAYER0_BUMP    _BumpMap
	#define LAYER1_BUMP    _Layer1BumpMap
	#define LAYER2_BUMP    _Layer2BumpMap
	#define LAYER3_BUMP    _Layer3BumpMap

	#define LAYER0_GLOSSINESS _Glossiness
	#define LAYER1_GLOSSINESS _Layer1Smoothness
	#define LAYER2_GLOSSINESS _Layer2Smoothness
	#define LAYER3_GLOSSINESS _Layer3Smoothness

	#define LAYER0_METALLIC _Metallic
	#define LAYER1_METALLIC _Layer1Metallic
	#define LAYER2_METALLIC _Layer2Metallic
	#define LAYER3_METALLIC _Layer3Metallic

#endif

#ifdef SO_SF_SPECULAR_ON
	#define SO_HALFDIR_ON
	#define SO_DOTNH_ON
	#if defined (SO_GD_SPECULAR_HQ) || defined(SO_GD_SPECULAR_NORMAL)|| defined(SO_GD_FRESNEL_ON)
	#define SO_DOTLH_ON
	#endif
#endif


#if !defined (SHADOWS_SCREEN) && !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE)
	#define SO_SHADOWS_OFF
#endif









