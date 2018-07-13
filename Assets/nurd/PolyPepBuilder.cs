using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyPepBuilder : MonoBehaviour {


	public GameObject amidePf;
	public GameObject calphaPf;
	public GameObject carbonylPf;

	public bool setAlphaPhiPsi = false;


	public GameObject[] polyArr;
	private int polyLength = 10;

	// Use this for initialization
	void Start()
	{
		polyArr = new GameObject[polyLength*6];

		// offset from handling cube
		var offsetPositionBase = new Vector3(0.5f, 0f, 0f);
		// periodic offsets for polymer
		var offsetPositionUnit = new Vector3(0.2f, 0f, 0f);
		var offsetRotationUnit = Quaternion.Euler(0, 0, 0); //45

		for (int i = 0; i < polyLength*6; i=i+6)
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

				polyArr[i] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * offsetRotationUnit, transform);
				SetRbDrag(polyArr[i]);

			if (i > 0)
			{
				AddBackboneConstraint(polyArr[i - 1], polyArr[i]);
			}


			lastUnitTransform = polyArr[i].transform;
			polyArr[i + 1] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 69, 0), transform);
			AddBackboneConstraint(polyArr[i], polyArr[i + 1]);
			SetRbDrag(polyArr[i + 1]);

			lastUnitTransform = polyArr[i + 1].transform;
			polyArr[i + 2] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 5, 0), transform);
			AddBackboneConstraint(polyArr[i + 1], polyArr[i + 2]);
			SetRbDrag(polyArr[i + 2]);

			lastUnitTransform = polyArr[i + 2].transform;
			polyArr[i + 3] = Instantiate(amidePf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, 63, 0), transform);
			AddBackboneConstraint(polyArr[i + 2], polyArr[i + 3]);
			SetRbDrag(polyArr[i + 3]);

			lastUnitTransform = polyArr[i + 3].transform;
			polyArr[i + 4] = Instantiate(calphaPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(180, -6, 0), transform);
			AddBackboneConstraint(polyArr[i + 3], polyArr[i + 4]);
			SetRbDrag(polyArr[i + 4]);

			lastUnitTransform = polyArr[i + 4].transform;
			polyArr[i + 5] = Instantiate(carbonylPf, (lastUnitTransform.position + transform.TransformDirection(offsetPositionUnit)), transform.rotation * Quaternion.Euler(0, 58, 0), transform);
			AddBackboneConstraint(polyArr[i + 4], polyArr[i + 5]);
			SetRbDrag(polyArr[i + 5]);

			//AddChainConstraint(i);
		}

		// test: add arbitrary distance constraints
		//AddDistanceConstraint(polyArr[2], polyArr[12], 0.6f, 20);
		//AddDistanceConstraint(polyArr[15], polyArr[30], 0.8f, 20);
		//AddDistanceConstraint(polyArr[0], polyArr[36], 0.8f, 20);

		// placeholder: should be created and updated on tick
		InvokeRepeating("UpdateDistanceConstraintGfx", 0, 0.05f);
	}

	void SetRbDrag(GameObject go)
	{
		go.GetComponent<Rigidbody>().drag = 1;
		go.GetComponent<Rigidbody>().angularDrag = 1;
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
				maximumForce = 10.0f
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

				if (s.tag == "dist")
				{
					var startPoint = polyArr[i].transform.position + polyArr[i].transform.TransformDirection(s.anchor);
					var endPoint = s.connectedBody.transform.position + s.connectedBody.transform.TransformDirection(s.connectedAnchor);

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

	void UpdateBackboneDihedrals()
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

		for (int i = 0; i < polyLength*6; i++)
		{
			if (polyArr[i].tag == "amide")
			{
				var cj = polyArr[i].GetComponent<ConfigurableJoint>();
				cj.targetRotation = Quaternion.Euler(180 - phi, 0, 0); 

			}
			else if (polyArr[i].tag == "calpha")
			{
				var cj = polyArr[i].GetComponent<ConfigurableJoint>();
				cj.targetRotation = Quaternion.Euler(180 - phi, 0, 0);
			}
		}
	}

	void UpdateSecStructToggle()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			setAlphaPhiPsi = !setAlphaPhiPsi;
			UpdateBackboneDihedrals();
		}
	}

	// Update is called once per frame
	void Update()
	{
		UpdateSecStructToggle();
	}
}
