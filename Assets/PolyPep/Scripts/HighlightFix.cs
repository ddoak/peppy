using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
	public RectTransform myRT;
	public Vector3 myStartScale;
	public Vector3 myTargetScale;
	public Vector3 myCurrentScale;

	public Toggle myToggle;

	public float selectScaleFactor = 1.45f;
	public float toggleOnScaleFactor = 1.3f;

	public Color normalColor;

	public bool isHovered;

	void Start()
	{
		SetUpRectTransformScales();
	}

	private void SetUpRectTransformScales ()
	{
		// some very brittle code here
		// makes assumptions about how UI is set up

		// a slider
		myRT = this.transform.Find("Slide_Area/Handle") as RectTransform;

		if (myRT)
		{
			//Debug.Log("-> Slider");
		}

		if (!myRT)
		{
			// a button
			myRT = this.transform.Find("Background") as RectTransform;
			if (!myToggle)
			{
				selectScaleFactor = 1.25f;
			}
		}

		if (!myRT)
		{
			// a toggle ??
			myRT = this.transform as RectTransform;
		}

		if (!myRT)
		{
			Debug.Log("---> Failed to find Rect Transform for UI Element");
		}

		myStartScale = myRT.localScale;
		myTargetScale = myStartScale;
		myCurrentScale = myStartScale;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("OnPointerEnter " + name);
		
		//if (!EventSystem.current.alreadySelecting)
		//{
		//	Debug.Log("1");
		//	EventSystem.current.SetSelectedGameObject(this.gameObject);
		//}

		isHovered = true;
		//EventSystem.current.SetSelectedGameObject(this.gameObject);

		//Debug.Log("highlight fix enter");
		//Debug.Log("Cursor Entering " + name + " GameObject");
		//Debug.Log("-->" + eventData.pointerCurrentRaycast.gameObject.name);

		//if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
			//Debug.Log(myRT.localScale);

			myTargetScale.x = myStartScale.x * selectScaleFactor;
			myTargetScale.y = myStartScale.y * selectScaleFactor;
			myTargetScale.z = 1f;
		}
		//myRT.localScale = myTargetScale;
		//myCurrentScale = myTargetScale;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovered = false;
		Debug.Log("OnPointerExit " + name);
		//Debug.Log("highlight fix leave");
		//if (EventSystem.current.alreadySelecting)

		//{
		//	Debug.Log("Pointer Exit");

		//}

		//Debug.Log("Cursor Exiting " + name + " GameObject");

		//if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		//{
		//	Debug.Log("2");
		//	EventSystem.current.SetSelectedGameObject(null);
		//}

		{
			// Toggle latching (colour and scale)

			myTargetScale = myStartScale;
			if (myToggle)
			{
				if (myToggle.isOn)
				{
					{
						//Debug.Log(myRT.localScale);
						// Latch On
						myTargetScale.x = myStartScale.x * toggleOnScaleFactor;
						myTargetScale.y = myStartScale.y * toggleOnScaleFactor;
						myTargetScale.z = 1f;


						ColorBlock colors = myToggle.colors;
						colors.normalColor = myToggle.colors.highlightedColor;
						myToggle.colors = colors;
					}
				}
				else
				{
					// Latch Off
					ColorBlock colors = myToggle.colors;
					colors.normalColor = normalColor;
					myToggle.colors = colors;
				}
			}

			//Debug.Log(myRT.localScale);
			
			//myRT.localScale = myTargetScale;
			//myCurrentScale = myTargetScale;
		}

		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		Debug.Log("OnDeSelect " + name);
		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	public void OnSelect(BaseEventData eventData)
	{
		Debug.Log("OnSelect " + name);
		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	private void UpdateRTScale()
	{
		//Debug.Log("update");
		myCurrentScale = Vector3.Lerp(myCurrentScale, myTargetScale, ((Time.deltaTime / 0.01f) * 0.2f));
		myRT.localScale = myCurrentScale;
	}

	void Update()
	{
		UpdateRTScale();
		//Debug.Log(EventSystem.current.currentSelectedGameObject);
		//Debug.Log(EventSystem.current.IsPointerOverGameObject());


	}
}
//
//https://forum.unity.com/threads/button-keyboard-and-mouse-highlighting.294147/
//
