// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using ShaderOneSystem;

//----------------------------------------------------------------------------
[InitializeOnLoad]
public class ShaderOneGenerator : EditorWindow
{
	Texture soLogo;
	[System.NonSerializedAttribute]
	static ShaderOneSettings _settings;
	[System.NonSerializedAttribute]
	bool lightingAdvancedFoldout 	= false;
	bool lightingAdvancedChanged 	= false;
	Vector2 scrollPos 				= Vector2.zero;

    private string toolTip = "";

	//============================================================
    [MenuItem("Window/ShaderOne Generator")]
    public static void Init()
    {
        ShaderOneGenerator window = (ShaderOneGenerator)EditorWindow.GetWindow(typeof(ShaderOneGenerator), false, "ShaderOne", true );

		window.minSize = new Vector2 ( 300, 400 );

        window.Show();
    }

	//============================================================
	public static void Generate ( ref ShaderOneSettings i_settings )
	{
		ShaderOneIO.SaveSettings( ref i_settings );
		ShaderOneParse.ShaderOneMake ( i_settings );
		AssetDatabase.Refresh();

		string path = ShaderOneIO.FindAsset ("ShaderOneGen.shader");

		if ( path != null )
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );

		//_settings = ShaderOneIO.LoadSettings();
		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

		ShaderOneManager.ResetManager();
	}

	//============================================================
    public void Awake()
    {
		string logoPath = ShaderOneIO.FindRootFolder() + "/Logos/ShaderOneLogo.psd";
		ShaderOneIO.LoadSettings(ref _settings);
//		soLogo = AssetDatabase.LoadAssetAtPath<Texture>("Assets/echoLogin/ShaderOne/Logos/ShaderOneLogo.psd");
		soLogo = AssetDatabase.LoadAssetAtPath<Texture> ( logoPath );
	}

	//============================================================
	private void OnDestroy()
	{
		AssetDatabase.Refresh();
	}

	//============================================================
	public bool ShaderOneManagerCheck()
	{
		ShaderOneManager[] som;

		som = Resources.FindObjectsOfTypeAll ( typeof ( ShaderOneManager ) ) as ShaderOneManager[];

		if ( som == null || som.Length < 1 )
		{
			if ( _settings.renderPipeline == RENDER_PIPELINE.SHADERONE_LIGHTING || _settings.fogMode != FOG_MODE.OFF )
			{
				if ( EditorUtility.DisplayDialog("ShaderOne", "The Options you have chosen require the ShaderOneManager be in scene!\n\nWould you like me to add it for you ?", "Yes", "No" ) )
				{
					GameObject go = new GameObject("ShaderOneManager");
					go.AddComponent<ShaderOneManager>();
					return(true);
				}
			}

			return false;
		}

		if ( som.Length > 1 )
		{
			EditorUtility.DisplayDialog("**WARNING**\nYou Have More than one ShaderOneManagers in Scene", "There can only be ONE !", "OK" );
		}

		return ( true );
	}

	//============================================================
	public void GetLayerOptions ( ref ShaderOneLayerSettings i_layer, int i_layerNum )
	{
		string toolTip = "";

		if ( i_layerNum == 0 )
		{
			i_layer.enabled 	= true;
			i_layer.uv2 		= UV2_OPTION.OFF;
		}
		else
        {
            toolTip = ( "Toggle Layer " + i_layerNum );
            i_layer.enabled 	= EditorGUILayout.Toggle ( new GUIContent("Enabled", toolTip ), i_layer.enabled);

			toolTip = "UV2 Options";
			i_layer.uv2	= ( UV2_OPTION ) EditorGUILayout.EnumPopup ( new GUIContent("UV2", toolTip), i_layer.uv2 );
        }

        EditorGUI.BeginDisabledGroup ( !i_layer.enabled );

		if ( i_layerNum == 0 )
			i_layer.blendModes		= false;
		else
			i_layer.blendModes 		= EditorGUILayout.Toggle( new GUIContent ( "Blend Modes", toolTip), i_layer.blendModes );

        toolTip = "Toggle normal map for layer";
        i_layer.normalMap 		= EditorGUILayout.Toggle( new GUIContent ( "Normal/Bump Map", toolTip), i_layer.normalMap );

		toolTip = "Surface Map: \nR = Metallic\nG = Custom\nB = Custom\nA = Smoothness";
		i_layer.surfaceMap    	= EditorGUILayout.Toggle(new GUIContent("SurfaceMap", toolTip), i_layer.surfaceMap );

        toolTip = "Toggle flowmap for layer \nUsed for water like effects";
        i_layer.flowMap 		= EditorGUILayout.Toggle( new GUIContent ( "Flow Map", toolTip), i_layer.flowMap );

        toolTip = "Animation: \nAdds options for\nCell Animation\nProgress/Dissolve\nRandom UV Animation";
        i_layer.animOptions 	= EditorGUILayout.Toggle( new GUIContent ( "Animation", toolTip ), i_layer.animOptions );

        toolTip = "Toggle texture scroll for layer";
        i_layer.scrollUV		= EditorGUILayout.Toggle( new GUIContent ( "Scroll UV", toolTip ), i_layer.scrollUV );

        toolTip = "Toggle texture rotation for layer";
        i_layer.rotateUV		= EditorGUILayout.Toggle( new GUIContent ( "Rotate UV", toolTip ), i_layer.rotateUV );

		if ( i_layerNum != 0 )
		{
			toolTip = "Be Able to turn layer on/off from script using Keywords";
			i_layer.scriptToggle 		= EditorGUILayout.Toggle( new GUIContent ( "Script Toggle", toolTip), i_layer.scriptToggle );
		}

		EditorGUI.EndDisabledGroup ();
	}

	//============================================================
    public void GetUserOptions()
    {
		ShaderOneGUI.Line();
		_settings.layer0Foldout = EditorGUILayout.Foldout ( _settings.layer0Foldout, "MainTex(Layer 0)" );
		if ( _settings.layer0Foldout )
			GetLayerOptions ( ref _settings.layer0, 0 );

		ShaderOneGUI.Line();
		_settings.layer1Foldout = EditorGUILayout.Foldout ( _settings.layer1Foldout, "Layer 1" );
		if ( _settings.layer1Foldout )
			GetLayerOptions ( ref _settings.layer1, 1 );

		ShaderOneGUI.Line();
		_settings.layer2Foldout = EditorGUILayout.Foldout ( _settings.layer2Foldout, "Layer 2" );
		if ( _settings.layer2Foldout )
			GetLayerOptions ( ref _settings.layer2, 2 );

		ShaderOneGUI.Line();
		_settings.layer3Foldout = EditorGUILayout.Foldout ( _settings.layer3Foldout, "Layer 3" );
		if ( _settings.layer3Foldout )
			GetLayerOptions ( ref _settings.layer3, 3 );

		ShaderOneGUI.Line();
		_settings.effectsFoldout = EditorGUILayout.Foldout ( _settings.effectsFoldout, "Effects" );

		if ( _settings.effectsFoldout )
		{
            toolTip = "Distortion: \nDistort image horizontally and vertically";
            _settings.distortion			= EditorGUILayout.Toggle(new GUIContent("Distortion", toolTip), _settings.distortion );

			toolTip = "Emission: \nLight emitted off object surface";
			_settings.emission 				= EditorGUILayout.Toggle(new GUIContent("Emission", toolTip), _settings.emission );

            toolTip = "Chromatic Abberation: \nRGB Offset of object";
            _settings.chromaticAbberation 	= EditorGUILayout.Toggle(new GUIContent("Chromatic Abberation", toolTip), _settings.chromaticAbberation );

            toolTip = "Saturation: \nColor Saturation of object";
            _settings.saturation            = EditorGUILayout.Toggle(new GUIContent("Saturation", toolTip), _settings.saturation );

            toolTip = "Scan Lines: \nChoose strength and scroll speed of horizontal or vertical scanlines";
            _settings.scanlines 			= EditorGUILayout.Toggle(new GUIContent("Scan Lines", toolTip), _settings.scanlines );

            toolTip = "Rim Lighting: \nEmissive light based on the angle between surface normal and view direction. ";
            _settings.rimLighting 			= EditorGUILayout.Toggle(new GUIContent("Rim Lighting", toolTip), _settings.rimLighting );

            toolTip = "Intersect: \nProvides lighting effect based on intersecting objects";
            _settings.intersect 			= EditorGUILayout.Toggle(new GUIContent("Intersect", toolTip), _settings.intersect );
		}

		ShaderOneGUI.Line();
    }

	//=========================================================================
	private void DisplayLightStats ( string i_name, int i_count, int i_perPixel )
	{
		EditorGUILayout.BeginHorizontal();
//    	string outString = i_name + " :" + i_count + "  PerPixel :" + i_perPixel + "  PerVertex :" + ( i_count - i_perPixel );

		GUILayout.Label ( i_name + " :" + i_count, GUILayout.Width ( 80 ) );
		GUILayout.Label ( "PerPixel :" + i_perPixel, GUILayout.Width ( 80 ) );
		GUILayout.Label ( "PerVertex :" + ( i_count - i_perPixel ) );

		EditorGUILayout.EndHorizontal();
	}

	//============================================================
	private int DisplayLightSlider ( int i_perPixel, int i_maxCount )
	{
		EditorGUILayout.GetControlRect(false, 16);
		Rect r = GUILayoutUtility.GetLastRect();
		r.width -= 8;
		r.x += 4;
		r.y-=2;
		return ( (int)GUI.HorizontalSlider( r, i_perPixel, 0, (float)i_maxCount ) );
	}

	//============================================================
	private int BitMaskToggle ( string i_msg, string i_toolTip, int i_mask, int i_bitPos )
	{
		bool bitFlag = ShaderOneIO.GetBit ( i_mask, i_bitPos );

		bitFlag	= EditorGUILayout.Toggle ( new GUIContent( i_msg, i_toolTip), bitFlag );

		i_mask = ShaderOneIO.SetBit ( i_mask, i_bitPos, bitFlag );

		return ( i_mask );
	}

	//============================================================
    private void OnGUI()
    {
		BeginWindows();

		if ( _settings.loaded == 0 )
			ShaderOneIO.LoadSettings(ref _settings);

		GUILayout.Label("Global ShaderOne Settings v0.21", EditorStyles.boldLabel );

		EditorGUILayout.GetControlRect(false, 37);
		EditorGUI.DrawRect(new Rect(16, 26, Screen.width-32, 37), Color.black);
		Rect rpos = new Rect (7,26,600,37);
		GUI.DrawTexture(rpos, soLogo );

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();

        toolTip = "Lighting Pipeline";
        _settings.renderPipeline	= ( RENDER_PIPELINE ) EditorGUILayout.EnumPopup ( new GUIContent("Render Pipeline", toolTip), _settings.renderPipeline );

		toolTip = "Color Precision: \nFixed is fastest \nFloat is slowest but most precise";
		_settings.colorPrecision   	= ( COLOR_PRECISION ) EditorGUILayout.EnumPopup ( new GUIContent("Color Precision", toolTip), _settings.colorPrecision );

		toolTip = "Reflection Type: \nSpheremap is faster";
		_settings.reflectionType	= ( REFLECTION_TYPE ) EditorGUILayout.EnumPopup ( new GUIContent("Reflection Type", toolTip), _settings.reflectionType );

		toolTip = "Terrain: \n Mesh Terrain or Unity Terrain";
		_settings.terrainType	= ( TERRAIN_TYPE ) EditorGUILayout.EnumPopup ( new GUIContent("Terrain Type", toolTip), _settings.terrainType );

		toolTip = "Warp world around pivot";
		_settings.bendType	= (BEND_TYPE) EditorGUILayout.EnumPopup ( new GUIContent("Warp Type", toolTip), _settings.bendType );

		toolTip = "Fog Mode: \nNeeds ShaderOneManager.cs in scene";
		_settings.fogMode	= ( FOG_MODE ) EditorGUILayout.EnumPopup ( new GUIContent("Fog Mode", toolTip), _settings.fogMode );

		if ( _settings.fogMode == FOG_MODE.VOLUMETRIC_3D || _settings.fogMode == FOG_MODE.VOLUMETRIC )
		{
			  toolTip = "Make final image affect Fog Lighting";
			  _settings.fogImageLight	= EditorGUILayout.Toggle(new GUIContent("Fog Image Light", toolTip), _settings.fogImageLight );
		}

		if ( _settings.fogMode == FOG_MODE.VOLUMETRIC_3D )
		{
			toolTip = "Make top and bottom of Fog change more";
			_settings.fogRoughness	= EditorGUILayout.Toggle(new GUIContent("Fog Roughness", toolTip), _settings.fogRoughness );

		  toolTip = "How many rays to cast out for volumetric Fog\n ( less is faster )";
			_settings.fogRays = EditorGUILayout.IntSlider(new GUIContent("Ray Cast Count", toolTip), _settings.fogRays, 3, 32 );
		}

		toolTip = "Instancing: \n GPU Instancing";
		_settings.instancing = EditorGUILayout.Toggle(new GUIContent("GPU Instancing", toolTip), _settings.instancing );

		toolTip = "Color, Movment and Terrain Splat Control";
		_settings.vertexOptions	= EditorGUILayout.Toggle(new GUIContent("Vertex Options", toolTip), _settings.vertexOptions );

		toolTip = "Alpha Map: \nSelect what Alpha channel is used for";
		_settings.alphaMap    		= EditorGUILayout.Toggle(new GUIContent("AlphaMap", toolTip), _settings.alphaMap );

		toolTip = "Use Unity baked light maps";
		_settings.bakedLightMapping 		= EditorGUILayout.Toggle(new GUIContent("Baked Light Mapping", toolTip), _settings.bakedLightMapping );

		toolTip = "Use Unity dynamic light maps";
		_settings.dynamicLightMapping 		= EditorGUILayout.Toggle(new GUIContent("Dynamic Light Mapping", toolTip), _settings.dynamicLightMapping );

		toolTip = "Amplify the combined layers RGB beyond 1.0";
		_settings.colorAmplify	= EditorGUILayout.Toggle(new GUIContent("Color Amplify", toolTip), _settings.colorAmplify );

		toolTip = "UV1 mapping can be normal or based on world position";
		_settings.uvWorldMap	= EditorGUILayout.Toggle(new GUIContent("UV World Mapping", toolTip), _settings.uvWorldMap );

		toolTip = "Fix normal maps exported from substance painter using UE4 packed option";
		_settings.directXNormal	= EditorGUILayout.Toggle(new GUIContent("DirectX Normal Map", toolTip), _settings.directXNormal );

		toolTip = "Adds slider to scale normal maps on just main layer, all layers or none";
		_settings.bumpScale	= ( SCALE_TYPE ) EditorGUILayout.EnumPopup ( new GUIContent("Normal Map Scale", toolTip), (SCALE_TYPE)_settings.bumpScale );

		toolTip = "Adds slider to scale Ambeint Occlusion on just main layer, all layers or none";
		_settings.aoScale	= ( SCALE_TYPE ) EditorGUILayout.EnumPopup ( new GUIContent("Ambient Occlusion Scale", toolTip), (SCALE_TYPE)_settings.aoScale );

		toolTip = "Force import of surface map to Linear or sRGB";
		_settings.surfaceMapImport	= ( SURFACE_MAP_IMPORT ) EditorGUILayout.EnumPopup ( new GUIContent("SurfaceMap Import", toolTip), _settings.surfaceMapImport );

		toolTip = "What Surface Map R channel is used for";
		_settings.surfaceMapR	= ( MAP_CHANNEL ) EditorGUILayout.EnumPopup ( new GUIContent("SurfaceMap(R)", toolTip), (MAP_CHANNEL_GEN)_settings.surfaceMapR );

		toolTip = "What Surface Map G channel is used for";
		_settings.surfaceMapG	= ( MAP_CHANNEL ) EditorGUILayout.EnumPopup ( new GUIContent("SurfaceMap(G)", toolTip), (MAP_CHANNEL_GEN)_settings.surfaceMapG );

		toolTip = "What Surface Map B channel is used for";
		_settings.surfaceMapB	= ( MAP_CHANNEL ) EditorGUILayout.EnumPopup ( new GUIContent("SurfaceMap(B)", toolTip), (MAP_CHANNEL_GEN)_settings.surfaceMapB );

		toolTip = "What Surface Map A channel is used for";
		_settings.surfaceMapA	= ( MAP_CHANNEL ) EditorGUILayout.EnumPopup ( new GUIContent("SurfaceMap(A)", toolTip), (MAP_CHANNEL_GEN)_settings.surfaceMapA );

		ShaderOneGUI.Line();
		_settings.blendFoldout = EditorGUILayout.Foldout ( _settings.blendFoldout, "Blend Modes" );

		if ( _settings.blendFoldout )
		{
			EditorGUI.BeginDisabledGroup ( true );
			_settings.blendModeMask = BitMaskToggle ( "Solid", toolTip, _settings.blendModeMask, 0 );
			_settings.blendModeMask = ShaderOneIO.SetBit ( _settings.blendModeMask, 0, true );
			EditorGUI.EndDisabledGroup();

			_settings.blendModeMask = BitMaskToggle ( "Cutout", toolTip, _settings.blendModeMask, 1 );
			_settings.blendModeMask = BitMaskToggle ( "Fade", toolTip, _settings.blendModeMask, 2 );
			_settings.blendModeMask = BitMaskToggle ( "Transparent", toolTip, _settings.blendModeMask, 3 );
			_settings.blendModeMask = BitMaskToggle ( "Additive", toolTip, _settings.blendModeMask, 4 );
			_settings.blendModeMask = BitMaskToggle ( "Additive Soft", toolTip, _settings.blendModeMask, 5 );
			_settings.blendModeMask = BitMaskToggle ( "Additive Blend", toolTip, _settings.blendModeMask, 6 );
			_settings.blendModeMask = BitMaskToggle ( "Additive Alpha", toolTip, _settings.blendModeMask, 7 );
			_settings.blendModeMask = BitMaskToggle ( "Multiply", toolTip, _settings.blendModeMask, 8 );
		}

		if ( _settings.renderPipeline != RENDER_PIPELINE.UNLIT )
		{
			ShaderOneGUI.Line();
			_settings.lightingFoldout = EditorGUILayout.Foldout ( _settings.lightingFoldout, "Lighting Options" );

			if ( _settings.lightingFoldout )
			{
				toolTip = "PBR: on or off ( off is faster )\n";
				_settings.pbr = EditorGUILayout.Toggle(new GUIContent("PBR", toolTip), _settings.pbr );

				toolTip = "Workflow";
				_settings.workflow	= ( WORKFLOW_MODE ) EditorGUILayout.EnumPopup ( new GUIContent("Workflow", toolTip), _settings.workflow );

				if ( _settings.workflow  == WORKFLOW_MODE.SPECULAR )
				{
					toolTip = "What Specular Map A channel is used for";
					_settings.specularMap = ( MAP_CHANNEL ) EditorGUILayout.EnumPopup ( new GUIContent("Specular Map(A)", toolTip), (MAP_CHANNEL_GEN)_settings.specularMap );
				}

				toolTip = "Use Fresnel for refelctions and specular\n";
				_settings.fresnel = EditorGUILayout.Toggle(new GUIContent("Fresnel", toolTip), _settings.fresnel );

				if ( _settings.fresnel )
				{
					toolTip = "Fresnel Quality\n";
					_settings.fresnelPow = ( FRESNEL_POW ) EditorGUILayout.EnumPopup ( new GUIContent("Fresnel Power", toolTip), _settings.fresnelPow );
				}

				toolTip = "Specular: \nGlossy light reflection";
				_settings.specularMode	= ( SPECULAR_MODE ) EditorGUILayout.EnumPopup ( new GUIContent("Specular Mode", toolTip), _settings.specularMode );

				toolTip = "Specular Blend - how its blended into albedo";
				_settings.specularBlend	= ( SPECULAR_BLEND ) EditorGUILayout.EnumPopup ( new GUIContent("Specular Blend", toolTip), _settings.specularBlend );

				toolTip = "Shadows: \nAllows you to use shadows with a single directional light";
				_settings.shadows    		= EditorGUILayout.Toggle(new GUIContent("Shadows", toolTip), _settings.shadows );

				toolTip = "Light Probes: \nStores baked and ambient lighting in the scene";
				_settings.lightProbes		= EditorGUILayout.Toggle(new GUIContent("LightProbes", toolTip), _settings.lightProbes );

				if ( _settings.renderPipeline == RENDER_PIPELINE.SHADERONE_LIGHTING )
				{
					EditorGUILayout.GetControlRect ( false, 2 );

					if ( _settings.dirArraySize > 0 )
					{
						DisplayLightStats ( "Directional", _settings.dirArraySize, _settings.dirPerPixelCount );
						toolTip = "Per-Pixel: \nHow many directional lights you have per pixel";
						_settings.dirPerPixelCount = DisplayLightSlider ( _settings.dirPerPixelCount, _settings.dirArraySize );
					}
					else
					{
						GUILayout.Label("Directional Lights Disabled");
						ShaderOneGUI.Line();
					}

					if ( _settings.pointArraySize > 0 )
					{
						DisplayLightStats ( "Point", _settings.pointArraySize, _settings.pointPerPixelCount );
						toolTip = "Per-Pixel: \nHow many point lights you have per pixel";
						_settings.pointPerPixelCount = DisplayLightSlider ( _settings.pointPerPixelCount, _settings.pointArraySize );
					}
					else
					{
						GUILayout.Label("Point Lights Disabled");
						ShaderOneGUI.Line();
					}

					if (_settings.spotArraySize > 0 )
					{
						DisplayLightStats ( "Spot", _settings.spotArraySize, _settings.spotPerPixelCount );
						toolTip = "Per-Pixel: \nHow many spot lights you have per pixel";
						_settings.spotPerPixelCount = DisplayLightSlider ( _settings.spotPerPixelCount, _settings.spotArraySize );
					}
					else
					{
						GUILayout.Label("Spot Lights Disabled");
						ShaderOneGUI.Line();
					}

					lightingAdvancedFoldout = EditorGUILayout.Foldout ( lightingAdvancedFoldout, "Advanced Lighting Options" );
					if ( lightingAdvancedFoldout )
					{
						float oldNum;

						GUILayout.Label("Maximum Number Lights\nChanging this may require a Restart Of Unity", EditorStyles.boldLabel );

						oldNum = _settings.dirArraySize;
						toolTip = "Directinal Array Size: \nTotal possible directional lights per pass";
						_settings.dirArraySize 		= EditorGUILayout.IntSlider(new GUIContent("Directional", toolTip), _settings.dirArraySize, 0, 4 );
						if ( oldNum != _settings.dirArraySize )
						{
							if ( _settings.dirPerPixelCount > _settings.dirArraySize )
								_settings.dirPerPixelCount = _settings.dirArraySize;

							lightingAdvancedChanged = true;
						}

						oldNum = _settings.pointArraySize;
						toolTip = "Point Array Size: \nTotal possible point lights per pass";
						_settings.pointArraySize	= EditorGUILayout.IntSlider(new GUIContent("Point", toolTip), _settings.pointArraySize, 0, 32 );
						if ( oldNum != _settings.pointArraySize )
						{
							if ( _settings.pointPerPixelCount > _settings.pointArraySize )
								_settings.pointPerPixelCount = _settings.pointArraySize;

							lightingAdvancedChanged = true;
						}

						oldNum = _settings.spotArraySize;
						toolTip = "Spot Array Size: \nTotal possible point lights per pass";
						_settings.spotArraySize 	= EditorGUILayout.IntSlider(new GUIContent("Spot", toolTip), _settings.spotArraySize, 0, 32 );
						if ( oldNum != _settings.spotArraySize )
						{
							if ( _settings.spotPerPixelCount > _settings.spotArraySize )
								_settings.spotPerPixelCount = _settings.spotArraySize;

							lightingAdvancedChanged = true;
						}
					}
				}

				if ( _settings.renderPipeline == RENDER_PIPELINE.UNITY_FORWARD )
				{
					toolTip = "Light Cookies: \nEnable use of texture cookies";
					_settings.lightCookies 		= EditorGUILayout.Toggle(new GUIContent("LightCookies", toolTip), _settings.lightCookies );
				}
			}
		}

        GetUserOptions();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

		ShaderOneGUI.Line(2);
		EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

		if ( GUILayout.Button("?", GUILayout.Width(24)) )
		{
			ShaderOneIO.OpenDocPDF("ShaderOne_Generator_Docs");
		}

		if (GUILayout.Button("Default Options", GUILayout.Width(128)))
		{
			if(EditorUtility.DisplayDialog("Default Options", "Are you sure you want to reset back to default options?", "Yes!", "No."))
			ShaderOneIO.SetDefaults(ref _settings);
		}

		if (GUILayout.Button("Save", GUILayout.Width(64)))
		{
			ShaderOneIO.SaveAs(ref _settings);
		}

		if (GUILayout.Button("Load", GUILayout.Width(64)))
		{
			ShaderOneIO.LoadAs(ref _settings);
		}

        EditorGUILayout.GetControlRect(false, GUILayout.MinWidth(-3));

		if ( GUILayout.Button( "Generate Shader", GUILayout.Width ( 128 ) ) )
		{
			bool reimport = false;

			ShaderOneIO.SaveSettings( ref _settings );
			if ( EditorUtility.DisplayDialog("ShaderOne", "Reimport Materials and Set Keywords ?", "Yes", "No" ) )
				reimport = true;

			ShaderOneTools.GenerateShader ( reimport );

			if ( ShaderOneManagerCheck() )
				ShaderOneManager.ResetManager();

			ShaderOneMaterialEditor.reloadSettings = 2;

			if ( lightingAdvancedChanged )
			{
				EditorUtility.DisplayDialog("**WARNING**", "You Have changed settings that Require you to Restart Unity", "OK" );
			}
    	}

		EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

		EndWindows();
    }

}

