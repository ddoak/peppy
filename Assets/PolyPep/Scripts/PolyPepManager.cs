using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PolyPepManager : MonoBehaviour {


	public GameObject polyPepBuilder_pf;
	public List<PolyPepBuilder> allPolyPepBuilders = new List<PolyPepBuilder>();

	public SideChainBuilder sideChainBuilder;
	public ElectrostaticsManager electrostaticsManager;

	// relative atom radii
	// values set in Awake()
	public float radiusN;
	public float radiusC;
	public float radiusO;
	public float radiusH;
	public float radiusS;
	public float radiusR;
	public float radiusFreeze;
	public float radiusColliderGlobalScale;

	public Mesh atomMesh;
	public Mesh bondMesh;
	public Material testMaterial;
	public Material matC;
	public Material matN;
	public Material matH;
	public Material matO;
	public Material matR;
	public Material matS;
	public Material matBond;
	public Material matTrans;

	public List<Matrix4x4> myAllBondMatrices = new List<Matrix4x4>();

	public List<Matrix4x4> myAllCAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllNAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllOAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllHAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllRAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllSAtomMatrices = new List<Matrix4x4>();

	public Vector3 refMeshAtomScale = new Vector3(0.1f, 0.1f, 0.1f); // matches prefabs
	public Vector3 drawMeshAtomScale = new Vector3(0.1f, 0.1f, 0.1f); // matches prefabs
	//private Vector3 drawMeshAtomScale = new Vector3(5f, 5f, 5f); // if use maya sphere
	public Vector3 drawMeshBondScale = new Vector3(0.025f, 0.05f, 0.025f); // matches prefabs
	//private Vector3 drawMeshBondScale = new Vector3(1.25f, 5.5f, 1.25f); // eyeballed to match original for maya cylinder

	Shader shaderStandard;
	Shader shaderToonOutline;

	public bool doRenderDrawMesh = true;
	public bool shadowsOn = true;
	public bool renderAtoms = false;
	public bool renderBonds = false;
	public bool renderMat4x4 = true;

	public Light skylight;
	private Quaternion skylightTargetRot;

	public bool collidersOn = false;
	public float vdwScale = 1.0f;

	public bool dragHigh = false;
	public float dragStrength = 5.0f;
	public float jiggleStrength = 0.0f;

	public bool hbondsOn = false;
	public float hbondStrength = 100.0f;
	public float hbondScale = 1000.0f; // prev 500.0f multiplier between UI slider strength and value used in config joint

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
	public Slider sfxVolumeSliderUI;
	public Slider bgmVolumeSliderUI;
	public Slider uiAlphaSliderUI;

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


	//WIP
	public List<Image> uiBgImages = new List<Image>();
	//public Image UIControlsBgImage;



	public GameObject myPlayerController;

	public AudioManager myAudioManager;


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
		{
			radiusC = 1.7f;
			radiusN = 1.55f;
			radiusO = 1.52f;
			radiusH = 1.2f; 
			radiusS = 1.8f; 
			radiusR = 1.8f; 


			radiusColliderGlobalScale = 0.675f; // empirical magic number


			radiusFreeze = 70.0f; // cosmetic - keep freeze shell visible

			drawMeshAtomScale = refMeshAtomScale / radiusC; // normalised to C radius

		}


		{
			//rendering
			matC = Resources.Load("Materials/mBlack", typeof(Material)) as Material;
			matH = Resources.Load("Materials/mWhite", typeof(Material)) as Material;
			matO = Resources.Load("Materials/mRed", typeof(Material)) as Material;
			matN = Resources.Load("Materials/mBlue", typeof(Material)) as Material;
			matR = Resources.Load("Materials/mPurple", typeof(Material)) as Material;
			matS = Resources.Load("Materials/mYellow", typeof(Material)) as Material;
			matBond = Resources.Load("Materials/mGrey2", typeof(Material)) as Material;
			matTrans = Resources.Load("Materials/mTrans", typeof(Material)) as Material;

			shaderStandard = Shader.Find("Standard");
			shaderToonOutline = Shader.Find("Toon/Basic Outline");
		}


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

		sfxVolumeSliderUI = GameObject.Find("Slider_SfxVolume").GetComponent<Slider>();
		bgmVolumeSliderUI = GameObject.Find("Slider_BgmVolume").GetComponent<Slider>();
		uiAlphaSliderUI = GameObject.Find("Slider_UI_Alpha").GetComponent<Slider>();

		temp = GameObject.Find("SideChainBuilder");
		sideChainBuilder = temp.GetComponent<SideChainBuilder>();

		temp = GameObject.Find("ElectrostaticsManager");
		electrostaticsManager = temp.GetComponent<ElectrostaticsManager>();



		UI = GameObject.Find("UI");

		// get UI bg images 
		uiBgImages.Add(GameObject.Find("Panel bg Controls").GetComponent<Image>());
		uiBgImages.Add(GameObject.Find("Panel bg Sidechains").GetComponent<Image>());
		uiBgImages.Add(GameObject.Find("Panel bg Camera").GetComponent<Image>());
		uiBgImages.Add(GameObject.Find("Panel bg Info").GetComponent<Image>());
		uiBgImages.Add(GameObject.Find("Panel bg Main").GetComponent<Image>());

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
		//UIPanelControls.SetActive(false);


		snapshotCameraResetTransform = GameObject.Find("CameraResetPos").transform;

		myPlayerController = GameObject.Find("OVRPlayerController");
		if (!myPlayerController)
		{
			myPlayerController = GameObject.Find("PlayerNonVR");
		}

		mySnapshotCamera = GameObject.Find("SnapshotCamera_pf");

		myAudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

		skylightTargetRot = skylight.transform.rotation;
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

			sfxVolumeSliderUI.GetComponent<Slider>().value = 5;
			bgmVolumeSliderUI.GetComponent<Slider>().value = 0;

			uiAlphaSliderUI.GetComponent<Slider>().value = 50;

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

			//SetAllUIBgImageAlpha(0.1f);

			//UIControlsBgImage.color = Color.white;
			//var tempColor = UIControlsBgImage.color;
			//tempColor.a = 0.01f;
			//UIControlsBgImage.color = tempColor;

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
		UIPanelControls.SetActive(false);

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
			//Debug.Log(_slider);

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
		if (delta == 1)
		{
			myAudioManager.PlayLatchOn();
		}
		if (delta == -1)
		{
			myAudioManager.PlayLatchOff();
		}
		spawnLengthSliderUI.GetComponent<Slider>().value += delta;
	}

	public void SpawnPolypeptide(Transform spawnTransform)
	{
		myAudioManager.PlaySpawn();

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

			ppb_cs.perfTestMat = testMaterial;
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
			// fixes selection highlight scaling but expensive
			//_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateVDWScalesEndDragUI()
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{		
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateCollidersFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		myAudioManager.PlayOnOffSfx(value);
		collidersOn = value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetAllColliderIsTrigger(!collidersOn);
		}
	}

	public void UpdateFreezeFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);

		myAudioManager.PlayOnOffSfx(value);

		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateAllFreeze(value);
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateLightingFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		myAudioManager.PlayOnOffSfx(value);
		//lightDayOn = value;
		if (value == true)
		{
			skylightTargetRot = Quaternion.Euler(60, -30, 0);
		}
		else
		{
			skylightTargetRot = Quaternion.Euler(-5, -30, 0);
		}
	}

	public void UpdateRendertestFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		myAudioManager.PlayOnOffSfx(value);
		doRenderDrawMesh = value;
		UpdateRenderingMode();
	}

	private void UpdateRenderingMode()
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.EnableOriginalPrefabRenderers(!doRenderDrawMesh);
		}
		// H atoms might be off
		UpdateHAtomRenderers(showHydrogenAtoms);
	}



	public void UpdateShadowsFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		myAudioManager.PlayOnOffSfx(value);
		shadowsOn = value;
	}

	public void UpdateRenderAtomsFromUI(bool value)
	{
		myAudioManager.PlayOnOffSfx(value);
		renderAtoms = value;
	}

	public void UpdateRenderBondsFromUI(bool value)
	{
		myAudioManager.PlayOnOffSfx(value);
		renderBonds = value;
	}

	public void UpdateRenderMat4x4FromUI(bool value)
	{
		myAudioManager.PlayOnOffSfx(value);
		renderMat4x4 = value;
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
		myAudioManager.PlayOnOffSfx(value);
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
		myAudioManager.PlayOnOffSfx(value);
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
		myAudioManager.PlaySelectOff();
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
		myAudioManager.PlaySelectSfx(value);
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetGlobalSelect(value);
		}
	}

	public void SelectionInvertFromUI()
	{
		myAudioManager.PlaySelectInvert();
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

		if ((value > 0) && myAudioManager)
		{
			myAudioManager.PlaySetSecondary();
		}

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
		
		
		// fix for NullReference on Start() race?
		// ref should be set on Awake() above
		if (phiSliderUI)
		{
			phiSliderUI.value = phi;
		}
		if (psiSliderUI)
		{
			psiSliderUI.value = psi;
		}


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
		myAudioManager.PlayOnOffSfx(value);
	}

	public void UpdateAllResidueLabelsOnFromUI(bool value)
	{
		allResLabelsOn = value;
		myAudioManager.PlayOnOffSfx(value);
	}

	public void UpdateShowPeptidePlanesOnFromUI(bool value)
	{
		 showPeptidePlanes= value;
		myAudioManager.PlayOnOffSfx(value);
	}

	public void UpdateShowPhiPsiTrailOnFromUI(bool value)
	{
		showPhiPsiTrail = value;
		myAudioManager.PlayOnOffSfx(value);
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

			UpdateAllBackboneRenderTransformLists();
			UpdateAllSidechainRenderTransformLists();

			UpdateRenderingMode();

		}
	}

	public void UpdateAllBackboneRenderTransformLists()
	{
		//Update sidechain atom and bond transform lists
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateBackboneRenderTransformLists();
		}
	}

	public void UpdateAllSidechainRenderTransformLists()
	{
		//Update sidechain atom and bond transform lists
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateSideChainRenderTransformLists();
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
			myAudioManager.PlaySetSecondary();
			sideChainBuilder.MakeDisulphide(pp1, resid1, pp2, resid2);
			UpdateAllSidechainRenderTransformLists();
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
		myAudioManager.PlayOnOffSfx(value);
		// TODO - store value, bond rendering
		showHydrogenAtoms = value;
		UpdateHAtomRenderers(showHydrogenAtoms);
		//

	}

	private void UpdateHAtomRenderers(bool value)
	{
		if (!doRenderDrawMesh)
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

		// selection outlines
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.UpdateRenderModeAllBbu();
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
		myAudioManager.PlayOnOffSfx(value);
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
			UIPanelSideChains.transform.position = Vector3.Lerp(UIPanelSideChains.transform.position, (UI.transform.position + (UI.transform.forward * 0.01f) + (UI.transform.right * 1.0f)), ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	public void TogglePanel03FromUI(bool value)
	{
		//Debug.Log("Click from TogglePanel03FromUI: " + value);
		myAudioManager.PlayOnOffSfx(value);
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
			UIPanelCamera.transform.position = Vector3.Lerp(UIPanelCamera.transform.position, (UI.transform.position + (UI.transform.forward * 0.01f) + (UI.transform.right * -1.0f)), ((Time.deltaTime / 0.01f) * 0.05f));
		}
	}

	public void TogglePanelInfoFromUI(bool value)
	{
		UIPanelInfo.SetActive(value);
		myAudioManager.PlayOnOffSfx(value);
	}

	private void UpdatePanelInfoPos()
	{
		if (UIPanelInfo.activeSelf == true)
		{
			UIPanelInfo.transform.position = Vector3.Lerp(UIPanelInfo.transform.position, UIInfoActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
			UIPanelInfo.transform.localScale = Vector3.Lerp(UIPanelInfo.transform.localScale, UIInfoActiveTf.localScale, ((Time.deltaTime / 0.01f) * 0.15f));
		}
		if (UIPanelInfo.activeSelf == false)
		{
			UIPanelInfo.transform.position = Vector3.Lerp(UIPanelInfo.transform.position, UIInfoNotActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
			UIPanelInfo.transform.localScale = Vector3.Lerp(UIPanelInfo.transform.localScale, UIInfoNotActiveTf.localScale, ((Time.deltaTime / 0.01f) * 0.15f));
		}
	}

	public void TogglePanelControlsFromUI(bool value)
	{
		UIPanelControls.SetActive(value);
		myAudioManager.PlayOnOffSfx(value);
	}

	private void UpdatePanelControlsPos()
	{
		if (UIPanelControls.activeSelf == true)
		{
			UIPanelControls.transform.position = Vector3.Lerp(UIPanelControls.transform.position, UIControlsActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
			UIPanelControls.transform.localScale = Vector3.Lerp(UIPanelControls.transform.localScale, UIControlsActiveTf.localScale, ((Time.deltaTime / 0.01f) * 0.15f));
		}
		if (UIPanelControls.activeSelf == false)
		{
			UIPanelControls.transform.position = Vector3.Lerp(UIPanelControls.transform.position, UIControlsNotActiveTf.position + (UI.transform.forward * 0.01f), ((Time.deltaTime / 0.01f) * 0.05f));
			UIPanelControls.transform.localScale = Vector3.Lerp(UIPanelControls.transform.localScale, UIControlsNotActiveTf.localScale, ((Time.deltaTime / 0.01f) * 0.15f));
		}
	}

	public void UpdateUIAlphaFromUI(float value)
	{
		SetAllUIBgImageAlpha(value / 100f);
	}

	private void SetAllUIBgImageAlpha(float alpha)
	{
		foreach (Image _image in uiBgImages)
		{
			_image.color = Color.white;
			var tempColor = _image.color;
			tempColor.a = alpha;
			_image.color = tempColor;
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

		//switch (m_Scene.name)
		//{
		//	case "Scene_VR":
		//		SceneManager.LoadScene("Scene_nonVR");
		//		break;
		//	case "Scene_nonVR":
		//		SceneManager.LoadScene("Scene_VR");
		//		break;
		//}
		SceneManager.LoadScene("FrontEnd");
	}

	void UpdateLerpSkylight()
	{
		Quaternion newRot = Quaternion.Slerp(skylight.transform.rotation, skylightTargetRot, Time.deltaTime*2f);
		skylight.transform.rotation = newRot;
	}

	private void UpdateRenderAllAtoms()
	{
		if (renderAtoms && doRenderDrawMesh && !renderMat4x4)
		{			
			foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
			{
				// render ATOMS
				RenderMeshTransformList(atomMesh, matC, _ppb.myCAtomTransforms, drawMeshAtomScale * radiusC * vdwScale);
				RenderMeshTransformList(atomMesh, matC, _ppb.mySideChainCAtomTransforms, drawMeshAtomScale * radiusC * vdwScale);

				RenderMeshTransformList(atomMesh, matN, _ppb.myNAtomTransforms, drawMeshAtomScale * radiusN * vdwScale);
				RenderMeshTransformList(atomMesh, matN, _ppb.mySideChainNAtomTransforms, drawMeshAtomScale * radiusN * vdwScale);

				RenderMeshTransformList(atomMesh, matO, _ppb.myOAtomTransforms, drawMeshAtomScale * radiusO * vdwScale);
				RenderMeshTransformList(atomMesh, matO, _ppb.mySideChainOAtomTransforms, drawMeshAtomScale * radiusO * vdwScale);

				RenderMeshTransformList(atomMesh, matR, _ppb.myRAtomTransforms, drawMeshAtomScale * radiusR * vdwScale);

				RenderMeshTransformList(atomMesh, matS, _ppb.mySideChainSAtomTransforms, drawMeshAtomScale * radiusS * vdwScale);

				if (showHydrogenAtoms)
				{
					RenderMeshTransformList(atomMesh, matH, _ppb.myHAtomTransforms, drawMeshAtomScale * radiusH * vdwScale);
					RenderMeshTransformList(atomMesh, matH, _ppb.mySideChainHAtomTransforms, drawMeshAtomScale * radiusH * vdwScale);
				}
			}
		}
	}

	private void UpdateRenderAllBonds()
	{
		if (renderBonds && doRenderDrawMesh && !renderMat4x4)
		{
			foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
			{
				//matBond.shader = shaderStandard;
				RenderMeshTransformList(bondMesh, matBond, _ppb.myBondTransforms, drawMeshBondScale);
				RenderMeshTransformList(bondMesh, matBond, _ppb.mySideChainBondTransforms, drawMeshBondScale);
				if (showHydrogenAtoms)
				{
					RenderMeshTransformList(bondMesh, matBond, _ppb.myBondToHTransforms, drawMeshBondScale);
					RenderMeshTransformList(bondMesh, matBond, _ppb.mySideChainBondToHTransforms, drawMeshBondScale);
				}
			}
		}
	}

	private void RenderMeshTransformList(Mesh _mesh, Material _mat, List<Transform> _atomTransforms, Vector3 _scale)
	{
		
		foreach (Transform _tf in _atomTransforms)
		{
			//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
			Matrix4x4 _matrix = Matrix4x4.TRS(_tf.position, _tf.rotation, _scale);
			//Graphics.DrawMesh(_mesh, _matrix, _mat, 0, null);
			Graphics.DrawMesh(_mesh, _matrix, _mat, 0, null, 0, null, shadowsOn, shadowsOn);
		}
	}



	void DrawMeshInstancedTest()
	{
		// matrices stored in ppm
		// was not expecting this to be faster but...
		if (renderMat4x4)
		{
				myAllCAtomMatrices.Clear();
				myAllNAtomMatrices.Clear();
				myAllOAtomMatrices.Clear();
				myAllHAtomMatrices.Clear();
				myAllRAtomMatrices.Clear();
				myAllSAtomMatrices.Clear();
				myAllBondMatrices.Clear();

			foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
			{
				if (renderAtoms)
				{
					foreach (Transform _tf in _ppb.myCAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllCAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusC));
					}
					foreach (Transform _tf in _ppb.myNAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllNAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusN));
					}
					foreach (Transform _tf in _ppb.myOAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllOAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusO));
					}
					if (showHydrogenAtoms)
					{
						foreach (Transform _tf in _ppb.myHAtomTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							myAllHAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusH));
						}
					}

					foreach (Transform _tf in _ppb.myRAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllRAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusR));
					}

					foreach (Transform _tf in _ppb.mySideChainCAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllCAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusC));
					}
					foreach (Transform _tf in _ppb.mySideChainNAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllNAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusN));
					}
					foreach (Transform _tf in _ppb.mySideChainOAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllOAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusO));
					}
					foreach (Transform _tf in _ppb.mySideChainSAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllSAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusS));
					}

					if (showHydrogenAtoms)
					{
						foreach (Transform _tf in _ppb.mySideChainHAtomTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							myAllHAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusH));
						}
					}
				}

				//if (renderBonds)
				if ( (renderBonds && vdwScale < 1.9f) || (renderBonds && !renderAtoms) )
				{
					if (showHydrogenAtoms)
					{
						foreach (Transform _tf in _ppb.myBondToHTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
						}
						foreach (Transform _tf in _ppb.mySideChainBondToHTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
						}
					}
					foreach (Transform _tf in _ppb.myBondTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
					}
					foreach (Transform _tf in _ppb.mySideChainBondTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
					}
				}
			}

			if (renderAtoms)
			{
				DoDrawMeshInstanced(atomMesh, myAllCAtomMatrices.ToArray(), matC);
				DoDrawMeshInstanced(atomMesh, myAllNAtomMatrices.ToArray(), matN);
				DoDrawMeshInstanced(atomMesh, myAllOAtomMatrices.ToArray(), matO);
				DoDrawMeshInstanced(atomMesh, myAllHAtomMatrices.ToArray(), matH);
				DoDrawMeshInstanced(atomMesh, myAllRAtomMatrices.ToArray(), matR);
				DoDrawMeshInstanced(atomMesh, myAllSAtomMatrices.ToArray(), matS);
			}
			if (renderBonds)
			{
				DoDrawMeshInstanced(bondMesh, myAllBondMatrices.ToArray(), matBond);
			}	
		}	
	}

	void DrawMeshInstancedTest_ppb()
	{
		// matrices stored in ppbs
		// was not expecting this to be faster but...
		if (renderMat4x4)
		{

			foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
			{
				_ppb.myAllCAtomMatrices.Clear();
				_ppb.myAllNAtomMatrices.Clear();
				_ppb.myAllOAtomMatrices.Clear();
				_ppb.myAllHAtomMatrices.Clear();
				_ppb.myAllRAtomMatrices.Clear();
				_ppb.myAllSAtomMatrices.Clear();

				if (renderAtoms)
				{
					foreach (Transform _tf in _ppb.myCAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllCAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusC));
					}
					foreach (Transform _tf in _ppb.myNAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllNAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusN));
					}
					foreach (Transform _tf in _ppb.myOAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllOAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusO));
					}
					if (showHydrogenAtoms)
					{
						foreach (Transform _tf in _ppb.myHAtomTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							_ppb.myAllHAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusH));
						}
					}

					foreach (Transform _tf in _ppb.myRAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllRAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusR));
					}

					foreach (Transform _tf in _ppb.mySideChainCAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllCAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusC));
					}
					foreach (Transform _tf in _ppb.mySideChainNAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllNAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusN));
					}
					foreach (Transform _tf in _ppb.mySideChainOAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllOAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusO));
					}
					foreach (Transform _tf in _ppb.mySideChainSAtomTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllSAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusS));
					}

					if (showHydrogenAtoms)
					{
						foreach (Transform _tf in _ppb.mySideChainHAtomTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							_ppb.myAllHAtomMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshAtomScale * vdwScale * radiusH));
						}
					}
				}

				_ppb.myAllBondMatrices.Clear();

				if (renderBonds)
				{
					if (showHydrogenAtoms)
					{
						foreach (Transform _tf in _ppb.myBondToHTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							_ppb.myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
						}
						foreach (Transform _tf in _ppb.mySideChainBondToHTransforms)
						{
							//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
							_ppb.myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
						}
					}
					foreach (Transform _tf in _ppb.myBondTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
					}
					foreach (Transform _tf in _ppb.mySideChainBondTransforms)
					{
						//Graphics.DrawMesh(atomMesh, _tf.position, _tf.rotation, _mat, 0, null);
						_ppb.myAllBondMatrices.Add(Matrix4x4.TRS(_tf.position, _tf.rotation, drawMeshBondScale));
					}
				}

				DoDrawMeshInstanced(atomMesh, _ppb.myAllCAtomMatrices.ToArray(), matC);
				DoDrawMeshInstanced(atomMesh, _ppb.myAllNAtomMatrices.ToArray(), matN);
				DoDrawMeshInstanced(atomMesh, _ppb.myAllOAtomMatrices.ToArray(), matO);
				DoDrawMeshInstanced(atomMesh, _ppb.myAllHAtomMatrices.ToArray(), matH);
				DoDrawMeshInstanced(atomMesh, _ppb.myAllRAtomMatrices.ToArray(), matR);
				DoDrawMeshInstanced(atomMesh, _ppb.myAllSAtomMatrices.ToArray(), matS);

				DoDrawMeshInstanced(bondMesh, _ppb.myAllBondMatrices.ToArray(), matBond);
			}
		}
	}

	private void DoDrawMeshInstanced(Mesh _mesh, Matrix4x4[] _matrixArray, Material _mat)
	{
		if (_matrixArray.Length < 1024) // max array size for DrawMeshInstanced !
		{
			if (shadowsOn)
			{
				Graphics.DrawMeshInstanced(_mesh, 0, _mat, _matrixArray, _matrixArray.Length, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
			}
			else
			{
				Graphics.DrawMeshInstanced(_mesh, 0, _mat, _matrixArray, _matrixArray.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
			}
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

		UpdateLerpSkylight();

		if (Input.GetKey(KeyCode.Escape))
		{
			AppQuit();
		}
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SwitchLevel();
		}

		UpdateRenderAllAtoms();
		UpdateRenderAllBonds();
		DrawMeshInstancedTest();
	}
}
