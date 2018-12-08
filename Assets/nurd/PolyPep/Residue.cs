using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residue : MonoBehaviour {

	public GameObject amide_pf;
	public GameObject calpha_pf;
	public GameObject carbonyl_pf;

	public PolyPepBuilder myPolyPepBuilder;
	public PolyPepManager myPolyPepManager;

	public GameObject sidechain;
	public int resid;
	public string type;
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

	private GameObject myPeptidePlane;

	public bool isNTerminal;
	public bool isCTerminal;

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
		myPlotCubeLabel.GetComponent<TextMesh>().fontSize = 300;
		myPlotCubeLabel.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
		myPlotCubeLabel.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;

		myPeptidePlane = carbonyl_pf.transform.Find("tf_bond_C_N/bond_CO_N/PeptidePlane").gameObject;


		if (resid == 0)
		{
			isNTerminal = true;
		}
		if (resid == (myPolyPepBuilder.GetComponent<PolyPepBuilder>().numResidues - 1))
		{
			isCTerminal = true;
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

	private void UpdateShowResLabels()
	{
		if (myPolyPepManager.GetComponent<PolyPepManager>().allResLabelsOn)
		{

			label = (resid+1).ToString() + ": " + type;
			myLabel.SetActive(true);
			myLabel.GetComponent<TextMesh>().text = label;
			myLabel.transform.position = calpha_pf.transform.position;

			// 180 x rot to face camera
			myLabel.transform.localScale = new Vector3(-1, 1, 1);
			myLabel.transform.LookAt(Camera.main.transform);

			myPlotCubeLabel.SetActive(true);
			myPlotCubeLabel.GetComponent<TextMesh>().text = (resid + 1).ToString();
			// TODO fix magic number in scaling - places label on top face of PlotCube
			myPlotCubeLabel.transform.position = myPlotCube.transform.position - (0.005f * myPlotCube.transform.forward);

			myPlotCubeLabel.transform.localScale = new Vector3(-1, 1, 1);
			myPlotCubeLabel.transform.LookAt(Camera.main.transform);
		}
		else
		{
			myLabel.SetActive(false);
			myPlotCubeLabel.SetActive(false);
		}
	}

	private void UpdateShowPeptidePlanes()
	{
		if ((myPolyPepManager.GetComponent<PolyPepManager>().showPeptidePlanes) && (!isCTerminal))
		{
			myPeptidePlane.SetActive(true);
		}
		else
		{
			myPeptidePlane.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		MeasurePhiPsi();
		UpdatePhiPsiPlotObj();
		UpdateShowResLabels();
		UpdateShowPeptidePlanes();
	}
}
		//{
		//  peptide bond atom positions
		//	Vector3 posC = myPolyPepBuilder.GetComponent<PolyPepBuilder>().GetCarbonylForResidue(resid - 1).transform.Find("C_carbonyl").position;
		//	Vector3 posCO = myPolyPepBuilder.GetComponent<PolyPepBuilder>().GetCarbonylForResidue(resid - 1).transform.Find("tf_O/O_carbonyl").position;
		//	Vector3 posN = amide_pf.transform.Find("N_amide").position;
		//	Vector3 posNH = amide_pf.transform.Find("tf_H/H_amide").position;
		//}