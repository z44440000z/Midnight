#if SHADERONE_USE_FASTNOISE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using ShaderOneSystem;

//----------------------------------------------------------------------------
public class ShaderOneFogTextureDesigner : EditorWindow
{
	static FastNoiseSettings _settings = null;
	static string _fileName;


	[Serializable]
	public class FastNoiseSettings
	{
		public int 									seed;
		public float 								frequency;
		public FastNoise.Interp 					interp;
		public FastNoise.NoiseType 					noiseType;
		public int 									octaves;
		public float 								lacunarity;
		public float 								gain;
		public FastNoise.FractalType 				fractalType;
		public FastNoise.CellularDistanceFunction 	cellularDistanceFunction;
		public FastNoise.CellularReturnType 		cellularReturnType;
		public FastNoiseUnity 						cellularNoiseLookup;
		public int 									cellularDistanceIndex0;
		public int 									cellularDistanceIndex1;
		public float 								cellularJitter;
		public float 								gradientPerturbAmp;
	}

	private float[] 	_fogCube;
	private int     	_fogSize 	= 64;
	private Texture2D 	_fogPreview = null;
	private int       	_sliceIndex = 32;
	Texture soLogo;
	Vector2 scrollPos 	= Vector2.zero;
	bool    initFlag 	= false;

	//============================================================
    [MenuItem("Window/ShaderOne Fog Texture Designer")]
    public static void Init()
	{
		string shaderOneRoot = ShaderOneIO.FindRootFolder();
		_fileName	= shaderOneRoot + "/Editor/Settings/ShaderOneFogTextureDesigner.json";

        ShaderOneFogTextureDesigner window = (ShaderOneFogTextureDesigner)EditorWindow.GetWindow(typeof(ShaderOneFogTextureDesigner), false, "Fog Texture", true );

		window.minSize = new Vector2 (420,300);

        window.Show();
    }

	//============================================================
	void Awake()
	{
		string shaderOneRoot = ShaderOneIO.FindRootFolder();
		_fileName	= shaderOneRoot + "/Editor/Settings/ShaderOneFogTextureDesigner.json";
		soLogo = AssetDatabase.LoadAssetAtPath<Texture>(shaderOneRoot + "/Logos/ShaderOneLogo.psd");
	}

	//============================================================
	private void SaveSettings()
	{
		string loadedInput;

		loadedInput = JsonUtility.ToJson ( _settings );

		if ( loadedInput != null )
		{
		  File.Delete ( _fileName );
		  File.WriteAllText ( _fileName, loadedInput );
		}
	}

	//============================================================
	private void LoadSettings()
	{
		string loadedOutput;

		if ( File.Exists ( _fileName ) )
		{
			loadedOutput = File.ReadAllText ( _fileName );

			if ( loadedOutput != null )
			{
				_settings = JsonUtility.FromJson<FastNoiseSettings>(loadedOutput);
			}
		}
		else
		{
			SetDefaults();
		}
	}

	//============================================================
	private void SetDefaults()
	{
		_settings = new FastNoiseSettings();

		_settings.seed 						= 1138;
		_settings.frequency					= 0.1f;
		_settings.interp					= FastNoise.Interp.Quintic;
		_settings.noiseType					= FastNoise.NoiseType.SimplexFractal;
		_settings.octaves					= 4;
		_settings.lacunarity				= 2.0f;
		_settings.gain						= 0.5f;
		_settings.fractalType				= FastNoise.FractalType.FBM;
		_settings.cellularDistanceFunction  = FastNoise.CellularDistanceFunction.Euclidean;
		_settings.cellularReturnType		= FastNoise.CellularReturnType.CellValue;
		_settings.cellularNoiseLookup		= null;
		_settings.cellularDistanceIndex0	= 0;
		_settings.cellularDistanceIndex1	= 1;
		_settings.cellularJitter			= 0.45f;
		_settings.gradientPerturbAmp		= 1.0f;
	}

	//============================================================
	private void OnDestroy()
	{
		SaveSettings();
	}

	//============================================================
	private void SetNoiseSettings( FastNoise i_fastNoise )
	{
		i_fastNoise.SetSeed(_settings.seed);
		i_fastNoise.SetFrequency(_settings.frequency);
		i_fastNoise.SetInterp(_settings.interp);
		i_fastNoise.SetNoiseType(_settings.noiseType);

		i_fastNoise.SetFractalOctaves(_settings.octaves);
		i_fastNoise.SetFractalLacunarity(_settings.lacunarity);
		i_fastNoise.SetFractalGain(_settings.gain);
		i_fastNoise.SetFractalType(_settings.fractalType);

		i_fastNoise.SetCellularDistanceFunction(_settings.cellularDistanceFunction);
		i_fastNoise.SetCellularReturnType(_settings.cellularReturnType);
		i_fastNoise.SetCellularJitter(_settings.cellularJitter);
		i_fastNoise.SetCellularDistance2Indicies(_settings.cellularDistanceIndex0, _settings.cellularDistanceIndex1);

		if (_settings.cellularNoiseLookup)
			i_fastNoise.SetCellularNoiseLookup(_settings.cellularNoiseLookup.fastNoise);

		i_fastNoise.SetGradientPerturbAmp(_settings.gradientPerturbAmp);
	}

	//============================================================
	private void Save3DTexture()
	{
		int lx,ly,lz;
		int index;
		byte[] sotex = new byte[_fogCube.Length];

		for ( lx = 0; lx < _fogSize; lx++ )
		{
			for ( ly = 0; ly < _fogSize; ly++ )
			{
				for ( lz = 0; lz < _fogSize; lz++ )
				{
					index = lx + ( ly * _fogSize ) + ( lz * _fogSize * _fogSize );

					sotex[index] = (byte)( _fogCube[index] * 255.0f );
				}
			}
		}

		string path = EditorUtility.SaveFilePanel("Save 3D Fog", Application.dataPath + "/echoLogin/ShaderOne/Resources/", "ShaderOneDefaultFog.bytes", "bytes");

		if ( path != "" && path.Length > 1 )
		{
			ShaderOneIO.SaveCompressedBytes ( path, sotex );
		}
	}

	//============================================================
 	private void GeneratePreview()
	{
		int lx,ly;
		int index;
		Color[] _pixels;
		int findex;

		_pixels = new Color[_fogSize*_fogSize*_fogSize];

		for ( lx = 0; lx < _fogSize; lx++ )
		{
			for ( ly = 0; ly < _fogSize; ly++ )
			{
				index = lx + ( ly * _fogSize );

				findex = index + ( _sliceIndex * _fogSize * _fogSize );
				_pixels[index] = new Color ( _fogCube[findex], _fogCube[findex], _fogCube[findex], 1 );
			}
		}

		_fogPreview = new Texture2D ( 64, 64, TextureFormat.RGB24, false );
		_fogPreview.SetPixels ( _pixels );
//		_fogPreview.filterMode = FilterMode.Point;
		_fogPreview.Apply();
	}

	//============================================================
	private void Generate()
	{
		int lx,ly,lz;
		int index;
		float px,py,pz;
		float lerpX;
		float lerpY;
		float lerpZ;
		float []  tempCube;
		float min,max;
		float scale;

		min = Single.MaxValue;
		max = Single.MinValue;

		FastNoise fastNoise = new FastNoise();
		SetNoiseSettings ( fastNoise );

		_fogCube = new float[_fogSize*_fogSize*_fogSize];
		tempCube = new float[_fogSize*_fogSize*_fogSize];

		for ( lx = 0; lx < _fogSize; lx++ )
		{
			for ( ly = 0; ly < _fogSize; ly++ )
			{
				for ( lz = 0; lz < _fogSize; lz++ )
				{
					index = lx + ( ly * _fogSize ) + ( lz * _fogSize * _fogSize );

					px = Mathf.Abs( (float)lx - 32 );
					py = Mathf.Abs( (float)ly - 32 );
					pz = Mathf.Abs( (float)lz - 32 );

					tempCube[index] = fastNoise.GetNoise(px,py,pz);
				}
			}
		}


		for ( lx = 0; lx < _fogSize; lx++ )
		{
			for ( ly = 0; ly < _fogSize; ly++ )
			{
				for ( lz = 0; lz < _fogSize; lz++ )
				{
					index = lx + ( ly * _fogSize ) + ( lz * _fogSize * _fogSize );

					px = (float)lx;
					py = (float)ly;
					pz = (float)lz;

					_fogCube[index] = fastNoise.GetNoise(px,py,pz);
    			}
			}
		}

		for ( lx = 0; lx < _fogSize; lx++ )
		{
			for ( ly = 0; ly < _fogSize; ly++ )
			{
				for ( lz = 0; lz < _fogSize; lz++ )
				{
					index = lx + ( ly * _fogSize ) + ( lz * _fogSize * _fogSize );

					px = (float)lx;
					py = (float)ly;
					pz = (float)lz;

					if ( lx <= 31 )
						px = -( px - 31.0f );
					else
						px -= 32.0f;

					if ( ly <= 31 )
						py = -( py - 31.0f );
					else
						py -= 32.0f;

					if ( lz <= 31 )
						pz = -( pz - 31.0f );
					else
						pz -= 32.0f;

					lerpX = px / 31.0f;
					lerpY = py / 31.0f;
					lerpZ = pz / 31.0f;

					lerpX = Mathf.Pow ( lerpX, 6 );
					lerpY = Mathf.Pow ( lerpY, 6 );
					lerpZ = Mathf.Pow ( lerpZ, 6 );

				    _fogCube[index] = Mathf.Lerp ( _fogCube[index], tempCube[index], Mathf.Max(lerpX,Mathf.Max(lerpY, lerpZ )));
//					_fogCube[index] = Mathf.Max(lerpX,lerpY );
//					_fogCube[index] = tempCube[index];
					min = Mathf.Min ( min, _fogCube[index] );
					max = Mathf.Max ( max, _fogCube[index] );
				}
			}
		}

		scale = 1.0f / ( max - min );

		for ( lx = 0; lx < _fogCube.Length; lx++ )
		{
			_fogCube[lx] = Mathf.Clamp ( ( _fogCube[lx] - min ) * scale, 0.0f, 1.0f );
		}

		GeneratePreview();
	}

	//============================================================
    private void OnGUI()
	{
		string toolTip ="";

		if ( _settings == null )
		{
			LoadSettings();
		}

		BeginWindows();
		GUILayout.Label("3D Texture Designer", EditorStyles.boldLabel );

		EditorGUILayout.GetControlRect(false, 37);
		EditorGUI.DrawRect(new Rect(16, 26, Screen.width-32, 37), Color.black);
		Rect rpos = new Rect (7,26,600,37);
		GUI.DrawTexture(rpos, soLogo );

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		EditorGUILayout.BeginVertical();
		EditorGUILayout.Space();

		toolTip = "Seed Value";
		_settings.seed	= EditorGUILayout.IntField ( new GUIContent("Seed", toolTip), _settings.seed );

		toolTip 	= "frequency";
		_settings.frequency	= EditorGUILayout.FloatField ( new GUIContent("Frequency", toolTip), _settings.frequency );

		toolTip = "";
		_settings.interp   	= (FastNoise.Interp) EditorGUILayout.EnumPopup ( new GUIContent("Interpolation", toolTip), _settings.interp );

		toolTip = "";
		_settings.noiseType   = (FastNoise.NoiseType) EditorGUILayout.EnumPopup ( new GUIContent("Noise Type", toolTip), _settings.noiseType );

		//fractal
		toolTip = "";
		_settings.fractalType   = (FastNoise.FractalType) EditorGUILayout.EnumPopup ( new GUIContent("Fractal Type", toolTip), _settings.fractalType );

		toolTip = "";
		_settings.octaves = EditorGUILayout.IntSlider(new GUIContent("Octaves", toolTip), _settings.octaves, 1, 9 );

		toolTip 	= "";
		_settings.lacunarity	= EditorGUILayout.FloatField ( new GUIContent("Lacunarity", toolTip), _settings.lacunarity );

		toolTip 	= "";
		_settings.gain	= EditorGUILayout.FloatField ( new GUIContent("Gain", toolTip), _settings.gain );

		if ( _settings.noiseType == FastNoise.NoiseType.Cellular)
		{
			// cellular
			toolTip = "";
			_settings.cellularReturnType = (FastNoise.CellularReturnType) EditorGUILayout.EnumPopup ( new GUIContent("Distance Function", toolTip), _settings.cellularReturnType );

			toolTip = "";
			_settings.cellularDistanceFunction = (FastNoise.CellularDistanceFunction) EditorGUILayout.EnumPopup ( new GUIContent("Distance Function", toolTip), _settings.cellularDistanceFunction );

			toolTip = "";
			_settings.cellularDistanceIndex0 = EditorGUILayout.IntSlider(new GUIContent("Distance Index 1", toolTip), _settings.cellularDistanceIndex0, 0, _settings.cellularDistanceIndex1 );
			_settings.cellularDistanceIndex1 = EditorGUILayout.IntSlider(new GUIContent("Distance Index 2", toolTip), _settings.cellularDistanceIndex1, 0, 3 );
			_settings.cellularJitter = EditorGUILayout.Slider(new GUIContent("Jitter", toolTip), _settings.cellularJitter, 0.0f, 1.0f );
		}

		if (!initFlag)
		{
			initFlag = true;
			Generate();
		}

		EditorGUILayout.Space();
		ShaderOneGUI.Line(1);

		GUILayout.Label("Preview", EditorStyles.boldLabel );

		int oldSlice = _sliceIndex;
        toolTip = "Traverse through Y component of the 3D Texture.";
		_sliceIndex = EditorGUILayout.IntSlider(new GUIContent("Slice", toolTip), _sliceIndex, 0, 63 );

		if (_sliceIndex != oldSlice )
		{
			if (_fogCube == null)
				Generate();
			else
				GeneratePreview();
		}

		ShaderOneGUI.Line(1);

		if ( _fogPreview != null )
		{
			EditorGUILayout.GetControlRect(false, Screen.width);
			Rect r = GUILayoutUtility.GetLastRect();

			if ( r.width > 512 )
				r.width = 512;

			if ( r.height > 512 )
				r.height = 512;

			EditorGUI.DrawPreviewTexture ( r, _fogPreview );
		}
		else
		{
			GUILayout.Label("Press Generate button to build Preview" );
		}

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		ShaderOneGUI.Line(1);
		EditorGUILayout.BeginHorizontal();

		if ( GUILayout.Button("?", GUILayout.Width(24)) )
		{
			ShaderOneIO.OpenDocPDF("ShaderOne_Fog_Texture_Designer");
		}

		if (GUILayout.Button("Default Options", GUILayout.Width(128)))
		{
			if(EditorUtility.DisplayDialog("Default Options", "Are you sure you want to reset back to default options?", "Yes!", "No."))
			{
				SetDefaults();
			}
		}

		EditorGUILayout.GetControlRect(false, GUILayout.MinWidth(-3));

		if (GUILayout.Button( "Save", GUILayout.Width ( 128 ) ) )
		{
			Save3DTexture();
			ShaderOneManager.ResetManager();
		}

		if (GUILayout.Button( "Generate", GUILayout.Width ( 128 ) ) )
		{
			_fogPreview = null;
			Generate();
			SaveSettings();
			ShaderOneManager.ResetManager();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		EndWindows();
	}
}
#endif
