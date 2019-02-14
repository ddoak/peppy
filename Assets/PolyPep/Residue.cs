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

	public Transform N;
	public Transform HN;
	public Transform C;
	public Transform O;

	public List<Transform> backboneAtomsList = new List<Transform>();
	public List<GameObject> sideChainList = new List<GameObject>();

	public float phiTarget;
	public float psiTarget;

	public float phiCurrent;
	public float psiCurrent;

	public GameObject ramaPlotOrigin;
	public float ramaPlotScale = 0.0012f; //  set visually against UI

	public ParticleSystem plotTrail_ps;
	private ParticleSystem myPlotTrail_ps;
	private ParticleSystem.MainModule myTrailPsMain;

	public GameObject myPlotCube;
	private Vector3 myPlotCubeBaseScale = new Vector3(0.018f, 0.018f, 0.01f);
	float deltaScale = 1.0f;

	public GameObject Label_pf;
	public GameObject myLabel;
	public GameObject myPlotCubeLabel;

	public GameObject LabelTMP_pf;
	public GameObject myLabelTMP;

	private GameObject myPeptidePlane;

	public bool isNTerminal;
	public bool isCTerminal;

	public bool residueSelected = false;
	public bool residueHovered = false;
	public bool residueGrabbed = false;

	public bool drivePhiPsiOn;
	public float drivePhiPsiTorqValue;

	public Residue disulphidePartnerResidue = null;
	public ConfigurableJoint disulphideCj = null;

	// Use this for initialization
	private void Awake()
	{
		
	}

	void Start ()
	{
		ramaPlotOrigin = GameObject.Find("RamaPlotOrigin_tf");

		myPlotCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myPlotCube.name = "PlotCube";
		myPlotCube.GetComponent<Collider>().isTrigger = true;
		myPlotCube.GetComponent<Collider>().enabled = false;
		myPlotCube.transform.parent = gameObject.transform;
		myPlotCube.transform.localScale = myPlotCubeBaseScale;
		myPlotCube.transform.rotation = ramaPlotOrigin.transform.rotation;
		myPlotCube.transform.position = ramaPlotOrigin.transform.position;



		Renderer myRenderer = myPlotCube.GetComponent<Renderer>();
		myRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		myRenderer.receiveShadows = false;


		myLabel = Instantiate(Label_pf, transform);
		myLabel.name = "residueLabel";

		myLabelTMP = Instantiate(LabelTMP_pf, transform);
		myLabelTMP.name = "residueLabelTMP";

		myLabel.SetActive(false);
		myLabelTMP.SetActive(false);

		myPlotCubeLabel = Instantiate(Label_pf, transform);
		myPlotCubeLabel.name = "plotCubeLabel";

		// plot particle trail
		// note: particle alpha in build is different from in editor 
		// bug? material? shader?
		// for the moment compromise is to have trails with reasonable alpha in build
		// which means they are almost invisible in editor
		myPlotTrail_ps = Instantiate(plotTrail_ps, myPlotCube.transform);
		myTrailPsMain = myPlotTrail_ps.main;
		myTrailPsMain.startSize = 0.02f;
		myTrailPsMain.customSimulationSpace = ramaPlotOrigin.transform;
		ParticleSystem.EmissionModule psEmission = myPlotTrail_ps.emission;
		psEmission.rateOverTime = 200;

		myPlotCubeLabel.GetComponent<TextMesh>().color = Color.black;
		myPlotCubeLabel.GetComponent<TextMesh>().characterSize = 0.0008f;
		myPlotCubeLabel.GetComponent<TextMesh>().fontSize = 300;
		myPlotCubeLabel.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
		myPlotCubeLabel.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;

		myPeptidePlane = carbonyl_pf.transform.Find("tf_bond_C_N/bond_CO_N/PeptidePlane").gameObject;

		// is terminal residue ?
		if (resid == 0)
		{
			isNTerminal = true;
		}
		if (resid == (myPolyPepBuilder.GetComponent<PolyPepBuilder>().numResidues - 1))
		{
			isCTerminal = true;
		}

		// store references to backbone atom transforms
		N = amide_pf.transform.Find("N_amide");
		HN = amide_pf.transform.Find("tf_H/H_amide");
		C = carbonyl_pf.transform.Find("C_carbonyl");
		O = carbonyl_pf.transform.Find("tf_O/O_carbonyl");

		backboneAtomsList.Add(N);
		backboneAtomsList.Add(HN);
		backboneAtomsList.Add(C);
		backboneAtomsList.Add(O);
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
		if (myPolyPepManager.showPhiPsiTrail && !myPlotTrail_ps.isPlaying)
		{
			myPlotTrail_ps.Play();
		}
		if (!myPolyPepManager.showPhiPsiTrail && myPlotTrail_ps.isPlaying)
		{
			myPlotTrail_ps.Stop();
		}

		Renderer _myPlotCubeRenderer  = myPlotCube.GetComponent<Renderer>();
		Vector3 deltaPos = new Vector3(0.0f, 0.0f, 0.0f);
		float targetDeltaScale = 1.0f;
		if (residueGrabbed)
		{
			_myPlotCubeRenderer.material.SetColor("_Color", Color.green);
			//deltaPos += ramaPlot.transform.forward * -0.015f;
			targetDeltaScale = 1.6f;
			myTrailPsMain.startColor = new Color(0.5f, 1f, 0.5f, 1f); // Color.green;
			myTrailPsMain.startSize = 0.02f;
		}
		else
		{
			if (residueHovered)
			{
				_myPlotCubeRenderer.material.SetColor("_Color", Color.red);
				//deltaPos += ramaPlot.transform.forward * -0.01f;
				targetDeltaScale = 1.4f;
				myTrailPsMain.startColor = new Color(1f, 0.5f, 0.5f, 1f); //Color.red;
				myTrailPsMain.startSize = 0.02f;
			}
			else
			{
				if(residueSelected)
				{
					_myPlotCubeRenderer.material.SetColor("_Color", Color.yellow);
					targetDeltaScale = 1.2f;
					myTrailPsMain.startColor = new Color(0.7f, 0.6f, 0f, 1f); // Color.yellow;
					myTrailPsMain.startSize = 0.02f;
				}
				else
				{
					_myPlotCubeRenderer.material.SetColor("_Color", Color.white);
					targetDeltaScale = 1.0f;
					myTrailPsMain.startColor = new Color(1f, 1f, 1f, 0.8f);  //white;
					myTrailPsMain.startSize = 0.01f;
				}
			}
		}


		myPlotCube.transform.rotation = ramaPlotOrigin.transform.rotation;
		myPlotCube.transform.position = ramaPlotOrigin.transform.position + (ramaPlotOrigin.transform.right * ramaPlotScale * phiCurrent) + (ramaPlotOrigin.transform.up * ramaPlotScale * psiCurrent) + deltaPos;
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

	public void EnableProxySideChain()
	{
		// switch off the proxy sidechain renderer
		foreach (Renderer renderer in calpha_pf.GetComponentsInChildren<Renderer>())
		{
			//Debug.Log(renderer);
			if (renderer.name == "R_sidechain" || renderer.name == "bond_CA_R")
			{
				renderer.enabled = true;
			}
		}
		// switch off the proxy sidechain collider
		foreach (Collider collider in calpha_pf.GetComponentsInChildren<Collider>())
		{
			//Debug.Log(collider);
			if (collider.name == "R_sidechain" || collider.name == "bond_CA_R")
			{
				collider.enabled = true;
			}
		}
	}

	private void UpdateShowResLabels()
	{
		if (myPolyPepManager.GetComponent<PolyPepManager>().allResLabelsOn)
		{

			string displayType = "";

			if (type != "XXX")
			{
				displayType = type;
			}

			label = (resid+1).ToString() + " " + displayType;
	
			//myLabel.SetActive(true);
			myLabel.GetComponent<TextMesh>().text = label;
			myLabel.transform.position = calpha_pf.transform.position;

			// 180 x rot to face camera
			myLabel.transform.localScale = new Vector3(-1, 1, 1);
			myLabel.transform.LookAt(Camera.main.transform);

			myLabelTMP.SetActive(true);
			myLabelTMP.GetComponent<TMPro.TextMeshPro>().text = label;
			myLabelTMP.transform.position = calpha_pf.transform.position;

			// 180 x rot to face camera
			myLabelTMP.transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
			myLabelTMP.transform.LookAt(Camera.main.transform);

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
			myLabelTMP.SetActive(false);
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