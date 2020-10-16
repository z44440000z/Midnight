// ShaderOne - PBR Shader System for Unity
// Copyright(c) Scott Host / EchoLogin 2018 <echologin@hotmail.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShaderOneSystem;

[ExecuteInEditMode]
public class ShaderOneMaterialControl : MonoBehaviour
{
	class ShaderOneMaterialLayer
	{
		public Vector4  		tex_ST;
		public int  			tex_ST_ID;

		public float 			scrollU;
		public int   			scrollU_ID;

		public float 			scrollV;
		public int   			scrollV_ID;

		public float 			rotation;
		public int				rotation_ID;

		public float    		animType;
		public int      		animType_ID;

		public int 				animCellsHorz;
		public int 				animCellsHorz_ID;

		public int	 			animCellsVert;
		public int 				animCellsVert_ID;

		public float			animFPS;
		public int 				animFPS_ID;

		public ANIM_LOOP_MODE	animLoopMode;
		public int 				animLoopMode_ID;

		public int  			animCellStart;
		public int 				animCellStart_ID;

		public int  			animCellEnd;
		public int 				animCellEnd_ID;

		public int 				animOffsetX_ID;

		public int 				animOffsetY_ID;

		public int 				animOffsetX2_ID;

		public int 				animOffsetY2_ID;

		public int 				animBlend_ID;

		public int 				animActive_ID;
		public bool             animActive;
		public float 			animTime;
		public float 			animTimeLooped;
		public int              animFrame;
		public float 			animTimeTotal;
		public float    		animFrameSlice;
		public int      		animFrameCount;
		public float    		animCellUSize;
		public float    		animCellVSize;
	};

	private ShaderOneMaterialLayer[] 	_layers;
	private ShaderOneSettings 			_settings;
	private Material                    _material;
	private Renderer                    _renderer;
	private bool                        _shaderOneAttached = false;

	//=========================================================================
	void OnDisable()
	{
		if ( _material != null )
		{
			_material.SetFloat("_ControlScriptFlag", 0);
			_material.DisableKeyword ("SO_MC_CONTROL_SCRIPT_ON");
		}
	}

	//=========================================================================
	void OnEnable()
	{
		_renderer		= gameObject.GetComponent<Renderer>();
		if (_renderer != null )
		{
			_material 		= _renderer.sharedMaterial;
			if ( _material != null )
			{
				_material.SetFloat("_ControlScriptFlag", 1977 );
				_material.EnableKeyword ("SO_MC_CONTROL_SCRIPT_ON");
			}
		}
	}

	//=========================================================================
	public void AnimationStart ( int i_index )
	{
		LayerCellAnimInit ( i_index );
	}

#if UNITY_EDITOR
   //=========================================================================
	public void RemoveScript()
	{
		DestroyImmediate(this);
	}
#endif

 	//=========================================================================
	public ShaderOneLayerSettings LayerSettingsGet ( int i_layerIndex )
	{
		ShaderOneLayerSettings layerSettings = new ShaderOneLayerSettings();

		switch ( i_layerIndex )
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

		return ( layerSettings );
	}

	//=========================================================================
	void LayerGetShaderData ( ref ShaderOneMaterialLayer i_soml )
	{
		i_soml.tex_ST 	   		=  _material.GetVector ( i_soml.tex_ST_ID );
		i_soml.scrollU 			=  _material.GetFloat ( i_soml.scrollU_ID );
		i_soml.scrollV 			=  _material.GetFloat ( i_soml.scrollV_ID );
		i_soml.rotation    		=  _material.GetFloat ( i_soml.rotation_ID );
		i_soml.animType    		=  _material.GetFloat ( i_soml.animType_ID );
		i_soml.animActive       =  ( _material.GetInt ( i_soml.animActive_ID ) == 1 ) ?  true : false;
		i_soml.animCellsHorz 	=  (int)_material.GetFloat ( i_soml.animCellsHorz_ID );
		i_soml.animCellsVert 	=  (int)_material.GetFloat ( i_soml.animCellsVert_ID );
		i_soml.animFPS 			=  _material.GetFloat ( i_soml.animFPS_ID );
		i_soml.animLoopMode 	=  (ANIM_LOOP_MODE)_material.GetInt ( i_soml.animLoopMode_ID );
		i_soml.animCellStart 	=  (int)_material.GetFloat ( i_soml.animCellStart_ID );
		i_soml.animCellEnd 		=  (int)_material.GetFloat ( i_soml.animCellEnd_ID );
	}

	//=========================================================================
	void LayerGetShaderIDS ( ref ShaderOneMaterialLayer i_soml, int i_layerIndex )
	{
		string prefix = "_Layer" + i_layerIndex;

		if ( i_layerIndex == 0 )
		{
			i_soml.tex_ST_ID 			=  Shader.PropertyToID ( "_MainTex_ST" );
		}
		else
		{
			i_soml.tex_ST_ID 			=  Shader.PropertyToID ( prefix + "Tex_ST" );
		}

		i_soml.scrollU_ID 			=  Shader.PropertyToID ( prefix + "ScrollU" );
		i_soml.scrollV_ID 			=  Shader.PropertyToID ( prefix + "ScrollV" );
		i_soml.rotation_ID 			=  Shader.PropertyToID ( prefix + "Rotation" );
		i_soml.animType_ID 			=  Shader.PropertyToID ( prefix + "AnimType" );
		i_soml.animActive_ID 		=  Shader.PropertyToID ( prefix + "AnimActive" );
		i_soml.animCellsHorz_ID 	=  Shader.PropertyToID ( prefix + "AnimCellsHorz" );
		i_soml.animCellsVert_ID 	=  Shader.PropertyToID ( prefix + "AnimCellsVert" );
		i_soml.animFPS_ID 			=  Shader.PropertyToID ( prefix + "AnimFPS" );
		i_soml.animLoopMode_ID 		=  Shader.PropertyToID ( prefix + "AnimLoopMode" );
		i_soml.animCellStart_ID 	=  Shader.PropertyToID ( prefix + "AnimCellStart" );
		i_soml.animCellEnd_ID 		=  Shader.PropertyToID ( prefix + "AnimCellEnd" );
		i_soml.animOffsetX_ID 		=  Shader.PropertyToID ( prefix + "AnimOffsetX" );
		i_soml.animOffsetY_ID 		=  Shader.PropertyToID ( prefix + "AnimOffsetY" );
		i_soml.animOffsetX2_ID 		=  Shader.PropertyToID ( prefix + "AnimOffsetX2" );
		i_soml.animOffsetY2_ID 		=  Shader.PropertyToID ( prefix + "AnimOffsetY2" );
		i_soml.animBlend_ID 		=  Shader.PropertyToID ( prefix + "AnimBlend" );
	}

	//==================================================================
	public void LayerRandomUVInit ( int i_layerIndex )
	{
		ShaderOneMaterialLayer layer	= _layers[i_layerIndex];

		if ( layer.animFPS > 0 )
			layer.animFrameSlice      = 1.0f / (float)layer.animFPS;
		else
			layer.animFrameSlice      = 1.0f / 60.0f;

		layer.animTime            = 0;
	}

	//==================================================================
	public void LayerRandomUVProcess ( int i_layerIndex )
	{
		ShaderOneMaterialLayer layer	= _layers[i_layerIndex];

		layer.animTime += Time.deltaTime;

		if ( layer.animTime > layer.animFrameSlice )
		{
			_material.SetFloat ( layer.animOffsetX_ID, Random.Range ( 0.0f, 1.0f ) );
			_material.SetFloat ( layer.animOffsetY_ID, Random.Range ( 0.0f, 1.0f ) );

			layer.animTime = 0.0f;
		}
	}

	//==================================================================
	public void LayerCellAnimInit ( int i_layerIndex )
	{
		ShaderOneMaterialLayer layer	= _layers[i_layerIndex];
		float padUV;
		Vector4 vec4;

		layer.animActive		= true;
		layer.animFrameSlice    = 1.0f / (float)layer.animFPS;
		layer.animFrameCount    = layer.animCellEnd - layer.animCellStart + 1;
		layer.animTimeTotal     = layer.animFrameCount / (float)layer.animFPS;
		layer.animTime          = 0;
		layer.animFrame			= layer.animCellStart;

		vec4 = _material.GetVector ( layer.tex_ST_ID );

		padUV = 1.0f - vec4.x;
		layer.animCellUSize      = ( 1.0f - padUV ) / (float)layer.animCellsHorz;

		padUV = 1.0f - vec4.y;
		layer.animCellVSize      = ( 1.0f - padUV ) / (float)layer.animCellsVert;
	}

	//==================================================================
	public void SetupLayers()
	{
		for ( int loop = 0; loop < 4; loop++ )
		{
			switch ((ANIMTYPE)_layers[loop].animType)
			{
			default:
				break;

			case ANIMTYPE.CELL_ANIM_BLEND:
			case ANIMTYPE.CELL_ANIM:
				if (_layers[loop].animActive)
					LayerCellAnimInit ( loop );
				break;

			case ANIMTYPE.RANDOM_UV:
				LayerRandomUVInit ( loop );
				break;
			}
		}
	}

	//==================================================================
	public void LayerCellAnimProcess ( int i_layerIndex )
	{
		float offset, cx, cy;
		ShaderOneMaterialLayer layer	= _layers[i_layerIndex];

		if ( !layer.animActive )
			return;

		switch ( layer.animLoopMode )
		{
		case ANIM_LOOP_MODE.ONCE:
			layer.animTimeLooped = Mathf.Repeat ( layer.animTime, layer.animTimeTotal );

			if ( layer.animTime >= layer.animTimeTotal )
			{
				layer.animActive = false;
				return;
			}
			break;

		case ANIM_LOOP_MODE.LOOP:
			layer.animTimeLooped = Mathf.Repeat ( layer.animTime, layer.animTimeTotal );
			break;

		case ANIM_LOOP_MODE.PING_PONG:
			layer.animTimeLooped = Mathf.PingPong ( layer.animTime, layer.animTimeTotal );
			break;
		}

		layer.animFrame = (int)( layer.animTimeLooped / layer.animFrameSlice ) + layer.animCellStart;

		layer.animTime += Time.deltaTime;

		cx = (float)( layer.animFrame % layer.animCellsHorz );
		cy = (float)( layer.animFrame / layer.animCellsHorz );

		offset = cx * layer.animCellUSize;

		_material.SetFloat ( layer.animOffsetX_ID, offset );

		offset = ( ( layer.animCellsVert - 1 ) - cy ) * layer.animCellVSize;

		_material.SetFloat ( layer.animOffsetY_ID, offset );

		return;
	}


	//==================================================================
	public void LayerCellAnimProcessBlend ( int i_layerIndex )
	{
		float offset, cx, cy;
		ShaderOneMaterialLayer layer	= _layers[i_layerIndex];

		if ( !layer.animActive )
			return;

		LayerCellAnimProcess ( i_layerIndex );

		// frame 2
		layer.animFrame = ( layer.animFrame + 1 ) % layer.animFrameCount;

		cx = (float)( layer.animFrame % layer.animCellsHorz );
		cy = (float)( layer.animFrame / layer.animCellsHorz );

		offset = cx * layer.animCellUSize;

		_material.SetFloat ( layer.animOffsetX2_ID, offset );

		offset = ( ( layer.animCellsVert - 1 ) - cy ) * layer.animCellVSize;

		_material.SetFloat ( layer.animOffsetY2_ID, offset );

		_material.SetFloat ( layer.animBlend_ID, Mathf.Repeat ( layer.animTimeLooped , layer.animFrameSlice ) );

	}

	//=========================================================================
	void Awake ()
	{
		if (_shaderOneAttached && _material.GetFloat("_IntersectToggle") > 0.001f )
		{
			Camera.main.depthTextureMode = DepthTextureMode.Depth;
		}
	}

	//=========================================================================
	void Start()
	{
		_renderer		= gameObject.GetComponent<Renderer>();

		if ( _renderer == null )
			return;

		_material 		= _renderer.sharedMaterial;

		if ( _material == null )
			return;

		if ( _material.shader == null )
			return;

		if ( _material.shader.name.Contains ( "shaderOne" ) )
			_shaderOneAttached = true;
		else
			_shaderOneAttached = false;

		if ( _shaderOneAttached )
		{
			ShaderOneIO.LoadSettings( ref _settings );

			_layers 	= new ShaderOneMaterialLayer[4];
			_layers[0] 	= new ShaderOneMaterialLayer();
			_layers[1] 	= new ShaderOneMaterialLayer();
			_layers[2] 	= new ShaderOneMaterialLayer();
			_layers[3] 	= new ShaderOneMaterialLayer();

			LayerGetShaderIDS ( ref _layers[0], 0 );
			LayerGetShaderIDS ( ref _layers[1], 1 );
			LayerGetShaderIDS ( ref _layers[2], 2 );
			LayerGetShaderIDS ( ref _layers[3], 3 );

			LayerGetShaderData (ref _layers[0] );
			LayerGetShaderData (ref _layers[1] );
			LayerGetShaderData (ref _layers[2] );
			LayerGetShaderData (ref _layers[3] );

			SetupLayers();
		}
	}

	void Update ()
	{
		if ( !_shaderOneAttached )
			return;

		#if UNITY_EDITOR
		{
			if ( _layers == null )
				Start();
		}
		#endif

		if ( _renderer.isVisible )
		{
			for ( int loop = 0; loop < 4; loop++ )
			{

				switch ((ANIMTYPE)_layers[loop].animType)
				{
				default:
					break;

				case ANIMTYPE.CELL_ANIM:
					LayerCellAnimProcess ( loop );
					break;

				case ANIMTYPE.CELL_ANIM_BLEND:
					LayerCellAnimProcessBlend ( loop );
					break;

				case ANIMTYPE.RANDOM_UV:
					LayerRandomUVProcess ( loop );
					break;
				}
			}
		}
	}
}

