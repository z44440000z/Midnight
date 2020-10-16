// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

struct appdata
{
	float4 vertex	: POSITION;
	float3 normal   : NORMAL;
#ifndef CALC_TANGENT_ON
	float4 tangent  : TANGENT;
#endif
	float2 uv 		: TEXCOORD0;
	float2 uv2 		: TEXCOORD1;
	float4 color    : COLOR;

#ifdef LIGHTMAP_ON
	float4 texcoord1: TEXCOORD2;
#endif

#ifdef DYNAMICLIGHTMAP_ON
	float4 texcoord2: TEXCOORD3;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	half4 pos 			: SV_POSITION;

#ifdef SO_GD_FOG_VOLUMETRIC_3D
	half3       fogUV   : TEXCOORD14;
#endif

#ifdef SO_VERTEX_SPLAT
	half4       vertexSplatControl : TEXCOORD16;
#endif

#ifdef SO_UV1_SIZE
    SO_UV1_SIZE uv1 	: TEXCOORD0;
#endif

#ifdef SO_UV2_SIZE
	SO_UV2_SIZE uv2 	: TEXCOORD1;
#endif

#ifdef SO_BUV1_SIZE
    SO_BUV1_SIZE buv1 	: TEXCOORD2;
#endif

#ifdef SO_BUV2_SIZE
	SO_BUV2_SIZE buv2 	: TEXCOORD3;
#endif

#ifdef SO_CLEAN_UV_ON
	half2 normUV 			: TEXCOORD4;
#endif

#ifdef SO_TERRAIN_ON
	half2 controlUV : TEXCOORD5;
#endif

#ifdef SO_POSWORLD_VARY_ON
	half3 positionWorld 	: TEXCOORD6;
#endif

#ifdef SO_WORLDNORMAL_VARY_ON
	half3 normalDir         : TEXCOORD7;
#endif

#ifdef SO_BUMP_ON
	half3 tspace0 : TEXCOORD8;
	half3 tspace1 : TEXCOORD9;
	half3 tspace2 : TEXCOORD10;
#endif

///SCOTTFIND
//#ifdef SO_SHADOWS
//   SHADOW_COORDS(8)
//#else
//	#ifdef SO_LIGHTING_ON
	LIGHTING_COORDS(11,12)
//	#endif
//#endif

#if defined (SO_GD_VERTEX_LIGHTING_ON) || defined (SO_LIGHT_PROBE_VERTEX)
	SOFLOAT4 vertexLighting      	: TEXCOORD13;
#endif

#if defined(SO_VERTEX_COLOR)
	SOFLOAT4 vertexColor :  COLOR;
#endif

#ifdef DYNAMICLIGHTMAP_ON
		half2 dlmUV         : TEXCOORD14;
#endif

#ifdef LIGHTMAP_ON
		half2 lmUV          : TEXCOORD15;
#endif

#ifdef SO_SF_INTERSECT_ON
	half4 screenPos         : TEXCOORD16;
#endif

#ifdef SO_PARALLAX_ON
	half3 surfaceCoords     : TEXCOORD17;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID

};

#ifdef SO_SF_UNITY_TERRAIN_ON

uniform sampler2D _Splat0;
uniform sampler2D _Splat1;
uniform sampler2D _Splat2;
uniform sampler2D _Splat3;

uniform float4 _Splat0_ST;
uniform float4 _Splat1_ST;
uniform float4 _Splat2_ST;
uniform float4 _Splat3_ST;

uniform sampler2D _Normal0;
uniform sampler2D _Normal1;
uniform sampler2D _Normal2;
uniform sampler2D _Normal3;

half _Metallic0;
half _Metallic1;
half _Metallic2;
half _Metallic3;

half _Smoothness0;
half _Smoothness1;
half _Smoothness2;
half _Smoothness3;
#endif


uniform float       _OptionsFold;

uniform float       _LightFold;
uniform float       _LightMode;
uniform float       _LightProbes;

uniform float       _SrcBlend;
uniform float       _DstBlend;

uniform float       _SrcBlendAddPass;
uniform float       _DstBlendAddPass;

uniform float       _ZWrite;
uniform float       _MyCullMode;

uniform float       _SpecularMapType;
uniform float       _AlphaMapType;
uniform float       _SurfaceMapTypeR;
uniform float       _SurfaceMapTypeG;
uniform float       _SurfaceMapTypeB;
uniform float       _SurfaceMapTypeA;

uniform float       _Mode;    			// blend mode
uniform float       _SO_ParticleMode;   // 0 = off
uniform float       _VertexColorMode;   // 0 = off, 1 = color, 2 = deform ?
uniform float       _SpecularHighlights;
uniform float       _SpecularAttenToggle;

uniform float       _ControlScriptFlag;

// echo lighting
uniform half4       _Directional_Position[SO_GD_DIR_COUNT];
uniform SOFLOAT4    _Directional_Color[SO_GD_DIR_COUNT];
uniform half4       _Directional_DistAtten[SO_GD_DIR_COUNT];
uniform half4       _Directional_SpotDir[SO_GD_DIR_COUNT];
uniform half4       _Directional_SpotAtten[SO_GD_DIR_COUNT];

uniform half4       _Point_Position[SO_GD_POINT_COUNT];
uniform SOFLOAT4    _Point_Color[SO_GD_POINT_COUNT];
uniform half4       _Point_DistAtten[SO_GD_POINT_COUNT];
uniform half4       _Point_SpotDir[SO_GD_POINT_COUNT];
uniform half4       _Point_SpotAtten[SO_GD_POINT_COUNT];

uniform half4       _Spot_Position[SO_GD_SPOT_COUNT];
uniform SOFLOAT4    _Spot_Color[SO_GD_SPOT_COUNT];
uniform half4       _Spot_DistAtten[SO_GD_SPOT_COUNT];
uniform half4       _Spot_SpotDir[SO_GD_SPOT_COUNT];
uniform half4       _Spot_SpotAtten[SO_GD_SPOT_COUNT];

uniform float      	_Instancing;

sampler2D _EmissionMap;
half4 _EmissionColor;

//float4 _EmissionColor;
uniform float     	_EmissionFold;
uniform float     	_EmissionToggle;
uniform sampler2D 	_EmissionTexture;
uniform float     	_EmissionScale;
uniform float     	_EmissionAffectObject;

uniform float       _SO_PBRToggle;
uniform float 		_SO_BendToggle;
uniform float4 		_SO_BendPivot;
uniform float 		_SO_BendAmountX;
uniform float 		_SO_BendAmountZ;

uniform SOFLOAT4    _ShaderOneIndirectColor;

uniform float       _SaturationFold;
uniform float       _SaturationToggle;
uniform SOFLOAT     _SaturationStrength;

uniform float       _IntersectToggle;
uniform SOFLOAT4    _IntersectColor;
uniform float       _IntersectThreshold;

// make sure precisions are correct
uniform float       _Layer0Fold;
uniform float       _Layer0Toggle;
uniform float       _Layer0UV2;
uniform sampler2D 	_MainTex;
uniform float4 		_MainTex_ST;
uniform sampler2D 	_BumpMap;
uniform half        _BumpScale;
uniform half        _Layer0AOScale;
uniform float       _Layer0BlendMode;
uniform SOFLOAT    	_Glossiness;
uniform SOFLOAT   	_Metallic;
uniform float       _Parallax;
//uniform SOFLOAT     _Layer0ScaleGloss;
uniform half        _Layer0Fresnel;
uniform sampler2D   _SpecGlossMap;
uniform SOFLOAT4    _SpecColor;
uniform sampler2D   _MetallicGlossMap;
uniform sampler2D 	_Layer0FlowMap;
uniform half      	_Layer0FlowSpeed;
uniform SOFLOAT4    _Layer0FlowColor;
uniform float       _Layer0ScrollU;
uniform float       _Layer0ScrollV;
uniform float       _Layer0Rotation;
uniform float       _Layer0RotationU;
uniform float       _Layer0RotationV;
uniform SOFLOAT4    _Layer0Color;
uniform float       _Layer0AnimType;
uniform float       _Layer0AnimLoopMode;
uniform float       _Layer0ProgressEdge;
uniform SOFLOAT4    _Layer0ProgressColor;
uniform SOFLOAT     _Layer0ProgressColorAmp;
uniform float       _Layer0Progress;
uniform float       _Layer0AnimActive;
uniform float       _Layer0AnimCellsHorz;
uniform float       _Layer0AnimCellsVert;
uniform float       _Layer0AnimFPS;
uniform float       _Layer0AnimCellStart;
uniform float       _Layer0AnimCellEnd;
uniform float       _Layer0AnimOffsetX;
uniform float       _Layer0AnimOffsetY;
uniform float       _Layer0AnimOffsetX2;
uniform float       _Layer0AnimOffsetY2;
uniform float       _Layer0AnimBlend;
uniform half       	_Layer0DistortionStrength;

uniform float       _Layer1Fold;
uniform float       _Layer1Toggle;
uniform float       _Layer1UV2;
uniform sampler2D 	_Layer1Tex;
uniform float4 		_Layer1Tex_ST;
uniform sampler2D   _Layer1BumpMap;
uniform half        _Layer1BumpScale;
uniform half        _Layer1AOScale;
uniform float       _Layer1BlendMode;
uniform SOFLOAT    	_Layer1Smoothness;
uniform SOFLOAT    	_Layer1Metallic;
uniform float       _Layer1Parallax;
uniform half       	_Layer1Fresnel;
//uniform SOFLOAT     _Layer1ScaleGloss;
uniform SOFLOAT4    _Layer1SpecColor;
uniform sampler2D   _Layer1SpecGlossMap;
uniform sampler2D   _Layer1SurfaceMap;
uniform sampler2D 	_Layer1FlowMap;
uniform half      	_Layer1FlowSpeed;
uniform SOFLOAT4    _Layer1FlowColor;
uniform float       _Layer1ScrollU;
uniform float       _Layer1ScrollV;
uniform float       _Layer1Rotation;
uniform float       _Layer1RotationU;
uniform float       _Layer1RotationV;
uniform float4      _Layer1Color;
uniform float       _Layer1AnimType;
uniform float       _Layer1AnimLoopMode;
uniform float       _Layer1ProgressEdge;
uniform SOFLOAT4    _Layer1ProgressColor;
uniform SOFLOAT     _Layer1ProgressColorAmp;
uniform float       _Layer1Progress;
uniform float       _Layer1AnimActive;
uniform float       _Layer1AnimCellsHorz;
uniform float       _Layer1AnimCellsVert;
uniform float       _Layer1AnimFPS;
uniform float       _Layer1AnimCellStart;
uniform float       _Layer1AnimCellEnd;
uniform float       _Layer1AnimOffsetX;
uniform float       _Layer1AnimOffsetY;
uniform float       _Layer1AnimOffsetX2;
uniform float       _Layer1AnimOffsetY2;
uniform float       _Layer1AnimBlend;
uniform half       	_Layer1DistortionStrength;


uniform float       _Layer2Fold;
uniform float       _Layer2Toggle;
uniform float       _Layer2UV2;
uniform sampler2D 	_Layer2Tex;
uniform float4 		_Layer2Tex_ST;
uniform sampler2D   _Layer2BumpMap;
uniform half        _Layer2BumpScale;
uniform half        _Layer2AOScale;
uniform float       _Layer2BlendMode;
uniform SOFLOAT    	_Layer2Smoothness;
uniform SOFLOAT    	_Layer2Metallic;
uniform float       _Layer2Parallax;
//uniform SOFLOAT     _Layer2ScaleGloss;
uniform half       	_Layer2Fresnel;
uniform SOFLOAT4    _Layer2SpecColor;
uniform sampler2D   _Layer2SpecGlossMap;
uniform sampler2D   _Layer2SurfaceMap;
uniform sampler2D 	_Layer2FlowMap;
uniform half      	_Layer2FlowSpeed;
uniform SOFLOAT4    _Layer2FlowColor;
uniform float       _Layer2ScrollU;
uniform float       _Layer2ScrollV;
uniform float       _Layer2Rotation;
uniform float       _Layer2RotationU;
uniform float       _Layer2RotationV;
uniform float4      _Layer2Color;
uniform float       _Layer2AnimType;
uniform float       _Layer2AnimLoopMode;
uniform float       _Layer2ProgressEdge;
uniform SOFLOAT4    _Layer2ProgressColor;
uniform SOFLOAT     _Layer2ProgressColorAmp;
uniform float       _Layer2Progress;
uniform float       _Layer2AnimActive;
uniform float       _Layer2AnimCellsHorz;
uniform float       _Layer2AnimCellsVert;
uniform float       _Layer2AnimFPS;
uniform float       _Layer2AnimCellStart;
uniform float       _Layer2AnimCellEnd;
uniform float       _Layer2AnimOffsetX;
uniform float       _Layer2AnimOffsetY;
uniform float       _Layer2AnimOffsetX2;
uniform float       _Layer2AnimOffsetY2;
uniform float       _Layer2AnimBlend;
uniform half       	_Layer2DistortionStrength;

uniform float       _Layer3Fold;
uniform float       _Layer3Toggle;
uniform float       _Layer3UV2;
uniform sampler2D 	_Layer3Tex;
uniform float4 		_Layer3Tex_ST;
uniform sampler2D   _Layer3BumpMap;
uniform half        _Layer3BumpScale;
uniform half        _Layer3AOScale;
uniform float       _Layer3BlendMode;
uniform SOFLOAT    	_Layer3Smoothness;
uniform SOFLOAT   	_Layer3Metallic;
uniform float       _Layer3Parallax;
//uniform SOFLOAT     _Layer3ScaleGloss;
uniform half       	_Layer3Fresnel;
uniform SOFLOAT4    _Layer3SpecColor;
uniform sampler2D   _Layer3SpecGlossMap;
uniform sampler2D   _Layer3SurfaceMap;
uniform sampler2D 	_Layer3FlowMap;
uniform half      	_Layer3FlowSpeed;
uniform SOFLOAT4    _Layer3FlowColor;
uniform float       _Layer3ScrollU;
uniform float       _Layer3ScrollV;
uniform float       _Layer3Rotation;
uniform float       _Layer3RotationU;
uniform float       _Layer3RotationV;
uniform SOFLOAT4    _Layer3Color;
uniform float       _Layer3AnimType;
uniform float       _Layer3AnimLoopMode;
uniform float       _Layer3ProgressEdge;
uniform SOFLOAT4    _Layer3ProgressColor;
uniform SOFLOAT     _Layer3ProgressColorAmp;
uniform float       _Layer3Progress;
uniform float       _Layer3AnimActive;
uniform float       _Layer3AnimCellsHorz;
uniform float       _Layer3AnimCellsVert;
uniform float       _Layer3AnimFPS;
uniform float       _Layer3AnimCellStart;
uniform float       _Layer3AnimCellEnd;
uniform float       _Layer3AnimOffsetX;
uniform float       _Layer3AnimOffsetY;
uniform float       _Layer3AnimOffsetX2;
uniform float       _Layer3AnimOffsetY2;
uniform float       _Layer3AnimBlend;
uniform half       	_Layer3DistortionStrength;

uniform float       _ManualControl;
uniform float       _ScrollU;
uniform float       _ScrollV;

uniform	float   	_MoveSpeed;
uniform	float   	_MoveStrength;
uniform	float3  	_MoveDirection;

uniform float       _ScanlineFold;
uniform float       _ScanlineToggle;
uniform float       _ScanlineStrengthHorz;
uniform float       _ScanlineScrollHorz;
uniform float       _ScanlineCountHorz;
uniform float       _ScanlineWidthHorz;

uniform float       _ScanlineStrengthVert;
uniform float       _ScanlineScrollVert;
uniform float       _ScanlineCountVert;
uniform float       _ScanlineWidthVert;

uniform float       _GlossyReflections;
uniform sampler2D   _ReflectTex;
uniform float       _ReflectTexFlag;

#if (SHADER_TARGET >= 30)
uniform sampler3D   _FogTexture3D;
#endif

uniform sampler2D 	_Control;   // terrain control texture this is name unity uses
uniform float4 		_Control_ST;
uniform float       _TerrainToggle;

uniform float       _FogFold;
uniform float       _FogToggle;
uniform sampler2D   _FogMap;
uniform half        _FogMapScaleX;
uniform half        _FogMapScaleY;
uniform half        _FogMapScaleZ;
uniform SOFLOAT4 	_FogColor;
uniform SOFLOAT4 	_FogColorFade;
uniform half        _FogAmbient;
uniform half 		_FogDensity;
uniform half 		_FogStartDistance;
uniform half 		_FogEndDistance;
uniform half 		_FogSolidDistance;
uniform half 		_FogHeight;
uniform half 		_FogHeightSize;
uniform half       	_FogMoveX;
uniform half       	_FogMoveY;
uniform half       	_FogMoveZ;
uniform half		_FogScale;
uniform half		_FogVerticalRoughness;

uniform float   	_DistortFold;
uniform float   	_DistortToggle;
uniform half        _DistortHorzCount;
uniform half       	_DistortHorzSpeed;
uniform half       	_DistortHorzStrength;
uniform half       	_DistortHorzWave;

uniform half       	_DistortVertCount;
uniform half       	_DistortVertSpeed;
uniform half       	_DistortVertStrength;
uniform half       	_DistortVertWave;

uniform half       	_DistortCircularCount;
uniform half       	_DistortCircularSpeed;
uniform half       	_DistortCircularStrength;
uniform half       	_DistortCircularCenterU;
uniform half       	_DistortCircularCenterV;

uniform float       _RimFold;
uniform float       _RimToggle;
uniform SOFLOAT4    _RimColor;
uniform SOFLOAT     _RimWidth;
uniform float       _RimBlend;

uniform half       	_RGBOffsetFold;
uniform half       	_RGBOffsetToggle;
uniform half       	_RGBOffsetStrength;
uniform half       	_RGBOffsetAmount;
uniform int         _RGBOffsetMode;

uniform float4      _Random;

uniform SOFLOAT4    _LightColor0;

uniform SOFLOAT     _ColorAmplify;

uniform float       _SO_UV1_WorldMap;
uniform half        _SO_UV1_WorldMapScale;

#ifdef SO_SF_INTERSECT_ON
    uniform sampler2D _CameraDepthTexture;
#endif

struct SOLightingData
{
	SOFLOAT4    finalRGBA;
	half3       positionWorld;

	SOFLOAT     ambientOcclusion;
	SOFLOAT     unlitMask;

    #if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
		SOFLOAT  bakedAtten;
		SOFLOAT  bakedAmount;
	#endif

#ifdef SO_TERRAIN_ON
	SOFLOAT4 	terrainControl;
#endif

#ifdef SO_FOG_LIGHT
	SOFLOAT3 	fogRayLight;
#endif

#ifdef SO_MAP_UNLIT_MASK
	SOFLOAT3 	unlitRGB;
#endif

	SOFLOAT3    lightRGB;

#ifdef SO_DISTORT_ON
	half2 		distortionUV;
#endif

#ifndef SO_ALPHA_MAP_EMPTY
	SOFLOAT		alphaMap;
#endif

#ifdef SO_SF_NORMAL_FIX
	half        facing;
#endif

#if defined (SO_SURFACE_VARS)
	SOFLOAT		metallic;
	SOFLOAT		smoothness;
	SOFLOAT		perceptualRoughness;
	SOFLOAT		roughness;
	SOFLOAT		roughnessX2;
	SOFLOAT3    specularColor;
	SOFLOAT 	reflectivity;
#endif

#ifdef SO_GD_BUMP_SCALE_PER_LAYER
	SOFLOAT     bumpScale;
#endif

#ifdef SO_GD_AO_SCALE_PER_LAYER
	SOFLOAT     aoScale;
#endif

#ifdef SO_GD_FRESNEL_ON
	SOFLOAT 	fresnel;
#endif

#ifdef SO_REFLECT_ON
	SOFLOAT3   	reflection;
#endif

#ifdef SO_SF_SPECULAR_ON
	SOFLOAT3   	specularLight;
#endif

#ifdef SO_BUMP_ON
	half4 		normalMap;
#endif

#ifdef SO_WORLDNORMAL_ON
	half3 		worldNormal;
#endif

#ifdef SO_VIEWDIR_ON
	half3   	viewDir;
#endif
};

struct SOLight
{
	half3		direction;
	SOFLOAT  	attenuation;

	#ifdef SO_FOG_LIGHT
	SOFLOAT	fogAttenuation;
	#endif

	half        dotNL;

#ifdef SO_HALFDIR_ON
	half3       halfDir;
#endif

#ifdef SO_DOTNH_ON
	half        dotNH;
#endif

#ifdef SO_DOTLH_ON
	half        dotLH;
#endif

	SOFLOAT3  	color;
};

struct SOLayer
{
	half2 		uv;
	half2 		buv;
	SOFLOAT4    finalRGBA;

	SOFLOAT     progress;

//#ifdef	SO_PARALLAX_ON
	SOFLOAT parallaxHeight;
//#endif

	SOFLOAT     ambientOcclusion;
	SOFLOAT     unlitMask;
//    SOFLOAT3    specular;

#if defined (SO_SURFACE_VARS)
	SOFLOAT		metallic;
	SOFLOAT		roughness;
	SOFLOAT		smoothness;
	SOFLOAT3	specularColor;
#endif

#ifdef SO_GD_BUMP_SCALE_PER_LAYER
	SOFLOAT     bumpScale;
#endif

#ifdef SO_GD_AO_SCALE_PER_LAYER
	SOFLOAT     aoScale;
#endif

#ifdef SO_FLOWMAP_ON
	SOFLOAT   	flowLerp;
	half2     	flowUV0;
	half2     	flowUV1;
#endif

};
