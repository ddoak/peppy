using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideChainBuilder : MonoBehaviour {


	public GameObject Csp3_pf;
	public List<GameObject> SideChainList = new List<GameObject>();

	int sideChainLength = 4;

	public Vector3 posA = new Vector3(0f, 0f, 0f);
	public Vector3 posB = new Vector3(0f, 0f, 0f);
	public Vector3 posC = new Vector3(0f, 0f, 0f);

	// Use this for initialization
	void Start () {

		for (int i = 0; i < sideChainLength; i++)
		{
			SideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity));
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
