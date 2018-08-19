using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour {

	Text textComponent;

	void Start()
	{
		textComponent = GetComponent<Text>();
	}

	public void SetSliderValue(float sliderValue)
	{
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
