using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramDiskControl : MonoBehaviour
{
	private GameObject[] 	_disks;
	private float 			_curDisc = 0;
	private bool            _fxEnabled = true;

	public float 			startDelay = 2;

	void Start ()
	{
		int loop;

		_disks = new GameObject[5];

		for ( loop = 0; loop < 5; loop++ )
		{
			_disks[loop] = transform.Find ( "d" + ( loop + 1 ) ).gameObject;
			_disks[loop].SetActive(false);
		}
	}

	void Update ()
	{
		int loop;

		if (!_fxEnabled)
			return;

		if ( startDelay > 0 )
			startDelay -= Time.deltaTime;
		else
		{
			for ( loop = 0; loop < 5; loop++ )
			{
				if ( loop == (int)_curDisc)
				{
					_disks[loop].SetActive(true);
				}
				else
				{
					_disks[loop].SetActive(false);
				}
			}

			_curDisc += Time.deltaTime * 5.0f;

			if (_curDisc > 6 )
			{
				_fxEnabled = false;
			}
		}
	}
}
