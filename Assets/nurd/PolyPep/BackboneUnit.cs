


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

	public Residue myResidue;

	// 
	Renderer rendererPhi;	// amide only
	Renderer rendererPsi;   // calpha only
	Renderer rendererPeptide; // carbonyl only

	// Use this for initialization
	void Start () {

		myResidue = (gameObject.transform.parent.gameObject.GetComponent("Residue") as Residue);
		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");
		UpdateRenderMode();

		// 
		//if (tag == "amide" || tag == "calpha")
		{
			Renderer[] allChildRenderers =gameObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer childRenderer in allChildRenderers)
			{
				Debug.Log(childRenderer.transform.parent.name);

				if (childRenderer.transform.parent.name == "tf_bond_N_CA")
				{
					rendererPhi = childRenderer;
				}
				if (childRenderer.transform.parent.name == "tf_bond_CA_CO")
				{
					rendererPsi = childRenderer;
				}
				if (childRenderer.transform.parent.name == "tf_bond_C_N")
				{
					rendererPeptide = childRenderer;
				}
			}
		}
	}

	public void SetBackboneUnitControllerHover(bool flag)
	{
		controllerHoverOn = flag;
		UpdateRenderMode();
	}

	public void SetMyResidueSelect(bool flag)
	{
		//Residue res = (gameObject.transform.parent.gameObject.GetComponent("Residue") as Residue);
		if (myResidue)
		{
			//Debug.Log("             " + res);
			BackboneUnit buAmide = myResidue.amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCalpha = myResidue.calpha_pf.GetComponent("BackboneUnit") as BackboneUnit;
			BackboneUnit buCarbonyl = myResidue.carbonyl_pf.GetComponent("BackboneUnit") as BackboneUnit;

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
							childRenderer.GetComponent<Renderer>().material.shader = shaderStandard;
						}
						//{
						//	Renderer _renderer = childRenderer.GetComponent<Renderer>();
						//	_renderer.material.shader = shaderToonOutline;
						//	_renderer.material.SetColor("_OutlineColor", Color.green);
						//	_renderer.material.SetFloat("_Outline", 0.005f);
						//}
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

				if (childRenderer == rendererPhi)
				{
					if (myResidue.drivePhiPsiOn)
					{
						Renderer _renderer = childRenderer.GetComponent<Renderer>();
						_renderer.material.shader = shaderToonOutline;
						_renderer.material.SetColor("_OutlineColor", Color.cyan);
						_renderer.material.SetFloat("_Outline", 0.005f);
					}
					else
					{
						childRenderer.GetComponent<Renderer>().material.shader = shaderStandard;
					}
				}
				if (childRenderer == rendererPsi)
				{
					if (myResidue.drivePhiPsiOn)
					{
						Renderer _renderer = childRenderer.GetComponent<Renderer>();
						_renderer.material.shader = shaderToonOutline;
						_renderer.material.SetColor("_OutlineColor", Color.magenta);
						_renderer.material.SetFloat("_Outline", 0.005f);
					}
					else
					{
						childRenderer.GetComponent<Renderer>().material.shader = shaderStandard;
					}
				}
				if (childRenderer == rendererPeptide)
				{
					if (true)
					{
						Renderer _renderer = childRenderer.GetComponent<Renderer>();
						_renderer.material.shader = shaderToonOutline;
						_renderer.material.SetColor("_OutlineColor", Color.black);
						_renderer.material.SetFloat("_Outline", 0.005f);
					}
					else
					{

					}
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
