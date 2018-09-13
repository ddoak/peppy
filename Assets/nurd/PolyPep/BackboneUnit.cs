


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
	public bool controllerSelectOn = false;

	// Use this for initialization
	void Start () {

		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");
		UpdateRenderMode();

	}

	public void SetBackboneUnitControllerHover(bool flag)
	{
		controllerHoverOn = flag;
		UpdateRenderMode();
	}

	public void SetMyResidueSelect(bool flag)
	{
		Residue res = (gameObject.transform.parent.gameObject.GetComponent("Residue") as Residue);
		if (res)
		{
			//Debug.Log("             " + res);
			BackboneUnit buAmide = res.amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCalpha = res.calpha_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCarbonyl = res.carbonyl_pf.GetComponent("BackboneUnit") as BackboneUnit;

			buAmide.SetBackboneUnitSelect(flag);
			buCalpha.SetBackboneUnitSelect(flag);
			buCarbonyl.SetBackboneUnitSelect(flag);
		}
	}

	public void SetBackboneUnitSelect (bool flag)
	{
		controllerSelectOn = flag;
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

	public void TractorBeam(Ray pointer, bool attract)
	{
		Debug.Log("tractor beam me!");

		float tractorBeamAttractionFactor = 50.0f;

		Vector3 tractorBeam = pointer.origin - gameObject.transform.position;
		if (!attract)
		{
			// repel
			tractorBeam = gameObject.transform.position - pointer.origin;
		}
		float tractorBeamScale = Mathf.Max(100.0f, tractorBeamAttractionFactor * (Vector3.Magnitude(tractorBeam) / 500.0f));
		gameObject.GetComponent<Rigidbody>().AddForce((tractorBeam * tractorBeamScale), ForceMode.Acceleration);

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

					case "ToonOutlineYellow":
						{
							Renderer _renderer = childRenderer.GetComponent<Renderer>();
							_renderer.material.shader = shaderToonOutline;
							_renderer.material.SetColor("_OutlineColor", Color.yellow);
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
		else if (controllerSelectOn)
		{
			SetRenderingMode(gameObject, "ToonOutlineYellow");
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
