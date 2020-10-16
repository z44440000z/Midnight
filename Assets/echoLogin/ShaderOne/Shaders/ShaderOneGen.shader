// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

Shader "echoLogin/shaderOne"
{
	Properties
	{
		_ControlScriptFlag 	("", Float)		  		  		= 0
		_SrcBlend 			("", Float) 					= 1
    	_DstBlend 			("", Float)						= 0
		_SrcBlendAddPass	("", Float) 					= 1
		_DstBlendAddPass	("", Float)						= 1
		_ZWrite   			("", Float)						= 1
		_MyCullMode   		("", Float)						= 2
 		_Mode 				("", float )					= 0
		_SO_PBRToggle   	("", float )					= 1
		_SO_ParticleMode  	("", float )					= 0
 		_VertexColorMode	("", float )					= 0
 		_SpecularHighlights	("", float )					= 0

		_SpecularMapType 	("", float )					= 0
		_AlphaMapType 		("", float )					= 0
		_SurfaceMapTypeR 	("", float )					= 0
		_SurfaceMapTypeG 	("", float )					= 0
		_SurfaceMapTypeB 	("", float )					= 0
		_SurfaceMapTypeA 	("", float )					= 0

		_SO_BendToggle( "", float ) 						= 0
		//_SO_BendAmountX( "", float ) 					= 0
		//_SO_BendAmountZ( "", float ) 						= 0

		_Color ("", Color ) 								= ( 1, 1, 1, 1 )
		_ColorBack ("", Color ) 							= ( 1, 1, 1, 1 )
		_TintColor ("", Color ) 							= ( 1, 1, 1, 1 )
		_ColorAmplify( "", float ) 							= 1

		_SO_UV1_WorldMap( "", float ) 						= 0
		_SO_UV1_WorldMapScale( "", float ) 					= 1

		_OptionsFold( "", float ) 							= 1

		_LightFold( "", float ) 							= 1
		_LightMode( "", float ) 							= 0
		_LightProbes("", float )	 				   		= 0

		_SaturationFold( "", float ) 						= 0
		_SaturationToggle( "", float ) 						= 0
		_SaturationStrength ("", Range ( 0.0 , 1.0 ) )		= 1

		_IntersectFold ( "", float )						= 0
		_IntersectToggle ( "", float )						= 0
		_IntersectColor ("", Color )  						= ( 1, 1, 1, 1 )
		_IntersectThreshold ( "", float )					= 1

		_LayerProgressGradient ("", 2D) 					= "white" {}

		_Layer0Fold( "", float ) 							= 1
		_Layer0Toggle( "", float ) 							= 1
		_Layer0UV2( "", float ) 							= 0
		_MainTex ("", 2D) 			         				= "white" {}
		_BumpMap ("", 2D) 									= "white" {}
		_BumpScale( "", float ) 							= 1
	   	_Layer0AOScale( "", float )    						= 1
		_Layer0BlendMode ("", float ) 						= 0
	   	_Glossiness( "", Range ( 0.0 , 1.0 ) ) 				= 0
		_Metallic( "", Range ( 0.0 , 1.0 ) ) 				= 0
		_MetallicGlossMap ("", 2D) 							= "white" {}
		_Parallax ("Height Scale", Range (0.005, 0.1)) 		= 0.02
		_Layer0ScaleGloss ( "", range ( 0.0, 2.0 ) )		= 0
		_Layer0Fresnel( "", Range ( 0.0 , 1.0 ) ) 			= 0
		_SpecColor ("", Color ) 							= ( 1, 1, 1, 1 )
		_SpecGlossMap ("", 2D) 								= "white" {}
		_Layer0FlowMap	("", 2D) 	  						= "white" {}
		_Layer0FlowSpeed ("", float ) 						= 0.1
		_Layer0FlowColor ("", Color )    					= ( 0, 0, 0, 0 )
		_Layer0ScaleU ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer0ScaleV ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer0PosU ("", Range ( -0.5, 0.5 ) )              = 0.0
		_Layer0PosV ("", Range ( -0.5, 0.5 ) )              = 0.0
		_Layer0ScrollU ("", float ) 	    	         	= 0.0
		_Layer0ScrollV ("", float )    	    	      		= 0.0
		_Layer0Rotation ("", float )    					= 0.0
		_Layer0RotationU ("", float )    					= 0.5
		_Layer0RotationV ("", float )    					= 0.5
		_Layer0Color ("", Color )			   				= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer0AnimType ("", float )						= 0
		_Layer0AnimActive ( "", float )						= 0
		_Layer0AnimFPS("", float )	 	  					= 0
		_Layer0AnimLoopMode("", float )	 	  				= 0
		_Layer0ProgressEdge ( "", Range ( 0.001, 0.4 ) )    = 0.001
		_Layer0ProgressColor ("", Color )					= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer0ProgressColorAmp ("", float )				= 1.0
		_Layer0Progress ( "", Range ( 0.0, 1.0 ) )          = 0.0
		_Layer0AnimCellsHorz( "", float ) 					= 1
		_Layer0AnimCellsVert( "", float ) 					= 1
		_Layer0AnimCellStart ( "", float ) 					= 0
		_Layer0AnimCellEnd ( "", float ) 					= 0
//		_Layer0AnimOffsetX ( "", float ) 					= 0
//		_Layer0AnimOffsetY ( "", float ) 					= 0
//		_Layer0AnimOffsetX2 ( "", float ) 					= 0
//   	_Layer0AnimOffsetY2 ( "", float ) 					= 0
//   	_Layer0AnimBlend ( "", float ) 						= 0
		_Layer0DistortionStrength ( "", float ) 			= 1

		_Layer1Fold( "", float ) 							= 0
		_Layer1Toggle( "", float ) 							= 0
		_Layer1UV2( "", float ) 							= 0
		_Layer1Tex ("", 2D) 								= "white" {}
		_Layer1BumpMap ("", 2D) 							= "white" {}
		_Layer1BumpScale( "", float )    					= 1
		_Layer1AOScale( "", float )    						= 1
		_Layer1BlendMode ("", float ) 						= 0
		_Layer1Smoothness( "", Range ( 0.0 , 1.0 ) ) 		= 0
		_Layer1Metallic( "", Range ( 0.0 , 1.0 ) ) 			= 0
   		_Layer1Parallax ("", Range (0.005, 0.1)) 			= 0.02
		_Layer1ScaleGloss ( "", range ( 0.0, 2.0 ) )		= 0
		_Layer1Fresnel( "", Range ( 0.0 , 1.0 ) ) 			= 0
		_Layer1SpecColor ("", Color ) 						= ( 1, 1, 1, 1 )
		_Layer1SpecGlossMap ("", 2D) 						= "white" {}
		_Layer1SurfaceMap ("", 2D) 							= "white" {}
		_Layer1FlowMap 			("", 2D) 	  				= "white" {}
		_Layer1FlowSpeed ("", float ) 						= 0.1
		_Layer1FlowColor ("", Color )    					= ( 0, 0, 0, 0 )
		_Layer1ScaleU ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer1ScaleV ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer1PosU ("", Range ( -0.5, 0.5 ) )              = 0.0
		_Layer1PosV ("", Range ( -0.5, 0.5 ) )              = 0.0
		_Layer1ScrollU ("", float ) 	    	        	= 0.0
		_Layer1ScrollV ("", float )    	    	    		= 0.0
		_Layer1Rotation ("", float )    					= 0.0
		_Layer1RotationU ("", float )    					= 0.5
		_Layer1RotationV ("", float )    					= 0.5
		_Layer1Color ("", Color )							= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer1AnimType ("", float )						= 0
		_Layer1AnimActive ( "", float )						= 0
		_Layer1AnimFPS("", float )	  	 					= 0
		_Layer1AnimLoopMode("", float )	 	  				= 0
		_Layer1ProgressEdge ( "", Range ( 0.001, 0.4 ) )    = 0.001
		_Layer1ProgressColor ("", Color )					= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer1ProgressColorAmp ("", float )				= 1.0
		_Layer1Progress ( "", Range ( 0.0, 1.0 ) )          = 0.0
		_Layer1AnimCellsHorz( "", float ) 					= 0
		_Layer1AnimCellsVert( "", float ) 					= 1
		_Layer1AnimCellStart ( "", float ) 					= 0
		_Layer1AnimCellEnd ( "", float ) 					= 0
//		_Layer1AnimOffsetX ( "", float ) 					= 0
//		_Layer1AnimOffsetY ( "", float ) 					= 0
//		_Layer1AnimOffsetX2 ( "", float ) 					= 0
//		_Layer1AnimOffsetY2 ( "", float ) 					= 0
//		_Layer1AnimBlend ( "", float ) 						= 0
		_Layer1DistortionStrength ( "", float ) 			= 1

		_Layer2Fold( "", float ) 							= 0
		_Layer2Toggle( "", float ) 							= 0
		_Layer2UV2( "", float ) 							= 0
		_Layer2Tex ("", 2D) 								= "white" {}
		_Layer2BumpMap ("", 2D) 							= "white" {}
		_Layer2BumpScale( "", float )    					= 1
		_Layer2AOScale( "", float )    						= 1
		_Layer2BlendMode ("", float ) 						= 0
		_Layer2Smoothness( "", Range ( 0.0 , 1.0 ) ) 		= 0
		_Layer2Metallic( "", Range ( 0.0 , 1.0 ) ) 			= 0
		_Layer2Parallax ("", Range (0.005, 0.1)) 			= 0.02
		_Layer2ScaleGloss ( "", range ( 0.0, 2.0 ) )		= 0
		_Layer2Fresnel( "", Range ( 0.0 , 1.0 ) ) 			= 0
		_Layer2SpecColor ("", Color ) 						= ( 1, 1, 1, 1 )
		_Layer2SpecGlossMap ("", 2D) 						= "white" {}
		_Layer2SurfaceMap ("", 2D) 							= "white" {}
		_Layer2FlowMap	("", 2D) 	  						= "white" {}
		_Layer2FlowSpeed ("", float ) 						= 0.1
		_Layer2FlowColor ("", Color )    					= ( 0, 0, 0, 0 )
		_Layer2ScaleU ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer2ScaleV ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer2PosU ("", Range ( -0.5, 0.5 ) )              = 0.0
		_Layer2PosV ("", Range ( -0.5, 0.5 ) )              = 0.0
		_Layer2ScrollU ("", float ) 	    	        	= 0.0
		_Layer2ScrollV ("", float )    	    	    		= 0.0
		_Layer2Rotation ("", float )  				  		= 0.0
		_Layer2RotationU ("", float )    					= 0.5
		_Layer2RotationV ("", float )    					= 0.5
		_Layer2Color ("", Color )							= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer2AnimType ("", float )						= 0
		_Layer2AnimActive ( "", float )						= 0
		_Layer2AnimFPS("", float )	   						= 0
		_Layer2AnimLoopMode("", float )	 	  				= 0
		_Layer2ProgressEdge ( "", Range ( 0.001, 0.4 ) )    = 0.001
		_Layer2ProgressColor ("", Color )					= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer2ProgressColorAmp ("", float )				= 1.0
		_Layer2Progress ( "", Range ( 0.0, 1.0 ) )          = 0.0
		_Layer2AnimCellsHorz( "", float ) 					= 1
		_Layer2AnimCellsVert( "", float ) 					= 1
		_Layer2AnimCellStart ( "", float ) 					= 0
		_Layer2AnimCellEnd ( "", float ) 					= 0
//		_Layer2AnimOffsetX ( "", float ) 					= 0
//	    _Layer2AnimOffsetY ( "", float ) 					= 0
//		_Layer2AnimOffsetX2 ( "", float ) 					= 0
//		_Layer2AnimOffsetY2 ( "", float ) 					= 0
//		_Layer2AnimBlend ( "", float ) 						= 0
		_Layer2DistortionStrength ( "", float ) 			= 1

		_Layer3Fold( "", float ) 							= 0
		_Layer3Toggle( "", float ) 							= 0
		_Layer3UV2( "", float ) 							= 0
		_Layer3Tex ("", 2D) 								= "white" {}
		_Layer3BumpMap ("", 2D) 							= "white" {}
		_Layer3BumpScale( "", float )    					= 1
		_Layer3AOScale( "", float )    						= 1
		_Layer3BlendMode ("", float ) 						= 0
		_Layer3Smoothness( "", Range ( 0.0 , 1.0 ) ) 		= 0
		_Layer3Metallic( "", Range ( 0.0 , 1.0 ) ) 			= 0
		_Layer3Parallax ("", Range (0.005, 0.1)) 			= 0.02
		_Layer3ScaleGloss ( "", range ( 0.0, 2.0 ) )		= 0
		_Layer3Fresnel( "", Range ( 0.0 , 1.0 ) ) 			= 0
		_Layer3SpecColor ("", Color ) 						= ( 1, 1, 1, 1 )
		_Layer3SpecGlossMap ("", 2D) 						= "white" {}
		_Layer3SurfaceMap ("", 2D) 							= "white" {}
		_Layer3FlowMap	("", 2D) 	  						= "white" {}
		_Layer3FlowSpeed ("", float ) 						= 0.1
		_Layer3FlowColor ("", Color )    					= ( 0, 0, 0, 0 )
		_Layer3ScaleU ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer3ScaleV ("", Range ( 0.0, 2.0 ) )             = 1.0
		_Layer3PosU ("", Range ( -0.5, 0.5 ) )             	= 0.0
		_Layer3PosV ("", Range ( -0.5, 0.5 ) )             	= 0.0
		_Layer3ScrollU ("", float ) 	    	        	= 0.0
		_Layer3ScrollV ("", float )    	    	    		= 0.0
		_Layer3Rotation ("", float ) 				   		= 0.0
		_Layer3RotationU ("", float )    					= 0.5
		_Layer3RotationV ("", float )    					= 0.5
		_Layer3Color ("", Color )							= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer3AnimType ("", float )						= 0
		_Layer3AnimActive ( "", float )						= 0
		_Layer3AnimFPS("", float )	 	  					= 0
		_Layer3AnimLoopMode("", float )	 	  				= 0
		_Layer3ProgressEdge ( "", Range ( 0.001, 0.4 ) )    = 0.001
		_Layer3ProgressColor ("", Color )					= ( 1.0, 1.0, 1.0, 1.0 )
		_Layer3ProgressColorAmp ("", float )				= 1.0
		_Layer3Progress ( "", Range ( 0.0, 1.0 ) )          = 0.0
		_Layer3AnimCellsHorz( "", float ) 					= 1
		_Layer3AnimCellsVert( "", float ) 					= 1
		_Layer3AnimCellStart ( "", float ) 					= 0
		_Layer3AnimCellEnd ( "", float ) 					= 0
//		_Layer3AnimOffsetX ( "", float ) 					= 0
//		_Layer3AnimOffsetY ( "", float ) 					= 0
//		_Layer3AnimOffsetX2 ( "", float ) 					= 0
//		_Layer3AnimOffsetY2 ( "", float ) 					= 0
//		_Layer3AnimBlend ( "", float ) 						= 0
		_Layer3DistortionStrength ( "", float ) 			= 1

   		_MoveSpeed ( "", float )							= 1
		_MoveStrength ( "", float )							= 0.2
		_MoveDirection ( "", Vector )						= ( 0.0, 0.0, 1.0, 0.0 )

		_ManualControl ( "", float )						= 0
    	_ScrollU ( "", float )								= 0
    	_ScrollV ( "", float ) 								= 0

		_ScanlineFold("", float ) 							= 0
		_ScanlineToggle("", float ) 						= 0
    	_ScanlineStrengthHorz("", Range( 0 , 1 ) ) 			= 0
		_ScanlineScrollHorz("", float ) 					= 0
		_ScanlineCountHorz("", float ) 						= 128
		_ScanlineWidthHorz("", Range( -2 , 2 ) ) 			= 0.5
    	_ScanlineStrengthVert("", Range( 0 , 1 ) ) 			= 0
		_ScanlineScrollVert("", float ) 					= 0
		_ScanlineCountVert("", float ) 						= 128
		_ScanlineWidthVert("", Range( -2 , 2 ) ) 			= 0.5

		_GlossyReflections("", float ) 						= 0
		_ReflectTex ("", 2D) 								= "white" {}
		_ReflectTexFlag("", float )    						= 0

		_TerrainToggle("", float ) 							= 0
		_Control ("", 2D) 									= "black" {}

		_FogToggle("", float ) 								= 0

		_DistortFold("", float ) 							= 0
   		_DistortToggle("", float ) 							= 0
		_DistortHorzStrength ("", Range ( 0 , 1.0 ) )		= 0
		_DistortHorzCount ("", Range ( 0 , 1024 ) ) 		= 0
		_DistortHorzSpeed ("", float ) 						= 0
		_DistortHorzWave ("", Range ( 0 , 1 ) ) 			= 0

		_DistortVertStrength ("", Range ( 0 , 1.0 ) ) 		= 0
		_DistortVertCount ("", Range ( 0 , 1024 ) ) 		= 0
		_DistortVertSpeed ("", float ) 						= 0
		_DistortVertWave ("", Range ( 0 , 1 ) ) 			= 0

		_DistortCircularStrength ("", Range ( 0 , 1.0 ) ) 	= 0
		_DistortCircularCount ("", Range ( 0 , 1024 ) ) 	= 0
		_DistortCircularSpeed ("", float ) 					= 0
		_DistortCircularCenterU ("", Range ( 0 , 1 ) ) 		= 0.5
		_DistortCircularCenterV ("", Range ( 0 , 1 ) ) 		= 0.5

		_RGBOffsetFold("", float ) 							= 0
		_RGBOffsetToggle("", float ) 						= 0
		_RGBOffsetStrength ("", Range ( 0 , 1 ) ) 			= 0
		_RGBOffsetAmount ("", Range ( -1 , 1 ) ) 			= 0
		_RGBOffsetMode ("", int )  	 						= 0

		_RimFold ("", float ) 								= 0
		_RimToggle ("", float ) 							= 0
		_RimColor ("", Color ) 								= ( 1, 1, 1, 1 )
		_RimWidth ("", Range ( 0 , 10.0 ) )    				= 0
		_RimBlend ("", float ) 								= 0

		_EmissionFold ("", float ) 							= 0
		_EmissionToggle ("", float ) 						= 0
    	_EmissionMap ("Emission Map", 2D) 					= "white" {}
    	[HDR]_EmissionColor ("Emission Color", Color ) 		= (0,0,0,0)
		_EmissionAffectObject("",float)                     = 1


//    	[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
	  [HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
	  [HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
	  [HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
	  [HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
	  [HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
	  [HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
	  [HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
	  [HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}
	  [HideInInspector][Gamma] _Metallic0("Metallic 0", Range(0.0, 1.0)) = 0.0
	  [HideInInspector][Gamma] _Metallic1("Metallic 1", Range(0.0, 1.0)) = 0.0
	  [HideInInspector][Gamma] _Metallic2("Metallic 2", Range(0.0, 1.0)) = 0.0
	  [HideInInspector][Gamma] _Metallic3("Metallic 3", Range(0.0, 1.0)) = 0.0
	  [HideInInspector] _Smoothness0("Smoothness 0", Range(0.0, 1.0)) = 1.0
	  [HideInInspector] _Smoothness1("Smoothness 1", Range(0.0, 1.0)) = 1.0
	  [HideInInspector] _Smoothness2("Smoothness 2", Range(0.0, 1.0)) = 1.0
	  [HideInInspector] _Smoothness3("Smoothness 3", Range(0.0, 1.0)) = 1.0

   }

	SubShader
	{
		Tags {"Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Geometry" }
        //Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]

Pass
{
Tags { "LightMode" = "ForwardBase" }
Blend [_SrcBlend] [_DstBlend]
Cull [_MyCullMode]
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
	#define SO_GD_DIR_COUNT 1
	#define SO_GD_V_DIR_START 1
	#define SO_GD_V_DIR_END 0
	#define SO_GD_P_DIR_START 0 
	#define SO_GD_P_DIR_END 0
	#define SO_GD_POINT_COUNT 4
	#define SO_GD_V_POINT_START 4
	#define SO_GD_V_POINT_END 3
	#define SO_GD_P_POINT_START 0 
	#define SO_GD_P_POINT_END 3
	#define SO_GD_SPOT_COUNT 4
	#define SO_GD_V_SPOT_START 4
	#define SO_GD_V_SPOT_END 3
	#define SO_GD_P_SPOT_START 0 
	#define SO_GD_P_SPOT_END 3

	#define SO_GD_BUMP_SCALE_GLOBAL
	#define SO_GD_AO_SCALE_GLOBAL
	#define SO_UV1_WORLDMAP_MESH_UV
	#define SO_GD_PIPELINE_UNITY_FORWARD 1 
	#pragma multi_compile __ LIGHTPROBE_SH
	#define SO_GD_LIGHTPROBES_ON
	#pragma multi_compile DIRECTIONAL DIRECTIONAL_COOKIE POINT POINT_COOKIE SPOT SPOT_COOKIE
	#pragma multi_compile __ VERTEXLIGHT_ON
	#define SO_GD_BASE_PASS 1
	#define SO_GD_VERTEX_LIGHTING_ON 
	#define SO_GD_FRAG_LIGHTING_ON 

	#define SO_GD_WORKFLOW_SMOOTHNESS
	#pragma shader_feature SO_SF_PBR_ON
	#define SO_GD_FRESNEL_ON
	#define SO_GD_FRESNEL_POW4
	#define SO_GD_SPECULAR_NORMAL
	#pragma shader_feature SO_SF_SPECULAR_ON
	#define SO_GD_SPECULAR_BLEND_COLOR

	#pragma shader_feature SO_SF_LIGHTING_BIT1
	#pragma shader_feature SO_SF_LIGHTING_BIT2


	#define SO_SURFACE_MAPR_METALLIC


	#pragma shader_feature SO_SF_SURFACE_MAPG_BIT1
	#pragma shader_feature SO_SF_SURFACE_MAPG_BIT2
	#pragma shader_feature SO_SF_SURFACE_MAPG_BIT3


	#pragma shader_feature SO_SF_SURFACE_MAPB_BIT1
	#pragma shader_feature SO_SF_SURFACE_MAPB_BIT2
	#pragma shader_feature SO_SF_SURFACE_MAPB_BIT3


	#define SO_SURFACE_MAPA_SMOOTHNESS


	#pragma shader_feature SO_SF_ALPHA_MAP_BIT1
	#pragma shader_feature SO_SF_ALPHA_MAP_BIT2
	#pragma shader_feature SO_SF_ALPHA_MAP_BIT3

	#pragma shader_feature SO_SF_REFLECT_PROBE_ON
	#define SOFLOAT half
	#define SOFLOAT2 half2
	#define SOFLOAT3 half3
	#define SOFLOAT4 half4

	//THIS IS THE FUTURE
	//THIS IS THE YEAR 1997

	#pragma shader_feature SO_SF_LAYER0_BUMP_ON
	#pragma shader_feature SO_SF_LAYER0_SURFACE_MAP_ON
	#pragma shader_feature SO_SF_LAYER0_SCROLLUV_ON
	#pragma shader_feature SO_SF_LAYER0_ROTATEUV_ON

	#pragma shader_feature SO_SF_LAYER1_ON


	#pragma shader_feature SO_SF_DISTORT_HORZ_ON
	#pragma shader_feature SO_SF_DISTORT_VERT_ON
	#pragma shader_feature SO_SF_DISTORT_CIRCULAR_ON
	#pragma shader_feature SO_SF_RGBOFFSET_ON
	#pragma shader_feature SO_SF_SATURATION_ON
	#pragma shader_feature SO_SF_SCANLINE_ON
	#pragma multi_compile __ _EMISSION

#pragma multi_compile __ SO_MC_CONTROL_SCRIPT_ON
#pragma shader_feature SO_SF_MANUAL_CONTROL
#pragma shader_feature SO_SF_NORMAL_FIX
#pragma shader_feature SO_SF_MANAGER_ON
#pragma shader_feature SO_SF_BLEND_BIT1
#pragma shader_feature SO_SF_BLEND_BIT2
#pragma shader_feature SO_SF_BLEND_BIT3
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "ShaderOneGen_BitDecode.cginc"
#include "ShaderOneDefine.cginc"
#include "ShaderOneVars.cginc"
#include "ShaderOneGen_FuncMaps.cginc"
#include "ShaderOneUnity.cginc"
#include "ShaderOneGen_UnityLightMacros.cginc"
#include "ShaderOneFunc.cginc"
#include "ShaderOneFuncLayersBlend.cginc"
#include "ShaderOneFuncLayer.cginc"
#include "ShaderOneFuncLighting.cginc"
#include "ShaderOneGen_Instancing.cginc"
#include "ShaderOneVert.cginc"
#include "ShaderOneFrag.cginc"
ENDCG
}


Pass
{
Tags { "LightMode" = "ForwardAdd" }
Blend [_SrcBlendAddPass] [_DstBlendAddPass]
Cull [_MyCullMode]
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
	#define SO_GD_DIR_COUNT 1
	#define SO_GD_V_DIR_START 1
	#define SO_GD_V_DIR_END 0
	#define SO_GD_P_DIR_START 0 
	#define SO_GD_P_DIR_END 0
	#define SO_GD_POINT_COUNT 4
	#define SO_GD_V_POINT_START 4
	#define SO_GD_V_POINT_END 3
	#define SO_GD_P_POINT_START 0 
	#define SO_GD_P_POINT_END 3
	#define SO_GD_SPOT_COUNT 4
	#define SO_GD_V_SPOT_START 4
	#define SO_GD_V_SPOT_END 3
	#define SO_GD_P_SPOT_START 0 
	#define SO_GD_P_SPOT_END 3

	#define SO_GD_BUMP_SCALE_GLOBAL
	#define SO_GD_AO_SCALE_GLOBAL
	#define SO_UV1_WORLDMAP_MESH_UV
	#define SO_GD_PIPELINE_UNITY_FORWARD 1 
	#pragma multi_compile_lightpass
	#define SO_GD_ADD_PASS
	#define SO_GD_VERTEX_LIGHTING_ON 
	#define SO_GD_FRAG_LIGHTING_ON 

	#define SO_GD_WORKFLOW_SMOOTHNESS
	#pragma shader_feature SO_SF_PBR_ON
	#define SO_GD_FRESNEL_ON
	#define SO_GD_FRESNEL_POW4
	#define SO_GD_SPECULAR_NORMAL
	#pragma shader_feature SO_SF_SPECULAR_ON
	#define SO_GD_SPECULAR_BLEND_COLOR

	#pragma shader_feature SO_SF_LIGHTING_BIT1
	#pragma shader_feature SO_SF_LIGHTING_BIT2


	#define SO_SURFACE_MAPR_METALLIC


	#pragma shader_feature SO_SF_SURFACE_MAPG_BIT1
	#pragma shader_feature SO_SF_SURFACE_MAPG_BIT2
	#pragma shader_feature SO_SF_SURFACE_MAPG_BIT3


	#pragma shader_feature SO_SF_SURFACE_MAPB_BIT1
	#pragma shader_feature SO_SF_SURFACE_MAPB_BIT2
	#pragma shader_feature SO_SF_SURFACE_MAPB_BIT3


	#define SO_SURFACE_MAPA_SMOOTHNESS


	#pragma shader_feature SO_SF_ALPHA_MAP_BIT1
	#pragma shader_feature SO_SF_ALPHA_MAP_BIT2
	#pragma shader_feature SO_SF_ALPHA_MAP_BIT3

	#pragma shader_feature SO_SF_REFLECT_PROBE_ON
	#define SOFLOAT half
	#define SOFLOAT2 half2
	#define SOFLOAT3 half3
	#define SOFLOAT4 half4

	//THIS IS THE FUTURE
	//THIS IS THE YEAR 1997

	#pragma shader_feature SO_SF_LAYER0_BUMP_ON
	#pragma shader_feature SO_SF_LAYER0_SURFACE_MAP_ON
	#pragma shader_feature SO_SF_LAYER0_SCROLLUV_ON
	#pragma shader_feature SO_SF_LAYER0_ROTATEUV_ON

	#pragma shader_feature SO_SF_LAYER1_ON


	#pragma shader_feature SO_SF_DISTORT_HORZ_ON
	#pragma shader_feature SO_SF_DISTORT_VERT_ON
	#pragma shader_feature SO_SF_DISTORT_CIRCULAR_ON
	#pragma shader_feature SO_SF_RGBOFFSET_ON
	#pragma shader_feature SO_SF_SATURATION_ON
	#pragma shader_feature SO_SF_SCANLINE_ON
	#pragma multi_compile __ _EMISSION

#pragma multi_compile __ SO_MC_CONTROL_SCRIPT_ON
#pragma shader_feature SO_SF_MANUAL_CONTROL
#pragma shader_feature SO_SF_NORMAL_FIX
#pragma shader_feature SO_SF_MANAGER_ON
#pragma shader_feature SO_SF_BLEND_BIT1
#pragma shader_feature SO_SF_BLEND_BIT2
#pragma shader_feature SO_SF_BLEND_BIT3
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "ShaderOneGen_BitDecode.cginc"
#include "ShaderOneDefine.cginc"
#include "ShaderOneVars.cginc"
#include "ShaderOneGen_FuncMaps.cginc"
#include "ShaderOneUnity.cginc"
#include "ShaderOneGen_UnityLightMacros.cginc"
#include "ShaderOneFunc.cginc"
#include "ShaderOneFuncLayersBlend.cginc"
#include "ShaderOneFuncLayer.cginc"
#include "ShaderOneFuncLighting.cginc"
#include "ShaderOneGen_Instancing.cginc"
#include "ShaderOneVert.cginc"
#include "ShaderOneFrag.cginc"
ENDCG
}



// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

Pass
{
    Name "META"
    Tags {"LightMode"="Meta"}
    Cull Off
    CGPROGRAM

    #include"UnityStandardMeta.cginc"

    //float4 _EmissionColor;
	sampler2D _EmissionTex;
	float  _EmissionScale;

    float4 frag_meta2 (v2f_meta i): SV_Target
    {
        FragmentCommonData data = UNITY_SETUP_BRDF_INPUT (i.uv);
        UnityMetaInput o;
        UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

        o.Albedo = tex2D ( _EmissionMap, i.uv ).rgb;

        o.Emission = _EmissionColor.rgb;
        return UnityMetaFragment(o);
    }

    #pragma vertex vert_meta
    #pragma fragment frag_meta2
    #pragma shader_feature _EMISSION
    #pragma shader_feature _METALLICGLOSSMAP
    #pragma shader_feature ___ _DETAIL_MULX2
    ENDCG

}





	}

	CustomEditor "ShaderOneMaterialEditor"
}



