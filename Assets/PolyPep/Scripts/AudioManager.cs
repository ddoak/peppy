using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource audioSource1;
	public AudioSource audioSource2;

	public AudioClip enterAudioClip;
	public AudioClip latchOnAudioClip;
	public AudioClip latchOffAudioClip;

	public AudioClip selectOnAudioClip;
	public AudioClip selectOffAudioClip;
	public AudioClip selectInvertAudioClip;

	public AudioClip setSecondaryAudioClip;

	public AudioClip selectGenericAudioClip;

	public AudioClip spawnAudioClip;

	public float masterVolume = 1.0f;

	float lastEnterTime = 0f;
	float retriggerThreshold = 0.2f;

	// Start is called before the first frame update
	void Start()
	{
		enterAudioClip = Resources.Load("Audio/Click03a", typeof(AudioClip)) as AudioClip;

		spawnAudioClip = Resources.Load("Audio/FX3", typeof(AudioClip)) as AudioClip;

		latchOnAudioClip = Resources.Load("Audio/FX55", typeof(AudioClip)) as AudioClip;
		latchOffAudioClip = Resources.Load("Audio/FX56", typeof(AudioClip)) as AudioClip;

		selectOnAudioClip = Resources.Load("Audio/FX58", typeof(AudioClip)) as AudioClip;
		selectOffAudioClip = Resources.Load("Audio/FX59", typeof(AudioClip)) as AudioClip;
		selectInvertAudioClip = Resources.Load("Audio/FX60", typeof(AudioClip)) as AudioClip;

		setSecondaryAudioClip = Resources.Load("Audio/FX10", typeof(AudioClip)) as AudioClip;

		selectGenericAudioClip = Resources.Load("Audio/FX51 - Select 4", typeof(AudioClip)) as AudioClip;

	}

	public void UpdateSfxVolumeFromUI(float sfxVolume)
	{
		masterVolume = sfxVolume * 10f / 25f;
		PlayAudio(audioSource1, selectGenericAudioClip, 0.1f);
	}

	public void PlayAudioOnEnter()
	{
		if (Time.time > (lastEnterTime + retriggerThreshold))
		{
			lastEnterTime = Time.time;
			PlayAudio(audioSource1, enterAudioClip, 0.1f);
			audioSource1.Play();
		}
	}

	public void PlayOnOffSfx(bool value)
	{
		if (value == true)
		{
			PlayLatchOn();
		}
		else
		{
			PlayLatchOff();
		}
	}

	public void PlayLatchOn()
	{
		PlayAudio(audioSource2, latchOnAudioClip, 0.2f);
	}

	public void PlayLatchOff()
	{
		PlayAudio(audioSource2, latchOffAudioClip, 0.2f);
	}


	public void PlaySelectSfx(bool value)
	{
		if (value == true)
		{
			PlaySelectOn();
		}
		else
		{
			PlaySelectOff();
		}
	}

	public void PlaySelectOn()
	{
		PlayAudio(audioSource2, selectOnAudioClip, 0.2f);
	}

	public void PlaySelectOff()
	{
		PlayAudio(audioSource2, selectOffAudioClip, 0.2f);
	}

	public void PlaySelectInvert()
	{
		PlayAudio(audioSource2, selectInvertAudioClip, 0.2f);
	}

	public void PlaySpawn()
	{
		PlayAudio(audioSource2, spawnAudioClip, 0.2f);
	}

	public void PlaySetSecondary()
	{
		PlayAudio(audioSource2, setSecondaryAudioClip, 0.1f);
	}

	private void PlayAudio(AudioSource audioSource, AudioClip audioclip, float volume)
	{
		audioSource.clip = audioclip;
		audioSource.volume = volume * masterVolume;
		audioSource.pitch = 1f;
		audioSource.Play();
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
