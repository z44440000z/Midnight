
inline void GetSurfaceMap_METALLIC( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_METALLIC

	#if defined (SO_SURFACE_MAPR_METALLIC)
		i_result *= (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_METALLIC)
		i_result *= (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_METALLIC)
		i_result *= (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_METALLIC)
		i_result *= (  i_surfaceMap.a );
	#endif

#else
#endif

}

inline void GetSurfaceMap_SMOOTHNESS( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_SMOOTHNESS

	#if defined (SO_SURFACE_MAPR_SMOOTHNESS)
		i_result *= (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_SMOOTHNESS)
		i_result *= (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_SMOOTHNESS)
		i_result *= (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_SMOOTHNESS)
		i_result *= (  i_surfaceMap.a );
	#endif

#else
#endif

}

inline void GetSurfaceMap_ROUGHNESS( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_ROUGHNESS

	#if defined (SO_SURFACE_MAPR_ROUGHNESS)
		i_result *= (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_ROUGHNESS)
		i_result *= (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_ROUGHNESS)
		i_result *= (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_ROUGHNESS)
		i_result *= (  i_surfaceMap.a );
	#endif

#else
#endif

}

inline void GetSurfaceMap_AMBIENT_OCCLUSION( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_AMBIENT_OCCLUSION

	#if defined (SO_SURFACE_MAPR_AMBIENT_OCCLUSION)
		i_result = (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_AMBIENT_OCCLUSION)
		i_result = (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_AMBIENT_OCCLUSION)
		i_result = (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_AMBIENT_OCCLUSION)
		i_result = (  i_surfaceMap.a );
	#endif

#endif

}

inline void GetSurfaceMap_PARALLAX_HEIGHT( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_PARALLAX_HEIGHT

	#if defined (SO_SURFACE_MAPR_PARALLAX_HEIGHT)
		i_result = (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_PARALLAX_HEIGHT)
		i_result = (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_PARALLAX_HEIGHT)
		i_result = (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_PARALLAX_HEIGHT)
		i_result = (  i_surfaceMap.a );
	#endif

#endif

}

inline void GetSurfaceMap_UNLIT_MASK( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_UNLIT_MASK

	#if defined (SO_SURFACE_MAPR_UNLIT_MASK)
		i_result = (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_UNLIT_MASK)
		i_result = (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_UNLIT_MASK)
		i_result = (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_UNLIT_MASK)
		i_result = (  i_surfaceMap.a );
	#endif

#endif

}

inline void GetSurfaceMap_PROGRESS_GRADIENT( in SOFLOAT4 i_surfaceMap, inout SOFLOAT i_result )
{

#ifdef SO_SURFACE_MAP_PROGRESS_GRADIENT

	#if defined (SO_SURFACE_MAPR_PROGRESS_GRADIENT)
		i_result = (  i_surfaceMap.r );
	#elif defined (SO_SURFACE_MAPG_PROGRESS_GRADIENT)
		i_result = (  i_surfaceMap.g );
	#elif defined (SO_SURFACE_MAPB_PROGRESS_GRADIENT)
		i_result = (  i_surfaceMap.b );
	#elif defined (SO_SURFACE_MAPA_PROGRESS_GRADIENT)
		i_result = (  i_surfaceMap.a );
	#endif

#endif

}

inline void GetAlphaMap_METALLIC( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_METALLIC
		i_result = (  i_sold.alphaMap ) * i_result;
#endif
}

inline void GetAlphaMap_SMOOTHNESS( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_SMOOTHNESS
		i_result = (  i_sold.alphaMap ) * i_result;
#endif
}

inline void GetAlphaMap_ROUGHNESS( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_ROUGHNESS
		i_result = (  i_sold.alphaMap ) * i_result;
#endif
}

inline void GetAlphaMap_AMBIENT_OCCLUSION( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_AMBIENT_OCCLUSION
		i_result = (  i_sold.alphaMap );
#endif
}

inline void GetAlphaMap_PARALLAX_HEIGHT( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_PARALLAX_HEIGHT
		i_result = (  i_sold.alphaMap );
#endif
}

inline void GetAlphaMap_UNLIT_MASK( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_UNLIT_MASK
		i_result = (  i_sold.alphaMap );
#endif
}

inline void GetAlphaMap_PROGRESS_GRADIENT( in SOLightingData i_sold, inout SOFLOAT i_result )
{
#ifdef SO_ALPHA_MAP_PROGRESS_GRADIENT
		i_result = (  i_sold.alphaMap );
#endif
}
