using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideChainBuilder : MonoBehaviour {


	public GameObject Csp3_pf;
	public List<GameObject> SideChainList = new List<GameObject>();

	int sideChainLength = 20;

	public Vector3 posA = new Vector3(0f, 0f, 0f);
	public Vector3 posB = new Vector3(0f, 0f, 0f);
	public Vector3 posC = new Vector3(0f, 0f, 0f);

	// Use this for initialization
	void Start () {
		BuildSideChain();
	}

	void BuildSideChain()
	{
		for (int i = 0; i < sideChainLength; i++)
		{
			SideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity));
		}


		GameObject CBeta = SideChainList[0];
		GameObject CGamma = SideChainList[1];
		GameObject CDelta = SideChainList[2];
		GameObject CEpsilon = SideChainList[3];

		//CGamma.transform.position = CBeta.transform.Find("H_3").position;
		//CGamma.transform.LookAt(CBeta.transform.position);

		//CDelta.transform.position = CGamma.transform.Find("H_3").position;
		//CDelta.transform.LookAt(CGamma.transform.position);

		//CEpsilon.transform.position = CDelta.transform.Find("H_3").position;
		//CEpsilon.transform.LookAt(CDelta.transform.position);

		for (int j = 1; j < SideChainList.Count; j++)
		{
			int i = j - 1;
			SideChainList[j].transform.position = SideChainList[i].transform.Find("H_3").position;
			SideChainList[j].transform.LookAt(SideChainList[i].transform.position);

			// quaternion that moves from a->b ?
			//Quaternion relative = Quaternion.Inverse(a) * b;
			//Quaternion.Inverse()
			//Quaternion.Slerp()


			ConfigurableJoint cj = SideChainList[j].AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
			cj.connectedBody = SideChainList[i].GetComponent<Rigidbody>();

			cj.autoConfigureConnectedAnchor = true;
			//cj.connectedAnchor = new Vector3(0f, 0f, 0f);
			cj.xMotion = ConfigurableJointMotion.Locked;
			cj.yMotion = ConfigurableJointMotion.Locked;
			cj.zMotion = ConfigurableJointMotion.Locked;

			cj.angularXMotion = ConfigurableJointMotion.Locked;
			cj.angularYMotion = ConfigurableJointMotion.Locked;
			cj.angularZMotion = ConfigurableJointMotion.Free;

		}
	}



	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(posA, 0.04f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(posB, 0.04f);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(posC, 0.2f);
		//if (lastHit)
		//{
		//	Gizmos.color = Color.black;
		//	Gizmos.DrawWireSphere(myHitPos, 0.04f);
		//}

	}

	// Update is called once per frame
	void Update () {

	}
}
