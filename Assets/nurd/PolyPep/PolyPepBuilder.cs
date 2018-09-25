using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PolyPepBuilder : MonoBehaviour {

	public GameObject amidePf;
	public GameObject calphaPf;
	public GameObject carbonylPf;
	public GameObject residuePf;
	public GameObject hBondPsPf;

	// bond lengths used in backbone configurable joints
	// values are replicated in the prefabs (Carbonyl_pf, Amide_pf, Calpha_pf)
	float bondLengthPeptide = 1.33f;
	float bondLengthAmideCalpha = 1.46f;
	float bondLengthCalphaCarbonyl = 1.51f;

	public int secondaryStructure { get; set; } // = 0;

	public int numResidues = 0;

	public GameObject[] polyArr;
	private int polyLength;

	public GameObject[] chainArr;

	public GameObject[] hbondBackbonePsPf;

	public SpringJoint[] hbondBackboneSj_HO;
	public SpringJoint[] hbondBackboneSj_HC;
	public SpringJoint[] hbondBackboneSj_NO;

	public JointDrive[] chainPhiJointDrives;
	private JointDrive[] chainPsiJointDrives;

	//public bool UseColliders { get; set; } //= true;
	public bool setTargetPhiPsi { get; set; }
	public bool setDrivePhiPsi { get; set; }
	public bool ActiveHbondSpringConstraints { get; set; }

	public float hbondStrength = 0f; // updated by PolyPepManager

	public float drivePhiPsiMaxForce = 200.0f; // 100.0f
	public float drivePhiPsiPosSpring = 200.0f; // 100.0f
	public int drivePhiPsiPosDamper = 1;

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

	// Use this for initialization
	void Start()
	{
		//Debug.Log("LOAD FILE = " + LoadPhiPsiData("Assets/Data/253l_phi_psi.txt"));

		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");

		buildPolypeptideChain();

		// test: add arbitrary distance constraints
		//AddDistanceConstraint(polyArr[2], polyArr[12], 0.6f, 20);
		//AddDistanceConstraint(polyArr[15], polyArr[30], 0.8f, 20);
		//AddDistanceConstraint(polyArr[0], polyArr[36], 0.8f, 20);

		// placeholder: should be created and updated on tick
		//InvokeRepeating("UpdateDistanceConstraintGfx", 0, 0.05f);

		//Debug.Log("LOAD FILE = " + Load("Assets/Data/253l_phi_psi.txt"));
		//disablePhiPsiUIInput = true;
		//Debug.Log("LOAD FILE = " + LoadPhiPsiData("Assets/Data/1xda_phi_psi.txt")); 

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


		// offset from handling cube
		var offsetPositionBase = new Vector3(0.5f, 0f, 0f);
		// periodic offsets for polymer
		float xOffset = 0.2f; // empirical - enough to avoid collider overlap
		var offsetPositionUnit = new Vector3(xOffset * transform.localScale.x, 0f, 0f);


		for (int i = 0; i < polyLength; i++)
		{
			Transform lastUnitTransform;
			if (i == 0)
			{
				//start of chain
				lastUnitTransform = transform;
			}
			else
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
			// prefab backbone bonds are aligned to Z axis, Y rotations of prefabs create correct backbone bond angles
			//
			// prefabs for 2,3 and 4 positions are flipped 180 X to make alternating extended chain

			switch (id)
			{
				case 0:
					AddResidueToChain(i / 3);
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 0, 0), chainArr[i/3].transform);
					break;
				case 1:
					// Yrot = +69
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 69, 0), chainArr[i / 3].transform);
					break;
				case 2:
					// Yrot = +69 -64 = 5
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 5, 0), chainArr[i / 3].transform);
					break;
				case 3:
					AddResidueToChain(i / 3);
					// Yrot = +69 -64 +58 = 63
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 63, 0), chainArr[i / 3].transform);
					
					break;
				case 4:
					// Yrot = +69 -64 +58 -69 = -6
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, -6, 0), chainArr[i / 3].transform);
					break;
				case 5:
					// Yrot = +69 -64 +58 -69 +64 = 58
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 58, 0), chainArr[i / 3].transform);
					break;

			}




			polyArr[i].name = ((i / 3)).ToString() + "_" + polyArr[i].name;
			polyArr[i].GetComponent<BackboneUnit>().residue = i / 3;

			ScaleVDW(1.0f);
			SetRbDrag(polyArr[i]);
			//SetCollidersGameObject(polyArr[i]);


			{
				// turn off shadows / renderer
				Renderer[] allChildren = GetComponentsInChildren<Renderer>();
				foreach (Renderer child in allChildren)
				{
					child.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				}
			}

			if (i > 0)
			{
				//AddBackboneConstraint(polyArr[i - 1], polyArr[i]);
				AddBackboneTopologyConstraint(i);
			}

		}

		SetAllColliderIsTrigger (true);

		for (int i = 0; i < polyLength; i++)
		{
			var rigidBodies = polyArr[i].GetComponentsInChildren<Rigidbody>();
			foreach (var rb in rigidBodies)
			{
				rb.mass = 1;
				rb.drag = 5;
				rb.angularDrag = 5;
			}
		}


		// assign references in chainArr
		for (int resid = 0; resid < numResidues; resid ++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			residue.amide_pf = polyArr[resid * 3];
			residue.calpha_pf = polyArr[(resid * 3) + 1];
			residue.carbonyl_pf = polyArr[(resid * 3) + 2];
		}

		InitBackboneHbondConstraints();

		ReCentrePolyPep();
	}

	void AddResidueToChain(int index)
	{
		chainArr[index] = Instantiate(residuePf, transform);
		chainArr[index].name = "Residue_" + (index).ToString();
	}

	public void ScaleVDW(float scale)
	{
		{
			float scaleVDW = scale;
			float radiusN = 1.0f;
			float radiusC = 1.0f;
			float radiusO = 1.0f;
			float radiusH = 0.75f;

			float radiusR = 1.25f;

			Transform[] allChildren = GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren)
			{
				float atomScale;
				switch (child.tag)
				{
					case "N":
						atomScale = radiusN * scaleVDW;
						child.transform.localScale = new Vector3(atomScale, atomScale, atomScale);
						break;
					case "C":
						atomScale = radiusC * scaleVDW;
						child.transform.localScale = new Vector3(atomScale, atomScale, atomScale);
						break;
					case "O":
						atomScale = radiusO * scaleVDW;
						child.transform.localScale = new Vector3(atomScale, atomScale, atomScale);
						break;
					case "H":
						atomScale = radiusH * scaleVDW;
						child.transform.localScale = new Vector3(atomScale, atomScale, atomScale);
						break;
					case "R":
						atomScale = radiusR * scaleVDW;
						child.transform.localScale = new Vector3(atomScale, atomScale, atomScale);
						break;
				}

			}
		}
	}


	void SetRbDrag(GameObject go)
	{
		// empirical values which seem to behave well
		go.GetComponent<Rigidbody>().drag = 5;
		go.GetComponent<Rigidbody>().angularDrag = 1;

		//test
		//go.GetComponent<Rigidbody>().mass = 0.001f;


	}

	public void SetAllColliderIsTrigger(bool value)
	{
		for (int i = 0; i < polyLength; i++)
		{
			var colliders = polyArr[i].GetComponentsInChildren<Collider>();
			foreach (var col in colliders)
			{
				col.isTrigger = value;
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
			float HBondLength = 1.0f;

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
			float HBondLength = 3.5f;

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
			float HBondLength = 3.5f;

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
		hbondBackbonePsPf[resid].name = "hb_backbone " + resid;
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

				if (ActiveHbondSpringConstraints)
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

	void HbondLineTrace()
	{
		float hbondCastScale = 3.0f; // length of cast in NH bond lengths ;)

		for (int resid = 0; resid < numResidues; resid++)
		//int resid = 1;
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
				

				//if (Physics.SphereCast(donorHLocation, castRadius, relativeNHBond.normalized, out hit, castLength))
				if (Physics.SphereCast(donorRay, castRadius, out hit, castLength, layerMask))
				{
					
					if ((hit.collider.gameObject.name == "hbond_acceptor") || (hit.collider.gameObject.name == "O_carbonyl"))
					{
						
						//Debug.Log(resid + " hit " + hit.collider.gameObject + " " + hit.collider.transform.parent.parent.name);
						GameObject acceptorGO = hit.collider.transform.parent.parent.gameObject;
						//Debug.Log(acceptorGO);

						if (acceptorGO.GetComponent<BackboneUnit>() != null)
						{
							int targetAcceptorResid = acceptorGO.GetComponent<BackboneUnit>().residue;
							//Debug.Log(resid + "---> " + targetAcceptorResid);
							//int offset = 3;
							//if ( ((resid + offset) <= targetAcceptorResid) || ((resid - offset) >= targetAcceptorResid) ) 
							{
								foundAcceptor = true;
								//DrawLine(donorHLocation, hit.point, Color.red, 0.02f);
								SetAcceptorForBackboneHbondConstraint(resid, acceptorGO);
								SwitchOnBackboneHbondConstraint(resid);
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
	}



	void SwitchOffBackboneHbondConstraint(int resid)
	{
		SetSpringJointValuesForBackboneHbondConstraint(resid, 0, 0);
	}

	void SwitchOnBackboneHbondConstraint(int resid)
	{
		if (ActiveHbondSpringConstraints)
		{
			SetSpringJointValuesForBackboneHbondConstraint(resid, (int)hbondStrength, 5); // empirical values
		}
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

	GameObject GetAmideForResidue(int residue)
	{
		return (polyArr[residue * 3]);
	}

	GameObject GetCalphaForResidue(int residue)
	{
		return (polyArr[(residue * 3) + 1]);
	}

	GameObject GetCarbonylForResidue(int residue)
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

	public void SetPhiPsiForSelection(float phi, float psi)
	{
		// use 'painted' selection from controller i.e. controllerSelectOn
		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			BackboneUnit buAmide = residue.amide_pf.GetComponent("BackboneUnit") as BackboneUnit;
			if (buAmide.controllerSelectOn)
			{
				SetPhiPsiForResidue(resid, phi, psi);
				residue.drivePhiPsiOn = true;
				//Debug.Log("hello");
			}
		}
		UpdatePhiPsiDrives();
	}

	void SetPhiPsiForResidue(int resid, float phi, float psi)
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

	public void UpdatePhiPsiDrives()
	{
		// Note that 'Position' is actually a rotation ;)
		//
		// values are empirical
		//

		//float drivePhiPsiMaxForce = 200.0f;	// 100.0f
		//float drivePhiPsiPosSpring = 200.0f; // 100.0f

		//int drivePhiPsiPosDamper = 1;
		int drivePhiPsiPosDamperPassive = 0;


		for (int resid = 0; resid < numResidues; resid++)
		{
			Residue residue = chainArr[resid].GetComponent<Residue>();
			if (residue.drivePhiPsiOn)
			//on
			{
				// active
				chainPhiJointDrives[resid].maximumForce = drivePhiPsiMaxForce;
				chainPhiJointDrives[resid].positionDamper = drivePhiPsiPosDamper;
				chainPhiJointDrives[resid].positionSpring = drivePhiPsiPosSpring;

				chainPsiJointDrives[resid].maximumForce = drivePhiPsiMaxForce;
				chainPsiJointDrives[resid].positionDamper = drivePhiPsiPosDamper;
				chainPsiJointDrives[resid].positionSpring = drivePhiPsiPosSpring;

				UpdatePhiPsiDriveParamForResidue(resid);

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

				UpdatePhiPsiDriveParamForResidue(resid);

				//Debug.Log("PhiPsi Drive = OFF ");
			}
			

		}

		//if (drivePhiPsi == true)
		//{
		//	for (int i = 0; i < numResidues; i++)
		//	{
		//		chainPhiJointDrives[i].maximumForce = drivePhiPsiMaxForce;
		//		chainPhiJointDrives[i].positionDamper = drivePhiPsiPosDamper;
		//		chainPhiJointDrives[i].positionSpring = drivePhiPsiPosSpring;

		//		chainPsiJointDrives[i].maximumForce = drivePhiPsiMaxForce;
		//		chainPsiJointDrives[i].positionDamper = drivePhiPsiPosDamper;
		//		chainPsiJointDrives[i].positionSpring = drivePhiPsiPosSpring;

		//		UpdatePhiPsiDriveParamForResidue(i);
		//	}
		//	Debug.Log("PhiPsi Drive = ON ");
		//}
		//else
		//{
		//	for (int i = 0; i < numResidues; i++)
		//	{
		//		chainPhiJointDrives[i].maximumForce = 0.0f;
		//		chainPhiJointDrives[i].positionDamper = drivePhiPsiPosDamperPassive;
		//		chainPhiJointDrives[i].positionSpring = 0.0f;

		//		chainPsiJointDrives[i].maximumForce = 0.0f;
		//		chainPsiJointDrives[i].positionDamper = drivePhiPsiPosDamperPassive;
		//		chainPsiJointDrives[i].positionSpring = 0.0f;

		//		UpdatePhiPsiDriveParamForResidue(i);
		//	}
		//	Debug.Log("PhiPsi Drive = OFF ");

		//}

	}


	public void UpdateHBondSprings()
	{
		if (ActiveHbondSpringConstraints)
		{
			for (int resid = 0; resid < numResidues; resid++)
			{
				GameObject donorGO = GetAmideForResidue(resid);
				var hbond_sj = donorGO.GetComponent<SpringJoint>();
				var donorHLocation = donorGO.transform.TransformPoint(hbond_sj.anchor);

				if (hbond_sj.connectedBody != null)
				{
					SwitchOnBackboneHbondConstraint(resid);
					WakeResidRbs(resid);
				}
			}
			Debug.Log("HBond Springs = ON ");
		}
		else
		{
			for (int resid = 0; resid < numResidues; resid++)
			{
				GameObject donorGO = GetAmideForResidue(resid);
				SwitchOffBackboneHbondConstraint(resid);
			}
			Debug.Log("HBond Springs = OFF ");
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


	void ParsePhiPsi(string line)
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


		Debug.Log("  resname = " + resName);
		Debug.Log("  resid = " + myResid);
		Debug.Log("  phi = " + myPhi);
		Debug.Log("  psi = " + myPsi);

		SetPhiPsiForResidue(myResid, myPhi, myPsi);
	}

	public void UpdateHbondStrengthFromUI()
	{
		//TODO  Update Hbonds with new strength!
		UpdateHBondSprings();
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
		for (int resid = 0; resid < residSelectStart; resid ++)
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
			_bbu.SetBackboneUnitSelect(value);
		}
	}

	public void ResetLevel()
	{
		Scene m_Scene = SceneManager.GetActiveScene();
		Debug.Log("Loading... " + m_Scene.name);
		SceneManager.LoadScene(m_Scene.name);
	}

	// Update is called once per frame
	void Update()
	{
		//UpdatePhiPsiDrives();

		//UpdateDistanceConstraintGfx();
		HbondLineTrace();
		UpdateHbondParticleSystems();
		//UpdateHBondSprings();
	}
}

