using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShaderOneSystem;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShaderOneIO
{
#if UNITY_EDITOR
	private static string _fileName 			= null;
	private static string _shaderOneFolder      = null;
#endif

#if UNITY_EDITOR
	//============================================================
	public static string FindRootFolder()
	{
		string[] assetPaths = AssetDatabase.GetAllAssetPaths();
		int loop;

		for ( loop = 0; loop < assetPaths.Length; loop++ )
		{
			if ( assetPaths[loop].IndexOf("/ShaderOne/Shaders/ShaderOneFunc.cginc") > 0 )
			{
				_shaderOneFolder = assetPaths[loop].Replace ( "/Shaders/ShaderOneFunc.cginc", "" );
				break;
			}
		}

		return ( _shaderOneFolder );
	}

	//============================================================
	public static void OpenDocPDF ( string i_filename )
	{
		string result = "";

		FindRootFolder();

		result =  "file://" + Application.dataPath + _shaderOneFolder.Replace ("Assets","") +"/!Docs/" + i_filename + ".pdf";

		Application.OpenURL( result );
	}

	//============================================================
	public static void GetPath()
	{
		FindRootFolder();
		_fileName 			= _shaderOneFolder + "/Resources/ShaderOneSettings.json" ;
	}
#endif

	//============================================================
	public static void SetDefaults ( ref ShaderOneSettings i_sos )
	{
		i_sos = new ShaderOneSettings();

		i_sos.renderPipeline 		= RENDER_PIPELINE.UNITY_FORWARD;
		i_sos.colorPrecision 		= COLOR_PRECISION.HALF;
		i_sos.reflectionType        = REFLECTION_TYPE.REFLECTION_PROBE;
		i_sos.terrainType           = TERRAIN_TYPE.NONE;
		i_sos.bendType              = BEND_TYPE.OFF;
		i_sos.fogMode	            = FOG_MODE.OFF;
		i_sos.fogRays               = 16;
		i_sos.fogRoughness          = false;
		i_sos.instancing			= false;
		i_sos.alphaMap 		        = true;
		i_sos.bakedLightMapping    	= false;
		i_sos.dynamicLightMapping  	= false;
		i_sos.vertexOptions         = false;
		i_sos.bumpScale             = SCALE_TYPE.GLOBAL;
		i_sos.aoScale             	= SCALE_TYPE.GLOBAL;

		i_sos.surfaceMapR			= MAP_CHANNEL.METALLIC;
		i_sos.surfaceMapG   		= MAP_CHANNEL.EMPTY;
		i_sos.surfaceMapB   		= MAP_CHANNEL.EMPTY;
		i_sos.surfaceMapA   		= MAP_CHANNEL.SMOOTHNESS;

		i_sos.surfaceMapImport		= SURFACE_MAP_IMPORT.MANUAL;

		i_sos.colorAmplify         	= false;
		i_sos.directXNormal			= false;
		i_sos.uvWorldMap            = false;

		i_sos.distortion			= true;
		i_sos.emission				= true;
		i_sos.chromaticAbberation	= true;
		i_sos.saturation    		= true;
		i_sos.scanlines     		= true;
		i_sos.rimLighting  			= false;
		i_sos.intersect     		= false;
		i_sos.pbr 					= true;
		i_sos.fresnel               = true;
		i_sos.fresnelPow            = FRESNEL_POW.NORMAL;
		i_sos.workflow              = WORKFLOW_MODE.SMOOTHNESS;
		i_sos.specularMap           = MAP_CHANNEL.SMOOTHNESS;
		i_sos.specularMode          = SPECULAR_MODE.NORMAL;
		i_sos.specularBlend         = SPECULAR_BLEND.COLOR;
		i_sos.shadows   			= false;
		i_sos.lightCookies          = false;
		i_sos.lightProbes           = true;

		i_sos.blendModeMask         = 0xFF;  // ALL ON by default

		i_sos.dirArraySize          = 1;
		i_sos.dirPerPixelCount		= 1;

		i_sos.pointArraySize        = 4;
		i_sos.pointPerPixelCount	= 4;

		i_sos.spotArraySize        	= 4;
		i_sos.spotPerPixelCount		= 4;

		i_sos.layer0.enabled  		= true;
		i_sos.layer0.surfaceMap  	= true;
		i_sos.layer0.uv2            = UV2_OPTION.OFF;
		i_sos.layer0.scriptToggle  	= false;
		i_sos.layer0.normalMap      = true;
		i_sos.layer0.flowMap      	= false;
		i_sos.layer0.animOptions	= false;
		i_sos.layer0.scrollUV 		= true;
		i_sos.layer0.rotateUV 		= true;
		i_sos.layer0.blendModes		= false;

		i_sos.layer1.enabled  		= true;
		i_sos.layer1.surfaceMap  	= false;
		i_sos.layer1.uv2            = UV2_OPTION.OFF;
		i_sos.layer1.scriptToggle  	= false;
		i_sos.layer1.normalMap      = false;
		i_sos.layer1.flowMap      	= false;
		i_sos.layer1.animOptions	= false;
		i_sos.layer1.scrollUV 		= false;
		i_sos.layer1.rotateUV 		= false;
		i_sos.layer1.blendModes		= false;

		i_sos.layer2.enabled  		= false;
		i_sos.layer2.surfaceMap  	= false;
		i_sos.layer2.uv2            = UV2_OPTION.OFF;
		i_sos.layer2.scriptToggle  	= false;
		i_sos.layer2.normalMap      = false;
		i_sos.layer2.flowMap      	= false;
		i_sos.layer2.animOptions	= false;
		i_sos.layer2.scrollUV 		= false;
		i_sos.layer2.rotateUV 		= false;
		i_sos.layer2.blendModes		= false;

		i_sos.layer3.enabled  		= false;
		i_sos.layer3.surfaceMap  	= false;
		i_sos.layer3.uv2            = UV2_OPTION.OFF;
		i_sos.layer3.scriptToggle  	= false;
		i_sos.layer3.normalMap      = false;
		i_sos.layer3.flowMap      	= false;
		i_sos.layer3.animOptions	= false;
		i_sos.layer3.scrollUV 		= false;
		i_sos.layer3.rotateUV 		= false;
		i_sos.layer3.blendModes		= false;

		i_sos.lightingFoldout       = true;
		i_sos.layer0Foldout			= false;
		i_sos.layer1Foldout			= false;
		i_sos.layer2Foldout			= false;
		i_sos.layer3Foldout			= false;
		i_sos.effectsFoldout    	= true;
		i_sos.managerScriptFlag     = false;

		SaveSettings ( ref i_sos );
	}

	//-------------------------------------------------------------------------
	public static int SetBit ( int i_num, int i_bitPos, bool i_flag )
	{
		if ( !i_flag )
			i_num &= ~( 1 << i_bitPos );
		else
			i_num |= ( 1 << i_bitPos );

		return ( i_num );
	}

	//-------------------------------------------------------------------------
	public static bool GetBit ( int i_num, int i_bitPos )
	{
		bool        bit;

		bit = ( i_num & ( 1 << i_bitPos ) ) != 0;

		return ( bit );
	}

	//============================================================
	public static int CountBitsNeeded ( int i_val )
	{
		int loop;
		int count = 0;
		int pwr2;

		// round to nearest power of 2
		pwr2 = (int)Mathf.Pow ( 2, Mathf.Ceil ( Mathf.Log ( i_val ) / Mathf.Log ( 2 ) ) );

		for ( loop = 30; loop >= 0; loop-- )
		{
			if ( GetBit ( pwr2, loop ) )
			{
				count = loop;
				break;
			}
		}

		return(count);
	}

	//============================================================
	public static bool UnityVersion ( int i_mainVer, int i_x, int i_y )
	{
		string sver = Application.unityVersion;
		string [] parts = sver.Split('.');
		int mainNum = 0;
		int x = 0;
		int y = 0;

		if ( parts.Length > 0 )
			mainNum = int.Parse(parts[0]);

		if ( parts.Length > 1 )
			x = int.Parse(parts[1]);

		if ( parts.Length > 2 && parts[2].Length == 1 )
			y = int.Parse(parts[2]);

		if ( i_mainVer >= mainNum && i_x >= x && i_y >= y )
			return ( true );

		return ( false );
	}

	//============================================================
	public static string FindAsset ( string i_name )
	{
#if UNITY_EDITOR
		string[] assetPaths = AssetDatabase.GetAllAssetPaths();
		int loop;

		for ( loop = 0; loop < assetPaths.Length; loop++ )
		{
			if ( assetPaths[loop].IndexOf( i_name ) >= 0 )
			{
				return ( assetPaths[loop] );
			}
		}

#endif
		return( null );
	}

	//============================================================
	public static void DeleteSettingsFile()
	{
#if UNITY_EDITOR
		FindRootFolder();
		GetPath();
		File.Delete ( _fileName );
#endif
	}

#if UNITY_EDITOR
	//============================================================
	public static void DeleteShaderOneGenFiles()
	{
		string path;

		FindRootFolder();
		GetPath();

		path = ShaderOneIO.FindAsset ("ShaderOneVersionCompiled.json");
		if ( path != null )
			File.Delete ( path );

		// SCOTTFIND
		//path = ShaderOneIO.FindAsset ("ShaderOneGen.shader");
		//if ( path != null )
		//	File.Delete ( path );

		//path = ShaderOneIO.FindAsset ("ShaderOneGen_BitDecode.cginc");
		//if ( path != null )
		//	File.Delete ( path );

		//path = ShaderOneIO.FindAsset ("ShaderOneGen_FuncMaps.cginc");
		//if ( path != null )
		//	File.Delete ( path );

		//path = ShaderOneIO.FindAsset ("ShaderOneGen_Instancing.cginc");
		//if ( path != null )
		//	File.Delete ( path );

		//path = ShaderOneIO.FindAsset ("ShaderOneGen_UnityLightMacros.cginc");
		//if ( path != null )
		//	File.Delete ( path );
	}

	//============================================================
	public static void SaveVersion ( ShaderOneVersion i_sov, bool i_compiledVersion = false )
	{
		string saveText;
		string fileName;

		i_sov.turboGloveActivate = UnityEngine.Random.Range ( 2048, 64738 );

		saveText = JsonUtility.ToJson ( i_sov );

		FindRootFolder();
		GetPath();

		fileName = _shaderOneFolder + "/Resources/ShaderOneVersion" ;

		if ( i_compiledVersion )
			fileName += "Compiled.json" ;
		else
			fileName += ".json" ;

		if ( saveText != null )
		{
			File.Delete ( fileName );
			File.WriteAllText ( fileName, saveText );
		}
	}

	//============================================================
	public static void SaveVersion ( string i_version, bool i_compiledVersion = false )
	{
		ShaderOneVersion sov = new ShaderOneVersion();

		sov.version = i_version;

		SaveVersion ( sov, i_compiledVersion );
	}

	//============================================================
	public static ShaderOneVersion LoadVersionSOV ( bool i_compiledVersion = false )
	{
		ShaderOneVersion 		sov = new ShaderOneVersion();
		string 					loadedOutput = "";
		string 					fileName;

		sov.turboGloveActivate = 0;

		if (i_compiledVersion )
			sov.version = "Apple";
		else
			sov.version = "TheKid";

		GetPath();

		fileName = _shaderOneFolder + "/Resources/ShaderOneVersion" ;

		if ( i_compiledVersion )
			fileName += "Compiled.json" ;
		else
			fileName += ".json" ;

		if ( File.Exists ( fileName ) )
			loadedOutput = File.ReadAllText ( fileName );

		if ( loadedOutput != null && loadedOutput.Length > 4 )
			sov = JsonUtility.FromJson<ShaderOneVersion>(loadedOutput);

		return(sov);
	}

	//============================================================
	public static string LoadVersion ( bool i_compiledVersion = false )
	{
		ShaderOneVersion 		sov;
		string 					version = "Zeus";

		sov 	= LoadVersionSOV ( i_compiledVersion );
		version = sov.version;

		return(version);
	}
#endif

	//============================================================
	public static void SaveSettings( ref ShaderOneSettings i_settings )
	{
#if UNITY_EDITOR
		string loadedInput;

		FindRootFolder();
		GetPath();

		loadedInput = JsonUtility.ToJson ( i_settings );

		GetPath();

		if ( !AssetDatabase.IsValidFolder ( _shaderOneFolder + "/Resources") )
			AssetDatabase.CreateFolder ( _shaderOneFolder, "Resources" );

		if ( loadedInput != null )
		{
			File.Delete ( _fileName );
			File.WriteAllText ( _fileName, loadedInput );
		}
#endif

	}

	//============================================================
	public static void SaveAs ( ref ShaderOneSettings i_settings )
	{
#if UNITY_EDITOR
		string path = EditorUtility.SaveFilePanel("Save Settings", Application.dataPath, "ShaderOneUserSettings", "json");

		if ( path != null && path.Length > 1 )
		{
			string loadedInput;
			loadedInput = JsonUtility.ToJson ( i_settings );

			if ( loadedInput != null )
				File.WriteAllText ( path, loadedInput );
		}
#endif
	}

	//============================================================
	public static void LoadAs ( ref ShaderOneSettings i_settings )
	{
#if UNITY_EDITOR
		string path = EditorUtility.OpenFilePanel("Load Settings", Application.dataPath, "json" );
		string loadedOutput = "";

		if ( path != null )
		{
			if ( File.Exists ( path ) )
			{
				loadedOutput = File.ReadAllText ( path );

				if ( loadedOutput != null )
				{
					i_settings = JsonUtility.FromJson<ShaderOneSettings>(loadedOutput);
					i_settings.loaded = 1;
				}
			}
		}
#endif
	}

#if UNITY_EDITOR
	//============================================================
	public static bool Exists()
	{
		GetPath();
		return ( File.Exists ( _fileName ) );
	}
#endif

	//============================================================
	public static void LoadSettings( ref ShaderOneSettings i_settings )
	{
		string loadedOutput = "";

#if UNITY_EDITOR
		GetPath();

		if ( File.Exists ( _fileName ) )
			loadedOutput = File.ReadAllText ( _fileName );
#else
		TextAsset ta;
		ta = (TextAsset)Resources.Load("ShaderOneSettings");

		if ( ta != null )
			loadedOutput = ta.text;
#endif

		if ( loadedOutput != null && loadedOutput.Length > 4 )
		{
			i_settings = JsonUtility.FromJson<ShaderOneSettings>(loadedOutput);
			i_settings.loaded = 1;
		}
		else
		{
			// settings must have gotten corrupted so lets remake it
			SetDefaults( ref i_settings );
			SaveSettings ( ref i_settings );
		}
	}

	//============================================================
	public static void SaveCompressedBytes ( string i_fileName, byte[] i_array )
	{
		i_array = ULZF.Compress ( i_array );
		File.WriteAllBytes ( i_fileName, i_array );
	}

	//============================================================
	public static byte[] LoadCompressedBytes ( string i_fileName )
	{
		byte[] tmpArray;

		tmpArray = File.ReadAllBytes ( i_fileName );

		tmpArray = ULZF.Decompress ( tmpArray );

		return ( tmpArray );
	}

	//============================================================
	public static bool FastNoiseExists()
	{
		Type myType = Type.GetType("FastNoise");

		if (myType != null)
			return(true);

		return(false);
	}
}


