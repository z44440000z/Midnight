using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[System.Serializable]
public class EchoSoundClip
{
	public int 		priority;
	public AudioClip 	clip;
}

//----------------------------------------------------------------------------
public class EchoAudioManager : MonoBehaviour
{
	public AudioClip[] 									songs;
	public EchoSoundClip[]                          	soundClips;
	public int         									numberOfChannels 	= 4;
	public float                            			globalMusicVol 		= 1.0f;
	public float                            			globalFXVol 		= 1.0f;
	public bool                         	    		musicOn        		= true;
	public bool                  		           		fxOn	       		= true;
	private static AudioSource 							_musicSource;
	private static AudioSource[] 						_fxSource;
	private static int              					_numberOfChannels;
	private static Dictionary <string, EchoSoundClip>	_fxLookup;
	private static Dictionary <string, AudioClip>		_musicLookup;
	private static int                              	_lastSound;
	private static float                            	_globalMusicVol = 1.0f;
	private static float                            	_globalFXVol 	= 1.0f;
	private static bool                             	_musicOn        = true;
	private static bool                             	_fxOn       	= true;

	//======================================================================
	void Awake ()
	{
		int loop;
		AudioClip clip;

		_numberOfChannels 	= numberOfChannels;
		_globalMusicVol 	= globalMusicVol;
		_globalFXVol 		= globalFXVol;
		_musicOn        	= musicOn;
		_fxOn 		      	= fxOn;

		_musicLookup = new Dictionary<string,AudioClip>();
		for ( loop = 0; loop < songs.Length; loop++ )
			_musicLookup.Add ( songs[loop].name, songs[loop] );

		_fxLookup = new Dictionary<string,EchoSoundClip>();
		for ( loop = 0; loop < soundClips.Length; loop++ )
		{
			clip = soundClips[loop].clip;
			_fxLookup.Add ( clip.name, soundClips[loop] );
			clip.LoadAudioData();
		}

		_musicSource 						= gameObject.AddComponent<AudioSource>();
		_musicSource.clip 					= null;
		_musicSource.volume 				= 1.0f;
		_musicSource.minDistance 			= 0.01f;
		_musicSource.maxDistance 			= 1024;
		_musicSource.ignoreListenerVolume 	= false;

		_fxSource 		= new AudioSource[numberOfChannels];

		for ( loop = 0; loop < _numberOfChannels; loop++ )
		{
			_fxSource[loop] 						= gameObject.AddComponent<AudioSource>();
			_fxSource[loop].clip 					= null;
			_fxSource[loop].volume 					= 1.0f;
			_fxSource[loop].minDistance 			= 0.01f;
			_fxSource[loop].maxDistance 			= 1024;
			_fxSource[loop].ignoreListenerVolume 	= false;
		}
	}

	//======================================================================
	public static void MuteMusic ( bool iflag )
	{
		_musicOn = iflag;
		_musicSource.Stop();
	}

	//======================================================================
	public static void MuteFX ( bool iflag )
	{
		_fxOn = iflag;

		for ( int loop = 0; loop < _numberOfChannels; loop++ )
		{
			if ( _fxSource[loop].isPlaying )
			{
				_fxSource[loop].Stop();
				break;
			}
		}
	}

	//======================================================================
	public static void GlobalMusicVolume ( float ivol )
	{
		_globalMusicVol 	= ivol;
		_musicSource.volume = ivol;
	}

	//======================================================================
	public static void GlobalFXVolume ( float ivol )
	{
		_globalFXVol = ivol;

		for ( int loop = 0; loop < _numberOfChannels; loop++ )
		{
			if ( _fxSource[loop].isPlaying )
			{
				_fxSource[loop].volume = ivol;
				break;
			}
		}
	}

	//======================================================================
	public static void GlobalVolume ( float ivol )
	{
		AudioListener.volume = ivol;
	}

	//======================================================================
	public static void PlaySong ( string iname, bool iloop = true, float ivolume = 1.0f )
	{
		if ( !_musicOn || _globalMusicVol <= 0.0f )
			return;

		AudioClip clip = _musicLookup[iname];

		if ( clip == null )
			return;

		_musicSource.loop       = iloop;
		_musicSource.clip 		= clip;
		_musicSource.volume 	= ivolume * _globalMusicVol;
		_musicSource.priority 	= 0;
		_musicSource.Play ();
	}

	//======================================================================
	public static void StopSong()
	{
		_musicSource.Stop();
	}

	//======================================================================
	public static void PlaySound ( string iname, float ivolume = 1.0f, float ipitch = 1.0f )
	{
		int 		pos;
		AudioSource asrc;
//		AudioClip   clip;
		int 		lastHold;
		int 		loop;
		int 		highNum;
		int         highIndex;

		if ( !_fxOn || _globalFXVol <= 0.0f )
			return;

		EchoSoundClip esc = _fxLookup[iname];

		if ( esc == null )
			return;

		asrc 		= null;
		pos 		= ( _lastSound + 1 ) % _numberOfChannels;
		lastHold 	= pos;

		for ( loop = 0; loop < _numberOfChannels; loop++ )
		{
			if ( !_fxSource[pos].isPlaying )
			{
				asrc = _fxSource[pos];
				lastHold = pos;
				break;
			}

			pos = ( pos + 1 ) % _numberOfChannels;
		}


		if ( asrc == null )
		{
			EchoSoundClip escCur;

			highNum 	= -9999;
			highIndex 	= 0;

			for ( loop = 0; loop < _numberOfChannels; loop++ )
			{
				escCur = _fxLookup[_fxSource[loop].clip.name];

				if ( esc.priority <= escCur.priority && escCur.priority > highNum )
				{
					highNum 	= escCur.priority;
					highIndex 	= loop;
				}
			}

			if ( highNum >= 0 )
			{
				asrc = _fxSource[highIndex];
			}
//			else
//			{
//				asrc = _fxSource[_lastSound];
//				_lastSound = ( _lastSound + 1 ) % _numberOfChannels;
//			}

		}
		else
		{
			_lastSound = lastHold;
		}

		if ( asrc )
		{
			asrc.pitch      = ipitch;
			asrc.clip 		= esc.clip;
			asrc.volume 	= ivolume * _globalFXVol;
			asrc.priority 	= 0;
			asrc.Play ();
		}
	}

	//======================================================================
	public static void PlaySoundRandomPitch ( string iname, float ivolume = 1.0f, float ipitchStart = 1.0f, float ipitchEnd = 1.0f )
	{
		PlaySound ( iname, ivolume, UnityEngine.Random.Range ( ipitchStart, ipitchEnd ) );
	}

	//======================================================================
	public static void PlaySoundRandomSound ( string[] inames, float ivolume = 1.0f, float ipitch = 1.0f )
	{
		PlaySound ( inames[ UnityEngine.Random.Range ( 0, inames.Length ) ], ivolume, ipitch );
	}

	//======================================================================
	public static void PlaySoundRandomSoundPitch ( string[] inames, float ivolume = 1.0f, float ipitchStart = 1.0f, float ipitchEnd = 1.0f )
	{
		PlaySound ( inames[ UnityEngine.Random.Range ( 0, inames.Length ) ], ivolume, UnityEngine.Random.Range ( ipitchStart, ipitchEnd ) );
	}


	//======================================================================
	//void Update ()
	//{
	//
	//}
}
