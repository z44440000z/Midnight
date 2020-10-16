// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

#if defined (SO_ALPHA_MAP_SMOOTHNESS) || \
	( defined ( SO_SF_LAYER0_SURFACE_MAP_ON ) && defined (SO_SURFACE_MAP_SMOOTHNESS) ) || \
	( defined ( SO_SF_LAYER1_SURFACE_MAP_ON ) && defined (SO_SURFACE_MAP_SMOOTHNESS) ) || \
    ( defined ( SO_SF_LAYER2_SURFACE_MAP_ON ) && defined (SO_SURFACE_MAP_SMOOTHNESS) ) || \
    ( defined ( SO_SF_LAYER3_SURFACE_MAP_ON ) && defined (SO_SURFACE_MAP_SMOOTHNESS) ) || \
	( defined ( SO_SF_LAYER0_SPECULAR_MAP_ON ) && defined (SO_SPECULAR_MAP_SMOOTHNESS) ) || \
	( defined ( SO_SF_LAYER1_SPECULAR_MAP_ON ) && defined (SO_SPECULAR_MAP_SMOOTHNESS) ) || \
	( defined ( SO_SF_LAYER2_SPECULAR_MAP_ON ) && defined (SO_SPECULAR_MAP_SMOOTHNESS) ) || \
	( defined ( SO_SF_LAYER3_SPECULAR_MAP_ON ) && defined (SO_SPECULAR_MAP_SMOOTHNESS) )

	#define SO_SMOOTHNESS_MAP_READ

#endif


#if defined (SO_ALPHA_MAP_ROUGHNESS) || \
	( defined (SO_SF_LAYER0_SURFACE_MAP_ON) && defined (SO_SURFACE_MAP_ROUGHNESS) ) || \
	( defined (SO_SF_LAYER1_SURFACE_MAP_ON) && defined (SO_SURFACE_MAP_ROUGHNESS) ) || \
    ( defined (SO_SF_LAYER2_SURFACE_MAP_ON) && defined (SO_SURFACE_MAP_ROUGHNESS) ) || \
    ( defined (SO_SF_LAYER3_SURFACE_MAP_ON) && defined (SO_SURFACE_MAP_ROUGHNESS) ) || \
	( defined (SO_SF_LAYER0_SPECULAR_MAP_ON) && defined (SO_SPECULAR_MAP_ROUGHNESS) ) || \
	( defined (SO_SF_LAYER1_SPECULAR_MAP_ON) && defined (SO_SPECULAR_MAP_ROUGHNESS) ) || \
	( defined (SO_SF_LAYER2_SPECULAR_MAP_ON) && defined (SO_SPECULAR_MAP_ROUGHNESS) ) || \
	( defined (SO_SF_LAYER3_SPECULAR_MAP_ON) && defined (SO_SPECULAR_MAP_ROUGHNESS) )

	#define SO_ROUGHNESS_MAP_READ

#endif

//-------------------------------------------------------------------------------------
inline void SurfacePropertiesInit ( inout SOLayer i_layer, SOFLOAT i_metallic, SOFLOAT i_glossiness, SOFLOAT3 i_specColor )
{
#ifdef SO_SURFACE_VARS

#if defined (SO_GD_WORKFLOW_SPECULAR) || defined(SO_SF_SPECULAR_ON)
    i_layer.specularColor 		= i_specColor;
#endif

	i_layer.metallic 			= i_metallic;

#ifdef	SO_GD_WORKFLOW_ROUGHNESS
	i_layer.roughness = i_glossiness;

	#ifdef SO_SMOOTHNESS_MAP_READ
	i_layer.smoothness = 1.0;
	#endif

#else
	i_layer.smoothness = i_glossiness;

	#ifdef SO_ROUGHNESS_MAP_READ
	i_layer.roughness = 1.0;
	#endif

#endif

#endif
}


//-------------------------------------------------------------------------------------
inline void SurfacePropertiesFromMap ( inout SOLayer i_layer, in half2 i_uv, sampler2D i_surfaceMap )
{
#ifdef SO_SURFACE_MAP_ON
#ifdef SO_SURFACE_VARS
		SOFLOAT4 surfaceMap =  tex2D ( i_surfaceMap, i_uv );

		GetSurfaceMap_METALLIC ( surfaceMap, i_layer.metallic );

	#ifdef SO_SURFACE_MAP_ROUGHNESS
		GetSurfaceMap_ROUGHNESS ( surfaceMap, i_layer.roughness );
	#else
		GetSurfaceMap_SMOOTHNESS ( surfaceMap, i_layer.smoothness );
	#endif

	GetSurfaceMap_AMBIENT_OCCLUSION ( surfaceMap, i_layer.ambientOcclusion );
	GetSurfaceMap_UNLIT_MASK ( surfaceMap, i_layer.unlitMask );
	GetSurfaceMap_PARALLAX_HEIGHT ( surfaceMap, i_layer.parallaxHeight  );
	GetSurfaceMap_PROGRESS_GRADIENT ( surfaceMap, i_layer.progress );


#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void SpecularPropertiesFromMap ( inout SOLayer i_layer, in half2 i_uv, sampler2D i_specularMap )
{
#ifdef SO_GD_WORKFLOW_SPECULAR
//#ifdef SO_SPECULAR_MAP_ON
#ifdef SO_SURFACE_VARS
	SOFLOAT4 specularMap =  tex2D ( i_specularMap, i_uv );

	i_layer.specularColor 	*= specularMap.rgb;

	GetSpecularMap_METALLIC ( specularMap, i_layer.metallic );

	#ifdef SO_SPECULAR_MAP_ROUGHNESS
	GetSpecularMap_ROUGHNESS ( specularMap, i_layer.roughness );
	#else
	GetSpecularMap_SMOOTHNESS ( specularMap, i_layer.smoothness );
	#endif

	GetSpecularMap_AMBIENT_OCCLUSION ( specularMap, i_layer.ambientOcclusion );
	GetSpecularMap_UNLIT_MASK ( specularMap, i_layer.unlitMask );
	GetSpecularMap_PARALLAX_HEIGHT ( specularMap, i_layer.parallaxHeight  );
	GetSpecularMap_PROGRESS_GRADIENT ( specularMap, i_layer.progress );


#endif
//#endif
#endif
}

//-------------------------------------------------------------------------------------
inline void SurfacePropertiesSet ( inout SOLightingData i_sold, inout SOLayer i_layer, in SOFLOAT i_fresnel, SOFLOAT i_bumpScale, SOFLOAT i_aoScale )
{
#ifdef SO_GD_BUMP_SCALE_PER_LAYER
	i_sold.bumpScale = i_bumpScale;
#endif

#if defined (SO_GD_AO_SCALE_PER_LAYER) && ( defined (SO_SURFACE_MAP_AMBIENT_OCCLUSION) || defined (SO_ALPHA_MAP_AMBIENT_OCCLUSION) )
	i_sold.aoScale = i_aoScale;
#endif

#if defined (SO_GD_AO_SCALE_PER_LAYER) && ( defined (SO_SURFACE_MAP_AMBIENT_OCCLUSION) || defined (SO_ALPHA_MAP_AMBIENT_OCCLUSION) )
	i_sold.bumpScale = i_aoScale;
#endif

#ifdef SO_SURFACE_VARS

#if defined (SO_GD_WORKFLOW_SPECULAR) || defined(SO_SF_SPECULAR_ON)
    	i_sold.specularColor = i_layer.specularColor;
#endif

		i_sold.metallic					= i_layer.metallic;
		i_sold.perceptualRoughness   	= i_layer.roughness;
		i_sold.smoothness   			= i_layer.smoothness;
#endif

#ifdef SO_GD_FRESNEL_ON
		i_sold.fresnel = i_fresnel;
#endif

#if defined (SO_SURFACE_MAP_AMBIENT_OCCLUSION) || defined (SO_ALPHA_MAP_AMBIENT_OCCLUSION)
		i_sold.ambientOcclusion	= i_layer.ambientOcclusion;
#endif

#if defined (SO_SURFACE_MAP_MASK) || defined (SO_ALPHA_MAP_MASK)
		i_sold.unlitMask  = i_layer.unlitMask;
#endif

}

//-------------------------------------------------------------------------------------
inline void SurfacePropertiesBlend ( inout SOLightingData i_sold, inout SOLayer i_layer, SOFLOAT i_alpha, in SOFLOAT i_fresnel, SOFLOAT i_bumpScale, SOFLOAT i_aoScale )
{
#ifdef SO_GD_BUMP_SCALE_PER_LAYER
	i_layer.bumpScale = lerp ( i_sold.bumpScale, i_bumpScale, i_alpha );
#endif

#if defined (SO_GD_AO_SCALE_PER_LAYER) && ( defined (SO_SURFACE_MAP_AMBIENT_OCCLUSION) || defined (SO_ALPHA_MAP_AMBIENT_OCCLUSION) )
	i_layer.aoScale = lerp ( i_sold.aoScale, i_aoScale, i_alpha );
#endif

#ifdef SO_SURFACE_VARS

	#if defined (SO_GD_WORKFLOW_SPECULAR) || defined(SO_SF_SPECULAR_ON)
	i_sold.specularColor = lerp ( i_sold.specularColor, i_layer.specularColor, i_alpha );
	#endif

	i_sold.metallic				= lerp ( i_sold.metallic, i_layer.metallic, i_alpha );
	i_sold.perceptualRoughness  = lerp ( i_sold.perceptualRoughness, i_layer.roughness, i_alpha );
	i_sold.smoothness   		= lerp ( i_sold.smoothness, i_layer.smoothness, i_alpha );
#endif

#ifdef SO_GD_FRESNEL_ON
		i_sold.fresnel = lerp ( i_sold.fresnel, i_fresnel, i_alpha );
#endif

#ifdef SO_SURFACE_MAP_AMBIENT_OCCLUSION
		i_sold.ambientOcclusion	= lerp ( i_sold.ambientOcclusion, i_layer.ambientOcclusion, i_alpha );
#endif

#ifdef SO_SURFACE_MAP_MASK
		i_sold.unlitMask	= lerp ( i_sold.unlitMask, i_layer.unlitMask, i_alpha );
#endif

}


