// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ShaderOneSystem;

//----------------------------------------------------------------------------

[ExecuteInEditMode]
public class ShaderOneManager : ShaderOneDLL
{
#if UNITY_2017_4_5 || UNITY_2018
	private static ColorSpace        _oldColorSpace;
#endif

#if UNITY_EDITOR
	private bool         _settingsLoaded        = false;
	private static bool _resetLights 	= false;
#endif

	//------------------------------------------------------------
	private static int PropertyToID ( string i_name )
	{
	#if SHADERONE_DEBUG
		Debug.Log( i_name );
	#endif

		return ( UnityEngine.Shader.PropertyToID ( i_name ) );
	}

	//============================================================
	protected void CreateFogTexture()
	{
		string resourceName;

		if ( _settings.fogMode == FOG_MODE.VOLUMETRIC_3D )
		{
			if ( fogTextureResourceName != null && fogTextureResourceName.Length > 1 )
				resourceName = fogTextureResourceName;
			else
				resourceName = "ShaderOneDefaultFog";

			_fogTexture3D = ShaderOneFog.Create ( resourceName );
		}
	}

	//============================================================
	private LightGroup MakeLightGroup ( int i_maxLights, int i_numPixelLights, SHADERONE_LIGHTTYPE i_soLightType )
	{
		LightGroup lg;
		string lightTypeName;

		lightTypeName = _lightGroupNames[(int)i_soLightType];

		lg = new LightGroup();

		lg.type                 	= i_soLightType;
		lg.first 					= null;
		lg.last 					= null;
		lg.lights 					= new ShaderOneLight [ i_maxLights ];
		lg.lightsRender         	= new LightRender [ i_maxLights ];
		lg.lightsMax 				= i_maxLights;
		lg.lightsPixelCount     	= i_numPixelLights;
		lg.count        			= 0;
		lg.positions       			= new Vector4[ i_maxLights ];
		lg.colors       			= new Vector4[ i_maxLights ];
		lg.distanceAttenuation  	= new Vector4[ i_maxLights ];
		lg.spotDirection    		= new Vector4[ i_maxLights ];
		lg.spotAttenuation   		= new Vector4[ i_maxLights ];

		lg.keywordV_on   = "SO_MC_V_" + lightTypeName.ToUpper() + "_ON";
		lg.keywordP_on   = "SO_MC_P_" + lightTypeName.ToUpper() + "_ON";

	#if SHADERONE_DEBUG
		Debug.Log("keywords = " + lg.keywordV_on );
		Debug.Log("keywords = " + lg.keywordP_on );
	#endif

		lg.positions_ID 	   		= PropertyToID( "_" + lightTypeName + "_Position");
		lg.colors_ID 				= PropertyToID( "_" + lightTypeName + "_Color");
		lg.distanceAttenuation_ID 	= PropertyToID( "_" + lightTypeName + "_DistAtten");
		lg.spotDirection_ID 		= PropertyToID( "_" + lightTypeName + "_SpotDir");
		lg.spotAttenuation_ID 		= PropertyToID( "_" + lightTypeName + "_SpotAtten");

		return ( lg );
	}

	//-------------------------------------------------------------------------
	private void LightsInit()
	{
		Light[] 		lights;
		ShaderOneLight 	sol;
		int 			loop;

		lights 		= Resources.FindObjectsOfTypeAll ( typeof ( Light ) ) as Light[];

		_lightGroups = new LightGroup[(int)SHADERONE_LIGHTTYPE.COUNT];

		_lightGroups[(int)SHADERONE_LIGHTTYPE.DIRECTIONAL]	= MakeLightGroup ( _settings.dirArraySize, _settings.dirPerPixelCount, SHADERONE_LIGHTTYPE.DIRECTIONAL );
		_lightGroups[(int)SHADERONE_LIGHTTYPE.POINT]		= MakeLightGroup ( _settings.pointArraySize, _settings.pointPerPixelCount, SHADERONE_LIGHTTYPE.POINT );
		_lightGroups[(int)SHADERONE_LIGHTTYPE.SPOT]			= MakeLightGroup ( _settings.spotArraySize, _settings.spotPerPixelCount, SHADERONE_LIGHTTYPE.SPOT );

		if ( Application.isPlaying )
		{
			for ( loop = 0; loop < lights.Length; loop++ )
			{
				if ( lights[loop].hideFlags == HideFlags.None )
				{
#if UNITY_EDITOR
					if ( lights[loop].lightmapBakeType == LightmapBakeType.Baked )
						continue;
#endif

					sol = lights[loop].gameObject.GetComponent<ShaderOneLight>();

					if ( sol == null )
						sol = lights[loop].gameObject.AddComponent<ShaderOneLight>();

					if ( lights[loop].type != LightType.Directional )
					{
						if ( disableUnityLights )
							lights[loop].enabled = false;
					}
				}
			}
		}
#if UNITY_EDITOR
		else
		{
			EditorUpdateLights();
		}
#endif

	}


#if UNITY_EDITOR
	public static void RefreshAllUnityLights()
	{
		if ( !Application.isPlaying )
		{
			_unityLights = FindObjectsOfType ( typeof ( Light ) ) as Light[];

			for (int loop = 0; loop < _unityLights.Length; loop++ )
			{
				if (_unityLights[loop].gameObject.activeSelf)
				{
					_unityLights[loop].gameObject.SetActive ( false );
					_unityLights[loop].gameObject.SetActive ( true );
				}
			}

			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}
	}

	//============================================================
	public static void EditorUpdateLights()
	{
		if ( !Application.isPlaying )
		{
			_unityLights = FindObjectsOfType ( typeof ( Light ) ) as Light[];
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}
	}

	//============================================================
	public static void ResetManager()
	{
		if ( !Application.isPlaying )
		{
			_resetLights = true;
			AssetDatabase.Refresh();
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			EditorApplication.RepaintHierarchyWindow();
		}
	}

#endif

	//============================================================
	void Awake()
	{
		if (QualitySettings.activeColorSpace == ColorSpace.Linear )
		{
#if UNITY_2017_4_5 || UNITY_2018
			GraphicsSettings.lightsUseLinearIntensity = true;
			GraphicsSettings.lightsUseColorTemperature = true;
#endif
    	}
		else
		{
#if UNITY_2017_4_5 || UNITY_2018
			GraphicsSettings.lightsUseLinearIntensity = false;
			GraphicsSettings.lightsUseColorTemperature = false;
#endif
		}
	}

	//============================================================
	public void OnEnable()
	{

		ShaderOneIO.LoadSettings ( ref _settings );

		#if UNITY_EDITOR
		CreateFogTexture();
		#endif

		GetShaderIDS();
		UpdateFogShaderData();

		if ( _settings.renderPipeline == RENDER_PIPELINE.SHADERONE_LIGHTING )
		{
			LightsInit();

			if ( _delegateCount == 0 )
			{
				Camera.onPreRender += CamerasPreRender;
				_delegateCount++;

#if UNITY_EDITOR
				if ( !Application.isPlaying )
				{
#if UNITY_2018
					EditorApplication.hierarchyChanged += EditorUpdateLights;
#else
					EditorApplication.hierarchyWindowChanged += EditorUpdateLights;
#endif
				}
#endif
			}
		}
	}

	//============================================================
    public void OnDisable()
    {
		if ( _settings.renderPipeline == RENDER_PIPELINE.SHADERONE_LIGHTING )
		{
			if ( _delegateCount == 1 )
			{
				Camera.onPreRender -= CamerasPreRender;
				_delegateCount--;

#if UNITY_EDITOR
				if ( !Application.isPlaying )
				{
#if UNITY_2018
					EditorApplication.hierarchyChanged -= EditorUpdateLights;
#else
					EditorApplication.hierarchyWindowChanged -= EditorUpdateLights;
#endif
				}
#endif
			}
		}
    }

	//============================================================
	public void Start()
	{
		ShaderOneIO.LoadSettings( ref _settings );

		GetShaderIDS();

		CreateFogTexture();

		UpdateFogShaderData();
	}

	//-------------------------------------------------------------------------
	private void LightGroupFindClosestEditor ( LightGroup i_lg, Vector3 i_center, Camera i_cam )
	{
		LightRender 	lr;
		float 			distance;
		int 			loop;

		if ( _unityLights == null )
			return;

    	i_lg.count 					= 0;

		Array.Clear( i_lg.lights, 0, i_lg.lightsMax );
		Array.Clear( i_lg.colors, 0, i_lg.colors.Length );

		for ( loop = 0; loop < _unityLights.Length; loop++ )
		{
			if ( !_unityLights[loop].gameObject.activeSelf )
				continue;

#if UNITY_EDITOR
			if ( _unityLights[loop].lightmapBakeType == LightmapBakeType.Baked )
      			continue;
#endif

 //   		if ( !_unityLights[loop].enabled )
 //   			continue;

			lr = UnityLightToLightRender(_unityLights[loop] );

			if ( lr.type != i_lg.type )
				continue;

			if ( ( i_cam.cullingMask & lr.unityLight.cullingMask ) == 0 )
				continue;

			distance = Vector3.Distance ( i_center, lr.position );

			if ( distance < i_cam.farClipPlane )
			{
				lr.dist = distance;

				LightGroupSortItem ( i_lg, lr );
			}
		}
	}

	//-------------------------------------------------------------------------
	private void LightGroupFindClosest ( LightGroup i_lg, Vector3 i_center, Camera i_cam )
	{
		ShaderOneLight 	sol;
		float 			distance;

		i_lg.count = 0;

		Array.Clear( i_lg.lights, 0, i_lg.lightsMax );
		Array.Clear( i_lg.colors, 0, i_lg.colors.Length );

		for ( sol = i_lg.first; sol != null; sol = sol.next )
		{
			if ( ( i_cam.cullingMask & sol.lr.unityLight.cullingMask ) == 0 )
				continue;

//			if ( !sol.lr.unityLight.enabled )
//				continue;

			distance = Vector3.Distance ( i_center, sol.lr.cachedTransform.position );

			if ( distance < i_cam.farClipPlane )
			{
				sol.lr.spotDir  = new Vector4 (-sol.lr.cachedTransform.forward.x, -sol.lr.cachedTransform.forward.y, -sol.lr.cachedTransform.forward.z, 0.0f);
				sol.lr.dist 	= distance;

				LightGroupSortItem ( i_lg, sol.lr );
			}
		}
	}

	//-------------------------------------------------------------------------
	private void LightGroupSendShaderData ( LightGroup i_lg, Camera i_cam )
	{
		int loop;
		LightRender lr;

		_indirectLight  = 0.0f;
		_indirectCount  = 0;
		_indirectColor  = new Color(0,0,0,0);

		for ( loop = 0; loop < i_lg.count; loop++ )
		{
			lr = i_lg.lightsRender[loop];

			CalcAttenuation(lr);

			i_lg.positions[loop] 			= lr.cachedTransform.position;

			if ( QualitySettings.activeColorSpace == ColorSpace.Linear )
			{
#if UNITY_2017_4_5 || UNITY_2018
				Color colorTemp 	= Mathf.CorrelatedColorTemperatureToRGB (lr.unityLight.colorTemperature);
				i_lg.colors[loop]   = lr.unityLight.color * lr.unityLight.intensity * colorTemp;
#else
				i_lg.colors[loop]   = lr.unityLight.color * Mathf.Pow ( lr.unityLight.intensity, 2.2f );
#endif
			}
			else
				i_lg.colors[loop]   = lr.unityLight.color * lr.unityLight.intensity;
//				i_lg.colors[loop]   		= lr.unityLight.color * Mathf.Pow ( lr.unityLight.intensity, 1.0f/2.2f );

			i_lg.distanceAttenuation[loop]  = lr.distanceAttenuation;
			i_lg.spotDirection[loop]        = lr.spotDir;
			i_lg.spotAttenuation[loop]      = lr.spotAttenuation;

			//CalcIndirectLight ( lr, i_cam );
		}

		if ( i_lg.count > 0 )
		{
			//_indirectColor /= _indirectCount;
			//_indirectColor += RenderSettings.ambientEquatorColor;
			//_indirectColor *= 0.5f;
			//_indirectColor *= _indirectLight * RenderSettings.reflectionIntensity;

			_indirectColor = new Color ( 115.0f/255.0f, 128.0f/255.0f, 146.0f/255.0f )*RenderSettings.reflectionIntensity;

			Shader.SetGlobalColor ( "_ShaderOneIndirectColor", _indirectColor );

			Shader.SetGlobalVectorArray ( i_lg.positions_ID, i_lg.positions );
			Shader.SetGlobalVectorArray ( i_lg.colors_ID, i_lg.colors );
			Shader.SetGlobalVectorArray ( i_lg.distanceAttenuation_ID, i_lg.distanceAttenuation );
			Shader.SetGlobalVectorArray ( i_lg.spotDirection_ID, i_lg.spotDirection );
			Shader.SetGlobalVectorArray ( i_lg.spotAttenuation_ID, i_lg.spotAttenuation );

			if ( i_lg.lightsPixelCount > 0 )
				Shader.EnableKeyword (i_lg.keywordP_on);

			if ( i_lg.count > i_lg.lightsPixelCount )
				Shader.EnableKeyword (i_lg.keywordV_on);
			else
				Shader.DisableKeyword (i_lg.keywordV_on);
		}
		else
		{
			Shader.DisableKeyword ( i_lg.keywordP_on );
			Shader.DisableKeyword ( i_lg.keywordV_on );
		}
	}

	//========================================================================
	public void CamerasPreRender( Camera i_cam )
	{
		float           lightLookAhead = 2;
		Vector3         lookAhead;
		int             loop;

		for ( loop = 0; loop < (int)SHADERONE_LIGHTTYPE.COUNT; loop++ )
		{
			lookAhead = i_cam.transform.position + i_cam.transform.forward * lightLookAhead;

#if UNITY_EDITOR
			if ( Application.isPlaying )
				LightGroupFindClosest ( _lightGroups[loop], lookAhead, i_cam );
			else
				LightGroupFindClosestEditor ( _lightGroups[loop], lookAhead, i_cam );
#else
			LightGroupFindClosest ( _lightGroups[loop], lookAhead, i_cam );
#endif

			LightGroupSendShaderData ( _lightGroups[loop], i_cam );
		}
    }


#if UNITY_EDITOR
	public void Update()
	{

#if UNITY_2017_4_5 || UNITY_2018
		if ( !Application.isPlaying )
		{
			if ( QualitySettings.activeColorSpace == ColorSpace.Linear )
			{
				GraphicsSettings.lightsUseLinearIntensity = true;
				GraphicsSettings.lightsUseColorTemperature = true;
			}
			else
			{
				GraphicsSettings.lightsUseLinearIntensity = false;
				GraphicsSettings.lightsUseColorTemperature = false;
			}
		}

		if ( PlayerSettings.colorSpace != _oldColorSpace )
		{
			string path = ShaderOneIO.FindAsset ("ShaderOneGen.shader");
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
			RefreshAllUnityLights();
		}

		_oldColorSpace = PlayerSettings.colorSpace;

#endif

		if (!_settingsLoaded)
		{
			ShaderOneIO.LoadSettings ( ref _settings );
			_settingsLoaded = true;
		}

		GetShaderIDS();
		UpdateFogShaderData();

		if ( _resetLights )
		{
			_resetLights = false;
			OnEnable();
		}
	}
#endif

}

