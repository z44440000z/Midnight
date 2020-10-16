using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramControl : MonoBehaviour
{
	private Transform 	_transform;
	private int 		_progress_ID;
	private Material 	_mat;
	private Renderer 	_renderer;
	private float 		_progressTime;

	public float 		startDelay = 0;

	public Vector3 rotation;
	public AnimationCurve progressCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public float progressDuration = 2.0f;

	void Start ()
	{
		_renderer		= gameObject.GetComponent<Renderer>();
		_mat 			= _renderer.sharedMaterial;
		_progress_ID 	= Shader.PropertyToID( "_Layer0Progress");
		_transform 		= transform;

		_progressTime = 0.0f;
		_mat.SetFloat ( _progress_ID, 0.0f );
	}

	void Update ()
	{
		float progress;

		if ( startDelay > 0 )
		{
			startDelay -= Time.deltaTime;
		}
		else
		{
			_progressTime += Time.deltaTime;

			progress =  progressCurve.Evaluate(_progressTime/progressDuration);

			_mat.SetFloat ( _progress_ID, progress );
		}

		_transform.Rotate ( rotation * Time.deltaTime );
	}
}
