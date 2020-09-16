using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AlwaysLookTarget : MonoBehaviour 
{
	public Transform Target;
	public Vector3 ShowAngles;

	void Update () 
	{
		if (Target)
		{
			transform.LookAt(Target);
			ShowAngles = transform.rotation.eulerAngles;
		}
	}
}