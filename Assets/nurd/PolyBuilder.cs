using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyBuilder: MonoBehaviour
{

	public GameObject monomerPrefab;


	public GameObject[] polyArr;
	private int polyLength = 50;

	// Use this for initialization
	void Start()
	{
		polyArr = new GameObject[polyLength];

		// offset from handling cube
		var offsetPositionBase = new Vector3(0f, -0.2f, 0f);
		// periodic offsets for polymer
		var offsetPositionUnit = new Vector3(0f, -0.2f, 0f);
		var offsetRotationUnit = Quaternion.Euler(0, 0, 0); //45

		for (int i = 0; i < polyLength; i++)
		{
			if (i == 0)
			{
				polyArr[i] = Instantiate(monomerPrefab, (transform.position + transform.TransformDirection(offsetPositionBase)), transform.rotation * offsetRotationUnit, transform);
			}
			else
			{
				Transform lastUnitTransform = polyArr[i - 1].transform;
				polyArr[i] = Instantiate(monomerPrefab, (lastUnitTransform.position + lastUnitTransform.TransformDirection(offsetPositionUnit)), lastUnitTransform.rotation * offsetRotationUnit, transform);
				AddChainConstraint(i);
			}
		}

		// test: add arbitrary distance constraints
		//AddDistanceConstraint(polyArr[2], polyArr[12], 0.6f, 20);
		//AddDistanceConstraint(polyArr[15], polyArr[30], 0.8f, 20);
		//AddDistanceConstraint(polyArr[0], polyArr[36], 0.8f, 20);

		// placeholder: should be created and updated on tick
		InvokeRepeating("UpdateDistanceConstraintGfx", 0, 0.05f);
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
		sjDist.maxDistance = distance/1.2f;
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
		lr.SetColors(color,color);
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

	// Update is called once per frame
	void Update()
	{


	}
}