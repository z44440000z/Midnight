// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

#define PI 3.14159
#define PI2 6.28318

//-------------------------------------------------------------------------------------
inline SOFLOAT GetMaxColor ( SOFLOAT3 i_color )
{
    return ( max ( max ( i_color.r, i_color.g ), i_color.b ) );
}

//-------------------------------------------------------------------------------------
inline void AmbientOcclusion ( inout SOLightingData i_sold, inout SOFLOAT3 i_val )
{
#if defined (SO_SURFACE_MAP_ON) && defined (SO_SURFACE_MAP_AMBIENT_OCCLUSION)
	i_val *= i_sold.ambientOcclusion;
#endif

#if defined (SO_ALPHA_MAP_AMBIENT_OCCLUSION)
	i_val *= i_sold.alphaMap;
#endif
}

//-------------------------------------------------------------------------------------
// return U or V distorted coords
inline half CalcDistortion ( half i_uorv, half i_waveCount, half i_speed, half i_strength, half i_lerpWave )
{
#ifdef SO_DISTORT_ON
	half sineWave;
	half finalWave;

	sineWave = sin ( ( i_waveCount * i_uorv - ( i_speed * _Time.y ) ) );
	finalWave = lerp ( sineWave, sign ( sineWave ), i_lerpWave ) * i_strength;

	return ( finalWave );
#else
	return(0);
#endif
}

//-------------------------------------------------------------------------------------
inline void DistortionHorz ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_SF_DISTORT_HORZ_ON
	i_sold.distortionUV.x += CalcDistortion ( i_v2f.normUV.y, _DistortHorzCount, _DistortHorzSpeed, _DistortHorzStrength, _DistortHorzWave );
#endif
}

//-------------------------------------------------------------------------------------
inline void DistortionVert ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_SF_DISTORT_VERT_ON
	i_sold.distortionUV.y += CalcDistortion ( i_v2f.normUV.x, _DistortVertCount, _DistortVertSpeed, _DistortVertStrength, _DistortVertWave );
#endif
}

//-------------------------------------------------------------------------------------
inline void DistortionCircular ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_SF_DISTORT_CIRCULAR_ON
	half dist = 1.0 - distance ( i_v2f.normUV, half2(_DistortCircularCenterU,_DistortCircularCenterV) );

	half offset = cos ( length ( dist ) * _DistortCircularCount - _DistortCircularSpeed * _Time.y ) *_DistortCircularStrength;

	i_sold.distortionUV.xy += half2 ( offset, offset );
#endif
}

//-------------------------------------------------------------------------------------
inline void DistortionApplyToLayerUV ( inout SOLightingData i_sold, inout SOLayer i_layer, in half i_strength )
{
#ifdef SO_DISTORT_ON
	i_layer.uv += i_sold.distortionUV * i_strength;
#endif
}

//-------------------------------------------------------------------------------------
inline void SetupShaderOneData ( inout SOLightingData i_sold, in v2f i_v2f )
{
	UNITY_INITIALIZE_OUTPUT ( SOLightingData, i_sold );

#ifdef SO_DISTORT_ON
	i_sold.distortionUV.xy = half2(0,0);
	DistortionHorz ( i_sold, i_v2f );
	DistortionVert ( i_sold, i_v2f );
	DistortionCircular ( i_sold, i_v2f );
#endif

#ifdef SO_POSWORLD_VARY_ON
		i_sold.positionWorld = i_v2f.positionWorld;
#endif

#ifdef SO_VIEWDIR_ON
		i_sold.viewDir = normalize( _WorldSpaceCameraPos.xyz - i_sold.positionWorld );
#endif

#ifdef SO_WORLDNORMAL_ON
	#ifdef SO_WORLDNORMAL_VARY_ON
		i_sold.worldNormal = normalize ( i_v2f.normalDir );
	#endif
#endif

#ifdef SO_BUMP_ON
	i_sold.worldNormal.x = i_v2f.tspace0.z;
	i_sold.worldNormal.y = i_v2f.tspace1.z;
	i_sold.worldNormal.z = i_v2f.tspace2.z;

	i_sold.worldNormal = normalize ( i_sold.worldNormal );
#endif

}

//-------------------------------------------------------------------------------------
inline void Flowmap2DInit ( inout SOLayer i_layer, sampler2D i_flowMap, half i_flowSpeed )
{
#ifdef SO_FLOWMAP_ON
	half4 flowDir = tex2D ( i_flowMap, i_layer.uv ).rgba;

	flowDir.xy = ( flowDir.xy * 2.0 - 1.0 ) * half2 ( i_flowSpeed, i_flowSpeed );

	half flowPhase0 = frac ( _Time.y * 0.5f + 0.5f );
	half flowPhase1 = frac ( _Time.y * 0.5f + 1.0f );

	i_layer.flowLerp = abs ( ( 0.5f - flowPhase0 ) / 0.5f );

	i_layer.flowUV0 = i_layer.uv + flowDir.xy * flowPhase0;
	i_layer.flowUV1 = i_layer.uv + flowDir.xy * flowPhase1;
#endif
}

//-------------------------------------------------------------------------------------
inline half4 Flowmap2DBump ( inout SOLayer i_layer, sampler2D i_normalTex )
{
	half4 normalMap;
#ifdef SO_BUMP_ON
#ifdef SO_FLOWMAP_ON
    half4 tex0 	= tex2D ( i_normalTex, i_layer.flowUV0 );
    half4 tex1 	= tex2D ( i_normalTex, i_layer.flowUV1 );

	normalMap 		= lerp ( tex0, tex1, i_layer.flowLerp );
#else
	normalMap 		= tex2D ( i_normalTex, i_layer.uv );
#endif
#endif
	return ( normalMap );
}

//-------------------------------------------------------------------------------------
inline void Flowmap2DTex ( inout SOLayer i_layer, sampler2D i_tex, SOFLOAT4 i_flowColor )
{
#ifdef SO_FLOWMAP_ON
    SOFLOAT4 tex0 = tex2D ( i_tex, i_layer.flowUV0 );
    SOFLOAT4 tex1 = tex2D ( i_tex, i_layer.flowUV1 );

	i_layer.finalRGBA 		= lerp ( tex0, tex1, i_layer.flowLerp );
//	i_layer.finalRGBA.rgb 	+= i_flowColor.aaa * i_flowColor.rgb;
#endif
}

//-------------------------------------------------------------------------------------
inline void CalcVertBumpMap ( in float3 i_worldNormal, in float4 i_worldTangent, in float3 i_binormal, out half3 o_tspace0, out half3 o_tspace1, out half3 o_tspace2 )
{
#ifdef SO_BUMP_ON
	i_binormal *= unity_WorldTransformParams.w;

	o_tspace0 = half3 ( i_worldTangent.x, i_binormal.x, i_worldNormal.x );
	o_tspace1 = half3 ( i_worldTangent.y, i_binormal.y, i_worldNormal.y );
	o_tspace2 = half3 ( i_worldTangent.z, i_binormal.z, i_worldNormal.z );
#endif
}

//-------------------------------------------------------------------------------------
inline half3 CalcVertParallax ( float3 i_vertex, float3 i_normal, float4 i_worldTangent, float3 i_binormal )
{
	float3 viewDirInObjectCoords = mul ( unity_WorldToObject, float4 ( _WorldSpaceCameraPos, 1.0 ) ).xyz - i_vertex;

	float3x3 localSurface2ScaledObjectT = float3x3( i_worldTangent.xyz, i_binormal, i_normal );

	return ( ( mul ( localSurface2ScaledObjectT, viewDirInObjectCoords ) ) );
}

//-------------------------------------------------------------------------------------
inline void CalcFragParallax ( inout SOLayer i_layer, in half3 i_surfaceCoords, half i_mapHeight, half i_userHeight )
{
#ifdef SO_PARALLAX_ON
	half parallaxHeight = i_mapHeight - 0.5;
	half3 v 			= normalize ( i_surfaceCoords );
	v.z 				+= 0.42;
	half2 offset 		= i_userHeight * parallaxHeight * v.xy / v.z;
	i_layer.uv    		+= offset;
#endif
}

//-------------------------------------------------------------------------------------
inline void ChromaticAbberation ( inout SOLightingData i_sold, inout SOLayer i_layer, sampler2D i_tex )
{
#ifdef SO_SF_RGBOFFSET_ON
	half offset;
	SOFLOAT3 rgb;

	SOFLOAT strength = _RGBOffsetStrength;

	offset = _RGBOffsetAmount;

	if ( _RGBOffsetMode < 0.5 )
		offset *= sqrt ( pow ( i_layer.uv.x - 0.5, 2.0 ) + pow ( i_layer.uv.y - 0.5, 2.0 ) );

	half2 rgbOffsetUV1 = half2 ( i_layer.uv.x - offset, i_layer.uv.y );
	half2 rgbOffsetUV2 = half2 ( i_layer.uv.x + offset, i_layer.uv.y );
	half2 rgbOffsetUV3 = half2 ( i_layer.uv.x, i_layer.uv.y - offset );

#ifdef SO_BLEND_TRANSPARENT
		SOFLOAT2 r,g,b;

		r = tex2D ( i_tex, rgbOffsetUV1 ).ra;
		g = tex2D ( i_tex, rgbOffsetUV2 ).ga;
		b = tex2D ( i_tex, rgbOffsetUV3 ).ba;

		rgb = SOFLOAT3 ( r.x, g.x, b.x );

		SOFLOAT alpha 		= ( ( r.x * r.y ) + ( g.x * g.y ) + ( b.x * b.y ) ) * 0.3 * strength;
		i_layer.finalRGBA.a = clamp ( alpha + i_sold.alpha, 0.0, 1.0 );
#else
	SOFLOAT r,g,b;

	r = tex2D ( i_tex, rgbOffsetUV1 ).r;
	g = tex2D ( i_tex, rgbOffsetUV2 ).g;
	b = tex2D ( i_tex, rgbOffsetUV3 ).b;

	rgb = SOFLOAT3 ( r, g, b ) * 0.33333;
#endif

	i_layer.finalRGBA.rgb = lerp ( i_layer.finalRGBA.rgb, rgb, strength );

#endif
}

//-------------------------------------------------------------------------------------
// = return
inline void Saturation ( inout SOLightingData i_sold )
{
	SOFLOAT strength = _SaturationStrength;
	SOFLOAT avgColor = ( i_sold.finalRGBA.r + i_sold.finalRGBA.g + i_sold.finalRGBA.b ) * 0.333333;

	i_sold.finalRGBA.rgb =  lerp ( SOFLOAT3 ( avgColor, avgColor, avgColor ), i_sold.finalRGBA.rgb, strength );
}

//-------------------------------------------------------------------------------------
// = return
inline SOFLOAT CalcScanlines ( half i_normUorV, half i_count, half i_width, half i_scroll, in SOFLOAT i_strength )
{
	SOFLOAT scanCurve = clamp ( sin ( ( _Time.w * i_scroll ) + ( i_normUorV * PI2 * i_count ) ) + i_width, 0, 1 );

	scanCurve = 1.0 - scanCurve * i_strength;

	return ( scanCurve );
}

//-------------------------------------------------------------------------------------
//SCANLINES
inline void Scanlines ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_SF_SCANLINE_ON
	SOFLOAT strength;
	SOFLOAT amount;

	// HORZ
	strength = _ScanlineStrengthHorz;

	i_sold.finalRGBA *= CalcScanlines ( i_v2f.normUV.y, _ScanlineCountHorz, _ScanlineWidthHorz, _ScanlineScrollHorz, strength );

	strength = _ScanlineStrengthVert;

	i_sold.finalRGBA *= CalcScanlines ( i_v2f.normUV.x, _ScanlineCountVert, _ScanlineWidthVert, _ScanlineScrollVert, strength );

#endif

}

//-------------------------------------------------------------------------------------
// add return
inline void Intersection ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_SF_INTERSECT_ON
	/*
	float screenDepth = DecodeFloatRG(tex2D(_CameraDepthTexture, i_v2f.screenPos.xy).zw);
	float diff = screenDepth - i_v2f.screenPos.z;
	float intersect = 0;

	diff = 1 - smoothstep ( 0, _ProjectionParams.w, clamp ( diff, 0, 1 ) );

	i_sold.finalRGBA = lerp ( i_sold.finalRGBA, _IntersectColor, diff );
*/

	SOFLOAT sceneZ = LinearEyeDepth ( SAMPLE_DEPTH_TEXTURE_PROJ ( _CameraDepthTexture, UNITY_PROJ_COORD ( i_v2f.screenPos ) ).r );
	SOFLOAT diff = length ( sceneZ - i_v2f.screenPos.w );


	diff = 1.0 - saturate ( diff / _IntersectThreshold );

	diff *= diff;

	i_sold.finalRGBA = lerp ( i_sold.finalRGBA, _IntersectColor, diff );

#endif
}

//-------------------------------------------------------------------------------------
// Rember to put in docs ... progress needs to be uncompressed and mipmaps off for best look
inline void CalcProgress (  inout SOLightingData i_sold, inout SOLayer i_layer, in half i_progress, in SOFLOAT4 i_progressColor, in SOFLOAT i_amplify, in half i_edgeWidth, const int i_index )
{
	SOFLOAT3 edgeColor;
	SOFLOAT d = i_layer.progress;
	SOFLOAT4 outRGB;

	SOFLOAT edge 	= lerp ( d + i_edgeWidth, d - i_edgeWidth, d );
	SOFLOAT alpha 	= smoothstep(  i_progress + i_edgeWidth, i_progress - i_edgeWidth, edge );

	i_layer.finalRGBA.rgb = lerp ( i_progressColor.rgb * i_amplify, i_layer.finalRGBA.rgb, alpha );

#if defined (SO_BLEND_CUTOUT)
    clip ( alpha - 0.5 );
#endif

#ifdef SO_PREMULTIPLY_ALPHA
	i_layer.finalRGBA.rgb *= alpha;

	#ifndef SO_ALPHA_MAP_EMPTY
	i_layer.finalRGBA.a *= alpha;
	#else
	i_layer.finalRGBA.a = lerp ( 0, i_layer.finalRGBA.a, alpha );
	#endif
#endif

}

//-------------------------------------------------------------------------------------
inline void RimLighting ( inout SOLightingData i_sold )
{
#ifdef SO_RIMLIT_ON
	SOFLOAT alpha = _RimColor.a;

#if defined (SO_PREMULTIPLY_ALPHA)
	alpha *= i_sold.finalRGBA.a;
#endif

	SOFLOAT f = pow ( 1.0 - abs ( dot( i_sold.worldNormal, i_sold.viewDir ) ), 10-_RimWidth );

#ifdef SO_SF_RIMLIT_ADD
	i_sold.finalRGBA.rgb += _RimColor.rgb * f;
#endif

#ifdef SO_SF_RIMLIT_SUBTRACT
	i_sold.finalRGBA.rgb -= _RimColor.rgb * f;
#endif

#endif
}

#ifdef SO_LIGHTING_ON
//-------------------------------------------------------------------------------------
inline SOFLOAT3 FresnelReflection ( inout SOLightingData i_sold )
{
	///SCOTTFIND
#ifdef SO_GD_FRESNEL_ON
	half3 dotNV = 1.0 - abs ( dot ( i_sold.worldNormal , i_sold.viewDir ) );
	half f = 0;

	#ifdef SO_GD_FRESNEL_POW3
	f = dotNV * dotNV * dotNV;
	#endif

	#ifdef SO_GD_FRESNEL_POW4
	f = dotNV * dotNV * dotNV * dotNV;
	#endif

	#ifdef SO_GD_FRESNEL_POW5
	f = dotNV * dotNV * dotNV * dotNV * dotNV;
	#endif

	f *= i_sold.fresnel;

   	SOFLOAT grazing = saturate ( ( 1.0 - i_sold.roughness ) + i_sold.reflectivity );

	SOFLOAT3 result = lerp ( i_sold.specularColor, grazing, f );

	return ( result );
#else
	return ( SOFLOAT3 ( 1, 1, 1 ) );
#endif
}

//-------------------------------------------------------------------------------------
inline SOFLOAT3 FresnelSpecular ( inout SOLightingData i_sold, in SOLight i_sol )
{
#ifdef SO_SURFACE_VARS
#ifdef SO_GD_FRESNEL_ON
#ifdef SO_SF_SPECULAR_ON
	half dotLH = 1.0 - i_sol.dotLH;
	half f = 0;

	#ifdef SO_GD_FRESNEL_POW3
	f = dotLH * dotLH * dotLH;
	#endif

	#ifdef SO_GD_FRESNEL_POW4
	f = dotLH * dotLH * dotLH * dotLH;
	#endif

	#ifdef SO_GD_FRESNEL_POW5
	f = dotLH * dotLH * dotLH * dotLH * dotLH;
	#endif

	return ( i_sold.specularColor + ( 1.0 - i_sold.specularColor ) * f );

#endif
#endif
	return ( i_sold.specularColor );
#endif
}

#ifdef SO_SF_SPECULAR_ON

#ifdef SO_GD_SPECULAR_FASTEST
sampler2D_float unity_NHxRoughness;
//-------------------------------------------------------------------------------------
inline SOFLOAT SpecularFastest ( inout SOLightingData i_sold, in SOLight i_sol )
{
	SOFLOAT specTerm;

	specTerm = tex2D ( unity_NHxRoughness, half2( i_sol.dotNH, i_sold.perceptualRoughness ) ).r * 16;

	return ( specTerm );
}
#endif


//-------------------------------------------------------------------------------------
inline SOFLOAT SpecularFast ( inout SOLightingData i_sold, in SOLight i_sol )
{
	SOFLOAT specTerm;

	half denom 	= i_sol.dotNH * i_sol.dotNH * ( i_sold.roughnessX2 - 1.0h ) + 1.00001h;
	specTerm 	= ( i_sold.roughnessX2 / (denom * denom) ) * 0.3;

#if defined (SHADER_API_MOBILE)
	specTerm = clamp ( specTerm, 0.0, 100 );
#endif

	return ( specTerm );
}

//-------------------------------------------------------------------------------------
inline SOFLOAT SpecularStandard ( inout SOLightingData i_sold, in SOLight i_sol )
{
#ifdef SO_GD_SPECULAR_NORMAL
	SOFLOAT	specTerm;

	half d = i_sol.dotNH * i_sol.dotNH * ( i_sold.roughnessX2 - 1.0h) + 1.00001h;

	#ifdef UNITY_COLORSPACE_GAMMA
	specTerm = i_sold.roughness / ( max ( 0.32h, i_sol.dotLH ) * ( 1.5h + i_sold.roughness ) * d  );
	#else
	specTerm = i_sold.roughnessX2 / ( ( d * d ) * max ( 0.1h, i_sol.dotLH * i_sol.dotLH ) * ( i_sold.roughness * 4.0h + 2.0h ) );
	#endif

#if defined (SHADER_API_MOBILE)
	specTerm = clamp ( specTerm, 0.0, 100 );
#endif

	return ( specTerm );
#else
	return (0);
#endif
}

//-------------------------------------------------------------------------------------
inline SOFLOAT SpecularHQ ( inout SOLightingData i_sold, in SOLight i_sol )
{
#ifdef SO_GD_SPECULAR_HQ
	half denom 	= i_sol.dotNH * i_sol.dotNH * ( i_sold.roughnessX2 - 1.0 ) + 1.0f;
	half D 		= i_sold.roughnessX2 / ( 3.14159 * denom * denom );

	half k 		= i_sold.roughness/2.0f;
	half k2 	= k * k;
	half invK2 	= 1.0f-k2;

#if (SHADER_TARGET >= 50)
	half vis	= rcp ( i_sol.dotLH * i_sol.dotLH * invK2 + k2 );
#else
	half vis	= rsqrt ( i_sol.dotLH * i_sol.dotLH * invK2 + k2 );
#endif

	SOFLOAT specTerm = vis * D;

#if defined (SHADER_API_MOBILE)
	specTerm = specTerm - 1e-4h;
	specTerm = clamp ( specTerm, 0.0, 100 );
#endif

	return ( specTerm );
#else
	return (0);
#endif

}

#endif


//-------------------------------------------------------------------------------------
inline void Specular ( inout SOLightingData i_sold, in SOLight i_sol )
{
#ifdef SO_SF_SPECULAR_ON
	#ifdef SO_GD_SPECULAR_FASTEST
	i_sold.specularLight += FresnelSpecular ( i_sold, i_sol )  * i_sol.color * i_sold.specularColor * SpecularFastest ( i_sold, i_sol );
	#endif

	#ifdef SO_GD_SPECULAR_FAST
	i_sold.specularLight += FresnelSpecular ( i_sold, i_sol )  * i_sol.color * i_sold.specularColor * SpecularFast ( i_sold, i_sol );
	#endif

	#ifdef SO_GD_SPECULAR_NORMAL
	i_sold.specularLight +=  FresnelSpecular ( i_sold, i_sol ) * i_sol.color * i_sold.specularColor * SpecularStandard ( i_sold, i_sol ) ;
	#endif

	#ifdef SO_GD_SPECULAR_HQ
	i_sold.specularLight +=   FresnelSpecular ( i_sold, i_sol ) * i_sol.color * i_sold.specularColor * SpecularHQ ( i_sold, i_sol );
	#endif
#endif
}

#endif

//-------------------------------------------------------------------------------------
inline void ReflectionGet ( inout SOLightingData i_sold )
{
#if defined(SO_GD_BASE_PASS)

#ifdef SO_REFLECT_ON
	half3 worldRefl = normalize ( reflect ( -i_sold.viewDir, i_sold.worldNormal ) );

#ifdef SO_PBR_LIGHTING
	half mipmap 	= ( i_sold.perceptualRoughness * ( 1.7 - 0.7 * i_sold.perceptualRoughness ) )* UNITY_SPECCUBE_LOD_STEPS;
#else
	half mipmap 	= i_sold.perceptualRoughness * UNITY_SPECCUBE_LOD_STEPS;
#endif

#ifdef SO_SF_REFLECT_PROBE_ON

	#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    	half3 cubeRefl 	= BoxProjectedCubemapDirectionFast ( worldRefl, i_sold.positionWorld, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax );
    #else
		half3 cubeRefl  = worldRefl;
    #endif

		half4 probe1  	= UNITY_SAMPLE_TEXCUBE_LOD ( unity_SpecCube0, cubeRefl, mipmap );

	#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    	if ( unity_SpecCube0_BoxMin.w < 1.0 )
    	{
    		cubeRefl 		= BoxProjectedCubemapDirectionFast ( worldRefl, i_sold.positionWorld, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax );
    		SOFLOAT4 probe2	= UNITY_SAMPLE_TEXCUBE_SAMPLER_LOD ( unity_SpecCube1, unity_SpecCube0, cubeRefl, mipmap );
    		probe1 			= lerp ( probe2, probe1, unity_SpecCube0_BoxMin.w );
    	}
	#endif

	#ifdef SHADER_API_MOBILE
		i_sold.reflection = probe1.rgb;
	#else
		i_sold.reflection = DecodeHDR ( probe1, unity_SpecCube0_HDR );
	#endif

#elif defined (SO_SF_REFLECT_2D_ON)
		half2 reflectUV;

		half num = ( sqrt ( worldRefl.x * worldRefl.x + worldRefl.y * worldRefl.y + ( worldRefl.z + 1 ) * ( worldRefl.z + 1 ) ) * 2 );

		reflectUV.x = ( worldRefl.x / num ) + 0.5;
		reflectUV.y = ( worldRefl.y / num ) + 0.5;

		float4 s = float4(0,0,0,0);

		s.xy 	= reflectUV;
		s.w 	= mipmap;

		i_sold.reflection = tex2Dlod ( _ReflectTex, s ).rgb;
#else
    	#ifdef SO_GD_PIPELINE_SHADER_ONE
    	i_sold.reflection = _ShaderOneIndirectColor.rgb;
    	#else
		i_sold.reflection = unity_IndirectSpecColor.rgb;
    	#endif
#endif
#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void UnlitMask ( inout SOLightingData i_sold )
{
#ifdef SO_LIGHTING_ON
#ifdef SO_MAP_UNLIT_MASK
	GetAlphaMap_UNLIT_MASK ( i_sold, i_sold.unlitMask );

	i_sold.lightRGB = lerp ( i_sold.lightRGB, SOFLOAT3(1,1,1), i_sold.unlitMask );
#endif
#endif
}

//-------------------------------------------------------------------------------------
inline half2 CalcLayerUVScrollAuto ( in float i_scrollU,  in float i_scrollV )
{
	half2 uv;

	uv = half2 ( i_scrollU * _Time.x, i_scrollV * _Time.x );

	return ( uv );
}

//-------------------------------------------------------------------------------------
inline void WindMovement( inout float4 i_worldPos, in float4 i_vertexStrength )
{
	float speed;
	float moveAmt;
	float moveSin;
	float moveCos;

	speed = _Time.y * _MoveSpeed;

	sincos ( speed, moveSin, moveCos );

	moveAmt = ( moveSin + moveCos ) * _MoveStrength;

	i_worldPos.xyz += float3 ( moveAmt * _MoveDirection.x * i_vertexStrength.x, moveAmt * _MoveDirection.y * i_vertexStrength.y, moveAmt * _MoveDirection.z * i_vertexStrength.z );
}

/*

#define SIDE_TO_SIDE_FREQ1 1.975
#define SIDE_TO_SIDE_FREQ2 0.793
#define UP_AND_DOWN_FREQ1 0.375
#define UP_AND_DOWN_FREQ2 0.193

float4 SmoothCurve( float4 x ) {
    return x * x *( 3.0 - 2.0 * x );
}
float4 TriangleWave( float4 x ) {
    return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
}
float4 SmoothTriangleWave( float4 x ) {
    return SmoothCurve( TriangleWave( x ) );
}


    inline float4 AnimateVertex(float4 pos, float3 normal, float4 animParams,float4 wind,float2 time)
    {
        // animParams stored in color
        // animParams.x = branch phase
        // animParams.y = edge flutter factor
        // animParams.z = primary factor
        // animParams.w = secondary factor

        float fDetailAmp = 0.1f;
        float fBranchAmp = 0.3f;

        // Phases (object, vertex, branch)
        float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
        float fBranchPhase = fObjPhase + animParams.x;

        float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);

        // x is used for edges; y is used for branches
        float2 vWavesIn = time  + float2(fVtxPhase, fBranchPhase );

        // 1.975, 0.793, 0.375, 0.193 are good frequencies
        float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);

        vWaves = SmoothTriangleWave( vWaves );
        float2 vWavesSum = vWaves.xz + vWaves.yw;

        // Edge (xz) and branch bending (y)
        float3 bend = animParams.y * fDetailAmp * normal.xyz;
        bend.y = animParams.w * fBranchAmp;
        pos.xyz += ((vWavesSum.xyx * bend) + (wind.xyz * vWavesSum.y * animParams.w)) * wind.w;

        // Primary bending
        // Displace position
        pos.xyz += animParams.z * wind.xyz;

        return pos;
    }

inline float4 ApplyDetailBending(
	float4 i_worldPos,		// The final world position of the vertex being modified
	float3 i_worldNormal,			// The world normal for this vertex
	float fDetailPhase,		// Optional phase for side-to-side. This is used to vary the phase for side-to-side motion
	float fBranchPhase,		// The green vertex channel per Crytek's convention
	float fEdgeAtten,		// "Leaf stiffness", red vertex channel per Crytek's convention
	float fBranchAtten,		// "Overall stiffness", *inverse* of blue channel per Crytek's convention
	float fBranchAmp,		// Controls how much up and down
	float fSpeed,			// Controls how quickly the leaf oscillates
	float fDetailFreq,		// Same thing as fSpeed (they could really be combined, but I suspect
							// this could be used to let you additionally control the speed per vertex).
	float fDetailAmp)		// Controls how much back and forth
{
	float fObjPhase = dot( unity_ObjectToWorld[3], 1);

	fBranchPhase += fObjPhase;

	float fVtxPhase = dot(i_worldPos.xyz, fDetailPhase + fBranchPhase );

	float2 vWavesIn = _Time.y + float2(fVtxPhase, fBranchPhase );
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0 );// * fSpeed * fDetailFreq;
	vWaves = SmoothCurve( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;

    i_worldPos.xyz += vWavesSum.x * float3 ( fEdgeAtten * fDetailAmp * i_worldNormal.xyz);
    i_worldPos.y += vWavesSum.y * fBranchAtten * fBranchAmp;

	return(i_worldPos);
}
*/

//-------------------------------------------------------------------------------------
inline half2 CalcLayerUVRotation ( in float2 iuv,in float4 itexST, in float i_angle, in float irotationU, in float irotationV )
{
	float2x2 	rotationMatrix;
	float 		sinX;
	float 		cosX;

	sincos ( i_angle, sinX, cosX );
	rotationMatrix = float2x2 ( cosX, -sinX, sinX, cosX );

	iuv -= float2 ( irotationU, irotationV );

    iuv = mul ( iuv, rotationMatrix );

    iuv += float2 ( irotationU, irotationV );

	return ( iuv );
}

//-------------------------------------------------------------------------------------
inline half2 CalcLayerUVRotationAuto ( in float2 iuv,in float4 itexST, in float irotationSpeed, in float irotationU, in float irotationV )
{
	float angle;

	angle = irotationSpeed * _Time.y;

	iuv = CalcLayerUVRotation ( iuv, itexST, angle, irotationU, irotationV );

	return ( iuv );
}

//-------------------------------------------------------------------------------------
inline float2 CellAnimUV ( half2 iuv, half icellsHorz, int icellsVert, half ioffsetX, half ioffsetY, half4 i_st )
{
	iuv.x = ( iuv.x / icellsHorz ) + ( ioffsetX / i_st.x ) ;
	iuv.y = ( iuv.y / icellsVert ) + ( ioffsetY / i_st.y ) ;

	return iuv;
}

//-------------------------------------------------------------------------------------
inline void FogApplyVolumetric3D ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_GD_BASE_PASS
#ifdef SO_SF_FOG_ON
#ifdef SO_GD_FOG_VOLUMETRIC_3D
	half  		dist;
	half		distFade;
	half    	stepSize;
	half3 		uvAdd;
	half 		fogAmount;
	half3 		fogUVOffset;
	half    	fogValue;
	half 		fogYLength;
	half    	heightIntensity;
	half3    	rayPos;
	half3    	rayAdd;
	SOFLOAT3 	fogColor;
	half        fogDensity = _FogDensity / SO_GD_FOG_RAYCOUNT;

	fogUVOffset.x = _Time.y * _FogMoveX;
	fogUVOffset.y = _Time.y * _FogMoveY;
	fogUVOffset.z = _Time.y * _FogMoveZ;

	half3 viewDir   = normalize ( i.positionWorld-_WorldSpaceCameraPos.xyz );
	dist 	 		= length ( _WorldSpaceCameraPos.xyz - i.positionWorld );
	distFade 		= 1.0 - clamp ( ( _FogEndDistance - dist ) / _FogSolidDistance, 0.0, 1.0 );
	stepSize 		= dist / (float)SO_GD_FOG_RAYCOUNT;
	rayPos 			= _WorldSpaceCameraPos.xyz;
    rayAdd 			= viewDir * stepSize;
	fogAmount 		= 0;

	for ( int i = 0; i < SO_GD_FOG_RAYCOUNT; i++ )
	{
		rayPos 		+= viewDir * stepSize;

		fogValue 	= tex3D ( _FogTexture3D, ( rayPos * _FogScale ) + fogUVOffset ).a;
		//fogValue = 1;

	#ifdef SO_GD_FOG_ROUGHNESS_ON
		fogYLength = length ( rayPos.y - ( _FogHeight + ( ( fogValue - 0.5 ) * _FogVerticalRoughness ) ) );
//		fogYLength = length ( _FogHeight - ( rayPos.y + ( ( fogValue - 0.5 ) * _FogVerticalRoughness ) ) );
	#else
		fogYLength 	= length ( rayPos.y - _FogHeight );
	#endif

		heightIntensity  = 1.0 - min ( ( fogYLength / _FogHeightSize ), 1.0 );

        fogAmount 	+= fogValue * heightIntensity;
	}

	fogAmount = ( fogAmount / SO_GD_FOG_RAYCOUNT ) * _FogDensity;

	fogAmount = 1.0 - exp ( -dist * fogAmount );

#ifdef SO_LIGHTING_ON

	#ifdef SO_GD_FOG_IMAGELIGHT_ON
	#ifdef SO_LIGHTING_UNLIT
	fogColor = _FogColor * ( i_sold.fogRayLight + i_sold.finalRGBA.rgb + _FogAmbient );
	#else
	fogColor = _FogColor * ( i_sold.fogRayLight + max ( i_sold.finalRGBA.rgb - half3(1.0,1.0,1.0), 0 ) + _FogAmbient );
	#endif
	#else
	fogColor = _FogColor * ( i_sold.fogRayLight + _FogAmbient );
	#endif

#else
	#ifdef SO_GD_FOG_IMAGELIGHT_ON
	fogColor = ( _FogColor * ( i_sold.finalRGBA.rgb + SOFLOAT3(1,1,1) ) ) + _FogAmbient;
	#else
	fogColor = _FogColor + _FogAmbient;
	#endif
#endif

#ifdef SO_PREMULTIPLY_ALPHA
		i_sold.finalRGBA = lerp ( i_sold.finalRGBA, SOFLOAT4(0,0,0,0), fogAmount );
#else
		i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, fogColor, fogAmount );
#endif

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColorFade, distFade );

#endif
#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void FogApplyVolumetric ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_GD_BASE_PASS
#ifdef SO_SF_FOG_ON
#ifdef SO_GD_FOG_VOLUMETRIC
	half  		dist 	 		= length ( _WorldSpaceCameraPos.xyz - i.positionWorld );
	half 		distFade 		= 1.0 - clamp ( ( _FogEndDistance - dist ) / _FogSolidDistance, 0.0, 1.0 );
	half 		fogAmount;
	half    	fogHeightIntensity;
	SOFLOAT3	fogColor;

	fogHeightIntensity = ( 1.0 - min ( ( length (  i.positionWorld.y - _FogHeight ) / _FogHeightSize ), 1.0 ) );

	/// can smooth out fog a little
	//fogHeightIntensity = sin ( fogHeightIntensity * ( PI / 2.0 ) );

	fogAmount = 1.0 - exp ( -dist * ( fogHeightIntensity * _FogDensity ) );

#ifdef SO_LIGHTING_ON
	#ifdef SO_GD_FOG_IMAGELIGHT_ON
	#ifdef SO_LIGHTING_UNLIT
	fogColor = _FogColor * ( i_sold.fogRayLight + i_sold.finalRGBA.rgb + _FogAmbient );
	#else
	fogColor = _FogColor * ( i_sold.fogRayLight + max ( i_sold.finalRGBA.rgb - half3(1.0,1.0,1.0), 0 ) + _FogAmbient );
	#endif
	#else
	fogColor = _FogColor * ( i_sold.fogRayLight + _FogAmbient );
	#endif
#else
	#ifdef SO_GD_FOG_IMAGELIGHT_ON
	fogColor = ( _FogColor * ( i_sold.finalRGBA.rgb + SOFLOAT3(1,1,1) ) ) + _FogAmbient;
	#else
	fogColor = _FogColor + _FogAmbient;
	#endif
#endif

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, fogColor, fogAmount );
	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColorFade, distFade );
#endif
#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void FogApplySolid ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_GD_BASE_PASS
#ifdef SO_SF_FOG_ON
#ifdef SO_GD_FOG_SOLID
	half  	dist 	 		= distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half	distFade 		= 1.0 - clamp ( ( _FogEndDistance - dist ) / _FogSolidDistance, 0.0, 1.0 );
	i_sold.finalRGBA.rgb 	= lerp ( i_sold.finalRGBA.rgb, _FogColorFade, distFade );
#endif
#endif
#endif
}

inline void FogApply ( inout SOLightingData i_sold, in v2f i )
{

#ifdef SO_GD_BASE_PASS
	#ifdef SO_GD_FOG_SOLID
		FogApplySolid ( i_sold, i );
	#endif

	#ifdef SO_GD_FOG_VOLUMETRIC
		FogApplyVolumetric ( i_sold, i );
	#endif

	#ifdef SO_GD_FOG_VOLUMETRIC_3D
		FogApplyVolumetric3D ( i_sold, i );
	#endif

#endif
}

/*


//-------------------------------------------------------------------------------------
inline void FogApplyVolumetric3D ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_GD_BASE_PASS
#ifdef SO_SF_FOG_ON
#ifdef SO_GD_FOG_VOLUMETRIC_3D
	half  		dist;
	half		distFade;
	half    	stepSize;
	half3 		uvAdd;
	half 		fogAmount;
	half3 		fogUVOffset;
	half    	fogDensity;
	half    	fogDensityScaled;
	half    	fogValue;
	half 		fogYLength;
	half    	heightIntensity;
	half3    	rayPos;
	half3    	rayAdd;
	SOFLOAT3 	fogColor;

	fogUVOffset.x = _Time.y * _FogMoveX;
	fogUVOffset.y = _Time.y * _FogMoveY;
	fogUVOffset.z = _Time.y * _FogMoveZ;

	half3 viewDir   = normalize (i.positionWorld-_WorldSpaceCameraPos.xyz );
	dist 	 		= length ( _WorldSpaceCameraPos.xyz - i.positionWorld );
	distFade 		= 1.0 - clamp ( ( _FogEndDistance - dist ) / _FogSolidDistance, 0.0, 1.0 );
	stepSize 		= dist / (float)SO_GD_FOG_RAYCOUNT;
	rayPos 			= _WorldSpaceCameraPos.xyz;
    rayAdd 			= viewDir * stepSize;
	fogDensity 		= _FogDensity / (SO_GD_FOG_RAYCOUNT-1);
	fogAmount 		= 0;

	for ( int i = 0; i < SO_GD_FOG_RAYCOUNT; i++ )
	{
		fogValue 	= tex3D ( _FogTexture3D, ( rayPos * _FogScale ) + fogUVOffset ).a;

	#ifdef SO_GD_FOG_ROUGHNESS_ON
		fogYLength = length ( rayPos.y - _FogHeight + ( ( fogValue - 0.5 ) * _FogVerticalRoughness ) );
	#else
		fogYLength 	= length ( _FogHeight - rayPos.y );
	#endif

		heightIntensity  = 1.0 - min ( ( fogYLength / _FogHeightSize ), 1.0 );

        fogAmount 	+= fogValue * heightIntensity * fogDensity;
		rayPos 		+= viewDir * stepSize;
	}

	fogAmount = ( 1.0 - exp ( -dist * fogAmount ) );

#ifdef SO_LIGHTING_ON
	fogColor = _FogColor * ( i_sold.fogRayLight + _FogAmbient );
#else
   fogColor = _FogColor + _FogAmbient;
#endif

#ifdef SO_PREMULTIPLY_ALPHA
		i_sold.finalRGBA = lerp ( i_sold.finalRGBA, SOFLOAT4(0,0,0,0), fogAmount );
#else
		i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, fogColor, fogAmount );
#endif

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColorFade, distFade );

#endif
#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void FogApply5 ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half _FogDensity 			= 1.0;
	half _FogScatterFactor 		= 0.03;
	half _FogExtinctionFactor 	= 0.01;

	half  dist 	 		= distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half3 rayPos 		= i.positionWorld;
	half  extinction 	= 1.0;
	half  scattering 	= 0.0;

	half scatterCoef 	=_FogScatterFactor * _FogDensity;
	half extinctionCoef = _FogExtinctionFactor * _FogDensity;

	half stepScattering = scatterCoef * dist;

	extinction *= exp ( -extinctionCoef * dist );

	half heightIntensity  = 1.0 - min ( ( ( length ( rayPos.y - _FogHeight ) ) / _FogHeightSize ), 1.0 );

	scattering += ( extinction * stepScattering ) + heightIntensity;

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColor * i_sold.fogRayLight * scattering, clamp ( 1.0 - extinction, 0, 1 ) );

#endif
#endif
}


inline void FogApply4 ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half c = 0.1;
	half b = 0.1;

	half  dist 	 = distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half3 rayOri = _WorldSpaceCameraPos.xyz;
	half3 rayDir = i_sold.viewDir;

    float fogAmount = clamp ( c*exp(-rayOri.y*b)*(1.0-exp(-dist*rayDir.y*b))/rayDir.y, 0, 1 );
	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColorFade, fogAmount );
#endif
#endif
}

inline void FogApply3 ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half    dist 	 = distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half fogAmount   = 1.0 - exp ( -dist * 0.05 );

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColorFade, fogAmount );
#endif
#endif

}


//-------------------------------------------------------------------------------------
inline void FogApply2 ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half _FogDensity 			= 1.5;
	half _FogScatterFactor 		= 0.02;
	half _FogExtinctionFactor 	= 0.05;

	half    dist 	 = distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half    stepSize = ( dist ) / (float)SO_GD_FOG_RAYCOUNT;
	half3 	uvAdd  	 = i_sold.viewDir * stepSize * _FogScale;
	half3 	fogUV 	 = i.fogUV;
	half    fogY 	= i.positionWorld.y;

	half3 viewDir 		= i_sold.viewDir;
	half  extinction 	= 1.0;
	half3  scattering 	= 0.0;
	half3 rayPos 		= i.positionWorld;
	half3 rayAdd 		= viewDir * ( dist / SO_GD_FOG_RAYCOUNT );
	half  fogYOffset;

	for ( int i = 0; i < SO_GD_FOG_RAYCOUNT; i++ )
	{
		half density 		= tex3D ( _FogTexture3D, fogUV ).a;

		fogYOffset = ( density - 0.5 ) * _FogVerticalRoughness;

		half scatterCoef 	=_FogScatterFactor * density;
		half extinctionCoef = _FogExtinctionFactor * density;

		extinction *= exp ( -extinctionCoef * stepSize );

		//half3 AmbientColor = ComputeAmbientColor( rayPos, extinctionCoef );

		half stepScattering = scatterCoef * stepSize;// * AmbientColor;

		half heightIntensity  = 1.0 - min ( ( ( length ( rayPos.y - _FogHeight - fogYOffset ) ) / _FogHeightSize ), 1.0 );

		scattering += ( extinction * stepScattering ) * heightIntensity;
//		scattering += ( stepScattering ) * heightIntensity;

		fogUV += uvAdd;
		rayPos += rayAdd;
	}


	//scattering = clamp ( scattering, 0, 0.9 );

	//half3 fogColor = lerp ( i_sold.finalRGBA.rgb, _FogColor * ( i_sold.fogRayLight + _FogAmbient ), scattering );

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, scattering, 1.0 - extinction );

	//i_sold.finalRGBA.rgb = scattering;

	//i_sold.finalRGBA.rgb = fogColor;

	//i_sold.finalRGBA.rgb = extinction;
	 //i_sold.finalRGBA.rgb = scattering;// * heightLight;


#endif
#endif

}
*/



/*

//-------------------------------------------------------------------------------------
inline void FogApply ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half _FogDensity 			= 1.5;
	half _FogScatterFactor 		= 0.08;
	half _FogExtinctionFactor 	= 0.02;

	half    dist 	 = distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half    stepSize = ( dist * 0.8 ) / (float)SO_GD_FOG_RAYCOUNT;
	half3 	uvAdd  	 = i_sold.viewDir * stepSize * _FogScale;
	half3 	fogUV 	 = i.fogUV;
	half    accum = 0;
	half    fogY = i.positionWorld.y;
half3 F
//	half3 viewDir 		= normalize ( i.positionWorld - _WorldSpaceCameraPos.xyz );
	half3 viewDir 		= i_sold.viewDir;
	half  extinction 	= 1.0;
	half  scattering 	= 0.0;

	for ( int i = 0; i < SO_GD_FOG_RAYCOUNT; i++ )
	{
		half density 		= tex3D ( _FogTexture3D, fogUV ).a;

		half scatterCoef 	=_FogScatterFactor * density;
		half extinctionCoef = _FogExtinctionFactor * density;

		extinction *= exp ( -extinctionCoef * stepSize );

		half stepScattering = scatterCoef * stepSize;

		scattering += extinction * stepScattering;

		fogUV += uvAdd;
	}

	half heightIntensity  = clamp ( ( ( length ( fogY - _FogHeight ) ) / _FogHeightSize ), 0, 1 );

	scattering = min ( scattering , 1.0 );// - heightIntensity;

	half3 fogColor = lerp ( i_sold.finalRGBA.rgb, _FogColor, scattering );

	fogColor *= min ( i_sold.fogRayLight, SOFLOAT3(2,2,2) );
//	half3 fogColor = _FogColor * min ( scattering, 1.0 );

	i_sold.finalRGBA.rgb = lerp ( fogColor, _FogColorFade, 1.0 - extinction );

	//i_sold.finalRGBA.r = heightIntensity;
//	i_sold.finalRGBA.rgb = extinction;
	i_sold.finalRGBA.rgb = scattering;

#endif
#endif

}

//-------------------------------------------------------------------------------------
inline void FogApply ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half    dist 	 = distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half3   rayOri   = i.positionWorld;
	half3   viewPos = _WorldSpaceCameraPos.xyz;

	viewPos.y = _FogHeight;

	half3 	rayDir 	 = normalize(i.positionWorld - viewPos  );
//	half3 	rayDir 	 = i_sold.viewDir;
	half    c = 0.01;
	half    b = 0.01;

	float fogAmount = 1.0 - exp( -dist*b );

   // half fogAmount = c * exp(-rayOri.y*b) * (1.0-exp( -dist*rayDir.y*b )) / rayDir.y;

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColor, fogAmount );
//	i_sold.finalRGBA.r = extinction;
//	i_sold.finalRGBA.r = scattering;

#endif
#endif

}

//-------------------------------------------------------------------------------------
inline void FogApply ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
#ifdef SO_FOG_VOLUME
	half _FogDensity 			= 0.5;
	half _FogScatterFactor 		= 0.01;
	half _FogExtinctionFactor 	= 0.01;

	half    dist 	 = distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half    stepSize = dist / (float)SO_GD_FOG_RAYCOUNT;
	half3 	uvAdd  	 = i_sold.viewDir * stepSize * _FogScale;
	half3 	fogUV 	 = i.fogUV;
	half    accum = 0;

	half3 viewDir 		= i_sold.viewDir;
	half  extinction 	= 1.0;
	half  scattering 	= 0.0;

	for ( int i = 0; i < SO_GD_FOG_RAYCOUNT; i++ )
	{
		half density 		= tex3D ( _FogTexture3D, fogUV ).a;

		half scatterCoef 	=_FogScatterFactor * density;
		half extinctionCoef = _FogExtinctionFactor * density;

		extinction *= exp ( -extinctionCoef * stepSize );

		half stepScattering = scatterCoef * stepSize;

		scattering += extinction * stepScattering;// * i_sold.lightRGB.r;

		fogUV += uvAdd;
	}

	i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColor, scattering );
//	i_sold.finalRGBA.r = extinction;
//	i_sold.finalRGBA.r = scattering;

#endif
#endif

}


//-------------------------------------------------------------------------------------
inline void FogApply ( inout SOLightingData i_sold, in v2f i )
{
#ifdef SO_SF_FOG_ON
	half    dist 		= distance ( _WorldSpaceCameraPos.xyz , i.positionWorld );
	half distAmt 		= clamp ( 1.0 - ( _FogEndDistance - dist ) / _FogSolidDistance, 0, 1 );

#ifdef SO_FOG_VOLUME
	half    fogHeightSize 	= _FogHeightSize;
	half   	fogY 			= _FogHeight;
	half3 	fogUV 			= i.fogUV;
	half    fogAmbient 		= _FogAmbient;
    half3   fogDir  		= normalize(i_sold.viewDir);
	half 	accum 			= 0;
	half  	heightIntensity;
	half 	intensity;
	half    fog;
	half3   uvAdd;
	half   	fogYOffset;

	intensity 		= ( _FogColor.a * length (i_sold.viewDir)  ) * ( 32.0 / (float)(SO_GD_FOG_RAYCOUNT+1) );

	half smokeAmt 	= 1.0 - clamp ( ( _FogStartDistance - dist ) / _FogStartSmokeDistance, 0.0, 1.0 );

	dist 	= dist * 0.8 / (float)SO_GD_FOG_RAYCOUNT;

	#ifdef SO_SF_FOGMAP_ON
		half3  	rayAdd 			= fogDir * dist;
		half3  	rayPos 			= i.positionWorld;
		half 	fogMapScaleX 	= _FogMapScaleX;
		half 	fogMapScaleY 	= _FogMapScaleY;
		half 	fogMapScaleZ 	= _FogMapScaleZ;
		half2   fuv;
		half4 	fogMap;
		half 	fogMapY;
		half    fogMapHeight;
	#else
		half    rayAdd = fogDir.y * dist;
		half  	rayPos = i.positionWorld.y;
	#endif

	uvAdd 	= fogDir * _FogScale * dist;

	heightIntensity  = 1.0 - clamp ( ( ( length ( rayPos - fogY ) ) / fogHeightSize ), 0.0h , 1.0h );

	for ( int i = 0; i < SO_GD_FOG_RAYCOUNT; i++ )
	{
		fog = tex3D ( _FogTexture3D, fogUV ).a;

		fogYOffset = ( fog - 0.5 ) * _FogVerticalRoughness;

	// for R = strength  G verticle position B verticle size
	#ifdef SO_SF_FOGMAP_ON
			/// SCOTTFIND
			// make sure mipmaps are off and texture compressions for MAP
			fuv     = half2 ( rayPos.x*fogMapScaleX+0.5h, rayPos.z*fogMapScaleZ+0.5h );
			fogMap 	= tex2D ( _FogMap, fuv );

			fog 		*= fogMap.r;
			fogMapY 	 = fogMap.g * fogMapScaleY + fogY;
			fogMapHeight = fogHeightSize + fogMap.b * _FogMapScaleY;

			heightIntensity  = 1.0 - clamp ( ( ( length ( rayPos.y - fogMapY - fogYOffset ) ) / fogMapHeight ), 0.0h , 1.0h );
	#else
	    	heightIntensity  = 1.0 - clamp ( ( ( length ( rayPos - fogY - fogYOffset ) ) / fogHeightSize ), 0.0h , 1.0h );
	#endif

		accum += fog * intensity * heightIntensity;

		fogUV += uvAdd;
		rayPos += rayAdd;
	}

	#ifdef SO_LIGHTING_ON
//		i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, ( i_sold.fogRayLight + fogAmbient )  * accum * _FogColor.rgb, clamp ( smokeAmt, 0.0, 0.5 ) );
		i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, ( i_sold.fogRayLight + fogAmbient )  * accum * _FogColor.rgb, clamp ( smokeAmt, 0.0, 0.25 ) );
	#else
		i_sold.finalRGBA.rgb += _FogColor * accum;
	#endif

#endif

		i_sold.finalRGBA.rgb = lerp ( i_sold.finalRGBA.rgb, _FogColorFade, distAmt );

#endif
}
*/



