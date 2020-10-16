using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramFlicker : MonoBehaviour
{
	private int  	 _layer0Color_ID;
	private Material _mat = null;
	private Color    _colorBase;
	private Color    _colorOff;
	private Color    _colorOn;
	private Color    _colorDone;
	private bool     _fxEnabled = true;

	public float startDelay	 				= 2;
	public float flickerDuration 			= 1;
	public float flickerOnIntensity			= 0.9f;
	public float flickerOffIntensity    	= 0.1f;
	public float flickerDoneIntensity   	= 0.6f;

	void OnDisable()
	{
		if(_mat != null)
		{
			_mat.SetColor ( _layer0Color_ID, _colorBase );
		}
	}

	void Start ()
	{
		_mat 			= gameObject.GetComponent<Renderer>().sharedMaterial;

		_layer0Color_ID	= Shader.PropertyToID( "_Layer0Color");

		_colorBase = _mat.GetColor ( _layer0Color_ID );

		_colorOn 	= _colorBase * flickerOnIntensity;
		_colorOff 	= _colorBase * flickerOffIntensity;
		_colorDone 	= _colorBase * flickerDoneIntensity;

		_mat.SetColor ( _layer0Color_ID, new Color (0,0,0,0) );
	}

	void Update ()
	{
		if (!_fxEnabled)
			return;

		if (startDelay > 0)
			startDelay -= Time.deltaTime;
		else
		{
			flickerDuration -= Time.deltaTime;

			if ( Random.Range ( 0.0f, 1.0f) > 0.6f )
			{
				_mat.SetColor ( _layer0Color_ID, _colorOn );
			}
			else
			{
				_mat.SetColor ( _layer0Color_ID, _colorOff );
			}
			if ( flickerDuration < 0.0f )
			{
				_fxEnabled = false;
				_mat.SetColor ( _layer0Color_ID, _colorDone );
			}
		}
	}
}
