


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboneUnit : MonoBehaviour {

	public int residue = -1;

	Shader shaderStandard;
	Shader shaderToonOutline;

	public bool activeSequenceSelect = false;
	private bool activeSequenceSelectLast = false;
	public bool controllerHoverOn = false;


	// Use this for initialization
	void Start () {

		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");
		UpdateRenderMode();

	}

	public void  SetBackboneUnitControllerHover(bool flag)
	{
		controllerHoverOn = flag;
		UpdateRenderMode();
	}

	public void SetActiveSequenceSelect(bool flag)
	{
		activeSequenceSelect = flag;
		if (activeSequenceSelect != activeSequenceSelectLast)
		{
			UpdateRenderMode();
			activeSequenceSelectLast = activeSequenceSelect;	
		}
	}


	private void SetRenderingMode(GameObject go, string shaderName)
	{
		Renderer[] allChildRenderers = go.GetComponentsInChildren<Renderer>();
		foreach (Renderer childRenderer in allChildRenderers)
		{
			//Debug.Log(child.GetType());

			var _type = childRenderer.GetType();
			if (_type.ToString() != "UnityEngine.ParticleSystemRenderer")

			{
				switch (shaderName)

				{
					case "ToonOutlineGreen":
						{
							Renderer _renderer = childRenderer.GetComponent<Renderer>();
							_renderer.material.shader = shaderToonOutline;
							_renderer.material.SetColor("_OutlineColor", Color.green);
							_renderer.material.SetFloat("_Outline", 0.005f);
						}
						break;

					case "ToonOutlineRed":
						{
							Renderer _renderer = childRenderer.GetComponent<Renderer>();
							_renderer.material.shader = shaderToonOutline;
							_renderer.material.SetColor("_OutlineColor", Color.red);
							_renderer.material.SetFloat("_Outline", 0.005f);
						}
						break;

					case "Standard":
						{
							childRenderer.GetComponent<Renderer>().material.shader = shaderStandard;
						}
						break;

				}
			}
			else
			{
				// particle system
			}
		}
	}

	public void UpdateRenderMode()
	{
		if (controllerHoverOn)
		{
			SetRenderingMode(gameObject, "ToonOutlineRed");
		}
		else if (activeSequenceSelect)
		{
			SetRenderingMode(gameObject, "ToonOutlineGreen");
		}
		else
		{
			SetRenderingMode(gameObject, "Standard");
		}
	}

	// Update is called once per frame
	void Update () {
		//UpdateRenderMode();
	}
}
