using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residue : MonoBehaviour {

	public GameObject amide_pf;
	public GameObject calpha_pf;
	public GameObject carbonyl_pf;

	public GameObject sidechain;
	public string type = "XXX";

	public List<GameObject> sideChainList = new List<GameObject>();

	public float phiTarget;
	public float psiTarget;

	public float phiCurrent;
	public float psiCurrent;

	public GameObject ramaPlot;
	public float ramaPlotScale = 0.0012f; //  set visually against UI

	public GameObject myPlotCube;
	private Vector3 myPlotCubeBaseScale = new Vector3(0.01f, 0.01f, 0.01f);
	float deltaScale = 1.0f;

	public bool residueSelected = false;
	public bool residueHovered = false;
	public bool residueGrabbed = false;

	public bool drivePhiPsiOn;
	public float drivePhiPsiTorqValue;

	// Use this for initialization
	private void Awake()
	{

	}

	void Start ()
	{
		ramaPlot = GameObject.Find("RamaPlotOrigin");

		myPlotCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myPlotCube.name = "PlotCube";
		myPlotCube.GetComponent<Collider>().isTrigger = true;
		myPlotCube.GetComponent<Collider>().enabled = false;
		myPlotCube.transform.parent = gameObject.transform;
		myPlotCube.transform.localScale = myPlotCubeBaseScale;
		myPlotCube.transform.rotation = ramaPlot.transform.rotation;
		myPlotCube.transform.position = ramaPlot.transform.position;

	}
	
	void MeasurePhiPsi()
	{
		phiCurrent = 180.0f - Vector3.SignedAngle(amide_pf.transform.up, calpha_pf.transform.up, amide_pf.transform.right);
		if (phiCurrent > 180.0f)
		{
			phiCurrent -= 360.0f;
		}
		//Debug.Log(phiTarget + " " + phiCurrent);

		psiCurrent = - Vector3.SignedAngle(calpha_pf.transform.up, carbonyl_pf.transform.up, calpha_pf.transform.right);

		//Debug.Log(psiTarget + " " + psiCurrent);
	}

	void UpdatePhiPsiPlotObj()
	{ 

		Renderer _myPlotCubeRenderer  = myPlotCube.GetComponent<Renderer>();
		Vector3 deltaPos = new Vector3(0.0f, 0.0f, 0.0f);
		float targetDeltaScale = 1.0f;
		if (residueGrabbed)
		{
			_myPlotCubeRenderer.material.SetColor("_Color", Color.green);
			//deltaPos += ramaPlot.transform.forward * -0.015f;
			targetDeltaScale = 1.6f;
		}
		else
		{
			if (residueHovered)
			{
				_myPlotCubeRenderer.material.SetColor("_Color", Color.red);
				//deltaPos += ramaPlot.transform.forward * -0.01f;
				targetDeltaScale = 1.4f;
			}
			else
			{
				if(residueSelected)
				{
					_myPlotCubeRenderer.material.SetColor("_Color", Color.yellow);
					targetDeltaScale = 1.2f;
				}
				else
				{
					_myPlotCubeRenderer.material.SetColor("_Color", Color.white);
					targetDeltaScale = 1.0f;
				}
			}
		}


		myPlotCube.transform.rotation = ramaPlot.transform.rotation;
		myPlotCube.transform.position = ramaPlot.transform.position + (ramaPlot.transform.right * ramaPlotScale * phiCurrent) + (ramaPlot.transform.up * ramaPlotScale * psiCurrent) + deltaPos;
		deltaScale = Mathf.Lerp(deltaScale, targetDeltaScale, 0.2f);
		myPlotCube.transform.localScale = myPlotCubeBaseScale * deltaScale;
	}

	public bool IsResidueSelected()
	{
		BackboneUnit buAmide = amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
		return buAmide.controllerSelectOn;
	}

	public void DisableProxySideChain()
	{
		// switch off the proxy sidechain renderer
		foreach (Renderer renderer in calpha_pf.GetComponentsInChildren<Renderer>())
		{
			//Debug.Log(renderer);
			if (renderer.name == "R_sidechain" || renderer.name == "bond_CA_R")
			{
				renderer.enabled = false;
			}
		}
		// switch off the proxy sidechain collider
		foreach (Collider collider in calpha_pf.GetComponentsInChildren<Collider>())
		{
			//Debug.Log(collider);
			if (collider.name == "R_sidechain" || collider.name == "bond_CA_R")
			{
				collider.enabled = false;
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		MeasurePhiPsi();
		UpdatePhiPsiPlotObj();
	}
}
