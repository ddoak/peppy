using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PeptideData
{
	public string residPD;
	public float phiPD;
	public float psiPD;
}

public class PolyPepBuilder : MonoBehaviour {

	public GameObject amidePf;
	public GameObject calphaPf;
	public GameObject carbonylPf;
	public GameObject residuePf;
	public GameObject hBondPsPf;

	public PolyPepManager myPolyPepManager;

	public SideChainBuilder sideChainBuilder;

	public Transform buildTransform;

	public Material perfTestMat;

	// bond lengths used in backbone configurable joints
	// values are replicated in the prefabs (Carbonyl_pf, Amide_pf, Calpha_pf)
	float bondLengthPeptide = 1.33f;
	float bondLengthAmideCalpha = 1.46f;
	float bondLengthCalphaCarbonyl = 1.51f;

	// used in dynamic hbond configurable joints
	// highly empirical magic numbers
	float hBondModelInnerLength = 1.2f;
	float hBondModelOuterLength = 3.5f;

	public int secondaryStructure { get; set; } // = 0;

	public int numResidues = 0;

	public List<PeptideData> myPeptideDataList = new List<PeptideData>();

	public List<Transform> myAtomTransforms = new List<Transform>();

	public List<Transform> myCAtomTransforms = new List<Transform>();
	public List<Transform> myNAtomTransforms = new List<Transform>();
	public List<Transform> myHAtomTransforms = new List<Transform>();
	public List<Transform> myOAtomTransforms = new List<Transform>();
	public List<Transform> myRAtomTransforms = new List<Transform>();

	public List<Transform> mySideChainCAtomTransforms = new List<Transform>();
	public List<Transform> mySideChainNAtomTransforms = new List<Transform>();
	public List<Transform> mySideChainHAtomTransforms = new List<Transform>();
	public List<Transform> mySideChainOAtomTransforms = new List<Transform>();
	public List<Transform> mySideChainSAtomTransforms = new List<Transform>();

	public List<Transform> myBondTransforms = new List<Transform>();
	public List<Transform> myBondToHTransforms = new List<Transform>();

	public List<Transform> mySideChainBondTransforms = new List<Transform>();
	public List<Transform> mySideChainBondToHTransforms = new List<Transform>();

	public List<Matrix4x4> myAllAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllBondMatrices = new List<Matrix4x4>();

	public List<Matrix4x4> myAllCAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllNAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllOAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllHAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllRAtomMatrices = new List<Matrix4x4>();
	public List<Matrix4x4> myAllSAtomMatrices = new List<Matrix4x4>();

	public GameObject[] polyArr;
	private int polyLength;

	public GameObject[] chainArr;

	public GameObject[] hbondBackbonePsPf;

	public SpringJoint[] hbondBackboneSj_HO;
	public SpringJoint[] hbondBackboneSj_HC;
	public SpringJoint[] hbondBackboneSj_NO;

	public JointDrive[] chainPhiJointDrives;
	private JointDrive[] chainPsiJointDrives;

	public float hbondStrength = 0f; // updated by PolyPepManager

	public int drivePhiPsiPosDamper = 1;
	public int drivePhiPsiPosDamperPassive = 0;

	private Slider hbondSliderUI;

	private Slider resStartSliderUI;
	private Slider resEndSliderUI;

	private int residSelectStart;
	private int residSelectEnd;

	private int residSelectStartLast;
	private int residSelectEndLast;


	private bool disablePhiPsiUIInput = false;

	Shader shaderStandard;
	Shader shaderToonOutline;


	private void Awake()
	{
		buildTransform = transform;
	}

	// Use this for initialization
	void Start()

	{
		// read peptide data from file
		bool readExternalPeptideData = false; 
		string filename = "Assets/PolyPep/Data/1l2y_phi_psi.txt";

		if (readExternalPeptideData)
		{
			// reads data into and overwrites numResidues
			Debug.Log("LOAD FILE = " + LoadPhiPsiData(filename));
			//foreach ( PeptideData _peptideData in peptideDataRead)
			//{
			//	Debug.Log(_peptideData.residPD + " " + _peptideData.phiPD + " " + _peptideData.psiPD);
			//}
		}
		

		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");

		buildPolypeptideChain();



		if (false)
		{
		//oxytocin
		sideChainBuilder.BuildSideChain(gameObject, 0, "CYS");
		sideChainBuilder.BuildSideChain(gameObject, 1, "TYR");
		sideChainBuilder.BuildSideChain(gameObject, 2, "ILE");
		sideChainBuilder.BuildSideChain(gameObject, 3, "GLN");
		sideChainBuilder.BuildSideChain(gameObject, 4, "ASN");
		sideChainBuilder.BuildSideChain(gameObject, 5, "CYS");
		sideChainBuilder.BuildSideChain(gameObject, 6, "PRO");
		sideChainBuilder.BuildSideChain(gameObject, 7, "LEU");
		sideChainBuilder.BuildSideChain(gameObject, 8, "GLY");

		sideChainBuilder.MakeDisulphide(gameObject, 0, gameObject, 5);
		}

		// test: add arbitrary distance constraints
		//AddDistanceConstraint(polyArr[2], polyArr[12], 0.6f, 20);
		//AddDistanceConstraint(polyArr[15], polyArr[30], 0.8f, 20);
		//AddDistanceConstraint(polyArr[0], polyArr[36], 0.8f, 20);

		// placeholder: should be created and updated on tick
		//InvokeRepeating("UpdateDistanceConstraintGfx", 0, 0.05f);

		if (readExternalPeptideData)
		{
			// push sequence and phi psi data to peptide
			for (int i = 0; i < myPeptideDataList.Count; i++)
			{
				PeptideData _pd = myPeptideDataList[i];
				//Debug.Log(i + " " + _pd.residPD + " " + _pd.phiPD + " " + _pd.psiPD);
				sideChainBuilder.BuildSideChain(gameObject, i, _pd.residPD);
				SetPhiPsiTargetValuesForResidue(i, _pd.phiPD, _pd.psiPD);
				chainArr[i].GetComponent<Residue>().drivePhiPsiOn = true;
			}
			UpdatePhiPsiDrives();
		}

		secondaryStructure = 0;

	}

	void buildPolypeptideChain()
	{
		polyLength = numResidues * 3;
		polyArr = new GameObject[polyLength];

		chainArr = new GameObject[numResidues];

		hbondBackbonePsPf = new GameObject[numResidues];
		hbondBackboneSj_HO = new SpringJoint[numResidues];
		hbondBackboneSj_HC = new SpringJoint[numResidues];
		hbondBackboneSj_NO = new SpringJoint[numResidues];

		{
			// init
			// phi psi joint drives stored in array so that they can be dynamically referenced
			//
			chainPhiJointDrives = new JointDrive[numResidues];
			chainPsiJointDrives = new JointDrive[numResidues];

			for (int i = 0; i < numResidues; i++)
			{
				chainPhiJointDrives[i].maximumForce = 0.0f;
				chainPhiJointDrives[i].positionDamper = 0;
				chainPhiJointDrives[i].positionSpring = 0.0f;

				chainPsiJointDrives[i].maximumForce = 0.0f;
				chainPsiJointDrives[i].positionDamper = 0;
				chainPsiJointDrives[i].positionSpring = 0.0f;
			}
		}

		// periodic offsets for backbone unit polymer
		float buildOffset = 0.2f; // empirical - enough to avoid collider overlap
		var offsetPositionUnit = (buildOffset * buildTransform.right);

		Transform lastUnitTransform = buildTransform;

		for (int i = 0; i < polyLength; i++)
		{
			
			if (i > 0)
			{
				lastUnitTransform = polyArr[i - 1].transform;
			}


			int id = i % 6;
			//Debug.Log("polyArr" + i + " " + id);

			//
			// id ==  0     1     2     3     4     5
			//
			//              R           H           O
			//        N--   C--   C--   N--   C--   C--
			//        H           O           R
			//
			// prefab backbone bonds are aligned to Z axis / buildTransform.right , Y rotations of prefabs create correct backbone bond angles
			//
			// prefabs for 2,3 and 4 positions are flipped 180 X to make alternating extended chain

			switch (id)
			{
				case 0:
					AddResidueToChain(i / 3);
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + offsetPositionUnit), transform.rotation * Quaternion.Euler(0, 0, 0), chainArr[i/3].transform);
					break;
				case 1:
					// Yrot = +69
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + offsetPositionUnit), transform.rotation * Quaternion.Euler(0, 69, 0), chainArr[i / 3].transform);
					break;
				case 2:
					// Yrot = +69 -64 = 5
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + offsetPositionUnit), transform.rotation * Quaternion.Euler(180, 5, 0), chainArr[i / 3].transform);
					break;
				case 3:
					AddResidueToChain(i / 3);
					// Yrot = +69 -64 +58 = 63
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + offsetPositionUnit), transform.rotation * Quaternion.Euler(180, 63, 0), chainArr[i / 3].transform);	
					break;
				case 4:
					// Yrot = +69 -64 +58 -69 = -6
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + offsetPositionUnit), transform.rotation * Quaternion.Euler(180, -6, 0), chainArr[i / 3].transform);
					break;
				case 5:
					// Yrot = +69 -64 +58 -69 +64 = 58
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + offsetPositionUnit), transform.rotation * Quaternion.Euler(0, 58, 0), chainArr[i / 3].transform);
					break;

			}

			polyArr[i].name = ((i / 3)).ToString() + "_" + polyArr[i].name;
			polyArr[i].GetComponent<BackboneUnit>().residue = i / 3;

			if (i > 0)
			{
				AddBackboneTopologyConstraint(i);
			}
	
			SetRbDrag(polyArr[i]);

			// BackboneUnit script handles setup of UI parameters (collisions / vdw scale etc.)
		}

		//SetAllColliderIsTrigger (true);

		//for (int i = 0; i < polyLength; i++)
		//{
		//	var rigidBodies = polyArr[i].GetComponentsInChildren<Rigidbody>();
		//	foreach (var rb in rigidBodies)
		//	{
		//		rb.mass = 1;
		//		rb.drag = 5;
		//		rb.angularDrag = 5;
		//	}
		//}


		// assign references in chainArr
		for (int resid = 0; resid < numResidues; resid ++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			residue.amide_pf = polyArr[resid * 3];
			residue.calpha_pf = polyArr[(resid * 3) + 1];
			residue.carbonyl_pf = polyArr[(resid * 3) + 2];

			residue.type = "XXX";
			//electrostatics - WIP - with goal to model hbonds
			sideChainBuilder.AddBackboneElectrostatics(residue);
		}

		InitBackboneHbondConstraints();

		//ReCentrePolyPep();

		UpdateBackboneRenderTransformLists();
		EnableOriginalPrefabRenderers(!myPolyPepManager.doRenderDrawMesh);
	}

	public void EnableOriginalPrefabRenderers(bool value)
	{
		// turn off shadows / renderer
		// makes surprisingly little difference

		Renderer[] allChildren = GetComponentsInChildren<Renderer>();
		foreach (Renderer child in allChildren)
		{
			Renderer myRenderer = child.GetComponent<Renderer>();
			myRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			myRenderer.receiveShadows = false;
			// performance ?
			myRenderer.allowOcclusionWhenDynamic = false;

			// excluding meshes by name
			//if (myRenderer.transform.name != "Freeze" && myRenderer.transform.name != "PlotCube"
			//		&& myRenderer.transform.name != "hb_backbone_ps" && myRenderer.transform.name != "chargedParticle_ps(Clone)"
			//		&& myRenderer.transform.name != "PeptidePlane")
			//{
			//	myRenderer.enabled = value;
			//	Debug.Log(myRenderer.transform.name);
			//	Debug.Log(myRenderer.transform.tag);
			//}

			switch (myRenderer.transform.tag)
			{
				case "N":
				case "C":
				case "O":
				case "H":
				case "S":
				case "bondToH":
				case "bond":
					myRenderer.enabled = value;
					break;
				case "R":
					// messy (to accomodate switchable render modes)
					{
						if (value && !myPolyPepManager.doRenderDrawMesh && myRenderer.transform.parent.parent.parent.GetComponent<Residue>().type == "XXX")
						{
							myRenderer.enabled = true;
						}
						else
						{
							myRenderer.enabled = false;
						}
					}

					break;
				default:
					break;

			}
		}
			UpdateRenderModeAllBbu();
	}

	public void UpdateBackboneRenderTransformLists()
	{
		myRAtomTransforms.Clear();
		myCAtomTransforms.Clear();
		myNAtomTransforms.Clear();
		myOAtomTransforms.Clear();
		myHAtomTransforms.Clear();
		myBondTransforms.Clear();
		myBondToHTransforms.Clear();


		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();

			myNAtomTransforms.Add(GetAmideForResidue(resid).transform.Find("N_amide"));

			myCAtomTransforms.Add(GetCalphaForResidue(resid).transform.Find("C_alpha"));
			myHAtomTransforms.Add(GetCalphaForResidue(resid).transform.Find("tf_H/H"));

			myCAtomTransforms.Add(GetCarbonylForResidue(resid).transform.Find("C_carbonyl"));
			myOAtomTransforms.Add(GetCarbonylForResidue(resid).transform.Find("tf_O/O_carbonyl"));

			myBondTransforms.Add(GetAmideForResidue(resid).transform.Find("tf_bond_N_CA/bond_N_CA"));
			myBondTransforms.Add(GetCalphaForResidue(resid).transform.Find("tf_bond_CA_CO/bond_CA_CO"));
			myBondToHTransforms.Add(GetCalphaForResidue(resid).transform.Find("tf_H/tf_bond_CA_H/bond_CA_H"));

			myBondTransforms.Add(GetCarbonylForResidue(resid).transform.Find("tf_bond_C_N/bond_CO_N"));
			myBondTransforms.Add(GetCarbonylForResidue(resid).transform.Find("tf_O/tf_bond_C_O/bond_C_O"));

			if (residue.type == "XXX")
			{
				myRAtomTransforms.Add(GetCalphaForResidue(resid).transform.Find("tf_sidechain/R_sidechain"));
			}

			if (residue.type != "PRO")
			{
				myHAtomTransforms.Add(GetAmideForResidue(resid).transform.Find("tf_H/H_amide"));
				myBondToHTransforms.Add(GetAmideForResidue(resid).transform.Find("tf_H/tf_bond_N_H/bond_N_H"));

			}

			if (residue.type != "GLY" && residue.type != "GLY")
			{
				myBondTransforms.Add(GetCalphaForResidue(resid).transform.Find("tf_sidechain/tf_bond_CA_R/bond_CA_R"));
			}
		}

	}

	public void UpdateSideChainRenderTransformLists()
	{
		mySideChainCAtomTransforms.Clear();
		mySideChainNAtomTransforms.Clear();
		mySideChainHAtomTransforms.Clear();
		mySideChainOAtomTransforms.Clear();
		mySideChainSAtomTransforms.Clear();

		mySideChainBondTransforms.Clear();
		mySideChainBondToHTransforms.Clear();

		foreach (GameObject _residueGo in chainArr)
		{
			foreach (GameObject _sideChainAtomGo in _residueGo.GetComponent<Residue>().sideChainList)
			{
				foreach (Transform _child in _sideChainAtomGo.transform)
				{
					//Debug.Log(_child.name + " " + _child.tag);
					//if (_child.name.Substring(0,6) == "tf_bond")
					// can just use string length ;)
					if (_child.name.Length > 3)
					{
						foreach (Transform _bond in _child.GetComponentsInChildren<Transform>())
						{
							switch (_bond.tag)
							{
								case "bond":
									mySideChainBondTransforms.Add(_bond);
									break;
								case "bondToH":
									mySideChainBondToHTransforms.Add(_bond);
									break;
								default:
									break;
							}
						}
					}
					else
					{
						switch (_child.tag)
						{
							case "C":
								mySideChainCAtomTransforms.Add(_child);
								break;
							case "N":
								mySideChainNAtomTransforms.Add(_child);
								break;
							case "H":
								mySideChainHAtomTransforms.Add(_child);
								break;
							case "O":
								mySideChainOAtomTransforms.Add(_child);
								break;
							case "S":
								mySideChainSAtomTransforms.Add(_child);
								break;
							default:
								break;
						}

					}
				}
			}
		}
	}

	void AddResidueToChain(int index)
	{
		chainArr[index] = Instantiate(residuePf, transform);
		chainArr[index].name = "Residue_" + (index).ToString();
		chainArr[index].GetComponent<Residue>().resid = index;
		chainArr[index].GetComponent<Residue>().myPolyPepBuilder = gameObject.GetComponent<PolyPepBuilder>();
		chainArr[index].GetComponent<Residue>().myPolyPepManager = myPolyPepManager;
	}

	public void ScaleVDW(float scale)
	{
		{
			float scaleVDW = scale;
			//// relative atom radii
			//float radiusN = 1.0f * 1.55f / 1.7f;
			//float radiusC = 1.0f;
			//float radiusO = 1.0f * 1.52f / 1.7f;
			//float radiusH = 1.0f * 1.2f / 1.7f; //0.75f;
			//float radiusS = 1.0f * 1.8f / 1.7f; // 1.1f;
			//float radiusR = 1.1f;
			//float radiusFreeze = 70.0f;

			Transform[] allChildren = GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren)
			{
				//float atomDisplayScale = 1.0f;
				switch (child.tag)
				{
					case "N":
						//atomDisplayScale = radiusN * scaleVDW;
						ScaleAtom(child, scaleVDW, myPolyPepManager.radiusN);
						break;
					case "C":
						//atomDisplayScale = radiusC * scaleVDW;
						ScaleAtom(child, scaleVDW, myPolyPepManager.radiusC);
						break;
					case "O":
						//atomDisplayScale = radiusO * scaleVDW;
						ScaleAtom(child, scaleVDW, myPolyPepManager.radiusO);
						break;
					case "H":
						//atomDisplayScale = radiusH * scaleVDW;
						ScaleAtom(child, scaleVDW, myPolyPepManager.radiusH);
						break;
					case "R":
						//atomDisplayScale = radiusR * scaleVDW;
						ScaleAtom(child, scaleVDW, myPolyPepManager.radiusR);
						break;
					case "S":
						//atomDisplayScale = radiusS * scaleVDW;
						ScaleAtom(child, scaleVDW, myPolyPepManager.radiusS);
						break;
					case "freeze":
						//atomDisplayScale = radiusS * scaleVDW;
						ScaleFreeze(child, scaleVDW, myPolyPepManager.radiusFreeze);
						break;
				}

			}
		}
	}

	private void ScaleAtom(Transform myAtom, float scaleVDW, float relativeRadiusAtomType)
	{
		// CPK / VDW slider changes rendering
		float atomDisplayScale = relativeRadiusAtomType * scaleVDW;
		myAtom.transform.localScale = new Vector3(atomDisplayScale, atomDisplayScale, atomDisplayScale);
		// physics collider should be constant radius and independent of rendering scale
		// BUT in transform hierarchy the SphereCollider inherits the transform.localscale
		// SO apply inverse scaling to SphereCollider to compensate
		myAtom.GetComponent<SphereCollider>().radius = 1.2f * relativeRadiusAtomType / scaleVDW; 
		// 1.2f is magic number

	}

	private void ScaleFreeze(Transform myAtom, float scaleVDW, float relativeRadiusAtomType)
	{
		// CPK / VDW slider changes rendering
		// min freeze radius clamped at empirical value (looks better)
		float atomDisplayScale = Mathf.Max(50.0f, relativeRadiusAtomType * scaleVDW);
		myAtom.transform.localScale = new Vector3(atomDisplayScale, atomDisplayScale, atomDisplayScale);
	}

	public void UpdateAllDrag()
	{
		for (int resid = 0; resid < numResidues; resid++)
		{
			SetRbDrag(GetAmideForResidue(resid));
			SetRbDrag(GetCalphaForResidue(resid));
			SetRbDrag(GetCarbonylForResidue(resid));

			//Debug.Log(chainArr[resid].GetComponent<Residue>().sidechain);
			foreach (GameObject _sidechainGO in chainArr[resid].GetComponent<Residue>().sideChainList)
			{
				SetRbDrag(_sidechainGO);
			}
		}
	}

	public void UpdateAllFreeze(bool freeze)
	{
		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			if (residue.residueSelected)
			{
				SetRbDragFreeze(GetAmideForResidue(resid), freeze);
				SetRbDragFreeze(GetCalphaForResidue(resid), freeze);
				SetRbDragFreeze(GetCarbonylForResidue(resid), freeze);

				//Debug.Log(chainArr[resid].GetComponent<Residue>().sidechain);
				foreach (GameObject _sidechainGO in chainArr[resid].GetComponent<Residue>().sideChainList)
				{
					SetRbDragFreeze(_sidechainGO, freeze);
				}

				residue.residueFrozen = freeze;
			}
		}
	}


	public void UpdateAllDragStrength(float dragStrength)
	{
		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			if (!residue.residueFrozen)
			{
				SetRbDragStrength(GetAmideForResidue(resid), dragStrength);
				SetRbDragStrength(GetCalphaForResidue(resid), dragStrength);
				SetRbDragStrength(GetCarbonylForResidue(resid), dragStrength);

				//Debug.Log(chainArr[resid].GetComponent<Residue>().sidechain);
				foreach (GameObject _sidechainGO in chainArr[resid].GetComponent<Residue>().sideChainList)
				{
					SetRbDragStrength(_sidechainGO, dragStrength);
				}
			}
		}
	}

	void SetRbDrag(GameObject go)
	{
		if (myPolyPepManager.dragHigh)
		{
			go.GetComponent<Rigidbody>().mass = 1;
			go.GetComponent<Rigidbody>().drag = 25;
			go.GetComponent<Rigidbody>().angularDrag = 20;
		}
		else
		{
		// empirical values which seem to behave well
		go.GetComponent<Rigidbody>().mass = 1;
		go.GetComponent<Rigidbody>().drag = 5;
		go.GetComponent<Rigidbody>().angularDrag = 5;
		}
	}

	void SetRbDragStrength(GameObject go, float dragStrength)
	{
	
		// use lerp to map to meaningful range
		float value = Mathf.Lerp(5, 25, dragStrength / 100);

		go.GetComponent<Rigidbody>().mass = 1;
		go.GetComponent<Rigidbody>().drag = value;
		go.GetComponent<Rigidbody>().angularDrag = value;
	}

	void SetRbDragFreeze(GameObject go, bool freeze)
	{

		// use lerp to map to meaningful range
		//float value = Mathf.Lerp(5, 25, dragStrength / 100);
		if (freeze)
		{
			go.GetComponent<Rigidbody>().mass = Mathf.Infinity;
			go.GetComponent<Rigidbody>().drag = Mathf.Infinity;
			go.GetComponent<Rigidbody>().angularDrag = Mathf.Infinity;
		}
		else
		{
			go.GetComponent<Rigidbody>().mass = 1;
			go.GetComponent<Rigidbody>().drag = 5;
			go.GetComponent<Rigidbody>().angularDrag = 5;
		}

		UpdateMeshRendererFreeze(go, freeze);
	}

	void UpdateMeshRendererFreeze(GameObject go, bool freeze)
	{
		Transform _tf = go.transform.Find("Freeze");
		if (_tf)
		{
			_tf.GetComponent<MeshRenderer>().enabled = freeze;
		}
	}

public void SetAllColliderIsTrigger(bool value)
	{
		//for (int i = 0; i < polyLength; i++)
		//{
		//	var colliders = polyArr[i].GetComponentsInChildren<Collider>();
		//	foreach (var col in colliders)
		//	{
		//		col.isTrigger = value;
		//	}
		//}

		//if (value == true)
		//{
		//	Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Atom"), LayerMask.NameToLayer("Atom"), true);
		//}
		//else
		//{
		//	Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Atom"), LayerMask.NameToLayer("Atom"), false);
		//}


		Collider[] allChildren = GetComponentsInChildren<Collider>();
		foreach (Collider childCollider in allChildren)
		{
			//childCollider.isTrigger = value;

			//sidechain colliders  need to be explicitly set - bug
			childCollider.isTrigger = false;

			if (childCollider.gameObject.layer != LayerMask.NameToLayer("Hbond"))
			{
				if (value == true)
				{
					childCollider.gameObject.layer = LayerMask.NameToLayer("Water");
				}
				else
				{
					childCollider.gameObject.layer = LayerMask.NameToLayer("Atom");

				}
			}

			if (childCollider.name.Contains("bond"))
			{
				// Debug.Log(childCollider.name);
				// deletes ALL bond colliders!
				// attempt at culling unused components
				Destroy(childCollider);
			}
			
		}
	}

	void AddBackboneTopologyConstraint(int index)
	{
		// 
		// adds a configurable joint between backbone prefabs
		//

		Assert.IsTrue((index > 0), "Assertion failed");

		GameObject go1 = polyArr[index - 1];
		GameObject go2 = polyArr[index];


		//float bondLengthPeptide = 1.33f;
		//float bondLengthAmideCalpha = 1.46f;
		//float bondLengthCalphaCarbonyl = 1.51f;

		ConfigurableJoint cj = go1.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
		cj.connectedBody = go2.GetComponent<Rigidbody>();
		if (go1.tag == "amide")
		{
			cj.anchor = new Vector3(bondLengthAmideCalpha, 0f, 0f);
		}
		else if (go1.tag == "calpha")
		{
			cj.anchor = new Vector3(bondLengthCalphaCarbonyl, 0f, 0f);
		}
		else if (go1.tag == "carbonyl")
		{
			cj.anchor = new Vector3(bondLengthPeptide, 0f, 0f);
		}
		cj.autoConfigureConnectedAnchor = false;
		cj.connectedAnchor = new Vector3(0f, 0f, 0f);
		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		cj.zMotion = ConfigurableJointMotion.Locked;
		if (go1.tag == "amide")
		{
			cj.angularXMotion = ConfigurableJointMotion.Free;
			//cj.angularXDrive = new JointDrive
			//{
			//	positionSpring = 100f,//10000.0f, // 40.0f,//20.0f
			//	positionDamper = 1,
			//	maximumForce = 100f,//10000.0f, //40.0f // 10.0f
			//};
			cj.angularXDrive = chainPhiJointDrives[index / 3];
			cj.targetRotation = Quaternion.Euler(180 + 0, 0, 0); // default to 0 phi
		}
		else if (go1.tag == "calpha")
		{
			cj.angularXMotion = ConfigurableJointMotion.Free;
			//cj.angularXDrive = new JointDrive
			//{
			//	positionSpring = 100f,//10000.0f, // 40.0f,//20.0f
			//	positionDamper = 1,
			//	maximumForce = 100f,//10000.0f, //40.0f // 10.0f
			//};
			cj.angularXDrive = chainPsiJointDrives[index / 3];
			cj.targetRotation = Quaternion.Euler(180 + 0, 0, 0); // default to 0 psi
		}
		else if (go1.tag == "carbonyl")
		{
			//peptide bond
			cj.angularXMotion = ConfigurableJointMotion.Locked;
		}
		cj.angularYMotion = ConfigurableJointMotion.Locked;
		cj.angularZMotion = ConfigurableJointMotion.Locked;
	}

	void AddDistanceConstraint(GameObject sourceGO, GameObject targetGO, float distance, int springStrength)
	{
		SpringJoint sjDist = sourceGO.AddComponent(typeof(SpringJoint)) as SpringJoint;
		sjDist.connectedBody = targetGO.GetComponent<Rigidbody>();
		sjDist.autoConfigureConnectedAnchor = false;
		sjDist.anchor = new Vector3(0f, 0f, 0f);
		sjDist.connectedAnchor = new Vector3(0f, 0f, 0f);
		sjDist.spring = springStrength;
		sjDist.enableCollision = true;
		sjDist.minDistance = distance;
		sjDist.maxDistance = distance / 1.2f;
		sjDist.tolerance = 0f;
		sjDist.tag = "dist";
	}

	void AddAlphaHelicalHbondConstraints()
	{
		for (int resid = (numResidues - 1); resid > 4; resid--)
		{
			//AddBackboneHbondConstraint(GetAmideForResidue(resid), GetCarbonylForResidue(resid - 4));
			//InitBackboneHBondConstraint(GetAmideForResidue(resid));
		}
	}

	void InitBackboneHbondConstraints()
	{
		for (int resid = 0; resid < numResidues; resid++)
		{
			InitBackboneHbondConstraint(resid);
		}
	}

	void InitBackboneHbondConstraint(int resid)
	{
		// hbonds implemented as three spring joints
		// http://pubs.sciepub.com/ajme/3/2/3/
		//
		{
			// H -> O
			GameObject donorGO = GetAmideForResidue(resid);
			SpringJoint sjHbond = donorGO.AddComponent(typeof(SpringJoint)) as SpringJoint;
			hbondBackboneSj_HO[resid] = sjHbond;
			sjHbond.connectedBody = null;
			sjHbond.autoConfigureConnectedAnchor = false;
			
			// calculate anchor position from molecular geometry
			//
			//
			//                       Z axis
			//                       ^
			//        \  122         |
			//  119.5  N----         o---> X axis
			//        /          
			//       H          
			//               
			//
			float axisRotOffset = -90f;
			float thetaAmide = (float)((Mathf.Deg2Rad * ((122 + 119.5) + axisRotOffset)) * -1);
			float NHBondLength = 1.0f;


			sjHbond.anchor = new Vector3(Mathf.Sin(thetaAmide) * NHBondLength, 0f, Mathf.Cos(thetaAmide) * NHBondLength);

			// scale joint parameters to PolyPepBuilder and Amide_pf

			float scale = gameObject.transform.localScale.x * donorGO.transform.localScale.x;
			float HBondLength = hBondModelInnerLength;

			sjHbond.minDistance = HBondLength * scale;
			sjHbond.maxDistance = HBondLength * scale;
			sjHbond.tolerance = HBondLength * scale * 0.1f;
			sjHbond.enableCollision = true;
		}

		{
			// H -> C
			GameObject donorGO = GetAmideForResidue(resid);
			SpringJoint sjHbond2 = donorGO.AddComponent(typeof(SpringJoint)) as SpringJoint;
			hbondBackboneSj_HC[resid] = sjHbond2;
			sjHbond2.connectedBody = null;
			sjHbond2.autoConfigureConnectedAnchor = false;

			// calculate anchor position from molecular geometry
			//
			//
			//                       Z axis
			//                       ^
			//        \  122         |
			//  119.5  N----         o---> X axis
			//        /          
			//       H          
			//               
			//
			float axisRotOffset = -90f;
			float thetaAmide = (float)((Mathf.Deg2Rad * ((122 + 119.5) + axisRotOffset)) * -1);
			float NHBondLength = 1.0f;
			sjHbond2.anchor = new Vector3(Mathf.Sin(thetaAmide) * NHBondLength, 0f, Mathf.Cos(thetaAmide) * NHBondLength);

			// scale joint parameters to PolyPepBuilder and Amide_pf

			float scale = gameObject.transform.localScale.x * donorGO.transform.localScale.x;
			float HBondLength = hBondModelOuterLength;

			sjHbond2.minDistance = HBondLength * scale;
			sjHbond2.maxDistance = HBondLength * scale;
			sjHbond2.tolerance = HBondLength * scale * 0.1f;
			sjHbond2.enableCollision = true;
		}

		{
			// N -> O
			GameObject donorGO = GetAmideForResidue(resid);
			SpringJoint sjHbond3 = donorGO.AddComponent(typeof(SpringJoint)) as SpringJoint;
			hbondBackboneSj_NO[resid] = sjHbond3;
			sjHbond3.connectedBody = null;
			sjHbond3.autoConfigureConnectedAnchor = false;

			// calculate anchor position from molecular geometry
			//
			//
			//                       Z axis
			//                       ^
			//        \  122         |
			//  119.5  N----         o---> X axis
			//        /          
			//       H          
			//               
			//
			float axisRotOffset = -90f;
			float thetaAmide = (float)((Mathf.Deg2Rad * ((122 + 119.5) + axisRotOffset)) * -1);
			float NHBondLength = 1.0f;
			sjHbond3.anchor = new Vector3(0f, 0f, 0f); //(Mathf.Sin(thetaAmide) * NHBondLength, 0f, Mathf.Cos(thetaAmide) * NHBondLength);

			// scale joint parameters to PolyPepBuilder and Amide_pf

			float scale = gameObject.transform.localScale.x * donorGO.transform.localScale.x;
			float HBondLength = hBondModelOuterLength;

			sjHbond3.minDistance = HBondLength * scale;
			sjHbond3.maxDistance = HBondLength * scale;
			sjHbond3.tolerance = HBondLength * scale * 0.1f;
			sjHbond3.enableCollision = true;
		}

		SwitchOffBackboneHbondConstraint(resid);
		InitParticleSystemForHbond(resid);

	}

	void InitParticleSystemForHbond(int resid)
	{
		// create particle system for hbond
		GameObject donorGO = GetAmideForResidue(resid);
		var hbond_sj = hbondBackboneSj_HO[resid];
		var donorHLocation = donorGO.transform.TransformPoint(hbond_sj.anchor);

		Transform tf_H = donorGO.transform.Find("tf_H");

		hbondBackbonePsPf[resid] = Instantiate(hBondPsPf, donorHLocation, tf_H.rotation, tf_H); //HBond2_ps_pf
		hbondBackbonePsPf[resid].transform.localScale = transform.localScale;
		hbondBackbonePsPf[resid].name = "hb_backbone_ps";// + resid;
	}


	void UpdateHbondParticleSystems()
	{
		// aligns hbond particle systems to acceptor (if set)
		for (int resid = 0; resid < numResidues; resid++)
		{
			GameObject donorGO =  GetAmideForResidue(resid);
			var hbond_sj = hbondBackboneSj_HO[resid];
			var donorHLocation = donorGO.transform.TransformPoint(hbond_sj.anchor);

			if (hbond_sj.connectedBody != null)
			{
				// orient particle system toward acceptor atom
				var acceptorOLocation = hbond_sj.connectedBody.transform.TransformPoint(hbond_sj.connectedAnchor);
				Vector3 relativePosition = acceptorOLocation - donorHLocation;
				Quaternion lookAtAcceptor = Quaternion.LookRotation(relativePosition);
				//Debug.Log(hBondPsPf.transform.localRotation + " " + hBondPsPf.transform.rotation + " " + lookAtAcceptor);
				//DrawLine(donorHLocation, acceptorOLocation, Color.yellow, 0.05f);
				hbondBackbonePsPf[resid].transform.rotation = lookAtAcceptor;

				if (myPolyPepManager.hbondsOn)
				{
					ParticleSystem ps = hbondBackbonePsPf[resid].GetComponent<ParticleSystem>();
					ParticleSystem.EmissionModule em = ps.emission;
					em.rateOverTime = 4.0f; // magic number
				}
				else
				{
					ParticleSystem.EmissionModule em = hbondBackbonePsPf[resid].GetComponent<ParticleSystem>().emission;
					em.rateOverTime = 0.0f;
				}
			}
			else
			{
				// default if connectedBody is not set
				// orient particle system with NH bond

				//Transform donorN_amide = donorGO.transform.Find("N_amide");
				//Vector3 relativeNHBond = donorHLocation - donorN_amide.position;
				//Quaternion lookAwayFromN = Quaternion.LookRotation(relativeNHBond);
				//hbondBackbonePsPf[resid].transform.rotation = lookAwayFromN;
				ParticleSystem.EmissionModule em = hbondBackbonePsPf[resid].GetComponent<ParticleSystem>().emission;
				em.rateOverTime = 0.0f;

			}

		}
	}

	void UpdateHbonds()
	{
		float hbondCastScale = 3.0f; // length of cast in NH bond lengths ;)

		for (int resid = 0; resid < numResidues; resid++)
		{

			string resType = chainArr[resid].GetComponent<Residue>().type;
			//Debug.Log(resType);

			if ( ( myPolyPepManager.hbondsOn || (myPolyPepManager.hbondStrength > 0) ) && (resType != "PRO")) // DGD dirty hack to disable PRO HN hbond
			{
				GameObject donorGO = GetAmideForResidue(resid);

				var hbond_sj = hbondBackboneSj_HO[resid];
				var donorHLocation = donorGO.transform.TransformPoint(hbond_sj.anchor);

				Transform donorN_amide = donorGO.transform.Find("N_amide");
				Transform tf_H = donorGO.transform.Find("tf_H");

				Vector3 relativeNHBond = donorHLocation - donorN_amide.position;

				Vector3 NHBondUnit = relativeNHBond.normalized;

				Vector3 endLocation = (donorHLocation + (hbondCastScale * relativeNHBond));

				//Quaternion lookAwayFromN = Quaternion.LookRotation(relativeNHBond);
				//hbondBackbonePsPf[resid].transform.rotation = lookAwayFromN;

				if (true)
				{
					RaycastHit hit;
					Ray donorRay = new Ray(donorHLocation, -tf_H.transform.up);
					float castLength = (hbondCastScale * relativeNHBond.magnitude);
					float castRadius = 0.05f;
					bool foundAcceptor = false;
					//set layerMask for hbond and default layers (? may help reduce snappy hbonding through other atoms)
					int layerMask = (1 << 9) + 1;

					if (Physics.SphereCast(donorRay, castRadius, out hit, castLength, layerMask))
					{

						if ((hit.collider.gameObject.name == "hb_acceptor")) // || (hit.collider.gameObject.name == "O_carbonyl"))
						{

							//Debug.Log(resid + " hit " + hit.collider.gameObject + " " + hit.collider.transform.parent.parent.name);
							GameObject acceptorGO = hit.collider.transform.parent.parent.gameObject;
							//Debug.Log(acceptorGO);

							if (acceptorGO.GetComponent<BackboneUnit>() != null)
							{
								int targetAcceptorResid = acceptorGO.GetComponent<BackboneUnit>().residue;
								//Debug.Log(resid + "---> " + targetAcceptorResid);
								int offset = 2;
                                //Debug.Log(gameObject.name + "---> " + acceptorGO.transform.root.name);
                                //bool sameChain = (gameObject.name == acceptorGO.transform.root.name);
                                bool sameChain = (gameObject.transform.root == acceptorGO.transform.root);
                                bool notTooClose = (((resid + offset) < targetAcceptorResid) || ((resid - offset) > targetAcceptorResid));
                                if (notTooClose)
                                {
                                    //Debug.Log(resid + "---> " + targetAcceptorResid);
                                }
                                //Debug.Log("->" + sameChain);
                                if (!sameChain || (sameChain && notTooClose) )
								{
									foundAcceptor = true;
									//DrawLine(donorHLocation, hit.point, Color.red, 0.02f);

									// scale hbond strength - 1st attempt at softer switching function
									float hbondLength = Vector3.Distance(hit.point, donorRay.origin);
									float hbondLengthRelative = ((castLength - hbondLength) / castLength);
									float scaledHbondStrength = myPolyPepManager.hbondStrength * hbondLengthRelative * hbondLengthRelative;
									//Debug.Log(resid + "---> " + targetAcceptorResid + "  hbond length = " + hbondLength + " " + hbondLengthRelative * hbondLengthRelative);

									SetAcceptorForBackboneHbondConstraint(resid, acceptorGO);
									SwitchOnBackboneHbondConstraint(resid, scaledHbondStrength);
								}
								//else
								//{
								//	//found CO but too close in chain
								//	//DrawLine(donorHLocation, hit.point, Color.magenta, 0.02f);
								//}
							}
						}
						else
						{
							// hit something - not CO
							//DrawLine(donorHLocation, hit.point, Color.cyan, 0.02f);
						}

					}
					else
					{
						// no hit
						//DrawLine(donorHLocation, endLocation, Color.green, 0.02f);
					}
					if (!foundAcceptor)
					{
						SwitchOffBackboneHbondConstraint(resid);
						ClearAcceptorForBackboneHbondConstraint(resid);
					}
				}

			}
			else
			{
				SwitchOffBackboneHbondConstraint(resid);
				ClearAcceptorForBackboneHbondConstraint(resid);
			}
		}

	}



	void SwitchOffBackboneHbondConstraint(int resid)
	{
		SetSpringJointValuesForBackboneHbondConstraint(resid, 0, 0);
	}

	void SwitchOnBackboneHbondConstraint(int resid, float hbondScaledStrength)
	{
		SetSpringJointValuesForBackboneHbondConstraint(resid, (int)hbondScaledStrength, 5); // empirical values
	}

	void SetSpringJointValuesForBackboneHbondConstraint(int resid, int springStrength, int springDamper)
	{
		hbondBackboneSj_HO[resid].spring = springStrength;
		hbondBackboneSj_HC[resid].spring = springStrength;
		hbondBackboneSj_NO[resid].spring = springStrength;

		hbondBackboneSj_HO[resid].damper = springDamper;
		hbondBackboneSj_HC[resid].damper = springDamper;
		hbondBackboneSj_NO[resid].damper = springDamper;
	}

	void SetAcceptorForBackboneHbondConstraint(int donorResid, GameObject acceptorGO)
	{

		GameObject donorGO = GetAmideForResidue(donorResid);
		//GameObject acceptorGO = GetCarbonylForResidue(acceptorResid);

		// HO spring
		SpringJoint sjHbond = hbondBackboneSj_HO[donorResid];
		if (sjHbond.connectedBody != acceptorGO.GetComponent<Rigidbody>())
		{
			sjHbond.connectedBody = acceptorGO.GetComponent<Rigidbody>();


			// calculate connected anchor position from molecular geometry
			//
			//
			//                          Z axis
			//       O                  ^
			//        \  123.5          |
			//         C----            o---> X axis
			//        /          
			//                 
			//               
			//
			float axisRotOffset = -90f;
			float thetaCarbonyl = (float)((Mathf.Deg2Rad * (123.5 + axisRotOffset)) * -1);
			float COBondLength = 1.24f;
			sjHbond.connectedAnchor = new Vector3(Mathf.Sin(thetaCarbonyl) * COBondLength, 0f, Mathf.Cos(thetaCarbonyl) * COBondLength);


		}

		//HC spring
		sjHbond = hbondBackboneSj_HC[donorResid];
		if (sjHbond.connectedBody != acceptorGO.GetComponent<Rigidbody>())
		{
			sjHbond.connectedBody = acceptorGO.GetComponent<Rigidbody>();
			// calculate connected anchor position from molecular geometry
			sjHbond.connectedAnchor = new Vector3(0f, 0f, 0f); // No offset as the connected anchor is C atom
		}



		//new Vector3(Mathf.Sin(thetaCarbonyl) * COBondLength, 0f, Mathf.Cos(thetaCarbonyl) * COBondLength);

		//NO spring
		sjHbond = hbondBackboneSj_NO[donorResid];
		if (sjHbond.connectedBody != acceptorGO.GetComponent<Rigidbody>())
		{
			sjHbond.connectedBody = acceptorGO.GetComponent<Rigidbody>();
			// calculate connected anchor position from molecular geometry
			//
			//
			//                          Z axis
			//       O                  ^
			//        \  123.5          |
			//         C----            o---> X axis
			//        /          
			//                 
			//               
			//
			float axisRotOffset = -90f;
			float thetaCarbonyl = (float)((Mathf.Deg2Rad * (123.5 + axisRotOffset)) * -1);
			float COBondLength = 1.24f;
			sjHbond.connectedAnchor = new Vector3(Mathf.Sin(thetaCarbonyl) * COBondLength, 0f, Mathf.Cos(thetaCarbonyl) * COBondLength);
		}
	}

	void ClearAcceptorForBackboneHbondConstraint(int resid)
	{
		if (hbondBackboneSj_HO[resid].connectedBody)
		{
			hbondBackboneSj_HO[resid].connectedBody = null;
		}
		if (hbondBackboneSj_HC[resid].connectedBody)
		{
			hbondBackboneSj_HC[resid].connectedBody = null;
		}
		if (hbondBackboneSj_NO[resid].connectedBody)
		{
			hbondBackboneSj_NO[resid].connectedBody = null;
		}

	}

	void SetChainAlphaHelicalHBonds()
	{
		SetChainPeriodicHBonds(-4);
	}

	void SetChain310HelicalHBonds()
	{
		SetChainPeriodicHBonds(-3);
	}

	void SetChainPiHelicalHBonds()
	{
		SetChainPeriodicHBonds(-5);
	}

	void SetChainPeriodicHBonds(int offset)
	{
		if (offset < 0)
		{
			for (int resid = 0; resid < numResidues; resid++)
			{
				var donorGO = GetAmideForResidue(resid);
				if (resid > ((-offset) - 1))
				{
					//GameObject acceptorGO = GetCarbonylForResidue(resid + offset);
					//SetAcceptorForBackboneHbondConstraint(resid, (resid + offset));
					//Debug.Log(i + " " + donorGO + " " + acceptorGO);
				}
				else
				{
					SwitchOffBackboneHbondConstraint(resid);

					ClearAcceptorForBackboneHbondConstraint(resid);
				}
			}
		}
		else
		{
			Debug.Log("ERROR: Can't set forward periodic HBonds");
		}
	}


	void ClearChainHBonds()
	{
		for (int resid = 0; resid < numResidues; resid++)
		{
			var donorGO = GetAmideForResidue(resid);
			SwitchOffBackboneHbondConstraint(resid);
			ClearAcceptorForBackboneHbondConstraint(resid);
		}
	}

	public GameObject GetAmideForResidue(int residue)
	{
		return (polyArr[residue * 3]);
	}

	public GameObject GetCalphaForResidue(int residue)
	{
		return (polyArr[(residue * 3) + 1]);
	}

	public GameObject GetCarbonylForResidue(int residue)
	{
		return (polyArr[(residue * 3) + 2]);
	}

	int GetResidForPolyArrGO(GameObject go)
	{
		string[] nameSplit = go.name.Split('_');
		int Resid = int.Parse(nameSplit[0]);
		return Resid;
	}

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(0.02f, 0.01f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

	void UpdateDistanceConstraintGfx()
	{
		// iterates through poly chain 
		for (int i = 0; i < polyLength; i++)
		{
			var springs = polyArr[i].GetComponents<SpringJoint>();
			if (GetResidForPolyArrGO(polyArr[i]) > 4) // hack - testing alpha helical hbonds
			{
			foreach (var s in springs)
			{
				//Debug.Log("spring " + s + " " + s.tag);

				//if (s.tag == "dist")
				{
					var startPoint = polyArr[i].transform.TransformPoint(s.anchor);
					var endPoint = s.connectedBody.transform.TransformPoint(s.connectedAnchor);

					Color constraintColor = Color.green;

					if (Vector3.Distance(startPoint, endPoint) >= (s.minDistance + s.tolerance))
					{
						constraintColor = Color.red;
					}
					if (Vector3.Distance(startPoint, endPoint) <= (s.maxDistance - s.tolerance))
					{
						constraintColor = Color.yellow;
					}
					DrawLine(startPoint, endPoint, constraintColor, 0.05f);
				}
			}
			}

		}
	}

	public void SetPhiPsiDriveForSelection(bool value)
	{
		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			if (residue.IsResidueSelected())
			{
				residue.drivePhiPsiOn = value;
			}
		}
		UpdatePhiPsiDrives();
	}

	public void SetPhiPsiTargetValuesForSelection(float phi, float psi)
	{
		// use 'painted' selection from controller i.e. controllerSelectOn
		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			if  (residue.IsResidueSelected())
			{
				SetPhiPsiTargetValuesForResidue(resid, phi, psi);
				residue.drivePhiPsiOn = true;
			}
		}
		UpdatePhiPsiDrives();
	}

	void SetPhiPsiTargetValuesForResidue(int resid, float phi, float psi)
	{
		Residue residue = chainArr[resid].GetComponent<Residue>();

		residue.phiTarget = phi;
		residue.psiTarget = psi;

		var cjPhi_NCa = GetAmideForResidue(resid).GetComponent<ConfigurableJoint>();
		cjPhi_NCa.targetRotation = Quaternion.Euler(180.0f - phi, 0, 0);

		var cjPsi_CaCO = GetCalphaForResidue(resid).GetComponent<ConfigurableJoint>();
		cjPsi_CaCO.targetRotation = Quaternion.Euler(180.0f - psi, 0, 0);
	}

	void UpdatePhiPsiDriveParamForResidue(int resid)
	{
		var cjPhi_NCa = GetAmideForResidue(resid).GetComponent<ConfigurableJoint>();
		cjPhi_NCa.angularXDrive = chainPhiJointDrives[resid];

		var cjPsi_CaCO = GetCalphaForResidue(resid).GetComponent<ConfigurableJoint>();
		cjPsi_CaCO.angularXDrive = chainPsiJointDrives[resid];

		// wake rbs
		// (sleeping rbs don't respond to changes in joint params)
		WakeResidRbs(resid);
	}

	void WakeResidRbs(int resid)
	{
		var RbAmide = GetAmideForResidue(resid).GetComponent<Rigidbody>();
		var RbCalpha  = GetCalphaForResidue(resid).GetComponent<Rigidbody>();
		var RbCarbonyl = GetCarbonylForResidue(resid).GetComponent<Rigidbody>();

		RbAmide.WakeUp();
		RbCalpha.WakeUp();
		RbCarbonyl.WakeUp();
	}

	public void UpdatePhiPsiDriveTorques(bool selectedResiduesOnly)
	{
		// selectedResiduesOnly == true default behaviour

		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();

			if (residue.IsResidueSelected() || !selectedResiduesOnly)
			{
				residue.drivePhiPsiTorqValue = myPolyPepManager.phiPsiDriveTorqueFromUI;
			}

			if (residue.drivePhiPsiOn)
			// DGD - currently (2019.1.2) this is always true - interface for switching is disabled in UI
			//on
			{
				// active
				chainPhiJointDrives[resid].maximumForce = residue.drivePhiPsiTorqValue;
				chainPhiJointDrives[resid].positionDamper = drivePhiPsiPosDamper;
				chainPhiJointDrives[resid].positionSpring = residue.drivePhiPsiTorqValue;

				chainPsiJointDrives[resid].maximumForce = residue.drivePhiPsiTorqValue;
				chainPsiJointDrives[resid].positionDamper = drivePhiPsiPosDamper;
				chainPsiJointDrives[resid].positionSpring = residue.drivePhiPsiTorqValue;
				//Debug.Log("PhiPsi Drive = ON ");
			}
			else
			//off
			{
				//passive
				chainPhiJointDrives[resid].maximumForce = 0.0f;
				chainPhiJointDrives[resid].positionDamper = drivePhiPsiPosDamperPassive;
				chainPhiJointDrives[resid].positionSpring = 0.0f;

				chainPsiJointDrives[resid].maximumForce = 0.0f;
				chainPsiJointDrives[resid].positionDamper = drivePhiPsiPosDamperPassive;
				chainPsiJointDrives[resid].positionSpring = 0.0f;
				//Debug.Log("PhiPsi Drive = OFF ");
			}

			UpdatePhiPsiDriveParamForResidue(resid);

		}

	}

	public void UpdatePhiPsiDrives()
	{
		bool selectionOnly = true;
		UpdatePhiPsiDriveTorques(selectionOnly);
	}

	public void UpdatePhiPsiDrivesForceAll()
	{
		bool selectionOnly = false;
		UpdatePhiPsiDriveTorques(selectionOnly);
	}

	public void NudgeHbondSprings()
	{
		if (myPolyPepManager.hbondStrength > 0)
		{
			for (int resid = 0; resid < numResidues; resid++)
			{
				WakeResidRbs(resid);
			}
		}
	}


	private bool LoadPhiPsiData(string fileName)
	{
		// https://answers.unity.com/questions/279750/loading-data-from-a-txt-file-c.html
		// Handle any problems that might arise when reading the text
		try
		{
			string line;
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			using (theReader)
			{
				// While there's lines left in the text file, do this:
				do
				{
					line = theReader.ReadLine();

					if (line != null)
					{
						// Do whatever you need to do with the text line, it's a string now
						// In this example, I split it into arguments based on comma
						// deliniators, then send that array to DoStuff()
						Debug.Log(line);
						ParsePhiPsi(line);
						string[] entries = line.Split(',');
						if (entries.Length > 0)
						{

							//DoStuff(entries);
						}

					}
				}
				while (line != null);
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				return true;
			}
		}
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
			return false;
		}
	}


	private void ParsePhiPsi(string line)
	{
		//
		// parses output from PYMOL command: phi_psi all
		//
		// " PHE-4:    (  -70.8,  -44.7 )"
		//
		string resName = line.Substring(1, 3);
		string residRaw = line.Substring(5, 5);
		string[] residSplit = residRaw.Split(':');

		int myResid = int.Parse(residSplit[0]);
		float myPhi = float.Parse(line.Substring(12, 7));
		float myPsi = float.Parse(line.Substring(20, 7));

		if (myResid == 2)
		{
			// add default data for resid 1
			PeptideData _peptideData = new PeptideData();
			_peptideData.residPD = "XXX";
			_peptideData.phiPD = 0f;
			_peptideData.psiPD = 0f;
			myPeptideDataList.Add(_peptideData);
		}

		Debug.Log("  resname = " + resName);
		Debug.Log("  resid = " + myResid);
		Debug.Log("  phi = " + myPhi);
		Debug.Log("  psi = " + myPsi);

		{
			// add data for current resid
			PeptideData _peptideData = new PeptideData();
			_peptideData.residPD = resName;
			_peptideData.phiPD = myPhi;
			_peptideData.psiPD = myPsi;
			myPeptideDataList.Add(_peptideData);
		}


		//SetPhiPsiTargetValuesForResidue(myResid, myPhi, myPsi);
		numResidues = myResid;
	}

	public void UpdateResidueSelectionStartFromUI()
	{
		residSelectStart = (int)resStartSliderUI.value - 1;
		//Debug.Log(residSelectStart);
		if (resEndSliderUI != null)
		{
			if (resStartSliderUI.value > resEndSliderUI.value)
			{
				resStartSliderUI.value = resEndSliderUI.value;
			}
		}

		if (residSelectStart != residSelectStartLast)
		{
			UpdateResidueSelection();
			residSelectStartLast = residSelectStart;
		}

	}

	public void UpdateResidueSelectionEndFromUI()
	{
		residSelectEnd = (int)resEndSliderUI.value - 1;
		//Debug.Log(residSelectEnd);
		if (resStartSliderUI != null)
		{
			if (resEndSliderUI.value < resStartSliderUI.value)
			{
				resEndSliderUI.value = resStartSliderUI.value;
			}
		}
		if (residSelectEnd != residSelectEndLast)
		{
			UpdateResidueSelection();
			residSelectEndLast = residSelectEnd;
		}
	}

	private void SetSequenceSelectionForBackboneUnit(GameObject go, bool flag)
	{
		BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
		if (bu != null)
		{
			//Debug.Log("      --> script");
			//bu.SetGoRendering(go, "ToonOutlineRed");
			bu.SetActiveSequenceSelect(flag);
		}
	}

	private void SetSequenceSelectionForResidue(int resid, bool flag)
	{
		SetSequenceSelectionForBackboneUnit(GetAmideForResidue(resid), flag);
		SetSequenceSelectionForBackboneUnit(GetCalphaForResidue(resid), flag);
		SetSequenceSelectionForBackboneUnit(GetCarbonylForResidue(resid), flag);
	}

	private void UpdateResidueSelection()
	{
		for (int resid = 0; resid < residSelectStart; resid++)
		{
			SetSequenceSelectionForResidue(resid, false);
		}
		for (int resid = residSelectStart; resid <= residSelectEnd; resid++)
		{
			SetSequenceSelectionForResidue(resid, true);
		}
		for (int resid = (residSelectEnd + 1); resid <= (numResidues - 1); resid++)
		{
			SetSequenceSelectionForResidue(resid, false);
		}
	}


	public void ReCentrePolyPep()
	{
		Vector3 reCentrePos = new Vector3 (0f, 1.5f, -0f); //arbitrary position
		Bounds b = GetNAtomBounds();

		gameObject.transform.position += (reCentrePos - b.center);

	}

	private Bounds GetNAtomBounds()
	{
		// bounds for amide N atoms = approx bounds for polypeptide 
		Bounds b = new Bounds();

		float minX = Mathf.Infinity;
		float minY = Mathf.Infinity;
		float minZ = Mathf.Infinity;
		float maxX = -Mathf.Infinity;
		float maxY = -Mathf.Infinity;
		float maxZ = -Mathf.Infinity;
	
		//CAUTION! legacy assumes single polypeptide!
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("N"))
		{

			if (go.transform.position.x < minX) minX = go.transform.position.x;
			if (go.transform.position.y < minY) minY = go.transform.position.y;
			if (go.transform.position.z < minZ) minZ = go.transform.position.z;

			if (go.transform.position.x > maxX) maxX = go.transform.position.x;
			if (go.transform.position.y > maxY) maxY = go.transform.position.y;
			if (go.transform.position.z > maxZ) maxZ = go.transform.position.z;

			//Debug.Log(go);
		}

	
		Vector3 _extent = new Vector3 ((maxX - minX), (maxY - minY), (maxZ - minZ));
		Vector3 _centre = new Vector3 ((maxX + minX)/2, (maxY + minY)/2, (maxZ + minZ)/2);

		b.extents = _extent;
		b.center = _centre;

		//Debug.Log(b);


		return b;
	}

	void OnDrawGizmos()
	{
		Bounds b = GetNAtomBounds();
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(b.center, b.extents);
	}

	public void SetGlobalSelect(bool value)
	{
		for (int i = 0; i < polyLength; i++)
		{
			BackboneUnit _bbu = (polyArr[i].GetComponent("BackboneUnit") as BackboneUnit);
			Assert.IsTrue(_bbu);
			_bbu.SetMyResidueSelect(value);
			//_bbu.SetBackboneUnitSelect(value);
		}
	}

	public void InvertSelection()
	{
		for (int i = 0; i < polyLength; i++)
		{
			BackboneUnit _bbu = (polyArr[i].GetComponent("BackboneUnit") as BackboneUnit);
			Assert.IsTrue(_bbu);
			if (_bbu.myResidue.IsResidueSelected())
			{
				_bbu.SetMyResidueSelect(false);
			}
			else
			{
				_bbu.SetMyResidueSelect(true);
			}
		}
	}

	public void UpdateRenderModeAllBbu()
	{
		for (int i = 0; i < polyLength; i++)
		{
			BackboneUnit _bbu = (polyArr[i].GetComponent("BackboneUnit") as BackboneUnit);
			Assert.IsTrue(_bbu);
			_bbu.UpdateRenderMode();
			//_bbu.SetBackboneUnitSelect(value);
		}
	}

	private void DoJiggle()
	{
		if (myPolyPepManager.jiggleStrength > 0.0f)
		{
			//backbone
			for (int i = 0; i < polyLength; i++)
			{
				JiggleRb(polyArr[i].GetComponent<Rigidbody>());
			}
			//sidechains
			for (int resid = 0; resid < numResidues; resid++)
			{ 
				foreach (GameObject _sidechainGO in chainArr[resid].GetComponent<Residue>().sideChainList)
				{
					JiggleRb(_sidechainGO.GetComponent<Rigidbody>());
				}
			}
		}

	}

	private void JiggleRb(Rigidbody rb)
	{
		rb.AddForce(UnityEngine.Random.onUnitSphere * 0.01f * myPolyPepManager.jiggleStrength, ForceMode.Impulse);
		//rb.AddTorque((0.1f * new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))), ForceMode.Impulse);
	}
	
	// Update is called once per frame
	void Update()
	{
		//UpdatePhiPsiDrives();
		//UpdateDistanceConstraintGfx();
		UpdateHbonds();
		UpdateHbondParticleSystems();
		DoJiggle();
	}
}

