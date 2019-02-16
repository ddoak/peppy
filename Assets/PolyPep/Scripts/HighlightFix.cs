using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
	public RectTransform myRT;

	void Start()
	{


		// some very brittle code here
		// makes lots of assumptions about how UI is set up

		myRT = this.transform.Find("Slide_Area/Handle") as RectTransform;
			if (myRT)
		{
			Debug.Log("===========> Hurrah!");
		}

			if (!myRT)
			{
				myRT = this.transform.Find("Background") as RectTransform;
			}

			if (!myRT)
			{
				myRT = this.transform as RectTransform;
			}
		

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//if (!EventSystem.current.alreadySelecting)
		//	EventSystem.current.SetSelectedGameObject(this.gameObject);
		//Debug.Log("highlight fix enter");

		//if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
			Debug.Log(myRT.localScale);

			myRT.localScale = new Vector3 (1.2f, (0.9f*1.2f), 1f);

		}

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("highlight fix leave");
		//if (EventSystem.current.alreadySelecting)
			
		//{
		//	Debug.Log("1");
			
		//}

		if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
			Debug.Log("2");
			EventSystem.current.SetSelectedGameObject(null);
		}

		{
			Debug.Log(myRT.localScale);

			myRT.localScale = new Vector3(1.0f, (0.9f), 1f);

		}

		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		//this.GetComponent<Selectable>().OnPointerExit(null);
	}

	void Update()
	{
		//if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		//{
		//	RectTransform _bg = this.transform.Find("Background") as RectTransform;

		//	Debug.Log("3");

		//}
	}
}
//
//https://forum.unity.com/threads/button-keyboard-and-mouse-highlighting.294147/
//
