using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicTMP : MonoBehaviour {

	public TMPro.TextMeshProUGUI textComponent;


	void Awake()
	{

		textComponent = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
		//Debug.Log(gameObject + " " + textComponent);
		if (textComponent == null)
		{
			//textComponent = gameObject.GetComponent<TextMeshProUGUI>();
		}
	}

	private void Start()
	{

	}

	public void SetSliderValue(float sliderValue)
	{
		//Debug.Log(sliderValue);
		textComponent.text = Mathf.Round(sliderValue/1).ToString();
	}

	public void SetSliderValue10(float sliderValue)
	{
		textComponent.text = System.Math.Round((sliderValue/10),1).ToString();
	}

	public void SetSliderValue100(float sliderValue)
	{
		textComponent.text = System.Math.Round((sliderValue / 100), 1).ToString();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
