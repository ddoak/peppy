using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyPepBuilder : MonoBehaviour {


	public GameObject amidePf;
	public GameObject calphaPf;
	public GameObject carbonylPf;

	public bool setAlphaPhiPsi = true;
	public bool useColliders = true;


	public GameObject[] polyArr;

	private int numResidues = 30;
	private int polyLength;

	// Use this for initialization
	void Start()
	{
		polyLength = numResidues * 3;
		polyArr = new GameObject[polyLength];

		// offset from handling cube
		var offsetPositionBase = new Vector3(0.5f, 0f, 0f);
		// periodic offsets for polymer
		var offsetPositionUnit = new Vector3(0.2f, 0f, 0f);
		var offsetRotationUnit = Quaternion.Euler(0, 0, 0); //45

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
				lastUnitTransform = polyArr[i-1].transform;
			}


			int id = i % 6;
			//Debug.Log("polyArr" + i + " " + id);


			//
			// id ==  0   1   2   3   4   5
			//
			//            R       H       O
			//        N---C---C---N---C---C
			//        H       O       R
			//

			switch (id)
			{
				case 0:
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * offsetRotationUnit, transform);
					break;
				case 1:
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 69, 0), transform);
					break;
				case 2:
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 5, 0), transform);
					break;
				case 3:
					polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 63, 0), transform);
					break;
				case 4:
					polyArr[i] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, -6, 0), transform);
					break;
				case 5:
					polyArr[i] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 58, 0), transform);
					break;

			}

			float scaleVDW = 0.75f;
			float radiusN = 1.0f;
			float radiusC = 1.0f;
			float radiusO = 1.0f;
			float radiusH = 0.75f;
			float radiusR = 1.25f;

			Transform [] allChildren = GetComponentsInChildren<Transform>();
			foreach ( Transform child in allChildren)
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


			polyArr[i].name = ((i/3)+1).ToString() + "_" + polyArr[i].name;

			SetRbDrag(polyArr[i]);
			SetCollidersGameObject(polyArr[i]);




			if (i > 0)
			{
				AddBackboneConstraint(polyArr[i - 1], polyArr[i]);
			}

		}

		//test: alpha helical hbonds
		AddAlphaHelicalHbondConstraints();

		// test: add arbitrary distance constraints
		//AddDistanceConstraint(polyArr[2], polyArr[12], 0.6f, 20);
		//AddDistanceConstraint(polyArr[15], polyArr[30], 0.8f, 20);
		//AddDistanceConstraint(polyArr[0], polyArr[36], 0.8f, 20);

		// placeholder: should be created and updated on tick
		//InvokeRepeating("UpdateDistanceConstraintGfx", 0, 0.05f);
	}

	void SetRbDrag(GameObject go)
	{
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

	void AddChainConstraint(int pos)
	{
		float offsetPolyChain = 0.09f;
		SpringJoint sjChain = polyArr[pos].AddComponent(typeof(SpringJoint)) as SpringJoint;
		sjChain.connectedBody = polyArr[pos - 1].GetComponent<Rigidbody>();
		sjChain.anchor = new Vector3(0f, offsetPolyChain, 0f);
		sjChain.autoConfigureConnectedAnchor = false;
		sjChain.connectedAnchor = new Vector3(0f, -offsetPolyChain, 0f);
		sjChain.spring = 1000;
		sjChain.enableCollision = true;
		sjChain.damper = 50;
		sjChain.minDistance = 0.01f;
		sjChain.tolerance = 0.01f;
		sjChain.tag = "chain";
	}

	void AddBackboneConstraint(GameObject go1, GameObject go2)
	{
		//return;

		ConfigurableJoint cj = go1.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
		cj.connectedBody = go2.GetComponent<Rigidbody>();
		if (go1.tag == "amide")
		{
			cj.anchor = new Vector3(1.46f, 0f, 0f);
		}
		else if (go1.tag == "calpha")
		{
			cj.anchor = new Vector3(1.51f, 0f, 0f);
		}
		else if (go1.tag == "carbonyl")
		{
			cj.anchor = new Vector3(1.33f, 0f, 0f);
		}
		cj.autoConfigureConnectedAnchor = false;
		cj.connectedAnchor = new Vector3(0f, 0f, 0f);
		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		cj.zMotion = ConfigurableJointMotion.Locked;
		if (go1.tag == "amide" || go1.tag == "calpha")
		{
			cj.angularXMotion = ConfigurableJointMotion.Free;
			cj.angularXDrive = new JointDrive
			{
				positionSpring = 20.0f,
				positionDamper = 1,
				maximumForce = 0.1f // 10.0f
			};
			if (go1.tag == "amide")
			{
				cj.targetRotation = Quaternion.Euler(180 + 57, 0, 0); // alpha helix phi -57
			}
			else if (go1.tag == "calpha")
			{
				cj.targetRotation = Quaternion.Euler(180 + 47, 0, 0); // alpha helix psi -47
			}
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
		for (int resid = (numResidues - 1); resid > 4; resid --)
		{
			AddBackboneHbondConstraint(GetAmideForResidue(resid), GetCarbonylForResidue(resid - 4));
		}
	}

	void AddBackboneHbondConstraint(GameObject donorGO, GameObject acceptorGO)
	{
		SpringJoint sjHbond = donorGO.AddComponent(typeof(SpringJoint)) as SpringJoint;
		sjHbond.connectedBody = acceptorGO.GetComponent<Rigidbody>();
		sjHbond.autoConfigureConnectedAnchor = false;
		float axisRotOffset = -90f;
		float thetaAmide = (float)((Mathf.Deg2Rad * ((122 + 119.5) + axisRotOffset)) * -1);
		float NHBondLength = 1.0f;
		sjHbond.anchor = new Vector3(Mathf.Sin(thetaAmide)* NHBondLength, 0f, Mathf.Cos(thetaAmide) * NHBondLength);
		float thetaCarbonyl = (float)((Mathf.Deg2Rad * (123.5 + axisRotOffset)) * -1);
		float COBondLength = 1.24f;
		sjHbond.connectedAnchor = new Vector3(Mathf.Sin(thetaCarbonyl) * COBondLength, 0f, Mathf.Cos(thetaCarbonyl) * COBondLength);
		sjHbond.spring = 1000;
		sjHbond.damper = 5;
		sjHbond.enableCollision = true;

		// scale o




		float scale = gameObject.transform.localScale.x * donorGO.transform.localScale.x;
		float HBondLength = 2.0f;

		sjHbond.minDistance =  HBondLength * scale;
		sjHbond.maxDistance = HBondLength * scale;
		sjHbond.tolerance = HBondLength * scale * 0.1f;
	}


	GameObject GetAmideForResidue (int residue)
	{
		return (polyArr[residue * 3]);
	}

	GameObject GetCarbonylForResidue(int residue)
	{
		return (polyArr[(residue * 3) + 2]);
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
		// iterates through poly chain - could just pull out springJoints with GO tags
		for (int i = 0; i < polyLength; i++)
		{
			var springs = polyArr[i].GetComponents<SpringJoint>();
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

	void SetBackboneDihedrals()
	{
		int phi;
		int psi;

		if (setAlphaPhiPsi)
		{
			phi = -57;
			psi = -47;
		}
		else
		{
			phi = -139;
			psi = 135;
		}

		for (int i = 0; i < polyLength; i++)
		{
			if (polyArr[i].tag == "amide")
			{
				var cj = polyArr[i].GetComponent<ConfigurableJoint>();
				cj.targetRotation = Quaternion.Euler(180 - phi, 0, 0); 

			}
			else if (polyArr[i].tag == "calpha")
			{
				var cj = polyArr[i].GetComponent<ConfigurableJoint>();
				cj.targetRotation = Quaternion.Euler(180 - psi, 0, 0);
			}
		}
	}

	void SetColliders()
	{
		for (int i = 0; i < polyLength; i++)
		{
			SetCollidersGameObject(polyArr[i]);
		}
	}

	void UpdateSecondaryStructureSwitch()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			setAlphaPhiPsi = !setAlphaPhiPsi;
			SetBackboneDihedrals();
			Debug.Log("Secondary Structure " + setAlphaPhiPsi);
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

	// Update is called once per frame
	void Update()
	{
		UpdateSecondaryStructureSwitch();
		UpdateColliderSwitch();
		UpdateDistanceConstraintGfx();
	}
}
