// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

#ifdef SO_LIGHTING_ON

//-------------------------------------------------------------------------------------
inline half DistanceAttenuation ( half distanceSqr, half3 distanceAttenuation)
{
    half quadFalloff 	= distanceAttenuation.x;
    half denom 			= distanceSqr * quadFalloff + 1.0h;
    half lightAtten 	= 1.0h / denom;
    half smoothFactor 	= saturate(distanceSqr * distanceAttenuation.y + distanceAttenuation.z);
	return lightAtten * smoothFactor;
}

//-------------------------------------------------------------------------------------
inline half SpotAttenuation ( half3 spotDirection, half3 lightDirection, half4 spotAttenuation )
{
    half 	SdotL = dot(spotDirection, lightDirection);
    half 	atten = saturate(SdotL * spotAttenuation.x + spotAttenuation.y);
    return 	atten * atten;
}

//-------------------------------------------------------------------------------------
inline SOLight ShaderOneDirLightSetup ( int i_lightIndex, in SOLightingData i_sold )
{
#ifdef SO_LIGHTING_ON
	SOLight sol;

	half3 delta = _Directional_SpotDir[i_lightIndex].xyz;

	sol.direction = delta;

 	sol.attenuation = 1.0;

	#ifdef SO_FOG_LIGHT
	sol.fogAttenuation = 1;
	#endif

	sol.color = _Directional_Color [ i_lightIndex ];

	return ( sol );
#endif
}

//-------------------------------------------------------------------------------------
inline SOLight ShaderOnePointLightSetup ( int i_lightIndex, in SOLightingData i_sold )
{
	SOLight sol;


#ifdef SO_POSWORLD_VARY_ON
	half3 delta = _Point_Position[i_lightIndex].xyz - i_sold.positionWorld.xyz;// * _Point_Position[i_lightIndex].w;
#else
	half3 delta = half3(1,1,1);
#endif

	half distSqr = max ( dot ( delta, delta ), 0 );
	sol.direction = half3 ( delta * rsqrt ( distSqr ) );

 	sol.attenuation = DistanceAttenuation ( distSqr, _Point_DistAtten[i_lightIndex].xyz );

	#ifdef SO_FOG_LIGHT
	sol.fogAttenuation = sol.attenuation;
	#endif

	sol.color = _Point_Color [ i_lightIndex ];

	return ( sol );
}

//-------------------------------------------------------------------------------------
inline SOLight ShaderOneSpotLightSetup (  int i_lightIndex, in SOLightingData i_sold )
{
	SOLight sol;

#ifdef SO_POSWORLD_VARY_ON
	half3 delta = _Spot_Position[i_lightIndex].xyz - i_sold.positionWorld;// * i_position[i_lightIndex].w;
	#else
	half3 delta = half3(1,1,1);
#endif

	sol.direction = normalize ( delta );

	half distSqr = max ( dot ( delta, delta ), 0 );

 	sol.attenuation = DistanceAttenuation ( distSqr, _Spot_DistAtten[i_lightIndex].xyz );
	sol.attenuation *= SpotAttenuation( _Spot_SpotDir[i_lightIndex], sol.direction, _Spot_SpotAtten[i_lightIndex]);

	#ifdef SO_FOG_LIGHT
	sol.fogAttenuation = sol.attenuation;
	#endif

	sol.color = _Spot_Color [ i_lightIndex ];

	return ( sol );
}

//-------------------------------------------------------------------------------------
inline SOFLOAT3 ShaderOneDistance ( in SOLightingData i_sold, in SOLight i_sol )
{
	return ( i_sol.color * i_sol.attenuation );
}

//-------------------------------------------------------------------------------------
inline void ShaderOneLighting ( inout SOLightingData i_sold, in SOLight i_sol )
{
#ifdef SO_LIGHTING_ON

	i_sol.dotNL   = saturate ( dot ( i_sold.worldNormal, i_sol.direction ) );

	#ifdef SO_HALFDIR_ON
	i_sol.halfDir = normalize ( i_sol.direction + i_sold.viewDir );
	#endif

	#ifdef SO_DOTNH_ON
	i_sol.dotNH   = saturate ( dot ( i_sold.worldNormal, i_sol.halfDir ) );
	#endif

	#ifdef SO_DOTLH_ON
	i_sol.dotLH   = saturate ( dot ( i_sol.direction, i_sol.halfDir ) );
	#endif

	#ifdef SO_FOG_LIGHT
    i_sold.fogRayLight += i_sol.color * i_sol.fogAttenuation;
	#endif

#ifndef SO_LIGHT_NO_AFFECT

	#ifdef SO_LIGHTING_DISTANCE
	i_sold.lightRGB.rgb += ShaderOneDistance ( i_sold, i_sol );
	#else

	i_sol.color *= ( i_sol.dotNL * i_sol.attenuation );

	i_sold.lightRGB.rgb += i_sol.color;
	#endif

#endif

	Specular ( i_sold, i_sol );

#endif
}

//-------------------------------------------------------------------------------------
inline void ShaderOneDirectionalVert ( inout SOLightingData i_sold )
{
#ifdef SO_MC_V_DIRECTIONAL_ON
   SOLight sol;

	for ( int index = SO_GD_V_DIR_START; index <= SO_GD_V_DIR_END; index++ )
	{
		sol =  ShaderOneDirLightSetup ( index, i_sold );

		ShaderOneLighting ( i_sold, sol );
	}
#endif
}

//-------------------------------------------------------------------------------------
inline void ShaderOnePointVert ( inout SOLightingData i_sold )
{
#ifdef SO_MC_V_POINT_ON
	SOLight sol;

	for ( int index = SO_GD_V_POINT_START; index <= SO_GD_V_POINT_END; index++ )
	{
		sol = ShaderOnePointLightSetup (  index, i_sold );

		ShaderOneLighting ( i_sold, sol );
	}
#endif
}


//-------------------------------------------------------------------------------------
inline void ShaderOneSpotVert ( inout SOLightingData i_sold )
{
#ifdef SO_MC_V_SPOT_ON
	SOLight sol;

	for ( int index = SO_GD_V_SPOT_START; index <= SO_GD_V_SPOT_END; index++ )
	{
		sol = ShaderOneSpotLightSetup (  index, i_sold );

		ShaderOneLighting ( i_sold, sol );
	}
#endif
}


//-------------------------------------------------------------------------------------
inline void ShaderOneDirectionalFrag ( inout SOLightingData i_sold, v2f i_v2f )
{
#ifdef SO_MC_P_DIRECTIONAL_ON
	SOLight sol;
	half shadowAtten;

	for ( int index = SO_GD_P_DIR_START; index <= SO_GD_P_DIR_END; index++ )
	{
		sol =  ShaderOneDirLightSetup ( index, i_sold );

#ifdef SO_SHADOWS
		shadowAtten = SHADOW_ATTENUATION(i_v2f);
//		sol.attenuation *= clamp ( shadowAtten + i_sold.metallic * 0.5, 0, 1 );

	#ifdef SO_FOG_LIGHT
		sol.fogAttenuation = sol.attenuation;
	#endif

    #if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
		sol.attenuation *= UnityMixRealtimeAndBakedShadows(shadowAtten, i_sold.bakedAtten, i_sold.bakedAmount );
	#else
		sol.attenuation *= shadowAtten;
	#endif


	#if defined(SO_SPECULAR_ONLY)
		i_sold.lightRGB = shadowAtten;
	#endif
#endif

	    ShaderOneLighting ( i_sold, sol );
	}
#endif
}


//-------------------------------------------------------------------------------------
inline void ShaderOnePointFrag ( inout SOLightingData i_sold )
{
#ifdef SO_MC_P_POINT_ON
	SOLight sol;

	for ( int index = SO_GD_P_POINT_START; index <= SO_GD_P_POINT_END; index++ )
	{
		sol = ShaderOnePointLightSetup (  index, i_sold );

		ShaderOneLighting ( i_sold, sol );
	}
#endif
}


//-------------------------------------------------------------------------------------
inline void ShaderOneSpotFrag ( inout SOLightingData i_sold )
{
#ifdef SO_MC_P_SPOT_ON
	SOLight sol;

	for ( int index = SO_GD_P_SPOT_START; index <= SO_GD_P_SPOT_END; index++ )
	{
		sol = ShaderOneSpotLightSetup (  index, i_sold );

		ShaderOneLighting ( i_sold, sol );
	}
#endif
}

//-------------------------------------------------------------------------------------
inline void ShaderOneUnityLight ( inout SOLightingData i_sold,  v2f i_v2f )
{
#if defined(SO_GD_PIPELINE_UNITY_FORWARD) && defined(SO_LIGHTING_ON)
	SOLight sol;
	half shadowAtten = 1;
	sol.attenuation = 1;

	#if defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
		sol.direction 	= normalize ( _WorldSpaceLightPos0 );
	#else
		sol.direction 	= normalize( _WorldSpaceLightPos0 - i_sold.positionWorld );
    #endif

#if defined (SO_UNITY_5) || defined (SO_UNITY_2017)
		sol.attenuation = LIGHT_ATTENUATION_ONLY(i_v2f);

	#if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SHADOWS_CUBE)
		shadowAtten = SHADOW_ATTENUATION(i_v2f);
	#endif

#else
	#if !defined (SHADOWS_SCREEN) && !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE)
		UNITY_LIGHT_ATTENUATION_NOSHADOW( sol.attenuation, i_v2f, i_sold.positionWorld);
	#else
		UNITY_LIGHT_ATTENUATION_SHADOW( sol.attenuation, shadowAtten, i_v2f, i_sold.positionWorld);
	#endif
#endif


	#ifdef SO_FOG_LIGHT
		sol.fogAttenuation = sol.attenuation;
	#endif

     #if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
		sol.attenuation *= UnityMixRealtimeAndBakedShadows(shadowAtten, i_sold.bakedAtten, i_sold.bakedAmount );
	#else

	#ifndef SO_SHADOWS_OFF
		sol.attenuation *= shadowAtten;
	#endif

	#endif

		sol.color 				= _LightColor0.rgb;

		ShaderOneLighting ( i_sold, sol );

#endif
}
#endif

//-------------------------------------------------------------------------------------
inline void SurfaceSetupDiffuse ( inout SOLightingData i_sold )
{
#if ( defined (SO_SF_SPECULAR_ON) || ( defined (SO_REFLECT_ON) ) && defined( SO_GD_BASE_PASS ) )
		i_sold.perceptualRoughness *= ( 1.0 - min ( i_sold.smoothness, 1 ) );
		i_sold.roughness 			= i_sold.perceptualRoughness * i_sold.perceptualRoughness;
		i_sold.roughnessX2 			= i_sold.roughness * i_sold.roughness;

	#ifdef SO_GD_WORKFLOW_SPECULAR
		i_sold.reflectivity  	= GetMaxColor ( i_sold.specularColor );
		i_sold.metallic  	   *= i_sold.reflectivity;
		i_sold.finalRGBA.rgb 	= i_sold.finalRGBA.rgb * ( SOFLOAT3 ( 1.0h, 1.0h, 1.0h ) - i_sold.specularColor );
	#else
		i_sold.reflectivity 	= i_sold.metallic;

		#ifdef SO_SF_SPECULAR_ON

		#ifdef SO_GD_SPECULAR_BLEND_MONOCHROMATIC
		SOFLOAT3 specularColor	= GetMaxColor(i_sold.finalRGBA.rgb);
		#else
		SOFLOAT3 specularColor	= i_sold.finalRGBA.rgb;
		#endif

		i_sold.specularColor *= specularColor;
		#endif

		i_sold.finalRGBA.rgb *= ( 1.0 - i_sold.metallic );
	#endif

#endif

	#if defined (SO_REFLECT_ON) && defined(SO_GD_BASE_PASS)
		ReflectionGet ( i_sold );
	    i_sold.reflection = i_sold.reflection * i_sold.metallic;
	#endif

	#if defined (SO_PREMULTIPLY_ALPHA) && defined (SO_SURFACE_VARS) && defined (SO_PBR_LIGHTING)
		i_sold.finalRGBA.a *= i_sold.metallic;
	#endif

}

//-------------------------------------------------------------------------------------
inline void  CalcRoughnessSmoothness ( inout SOLightingData i_sold )
{
#if defined (SO_SURFACE_VARS)
#ifdef	SO_GD_WORKFLOW_ROUGHNESS
		#ifdef SO_SMOOTHNESS_MAP_READ
		i_sold.perceptualRoughness *= ( 1.0 - min ( i_sold.smoothness, 1 ) );
		#else
		i_sold.smoothness = ( 1.0 - min ( i_sold.perceptualRoughness, 1 ) );
		#endif
#else
	#ifdef SO_ROUGHNESS_MAP_READ
		i_sold.perceptualRoughness *= ( 1.0 - min ( i_sold.smoothness, 1 ) );
	#else
		i_sold.perceptualRoughness = ( 1.0 - min ( i_sold.smoothness, 1 ) );
	#endif

#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void SurfaceSetupPBR ( inout SOLightingData i_sold )
{
#if defined (SO_PBR_LIGHTING) && defined (SO_LIGHTING_ON)
	half reflectInverse;

	CalcRoughnessSmoothness ( i_sold );

	#ifdef SO_GD_WORKFLOW_SPECULAR
		i_sold.reflectivity  	= GetMaxColor ( i_sold.specularColor );
		i_sold.metallic 	   *=  i_sold.reflectivity;
		reflectInverse 			= 1.0 - i_sold.reflectivity;
		i_sold.finalRGBA.rgb 	= i_sold.finalRGBA.rgb * ( SOFLOAT3 ( 1.0h, 1.0h, 1.0h ) - i_sold.specularColor );
	#else

		reflectInverse 			= DielectricSpec.a - i_sold.metallic * DielectricSpec.a;
		i_sold.reflectivity 	= 1.0h - reflectInverse;

		#ifdef SO_GD_SPECULAR_BLEND_MONOCHROMATIC
		SOFLOAT3 monoColor 		= GetMaxColor(i_sold.finalRGBA.rgb);
		SOFLOAT3 specularColor	= lerp ( DielectricSpec.rgb, monoColor, i_sold.metallic );
		#else
		SOFLOAT3 specularColor	= lerp ( DielectricSpec.rgb, i_sold.finalRGBA.rgb, i_sold.metallic );
		#endif

		i_sold.specularColor *= specularColor;

    	i_sold.finalRGBA.rgb 	= i_sold.finalRGBA.rgb * reflectInverse;
	#endif

		i_sold.roughness 		= max ( (i_sold.perceptualRoughness * i_sold.perceptualRoughness), 0.002 );
		i_sold.roughnessX2 		= i_sold.roughness * i_sold.roughness;

#if defined( SO_GD_BASE_PASS )
		ReflectionGet ( i_sold );

		#ifdef UNITY_COLORSPACE_GAMMA
		SOFLOAT surfaceReduction = 1.0 - 0.28 * i_sold.roughness * i_sold.perceptualRoughness;
		#else
		SOFLOAT surfaceReduction = 1.0 / ( i_sold.roughnessX2 + 1.0 );
		#endif

	    i_sold.reflection.rgb = surfaceReduction * i_sold.reflection * FresnelReflection ( i_sold );
#endif

	#ifdef SO_PREMULTIPLY_ALPHA
	    i_sold.finalRGBA.a = i_sold.finalRGBA.a * reflectInverse + i_sold.reflectivity;
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void SurfaceSetup ( inout SOLightingData i_sold )
{
	#if defined (SO_PBR_LIGHTING) && defined (SO_LIGHTING_ON)
		SurfaceSetupPBR ( i_sold );
	#else
		SurfaceSetupDiffuse ( i_sold );
	#endif
}

//-------------------------------------------------------------------------------------
inline void ShaderOneProcessLightsFrag ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_FRAG_LIGHTING_ON
	#ifdef SO_GD_PIPELINE_SHADER_ONE
		ShaderOneDirectionalFrag ( i_sold, i_v2f );
		ShaderOnePointFrag ( i_sold );
		ShaderOneSpotFrag ( i_sold );
	#else
		ShaderOneUnityLight ( i_sold, i_v2f );
	#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void ShaderOneApplyLightFrag ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_VERTEX_COLOR
	   i_sold.finalRGBA *= i_v2f.vertexColor;
#endif

///SCOTTFIND LIGHTNOEFFECT
#ifdef SO_FRAG_LIGHTING_ON
#ifndef SO_LIGHT_NO_AFFECT
	   i_sold.finalRGBA.rgb *= i_sold.lightRGB;
		UnlitMask ( i_sold );
#endif
#endif

#ifdef SO_REFLECT_ON
		i_sold.finalRGBA.rgb += i_sold.reflection;
#endif

#ifdef SO_SF_SPECULAR_ON
		i_sold.finalRGBA.rgb += i_sold.specularLight;
#endif

#ifdef SO_PREMULTIPLY_ALPHA
		i_sold.finalRGBA.rgb *= i_sold.finalRGBA.a;
#endif

		AmbientOcclusion ( i_sold, i_sold.finalRGBA.rgb );
}



