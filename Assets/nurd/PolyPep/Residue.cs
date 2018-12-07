using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residue : MonoBehaviour {

	public GameObject amide_pf;
	public GameObject calpha_pf;
	public GameObject carbonyl_pf;

	public PolyPepBuilder myPolyPepBuilder;

	public GameObject sidechain;
	public int resid;
	public string type = "xxx";
	public string label;

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

	public GameObject Label_pf;
	public GameObject myLabel;
	public GameObject myPlotCubeLabel;
	public GameObject myPeptidePlane;

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

		myLabel = Instantiate(Label_pf, transform);
		myLabel.name = "residueLabel";

		myPlotCubeLabel = Instantiate(Label_pf, transform);
		myPlotCubeLabel.name = "plotCubeLabel";

		myPlotCubeLabel.GetComponent<TextMesh>().color = Color.black;
		myPlotCubeLabel.GetComponent<TextMesh>().characterSize = 0.0005f;
		myPlotCubeLabel.GetComponent<TextMesh>().fontSize = 250;
		myPlotCubeLabel.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
		myPlotCubeLabel.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;

		myPeptidePlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
		myPeptidePlane.name = "PeptidePlane";
		//myPeptidePlane.transform.parent = gameObject.transform;

		if (resid < 0)
		{
			//Mesh _mesh = myPeptidePlane.GetComponent<Mesh>();

			Debug.Log( carbonyl_pf.transform.Find("C_carbonyl").position );
			//_mesh.vertices[1] = carbonyl_pf.transform.Find("O_carbonyl").position;
			//_mesh.vertices[2] = myPolyPepBuilder.GetComponent<PolyPepBuilder>().GetAmideForResidue(resid - 1).transform.Find("N_amide").position;
			//_mesh.vertices[3] = myPolyPepBuilder.GetComponent<PolyPepBuilder>().GetAmideForResidue(resid - 1).transform.Find("H_amide").position;

		}


		
		
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

	private void UpdateLabels()
	{
		//TODO - camera facing prob expensive

		label = (resid+1).ToString() + ": " + type;
		myLabel.GetComponent<TextMesh>().text = label;
		myLabel.transform.position = calpha_pf.transform.position;

		myLabel.transform.localScale = new Vector3(-1, 1, 1);
		myLabel.transform.LookAt(Camera.main.transform);

		
		myPlotCubeLabel.GetComponent<TextMesh>().text = (resid + 1).ToString();
		myPlotCubeLabel.transform.position = myPlotCube.transform.position - (0.005f * myPlotCube.transform.forward);

		myPlotCubeLabel.transform.localScale = new Vector3(-1, 1, 1);
		myPlotCubeLabel.transform.LookAt(Camera.main.transform);

	}

	// Update is called once per frame
	void Update ()
	{
		MeasurePhiPsi();
		UpdatePhiPsiPlotObj();
		UpdateLabels();

		//if (resid > 0)
		//{
		//	Vector3 posC = myPolyPepBuilder.GetComponent<PolyPepBuilder>().GetCarbonylForResidue(resid - 1).transform.Find("C_carbonyl").position;
		//	Vector3 posCO = myPolyPepBuilder.GetComponent<PolyPepBuilder>().GetCarbonylForResidue(resid - 1).transform.Find("tf_O/O_carbonyl").position;
		//	Vector3 posN = amide_pf.transform.Find("N_amide").position;
		//	Vector3 posNH = amide_pf.transform.Find("tf_H/H_amide").position;
		//}


	}
}
