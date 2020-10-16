// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

using System;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using ShaderOneSystem;

/*
	things that should be off in simple unlit

    ** lighting
    spec
    reflection
    surface properties

    ** layers
    normal map
	metallic
	smoothness
	fresnel


Things that should be off when Unity terrain is on

Cell Animation
layer UV stuff

*/

 	public class SOMaterialProperty
 	{
		MaterialProperty			mp;
 		MaterialProperty.PropType	ptype;
 		int 						id;
 		string 						name;
 		float 						floatValue;
 		Texture2D 					textureValue;
 		Vector4     				vectorValue;
 		Color       				colorValue;
 		float       				min;
 		float       				max;
 	}

//namespace UnityEditor
	public class LayerMaterialProperties
	{
		public string           prefix;
		public string           name;
		public MaterialProperty fold;
		public MaterialProperty uv2;
		public MaterialProperty toggle;
		public MaterialProperty tex;
		public MaterialProperty bumpMap;
		public MaterialProperty bumpScale;
		public MaterialProperty aoScale;
		public MaterialProperty blendMode;
		public MaterialProperty smoothness;
		public MaterialProperty metallic;
		public MaterialProperty parallaxHeight;
		public MaterialProperty fresnel;
		public MaterialProperty specularColor;
		public MaterialProperty specularMap;
		public MaterialProperty surfaceMap;
		public MaterialProperty flowMap;
		public MaterialProperty flowSpeed;
		public MaterialProperty flowColor;
		public MaterialProperty scrollSpeedU;
		public MaterialProperty scrollSpeedV;
//		public MaterialProperty rotationMode;
		public MaterialProperty rotationSpeed;
		public MaterialProperty rotationU;
		public MaterialProperty rotationV;
		public MaterialProperty color;
		public MaterialProperty animType;
		public MaterialProperty animActive;
		public MaterialProperty progressEdge;
		public MaterialProperty progressColor;
		public MaterialProperty progressColorAmp;
		public MaterialProperty progress;
		public MaterialProperty animCellsHorz;
		public MaterialProperty animCellsVert;
		public MaterialProperty animFPS;
		public MaterialProperty animLoopMode;
		public MaterialProperty animCellStart;
		public MaterialProperty animCellEnd;
		public MaterialProperty distortionStrength;
	}

	public class PreviousShaderSettings
	{
		public bool                 apply;
		public LIGHTINGMODE         lightMode;
		public BLENDMODE  			blendMode;
		public WORKFLOW_MODE	    workFlow;
		public Texture  			surfaceMap;
		public Texture  			specularMap;
		public float      			metallic;
		public float      			smoothness;
		public float      			fresnel;
		public MAP_CHANNEL 			alphaMapType;
		public MAP_CHANNEL 			surfaceR;
		public MAP_CHANNEL 			surfaceG;
		public MAP_CHANNEL 			surfaceB;
		public MAP_CHANNEL 			surfaceA;
		public Color                specColor;
		public Color                color;
		public bool                 emission;
	}

	public class ShaderOneMaterialEditor : ShaderGUI
	{
		public static int	        reloadSettings 		= 0;
		public static bool          resetSurfaceMapR 	= false;
		public static bool          resetSurfaceMapG 	= false;
		public static bool          resetSurfaceMapB 	= false;
		public static bool          resetSurfaceMapA 	= false;
		private ShaderOneSettings	_settings;
		private bool                _settingsLoaded  	= false;
		private bool				_controlScriptOn 	= false;
		MaterialEditor 				materialEditor;
		MaterialProperty[] 			properties;
		Material 					targetMat;
		bool                        init = true;
		int textureCount 	= 0;
		int texcoordCount   = 0;
		int effectCount    	= 0;

		LayerMaterialProperties[] layers;

		MaterialProperty inputLayerFlag;

		MaterialProperty controlScriptFlag = null;
		MaterialProperty myCullMode;
		MaterialProperty blendMode;
		MaterialProperty specularMapType;
		MaterialProperty alphaMapType;
		MaterialProperty surfaceMapTypeR;
		MaterialProperty surfaceMapTypeG;
		MaterialProperty surfaceMapTypeB;
		MaterialProperty surfaceMapTypeA;
		MaterialProperty vertexColorMode;

		MaterialProperty specularHighlights;
//		MaterialProperty specularColor;

		MaterialProperty moveSpeed;
		MaterialProperty moveStrength;
		MaterialProperty moveDirection;

		MaterialProperty emissionFold;
		MaterialProperty emissionToggle;
		MaterialProperty emissionColor;
		MaterialProperty emissionMap;
		MaterialProperty emissionAffectObject;

		MaterialProperty optionsFold;

		MaterialProperty lightFold;
		MaterialProperty lightMode;
		MaterialProperty pbrToggle;
//		MaterialProperty lightProbes;

//		MaterialProperty parallax;

		MaterialProperty mainFold;
		MaterialProperty layerFold;

//		MaterialProperty smoothness;
//		MaterialProperty metallic;
//		MaterialProperty fresnel;
//		MaterialProperty specular;

		MaterialProperty colorFront;
		MaterialProperty colorBack;
		MaterialProperty colorAmplify;

		MaterialProperty uvWorldMap;
		MaterialProperty uvWorldMapScale;

		MaterialProperty saturationFold;
		MaterialProperty saturationToggle;
		MaterialProperty saturationStrength;

		MaterialProperty intersectFold;
		MaterialProperty intersectToggle;
		MaterialProperty intersectColor;
		MaterialProperty intersectThreshold;

		MaterialProperty manualControl;
		MaterialProperty scrollU;
		MaterialProperty scrollV;

		MaterialProperty scanlineFold;
		MaterialProperty scanlineToggle;
		MaterialProperty scanlineStrengthHorz;
		MaterialProperty scanlineScrollHorz;
		MaterialProperty scanlineCountHorz;
		MaterialProperty scanlineWidthHorz;
		MaterialProperty scanlineStrengthVert;
		MaterialProperty scanlineScrollVert;
		MaterialProperty scanlineCountVert;
		MaterialProperty scanlineWidthVert;

		MaterialProperty reflectToggle;
		MaterialProperty reflectTex;

		MaterialProperty rimFold;
		MaterialProperty rimToggle;
		MaterialProperty rimColor;
		MaterialProperty rimWidth;
		MaterialProperty rimBlend;

//		MaterialProperty fogFold;
		MaterialProperty bendToggle;
		MaterialProperty fogToggle;
		MaterialProperty terrainToggle;
		MaterialProperty terrainControl;

		MaterialProperty distortFold;
		MaterialProperty distortToggle;

		MaterialProperty distortHorzCount;
		MaterialProperty distortHorzSpeed;
		MaterialProperty distortHorzStrength;
		MaterialProperty distortHorzWave;

		MaterialProperty distortVertCount;
		MaterialProperty distortVertSpeed;
		MaterialProperty distortVertStrength;
		MaterialProperty distortVertWave;

		MaterialProperty distortCircularCount;
		MaterialProperty distortCircularSpeed;
		MaterialProperty distortCircularStrength;
		MaterialProperty distortCircularCenterU;
		MaterialProperty distortCircularCenterV;

		MaterialProperty rgbOffsetFold;
		MaterialProperty rgbOffsetToggle;
		MaterialProperty rgbOffsetStrength;
		MaterialProperty rgbOffsetAmount;
		MaterialProperty rgbOffsetMode;

		//-------------------------------------------------------------------------
		public static bool IsForwardRendering ( )
		{
			// if ( TierSettings.renderingPath != RenderingPath.Forward )
			// return(false);

			if ( Camera.main.actualRenderingPath != RenderingPath.Forward && Camera.main.actualRenderingPath != RenderingPath.UsePlayerSettings )
				return(false);

			return ( true );
		}

		//=====================================================================
		public void SetSurfaceMap ( Texture2D i_tex, bool i_sRGB )
		{
			string path = AssetDatabase.GetAssetPath( i_tex );
			TextureImporter A = (TextureImporter) AssetImporter.GetAtPath( path );
			if ( A.sRGBTexture != i_sRGB )
			{
				A.sRGBTexture = i_sRGB;
				AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
			}
		}

		//=====================================================================
		public void CheckSurfaceMap ( Texture2D i_tex, Texture2D i_texOld, SURFACE_MAP_IMPORT i_smi )
		{
			if ( i_tex != null && i_tex != i_texOld )
			{
				switch ( i_smi )
				{
				case SURFACE_MAP_IMPORT.MANUAL:
					break;

				case SURFACE_MAP_IMPORT.FORCE_SRGB:
					SetSurfaceMap ( i_tex, true );
					break;

				case SURFACE_MAP_IMPORT.FORCE_LINEAR:
					SetSurfaceMap ( i_tex, false );
					break;
				}
			}
		}

		//=====================================================================
		public float GetDefautShaderFloat ( Material i_mat, string i_name, float i_default = 0 )
		{
			float rval = i_default;

			if ( i_mat.HasProperty( i_name ) )
			{
				rval = i_mat.GetFloat ( i_name );
			}

			return ( rval );
		}

		//=====================================================================
		private BLENDMODE ChooseBestBlendMode ( BLENDMODE i_originalBlend )
		{
			BLENDMODE bm = BLENDMODE.SOLID;

			switch ( i_originalBlend )
			{
			case BLENDMODE.SOLID:
			case BLENDMODE.CUTOUT:
			case BLENDMODE.MULTIPLY:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
					bm = i_originalBlend;
				break;

			case BLENDMODE.TRANSPARENT:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
				{
					bm = i_originalBlend;
				}
				else
				{
					if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.FADE ) )
						bm = BLENDMODE.FADE;
				}
				break;

			case BLENDMODE.FADE:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
				{
					bm = i_originalBlend;
				}
				else
				{
					if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.TRANSPARENT ) )
						bm = BLENDMODE.TRANSPARENT;
				}
				break;

			case BLENDMODE.ADDITIVE:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
				{
					bm = i_originalBlend;
				}
				else
				{
					if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE_BLEND ) )
						bm = BLENDMODE.ADDITIVE_BLEND;
					else
					{
						if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE_SOFT ) )
							bm = BLENDMODE.ADDITIVE_SOFT;
					}
				}
				break;

			case BLENDMODE.ADDITIVE_SOFT:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
				{
					bm = i_originalBlend;
				}
				else
				{
					if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE_BLEND ) )
						bm = BLENDMODE.ADDITIVE_BLEND;
					else
					{
						if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE ) )
							bm = BLENDMODE.ADDITIVE;
					}
				}
				break;

			case BLENDMODE.ADDITIVE_BLEND:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
				{
					bm = i_originalBlend;
				}
				else
				{
					if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE_SOFT ) )
						bm = BLENDMODE.ADDITIVE_SOFT;
					else
					{
						if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE ) )
							bm = BLENDMODE.ADDITIVE;
					}
				}
				break;

			case BLENDMODE.ADDITIVE_ALPHA:
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)i_originalBlend ) )
				{
					bm = i_originalBlend;
				}
				else
				{
					if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE_BLEND ) )
						bm = BLENDMODE.ADDITIVE_SOFT;
					else
					{
						if ( ShaderOneIO.GetBit ( _settings.blendModeMask, (int)BLENDMODE.ADDITIVE ) )
							bm = BLENDMODE.ADDITIVE;
					}
				}
				break;

			}

			if ( bm != i_originalBlend )
			{
				if ( EditorUtility.DisplayDialog( "**WARNING**", "Blend Mode not found !\nShaderOne can Try to find closest Blend Mode\nOr generate shader with the needed blend mode", "Generate Shader", "Best Guess" ) )
				{
					_settings.blendModeMask = ShaderOneIO.SetBit ( _settings.blendModeMask, (int)i_originalBlend, true );
					ShaderOneGenerator.Generate ( ref _settings );
					bm = i_originalBlend;
				}
			}

			return ( bm );

		}

		//=====================================================================
		public PreviousShaderSettings GetPreviousShaderSettings ( Material i_mat, Shader i_oldShader )
		{
			PreviousShaderSettings pss = new PreviousShaderSettings();

			pss.workFlow 		= WORKFLOW_MODE.SMOOTHNESS;
			pss.blendMode 		= BLENDMODE.SOLID;
			pss.specularMap		= null;
			pss.surfaceMap		= null;
			pss.alphaMapType	= MAP_CHANNEL.EMPTY;
			pss.surfaceR 		= MAP_CHANNEL.EMPTY;
			pss.surfaceG 		= MAP_CHANNEL.EMPTY;
			pss.surfaceB 		= MAP_CHANNEL.EMPTY;
			pss.surfaceA 		= MAP_CHANNEL.EMPTY;
			pss.metallic        = 0.0f;
			pss.smoothness      = 1.0f;
			pss.specColor 		= new Color(1,1,1,1);
			pss.color 			= new Color(1,1,1,1);
			pss.emission        = false;
			pss.lightMode 		= LIGHTINGMODE.NORMAL;

			ShaderOneIO.LoadSettings ( ref _settings );

			if ( i_oldShader.name.Contains("Error"))
			{
				pss.apply = false;
				return(pss);
			}

			// if its comming from itself do nothing
			if ( i_mat.HasProperty("_Layer1Fold") )
			{
				pss.apply = false;
				return(pss);
			}

			pss.apply = true;

			if ( i_mat.HasProperty("_SpecColor") )
			{
				pss.specColor = i_mat.GetColor ("_SpecColor");
			}

			if ( i_mat.HasProperty("_Color") )
			{
				pss.color = i_mat.GetColor ("_Color");
			}

			if ( i_mat.HasProperty("_TintColor") )
			{
				pss.color = i_mat.GetColor ("_TintColor");
			}

			if ( i_mat.IsKeywordEnabled("_EMISSION") )
			{
				pss.emission = true;
			}

			if ( i_mat.HasProperty("_Glossiness") )
				pss.smoothness 	= GetDefautShaderFloat (i_mat, "_Glossiness", 1.0f );

			if ( i_mat.HasProperty("_Metallic") )
				pss.metallic  	= GetDefautShaderFloat (i_mat, "_Metallic", 0.0f );

			// coming from standard shader
			if ( i_oldShader.name.Contains("Standard"))
			{
				pss.lightMode = LIGHTINGMODE.NORMAL;

				// find the work flow mode from standard shader
				if ( i_mat.HasProperty("_SpecGlossMap") && i_mat.HasProperty("_SpecColor") )
					pss.workFlow = WORKFLOW_MODE.SPECULAR;
				else if ( i_mat.HasProperty("_MetallicGlossMap") && i_mat.HasProperty("_Metallic") )
					pss.workFlow = WORKFLOW_MODE.SMOOTHNESS;
				else
					pss.workFlow = WORKFLOW_MODE.ROUGHNESS;

				if ( i_mat.HasProperty("_Mode") )
					pss.blendMode 		= (BLENDMODE)i_mat.GetInt ("_Mode");

				if ( i_mat.HasProperty("_MetallicGlossMap") )
				{
					pss.surfaceMap = i_mat.GetTexture ("_MetallicGlossMap");
					CheckSurfaceMap ( (Texture2D)pss.surfaceMap, null, _settings.surfaceMapImport );

					if ( _settings.surfaceMapR != MAP_CHANNEL.EMPTY )
						pss.surfaceR = MAP_CHANNEL.METALLIC;
				}
				else if ( i_mat.HasProperty("_SpecGlossMap") )
				{
					pss.specularMap = i_mat.GetTexture ("_SpecGlossMap");
				}

				if ( i_mat.HasProperty("_SmoothnessTextureChannel") )
				{
					int smoothnessChan = i_mat.GetInt("_SmoothnessTextureChannel");

					if ( smoothnessChan == 1 && _settings.alphaMap )
						pss.alphaMapType = MAP_CHANNEL.SMOOTHNESS;
					else
					{
						if ( _settings.surfaceMapA != MAP_CHANNEL.EMPTY )
							pss.surfaceA = MAP_CHANNEL.SMOOTHNESS;
					}
				}

				pss.fresnel 	= 1.0f;

				if ( pss.surfaceMap != null )
				{
					pss.smoothness  = GetDefautShaderFloat (i_mat, "_GlossMapScale", 1.0f );
					pss.metallic 	= 1.0f;
				}
			}

			if ( i_oldShader.name.Contains("Cutout"))
			{
				pss.blendMode 	= ChooseBestBlendMode ( BLENDMODE.CUTOUT );
			}
			else if ( i_oldShader.name.Contains("Transparent") )
			{
				pss.blendMode 	= ChooseBestBlendMode ( BLENDMODE.FADE );
			}
			else if ( i_oldShader.name.Contains("Additive") )
			{
				if ( i_oldShader.name.Contains("Blend") )
					pss.blendMode 	= ChooseBestBlendMode ( BLENDMODE.ADDITIVE_BLEND );
				else if ( i_oldShader.name.Contains("Soft") )
					pss.blendMode 	= ChooseBestBlendMode ( BLENDMODE.ADDITIVE_SOFT );
				else
					pss.blendMode 	= ChooseBestBlendMode ( BLENDMODE.ADDITIVE );
			}

			if ( i_oldShader.name.Contains("Unlit"))
			{
				pss.lightMode = LIGHTINGMODE.UNLIT;
			}

			if ( _settings.renderPipeline == RENDER_PIPELINE.UNITY_FORWARD &&
				pss.lightMode == LIGHTINGMODE.UNLIT )
				i_mat.SetShaderPassEnabled("ForwardAdd", false );
			else
				i_mat.SetShaderPassEnabled("ForwardAdd", true );

			return ( pss );
		}

		//=====================================================================
		public void SetPreviousShaderSettings ( Material i_mat,  PreviousShaderSettings i_pss )
		{
			if (i_pss.apply)
			{
				if ( _settings.workflow != i_pss.workFlow )
				{
					string [] names = Enum.GetNames ( typeof(WORKFLOW_MODE) );
					string nameStandard = names [(int)i_pss.workFlow];
					string nameShaderOne = names [(int)_settings.workflow];
					string msg1 = "ShaderOne:" + nameShaderOne + " Standard:" + nameStandard;

					if ( EditorUtility.DisplayDialog( "**WARNING**", "Workflow is different !\n" + msg1 + "\nGenerate ShaderOne as " + nameStandard + " workflow ?", "Generate Shader", "Change Nothing" ) )
					{
						_settings.workflow = i_pss.workFlow;
						ShaderOneGenerator.Generate ( ref _settings );
					}
				}

				ShaderOneTools.SetBlendMode ( i_mat, i_pss.blendMode );

				if ( i_pss.surfaceMap != null)
					i_mat.SetTexture ( "_MetallicGlossMap", i_pss.surfaceMap );

				if ( i_pss.specularMap != null)
					i_mat.SetTexture ( "_SpecGlossMap", i_pss.specularMap );

				i_mat.SetInt ("_LightMode", (int)i_pss.lightMode );
				i_mat.SetFloat ("_Metallic", i_pss.metallic );
				i_mat.SetFloat ("_Glossiness", i_pss.smoothness );
				i_mat.SetFloat ("_Fresnel", i_pss.fresnel );
				i_mat.SetInt ("_AlphaMapType", (int)i_pss.alphaMapType );
				i_mat.SetInt ("_SurfaceMapTypeG", (int)i_pss.surfaceG );
				i_mat.SetInt ("_SurfaceMapTypeB", (int)i_pss.surfaceB );
				i_mat.SetColor ("_SpecColor", i_pss.specColor );
				i_mat.SetColor ("_Color", i_pss.color );

				i_mat.SetFloat ("_EmissionToggle", i_pss.emission ? 1 : 0 );

				if ( i_pss.surfaceR != MAP_CHANNEL.EMPTY )
					i_mat.SetFloat ("_SurfaceMapTypeR", (int)i_pss.surfaceR );

				if ( i_pss.surfaceG != MAP_CHANNEL.EMPTY )
					i_mat.SetFloat ("_SurfaceMapTypeG", (int)i_pss.surfaceG );

				if ( i_pss.surfaceB != MAP_CHANNEL.EMPTY )
					i_mat.SetFloat ("_SurfaceMapTypeB", (int)i_pss.surfaceB );

				if ( i_pss.surfaceA != MAP_CHANNEL.EMPTY )
					i_mat.SetFloat ("_SurfaceMapTypeA", (int)i_pss.surfaceA );
			}
		}

		//=====================================================================
		public override void AssignNewShaderToMaterial( Material i_mat, Shader i_oldShader, Shader i_newShader )
		{
			if ( ShaderOneIO.Exists() )
				ShaderOneIO.LoadSettings(ref _settings);
			else
			{
				base.AssignNewShaderToMaterial( i_mat, i_oldShader, i_newShader );
				return;
			}

			PreviousShaderSettings pss = GetPreviousShaderSettings( i_mat, i_oldShader );

			base.AssignNewShaderToMaterial( i_mat, i_oldShader, i_newShader );

			SetPreviousShaderSettings ( i_mat, pss );

			ShaderOneTools.ProcessMaterialKeywords( i_mat);
		}

		//=====================================================================
		bool IsUnityTerrainOn ( Material i_mat )
		{
//			return ( _settings.terrainType == TERRAIN_TYPE.UNITY_TERRAIN && terrainToggle.floatValue > 0.001f );
			return ( _settings.terrainType == TERRAIN_TYPE.UNITY_TERRAIN && i_mat.GetFloat ("_TerrainToggle") > 0f );
		}

		//=====================================================================
		#if !UNITY_5
		override public void OnClosed( Material i_mat )
		{
			i_mat.SetShaderPassEnabled( "ForwardAdd", true );
		}
		#endif


		//=====================================================================
		private void BlendModePopup ()
		{
			string 	[] 	names;
			string 	[] 	namesFinal;
			int 		loop;
			int 	[] 	ids;
			int 		count;
			int         mode;

			names 		= Enum.GetNames ( typeof(BLENDMODE) );
			ids         = new int [ names.Length ];
			count 		= 0;

			for ( loop = 0; loop < names.Length; loop++ )
			{
				if ( ShaderOneIO.GetBit ( _settings.blendModeMask, loop ) )
				 {
					 ids [ count ] = loop;
					 count++;
				 }
			}

			namesFinal = new string[count];

			mode = 0;

			for ( loop = 0; loop < count; loop++ )
			{
				namesFinal[loop] = names[ids[loop]];

				if ( (int)blendMode.floatValue == ids[loop] )
				{
					mode = loop;
				}
			}

			mode = EditorGUILayout.Popup("Blend Type", mode,  namesFinal );

			blendMode.floatValue = (float)ids[mode];
		}

		//=====================================================================
		private bool MyGuiToggleEffect ( MaterialProperty imp, string itext )
		{
			bool flag = true;

			if ( imp.floatValue <= 0.0f )
				flag = false;

			EditorGUILayout.BeginHorizontal();

			flag = GUILayout.Toggle ( flag , "", GUILayout.Width(16) );
			EditorGUILayout.LabelField (itext );

			EditorGUILayout.EndHorizontal();

			if ( flag )
				imp.floatValue = 1;
			else
				imp.floatValue = 0;

			return ( flag );
		}


		//=====================================================================
		private bool MyGuiToggle ( MaterialProperty imp, string itext )
		{
			bool flag = true;

			if ( imp.floatValue <= 0.0f )
				flag = false;

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField (itext, GUILayout.Width( EditorGUIUtility.labelWidth ) );
			flag = GUILayout.Toggle ( flag , "" );

			EditorGUILayout.EndHorizontal();

			if ( flag )
				imp.floatValue = 1;
			else
				imp.floatValue = 0;

			return ( flag );
		}

		//=====================================================================
		private bool MyGuiToggle ( MaterialProperty imp, MaterialProperty i_color, string itext )
		{
			bool flag = true;

			if ( imp.floatValue <= 0.0f )
				flag = false;

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField (itext, GUILayout.Width( EditorGUIUtility.labelWidth ) );
			flag = GUILayout.Toggle ( flag , "" );

			if ( flag )
			{
				materialEditor.ColorProperty ( i_color, "" );
			}

			EditorGUILayout.EndHorizontal();

			if ( flag )
				imp.floatValue = 1;
			else
				imp.floatValue = 0;

			return ( flag );
		}

 	   //=====================================================================
		private bool TitleFold ( MaterialProperty inFold, string inmsg )
		{
			bool outState;
			bool inState;

			ShaderOneGUI.Line();

			if ( inFold.floatValue > 0.0f )
				inState = true;
			else
				inState = false;

			outState = EditorGUILayout.Foldout ( inState, inmsg );

			if ( outState )
				inFold.floatValue = 1.0f;
			else
				inFold.floatValue = 0.0f;

			return ( outState );
		}

 	   //=====================================================================
		private bool TitleFoldTexture ( MaterialProperty inFold, MaterialProperty i_tex, string inmsg, string i_toolTip = "" )
		{
			bool outState;
			bool inState;

			if ( inFold.floatValue > 0.0f )
				inState = true;
			else
				inState = false;

			Rect rc = EditorGUILayout.BeginHorizontal();
			rc.width = 8;
			rc.xMax = rc.xMin + 4;

			outState = EditorGUI.Foldout ( rc, inState, "" );

			materialEditor.TexturePropertySingleLine ( new GUIContent( inmsg, i_toolTip), i_tex );

			EditorGUILayout.EndHorizontal();

			if ( outState )
				inFold.floatValue = 1.0f;
			else
				inFold.floatValue = 0.0f;

			return ( outState );
		}

 	   //=====================================================================
		private bool TitleFoldTextureColor(MaterialProperty inFold, MaterialProperty i_tex, MaterialProperty i_color, string inmsg, string i_toolTip = "")
		{
			bool outState;
			bool inState;

			if ( inFold.floatValue > 0.0f )
				inState = true;
			else
				inState = false;

			Rect rc = EditorGUILayout.BeginHorizontal();
			rc.width = 8;
			rc.xMax = rc.xMin + 4;

			outState = EditorGUI.Foldout ( rc, inState, "" );

			materialEditor.TexturePropertySingleLine ( new GUIContent( inmsg, i_toolTip), i_tex, i_color );

			EditorGUILayout.EndHorizontal();

			if ( outState )
				inFold.floatValue = 1.0f;
			else
				inFold.floatValue = 0.0f;

			return ( outState );
		}

		//=====================================================================
		private bool TitleFoldTextureColor(MaterialProperty inFold, MaterialProperty i_tex, MaterialProperty i_color, MaterialProperty i_toggle, string inmsg, string i_toolTip = "")
		{
			bool outState;
			bool inState;

			if ( inFold.floatValue > 0.0f )
				inState = true;
			else
				inState = false;

			Rect rc = EditorGUILayout.BeginHorizontal();
			rc.width = 8;
			rc.xMax = rc.xMin + 4;

			outState = EditorGUI.Foldout ( rc, inState, "" );

			if ( GUILayout.Toggle ( ( i_toggle.floatValue == 1 ) ? true : false , "", GUILayout.Width( 16 ) ) )
				i_toggle.floatValue = 1;
			else
				i_toggle.floatValue = 0;

			materialEditor.TexturePropertySingleLine ( new GUIContent( inmsg, i_toolTip), i_tex, i_color );

			EditorGUILayout.EndHorizontal();

			if ( outState )
				inFold.floatValue = 1.0f;
			else
				inFold.floatValue = 0.0f;

			return ( outState );
		}

  	   //=====================================================================
		private void TitleFoldToggle ( MaterialProperty inFold, MaterialProperty inToggle, string inmsg )
		{
			bool outState;
			bool inState;
			bool flag;

			ShaderOneGUI.Line();

			if ( inToggle.floatValue > 0.0f )
				flag = true;
			else
				flag = false;

			if ( inFold.floatValue > 0.0f )
				inState = true;
			else
				inState = false;

			Rect rc = EditorGUILayout.BeginHorizontal();
			rc.width = 8;
			rc.xMax = rc.xMin + 4;

			outState = EditorGUI.Foldout ( rc, inState, "" );
			flag = GUILayout.Toggle ( flag, "", GUILayout.Width( 16  ) );

			EditorGUILayout.LabelField ( inmsg );

			EditorGUILayout.EndHorizontal();

			if ( outState )
				inFold.floatValue = 1.0f;
			else
				inFold.floatValue = 0.0f;

			if ( flag )
				inToggle.floatValue = 1.0f;
			else
				inToggle.floatValue = 0.0f;
		}

		//=====================================================================
		private void TerrainFoldToggle ( MaterialProperty inFold, MaterialProperty inToggle, string inmsg )
		{
			bool outState;
			bool inState;
			bool flag;

			ShaderOneGUI.Line();

			if ( inToggle.floatValue > 0.0f )
				flag = true;
			else
				flag = false;

			if ( inFold.floatValue > 0.0f )
				inState = true;
			else
				inState = false;

			Rect rc = EditorGUILayout.BeginHorizontal();
			rc.width = 8;
			rc.xMax = rc.xMin + 4;

			outState = EditorGUI.Foldout ( rc, inState, "" );
			flag = GUILayout.Toggle ( flag, inmsg );

			EditorGUILayout.EndHorizontal();

			if ( outState )
				inFold.floatValue = 1.0f;
			else
				inFold.floatValue = 0.0f;

			if ( flag )
				inToggle.floatValue = 1.0f;
			else
				inToggle.floatValue = 0.0f;
		}

		//=====================================================================
		private void TextureOption( MaterialProperty i_tex, MaterialProperty i_opt, string inmsg, string i_toolTip = "")
		{
			EditorGUILayout.BeginHorizontal();

			materialEditor.TexturePropertySingleLine ( new GUIContent( inmsg, i_toolTip), i_tex, i_opt );

			EditorGUILayout.EndHorizontal();
		}

		//=====================================================================
		private void TextureOption( MaterialProperty i_tex, MaterialProperty i_opt1, MaterialProperty i_opt2, string inmsg, string i_toolTip = "")
		{
			EditorGUILayout.BeginHorizontal();

			materialEditor.TexturePropertySingleLine ( new GUIContent( inmsg, i_toolTip), i_tex, i_opt1, i_opt2 );

			EditorGUILayout.EndHorizontal();
		}

		//=====================================================================
		void BeginGroup ( bool i_dflag )
		{
			GUILayout.BeginVertical ();
			EditorGUI.BeginDisabledGroup ( i_dflag );
		}

		//=====================================================================
		void EndGroup ()
		{
			EditorGUI.EndDisabledGroup();
			GUILayout.EndVertical();
		}

		//=====================================================================
		public void FindProperties ( MaterialProperty[] props )
		{
			int loop;

			layers = new LayerMaterialProperties[4];

			for (loop = 0; loop < 4; loop++ )
			{
				layers[loop] = new LayerMaterialProperties();
			}

			LayerFindProperties ( 0 );
			LayerFindProperties ( 1 );
			LayerFindProperties ( 2 );
			LayerFindProperties ( 3 );

			controlScriptFlag 	   	= FindProperty ("_ControlScriptFlag", props );
			myCullMode 				= FindProperty ("_MyCullMode", props );
			blendMode 				= FindProperty ("_Mode", props );
			specularMapType 		= FindProperty ("_SpecularMapType", props );
			alphaMapType 			= FindProperty ("_AlphaMapType", props );
			surfaceMapTypeR 		= FindProperty ("_SurfaceMapTypeR", props );
			surfaceMapTypeG 		= FindProperty ("_SurfaceMapTypeG", props );
			surfaceMapTypeB 		= FindProperty ("_SurfaceMapTypeB", props );
			surfaceMapTypeA 		= FindProperty ("_SurfaceMapTypeA", props );
			vertexColorMode 		= FindProperty ("_VertexColorMode", props );

			specularHighlights 		= FindProperty ("_SpecularHighlights", props );
//			specularColor 			= FindProperty ("_SpecColor", props );
//			surfaceMap 				= FindProperty ("_MetallicGlossMap", props );

//			parallax				= FindProperty ("_Parallax", props );

			moveSpeed 	   			= FindProperty ("_MoveSpeed", props );
			moveStrength  			= FindProperty ("_MoveStrength", props );
			moveDirection 			= FindProperty ("_MoveDirection", props );

			emissionFold 			= FindProperty ("_EmissionFold", props );
			emissionToggle 			= FindProperty ("_EmissionToggle", props );
			emissionColor 			= FindProperty ("_EmissionColor", props );
			emissionMap 			= FindProperty ("_EmissionMap", props );
			emissionAffectObject	= FindProperty ("_EmissionAffectObject", props );

			optionsFold				= FindProperty ("_OptionsFold", props );

			lightFold				= FindProperty ("_LightFold", props );
			lightMode				= FindProperty ("_LightMode", props );
			pbrToggle				= FindProperty ("_SO_PBRToggle", props );
//			lightProbes				= FindProperty ("_LightProbes", props );

			//smoothness 				= FindProperty("_Glossiness", props);
			//metallic 				= FindProperty("_Metallic", props);
			//fresnel 				= FindProperty("_Fresnel", props);
			//specular 				= FindProperty("_Specular", props);

			colorFront  			= FindProperty ("_Color", props );
			colorBack  				= FindProperty ("_ColorBack", props );
			colorAmplify			= FindProperty ("_ColorAmplify", props );

			uvWorldMap				= FindProperty ("_SO_UV1_WorldMap", props );
			uvWorldMapScale			= FindProperty ("_SO_UV1_WorldMapScale", props );

			saturationFold			= FindProperty ("_SaturationFold", props );
			saturationToggle		= FindProperty ("_SaturationToggle", props );
			saturationStrength		= FindProperty ("_SaturationStrength", props );

			intersectFold			= FindProperty ("_IntersectFold", props );
			intersectToggle			= FindProperty ("_IntersectToggle", props );
			intersectColor			= FindProperty ("_IntersectColor", props );
			intersectThreshold 	    = FindProperty ("_IntersectThreshold", props );

			manualControl			= FindProperty ("_ManualControl", props );
			scrollU					= FindProperty ("_ScrollU", props );
			scrollV					= FindProperty ("_ScrollV", props );

			scanlineFold			= FindProperty ("_ScanlineFold", props );
			scanlineToggle			= FindProperty ("_ScanlineToggle", props );
			scanlineStrengthHorz	= FindProperty ("_ScanlineStrengthHorz", props );
			scanlineScrollHorz		= FindProperty ("_ScanlineScrollHorz", props );
			scanlineCountHorz		= FindProperty ("_ScanlineCountHorz", props );
			scanlineWidthHorz		= FindProperty ("_ScanlineWidthHorz", props );
			scanlineStrengthVert	= FindProperty ("_ScanlineStrengthVert", props );
			scanlineScrollVert		= FindProperty ("_ScanlineScrollVert", props );
			scanlineCountVert		= FindProperty ("_ScanlineCountVert", props );
			scanlineWidthVert		= FindProperty ("_ScanlineWidthVert", props );

			rimFold             	= FindProperty ("_RimFold", props );
			rimToggle           	= FindProperty ("_RimToggle", props );
			rimColor            	= FindProperty ("_RimColor", props );
			rimWidth         		= FindProperty ("_RimWidth", props );
			rimBlend         		= FindProperty ("_RimBlend", props );

			reflectToggle			= FindProperty ("_GlossyReflections", props );
			reflectTex 		 		= FindProperty ("_ReflectTex", props );

			terrainToggle    		= FindProperty ("_TerrainToggle", props );
			terrainControl    		= FindProperty ("_Control", props );
			fogToggle    			= FindProperty ("_FogToggle", props );

			bendToggle    			= FindProperty ("_SO_BendToggle", props );

			distortFold    			= FindProperty ("_DistortFold", props );
			distortToggle  			= FindProperty ("_DistortToggle", props );
			distortHorzCount   		= FindProperty ("_DistortHorzCount", props );
			distortHorzSpeed   		= FindProperty ("_DistortHorzSpeed", props );
			distortHorzStrength		= FindProperty ("_DistortHorzStrength", props );
			distortHorzWave			= FindProperty ("_DistortHorzWave", props );

			distortVertCount		= FindProperty ("_DistortVertCount", props );
			distortVertSpeed		= FindProperty ("_DistortVertSpeed", props );
			distortVertStrength		= FindProperty ("_DistortVertStrength", props );
			distortVertWave			= FindProperty ("_DistortVertWave", props );

			distortCircularCount	= FindProperty ("_DistortCircularCount", props );
			distortCircularSpeed	= FindProperty ("_DistortCircularSpeed", props );
			distortCircularStrength	= FindProperty ("_DistortCircularStrength", props );
			distortCircularCenterU	= FindProperty ("_DistortCircularCenterU", props );
			distortCircularCenterV	= FindProperty ("_DistortCircularCenterV", props );

			rgbOffsetFold  			= FindProperty ("_RGBOffsetFold", props );
			rgbOffsetToggle			= FindProperty ("_RGBOffsetToggle", props );
			rgbOffsetStrength		= FindProperty ("_RGBOffsetStrength", props );
			rgbOffsetAmount			= FindProperty ("_RGBOffsetAmount", props );
			rgbOffsetMode			= FindProperty ("_RGBOffsetMode", props );
		}

		//=====================================================================
		public MaterialProperty LayerFindProperty ( string i_propName )
		{
			return ( FindProperty ( i_propName, properties ) );
		}

		//=====================================================================
		public void LayerFindProperties ( int i_layerIndex )
		{
			LayerMaterialProperties layer;

			layer = layers[i_layerIndex];

			layer.prefix             = "_Layer"+i_layerIndex;

			if ( i_layerIndex == 0 )
			{
				layer.name 				= "_MainTex";
				layer.tex				= LayerFindProperty ( layer.name );
				layer.bumpMap			= LayerFindProperty ( "_BumpMap" );

				if ( _settings.bumpScale != SCALE_TYPE.NONE)
					layer.bumpScale			= LayerFindProperty ( "_BumpScale" );

				if ( _settings.aoScale != SCALE_TYPE.NONE )
					layer.aoScale			= LayerFindProperty ( "_Layer0AOScale" );

				layer.smoothness        = LayerFindProperty ( "_Glossiness" );
				layer.metallic 			= LayerFindProperty ( "_Metallic" );
				layer.surfaceMap        = LayerFindProperty ( "_MetallicGlossMap");
				layer.specularMap       = LayerFindProperty ( "_SpecGlossMap");
				layer.specularColor		= LayerFindProperty ( "_SpecColor" );
				layer.parallaxHeight    = LayerFindProperty ( "_Parallax" );
			}
			else
			{
				layer.name 				= layer.prefix;
				layer.tex				= LayerFindProperty ( layer.prefix + "Tex" );
				layer.bumpMap			= LayerFindProperty ( layer.prefix + "BumpMap" );

				if ( _settings.bumpScale == SCALE_TYPE.PER_LAYER)
					layer.bumpScale			= LayerFindProperty ( layer.prefix + "BumpScale" );

				if ( _settings.aoScale == SCALE_TYPE.PER_LAYER)
					layer.aoScale			= LayerFindProperty ( layer.prefix + "AOScale" );

				layer.smoothness        = LayerFindProperty ( layer.prefix + "Smoothness" );
				layer.metallic 			= LayerFindProperty ( layer.prefix + "Metallic" );
				layer.surfaceMap        = LayerFindProperty ( layer.prefix + "SurfaceMap");
				layer.specularColor		= LayerFindProperty ( layer.prefix + "SpecColor" );
				layer.specularMap       = LayerFindProperty ( layer.prefix + "SpecGlossMap");
				layer.parallaxHeight    = LayerFindProperty ( layer.prefix + "Parallax" );
			}

			layer.fold					= LayerFindProperty ( layer.prefix + "Fold" );
			layer.toggle				= LayerFindProperty ( layer.prefix + "Toggle" );
			layer.uv2					= LayerFindProperty ( layer.prefix + "UV2" );
			layer.blendMode				= LayerFindProperty ( layer.prefix + "BlendMode" );
			layer.fresnel 				= LayerFindProperty ( layer.prefix + "Fresnel" );
			layer.flowMap				= LayerFindProperty ( layer.prefix + "FlowMap" );
			layer.flowSpeed				= LayerFindProperty ( layer.prefix + "FlowSpeed" );
//			layer.flowColor				= LayerFindProperty ( layer.prefix + "FlowColor" );
			layer.color					= LayerFindProperty ( layer.prefix + "Color" );
			layer.scrollSpeedU 			= LayerFindProperty ( layer.prefix + "ScrollU" );
			layer.scrollSpeedV 			= LayerFindProperty ( layer.prefix + "ScrollV" );
			layer.rotationSpeed			= LayerFindProperty ( layer.prefix + "Rotation" );
			layer.rotationU				= LayerFindProperty ( layer.prefix + "RotationU" );
			layer.rotationV				= LayerFindProperty ( layer.prefix + "RotationV" );
			layer.animType  	 		= LayerFindProperty ( layer.prefix + "AnimType" );
			layer.animActive  	 		= LayerFindProperty ( layer.prefix + "AnimActive" );
			layer.progressEdge			= LayerFindProperty ( layer.prefix + "ProgressEdge" );
			layer.progressColor			= LayerFindProperty ( layer.prefix + "ProgressColor" );
			layer.progressColorAmp		= LayerFindProperty ( layer.prefix + "ProgressColorAmp" );
			layer.progress	 			= LayerFindProperty ( layer.prefix + "Progress" );
			layer.animCellsHorz			= LayerFindProperty ( layer.prefix + "AnimCellsHorz" );
			layer.animCellsVert			= LayerFindProperty ( layer.prefix + "AnimCellsVert" );
			layer.animFPS 				= LayerFindProperty ( layer.prefix + "AnimFPS" );
			layer.animLoopMode			= LayerFindProperty ( layer.prefix + "AnimLoopMode" );
			layer.animCellStart 		= LayerFindProperty ( layer.prefix + "AnimCellStart" );
			layer.animCellEnd 			= LayerFindProperty ( layer.prefix + "AnimCellEnd" );
			layer.distortionStrength	= LayerFindProperty ( layer.prefix + "DistortionStrength" );
		}


		//=====================================================================
		void LayerAnimationOptions ( LayerMaterialProperties i_lp, ShaderOneLayerSettings i_ls )
		{
		}

		//=====================================================================
		public void LayerExtraOptions ( LayerMaterialProperties i_lp, ShaderOneLayerSettings i_ls )
		{
			materialEditor.TextureScaleOffsetProperty ( i_lp.tex );

			if ( i_ls.scrollUV )
			{
				materialEditor.FloatProperty( i_lp.scrollSpeedU, "Scroll U" );
				materialEditor.FloatProperty( i_lp.scrollSpeedV, "Scroll V" );
			}

			if ( i_ls.rotateUV )
				materialEditor.FloatProperty( i_lp.rotationSpeed, "Rotation" );

   			if ( i_ls.rotateUV )
			{
				materialEditor.FloatProperty( i_lp.rotationU, "Rotation Center U" );
				materialEditor.FloatProperty( i_lp.rotationV, "Rotation Center V" );
			}

			if ( _settings.distortion )
			{
				materialEditor.FloatProperty( i_lp.distortionStrength, "Distortion Strength" );
			}

			if ( i_ls.animOptions )
			{
				ANIMTYPE at;
				at = ( ANIMTYPE ) i_lp.animType.floatValue;
				at = ( ANIMTYPE )EditorGUILayout.EnumPopup("Animation Type", at );
				i_lp.animType.floatValue = (float)at;

				switch ( at )
				{
				case ANIMTYPE.RANDOM_UV:
					materialEditor.FloatProperty( i_lp.animFPS, "FPS" );
					break;

				case ANIMTYPE.CELL_ANIM:
				case ANIMTYPE.CELL_ANIM_BLEND:
					MyGuiToggle ( i_lp.animActive, "Anim Active" );

					i_lp.animLoopMode.floatValue  = (float)(ANIM_LOOP_MODE)EditorGUILayout.EnumPopup("Loop Mode", (ANIM_LOOP_MODE)i_lp.animLoopMode.floatValue );

					materialEditor.FloatProperty ( i_lp.animCellsHorz, "Cells Across" );
					materialEditor.FloatProperty ( i_lp.animCellsVert, "Cells Down" );

					if ( i_lp.animCellsHorz.floatValue < 2.0f )
					{
						i_lp.animCellsHorz.floatValue = 2.0f;
					}

					if ( i_lp.animCellsVert.floatValue < 1.0f )
					{
						i_lp.animCellsVert .floatValue = 1.0f;
					}

					materialEditor.FloatProperty( i_lp.animCellStart, "Start Cell" );
					materialEditor.FloatProperty( i_lp.animCellEnd, "End Cell" );
					materialEditor.FloatProperty( i_lp.animFPS, "FPS" );
					break;

				case ANIMTYPE.PROGRESS:
					materialEditor.ColorProperty( i_lp.progressColor, "Color" );
					materialEditor.FloatProperty( i_lp.progressColorAmp, "Amplify Color" );
					materialEditor.RangeProperty( i_lp.progressEdge, "Edge Size" );
					materialEditor.RangeProperty( i_lp.progress, "Progress" );
					break;
				}
			}
		}

		//=====================================================================
		public void SliderMinMax ( MaterialProperty i_mp, string i_msg, float i_min = 0.0f, float i_max = 2.0f )
		{
			i_mp.floatValue = EditorGUILayout.Slider ( i_msg, i_mp.floatValue, i_min, i_max );
		}

		//=====================================================================
		public bool IsOnSurfaceMap( MAP_CHANNEL i_type )
		{
			if ( (MAP_CHANNEL)surfaceMapTypeR.floatValue == i_type ||
				(MAP_CHANNEL)surfaceMapTypeG.floatValue == i_type ||
				(MAP_CHANNEL)surfaceMapTypeB.floatValue == i_type ||
				(MAP_CHANNEL)surfaceMapTypeA.floatValue == i_type ||
				(MAP_CHANNEL)specularMapType.floatValue == i_type )
				return ( true );

			if ( _settings.surfaceMapR == i_type ||
				_settings.surfaceMapG == i_type ||
				_settings.surfaceMapB == i_type ||
				_settings.surfaceMapA == i_type ||
				_settings.specularMap == i_type )
				return ( true );


			return(false);
		}

		//=====================================================================
		public bool IsOnMap( MAP_CHANNEL i_type )
		{
			if ( IsOnSurfaceMap( i_type ) )
				return ( true );

			if ( (MAP_CHANNEL)alphaMapType.floatValue == i_type )
				return(true);

			return(false);
		}

		//=====================================================================
		public void Popup ( MaterialProperty i_prop, string i_name, Type i_enum, int i_count )
		{
			int 		loop;
			string []   namesEnum 	= Enum.GetNames ( i_enum );
			string [] 	namesFinal 	= new string [ i_count ];

			for ( loop = 0; loop < i_count; loop++ )
				namesFinal[loop] = namesEnum[loop];

			i_prop.floatValue = (float)EditorGUILayout.Popup( i_name, (int)i_prop.floatValue,  namesFinal );
		}

		//=====================================================================
		public void LayerSurfaceMap ( LayerMaterialProperties i_layer, ShaderOneLayerSettings i_layerSettings, int i_index, Material i_mat )
		{
			bool metallicScale;
			bool smoothnessScale;
			bool roughnessScale;
			bool textureExists;

			textureExists = ( i_layer.surfaceMap.textureValue != null || i_layer.specularMap.textureValue != null );

			metallicScale   = IsOnMap ( MAP_CHANNEL.METALLIC ) && textureExists;
			smoothnessScale = IsOnMap ( MAP_CHANNEL.SMOOTHNESS ) && textureExists;
			roughnessScale  = IsOnMap ( MAP_CHANNEL.ROUGHNESS ) && textureExists;

			if ( pbrToggle.floatValue > 0.001f || reflectToggle.floatValue > 0.001f )
			{
				if ( metallicScale )
					SliderMinMax ( i_layer.metallic, "Metallic(S)");
				else
				{
					i_layer.metallic.floatValue = Mathf.Clamp ( i_layer.metallic.floatValue, 0.0f, 1.0f );
					materialEditor.RangeProperty( i_layer.metallic, "Metallic");
				}
			}

			if ( pbrToggle.floatValue > 0.001f || specularHighlights.floatValue > 0.001f )
			{
				switch ( _settings.workflow )
				{
				default:

					if ( smoothnessScale  )
						SliderMinMax ( i_layer.smoothness, "Smoothness(S)");
					else
					{
						i_layer.smoothness.floatValue = Mathf.Clamp ( i_layer.smoothness.floatValue, 0.0f, 1.0f );
						materialEditor.RangeProperty( i_layer.smoothness, "Smoothness");
					}
					break;

				case WORKFLOW_MODE.ROUGHNESS:
					if ( roughnessScale )
						SliderMinMax ( i_layer.smoothness, "Roughness(S)");
					else
					{
						i_layer.smoothness.floatValue = Mathf.Clamp ( i_layer.smoothness.floatValue, 0.0f, 1.0f );
						materialEditor.RangeProperty( i_layer.smoothness, "Roughness");
					}
					break;
			   }
			}

			if ( _settings.pbr && pbrToggle.floatValue > 0.001f && _settings.renderPipeline != RENDER_PIPELINE.UNLIT )
				materialEditor.RangeProperty( i_layer.fresnel, "Fresnel");

			if ( (MAP_CHANNEL)alphaMapType.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				(MAP_CHANNEL)surfaceMapTypeR.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				(MAP_CHANNEL)surfaceMapTypeG.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				(MAP_CHANNEL)surfaceMapTypeB.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				(MAP_CHANNEL)surfaceMapTypeA.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				(MAP_CHANNEL)specularMapType.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT )
				{
					materialEditor.RangeProperty ( i_layer.parallaxHeight, "Height" );
				}

		}

		//=====================================================================
		public void LayerDisplayGUI ( int i_index, Material i_mat )
		{
			ShaderOneLayerSettings layerSettings = _settings.layer0;
			LayerMaterialProperties lp = layers[i_index];
			//string toolTip ="";

			switch ( i_index )
			{
			case 0:
				layerSettings = _settings.layer0;
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

			ShaderOneGUI.Line();

			if ( IsUnityTerrainOn( i_mat ) )
			{
				TerrainFoldToggle ( lp.fold, lp.toggle, lp.name );
			}
			else
			{
				if ( i_index == 0 )
					TitleFoldTextureColor ( lp.fold, lp.tex, lp.color, lp.name );
				else
					TitleFoldTextureColor ( lp.fold, lp.tex, lp.color, lp.toggle, lp.name );
			}


			if ( lp.fold.floatValue > 0.001f )
			{
				BeginGroup ( ( lp.toggle.floatValue < 1.0f ) ? true : false );

				if ( layerSettings.uv2 == UV2_OPTION.MATERIAL_SELECTABLE )
				{
					UV2GUI ( lp );
				}

				if ( i_index == 0 )
				{
					AlphaMapGUI();
				}
				else
				{
			    	if (layerSettings.blendModes)
					{
						LayerBlendModeGUI(lp);
					}
				}

///SCOTTFIND
///				if ( lightMode.floatValue != (float)LIGHTINGMODE.UNLIT_SIMPLE )
				{
					bool useScale = false;

					if ( layerSettings.normalMap && !IsUnityTerrainOn(i_mat)  )
					{
						if ( i_index == 0  )
						{
							if ( _settings.bumpScale != SCALE_TYPE.NONE )
								useScale = true;
						}
						else
						{
							if ( _settings.bumpScale == SCALE_TYPE.PER_LAYER )
								useScale = true;
						}
					}

					if ( useScale )
						TextureOption( lp.bumpMap, lp.bumpScale, "Normal Map", "Normal Map:  \n    A special kind of texture that allow you to add surface detail to a model which catch the light as if they are represented by real geometry. (Bump Map)");
					else
						TextureOption( lp.bumpMap, null, "Normal Map", "Normal Map:  \n    A special kind of texture that allow you to add surface detail to a model which catch the light as if they are represented by real geometry. (Bump Map)");

					if ( layerSettings.surfaceMap )
					{
						Texture2D oldTex = (Texture2D)lp.surfaceMap.textureValue;

						useScale = false;
						if ( _settings.aoScale != SCALE_TYPE.NONE )
						{
							if ( _settings.aoScale == SCALE_TYPE.GLOBAL && i_index == 0 )
								useScale = true;

							if ( _settings.aoScale == SCALE_TYPE.PER_LAYER )
								useScale = true;
						}

						if ( useScale && IsOnSurfaceMap ( MAP_CHANNEL.AMBIENT_OCCLUSION ) )
							TextureOption ( lp.surfaceMap, lp.aoScale, "Surface Map", "Surface Map: \n    Put your surface map on the layer using your custom settings.");
						else
							TextureOption ( lp.surfaceMap, null, "Surface Map", "Surface Map: \n    Put your surface map on the layer using your custom settings.");
						CheckSurfaceMap ( (Texture2D)lp.surfaceMap.textureValue, oldTex, _settings.surfaceMapImport );
					}


					if ( _settings.workflow == WORKFLOW_MODE.SPECULAR )
					{
						TextureOption ( lp.specularMap, null, "Specular Map", "Specular Map: \n    For Specular workflow only. defines which areas of the object are more reflective than others.");
					}

					if ( !IsUnityTerrainOn(i_mat) )
					{
						if ( _settings.specularMode != SPECULAR_MODE.OFF )
						{
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField ( "Specular", GUILayout.Width( EditorGUIUtility.labelWidth ) );
							lp.specularColor.colorValue = EditorGUILayout.ColorField ( lp.specularColor.colorValue, GUILayout.Width( 48 ) );
							EditorGUILayout.EndHorizontal();
						}

							LayerSurfaceMap ( lp, layerSettings, i_index, i_mat );
					}
				}

				if ( layerSettings.flowMap )
					TextureOption ( lp.flowMap, lp.flowSpeed, "Flow Map", "Flow Map: \n    Texture used to make fluid effects. (R: Horizontal, G: Vertical) ");

				if ( !IsUnityTerrainOn(i_mat) )
				{
					LayerExtraOptions ( lp, layerSettings );
				}

				EndGroup();
			}
    	}

		//=====================================================================
		public void UV2GUI ( LayerMaterialProperties i_lp )
		{
            string toolTip = "";
			UV2_MATERIAL uv2;

   			uv2 = (UV2_MATERIAL)i_lp.uv2.floatValue;
            toolTip = "Pick which UV set to use from Mesh";
  			uv2 = (UV2_MATERIAL)EditorGUILayout.EnumPopup( new GUIContent( "UV", toolTip), uv2 );

   			i_lp.uv2.floatValue = (float)uv2;
		}

		//=====================================================================
		public void AlphaMapGUI()
		{
            string toolTip = "";
			MAP_CHANNEL mta;

			if ( _settings.alphaMap )
			{
				mta = (MAP_CHANNEL)alphaMapType.floatValue;
                toolTip = "Alpha Map: \n    Uses alpha channel of the layer for various options.";
				mta = (MAP_CHANNEL)EditorGUILayout.EnumPopup( new GUIContent( "Alpha Map", toolTip), mta );

				alphaMapType.floatValue = (float)mta;

				if ( (MAP_CHANNEL)alphaMapType.floatValue == MAP_CHANNEL.AMBIENT_OCCLUSION &&
					 _settings.aoScale != SCALE_TYPE.NONE )
				{
					materialEditor.FloatProperty( layers[0].aoScale, "AO Scale" );
				}
			}
		}

		//=====================================================================
		public void SpecularMapGUI()
		{
            string toolTip = "";
            MAP_CHANNEL mta;

			if ( _settings.workflow == WORKFLOW_MODE.SPECULAR && _settings.specularMap == MAP_CHANNEL.EMPTY )
			{
				mta = (MAP_CHANNEL)specularMapType.floatValue;
                toolTip = "Specular Map (A): \n    Decide options for the alpha channel on the specular map.";
                mta = (MAP_CHANNEL)EditorGUILayout.EnumPopup( new GUIContent ("Specular Map (A)", toolTip), mta );

				specularMapType.floatValue = (float)mta;
			}
		}

		//=====================================================================
		public void LayerBlendModeGUI( LayerMaterialProperties i_layerProps )
		{
			LAYER_BLEND_MODE lbm;

			lbm = (LAYER_BLEND_MODE)i_layerProps.blendMode.floatValue;

			lbm = (LAYER_BLEND_MODE)EditorGUILayout.EnumPopup( "Layer Blend Mode", lbm );

			i_layerProps.blendMode.floatValue = (float)lbm;

		}

		//=====================================================================
		public void SurfaceMapGUI()
		{
            string toolTip = "";
			MAP_CHANNEL mtt;

			if ( _settings.layer0.surfaceMap || _settings.layer1.surfaceMap || _settings.layer2.surfaceMap || _settings.layer3.surfaceMap )
			{
				if ( _settings.surfaceMapR == MAP_CHANNEL.EMPTY )
				{
					mtt = ( MAP_CHANNEL )surfaceMapTypeR.floatValue;
                    toolTip = "Surface Map (R): \n    Decide options for the red channel on the surface map.";

                    mtt = ( MAP_CHANNEL )EditorGUILayout.EnumPopup( new GUIContent ( "Surface Map (R)", toolTip ) , mtt );
					surfaceMapTypeR.floatValue = (float)mtt;
				}

				if ( _settings.surfaceMapG == MAP_CHANNEL.EMPTY )
				{
					mtt = ( MAP_CHANNEL )surfaceMapTypeG.floatValue;
                    toolTip = "Surface Map (G): \n    Decide options for the green channel on the surface map.";
                    mtt = ( MAP_CHANNEL )EditorGUILayout.EnumPopup(new GUIContent("Surface Map (G)", toolTip), mtt );
					surfaceMapTypeG.floatValue = (float)mtt;
				}

				if ( _settings.surfaceMapB == MAP_CHANNEL.EMPTY )
				{
					mtt = ( MAP_CHANNEL )surfaceMapTypeB.floatValue;
                    toolTip = "Surface Map (B): \n    Decide options for the blue channel on the surface map.";
                    mtt = ( MAP_CHANNEL )EditorGUILayout.EnumPopup(new GUIContent("Surface Map (B)", toolTip), mtt );

					surfaceMapTypeB.floatValue = (float)mtt;
				}

				if ( _settings.surfaceMapA == MAP_CHANNEL.EMPTY )
				{
					mtt = ( MAP_CHANNEL )surfaceMapTypeA.floatValue;
                    toolTip = "Surface Map (A): \n    Decide options for the alpha channel on the surface map.";
                    mtt = ( MAP_CHANNEL )EditorGUILayout.EnumPopup(new GUIContent("Surface Map (A)", toolTip), mtt );
					surfaceMapTypeA.floatValue = (float)mtt;
				}
			}
		}

		//=====================================================================
		public void LightingOptionsGUI()
		{
//            string toolTip = "";
			string wf="";

			GUILayout.BeginVertical ();

			switch ( _settings.workflow )
			{
			case WORKFLOW_MODE.ROUGHNESS:
				wf = "Roughness";
				break;

			case WORKFLOW_MODE.SMOOTHNESS:
				wf = "Smoothness";
				break;

			case WORKFLOW_MODE.SPECULAR:
				wf = "Specular";
				break;
			}

			EditorGUILayout.LabelField ( "Workflow : " + wf );

			///SCOTTFIND
			if ( _settings.renderPipeline != RENDER_PIPELINE.UNLIT )
			{
				Popup ( lightMode, "Lighting Mode", typeof(LIGHTINGMODE), 3 );

				if ( _settings.pbr )
					MyGuiToggle ( pbrToggle, "PBR" );

				if ( _settings.specularMode != SPECULAR_MODE.OFF )
					MyGuiToggle ( specularHighlights, "Specular" );
			}

			if ( _settings.reflectionType != REFLECTION_TYPE.NONE )
				MyGuiToggle ( reflectToggle, "Reflection" );

			if ( _settings.reflectionType == REFLECTION_TYPE.SPHERE_MAP )
				 TextureOption ( reflectTex, null, "Sphere Map", "ToolTip");

			SpecularMapGUI();

			SurfaceMapGUI();

			GUILayout.EndVertical();
		}

		//=====================================================================
		public void GlobalOptions( Material i_mat )
		{
			string toolTip = "";
			GUILayout.BeginVertical ();

			if ( TitleFold ( optionsFold, "Material Options" ) )
			{
				toolTip = "Blend Type: \n    How the Material blends in the scene";

				BlendModePopup();

				materialEditor.RenderQueueField();

				UnityEngine.Rendering.CullMode cm;
				cm = ( UnityEngine.Rendering.CullMode ) myCullMode.floatValue;
                toolTip = "Cull Mode: \n    Side of mesh face that is rendered";
                cm = ( UnityEngine.Rendering.CullMode ) EditorGUILayout.EnumPopup(new GUIContent("Cull Mode", toolTip), cm );
				myCullMode.floatValue = (float)cm;

				if ( _settings.vertexOptions )
				{
					VERTEXMODE vm;
					vm = (VERTEXMODE)vertexColorMode.floatValue;
                    toolTip = "Vertex Color: \n    Decide what the vertex color is used for.";
					vm = (VERTEXMODE)EditorGUILayout.EnumPopup(new GUIContent("Vertex Color", toolTip), vm );
					vertexColorMode.floatValue = (float)vm;

					if ( vm == VERTEXMODE.MOVEMENT )
					{
						ShaderOneGUI.Line();
						EditorGUILayout.LabelField ("Vertex Movement");
						materialEditor.FloatProperty( moveSpeed, "Speed" );
						materialEditor.FloatProperty( moveStrength, "Strength" );

						moveDirection.vectorValue = (Vector4)EditorGUILayout.Vector3Field ("Direction", (Vector3)moveDirection.vectorValue );
						ShaderOneGUI.Line();
					}
				}

				switch ( ( UnityEngine.Rendering.CullMode )myCullMode.floatValue )
				{
				default:
					materialEditor.ColorProperty ( colorFront, "Color" );
					materialEditor.ColorProperty ( colorBack, "Color Back Face" );
					break;

				case UnityEngine.Rendering.CullMode.Front:
					materialEditor.ColorProperty ( colorBack, "Color Back Face" );
					break;

				case UnityEngine.Rendering.CullMode.Back:
					materialEditor.ColorProperty ( colorFront, "Color" );
					break;
				}

				if ( _settings.colorAmplify )
					materialEditor.FloatProperty ( colorAmplify, "Color Amplify" );

				if ( _settings.uvWorldMap )
				{
					uvWorldMap.floatValue = (float)(UVCONTROL)EditorGUILayout.EnumPopup(new GUIContent("UV Mapping", toolTip), (UV_WORLD_MAPPING)uvWorldMap.floatValue );

					if ( uvWorldMap.floatValue > 0.001f )
						materialEditor.FloatProperty( uvWorldMapScale, "UV Map Scale" );
				}

				UVCONTROL uvc;;
				uvc = (UVCONTROL)manualControl.floatValue;
                toolTip = "UV Control:\n    Automated: UV values automatically animate over time.\n    Manual: Directly set UV values or control values from script.";
				uvc = (UVCONTROL)EditorGUILayout.EnumPopup(new GUIContent("UV Control", toolTip), uvc );
				manualControl.floatValue = (float)uvc;

	//		    MyGuiToggle ( manualControl, "Manual Control of UV" );

				// GLOBAL SCROLL
				materialEditor.FloatProperty( scrollU, "Scroll U" );
				materialEditor.FloatProperty( scrollV, "Scroll V" );

				if ( _settings.instancing )
				{
					EditorGUILayout.BeginHorizontal();

                    toolTip = "GPU Instancing:\n    Used to draw multiple copies of the same Mesh at once, using a small number of draw calls.";
					EditorGUILayout.LabelField ( new GUIContent( "Instancing", toolTip), GUILayout.Width( EditorGUIUtility.labelWidth ) );
					i_mat.enableInstancing = GUILayout.Toggle ( i_mat.enableInstancing, "" );

					EditorGUILayout.EndHorizontal();
				}


				if ( _settings.fogMode != FOG_MODE.OFF )
					MyGuiToggle ( fogToggle, "Fog" );

				if ( _settings.bendType != BEND_TYPE.OFF )
					MyGuiToggle ( bendToggle, "Warp" );

				TerrainGUI();
			}

			GUILayout.EndVertical();
		}
 /*
		//=====================================================================
		void SurfaceMapGUI()
		{
			//if ( _settings.surfaceMap  )
			//	TextureOption ( surfaceMap, null, "Surface Map", "ToolTip");

			SurfaceMapGUI();

//			AlphaMapGUI();
			// PUT KEYWORD FOR PARRALLAX
			if ( (MAP_CHANNEL)alphaMapType.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				 (MAP_CHANNEL)surfaceMapTypeR.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				 (MAP_CHANNEL)surfaceMapTypeG.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				 (MAP_CHANNEL)surfaceMapTypeB.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT ||
				 (MAP_CHANNEL)surfaceMapTypeA.floatValue == MAP_CHANNEL.PARALLAX_HEIGHT )
			{
				materialEditor.RangeProperty( parallax, "Height");
			}
		}
*/
		//=====================================================================
		public void BasicShaderOptionsGUI ( Material i_mat )
		{
			String foldName;

			GlobalOptions( i_mat );

			if ( _settings.renderPipeline == RENDER_PIPELINE.UNLIT )
				foldName = "Surface Options";
			else
				foldName = "Lighting Options";

			if ( TitleFold ( lightFold, foldName ) )
			{
				LightingOptionsGUI();
			}
		}

		//=====================================================================
		void TerrainGUI()
		{
			if ( _settings.terrainType == TERRAIN_TYPE.MESH_TERRAIN )
			{
				MyGuiToggle ( terrainToggle, "Mesh Terrain" );

				materialEditor.TexturePropertySingleLine ( new GUIContent( "Terrain Control", ""), terrainControl );
   			    materialEditor.TextureScaleOffsetProperty ( terrainControl );
			}

			if ( _settings.terrainType == TERRAIN_TYPE.UNITY_TERRAIN )
			{
				MyGuiToggle ( terrainToggle, "Unity Terrain" );
			}
		}

		//=====================================================================
		void DistortionGUI()
		{
			TitleFoldToggle ( distortFold, distortToggle, "Distortion" );

			if ( distortFold.floatValue > 0.0f )
			{
				BeginGroup ( ( distortToggle.floatValue < 1.0f ) ? true : false );

				materialEditor.RangeProperty ( distortHorzStrength , "Strength" );
				materialEditor.RangeProperty( distortHorzCount , "Count" );
				materialEditor.FloatProperty( distortHorzSpeed , "Speed" );
				materialEditor.RangeProperty( distortHorzWave , "Wave Shape" );

				EditorGUILayout.Space();
				EditorGUILayout.LabelField ("Vertical");

				materialEditor.RangeProperty( distortVertStrength , "Strength" );
				materialEditor.RangeProperty( distortVertCount , "Count" );
				materialEditor.FloatProperty( distortVertSpeed , "Speed" );
				materialEditor.RangeProperty( distortVertWave , "Wave Shape" );

				EditorGUILayout.Space();
	   			EditorGUILayout.LabelField ("Circular");

	   			materialEditor.RangeProperty( distortCircularStrength , "Strength" );
	   			materialEditor.RangeProperty( distortCircularCount , "Count" );
	   			materialEditor.FloatProperty( distortCircularSpeed , "Speed" );
	   			materialEditor.RangeProperty( distortCircularCenterU , "Center U" );
				materialEditor.RangeProperty( distortCircularCenterV , "Center V" );

				EndGroup();
			}
		}

		//=====================================================================
		void EmissionGUI()
		{
			TitleFoldToggle ( emissionFold, emissionToggle, "Emission" );
			if ( emissionFold.floatValue > 0.0f )
			{
				BeginGroup ( ( emissionToggle.floatValue < 1.0f ) ? true : false );
				materialEditor.TexturePropertySingleLine( new GUIContent("Emission Map", "Base"), emissionMap );
				materialEditor.ColorProperty( emissionColor , "Emission Color" );
				materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
				MyGuiToggle ( emissionAffectObject, "Emission Affect Object" );

				EndGroup();
			}
		}

		//=====================================================================
		void ChromaticAbberationGUI()
		{
			TitleFoldToggle ( rgbOffsetFold, rgbOffsetToggle, "Chromatic Abberation" );
			if ( rgbOffsetFold.floatValue > 0.0f )
			{
				RGBOFFSETTYPE rot;

				BeginGroup ( ( rgbOffsetToggle.floatValue < 1.0f ) ? true : false );

				materialEditor.RangeProperty( rgbOffsetStrength , "Strength" );
				materialEditor.RangeProperty( rgbOffsetAmount, "Amount" );

				if ( rgbOffsetMode.floatValue == 0 )
					rot = RGBOFFSETTYPE.CIRCULAR;
				else
					rot = RGBOFFSETTYPE.CONSTANT;

				rot = ( RGBOFFSETTYPE )EditorGUILayout.EnumPopup("Mode", rot );

				if ( rot == RGBOFFSETTYPE.CIRCULAR )
					rgbOffsetMode.floatValue = 0;
				else
					rgbOffsetMode.floatValue = 1;

				EndGroup();
			}
		}

		//=====================================================================
		void SaturationGUI()
		{
			TitleFoldToggle ( saturationFold, saturationToggle, "Saturation" );
			if ( saturationFold.floatValue > 0.0f )
			{
				BeginGroup ( ( saturationToggle.floatValue < 1.0f ) ? true : false );

				materialEditor.RangeProperty( saturationStrength, "Strength" );

				EndGroup();
			}
		}

		//=====================================================================
		void ScanlinesGUI()
		{
			TitleFoldToggle (scanlineFold, scanlineToggle, "Scanlines" );
			if ( scanlineFold.floatValue > 0.0f )
			{
				EditorGUILayout.LabelField ("Horizontal");

				BeginGroup ( ( scanlineToggle.floatValue < 1.0f ) ? true : false );

				materialEditor.RangeProperty( scanlineStrengthHorz, "Strength" );
				materialEditor.FloatProperty( scanlineScrollHorz, "Scroll" );
				materialEditor.FloatProperty( scanlineCountHorz, "Line Count" );

				scanlineCountHorz.floatValue = Mathf.Abs ( scanlineCountHorz.floatValue );

				materialEditor.RangeProperty( scanlineWidthHorz, "Width" );

				EditorGUILayout.LabelField ("Vertical");

				materialEditor.RangeProperty( scanlineStrengthVert, "Strength" );
				materialEditor.FloatProperty( scanlineScrollVert, "Scroll" );
				materialEditor.FloatProperty( scanlineCountVert, "Line Count" );

				scanlineCountVert.floatValue = Mathf.Abs ( scanlineCountVert.floatValue );

				materialEditor.RangeProperty( scanlineWidthVert, "Width" );

				EndGroup();
			}
		}

		//=====================================================================
		void RimLightGUI()
		{
			TitleFoldToggle ( rimFold, rimToggle, "Rim Lighting" );
			if ( rimFold.floatValue > 0.0f )
			{
				BeginGroup ( ( rimToggle.floatValue < 1.0f ) ? true : false );

				RIMLITBLEND rlb;
	   			rlb = (RIMLITBLEND)rimBlend.floatValue;
	   			rlb = (RIMLITBLEND)EditorGUILayout.EnumPopup("Blend Mode", rlb );
	   			rimBlend.floatValue = (float)rlb;

				materialEditor.ColorProperty ( rimColor, "Color");
				materialEditor.RangeProperty ( rimWidth, "Width" );

				EndGroup();
			}
		}

		//=====================================================================
		void IntersectGUI()
		{
			TitleFoldToggle ( intersectFold, intersectToggle, "Intersect" );

			if ( intersectFold.floatValue > 0.0f )
			{
				BeginGroup ( ( intersectToggle.floatValue < 1.0f ) ? true : false );

				materialEditor.ColorProperty ( intersectColor, "Color" );
				materialEditor.FloatProperty ( intersectThreshold, "Threshold" );

				EndGroup();
			}
		}

		//=====================================================================
		private ShaderOneLayerSettings SettingsLayer ( int i_index )
		{
			ShaderOneLayerSettings layer = _settings.layer0;

			switch (i_index)
			{
			case 0:
				layer = _settings.layer0;
				break;

			case 1:
				layer = _settings.layer1;
				break;

			case 2:
				layer = _settings.layer2;
				break;

			case 3:
				layer = _settings.layer3;
				break;
			}

			return ( layer );
		}

		//=====================================================================
		private void CalculateStats()
		{
			LayerMaterialProperties layerProps;
			ShaderOneLayerSettings  layer;
			int index;
			float layerTexcoordCount 	= 0;
			int layersActive 			= 0;
			bool normUV 				= false;

			textureCount 	= 0;
			texcoordCount   = 0;
			effectCount     = 0;

			// layers
			for ( index = 0; index < 4; index++ )
			{
				layerProps = layers[index];
			    layer = SettingsLayer ( index );

				if (!layer.enabled)
					continue;

				layersActive++;

				if ( layerProps.tex.textureValue != null )
				{
					layerTexcoordCount+=0.5f;
					textureCount++;
				}

				if ( layer.normalMap && layerProps.bumpMap.textureValue != null )
					textureCount++;

				if ( layer.flowMap && layerProps.flowMap.textureValue != null )
					textureCount++;

				if ( layer.animOptions && (ANIMTYPE)layerProps.animType.floatValue != ANIMTYPE.OFF )
				{
					effectCount++;

					if ( (ANIMTYPE)layerProps.animType.floatValue == ANIMTYPE.PROGRESS )
					{
						normUV = true;
					}
				}
			}

			texcoordCount += (int)(layerTexcoordCount + 0.51f );

			///SCOTTFIND account for NormUV
			// Emission
			if ( _settings.emission && emissionToggle.floatValue > 0.001f )
			{
				effectCount++;
				textureCount++;
			}

			// FOG
			if ( _settings.fogMode != FOG_MODE.OFF && fogToggle.floatValue > 0.001f )
			{
				textureCount++;
				effectCount++;
			}

			// Surface MAP
			//if ( _settings.surfaceMap && surfaceMap.textureValue != null )
			//{
			//	normUV = true;
			//	textureCount++;
			//}

			// Saturation
			if ( _settings.saturation && saturationToggle.floatValue > 0.001f )
			{
				effectCount++;
			}

			// intersection
			if ( _settings.intersect && intersectToggle.floatValue > 0.001f )
			{
				effectCount++;
			}

			// scanline
			if ( _settings.scanlines && scanlineToggle.floatValue > 0.001f )
			{
				normUV = true;
				effectCount++;
			}

			// reflection
			if ( _settings.reflectionType != REFLECTION_TYPE.NONE && reflectToggle.floatValue > 0.001f )
			{
				textureCount++;
			}

			// Rim Lighting
			if ( _settings.rimLighting && rimToggle.floatValue > 0.001f  )
			{
				effectCount++;
			}

			// DISTORTION
			if ( _settings.distortion && ( distortHorzStrength.floatValue > 0.001f || distortVertStrength.floatValue > 0.001f || distortCircularStrength.floatValue > 0.001f ) )
			{
				normUV = true;
				effectCount++;
			}

			// RGBOFFSET
			if ( _settings.chromaticAbberation && rgbOffsetToggle.floatValue > 0.001 )
			{
				normUV 		= true;
				effectCount++;
				textureCount += ( 3 *layersActive );
			}

			if ( normUV )
			{
				texcoordCount++;
			}

		}

		//=====================================================================
		private void DisplayStats()
		{
			GUILayout.BeginVertical ( EditorStyles.helpBox );

			EditorGUILayout.LabelField ("ShaderOneControlScript : " + (_controlScriptOn ? "On" : "Off" ));

			EditorGUILayout.LabelField ("Texture Reads :" + textureCount );
			EditorGUILayout.LabelField ("Texcoords Used :" + texcoordCount );
			EditorGUILayout.LabelField ("Effects :" + effectCount );

			GUILayout.EndVertical();
		}

		//=====================================================================
		private void AddRemoveControlScript( Material i_mat, bool i_addFlag = true )
		{
			GameObject[] gos;
			GameObject go;
			Renderer r;
			string name;
			ShaderOneMaterialControl somc;

			gos = Resources.FindObjectsOfTypeAll ( typeof ( GameObject ) )as GameObject[];

			for ( int loop = 0; loop < gos.Length; loop++ )
			{
				go = gos[loop];

				r = null;

				if ( go != null )
					r = go.GetComponent<Renderer>();

				if ( r != null && r.sharedMaterial != null )
				{
					name = r.sharedMaterial.name;

					if ( name == i_mat.name )
					{
						somc = go.GetComponent<ShaderOneMaterialControl>();

						if ( i_addFlag )
						{
							if ( somc == null )
							{
								go.AddComponent<ShaderOneMaterialControl>();
							}
							else
							{
								if ( somc.enabled == false )
								{
									somc.enabled = true;
								}
							}
						}
						else
						{
							if ( somc != null )
							{
								somc.RemoveScript();
							}
						}
					}
				}
			}
		}

		//=====================================================================
		private void DisplayScriptWarning()
		{
			bool scriptNeeded = false;

			if ( ( _settings.layer0.enabled && ( ANIMTYPE )layers[0].animType.floatValue == ANIMTYPE.CELL_ANIM ) ||
				 ( _settings.layer1.enabled && ( ANIMTYPE )layers[1].animType.floatValue == ANIMTYPE.CELL_ANIM ) ||
				 ( _settings.layer2.enabled && ( ANIMTYPE )layers[2].animType.floatValue == ANIMTYPE.CELL_ANIM ) ||
				 ( _settings.layer3.enabled && ( ANIMTYPE )layers[3].animType.floatValue == ANIMTYPE.CELL_ANIM ) )
			{
				scriptNeeded = true;
			}

			if ( ( _settings.layer0.enabled && ( ANIMTYPE )layers[0].animType.floatValue == ANIMTYPE.CELL_ANIM_BLEND ) ||
				( _settings.layer1.enabled && ( ANIMTYPE )layers[1].animType.floatValue == ANIMTYPE.CELL_ANIM_BLEND ) ||
				( _settings.layer2.enabled && ( ANIMTYPE )layers[2].animType.floatValue == ANIMTYPE.CELL_ANIM_BLEND ) ||
				( _settings.layer3.enabled && ( ANIMTYPE )layers[3].animType.floatValue == ANIMTYPE.CELL_ANIM_BLEND ) )
			{
				scriptNeeded = true;
			}

			if ( _settings.intersect && intersectToggle.floatValue > 0.001f )
			{
				scriptNeeded = true;
			}

			if ( scriptNeeded && !_controlScriptOn )
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUILayout.LabelField ("Warning: One or more options in use...");
				EditorGUILayout.LabelField ("require the ShaderOneMaterialControl.cs");
				EditorGUILayout.LabelField ("Would you like to add it to all GameObjects");
				EditorGUILayout.LabelField ("Using this Materal ?");

				if (GUILayout.Button("Add", GUILayout.Width(60)))
				{
					if ( EditorUtility.DisplayDialog("Attention !", "This will add or enable ShaderOneMaterialControl.cs on every GameObject using this material!\nProceed ?", "Yes", "No" ) )
						AddRemoveControlScript( targetMat, true );
				}

				GUILayout.EndVertical();
			}

			if ( !scriptNeeded && _controlScriptOn )
			{
				GUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUILayout.LabelField ("Warning: ShaderOneMaterialScript.cs is not needed");
				EditorGUILayout.LabelField ("Would you like to remove it from all GameObjects");
				EditorGUILayout.LabelField ("using this material ?");
				if (GUILayout.Button("Remove", GUILayout.Width(60)))
	   			{
					if ( EditorUtility.DisplayDialog("Attention !", "This will Remove ShaderOneMaterialControl.cs on every GameObject using this material!/nroceed ?", "Yes", "No" ) )
						AddRemoveControlScript( targetMat, false );
	   			}
				GUILayout.EndVertical();
			}
		}

		//=====================================================================
		public override void OnGUI ( MaterialEditor i_materialEditor, MaterialProperty[] i_properties)
		{
			materialEditor 	= i_materialEditor;
			properties 		= i_properties;
			targetMat 		= i_materialEditor.target as Material;

			if ( GUILayout.Button ( "Material Docs" ) )
			{
				ShaderOneIO.OpenDocPDF("ShaderOne_Material_Docs");
			}

			if ( reloadSettings > 0 )
			{
				reloadSettings--;
				ShaderOneIO.LoadSettings( ref _settings );
			}

			if ( !_settingsLoaded )
			{
				ShaderOneIO.LoadSettings( ref _settings );
				_settingsLoaded = true;
				ShaderOneTools.ProcessMaterialKeywords(targetMat);
			}

			if ( controlScriptFlag == null)
			{
				FindProperties ( properties );
			}

			_controlScriptOn = false;
			if ( controlScriptFlag.floatValue > 0.001f )
				_controlScriptOn = true;

			DisplayScriptWarning();

			EditorGUI.BeginChangeCheck();
			{
				BasicShaderOptionsGUI( targetMat );

				// MainTex
				LayerDisplayGUI ( 0, targetMat );

				if (_settings.layer1.enabled )
					LayerDisplayGUI ( 1, targetMat );

				if (_settings.layer2.enabled )
					LayerDisplayGUI ( 2, targetMat );

				if (_settings.layer3.enabled )
					LayerDisplayGUI ( 3, targetMat );

				// Distortion
				if ( _settings.distortion )
					DistortionGUI();

				// EMISSION
				if ( _settings.emission )
					EmissionGUI();

				// RGB OFFSET
				if ( _settings.chromaticAbberation )
					ChromaticAbberationGUI();

				// SATURATION
				if ( _settings.saturation )
					SaturationGUI();

				// Scanlines
				if ( _settings.scanlines )
					ScanlinesGUI();

				// RIM LIGHT
				if ( _settings.rimLighting )
					RimLightGUI();

				// Intersect
				if ( _settings.intersect )
				{
					IntersectGUI();
				}

				// instancing
			    ShaderOneGUI.Line();
				DisplayStats();
			}

			if ( EditorGUI.EndChangeCheck() || init )
			{
				init = false;
				ShaderOneTools.ProcessMaterialKeywords(targetMat);
				CalculateStats();
			}
		}
	}

