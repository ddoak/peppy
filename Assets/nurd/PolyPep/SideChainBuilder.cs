using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideChainBuilder : MonoBehaviour {


	public GameObject Csp3_pf;
	//public List<GameObject> residue_cssideChainList = new List<GameObject>();

	int sideChainLength = 5;

	public Vector3 posA = new Vector3(0f, 0f, 0f);
	public Vector3 posB = new Vector3(0f, 0f, 0f);
	public Vector3 posC = new Vector3(0f, 0f, 0f);

	// Use this for initialization
	void Start () {
		//BuildSideChain();
	}

	public void BuildSideChain(GameObject ppb_go, int resid)
	{
		PolyPepBuilder ppb_cs = ppb_go.GetComponent<PolyPepBuilder>();
		Residue residue_cs = ppb_cs.chainArr[resid].GetComponent<Residue>();

		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity));
		}


		//GameObject CBeta = SideChainList[0];
		//GameObject CGamma = SideChainList[1];
		//GameObject CDelta = SideChainList[2];
		//GameObject CEpsilon = SideChainList[3];

		//CGamma.transform.position = CBeta.transform.Find("H_3").position;
		//CGamma.transform.LookAt(CBeta.transform.position);

		//CDelta.transform.position = CGamma.transform.Find("H_3").position;
		//CDelta.transform.LookAt(CGamma.transform.position);

		//CEpsilon.transform.position = CDelta.transform.Find("H_3").position;
		//CEpsilon.transform.LookAt(CDelta.transform.position);

		Debug.Log(ppb_cs + " " + resid);
		Debug.Log(ppb_cs + " " + ppb_cs.numResidues);

	
		Rigidbody rb =  residue_cs.calpha_pf.GetComponent<Rigidbody>();

		Transform r = residue_cs.calpha_pf.transform.Find("tf_sidechain/R_sidechain");
		Transform ca = residue_cs.calpha_pf.transform;

		residue_cs.sideChainList[0].transform.position = r.position;
		residue_cs.sideChainList[0].transform.LookAt(ca.position);

		{
			ConfigurableJoint cj = residue_cs.sideChainList[0].AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
			cj.connectedBody = rb;

			// NOTE
			// in PolyPepBuilder.cs anchor and connected anchor are inverted
			// incorrect - but works because cj is used only for rotation
			// and rot direction is accounted for in code


			// orient config joint along bond axis (z = forward)
			// => Xrot for joint is along this axis
			cj.axis = Vector3.forward;

			// can use autoconfigure because geometry has been set up correctly ?
			cj.autoConfigureConnectedAnchor = true;

			cj.xMotion = ConfigurableJointMotion.Locked;
			cj.yMotion = ConfigurableJointMotion.Locked;
			cj.zMotion = ConfigurableJointMotion.Locked;

			cj.angularXMotion = ConfigurableJointMotion.Free;
			cj.angularYMotion = ConfigurableJointMotion.Locked;
			cj.angularZMotion = ConfigurableJointMotion.Locked;
		}

		for (int j = 1; j < residue_cs.sideChainList.Count; j++)
		{
			int i = j - 1;
			// place distal Csp3 on H_3 position
			residue_cs.sideChainList[j].transform.position = residue_cs.sideChainList[i].transform.Find("H_3").position;
			// orient distal Csp3 toward proximal Csp3
			residue_cs.sideChainList[j].transform.LookAt(residue_cs.sideChainList[i].transform.position);

			// quaternion that moves from a->b ?
			//Quaternion relative = Quaternion.Inverse(a) * b;
			//Quaternion.Inverse()
			//Quaternion.Slerp()


			ConfigurableJoint cj = residue_cs.sideChainList[j].AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
			cj.connectedBody = residue_cs.sideChainList[i].GetComponent<Rigidbody>();

			// NOTE
			// in PolyPepBuilder.cs anchor and connected anchor are inverted
			// incorrect - but works because cj is used only for rotation
			// and rot direction is accounted for in code


			// orient config joint along bond axis (z = forward)
			// => Xrot for joint is along this axis
			cj.axis = Vector3.forward;

			// can use autoconfigure because geometry has been set up correctly ?
			cj.autoConfigureConnectedAnchor = true;
			
			cj.xMotion = ConfigurableJointMotion.Locked;
			cj.yMotion = ConfigurableJointMotion.Locked;
			cj.zMotion = ConfigurableJointMotion.Locked;

			cj.angularXMotion = ConfigurableJointMotion.Free;
			cj.angularYMotion = ConfigurableJointMotion.Locked;
			cj.angularZMotion = ConfigurableJointMotion.Locked;

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
