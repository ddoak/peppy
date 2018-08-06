using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class PolyPepBuilder : MonoBehaviour {

	public GameObject amidePf;
	public GameObject calphaPf;
	public GameObject carbonylPf;

	public GameObject hBondPsPf;

	// bond lengths used in backbone configurable joints
	float bondLengthPeptide = 1.33f;
	float bondLengthAmideCalpha = 1.46f;
	float bondLengthCalphaCarbonyl = 1.51f;

	public bool useColliders = true;

	public int secondaryStructure = 0;

	public int numResidues = 30;

	public GameObject[] polyArr;
	private int polyLength;

	public GameObject[] hbondBackbonePsPf;

	public JointDrive[] chainPhiJointDrives;
	private JointDrive[] chainPsiJointDrives;

	// Use this for initialization
	void Start()
	{
		//Debug.Log("LOAD FILE = " + LoadPhiPsiData("Assets/Data/253l_phi_psi.txt"));

		buildPolypeptideChain();

		// test: add arbitrary distance constraints
		//AddDistanceConstraint(polyArr[2], polyArr[12], 0.6f, 20);
		//AddDistanceConstraint(polyArr[15], polyArr[30], 0.8f, 20);
		//AddDistanceConstraint(polyArr[0], polyArr[36], 0.8f, 20);

		// placeholder: should be created and updated on tick
		//InvokeRepeating("UpdateDistanceConstraintGfx", 0, 0.05f);

		//Debug.Log("LOAD FILE = " + Load("Assets/Data/253l_phi_psi.txt"));
		//Debug.Log("LOAD FILE = " + Load("Assets/Data/1xda_phi_psi.txt")); 

	}

	void buildPolypeptideChain()
	{

		polyLength = numResidues * 3;
		polyArr = new GameObject[polyLength];
		hbondBackbonePsPf = new GameObject[numResidues];

		{
			// init
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
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 0, 0), transform);
					break;
				case 1:
					// Yrot = +69
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 69, 0), transform);
					break;
				case 2:
					// Yrot = +69 -64 = 5
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 5, 0), transform);
					break;
				case 3:
					// Yrot = +69 -64 +58 = 63
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 63, 0), transform);
					break;
				case 4:
					// Yrot = +69 -64 +58 -69 = -6
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, -6, 0), transform);
					break;
				case 5:
					// Yrot = +69 -64 +58 -69 +64 = 58
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 58, 0), transform);
					break;

			}

			float scaleVDW = 1.0f;
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


			polyArr[i].name = ((i / 3)).ToString() + "_" + polyArr[i].name;

			SetRbDrag(polyArr[i]);
			SetCollidersGameObject(polyArr[i]);

			if (i > 0)
			{
				//AddBackboneConstraint(polyArr[i - 1], polyArr[i]);
				AddBackboneTopologyConstraint(i);
			}

		}

		InitBackboneHbondConstraints();
	}

	void SetRbDrag(GameObject go)
	{
		// empirical values which seem to behave well
		go.GetComponent<Rigidbody>().drag = 5;
		go.GetComponent<Rigidbody>().angularDrag = 1;
	}

	void SetCollidersGameObject(GameObject go)
	{
		var colliders = go.GetComponentsInChildren<Collider>();
		foreach (var col in colliders)
		{
			col.isTrigger = !useColliders;
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


		float bondLengthPeptide = 1.33f;
		float bondLengthAmideCalpha = 1.46f;
		float bondLengthCalphaCarbonyl = 1.51f;

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
			cj.targetRotation = Quaternion.Euler(180 + 57, 0, 0); // alpha helix phi -57
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
			cj.targetRotation = Quaternion.Euler(180 + 47, 0, 0); // alpha helix psi -47
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
			InitBackboneHbondConstraint(GetAmideForResidue(resid));
		}
	}

	void InitBackboneHbondConstraint(GameObject donorGO)
	{
		SpringJoint sjHbond = donorGO.AddComponent(typeof(SpringJoint)) as SpringJoint;
		sjHbond.connectedBody = null;
		sjHbond.autoConfigureConnectedAnchor = false;

		{
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
		}

		SwitchOffBackboneHbondConstraint(donorGO);

		// scale joint parameters to PolyPepBuilder and Amide_pf

		float scale = gameObject.transform.localScale.x * donorGO.transform.localScale.x;
		float HBondLength = 2.0f;

		sjHbond.minDistance = HBondLength * scale;
		sjHbond.maxDistance = HBondLength * scale;
		sjHbond.tolerance = HBondLength * scale * 0.1f;

		{
			// create particle system for hbond
			// assumes that only SpringJoint is the hbond constraint
			var hbond_sj = donorGO.GetComponent<SpringJoint>();
			var donorHLocation = donorGO.transform.TransformPoint(hbond_sj.anchor);

		
			Transform tf_H = donorGO.transform.Find("tf_H");

			int resid = GetResidForPolyArrGO(donorGO);
			hbondBackbonePsPf[resid] = Instantiate(hBondPsPf, donorHLocation, tf_H.rotation, tf_H); //HBond2_ps_pf
			hbondBackbonePsPf[resid].transform.localScale = transform.localScale;
			hbondBackbonePsPf[resid].name = "hb_backbone " + resid;
			//hBondPsPf.transform.localScale = transform.localScale;
		}

	}

	void UpdateHBondPSTransforms()
	{
		// aligns hbond particle systems to acceptor (if set)
		for (int resid = 0; resid < numResidues; resid++)
		{
			GameObject donorGO =  GetAmideForResidue(resid);
			var hbond_sj = donorGO.GetComponent<SpringJoint>();
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

			}
			else
			{
				// orient particle system with NH bond
				Transform donorN_amide = donorGO.transform.Find("N_amide");
				Vector3 relativeNHBond = donorHLocation - donorN_amide.position;
				Quaternion lookAwayFromN = Quaternion.LookRotation(relativeNHBond);
				hbondBackbonePsPf[resid].transform.rotation = lookAwayFromN;
			}

		}
	}

	void SwitchOffBackboneHbondConstraint(GameObject donorGO)
	{
		SpringJoint sjHbond = donorGO.GetComponent<SpringJoint>();
		sjHbond.spring = 0;
		sjHbond.damper = 0;
		sjHbond.enableCollision = false;
	}

	void SwitchOnBackboneHbondConstraint(GameObject donorGO)
	{
		SpringJoint sjHbond = donorGO.GetComponent<SpringJoint>();
		sjHbond.spring = 1000;
		sjHbond.damper = 5;
		sjHbond.enableCollision = true;
	}


	void SetAcceptorForBackboneHbondConstraint(GameObject donorGO, GameObject acceptorGO)
	{
		SpringJoint sjHbond = donorGO.GetComponent<SpringJoint>();

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

	void ClearAcceptorForBackboneHbondConstraint(GameObject donorGO)
	{
		SpringJoint sjHbond = donorGO.GetComponent<SpringJoint>();
		sjHbond.connectedBody = null;
	}

	void SetChainAlphaHelicalHBonds()
	{
		for (int i = 0; i < numResidues; i++)
		{
			var donorGO = GetAmideForResidue(i);
			if (i > 3)
			{
				GameObject acceptorGO = GetCarbonylForResidue(i - 4);
				SetAcceptorForBackboneHbondConstraint(donorGO, acceptorGO);
				//Debug.Log(i + " " + donorGO + " " + acceptorGO);
			}
		}
	}

	void SetChain310HelicalHBonds()
	{
		for (int i = 0; i < numResidues; i++)
		{
			var donorGO = GetAmideForResidue(i);
			if (i > 2)
			{
				GameObject acceptorGO = GetCarbonylForResidue(i - 3);
				SetAcceptorForBackboneHbondConstraint(donorGO, acceptorGO);
				//Debug.Log(i + " " + donorGO + " " + acceptorGO);
			}
		}
	}

	void ClearChainHBonds()
	{
		for (int i = 0; i < numResidues; i++)
		{
			var donorGO = GetAmideForResidue(i);
			SwitchOffBackboneHbondConstraint(donorGO);
			ClearAcceptorForBackboneHbondConstraint(donorGO);
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
		lr.SetWidth(0.02f, 0.02f);
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


	void UpdateSecondaryStructureSwitch()
	{
		int numSecondaryStructures = 3;

		if (Input.GetKeyDown(KeyCode.P))
		{
			secondaryStructure++;

			if (secondaryStructure > numSecondaryStructures)
			{
				secondaryStructure = 0;
			}
			SetAllPhiPsi();
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			secondaryStructure--;

			if (secondaryStructure < 0)
			{
				secondaryStructure = numSecondaryStructures;
			}
			SetAllPhiPsi();
		}
	}

	void SetAllPhiPsi()
	{
		float phi = 0.0f;
		float psi = 0.0f;

		Debug.Log("Secondary Structure " + secondaryStructure);

		switch (secondaryStructure)
		{
			case 0:
				ClearChainHBonds();
				break;
			case 1:
				//alpha helix
				phi = -57.0f;
				psi = -47.0f;
				SetChainAlphaHelicalHBonds();
				break;
			case 2:
				//310 helix
				phi = -74.0f;
				psi = -4.0f;
				SetChain310HelicalHBonds();
				break;
			case 3:
				//beta sheet
				phi = -139.0f;
				psi = 135.0f;
				ClearChainHBonds();
				break;
		}

		for (int resid = 0; resid < numResidues; resid++)
		{
			SetPhiPsiForResidue(resid, phi, psi);
		}
	}

	void SetPhiPsiForResidue(int resid, float phi, float psi)
	{
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
	void SetColliders()
	{
		for (int i = 0; i < polyLength; i++)
		{
			SetCollidersGameObject(polyArr[i]);
		}
	}


	void UpdateColliderSwitch()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			useColliders = !useColliders;
			SetColliders();
			Debug.Log("Colliders " + useColliders);
		}
	}

	void UpdatePhiPsiDrives()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{

			for (int i = 0; i < numResidues; i++)
			{
				chainPhiJointDrives[i].maximumForce = 100.0f;
				chainPhiJointDrives[i].positionDamper = 1;
				chainPhiJointDrives[i].positionSpring = 100.0f;

				chainPsiJointDrives[i].maximumForce = 100.0f;
				chainPsiJointDrives[i].positionDamper = 1;
				chainPsiJointDrives[i].positionSpring = 100.0f;

				UpdatePhiPsiDriveParamForResidue(i);
			}
			Debug.Log("PhiPsi Drive = ON ");
		}

		if (Input.GetKeyDown(KeyCode.V))
		{

			for (int i = 0; i < numResidues; i++)
			{
				chainPhiJointDrives[i].maximumForce = 0.0f;
				chainPhiJointDrives[i].positionDamper = 0;
				chainPhiJointDrives[i].positionSpring = 0.0f;

				chainPsiJointDrives[i].maximumForce = 0.0f;
				chainPsiJointDrives[i].positionDamper = 0;
				chainPsiJointDrives[i].positionSpring = 0.0f;

				UpdatePhiPsiDriveParamForResidue(i);
			}
			Debug.Log("PhiPsi Drive = OFF ");
		}
	}


	void UpdateHBondSprings()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			for (int resid = 0; resid < numResidues; resid++)
			{
				GameObject donorGO = GetAmideForResidue(resid);
				var hbond_sj = donorGO.GetComponent<SpringJoint>();
				var donorHLocation = donorGO.transform.TransformPoint(hbond_sj.anchor);

				if (hbond_sj.connectedBody != null)
				{
					SwitchOnBackboneHbondConstraint(donorGO);
					WakeResidRbs(resid);
				}
			}
			Debug.Log("HBond Springs = ON ");
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			for (int resid = 0; resid < numResidues; resid++)
			{
				GameObject donorGO = GetAmideForResidue(resid);
				SwitchOffBackboneHbondConstraint(donorGO);
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



	// Update is called once per frame
	void Update()
	{
		UpdateSecondaryStructureSwitch();
		UpdateColliderSwitch();
		UpdatePhiPsiDrives();
		//UpdateDistanceConstraintGfx();
		UpdateHBondPSTransforms();
		UpdateHBondSprings();
	}
}
