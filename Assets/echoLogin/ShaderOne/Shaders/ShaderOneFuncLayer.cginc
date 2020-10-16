// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

//=====================================================================================
// VERT Functions
//=====================================================================================

//-------------------------------------------------------------------------------------
inline half2 TransformTexUV1 ( in SOLightingData i_sold, half2 i_uv )
{
	half2 uv;

	half3 worldPos = i_sold.positionWorld * _SO_UV1_WorldMapScale;

	#ifdef SO_UV1_WORLDMAP_AXIS_X
	uv = worldPos.yz;
	#elif defined(SO_UV1_WORLDMAP_AXIS_Y)
	uv = worldPos.xz;
	#else
	uv = worldPos.xy;
	#endif

	return(uv);
}

//-------------------------------------------------------------------------------------
inline void Layer0VertSetup ( in SOLightingData i_sold, inout v2f i_v2f, appdata i_v )
{
#ifdef SO_LAYER0_ON
	#ifdef SO_UV1_WORLDMAP_MESH_UV
		SO_UV_LAYER0(i_v2f) = TRANSFORM_TEX ( i_v.uv, LAYER0_TEX );
	#else
		SO_UV_LAYER0(i_v2f) = TransformTexUV1 ( i_sold, i_v.uv );
	#endif

	// Only for layer 0 ( mainTex )
	#ifdef SO_CLEAN_UV_ON
		i_v2f.normUV        = SO_UV_LAYER0(i_v2f);
	#endif

	#ifdef SO_SF_MANUAL_CONTROL

		///SCOTTFIND
		#ifdef SO_SF_LAYER0_ROTATEUV_ON
    	SO_UV_LAYER0(i_v2f) = CalcLayerUVRotation ( SO_UV_LAYER0(i_v2f), LAYER0_TEX_ST, _Layer0Rotation, _Layer0RotationU, _Layer0RotationV );
		SO_UV_LAYER0(i_v2f) += half2 ( _Layer0ScrollU, _Layer0ScrollV );
		#endif

	#else
        SO_UV_LAYER0(i_v2f) += half2 (_Time.y * _ScrollU, _Time.y * _ScrollV);

		#ifdef SO_SF_LAYER0_SCROLLUV_ON
        SO_UV_LAYER0(i_v2f) += CalcLayerUVScrollAuto( _Layer0ScrollU, _Layer0ScrollV );
		#endif

		#ifdef SO_SF_LAYER0_ROTATEUV_ON
        SO_UV_LAYER0(i_v2f) = CalcLayerUVRotationAuto ( SO_UV_LAYER0(i_v2f), LAYER0_TEX_ST, _Layer0Rotation, _Layer0RotationU, _Layer0RotationV );
		#endif

	#endif

	#ifdef SO_LAYER0_ANIMTYPE_RANDOM_UV
	#ifdef SO_MC_CONTROL_SCRIPT_ON
    	SO_UV_LAYER0(i_v2f)   += half2 ( _Layer0AnimOffsetX, _Layer0AnimOffsetY );
	#else
        SO_UV_LAYER0(i_v2f)   += half2 ( _SinTime.y*23.8192, _CosTime.y*33.1977 );
	#endif
	#endif


	#ifdef SO_LAYER0_CELLANIM_ON
		#ifdef SO_LAYER0_ANIMTYPE_CELL_ANIM_BLEND
		SO_BUV_LAYER0(i_v2f) = CellAnimUV ( SO_UV_LAYER0(i_v2f), _Layer0AnimCellsHorz, _Layer0AnimCellsVert, _Layer0AnimOffsetX2, _Layer0AnimOffsetY2, LAYER0_TEX_ST );
		#endif
    	SO_UV_LAYER0(i_v2f) = CellAnimUV ( SO_UV_LAYER0(i_v2f), _Layer0AnimCellsHorz, _Layer0AnimCellsVert, _Layer0AnimOffsetX, _Layer0AnimOffsetY, LAYER0_TEX_ST );
	#endif

#else

	#ifdef SO_CLEAN_UV_ON
		i_v2f.normUV        = TRANSFORM_TEX ( i_v.uv, LAYER0_TEX );
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void Layer1VertSetup ( in SOLightingData i_sold, inout v2f i_v2f, appdata i_v )
{
#ifdef SO_LAYER1_ON
	#ifdef SO_SF_LAYER1_UV2_ON
		SO_UV_LAYER1(i_v2f) = TRANSFORM_TEX( i_v.uv2, LAYER1_TEX );
	#else
	#ifdef SO_UV1_WORLDMAP_MESH_UV
		SO_UV_LAYER1(i_v2f) = TRANSFORM_TEX( i_v.uv, LAYER1_TEX );
	#else
		SO_UV_LAYER1(i_v2f) = TransformTexUV1 ( i_sold, i_v.uv );
	#endif
	#endif

	#ifdef SO_SF_MANUAL_CONTROL

		#ifdef SO_SF_LAYER1_ROTATEUV_ON
		SO_UV_LAYER1(i_v2f) = CalcLayerUVRotation ( SO_UV_LAYER1(i_v2f), LAYER1_TEX_ST, _Layer1Rotation, _Layer1RotationU, _Layer1RotationV );
		#endif

		#if SO_SF_LAYER1_SCROLLUV_ON
		SO_UV_LAYER1(i_v2f) += half2 ( _Layer1ScrollU, _Layer1ScrollV );
		#endif
	#else
		SO_UV_LAYER1(i_v2f) += half2 (_Time.y * _ScrollU, _Time.y * _ScrollV);

		#ifdef SO_SF_LAYER1_SCROLLUV_ON
		SO_UV_LAYER1(i_v2f) += CalcLayerUVScrollAuto( _Layer1ScrollU, _Layer1ScrollV );
		#endif

		#ifdef SO_SF_LAYER1_ROTATEUV_ON
		SO_UV_LAYER1(i_v2f) = CalcLayerUVRotationAuto ( SO_UV_LAYER1(i_v2f), LAYER1_TEX_ST, _Layer1Rotation, _Layer1RotationU, _Layer1RotationV );
		#endif

	#endif

	#ifdef SO_LAYER1_ANIMTYPE_RANDOM_UV
	#ifdef SO_MC_CONTROL_SCRIPT_ON
		SO_UV_LAYER1(i_v2f)   += half2 ( _Layer1AnimOffsetX, _Layer1AnimOffsetY );
	#else
	    SO_UV_LAYER1(i_v2f)   += half2 ( _SinTime.y*23.8192, _CosTime.y*33.1977 );
	#endif
	#endif

	#ifdef SO_LAYER1_CELLANIM_ON
		#ifdef SO_LAYER1_ANIMTYPE_CELL_ANIM_BLEND
		SO_BUV_LAYER1(i_v2f) = CellAnimUV ( SO_UV_LAYER1(i_v2f), _Layer1AnimCellsHorz, _Layer1AnimCellsVert, _Layer1AnimOffsetX2, _Layer1AnimOffsetY2, LAYER1_TEX_ST );
		#endif
		SO_UV_LAYER1(i_v2f) = CellAnimUV ( SO_UV_LAYER1(i_v2f), _Layer1AnimCellsHorz, _Layer1AnimCellsVert, _Layer1AnimOffsetX, _Layer1AnimOffsetY, LAYER1_TEX_ST );
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void Layer2VertSetup (  in SOLightingData i_sold, inout v2f i_v2f, appdata i_v )
{
#ifdef SO_LAYER2_ON
	#ifdef SO_SF_LAYER2_UV2_ON
		SO_UV_LAYER2(i_v2f) = TRANSFORM_TEX( i_v.uv2, LAYER2_TEX );
	#else
	#ifdef SO_UV1_WORLDMAP_MESH_UV
		SO_UV_LAYER2(i_v2f) = TRANSFORM_TEX( i_v.uv, LAYER2_TEX );
	#else
		SO_UV_LAYER2(i_v2f) = TransformTexUV1 ( i_sold, i_v.uv );
	#endif
	#endif

	#ifdef SO_SF_MANUAL_CONTROL

		#ifdef SO_SF_LAYER2_ROTATEUV_ON
		SO_UV_LAYER2(i_v2f) = CalcLayerUVRotation ( SO_UV_LAYER2(i_v2f), LAYER2_TEX_ST, _Layer2Rotation, _Layer2RotationU, _Layer2RotationV );
		#endif

		SO_UV_LAYER2(i_v2f) += half2 ( _Layer2ScrollU, _Layer2ScrollV );
	#else
		SO_UV_LAYER2(i_v2f) += half2 (_Time.y * _ScrollU, _Time.y * _ScrollV);

		#ifdef SO_SF_LAYER2_SCROLLUV_ON
		SO_UV_LAYER2(i_v2f) += CalcLayerUVScrollAuto( _Layer2ScrollU, _Layer2ScrollV );
		#endif

		#ifdef SO_SF_LAYER2_ROTATEUV_ON
		SO_UV_LAYER2(i_v2f) = CalcLayerUVRotationAuto ( SO_UV_LAYER2(i_v2f), LAYER2_TEX_ST, _Layer2Rotation, _Layer2RotationU, _Layer2RotationV );
		#endif

	#endif

	#ifdef SO_LAYER2_ANIMTYPE_RANDOM_UV
	#ifdef SO_MC_CONTROL_SCRIPT_ON
		SO_UV_LAYER2(i_v2f)   += half2 ( _Layer2AnimOffsetX, _Layer2AnimOffsetY );
	#else
	    SO_UV_LAYER2(i_v2f)   += half2 ( _SinTime.y*23.8192, _CosTime.y*33.1977 );
	#endif
	#endif

	#ifdef SO_LAYER2_CELLANIM_ON
		#ifdef SO_LAYER2_ANIMTYPE_CELL_ANIM_BLEND
		SO_BUV_LAYER2(i_v2f) = CellAnimUV ( SO_UV_LAYER2(i_v2f), _Layer2AnimCellsHorz, _Layer2AnimCellsVert, _Layer2AnimOffsetX2, _Layer2AnimOffsetY2, LAYER2_TEX_ST );
		#endif
		SO_UV_LAYER2(i_v2f) = CellAnimUV ( SO_UV_LAYER2(i_v2f), _Layer2AnimCellsHorz, _Layer2AnimCellsVert, _Layer2AnimOffsetX, _Layer2AnimOffsetY, LAYER2_TEX_ST );
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void Layer3VertSetup (  in SOLightingData i_sold, inout v2f i_v2f, appdata i_v )
{
#ifdef SO_LAYER3_ON
	#ifdef SO_SF_LAYER3_UV2_ON
		SO_UV_LAYER3(i_v2f) = TRANSFORM_TEX( i_v.uv2, LAYER3_TEX );
	#else
	#ifdef SO_UV1_WORLDMAP_MESH_UV
		SO_UV_LAYER3(i_v2f) = TRANSFORM_TEX( i_v.uv, LAYER3_TEX );
	#else
		SO_UV_LAYER3(i_v2f) = TransformTexUV1 ( i_sold, i_v.uv );
	#endif
	#endif

	#ifdef SO_SF_MANUAL_CONTROL

		#ifdef SO_SF_LAYER3_ROTATEUV_ON
		SO_UV_LAYER3(i_v2f) = CalcLayerUVRotation ( SO_UV_LAYER3(i_v2f), LAYER3_TEX_ST, _Layer3Rotation, _Layer3RotationU, _Layer3RotationV );
		#endif

		SO_UV_LAYER3(i_v2f) += half2 ( _Layer3ScrollU, _Layer3ScrollV );
	#else
		SO_UV_LAYER3(i_v2f) += half2 (_Time.y * _ScrollU, _Time.y * _ScrollV);

		#if SO_SF_LAYER3_SCROLLUV_ON
		SO_UV_LAYER3(i_v2f) += CalcLayerUVScrollAuto( _Layer3ScrollU, _Layer3ScrollV );
		#endif

		#ifdef SO_SF_LAYER3_ROTATEUV_ON
		SO_UV_LAYER3(i_v2f) = CalcLayerUVRotationAuto ( SO_UV_LAYER3(i_v2f), LAYER3_TEX_ST, _Layer3Rotation, _Layer3RotationU, _Layer3RotationV );
		#endif

	#endif

	#ifdef SO_LAYER3_ANIMTYPE_RANDOM_UV
	#ifdef SO_MC_CONTROL_SCRIPT_ON
		SO_UV_LAYER3(i_v2f)   += half2 ( _Layer3AnimOffsetX, _Layer3AnimOffsetY );
	#else
	    SO_UV_LAYER3(i_v2f)   += half2 ( _SinTime.y*23.8192, _CosTime.y*33.1977 );
	#endif
	#endif

	#ifdef SO_LAYER3_CELLANIM_ON
		#ifdef SO_LAYER3_ANIMTYPE_CELL_ANIM_BLEND
		SO_BUV_LAYER3(i_v2f) = CellAnimUV ( SO_UV_LAYER3(i_v2f), _Layer2AnimCellsHorz, _Layer3AnimCellsVert, _Layer3AnimOffsetX2, _Layer3AnimOffsetY2, LAYER3_TEX_ST );
		#endif
		SO_UV_LAYER3(i_v2f) = CellAnimUV ( SO_UV_LAYER3(i_v2f), _Layer3AnimCellsHorz, _Layer3AnimCellsVert, _Layer3AnimOffsetX, _Layer3AnimOffsetY, LAYER3_TEX_ST );
	#endif

#endif
}

//=====================================================================================
// FRAG Functions
//=====================================================================================

/// LAYER 0 ( MainTex )
//-------------------------------------------------------------------------------------
inline void CalcFragBumpMapLayer0 ( inout SOLightingData i_sold, inout SOLayer i_layer )
{
#ifdef SO_SF_LAYER0_BUMP_ON
	half4 normalMap;

	#ifdef SO_SF_LAYER0_FLOWMAP_ON
		normalMap = Flowmap2DBump ( i_layer, LAYER0_BUMP );
	#else
		normalMap = tex2D ( LAYER0_BUMP, i_layer.uv );
	#endif

	i_sold.normalMap = normalMap;

#endif
}

//-------------------------------------------------------------------------------------
inline void GetTextureLayer0 ( inout SOLightingData i_sold, inout SOLayer i_layer, SOFLOAT4 i_layerColor, SOFLOAT4 i_flowColor )
{
#ifdef SO_LAYER0_ON
	#ifdef SO_SF_LAYER0_FLOWMAP_ON
		Flowmap2DTex ( i_layer, LAYER0_TEX, i_flowColor );
	#else
		i_layer.finalRGBA = tex2D ( LAYER0_TEX, i_layer.uv );

		#ifdef SO_LAYER0_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.finalRGBA = lerp ( i_layer.finalRGBA, tex2D ( LAYER0_TEX, i_layer.buv ), _Layer0AnimBlend );
		#endif

	#ifndef SO_ALPHA_MAP_EMPTY
		i_sold.alphaMap = i_layer.finalRGBA.a;
	#endif

		#ifdef SO_SF_RGBOFFSET_ON
		ChromaticAbberation ( i_sold, i_layer, LAYER0_TEX );
		#endif
	#endif

	i_layer.finalRGBA *= i_layerColor;
#endif
}

//-------------------------------------------------------------------------------------
inline void ProcessFragLayer0 ( inout SOLightingData i_sold, inout SOLayer i_layer, in v2f i_v2f )
{
#ifdef SO_LAYER0_ON

		i_layer.uv = SO_UV_LAYER0(i_v2f);

		#ifdef SO_LAYER0_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.buv = SO_BUV_LAYER0(i_v2f);
		#endif

	// only layer0 can have alpha map
	#ifdef SO_ALPHA_MAP_PARALLAX_HEIGHT
		i_layer.parallaxHeight = tex2D ( LAYER0_TEX, i_layer.uv ).a;
	#endif

	SurfacePropertiesInit ( i_layer, LAYER0_METALLIC, LAYER0_GLOSSINESS, _SpecColor );

	#ifdef SO_SF_LAYER0_SPECULAR_MAP_ON
		SpecularPropertiesFromMap ( i_layer, SO_UV_LAYER0(i_v2f), _SpecGlossMap );
	#endif

	#ifdef SO_SF_LAYER0_SURFACE_MAP_ON
		SurfacePropertiesFromMap ( i_layer, SO_UV_LAYER0(i_v2f), _MetallicGlossMap );
	#endif

	#if defined (SO_ALPHA_MAP_PARALLAX_HEIGHT) || \
	    ( defined (SO_SURFACE_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER0_SURFACE_MAP_ON) ) || \
		( defined (SO_SPECULAR_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER0_SPECULAR_MAP_ON) )
		CalcFragParallax ( i_layer, i_v2f.surfaceCoords, i_layer.parallaxHeight, _Parallax );
	#endif

		DistortionApplyToLayerUV ( i_sold, i_layer, _Layer0DistortionStrength );

	#ifdef SO_SF_LAYER0_FLOWMAP_ON
		Flowmap2DInit ( i_layer, _Layer0FlowMap, _Layer0FlowSpeed );
	#endif

		GetTextureLayer0 ( i_sold, i_layer, _Layer0Color, _Layer0FlowColor );
		CalcFragBumpMapLayer0 ( i_sold, i_layer );

	// GET ALPHA MAP VALUES IF THEY EXIST
		GetAlphaMap_PROGRESS_GRADIENT ( i_sold, i_layer.progress );

	#ifdef SO_SURFACE_VARS
		GetAlphaMap_METALLIC ( i_sold, i_sold.metallic );
		#ifdef SO_ALPHA_MAP_ROUGHNESS
		GetAlphaMap_ROUGHNESS ( i_sold, i_sold.perceptualRoughness );
		#else
		GetAlphaMap_SMOOTHNESS ( i_sold, i_sold.perceptualRoughness );
		#endif
	#endif


	#ifdef SO_TERRAIN_ON
		#ifdef SO_VERTEX_SPLAT
		i_sold.terrainControl = i_v2f.vertexSplatControl;
		#else
		i_sold.terrainControl = tex2D ( _Control, i_v2f.controlUV );
		#endif

		#ifdef SO_SF_LAYER0_BUMP_ON
		i_sold.normalMap *= i_sold.terrainControl.r;
		#endif
	#endif

		SurfacePropertiesSet ( i_sold, i_layer, _Layer0Fresnel, _BumpScale, _Layer0AOScale );

	#ifdef SO_LAYER0_ANIMTYPE_PROGRESS
    	CalcProgress ( i_sold, i_layer, _Layer0Progress ,_Layer0ProgressColor, _Layer0ProgressColorAmp, _Layer0ProgressEdge, 0 );
	#endif

		i_sold.finalRGBA   	= i_layer.finalRGBA;
#else
		i_sold.finalRGBA 	= _Layer0Color;
#endif
}

/// LAYER 1

//-------------------------------------------------------------------------------------
inline void CalcFragBumpMapLayer1 ( inout SOLightingData i_sold, inout SOLayer i_layer, half i_alpha )
{
#ifdef SO_SF_LAYER1_BUMP_ON
	half4 normalMap;
	half  blend;

	#ifdef SO_SF_LAYER1_FLOWMAP_ON
		normalMap = Flowmap2DBump ( i_layer, LAYER1_BUMP );
	#else
		normalMap = tex2D ( LAYER1_BUMP, i_layer.uv );
	#endif

	#ifdef SO_TERRAIN_ON
		i_sold.normalMap += normalMap * i_alpha;
	#else
		i_sold.normalMap = lerp ( i_sold.normalMap, normalMap, i_alpha );
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void GetTextureLayer1 ( inout SOLightingData i_sold, inout SOLayer i_layer, SOFLOAT4 i_layerColor, SOFLOAT4 i_flowColor )
{
#ifdef SO_LAYER1_ON
	#ifdef SO_SF_LAYER1_FLOWMAP_ON
		Flowmap2DTex ( i_layer, LAYER1_TEX, i_flowColor );
	#else
		i_layer.finalRGBA = tex2D ( LAYER1_TEX, i_layer.uv );

		#ifdef SO_LAYER1_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.finalRGBA = lerp ( i_layer.finalRGBA, tex2D ( LAYER1_TEX, i_layer.buv ), _Layer1AnimBlend );
		#endif

		#ifdef SO_SF_RGBOFFSET_ON
		ChromaticAbberation ( i_sold, i_layer, LAYER1_TEX );
		#endif
	#endif

	i_layer.finalRGBA *= i_layerColor;
#else
	i_layer.finalRGBA = i_layerColor;
#endif
}

//-------------------------------------------------------------------------------------
inline void ProcessFragLayer1 ( inout SOLightingData i_sold, inout SOLayer i_layer, in v2f i_v2f )
{
#ifdef SO_LAYER1_ON
	SOFLOAT alpha;

	 i_layer.uv = SO_UV_LAYER1(i_v2f);

	#ifdef SO_LAYER1_ANIMTYPE_CELL_ANIM_BLEND
	i_layer.buv = SO_BUV_LAYER1(i_v2f);
	#endif

	 SurfacePropertiesInit ( i_layer, LAYER1_METALLIC, LAYER1_GLOSSINESS, _Layer1SpecColor );

 	#ifdef SO_SF_LAYER0_SPECULAR_MAP_ON
		SpecularPropertiesFromMap ( i_layer, SO_UV_LAYER1(i_v2f), _SpecGlossMap );
	#endif

 	#ifdef SO_SF_LAYER1_SURFACE_MAP_ON
		SurfacePropertiesFromMap ( i_layer, SO_UV_LAYER1(i_v2f), _Layer1SurfaceMap );
	#endif

	#if ( defined (SO_SURFACE_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER1_SURFACE_MAP_ON) ) || \
		( defined (SO_SPECULAR_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER1_SPECULAR_MAP_ON) )
		CalcFragParallax ( i_layer, i_v2f.surfaceCoords, i_layer.parallaxHeight, _Layer1Parallax );
	#endif

		DistortionApplyToLayerUV ( i_sold, i_layer, _Layer1DistortionStrength );

	#ifdef SO_SF_LAYER1_FLOWMAP_ON
		Flowmap2DInit ( i_layer, _Layer1FlowMap, _Layer1FlowSpeed );
	#endif

		GetTextureLayer1 ( i_sold, i_layer, _Layer1Color, _Layer1FlowColor );

	#ifdef SO_TERRAIN_ON
		alpha = i_sold.terrainControl.g;
	#else
		alpha = i_layer.finalRGBA.a;
	#endif

		CalcFragBumpMapLayer1 ( i_sold, i_layer , alpha );
		SurfacePropertiesBlend ( i_sold, i_layer, alpha, _Layer1Fresnel, _Layer1BumpScale, _Layer1AOScale );

	#ifdef SO_LAYER1_ANIMTYPE_PROGRESS
    	CalcProgress ( i_sold, i_layer, _Layer1Progress ,_Layer1ProgressColor, _Layer1ProgressColorAmp, _Layer1ProgressEdge, 0 );
	#endif

#ifdef SO_PREMULTIPLY_ALPHA
		alpha *= i_sold.finalRGBA.a;
#endif

#ifdef SO_LAYER1_BLEND_TRANSPARENT
		i_sold.finalRGBA   	= lerp ( i_sold.finalRGBA, i_layer.finalRGBA, alpha ) ;
#endif

#ifdef SO_LAYER1_BLEND_ADDITIVE
		i_sold.finalRGBA  += i_layer.finalRGBA * alpha;
#endif

#ifdef SO_LAYER1_BLEND_SUBTRACTIVE
		i_sold.finalRGBA  -= i_layer.finalRGBA * alpha;
#endif

#ifdef SO_LAYER1_BLEND_MULTIPLY
		i_sold.finalRGBA  *= ( i_layer.finalRGBA * ColorSpaceDouble ) * alpha;
#endif

#endif
}

/// LAYER 2
//-------------------------------------------------------------------------------------
inline void CalcFragBumpMapLayer2 ( inout SOLightingData i_sold, inout SOLayer i_layer, in v2f i_v2f, SOFLOAT i_alpha )
{
#ifdef SO_SF_LAYER2_BUMP_ON
	half4 normalMap;

	#ifdef SO_SF_LAYER2_FLOWMAP_ON
		normalMap = Flowmap2DBump ( i_layer, LAYER2_BUMP );
	#else
		normalMap = tex2D ( LAYER2_BUMP, i_layer.uv );
	#endif

	#ifdef SO_TERRAIN_ON
		i_sold.normalMap += normalMap * i_alpha;
	#else
		i_sold.normalMap = lerp ( i_sold.normalMap, normalMap, i_alpha );
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void GetTextureLayer2 ( inout SOLightingData i_sold, inout SOLayer i_layer, SOFLOAT4 i_layerColor, SOFLOAT4 i_flowColor )
{
#ifdef SO_LAYER2_ON
	#ifdef SO_SF_LAYER2_FLOWMAP_ON
		Flowmap2DTex ( i_layer, LAYER2_TEX, i_flowColor );
	#else
		i_layer.finalRGBA = tex2D ( LAYER2_TEX, i_layer.uv );

		#ifdef SO_LAYER2_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.finalRGBA = lerp ( i_layer.finalRGBA, tex2D ( LAYER2_TEX, i_layer.buv ), _Layer2AnimBlend );
		#endif

		#ifdef SO_SF_RGBOFFSET_ON
		ChromaticAbberation ( i_sold, i_layer, LAYER2_TEX );
		#endif
	#endif

	i_layer.finalRGBA *= i_layerColor;
#else
	i_layer.finalRGBA = i_layerColor;
#endif
}

//-------------------------------------------------------------------------------------
inline void ProcessFragLayer2 ( inout SOLightingData i_sold, inout SOLayer i_layer, in v2f i_v2f )
{
#ifdef SO_LAYER2_ON
		SOFLOAT alpha;

		i_layer.uv = SO_UV_LAYER2(i_v2f);

		#ifdef SO_LAYER2_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.buv = SO_BUV_LAYER2(i_v2f);
		#endif

		SurfacePropertiesInit ( i_layer, LAYER2_METALLIC, LAYER2_GLOSSINESS, _Layer2SpecColor );

 	#ifdef SO_SF_LAYER0_SPECULAR_MAP_ON
		SpecularPropertiesFromMap ( i_layer, SO_UV_LAYER2(i_v2f), _SpecGlossMap );
	#endif

	#ifdef SO_SF_LAYER2_SURFACE_MAP_ON
		SurfacePropertiesFromMap ( i_layer, SO_UV_LAYER2(i_v2f), _Layer2SurfaceMap );
	#endif

	#if ( defined (SO_SURFACE_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER2_SURFACE_MAP_ON) ) || \
		( defined (SO_SPECULAR_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER2_SPECULAR_MAP_ON) )
		CalcFragParallax ( i_layer, i_v2f.surfaceCoords, i_layer.parallaxHeight, _Layer2Parallax );
	#endif

		DistortionApplyToLayerUV ( i_sold, i_layer, _Layer2DistortionStrength );

	#ifdef SO_SF_LAYER2_FLOWMAP_ON
		Flowmap2DInit ( i_layer, _Layer2FlowMap, _Layer2FlowSpeed );
	#endif

		GetTextureLayer2 ( i_sold, i_layer, _Layer2Color, _Layer2FlowColor );

   	#ifdef SO_TERRAIN_ON
   		alpha = i_sold.terrainControl.b;
   	#else
   		alpha = i_layer.finalRGBA.a;
   	#endif

		CalcFragBumpMapLayer2 ( i_sold, i_layer, i_v2f, alpha );
		SurfacePropertiesBlend ( i_sold, i_layer, alpha, _Layer2Fresnel, _Layer2BumpScale, _Layer2AOScale );

	#ifdef SO_LAYER2_ANIMTYPE_PROGRESS
    	CalcProgress ( i_sold, i_layer, _Layer2Progress ,_Layer2ProgressColor, _Layer2ProgressColorAmp, _Layer2ProgressEdge, 0 );
	#endif

#ifdef SO_PREMULTIPLY_ALPHA
		alpha *= i_sold.finalRGBA.a;
#endif

#ifdef SO_LAYER2_BLEND_TRANSPARENT
		i_sold.finalRGBA   	= lerp ( i_sold.finalRGBA, i_layer.finalRGBA, alpha );
#endif

#ifdef SO_LAYER2_BLEND_ADDITIVE
		i_sold.finalRGBA  += i_layer.finalRGBA * alpha;
#endif

#ifdef SO_LAYER2_BLEND_SUBTRACTIVE
		i_sold.finalRGBA  -= i_layer.finalRGBA * alpha;
#endif

#ifdef SO_LAYER2_BLEND_MULTIPLY
		i_sold.finalRGBA  *= ( i_layer.finalRGBA * ColorSpaceDouble ) * alpha;
#endif

#endif
}

/// LAYER 3
//-------------------------------------------------------------------------------------
inline void CalcFragBumpMapLayer3 ( inout SOLightingData i_sold, inout SOLayer i_layer, in v2f i_v2f, SOFLOAT i_alpha )
{
#ifdef SO_SF_LAYER3_BUMP_ON
	half4 normalMap;

	#ifdef SO_SF_LAYER3_FLOWMAP_ON
		normalMap = Flowmap2DBump ( i_layer, LAYER3_BUMP );
	#else
		normalMap = tex2D ( LAYER3_BUMP, i_layer.uv );
	#endif

	#ifdef SO_TERRAIN_ON
		i_sold.normalMap += normalMap * i_alpha;
	#else
		i_sold.normalMap = lerp ( i_sold.normalMap, normalMap, i_alpha );
	#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void GetTextureLayer3 ( inout SOLightingData i_sold, inout SOLayer i_layer, SOFLOAT4 i_layerColor, SOFLOAT4 i_flowColor )
{
#ifdef SO_LAYER3_ON
	#ifdef SO_SF_LAYER3_FLOWMAP_ON
		Flowmap2DTex ( i_layer, LAYER3_TEX, i_flowColor );
	#else
		i_layer.finalRGBA = tex2D ( LAYER3_TEX, i_layer.uv );

		#ifdef SO_LAYER3_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.finalRGBA = lerp ( i_layer.finalRGBA, tex2D ( LAYER3_TEX, i_layer.buv ), _Layer3AnimBlend );
		#endif

		#ifdef SO_SF_RGBOFFSET_ON
		ChromaticAbberation ( i_sold, i_layer, LAYER3_TEX );
		#endif
	#endif

	i_layer.finalRGBA *= i_layerColor;
#else
	i_layer.finalRGBA = i_layerColor;
#endif
}

//-------------------------------------------------------------------------------------
inline void ProcessFragLayer3 ( inout SOLightingData i_sold, inout SOLayer i_layer, in v2f i_v2f )
{
#ifdef SO_LAYER3_ON
		SOFLOAT alpha;

		i_layer.uv = SO_UV_LAYER3(i_v2f);

		#ifdef SO_LAYER3_ANIMTYPE_CELL_ANIM_BLEND
		i_layer.buv = SO_BUV_LAYER3(i_v2f);
		#endif

		SurfacePropertiesInit ( i_layer, LAYER3_METALLIC, LAYER3_GLOSSINESS, _Layer3SpecColor );

 	#ifdef SO_SF_LAYER3_SPECULAR_MAP_ON
		SpecularPropertiesFromMap ( i_layer, SO_UV_LAYER3(i_v2f), _Layer3SpecGlossMap );
	#endif

	#ifdef SO_SF_LAYER3_SURFACE_MAP_ON
		SurfacePropertiesFromMap ( i_layer, SO_UV_LAYER3(i_v2f), _Layer3SurfaceMap );
	#endif

	#if ( defined (SO_SURFACE_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER3_SURFACE_MAP_ON) ) || \
		( defined (SO_SPECULAR_MAP_PARALLAX_HEIGHT) && defined (SO_SF_LAYER3_SPECULAR_MAP_ON) )
		CalcFragParallax ( i_layer, i_v2f.surfaceCoords, i_layer.parallaxHeight, _Layer3Parallax );
	#endif

		DistortionApplyToLayerUV ( i_sold, i_layer, _Layer3DistortionStrength );

	#ifdef SO_SF_LAYER3_FLOWMAP_ON
		Flowmap2DInit ( i_layer, _Layer3FlowMap, _Layer3FlowSpeed );
	#endif

		GetTextureLayer3 ( i_sold, i_layer, _Layer3Color, _Layer3FlowColor );

   	#ifdef SO_TERRAIN_ON
   		alpha = i_sold.terrainControl.a;
   	#else
   		alpha = i_layer.finalRGBA.a;
   	#endif

		CalcFragBumpMapLayer3 ( i_sold, i_layer, i_v2f, alpha );
		SurfacePropertiesBlend ( i_sold, i_layer, alpha, _Layer3Fresnel, _Layer3BumpScale, _Layer3AOScale );

	#ifdef SO_LAYER3_ANIMTYPE_PROGRESS
    	CalcProgress ( i_sold, i_layer, _Layer3Progress ,_Layer3ProgressColor, _Layer3ProgressColorAmp, _Layer3ProgressEdge, 0 );
	#endif

#ifdef SO_PREMULTIPLY_ALPHA
		alpha *= i_sold.finalRGBA.a;
#endif

#ifdef SO_LAYER3_BLEND_TRANSPARENT
		i_sold.finalRGBA = lerp ( i_sold.finalRGBA, i_layer.finalRGBA, alpha );
#endif

#ifdef SO_LAYER3_BLEND_ADDITIVE
		i_sold.finalRGBA  += i_layer.finalRGBA * alpha;
#endif

#ifdef SO_LAYER3_BLEND_SUBTRACTIVE
		i_sold.finalRGBA  -= i_layer.finalRGBA * alpha;
#endif

#ifdef SO_LAYER3_BLEND_MULTIPLY
		i_sold.finalRGBA  *= ( i_layer.finalRGBA * ColorSpaceDouble ) * alpha;
#endif

#endif
}

//-------------------------------------------------------------------------------------
inline void LayersFinalize ( inout SOLightingData i_sold, in v2f i_v2f )
{
#ifdef SO_BUMP_ON
	half3 tnormal = UnpackNormal ( i_sold.normalMap );
	half3 normalBump;

#ifdef SO_GD_NORMAL_DIRECTX
	tnormal.g = 1.0 - tnormal.g;
#endif

	normalBump.x = dot ( i_v2f.tspace0, tnormal );
	normalBump.y = dot ( i_v2f.tspace1, tnormal );
	normalBump.z = dot ( i_v2f.tspace2, tnormal );

#ifdef SO_GD_BUMP_SCALE_GLOBAL
	i_sold.worldNormal = lerp ( i_sold.worldNormal, normalize ( normalBump ), _BumpScale );
#elif defined(SO_GD_BUMP_SCALE_PER_LAYER)
	i_sold.worldNormal = lerp ( i_sold.worldNormal, normalize ( normalBump ), i_sold.bumpScale );
#else
	i_sold.worldNormal = normalize ( normalBump );
#endif

#ifdef SO_GD_BUMP_SCALE_PER_LAYER
#endif

#ifdef SO_GD_AO_SCALE_GLOBAL
	i_sold.ambientOcclusion *= _Layer0AOScale;
#endif

#if ( defined (SO_SURFACE_MAP_ON) && defined (SO_SURFACE_MAP_AMBIENT_OCCLUSION) ) || defined (SO_ALPHA_MAP_AMBIENT_OCCLUSION)
#ifdef SO_GD_AO_SCALE_PER_LAYER
	i_sold.ambientOcclusion *= i_sold.aoScale;
#endif
#endif


#endif

}


