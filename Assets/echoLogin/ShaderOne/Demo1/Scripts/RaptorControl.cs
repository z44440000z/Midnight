using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaptorControl : MonoBehaviour
{
	private Material 	_mat;
	private int _layer0ScrollV_ID;
	private int _layer1ScrollU_ID;
	private int _layer2ScrollU_ID;

	private float _layer0ScrollV = 0;
	private float _layer1ScrollU = 0;
	private float _layer2ScrollU = 0;

	public float scrollSpeed 	= 0.3f;

	void Start ()
	{
		_mat = gameObject.GetComponent<Renderer>().sharedMaterial;

		_layer0ScrollV_ID 	= Shader.PropertyToID( "_Layer0ScrollV");
		_layer1ScrollU_ID 	= Shader.PropertyToID( "_Layer1ScrollU");
		_layer2ScrollU_ID 	= Shader.PropertyToID( "_Layer2ScrollU");
	}

	void Update ()
	{
		_layer0ScrollV += scrollSpeed * Time.deltaTime;

		_mat.SetFloat ( _layer0ScrollV_ID, _layer0ScrollV );

		_layer1ScrollU = ( Mathf.PingPong ( Time.time, 1.0f ) - 0.5f ) * 0.1f;
		_mat.SetFloat ( _layer1ScrollU_ID, _layer1ScrollU );

		_layer2ScrollU = ( Mathf.PingPong ( Time.time * 0.5f, 1.0f ) - 0.5f ) * 0.4f;
		_mat.SetFloat ( _layer2ScrollU_ID, _layer2ScrollU );
	}
}
