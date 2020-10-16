using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconControl : MonoBehaviour
{
	private int 		_stage 	= 0;
	private float 		_time 	= 0;
	private int  		_layer0Color_ID;
	private Color   	_layer0Color = new Color(1,1,1,1);
	private Material 	_mat;

	public float fadeDuration = 0.1f;
	public float fadeHold     = 1;

	public void Engage()
	{
		_time 	= 0;
		_stage 	= 1;

		if (_mat != null)
		{
			_mat.SetColor ( _layer0Color_ID, _layer0Color * 0 );
		}
	}

	void OnApplicationQuit()
	{
		gameObject.SetActive(true);
		if ( _mat != null )
		{
			_mat.SetColor ( _layer0Color_ID, _layer0Color );
		}
	}

	void Start ()
	{
		_mat 			= gameObject.GetComponent<Renderer>().sharedMaterial;
		_layer0Color_ID	= Shader.PropertyToID( "_Layer0Color");
	}

	void Update ()
	{
		float per;

		switch ( _stage )
		{
		// idle
		case 0:
			break;

		// fade in
		case 1:
			_time += Time.deltaTime;

			per = _time / fadeDuration;

			if ( per < 1.0f )
			{
				_mat.SetColor ( _layer0Color_ID, _layer0Color * per );
			}
			else
			{
				_mat.SetColor ( _layer0Color_ID, _layer0Color );
				_stage = 2;
				_time = 0;
			}

			break;

		// hold
		case 2:
			_time += Time.deltaTime;

			if ( _time >= fadeHold )
			{
				_time = 0;
				_stage = 3;
			}
			break;

		// fade out
		case 3:
			_time += Time.deltaTime;

			per = 1.0f - _time / fadeDuration;

			if ( per > 0.0f )
			{
				_mat.SetColor ( _layer0Color_ID, _layer0Color * per );
			}
			else
			{
				_mat.SetColor ( _layer0Color_ID, _layer0Color * 0 );
				_stage = 0;
				_time = 0;
				gameObject.SetActive(false);
			}
			break;
		}
	}
}
