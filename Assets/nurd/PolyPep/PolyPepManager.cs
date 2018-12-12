using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PolyPepManager : MonoBehaviour {


	public GameObject polyPepBuilder_pf;
	public List<PolyPepBuilder> allPolyPepBuilders = new List<PolyPepBuilder>();

	public SideChainBuilder sideChainBuilder;

	public bool collidersOn = false;
	public float vdwScale = 1.0f;

	public bool dragHigh = false;
	public float jiggleStrength = 0.0f;

	public bool hbondsOn = false;
	public float hbondStrength = 100.0f;
	public float hbondScale = 500.0f; // multiplier between UI slider strength and value used in config joint

	public int UIDefinedSecondaryStructure { get; set; }

	public float phiTarget = 0f;
	public float psiTarget = 0f;
	public float phiPsiDrive = 100.0f;

	public bool showDrivenBondsOn = true;
	public bool doCartoonBondRendering = true;

	public bool allResLabelsOn = false;
	public bool showPeptidePlanes = false;

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

	private int testCount = 0;

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

			temp = GameObject.Find("SideChainBuilder");
			sideChainBuilder = temp.GetComponent<SideChainBuilder>();

	}

	void Start()
	{

		{
			//UI
			// initialise phi psi slider values (hacky?)

			phiSliderUI.GetComponent<Slider>().value = 0;
			psiSliderUI.GetComponent<Slider>().value = 0;
			vdwSliderUI.GetComponent<Slider>().value = 10;
			hbondSliderUI.GetComponent<Slider>().value = 50;
			phiPsiDriveSliderUI.GetComponent<Slider>().value = 50;
			spawnLengthSliderUI.GetComponent<Slider>().value = 18; //10
			jiggleStrengthSliderUI.GetComponent<Slider>().value = 0;

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

		}

		// dev: test always spawn pp on startup
		SpawnPolypeptide(transform);

	}

	public void SpawnPolypeptide(Transform spawnTransform)
	{
		//if (!collidersOn)
		{
			int numResidues = (int)spawnLengthSliderUI.GetComponent<Slider>().value;
			//Debug.Log(spawnTransform.position);

			Vector3 offset = -spawnTransform.transform.right * (numResidues-1) * 0.2f;
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

	public void UpdateDragFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		dragHigh = value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			//_ppb.ActiveHbondSpringConstraints = hbondsOn;
			_ppb.UpdateAllDrag();
		}
	}


	public void UpdateHbondOnFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		hbondsOn = value;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			//_ppb.ActiveHbondSpringConstraints = hbondsOn;
			_ppb.UpdateHBondSprings();
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
	}

	public void UpdatePhiPsiDriveFromUI(float phiPsiDriveFromUI)
	{

		//Debug.Log("hello PhiPsi Drive from the manager! ---> " + phiPsiDrive);
		phiPsiDrive = phiPsiDriveFromUI;
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.drivePhiPsiMaxForce = phiPsiDrive;
			_ppb.drivePhiPsiPosSpring = phiPsiDrive;
			_ppb.UpdatePhiPsiDrives();
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

	public void SetSelectionDriveFromUI (bool value)
	{
		//Debug.Log("hello from the manager! ---> SetSelectionDriveOffFromUI");
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetPhiPsiDriveForSelection(value);
			_ppb.UpdateRenderModeAllBbu();
		}
	}

	public void UpdateDefinedSecondaryStructureFromUI()
	{
		float phi = phiTarget;
		float psi = psiTarget;
		switch (UIDefinedSecondaryStructure)
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

	public void UpdateAllResidueLabelsOnFromUI(bool value)
	{
		allResLabelsOn = value;
	}

	public void UpdateShowPeptidePlanesOnFromUI(bool value)
	{
		 showPeptidePlanes= value;
	}

	public void MutateSelectedResiduesFromUI()
	{
		Debug.Log("Mutate: " + UISelectedAminoAcid);

		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			foreach (GameObject _residueGo in _ppb.chainArr)
			{
				if (_residueGo.GetComponent<Residue>().residueSelected == true)
				{

					
					//_ppb.sideChainBuilder.DeleteSideChain(_ppb.gameObject, _residueGo.GetComponent<Residue>().resid);
					//if (UISelectedAminoAcid > 0)
					{
						string selectedAminoAcidStr = "-";
						switch (UISelectedAminoAcid)
						{
							case 0:
								// not defined
								selectedAminoAcidStr = "XXX"; 
								break;
									
							case 1:
								// GLY
								selectedAminoAcidStr = "XXX"; //  "GLY";
								break;

							case 2:
								// ALA
								selectedAminoAcidStr = "ALA";
								break;

							case 3:
								// VAL
								selectedAminoAcidStr = "VAL";
								break;

							case 4:
								//
								selectedAminoAcidStr = "LEU";
								break;

							case 5:
								//
								selectedAminoAcidStr = "ILE";
								break;

							case 6:
								//
								selectedAminoAcidStr = "MET";
								break;

							case 7:
								//
								selectedAminoAcidStr = "PHE";
								break;

							case 8:
								//
								selectedAminoAcidStr = "XXX"; // "TRP";
								break;

							case 9:
								//
								selectedAminoAcidStr = "XXX"; // "PRO";
								break;

							case 10:
								// GLY
								selectedAminoAcidStr = "SER";
								break;

							case 11:
								// ALA
								selectedAminoAcidStr = "THR";
								break;

							case 12:
								// VAL
								selectedAminoAcidStr = "CYS";
								break;

							case 13:
								//
								selectedAminoAcidStr = "TYR";
								break;

							case 14:
								//
								selectedAminoAcidStr = "ASN";
								break;

							case 15:
								//
								selectedAminoAcidStr = "GLN";
								break;

							case 16:
								//
								selectedAminoAcidStr = "ASP";
								break;

							case 17:
								//
								selectedAminoAcidStr = "GLU";
								break;

							case 18:
								//
								selectedAminoAcidStr = "LYS";
								break;

							case 19:
								//
								selectedAminoAcidStr = "ARG";
								break;

							case 20:
								//
								selectedAminoAcidStr = "XXX"; // "HIS";
								break;
						}

						if (_residueGo.GetComponent<Residue>().type != selectedAminoAcidStr)
						{
							//replace sidechain
							Debug.Log ("Click from UI: " + UISelectedAminoAcid + " " + selectedAminoAcidStr);
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
			}
			//push update of scale and colliders
			_ppb.ScaleVDW(vdwScale);
			_ppb.SetAllColliderIsTrigger(!collidersOn);
		}

	}

	public void UpdateAminoAcidSelFromUI()
	{
		Debug.Log("UI selected amino acid = " + UISelectedAminoAcid);
	}

	public void UpdateTestToggleFromUI(bool value)
	{
		Debug.Log("Click from UI: " + value);
	}

	public void ResetLevel()
	{
		Scene m_Scene = SceneManager.GetActiveScene();
		Debug.Log("Loading... " + m_Scene.name);
		SceneManager.LoadScene(m_Scene.name);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
