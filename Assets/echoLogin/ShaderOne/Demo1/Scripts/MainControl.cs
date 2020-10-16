using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour
{
	private bool        _isEnding 	= false;
	private float       _fadeTime;
	private int  	 	_layer0Color_ID;
	private Material 	_mat1;
	private Material 	_mat2;
	private GameObject 	_door1;
	private GameObject 	_door2;
	private Color    	_colorBase;
	private Color    	_colorOff;
	private Color    	_colorOn;
	private Color    	_colorDone;
	private int         _stage;
	private Animator   	_anim1;
	private Animator   	_anim2;
	private float      	_flickerDuration;
	private int        	_stageNext;

	private int         _progressMode = 0;
	private float       _progressTime = 0;
	private Material 	_progressMat;

	private int			_mainTVDist_ID;
	private float      	_mainTVDistTime;
	private float      	_mainTVDistDuration = 0;
	private float      	_mainTVDistStrength;
	private int        	_mainTVRoll_ID;
	private float      	_mainTVRollTime;
	private float      	_mainTVRollDuration = 0;
	private Material 	_mainTVMat;
	private int         _mainTVLayer1Color_ID;

	private bool        _fogLight 		= false;
	private float       _fogLightTime   = 0;

	private int         				_progress_ID;
	private int         				_cellAnim_ID;
	private ShaderOneMaterialControl 	_somcCellAnim;

	public GameObject doorObject;
	public GameObject robotHead;

	public float flickerDuration 			= 1;
	public float flickerOnIntensity			= 0.9f;
	public float flickerOffIntensity    	= 0.1f;
	public float flickerDoneIntensity   	= 0.6f;

	public GameObject mainTV;
    public AnimationCurve mainTVDistCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    public AnimationCurve mainTVRollCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

	public Light	 		fogLight;
	public float            fogLightDuration = 1;
	public AnimationCurve 	fogLightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
	public GameObject       hologram;

	public GameObject logo;
	public GameObject wallObject;
	public GameObject cellAnimObj;
	public GameObject progressObj;
	public float      progressDuration = 1.0f;

	public IconControl[] iconControl;

    void OnDisable()
	{
		_mainTVMat.SetFloat ( _mainTVDist_ID, 0 );
		_mat1.SetColor ( _layer0Color_ID, _colorBase );
		_mat2.SetColor ( _layer0Color_ID, _colorBase );
	}

	public void EndApp()
	{
		_isEnding = true;
		_fadeTime = 2.0f;
	}

	public void EngageIcon ( int i_index )
	{
		iconControl[i_index].gameObject.SetActive(true);
		iconControl[i_index].Engage();
	}

	public void TurnOnFogLight()
	{
		_fogLight = true;
		hologram.SetActive(true);
	}

	public void TurnOnShaderOne()
	{
		_mainTVMat.SetColor ( _mainTVLayer1Color_ID, new Color (1,1,1,1) );
	}

	public void TurnWallOn()
	{
		hologram.SetActive(false);
		fogLight.intensity = 0;
		wallObject.SetActive(true);
		_somcCellAnim.AnimationStart(0);
		_progressMode = 1;
		_progressTime = 0;
	}

	public void MainTvDistortion ( float i_duration )
	{
		_mainTVDistDuration = i_duration;
		_mainTVDistTime 	= 0;
	}

	public void MainTvRoll ( float i_duration )
	{
		_mainTVRollDuration = i_duration;
		_mainTVRollTime 	= 0;
	}

	public void TurnOnDoors()
	{
		_door1.SetActive(true);
		_door2.SetActive(true);
		logo.SetActive(true);
	}

	public void DoorCountDownStart()
	{
		_somcCellAnim.AnimationStart(0);
	}

	public void DoorsStart()
	{
		_stage 			 = 1;
		_stageNext 		 = 2;
		_flickerDuration = flickerDuration;
	}

	//
	public void FlickerOff()
	{
		//_flickerDuration = flickerDuration;
		//_stage = 1;
		//_stageNext = 3;
	}

	public void RobotHeadDrop()
	{
		robotHead.SetActive(true);
	}

	void Awake()
	{
		hologram.SetActive(false);
	}

	// Use this for initialization
	void Start ()
	{
		_stage = 0;

		EchoAudioManager.PlaySong ("shaderoneSong", false, 1.0f );
		EchoAudioManager.GlobalMusicVolume ( 1.0f );

		fogLight.intensity = 0;

		for ( int loop = 0; loop < iconControl.Length; loop++ )
		{
			iconControl[loop].gameObject.SetActive(false);
		}

		robotHead.SetActive(false);
		wallObject.SetActive(false);

		_door1 = doorObject.transform.Find ( "Door1" ).gameObject;
		_door2 = doorObject.transform.Find ( "Door2" ).gameObject;

		_anim1 = doorObject.GetComponent<Animator>();

		_mat1 = _door1.GetComponent<Renderer>().sharedMaterial;
		_mat2 = _door2.GetComponent<Renderer>().sharedMaterial;

		_layer0Color_ID	= Shader.PropertyToID( "_Layer0Color");

		_mainTVMat 				= mainTV.GetComponent<Renderer>().sharedMaterial;
		_mainTVLayer1Color_ID	= Shader.PropertyToID( "_Layer1Color");
		_mainTVDist_ID			= Shader.PropertyToID( "_Layer0DistortionStrength");
		_mainTVRoll_ID 			= Shader.PropertyToID( "_ScrollV");

		_mainTVMat.SetFloat ( _mainTVDist_ID, 0 );
		_mainTVMat.SetColor ( _mainTVLayer1Color_ID, new Color (0,0,0,0) );

		_colorBase = _mat1.GetColor ( _layer0Color_ID );

		_colorOn 		= _colorBase * flickerOnIntensity;
		_colorOff 		= _colorBase * flickerOffIntensity;
		_colorDone 	= _colorBase * flickerDoneIntensity;

		 //_mat1.SetColor ( _layer0Color_ID, new Color (0,0,0,0) );
		 //_mat2.SetColor ( _layer0Color_ID, new Color (0,0,0,0) );

		 _door1.SetActive(false);
		 _door2.SetActive(false);
		 logo.SetActive(false);

		_somcCellAnim 	= cellAnimObj.GetComponent<ShaderOneMaterialControl>();

		_progressMat = progressObj.GetComponent<Renderer>().sharedMaterial;

		_progress_ID 	= Shader.PropertyToID( "_Layer0Progress");

		_progressMat.SetFloat ( _progress_ID, 0 );

//		_anim1.Play("CloseSlow");
	}

	// Update is called once per frame
	void Update ()
	{
		if ( _isEnding )
		{
			_fadeTime -= Time.deltaTime;
			EchoAudioManager.GlobalMusicVolume ( _fadeTime / 2.0f );

			if ( _fadeTime <= 0.0001f )
			{
				_isEnding = false;
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
				Application.Quit();
#endif
			}
		}


		if ( _fogLight )
		{
			_fogLightTime += Time.deltaTime;

			float per =  Mathf.Clamp ( _fogLightTime / fogLightDuration, 0, 1 );

			fogLight.intensity = fogLightCurve.Evaluate ( per );
		}

		if ( _mainTVDistDuration > 0 )
		{
			_mainTVDistTime += Time.deltaTime;

			_mainTVMat.SetFloat ( _mainTVDist_ID, mainTVDistCurve.Evaluate( _mainTVDistTime / _mainTVDistDuration ) );

			if ( _mainTVDistTime >=  _mainTVDistDuration )
			{
				_mainTVDistDuration = 0;
			}
		}


		if ( _mainTVRollDuration > 0 )
		{
			_mainTVRollTime += Time.deltaTime;

			_mainTVMat.SetFloat ( _mainTVRoll_ID, mainTVRollCurve.Evaluate( _mainTVRollTime / _mainTVRollDuration ) );

			if ( _mainTVRollTime >=  _mainTVRollDuration )
			{
				_mainTVRollDuration = 0;
			}
		}

		if ( _progressMode != 0 )
		{
			_progressTime += Time.deltaTime;
			_progressMat.SetFloat ( _progress_ID, 1.0f - Mathf.Clamp ( _progressTime / progressDuration, 0.0f, 1.0f ) );

			if (_progressTime > progressDuration )
				_progressMode = 0;
		}


		switch ( _stage )
		{
		// idle billy idle
		case 0:
			break;

		// flicker
		case 1:
			_flickerDuration -= Time.deltaTime;

			if ( Random.Range ( 0.0f, 1.0f) > 0.6f )
			{
				_mat1.SetColor ( _layer0Color_ID, _colorOn );
				_mat2.SetColor ( _layer0Color_ID, _colorOn );
			}
			else
			{
				_mat1.SetColor ( _layer0Color_ID, _colorOff );
				_mat2.SetColor ( _layer0Color_ID, _colorOff );
			}

			if ( _flickerDuration < 0.0f )
			{
				_stage = _stageNext;
			}
			break;

		// open doors;
		case 2:
			_mat1.SetColor ( _layer0Color_ID, _colorDone );
			_mat2.SetColor ( _layer0Color_ID, _colorDone );
			_stage = 0;
			_anim1.Play("Open");
			break;

		case 3:
			_mat1.SetColor ( _layer0Color_ID, _colorOff );
			_mat2.SetColor ( _layer0Color_ID, _colorOff );
			_stage = 0;
			break;
		}
	}
}
