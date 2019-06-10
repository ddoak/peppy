using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource audioSource;

	public AudioClip enterAudioClip;
	public AudioClip latchOnAudioClip;
	public AudioClip latchOffAudioClip;

	float lastEnterTime = 0f;
	float retriggerThreshold = 0.2f;

	// Start is called before the first frame update
	void Start()
	{
		enterAudioClip = Resources.Load("Audio/blip_over02", typeof(AudioClip)) as AudioClip;
		latchOnAudioClip = Resources.Load("Audio/Pickup_on01", typeof(AudioClip)) as AudioClip;
		latchOffAudioClip = Resources.Load("Audio/Pickup_off01", typeof(AudioClip)) as AudioClip;
	}

	public void PlayAudioOnEnter()
	{
		if (Time.time > (lastEnterTime + retriggerThreshold))
		{
			lastEnterTime = Time.time;

			audioSource.clip = enterAudioClip;
			audioSource.volume = 0.25f;
			audioSource.pitch = 1.0f;
			audioSource.Play();
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
		audioSource.clip = latchOnAudioClip;
		audioSource.volume = 0.2f;
		audioSource.pitch = 1.0f;
		audioSource.Play();
	}

	public void PlayLatchOff()
	{
		audioSource.clip = latchOffAudioClip;
		audioSource.volume = 0.2f;
		audioSource.pitch = 1.0f;
		audioSource.Play();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
