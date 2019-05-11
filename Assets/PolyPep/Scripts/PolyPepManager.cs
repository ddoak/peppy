using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PolyPepManager : MonoBehaviour {


	public GameObject polyPepBuilder_pf;
	public List<PolyPepBuilder> allPolyPepBuilders = new List<PolyPepBuilder>();

	public SideChainBuilder sideChainBuilder;
	public ElectrostaticsManager electrostaticsManager;

	public bool collidersOn = false;
	public float vdwScale = 1.0f;

	public bool dragHigh = false;
	public float dragStrength = 5.0f;
	public float jiggleStrength = 0.0f;

	public bool hbondsOn = false;
	public float hbondStrength = 100.0f;
	public float hbondScale = 500.0f; // multiplier between UI slider strength and value used in config joint

	public int UIDefinedSecondaryStructure { get; set; }

	public float phiTarget = 0f;
	public float psiTarget = 0f;
	public float phiPsiDriveTorqueFromUI = 100.0f;

	public bool showDrivenBondsOn = true;
	public bool doCartoonBondRendering = true;

	public bool allResLabelsOn = false;
	public bool showPeptidePlanes = false;
	public bool showHydrogenAtoms = true;
	public bool showPhiPsiTrail = false;

	public int UISelectedAminoAcid { get; set; }

	public float toonRenderScale = 0.002f;

	public Slider phiSliderUI;
	public Slider psiSliderUI;
	public Slider vdwSliderUI;
	public Slider scaleSliderUI;
	public Slider hbondSliderUI;
	public Slider phiPsiDriveSliderUI;
	public Slider spawnLengthSliderUI;
	public Slider jiggleStrengthSliderUI;
	public Slider dragStrengthSliderUI;
	public Slider electrostaticsStrengthSliderUI;

	private int testCount = 0;
	public GameObject snapshotCamera_pf;
	public GameObject mySnapshotCamera;
	private Transform snapshotCameraResetTransform;

	public GameObject UI;
	public GameObject UIPanelSideChains;
	public GameObject UIPanelCamera;
	public GameObject UIPanelInfo;
	private Transform UIInfoActiveTf;
	private Transform UIInfoNotActiveTf;
	public GameObject UIPanelControls;
	private Transform UIControlsActiveTf;
	private Transform UIControlsNotActiveTf;

	public GameObject myPlayerController;

	public List<Toggle> aaTogglesList = new List<Toggle> { };


	public List<string> residueTypeList = new List<string>
	{
		"XXX",
		"GLY",
		"ALA",
		"VAL",
		"LEU",
		"ILE",
		"MET",
		"PHE",
		"TRP",
		"PRO",
		"SER",
		"THR",
		"CYS",
		"TYR",
		"ASN",
		"GLN",
		"ASP",
		"GLU",
		"LYS",
		"ARG",
		"HIS",
	};

	void Awake()
	{
		GameObject temp = GameObject.Find("Slider_Phi");
		phiSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_Psi");
		psiSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_Vdw");
		vdwSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_HbondStrength");
		hbondSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_PhiPsiDrive");
		phiPsiDriveSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_SpawnLength");
		spawnLengthSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_JiggleStrength");
		jiggleStrengthSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_DragStrength");
		dragStrengthSliderUI = temp.GetComponent<Slider>();

		temp = GameObject.Find("Slider_ElectrostaticsStrength");
		electrostaticsStrengthSliderUI = temp.GetComponent<Slider>();


		temp = GameObject.Find("SideChainBuilder");
		sideChainBuilder = temp.GetComponent<SideChainBuilder>();

		temp = GameObject.Find("ElectrostaticsManager");
		electrostaticsManager = temp.GetComponent<ElectrostaticsManager>();

		UI = GameObject.Find("UI");

		UIPanelSideChains = GameObject.Find("UI_PanelSideChains");
		//UIPanelSideChains.SetActive(false);

		UIPanelCamera = GameObject.Find("UI_PanelCamera");
		//UIPanelCamera.SetActive(false);

		UIPanelInfo = GameObject.Find("UI_PanelInfo");
		UIInfoActiveTf = GameObject.Find("InfoActivePos").transform;
		UIInfoNotActiveTf = GameObject.Find("InfoNotActivePos").transform;
		UIPanelInfo.SetActive(false);

		UIPanelControls = GameObject.Find("UI_PanelControls");
		UIControlsActiveTf = GameObject.Find("ControlsActivePos").transform;
		UIControlsNotActiveTf = GameObject.Find("ControlsNotActivePos").transform;
		UIPanelControls.SetActive(false);


		snapshotCameraResetTransform = GameObject.Find("CameraResetPos").transform;

		myPlayerController = GameObject.Find("OVRPlayerController");
		if (!myPlayerController)
		{
			myPlayerController = GameObject.Find("PlayerNonVR");
		}

		mySnapshotCamera = GameObject.Find("SnapshotCamera_pf");

	}

	void Start()
	{

		{
			//UI
			// initialise phi psi slider values (hacky?)

			phiSliderUI.GetComponent<Slider>().value = 0;
			psiSliderUI.GetComponent<Slider>().value = 0;
			vdwSliderUI.GetComponent<Slider>().value = 10;
			hbondSliderUI.GetComponent<Slider>().value = 0; //50
			phiPsiDriveSliderUI.GetComponent<Slider>().value = 50;
			spawnLengthSliderUI.GetComponent<Slider>().value = 6; //10
			jiggleStrengthSliderUI.GetComponent<Slider>().value = 0;
			dragStrengthSliderUI.GetComponent<Slider>().value = 0;

			electrostaticsStrengthSliderUI.GetComponent<Slider>().value = 0;

			electrostaticsManager.electrostaticsStrength = electrostaticsStrengthSliderUI.GetComponent<Slider>().value;

			//temp = GameObject.Find("Slider_ResStart");

			//resStartSliderUI = temp.GetComponent<Slider>();
			//resStartSliderUI.maxValue = numResidues;
			//resStartSliderUI.value = 1;

			//temp = GameObject.Find("Slider_ResEnd");

			//Assert.IsNotNull(temp);

			//resEndSliderUI = temp.GetComponent<Slider>();
			//resEndSliderUI.maxValue = numResidues;
			//resEndSliderUI.value = 3; // numResidues; // initial value (+1)

			//temp = GameObject.Find("Slider_Scale");

			//scaleSliderUI = temp.GetComponent<Slider>();
			//scaleSliderUI.value = 10;



			//DoInitialUIPrep(UI);

			//UIPanelSideChains.SetActive(false);
			//UIPanelCamera.SetActive(false);


		}

		// dev: test always spawn pp on startup
		//SpawnPolypeptide(transform);

		//mySnapshotCamera = Instantiate(snapshotCamera_pf);
		mySnapshotCamera.SetActive(false);

		DoInitialUIPrep(UI);
		DoInitialUIPrep(mySnapshotCamera);

		// hide inactive UI 
		UIPanelSideChains.SetActive(false);
		UIPanelCamera.SetActive(false);


	}

	private void DoInitialUIPrep(GameObject thisUI)
	{
		System.Type highlightFixScriptType;
		string ScriptName = "HighlightFix";
		highlightFixScriptType = System.Type.GetType(ScriptName + ",Assembly-CSharp");

		//String ScriptName = "YourScriptNameWithoutExtension";
		////We need to fetch the Type
		//System.Type MyScriptType = System.Type.GetType(ScriptName + ",Assembly-CSharp");
		////Now that we have the Type we can use it to Add Component
		//gameObject.AddComponent(MyScriptType);


		Button[] buttons = thisUI.GetComponentsInChildren<Button>();

		//Debug.Log("buttons.length = " + buttons.Length);

		Toggle[] toggles = thisUI.GetComponentsInChildren<Toggle>();

		//Debug.Log("toggles.length = " + toggles.Length);

		int i = 0;
		foreach (Button _button in buttons)
		{
			//Debug.Log(_toggle.colors.normalColor);

			ColorBlock colors = _button.colors;
			colors.normalColor = new Color(0.8f, 0.8f, 0.3f);
			colors.highlightedColor = new Color(0.9f, 0.9f, 0.6f);
			colors.pressedColor = new Color(1f, 1f, 0.8f);
			colors.fadeDuration = 0.1f;

			_button.colors = colors;

			//var colors = GetComponent<Button>().colors;
			//colors.normalColor = Color.red;
			//GetComponent<Button>().colors = colors;

			//if (i%2 == 0)
				_button.gameObject.AddComponent(highlightFixScriptType);

			i++;
		}

		i = 0;
		foreach (Toggle _toggle in toggles)
		{

			ColorBlock colors = _toggle.colors;
			colors.normalColor = new Color(0.7f, 0.7f, 0.6f); //(0.7f, 0.7f, 0.6f);
			colors.highlightedColor = new Color(0.9f, 0.9f, 0.6f);
			colors.pressedColor = new Color(1f, 1f, 0.4f);
			colors.fadeDuration = 0.1f;

			_toggle.colors = colors;

			{
				_toggle.gameObject.AddComponent(highlightFixScriptType);
				_toggle.gameObject.GetComponent<HighlightFix>().myToggle = _toggle;
				_toggle.gameObject.GetComponent<HighlightFix>().normalColor = colors.normalColor;
			}

			if (_toggle.tag == "AAtoggle")
			{
				aaTogglesList.Add(_toggle);
			}
		}

		Slider[] sliders = thisUI.GetComponentsInChildren<Slider>();
		foreach (Slider _slider in sliders)
		{
			//Debug.Log(_toggle.colors.normalColor);

			ColorBlock colors = _slider.colors;
			colors.normalColor = new Color(0.7f, 0.7f, 0.6f); //(0.7f, 0.7f, 0.6f);
			colors.highlightedColor = new Color(0.9f, 0.9f, 0.6f);
			colors.pressedColor = new Color(1f, 1f, 0.4f);
			colors.fadeDuration = 0.1f;

			_slider.colors = colors;

			//var colors = GetComponent<Button>().colors;
			//colors.normalColor = Color.red;
			//GetComponent<Button>().colors = colors;

			_slider.gameObject.AddComponent(highlightFixScriptType);
		}
	}

	public void SpawnSliderIncrement(int delta)
	{
		spawnLengthSliderUI.GetComponent<Slider>().value += delta;
	}

	public void SpawnPolypeptide(Transform spawnTransform)
	{
		//if (!collidersOn)
		{
			int numResidues = (int)spawnLengthSliderUI.GetComponent<Slider>().value;
			//Debug.Log(spawnTransform.position);

			Vector3 offset = -spawnTransform.transform.right * (numResidues - 1) * 0.2f;
			// offset to try to keep new pp in sensible position
			// working solution - no scale, centre of mass / springs ...
			//spawnTransform.transform.position += offset; // NO! this is a reference not a copy!
			GameObject ppb = Instantiate(polyPepBuilder_pf, spawnTransform.transform.position + offset, Quaternion.identity);
			PolyPepBuilder ppb_cs = ppb.GetComponent<PolyPepBuilder>();
			ppb_cs.numResidues = numResidues;
			ppb_cs.myPolyPepManager = GetComponent<PolyPepManager>();
			ppb.name = "polyPep_" + allPolyPepBuilders.Count;
			allPolyPepBuilders.Add(ppb_cs);

			ppb_cs.sideChainBuilder = sideChainBuilder;
		}

	}

	void OnDrawGizmos()
	{
		//if (spawnTransform)
		//{
		//	Gizmos.color = Color.cyan;
		//	Gizmos.DrawWireSphere(spawnTransform.transform.position, 0.04f);
		//}
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(gameObject.transform.position, 0.04f);
	}


	public void UpdateVDWScalesFromUI(float scaleVDWx10)
	{

		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		vdwScale = scaleVDWx10 / 10.0f;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.ScaleVDW(vdwScale);
		}
	}

	public void UpdateCollidersFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		collidersOn = value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetAllColliderIsTrigger(!collidersOn);
		}
	}

	public void UpdateFreezeFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);

		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateAllFreeze(value);
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateDragFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		dragHigh = value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateAllDrag();
		}
	}

	public void UpdateDragStrengthFromUI(float dragStrengthFromUI)
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateAllDragStrength(dragStrengthFromUI);
		}
	}

	public void UpdateHbondOnFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		hbondsOn = value;
		PushHbondStrengthUpdate();
	}

	private void PushHbondStrengthUpdate()
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.NudgeHbondSprings();
		}
	}
	public void UpdateShowDrivenBondsOnFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		showDrivenBondsOn = value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateHbondStrengthFromUI(float hbondStrengthFromUI)
	{

		//Debug.Log("hello Hbond Strength from the manager! ---> " + hbondStrength);
		hbondStrength = hbondStrengthFromUI * hbondScale;
		//
		PushHbondStrengthUpdate();
	}

	public void UpdatePhiPsiDriveFromUI(float phiPsiTorqueSliderValueFromUI)
	{

		//Debug.Log("hello PhiPsi Drive from the manager! ---> " + phiPsiDrive);
		phiPsiDriveTorqueFromUI = phiPsiTorqueSliderValueFromUI;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			//_ppb.drivePhiPsiMaxForce = phiPsiDriveTorqueFromUI;
			//_ppb.drivePhiPsiPosSpring = phiPsiDriveTorqueFromUI;
			_ppb.UpdatePhiPsiDrives();
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void ZeroAllPhiPsiTorqueFromUI()
	{
		phiPsiDriveSliderUI.value = 0;
		phiPsiDriveTorqueFromUI = phiPsiDriveSliderUI.value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdatePhiPsiDrivesForceAll();
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void SelectAllFromUI(bool value)
	{
		//Debug.Log("Select All from the manager! ---> " +  value);
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetGlobalSelect(value);
		}
	}

	public void SelectionInvertFromUI()
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.InvertSelection();
		}
	}



	public void SetSelectionDriveFromUI (bool value)
	{
		//Debug.Log("hello from the manager! ---> SetSelectionDriveOffFromUI");
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetPhiPsiDriveForSelection(value);
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateElectroStaticsOnFromUI(bool value)
	{
		//if (electrostaticsManager.electrostaticsOn != value)
		//{
		//	electrostaticsManager.SwitchElectrostatics();
		//}
	}

	public void UpdateDefinedSecondaryStructureFromUI(int value)
	{
		float phi = phiTarget;
		float psi = psiTarget;
		//switch (UIDefinedSecondaryStructure)
		switch (value)
		{
			case 0:     
				// not defined
				phi = phiTarget;
				psi = psiTarget;
				break;

			case 1:     
				//alpha helix (right handed) (phi + ps ~ -105)
				phi = -57.0f;
				psi = -47.0f;
				break;

			case 2:     
				//310 helix (phi + psi ~ -75)
				phi = -49.0f;// -74.0f;
				psi = -26.0f;// -4.0f;
				break;

			case 3:     
				//anti beta sheet
				phi = -139.0f;
				psi = 135.0f;
				break;

			case 4:     
				//parallel beta sheet
				phi = -119.0f;
				psi = 113.0f;
				break;

			case 5:     
				//pi helix (phi + ps ~ -125)
				phi = -55.0f;
				psi = -70.0f;
				break;

			case 6:     
				//alpha helix (left handed)
				phi = 47.0f;
				psi = 57.0f;
				break;
		}

		phiTarget = phi;
		psiTarget = psi;
		
		phiSliderUI.value = phi;
		psiSliderUI.value = psi;

		UpdatePhiPsiForPolyPeptides();
	}

	public void UpdatePhiFromUI(float phi)
	{
		//Debug.Log("hello from the manager! ---> " + phi);
		phiTarget = phi;
		UpdatePhiPsiForPolyPeptides();
	}

	public void UpdatePsiFromUI(float psi)
	{
		//Debug.Log("hello from the manager! ---> " + psi);
		psiTarget = psi;
		UpdatePhiPsiForPolyPeptides();
	}

	private void UpdatePhiPsiForPolyPeptides()
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetPhiPsiTargetValuesForSelection(phiTarget, psiTarget);
		}
	}

	public void UpdateJiggleFromUI(float jiggleFromUI)
	{
		jiggleStrength = jiggleFromUI;
	}

	public void UpdateElectrostaticsStrengthFromUI(float electrostaticsStrengthFromUI)
	{
	    electrostaticsManager.electrostaticsStrength = electrostaticsStrengthFromUI;
	}

	public void UpdateShowElectrostaticsFromUI(bool value)
	{
		electrostaticsManager.showElectrostatics = value;
	}

	public void UpdateAllResidueLabelsOnFromUI(bool value)
	{
		allResLabelsOn = value;
	}

	public void UpdateShowPeptidePlanesOnFromUI(bool value)
	{
		 showPeptidePlanes= value;
	}

	public void UpdateShowPhiPsiTrailOnFromUI(bool value)
	{
		showPhiPsiTrail = value;
	}



	public void MutateSelectedResiduesFromUI()
	{
		//Debug.Log("Mutate: " + UISelectedAminoAcid);

		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			foreach (GameObject _residueGo in _ppb.chainArr)
			{
				if (_residueGo.GetComponent<Residue>().residueSelected == true)
				{
					string selectedAminoAcidStr = "-";

					if (UISelectedAminoAcid == -1)
					{
						// random
						int randomResidue = Random.Range(1, 20);
						//Debug.Log(randomResidue);
						selectedAminoAcidStr = residueTypeList[randomResidue];
					}
					else
					{
						selectedAminoAcidStr = residueTypeList[UISelectedAminoAcid];
					}

					//Debug.Log(UISelectedAminoAcid + " = mutate to " + selectedAminoAcidStr);

					if (_residueGo.GetComponent<Residue>().type != selectedAminoAcidStr)
					{
						// selected residue type is different => replace sidechain
						//Debug.Log ("Click from UI: " + UISelectedAminoAcid + " " + selectedAminoAcidStr);
						_ppb.sideChainBuilder.BuildSideChain(_ppb.gameObject, _residueGo.GetComponent<Residue>().resid, selectedAminoAcidStr);
					}
					else
					{
						//don't replace sidechain as the type is unchanged
					}

					if (selectedAminoAcidStr == "-")
					{
						// shouldn't happen!
					}

				}
			}
			//push update of scale and colliders
			_ppb.ScaleVDW(vdwScale);
			_ppb.SetAllColliderIsTrigger(!collidersOn);
			
			//force update of H Atom rendering
			UpdateShowHAtomsFromUI(showHydrogenAtoms);
		}

	}

	public void MakeDisulphideFromUI()
	{
		int numSelectedCYS = 0;
		GameObject pp1 = gameObject;
		GameObject pp2 = gameObject;
		int resid1 = 0, resid2 = 0;


		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			foreach (GameObject _residueGo in _ppb.chainArr)
			{
				if (_residueGo.GetComponent<Residue>().residueSelected == true)
				{
					bool notDisulphideBonded = !_residueGo.GetComponent<Residue>().disulphidePartnerResidue;
					if ((_residueGo.GetComponent<Residue>().type == "CYS") && notDisulphideBonded)
					{
						if (numSelectedCYS == 0)
						{
							// store 1st candidate
							pp1 = _ppb.gameObject;
							resid1 = _residueGo.GetComponent<Residue>().resid;
						}
						if (numSelectedCYS == 1)
						{
							// store 2nd candidate
							pp2 = _ppb.gameObject;
							resid2 = _residueGo.GetComponent<Residue>().resid;
						}
						numSelectedCYS++;
					}

				}
			}
		}
		//Debug.Log("Make Disulphide " + numSelectedCYS + " CYS residues selected");
		if (numSelectedCYS == 2)
		{
				sideChainBuilder.MakeDisulphide(pp1, resid1, pp2, resid2);
		}
	}


	public void UpdateAminoAcidSelFromUI(Toggle myToggle)
	{
		// bespoke implementation of toggle groups

		// workaround for recursive 'on value changed' call ;)
		int storedUISelectedAminoAcid = UISelectedAminoAcid;

		//Debug.Log("called by " + myToggle + "sel amino = " + UISelectedAminoAcid);
		if (myToggle.isOn)
		{
			int	cacheValue = UISelectedAminoAcid;
			foreach (Toggle _toggle in aaTogglesList)
			{
				if (_toggle != myToggle)
				{
					_toggle.isOn = false;
					_toggle.GetComponent<HighlightFix>().UpdateToggleLatch();
				}
			}
		}
		UISelectedAminoAcid = storedUISelectedAminoAcid;
	}

	public void UpdateShowHAtomsFromUI(bool value)
	{
		//Debug.Log("Click from UI: " + value);
		// TODO - store value, bond rendering
		showHydrogenAtoms = value;
		{
			GameObject[] gos;
			gos = GameObject.FindGameObjectsWithTag("H");
			foreach (GameObject _H in gos)
			{
				_H.GetComponent<Renderer>().enabled = value;
			}
			gos = GameObject.FindGameObjectsWithTag("bondToH");
			foreach (GameObject _bondToH in gos)
			{
				_bondToH.GetComponent<Renderer>().enabled = value;
			}
		}
	}
	public void TakeSnapshotFromUI()
	{
		mySnapshotCamera.GetComponent<SnapshotCamera>().CamCapture();
	}

	public void UpdateTestToggleFromUI(bool value)
	{
		//Debug.Log("Click from UI: " + value);
		if (value == true)
		{
			
		}

	}


	// TODO refactor duplicated code for UI panel activation / transitions

	public void TogglePanel02FromUI(bool value)
	{
		//Debug.Log("Click from TogglePanel02FromUI: " + value);
		UIPanelSideChains.SetActive(value);
		
	}

	private void UpdatePanel02Pos()
	{
		if (UIPanelSideChains.activeSelf == true)
		{
			UIPanelSideChains.transform.position = Vector3.Lerp(UIPanelSideChains.transform.position, (UI.transform.position + (UI.transform.forward * 0.01f) + (UI.transform.right * 1.36f)), ((Time.deltaTime / 0.01f) * 0.05f));
		}
		if (UIPanelSideChains.activeSelf == false)
		{
			UIPanelSideChains.transform.position = Vector3.Lerp(UIPanelSideChains.transform.position, UI.transform.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	public void TogglePanel03FromUI(bool value)
	{
		//Debug.Log("Click from TogglePanel03FromUI: " + value);
		UIPanelCamera.SetActive(value);

		mySnapshotCamera.transform.position = snapshotCameraResetTransform.position;
		mySnapshotCamera.transform.rotation = snapshotCameraResetTransform.rotation; 
		
		// TODO
		// camera reset rotation should probably not have any pitch (X rot)
		// but this doesn't work:
		//mySnapshotCamera.transform.rotation = Quaternion.Euler(0, UI.transform.rotation.y, UI.transform.rotation.z);

		mySnapshotCamera.SetActive(value);
	}

	private void UpdatePanel03Pos()
	{
		if (UIPanelCamera.activeSelf == true)
		{
			UIPanelCamera.transform.position = Vector3.Lerp(UIPanelCamera.transform.position, (UI.transform.position + (UI.transform.forward * 0.01f) + (UI.transform.right * -1.15f)), ((Time.deltaTime / 0.01f) * 0.05f));
		}
		if (UIPanelCamera.activeSelf == false)
		{
			UIPanelCamera.transform.position = Vector3.Lerp(UIPanelCamera.transform.position, UI.transform.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	public void TogglePanelInfoFromUI(bool value)
	{
		UIPanelInfo.SetActive(value);
	}

	private void UpdatePanelInfoPos()
	{
		if (UIPanelInfo.activeSelf == true)
		{
			UIPanelInfo.transform.position = Vector3.Lerp(UIPanelInfo.transform.position, UIInfoActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
		}
		if (UIPanelInfo.activeSelf == false)
		{
			UIPanelInfo.transform.position = Vector3.Lerp(UIPanelInfo.transform.position, UIInfoNotActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	public void TogglePanelControlsFromUI(bool value)
	{
		UIPanelControls.SetActive(value);
	}

	private void UpdatePanelControlsPos()
	{
		if (UIPanelControls.activeSelf == true)
		{
			UIPanelControls.transform.position = Vector3.Lerp(UIPanelControls.transform.position, UIControlsActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
		}
		if (UIPanelControls.activeSelf == false)
		{
			UIPanelControls.transform.position = Vector3.Lerp(UIPanelControls.transform.position, UIControlsNotActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	private void UpdateKeepGameObjectAccessible(GameObject go, float minY, float maxY)
	{
		// hacky repositioning lerp to keep things from getting lost...
		// TODO - bounding box for play area?
		if (go.transform.position.y < minY)
		{
			Vector3 target = new Vector3(go.transform.position.x, minY, go.transform.position.z);
			
			go.transform.position = Vector3.Lerp(go.transform.position, target, ((Time.deltaTime / 0.01f) * 0.05f));
		}

		if (go.transform.position.y > maxY)
		{
			Vector3 target = new Vector3(go.transform.position.x, maxY, go.transform.position.z);

			go.transform.position = Vector3.Lerp(go.transform.position, target, ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	private void UpdateKeepGameObjectCloseToPlayer(GameObject go, float maxDistance)
	{
		// hacky repositioning lerp to keep things from getting lost...
		// TODO - bounding box for play area?
		Vector3 offSet = go.transform.position - myPlayerController.transform.position;
		

		//Debug.Log(offSet.magnitude);
		if (offSet.magnitude > maxDistance)
		{
			//go.layer = LayerMask.NameToLayer("Default");
			Vector3 targetPos = myPlayerController.transform.position + (offSet.normalized * maxDistance);
			go.transform.position = Vector3.Lerp(go.transform.position, targetPos, ((Time.deltaTime / 0.01f) * 0.05f));

			//go.transform.position = Vector3.Lerp(go.transform.position, go.transform.position - offSet.normalized, ((Time.deltaTime / 0.01f) * 0.05f));
		}
		else
		{
			//go.layer = LayerMask.NameToLayer("Default");
		}
	}

	public void TestFromUI(string message)
	{
		Debug.Log(message);
	}

	public void ResetLevel()
	{
		Scene m_Scene = SceneManager.GetActiveScene();
		Debug.Log("Loading... " + m_Scene.name);
		SceneManager.LoadScene(m_Scene.name);
	}

	public void AppQuit()
	{
		Debug.Log("Application.Quit");
		Application.Quit();
	}

	public void SwitchLevel()
	{
		Scene m_Scene = SceneManager.GetActiveScene();
		Debug.Log("Currently in... " + m_Scene.name);

		switch (m_Scene.name)
		{
			case "Scene_VR":
				SceneManager.LoadScene("Scene_nonVR");
				break;
			case "Scene_nonVR":
				SceneManager.LoadScene("Scene_VR");
				break;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		UpdatePanel02Pos();
		UpdatePanel03Pos();
		UpdatePanelInfoPos();
		UpdatePanelControlsPos();
		UpdateKeepGameObjectAccessible(UI, 0.4f, 5.0f);
		UpdateKeepGameObjectCloseToPlayer(UI, 6.0f);
		UpdateKeepGameObjectAccessible(mySnapshotCamera, 0.2f, 5.0f);
		UpdateKeepGameObjectCloseToPlayer(mySnapshotCamera, 10.0f);
		if (Input.GetKey(KeyCode.Escape))
		{
			AppQuit();
		}
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SwitchLevel();
		}
	}
}
