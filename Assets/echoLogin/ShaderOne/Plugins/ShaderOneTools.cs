#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using ShaderOneSystem;

public class ShaderOneTools
{
	private static ShaderOneSettings	_settings;

	//=====================================================================
	private static void SetShaderFeature ( Material i_mat, string i_name, bool i_flag )
	{
		if ( i_flag )
		  i_mat.EnableKeyword(i_name);
	  else
		  i_mat.DisableKeyword(i_name);
	}

	//=====================================================================
	private static void BinaryToShaderFeature ( Material i_mat, string i_prefix, int i_binNum, int i_numBits = 3 )
	{
		byte  		byteNum = (byte)i_binNum;
		string  	keyword;
		int 		loop;
		int 		num;
		bool        bit;

		for ( loop = 0; loop < i_numBits; loop++ )
		{
			bit = ( byteNum & ( 1 << loop ) ) != 0;

			num = loop+1;
			keyword = i_prefix + "_BIT" + num;

			if ( bit )
				i_mat.EnableKeyword(keyword);
			else
				i_mat.DisableKeyword(keyword);
		 }
	}

	//=====================================================================
	public static void SetBlendMode ( Material i_mat, BLENDMODE i_bm )
	{
		i_mat.SetInt ("_Mode", (int)i_bm );

		switch ( i_bm )
		{
		default:
			i_mat.SetOverrideTag("RenderType", "");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );

			i_mat.SetInt ("_ZWrite", 1 );
			i_mat.renderQueue = -1;
			break;

		case BLENDMODE.CUTOUT:
			i_mat.SetOverrideTag("RenderType", "TransparentCutout");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );

			i_mat.SetInt ("_ZWrite", 1 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
			break;

		case BLENDMODE.FADE:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );

			i_mat.SetInt ("_ZWrite", 0 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			break;

		case BLENDMODE.TRANSPARENT:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );

			i_mat.SetInt ("_ZWrite", 0 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			break;

		case BLENDMODE.ADDITIVE:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );

			i_mat.SetInt ("_ZWrite", 0 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			break;

		case BLENDMODE.ADDITIVE_SOFT:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor );

			i_mat.SetInt ("_ZWrite", 0 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			break;

		case BLENDMODE.ADDITIVE_BLEND:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );

			i_mat.SetInt ("_ZWrite", 0 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			break;

		case BLENDMODE.ADDITIVE_ALPHA:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.One );

			i_mat.SetInt ("_ZWrite", 0 );
			i_mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
			break;

		case BLENDMODE.MULTIPLY:
			i_mat.SetOverrideTag("RenderType", "Transparent");
			i_mat.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero );
			i_mat.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcColor );

			i_mat.SetInt ("_SrcBlendAddPass", (int)UnityEngine.Rendering.BlendMode.Zero );
			i_mat.SetInt ("_DstBlendAddPass", (int)UnityEngine.Rendering.BlendMode.SrcColor );

			i_mat.SetInt ("_ZWrite", 0 );
			break;
		}

		BinaryToShaderFeature ( i_mat, "SO_SF_BLEND", (int)i_bm );
	}

	//=====================================================================
	private static bool GetMaterialBoolLayer ( Material i_mat, string i_propName, int i_index )
	{
		float value;

		string fullName = "_Layer" + i_index + i_propName;

		value = i_mat.GetFloat ( fullName );

		return ( value > 0.0001f ? true : false );
	}

	//=====================================================================
	private static float GetMaterialFloatLayer ( Material i_mat, string i_propName, int i_index )
	{
		float value;

		string fullName = "_Layer" + i_index + i_propName;

		value = i_mat.GetFloat ( fullName );

		return ( value );
	}

	//=====================================================================
	private static bool IsUnityTerrainOn ( Material i_mat )
	{
		return ( _settings.terrainType == TERRAIN_TYPE.UNITY_TERRAIN && i_mat.GetFloat ("_TerrainToggle") > 0f );
	}

	//=====================================================================
	private static void LayerKeywords ( Material i_mat, int i_index )
	{
		ShaderOneLayerSettings layerSettings 	= _settings.layer0;
		string layerName 						= "LAYER" + i_index;
		string layerVarName 					= "Layer" + i_index;
		//string texName 							= "_" + layerVarName + "Tex";
		string bumpName 						= "_" + layerVarName + "BumpMap";
		string surfaceMapName 					= "_" + layerVarName + "SurfaceMap";
		string specularMapName 					= "_" + layerVarName + "SpecGlossMap";

		switch ( i_index )
		{
		case 0:
			layerSettings = _settings.layer0;
			//texName 		= "_MainTex";
			bumpName 		= "_BumpMap";
			surfaceMapName 	= "_MetallicGlossMap";
			specularMapName = "_SpecGlossMap";
			break;

		case 1:
			layerSettings = _settings.layer1;
			break;

		case 2:
			layerSettings = _settings.layer2;
			break;

		case 3:
			layerSettings = _settings.layer3;
			break;
		}

		if ( layerSettings.blendModes )
		{
			BinaryToShaderFeature ( i_mat, "SO_SF_" + layerName + "_BLEND", (int)GetMaterialFloatLayer ( i_mat, "BlendMode", i_index ), 2 );
		}

		//		if ( !layerOn )
		//			layerOn = layerSettings.enabled && layerSettings.normalMap && i_mat.GetTexture( bumpName ) != null;

		// texture
		bool layerOn = layerSettings.enabled && GetMaterialBoolLayer ( i_mat, "Toggle", i_index );

		if ( i_index == 0 )
			layerOn = true;

		SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_ON", layerOn );

		if ( layerOn )
		{
			if ( IsUnityTerrainOn(i_mat) )
			{
				// BumpMap
				SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_BUMP_ON", layerOn );
			}
			else
			{
				if ( layerSettings.uv2 == UV2_OPTION.MATERIAL_SELECTABLE && (int)GetMaterialFloatLayer ( i_mat, "UV2", i_index ) > 0 )
					SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_UV2_ON", layerOn );

				// BumpMap
				SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_BUMP_ON", layerOn && layerSettings.normalMap && i_mat.GetTexture( bumpName ) != null );

					if ( layerSettings.animOptions )
						BinaryToShaderFeature ( i_mat, "SO_SF_" + layerName + "_ANIMTYPE", (int)i_mat.GetFloat ("_" + layerVarName + "AnimType") );

					bool flag = false;

					if ((UVCONTROL)i_mat.GetFloat ("_ManualControl") == UVCONTROL.MANUAL )
						flag = true;

					// ScrollUV
					if ( layerSettings.scrollUV )
					{
						SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_SCROLLUV_ON", layerSettings.enabled &&
											 ( i_mat.GetFloat( "_" + layerVarName + "ScrollU" ) != 0 || i_mat.GetFloat( "_" + layerVarName + "ScrollV" ) != 0 ) || flag );
					}

					// ROTATION
					if ( layerSettings.rotateUV )
						SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_ROTATEUV_ON", ( layerSettings.enabled && layerSettings.rotateUV && i_mat.GetFloat( "_" + layerVarName + "Rotation" ) != 0 ) || flag );
			}
    	}
		else
		{
			if ( layerSettings.animOptions )
				BinaryToShaderFeature ( i_mat, "SO_SF_" + layerName + "_ANIMTYPE", 0 );

			if ( layerSettings.scrollUV )
				SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_SCROLLUV_ON", false );

			if ( layerSettings.rotateUV )
				SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_ROTATEUV_ON", false );
		}


		// specularmap
		if ( _settings.workflow == WORKFLOW_MODE.SPECULAR )
			SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_SPECULAR_MAP_ON", layerOn && i_mat.GetTexture( specularMapName ) != null );

		// surfacemap
		if ( layerSettings.surfaceMap )
			SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_SURFACE_MAP_ON", layerOn && i_mat.GetTexture( surfaceMapName ) != null );

		// Flow Map
		if ( layerSettings.flowMap )
			SetShaderFeature ( i_mat, "SO_SF_"+ layerName + "_FLOWMAP_ON", layerOn && i_mat.GetTexture( "_" + layerVarName + "FlowMap" ) != null );
	}

	//------------------------------------------------------------------------
	private static void FogKeywords ( Material i_mat )
	{
		if ( _settings.fogMode != FOG_MODE.OFF )
			SetShaderFeature ( i_mat, "SO_SF_FOG_ON", _settings.fogMode != FOG_MODE.OFF && i_mat.GetFloat("_FogToggle") > 0.001f  );
	}

	//=====================================================================
	private static void TerrainKeywords ( Material i_mat )
	{
		switch (_settings.terrainType)
		{
		case TERRAIN_TYPE.NONE:
			SetShaderFeature ( i_mat, "SO_SF_MESH_TERRAIN_ON", false );
			SetShaderFeature ( i_mat, "SO_SF_UNITY_TERRAIN_ON", false );
			break;

		case TERRAIN_TYPE.MESH_TERRAIN:
			SetShaderFeature ( i_mat, "SO_SF_MESH_TERRAIN_ON", i_mat.GetFloat ("_TerrainToggle") > 0f );
			SetShaderFeature ( i_mat, "SO_SF_UNITY_TERRAIN_ON", false );
			break;

		case TERRAIN_TYPE.UNITY_TERRAIN:
			SetShaderFeature ( i_mat, "SO_SF_MESH_TERRAIN_ON", false );
			SetShaderFeature ( i_mat, "SO_SF_UNITY_TERRAIN_ON", i_mat.GetFloat ("_TerrainToggle") > 0f );
			break;
		}
	}

	//------------------------------------------------------------------------
	private static void WorldWarpKeywords ( Material i_mat )
	{
		if ( _settings.bendType != BEND_TYPE.OFF )
			SetShaderFeature ( i_mat, "SO_SF_BENDING_ON", i_mat.GetFloat("_SO_BendToggle") > 0.001f  );
	}

	//------------------------------------------------------------------------
	private static void LightingKeywords( Material i_mat )
	{
		if ( _settings.renderPipeline != RENDER_PIPELINE.UNLIT )
		{
			if ( _settings.pbr && i_mat.GetFloat("_SO_PBRToggle") > 0.001f )
			{
				i_mat.EnableKeyword("SO_SF_PBR_ON");
			}
			else
				i_mat.DisableKeyword("SO_SF_PBR_ON");

			if ( _settings.specularMode != SPECULAR_MODE.OFF && i_mat.GetFloat("_SpecularHighlights") > 0.001f )
				i_mat.EnableKeyword("SO_SF_SPECULAR_ON");
			else
				i_mat.DisableKeyword("SO_SF_SPECULAR_ON");

				BinaryToShaderFeature ( i_mat, "SO_SF_LIGHTING", i_mat.GetInt("_LightMode"), 2 );

				SetShaderFeature ( i_mat, "SO_SF_LIGHT_PROBES_ON", i_mat.GetFloat( "_LightProbes" ) > 0.001f );
		}
		else
		{
			i_mat.DisableKeyword("SO_SF_PBR_ON");
			BinaryToShaderFeature ( i_mat, "SO_SF_LIGHTING", 0, 2 );
			i_mat.DisableKeyword("SO_SF_SPECULAR_ON");
		}

		if ( _settings.renderPipeline == RENDER_PIPELINE.UNITY_FORWARD &&
			 (LIGHTINGMODE)i_mat.GetInt("_LightMode") == LIGHTINGMODE.UNLIT )
				i_mat.SetShaderPassEnabled("ForwardAdd", false );
			else
				i_mat.SetShaderPassEnabled("ForwardAdd", true );
	}

	//------------------------------------------------------------------------
	private static void ReflectionKeywords ( Material i_mat )
	{
		if ( _settings.reflectionType != REFLECTION_TYPE.NONE && i_mat.GetFloat("_GlossyReflections") > 0.001f )
		{
			if ( _settings.reflectionType == REFLECTION_TYPE.REFLECTION_PROBE )
			{
				i_mat.EnableKeyword("SO_SF_REFLECT_PROBE_ON");
				i_mat.DisableKeyword("SO_SF_REFLECT_2D_ON");
			}
			else
			{
				i_mat.DisableKeyword("SO_SF_REFLECT_PROBE_ON");
				i_mat.EnableKeyword("SO_SF_REFLECT_2D_ON");
			}
		}
		else
		{
			i_mat.DisableKeyword("SO_SF_REFLECT_PROBE_ON");
			i_mat.DisableKeyword("SO_SF_REFLECT_2D_ON");
		}
	}

	//------------------------------------------------------------------------
	private static void UVWorldMapKeywords ( Material i_mat )
	{
		if ( _settings.uvWorldMap )
			BinaryToShaderFeature ( i_mat, "SO_SF_UV1_WORLDMAP", (int)i_mat.GetFloat("_SO_UV1_WorldMap"), 2 );
		else
			BinaryToShaderFeature ( i_mat, "SO_SF_UV1_WORLDMAP", 0, 2 );
	}

	//------------------------------------------------------------------------
	private static void DistortionKeywords ( Material i_mat )
	{
		bool toggle = i_mat.GetFloat( "_DistortToggle" ) > 0.001 ? true : false;

		SetShaderFeature ( i_mat, "SO_SF_DISTORT_HORZ_ON", _settings.distortion && toggle && i_mat.GetFloat( "_DistortHorzStrength" ) > 0.001f );
		SetShaderFeature ( i_mat, "SO_SF_DISTORT_VERT_ON", _settings.distortion && toggle && i_mat.GetFloat( "_DistortVertStrength" ) > 0.001f );
		SetShaderFeature ( i_mat, "SO_SF_DISTORT_CIRCULAR_ON", _settings.distortion && toggle && i_mat.GetFloat( "_DistortCircularStrength" ) > 0.001f );
	}

	//------------------------------------------------------------------------
	private static void EmissionKeywords ( Material i_mat )
	{
		SetShaderFeature ( i_mat, "_EMISSION", i_mat.GetFloat( "_EmissionToggle" ) > 0.001f && _settings.emission );
		MaterialEditor.FixupEmissiveFlag( i_mat );
	}

	//------------------------------------------------------------------------
	private static void ChromaticAbberationKeywords( Material i_mat )
	{
		SetShaderFeature ( i_mat, "SO_SF_RGBOFFSET_ON", i_mat.GetFloat( "_RGBOffsetToggle" ) > 0.001f && _settings.chromaticAbberation );
	}

	//------------------------------------------------------------------------
	private static void SaturationKeywords( Material i_mat )
	{
		SetShaderFeature ( i_mat, "SO_SF_SATURATION_ON", i_mat.GetFloat( "_SaturationToggle" ) > 0.001f && _settings.saturation );
	}

	//------------------------------------------------------------------------
	private static void ScanlinesKeywords ( Material i_mat )
	{
		SetShaderFeature ( i_mat, "SO_SF_SCANLINE_ON", i_mat.GetFloat( "_ScanlineToggle" ) > 0.001f && _settings.scanlines );
	}

	//------------------------------------------------------------------------
	private static void IntersectKeywords( Material i_mat )
	{
		SetShaderFeature ( i_mat, "SO_SF_INTERSECT_ON", i_mat.GetFloat( "_IntersectToggle" ) > 0.001f && _settings.intersect );
	}

	//------------------------------------------------------------------------
	private static void MapKeywords( Material i_mat )
	{
		BinaryToShaderFeature ( i_mat, "SO_SF_ALPHA_MAP", (int)i_mat.GetFloat("_AlphaMapType"), 3 );

		if ( _settings.layer0.surfaceMap || _settings.layer1.surfaceMap || _settings.layer2.surfaceMap || _settings.layer3.surfaceMap )
		{
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPR", (int)i_mat.GetFloat("_SurfaceMapTypeR"), 3 );
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPG", (int)i_mat.GetFloat("_SurfaceMapTypeG"), 3 );
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPB", (int)i_mat.GetFloat("_SurfaceMapTypeB"), 3 );
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPA", (int)i_mat.GetFloat("_SurfaceMapTypeA"), 3 );
		}
		else
		{
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPR", 0, 3 );
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPG", 0, 3 );
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPB", 0, 3 );
			BinaryToShaderFeature (i_mat, "SO_SF_SURFACE_MAPA", 0, 3 );
		}

		if ( _settings.workflow == WORKFLOW_MODE.SPECULAR)
			BinaryToShaderFeature ( i_mat, "SO_SF_SPECULAR_MAP", (int)i_mat.GetFloat("_SpecularMapType"), 3 );
		else
			BinaryToShaderFeature ( i_mat, "SO_SF_SPECULAR_MAP", 0, 3 );
	}


	//------------------------------------------------------------------------
	private static void RimLightKeywords ( Material i_mat )
	{
		if ( _settings.rimLighting && i_mat.GetFloat("_RimToggle") > 0.001f )
		{
			switch ((RIMLITBLEND)i_mat.GetFloat("_RimBlend") )
			{
			case RIMLITBLEND.ADDITIVE:
				i_mat.EnableKeyword("SO_SF_RIMLIT_ADD");
				i_mat.DisableKeyword("SO_SF_RIMLIT_SUBTRACT");
				break;

			case RIMLITBLEND.SUBTRACTIVE:
				i_mat.DisableKeyword("SO_SF_RIMLIT_ADD");
				i_mat.EnableKeyword("SO_SF_RIMLIT_SUBTRACT");
				break;
			}
		}
		else
		{
			i_mat.DisableKeyword("SO_SF_RIMLIT_SUBTRACT");
			i_mat.DisableKeyword("SO_SF_RIMLIT_ADD");
		}
	}

	//=====================================================================
	public static void ProcessMaterialKeywords ( Material i_mat, bool i_loadSettings = true )
	{
		if ( i_loadSettings )
			ShaderOneIO.LoadSettings( ref _settings );

		SetShaderFeature (i_mat, "SO_SF_NORMAL_FIX", (UnityEngine.Rendering.CullMode)i_mat.GetFloat("_MyCullMode") != UnityEngine.Rendering.CullMode.Back);
		SetShaderFeature (i_mat , "SO_SF_MANUAL_CONTROL", i_mat.GetFloat("_ManualControl") > 0.001f );
		BinaryToShaderFeature ( i_mat, "SO_SF_VERTEX", (int)i_mat.GetFloat("_VertexColorMode"), 2 );

		SetBlendMode ( i_mat, (BLENDMODE)i_mat.GetInt ("_Mode") );

		LayerKeywords (i_mat, 0 );
		LayerKeywords (i_mat, 1 );
		LayerKeywords (i_mat, 2 );
		LayerKeywords (i_mat, 3 );

		UVWorldMapKeywords ( i_mat );
		WorldWarpKeywords ( i_mat );

		TerrainKeywords ( i_mat );

		LightingKeywords(i_mat);
		ReflectionKeywords(i_mat);
		DistortionKeywords(i_mat);
		EmissionKeywords(i_mat);
		ChromaticAbberationKeywords(i_mat);
		SaturationKeywords(i_mat);
		ScanlinesKeywords(i_mat);
		RimLightKeywords(i_mat);
		IntersectKeywords(i_mat);
		MapKeywords(i_mat);
		FogKeywords ( i_mat );
	}

	//=====================================================================
	public static void GenerateShader( bool i_reimportMaterials = false )
	{
		ShaderOneSettings settings = new ShaderOneSettings();
		ShaderOneVersion sov = ShaderOneIO.LoadVersionSOV();

		ShaderOneIO.LoadSettings(ref settings);
		ShaderOneParse.ShaderOneMake ( settings );
		AssetDatabase.Refresh();

		string path = ShaderOneIO.FindAsset ("ShaderOneGen.shader");
		if ( path != null )
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );

		if ( i_reimportMaterials )
			ShaderOneTools.ReimportMaterials();

  		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

 		ShaderOneIO.SaveVersion(sov, true);
	}

	//=====================================================================
	public static void ReimportMaterials()
	{
		int loop;
		string[] dep;
		Material mat;

		ShaderOneIO.LoadSettings( ref _settings );

		string shaderPath = ShaderOneIO.FindAsset ("ShaderOneGen.shader");

		string[] allMaterials = AssetDatabase.FindAssets("t:Material");

		for ( loop = 0; loop < allMaterials.Length; loop++ )
		{
			allMaterials[loop] = AssetDatabase.GUIDToAssetPath(allMaterials[loop]);

			dep = AssetDatabase.GetDependencies(allMaterials[loop]);

            if ( ArrayUtility.Contains ( dep, shaderPath ) )
			{
				mat = (Material)AssetDatabase.LoadAssetAtPath ( allMaterials[loop], typeof(Material));
				if ( mat != null )
				{
#if SHADERONE_DEBUG
					Debug.Log("Name="+mat.name);
#endif
					ProcessMaterialKeywords ( mat, false );
				}
			}
		}

		AssetDatabase.SaveAssets();
	}

}
#endif

