using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalactixControl : MonoBehaviour
{
	private Material 	_mat;
	private int _layer1ScrollU_ID;
	private int _layer2ScrollU_ID;
	private int _layer3ScrollU_ID;

	private float _layer1ScrollU = 0;
	private float _layer2ScrollU = 0;
	private float _layer3ScrollU = 0;

	void Start ()
	{
		_mat = gameObject.GetComponent<Renderer>().sharedMaterial;

		_layer1ScrollU_ID 	= Shader.PropertyToID( "_Layer1ScrollU");
		_layer2ScrollU_ID 	= Shader.PropertyToID( "_Layer2ScrollU");
		_layer3ScrollU_ID 	= Shader.PropertyToID( "_Layer3ScrollU");
	}

	void Update ()
	{
		_layer1ScrollU = ( Mathf.PingPong ( Time.time, 1.0f ) - 0.5f ) * 0.2f;
		_mat.SetFloat ( _layer1ScrollU_ID, _layer1ScrollU );

		_layer2ScrollU = ( Mathf.PingPong ( Time.time * 0.5f, 1.0f ) - 0.5f ) * 0.5f;
		_mat.SetFloat ( _layer2ScrollU_ID, _layer2ScrollU );

		_layer3ScrollU = ( Mathf.PingPong ( Time.time, 1.0f ) - 0.5f ) * 0.9f;
		_mat.SetFloat ( _layer3ScrollU_ID, _layer3ScrollU );

	}
}
