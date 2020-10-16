// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

//=================================================================
	SOFLOAT4 frag ( v2f i, half i_facing : VFACE ) : SV_Target
	{
		sampler2D temp = _MainTex;

		UNITY_SETUP_INSTANCE_ID(i);
		SOLight sol;
		SOLightingData sold;

		SetupShaderOneData ( sold, i );

	#ifdef SO_SF_NORMAL_FIX
		sold.facing = i_facing;
	#endif

//=============================================================================
// LAYERS
//=============================================================================
		SOLayer layer;

		UNITY_INITIALIZE_OUTPUT ( SOLayer, layer );

		ProcessFragLayer0 ( sold, layer, i );
		ProcessFragLayer1 ( sold, layer, i );
		ProcessFragLayer2 ( sold, layer, i );
		ProcessFragLayer3 ( sold, layer, i );

		LayersFinalize ( sold, i );

#if defined (SO_BLEND_CUTOUT) && !defined(SO_PROGRESS_ON)
		clip ( sold.finalRGBA.a - 0.5 );
#endif

		ApplyGlobalColor ( sold );

#ifdef SO_MAP_UNLIT_MASK
		sold.unlitRGB = sold.finalRGBA.rgb;
#endif

//------------------------------------
//  If any kind of light was calculated in Vertex put it in lightRGB
//------------------------------------
#if defined (SO_VERTEX_LIGHTING_ON) || defined (SO_LIGHT_PROBE_VERTEX)
	sold.lightRGB = i.vertexLighting.rgb;
#endif

#ifdef SO_LIGHT_PROBE_FRAG
	   SOFLOAT3 lpRGB;

	   lpRGB = ShadeSH9 ( half4(sold.worldNormal,1) );

//	   AmbientOcclusion ( sold, lpRGB );

	   sold.lightRGB += lpRGB;
#endif

// ------------------------------------
// Unity lightmapping
// ------------------------------------
#ifndef LIGHTMAP_OFF
	   sold.lightRGB.rgb += DecodeLightmap ( UNITY_SAMPLE_TEX2D ( unity_Lightmap, i.lmUV ) );

    #if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
		sold.bakedAtten 	= UnitySampleBakedOcclusion(i.lmUV, i.positionWorld);
		float zDist 		= dot(_WorldSpaceCameraPos.xyz - i.positionWorld.xyz, UNITY_MATRIX_V[2].xyz);
		float fadeDist 		= UnityComputeShadowFadeDistance(i.positionWorld, zDist);
		sold.bakedAmount 	= UnityComputeShadowFade(fadeDist);
	#endif

#endif

// ------------------------------------
// UNity realtime lightmapping ( emissive )
// ------------------------------------
#ifdef DYNAMICLIGHTMAP_ON
		SOFLOAT4 realtimeColorTex = UNITY_SAMPLE_TEX2D ( unity_DynamicLightmap, i.dlmUV  );
		half3 realtimeColor = DecodeRealtimeLightmap ( realtimeColorTex );
    	sold.lightRGB.rgb += realtimeColor;
#endif

//=====================================
#ifdef SO_SF_SATURATION_ON
	   Saturation ( sold );
#endif

	#ifdef SO_SF_NORMAL_FIX
	   sold.worldNormal *= sold.facing;
	#endif

	   SurfaceSetup ( sold );
	   ShaderOneProcessLightsFrag ( sold, i );
	   ShaderOneApplyLightFrag ( sold, i );

#if defined (SO_RIMLIT_ON) && defined(SO_GD_BASE_PASS)
		RimLighting ( sold );
#endif

//=====================================
#ifdef SO_SF_INTERSECT_ON
		Intersection ( sold, i );
#endif

// ------------------------------------
// UNITY EMISSION
// ------------------------------------
#ifdef _EMISSION
		///SCOTTFIND make this a function
		half2 euv = half2(0,0);
#if defined(SO_LAYER0_ON)
		euv = SO_UV_LAYER0(i);
#else
#ifdef SO_CLEAN_UV_ON
		euv = i.normUV;
#endif
#endif
		sold.finalRGBA.rgb += _EmissionColor.rgb * tex2D ( _EmissionMap, euv ).rgb;

#endif

//=====================================
#ifdef SO_SF_SCANLINE_ON
		Scanlines ( sold, i );
#endif

//#ifdef SO_GD_AMPLIFY_COLORS
		sold.finalRGBA.rgb *= _ColorAmplify;
//#endif

	    FogApply ( sold, i );

		return sold.finalRGBA;
}


