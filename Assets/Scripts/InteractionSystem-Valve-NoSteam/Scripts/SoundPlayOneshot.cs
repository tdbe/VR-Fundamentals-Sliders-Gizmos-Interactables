//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Play one-shot sounds as opposed to continuos/looping ones
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	//[RequireComponent(typeof(AudioSource))]
	public class SoundPlayOneshot : MonoBehaviour
	{
		public AudioClip[] waveFiles;
		
		public AudioSource thisAudioSource;

		public float volMin = 1;
		public float volMax = 1;

		public float pitchMin = 1;
		public float pitchMax = 1;

		public bool playOnAwake = false;

		public bool playAllSoundsInArrayAtOnce = true;


		//-------------------------------------------------
		void Awake()
		{
			if(thisAudioSource == null)
				thisAudioSource = GetComponent<AudioSource>();

			if ( playOnAwake )
			{
				Play();
			}
		}


		//-------------------------------------------------
		public void Play(int indexToPlay = -1)
		{
			//Debug.Log("Attempting to play OneShot audio on "+gameObject.name);

			if ( thisAudioSource != null && thisAudioSource.isActiveAndEnabled && !Util.IsNullOrEmpty( waveFiles ) )
			{	//Debug.Log("Playing OneShot audio on "+gameObject.name);
				//randomly apply a volume between the volume min max
				thisAudioSource.volume = Random.Range( volMin, volMax );

				//randomly apply a pitch between the pitch min max
				thisAudioSource.pitch = Random.Range( pitchMin, pitchMax );

				// play the sound
				if(playAllSoundsInArrayAtOnce){
					for(int i = 0; i< waveFiles.Length; i++){
						thisAudioSource.PlayOneShot( waveFiles[i] );
					}	
				}
				else if(indexToPlay>=0){
					thisAudioSource.PlayOneShot( waveFiles[indexToPlay] );
				}
				else
				{
					thisAudioSource.PlayOneShot( waveFiles[Random.Range( 0, waveFiles.Length )] );
					
				}
			}
		}


		//-------------------------------------------------
		public void Pause()
		{
			if ( thisAudioSource != null )
			{
				thisAudioSource.Pause();
			}
		}


		//-------------------------------------------------
		public void UnPause()
		{
			if ( thisAudioSource != null )
			{
				thisAudioSource.UnPause();
			}
		}
	}
}
