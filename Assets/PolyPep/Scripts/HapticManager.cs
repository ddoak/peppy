using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControllerSelection;

public class HapticManager : MonoBehaviour
{
	public AudioClip hapticAudioClip;

	float lastEnterTime = 0f;
	float retriggerThreshold = 0.2f;

	// keep track of active controller - 
	public OVRInput.Controller activeController = OVRInput.Controller.None;

	// Start is called before the first frame update
	void Start()
    {
		//hapticAudioClip = Resources.Load("Audio/Click_Soft_01", typeof(AudioClip)) as AudioClip;
		//hapticAudioClip = Resources.Load("Audio/FX13 - Bleep 2", typeof(AudioClip)) as AudioClip;
		//hapticAudioClip = Resources.Load("Audio/DM-CGS-21", typeof(AudioClip)) as AudioClip;
		hapticAudioClip = Resources.Load("Audio/Chirp02", typeof(AudioClip)) as AudioClip;
	}

	public void PlayHapticOnEnter()
	{
		OVRHapticsClip hapticsClip = new OVRHapticsClip(hapticAudioClip);

		//Debug.Log(OVRInput.GetActiveController());

		if (Time.time > (lastEnterTime + retriggerThreshold))
		{
			lastEnterTime = Time.time;

			if (activeController == OVRInput.Controller.LTouch)
			{
				OVRHaptics.LeftChannel.Preempt(hapticsClip);
			}
			else if (activeController == OVRInput.Controller.RTouch)
			{
				OVRHaptics.RightChannel.Preempt(hapticsClip);
			}
		}
	}

	// Update is called once per frame
	void Update()
    {
		activeController = OVRInputHelpers.GetControllerForButton(OVRInput.Button.PrimaryIndexTrigger, activeController);
	}
}
