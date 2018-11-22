using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideChainBuilder : MonoBehaviour {


	public GameObject Csp3_pf;
	//public List<GameObject> residue_cssideChainList = new List<GameObject>();

	int sideChainLength = 0;

	// debug vectors for gizmos
	public Vector3 posA = new Vector3(0f, 0f, 0f);
	public Vector3 posB = new Vector3(0f, 0f, 0f);
	public Vector3 posC = new Vector3(0f, 0f, 0f);

	// Use this for initialization
	void Start () {

	}

	public void BuildSideChain(GameObject ppb_go, int resid, string type)
	{
		PolyPepBuilder ppb_cs = ppb_go.GetComponent<PolyPepBuilder>();
		Residue residue_cs = ppb_cs.chainArr[resid].GetComponent<Residue>();

		// ought to do this in residue.cs ?
		residue_cs.sidechain = new GameObject(type);
		residue_cs.sidechain.transform.parent = residue_cs.transform;

		switch (type)
		{
			case "ALA":
				build_ALA(residue_cs);
				break;
			case "VAL":
				build_VAL(residue_cs);
				break;
			case "LEU":
				build_LEU(residue_cs);
				break;
			case "ILE":
				build_ILE(residue_cs);
				break;
			default:
				break;
		}

		residue_cs.DisableProxySideChain();
	}

	void build_ALA(Residue residue_cs)
	{
		sideChainLength = 1;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}
		GameObject _CB = residue_cs.sideChainList[0];
		_CB.name = "CB";

		{
			// Get CBeta position => R group
			Transform CB_tf = residue_cs.calpha_pf.transform.Find("tf_sidechain/R_sidechain");
			// Get CAlpha position
			Transform CA_tf = residue_cs.calpha_pf.transform;

			// Place and orient CBeta
			_CB.transform.position = CB_tf.position;
			_CB.transform.LookAt(CA_tf.position);
			AddConfigJointBond(_CB, residue_cs.calpha_pf);
		}
	}

	void build_VAL(Residue residue_cs)
	{
		sideChainLength = 3;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG1 = residue_cs.sideChainList[1];
		GameObject _CG2 = residue_cs.sideChainList[2];

		_CB.name = "CB";
		_CG1.name = "CG1";
		_CG2.name = "CG2";

		{
			// Get CBeta position => R group
			Transform CB_tf = residue_cs.calpha_pf.transform.Find("tf_sidechain/R_sidechain");
			// Get CAlpha position
			Transform CA_tf = residue_cs.calpha_pf.transform;

			// Place and orient CBeta
			_CB.transform.position = CB_tf.position;
			_CB.transform.LookAt(CA_tf.position);
			AddConfigJointBond(_CB, residue_cs.calpha_pf);
		}
		{
			_CG1.transform.position = _CB.transform.Find("H_3").position;
			_CG1.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_CG1, _CB);
		}
		{
			_CG2.transform.position = _CB.transform.Find("H_2").position;
			_CG2.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_CG2, _CB);
		}

	}

	void build_LEU(Residue residue_cs)
	{
		sideChainLength = 4;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD1 = residue_cs.sideChainList[2];
		GameObject _CD2 = residue_cs.sideChainList[3];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD1.name = "CD1";
		_CD2.name = "CD2";

		{
			// Get CBeta position => R group
			Transform CB_tf = residue_cs.calpha_pf.transform.Find("tf_sidechain/R_sidechain");
			// Get CAlpha position
			Transform CA_tf = residue_cs.calpha_pf.transform;

			// Place and orient CBeta
			_CB.transform.position = CB_tf.position;
			_CB.transform.LookAt(CA_tf.position);
			AddConfigJointBond(_CB, residue_cs.calpha_pf);
		}
		{
			_CG.transform.position = _CB.transform.Find("H_3").position;
			_CG.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_CG, _CB);
		}
		{
			_CD1.transform.position = _CG.transform.Find("H_3").position;
			_CD1.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD1, _CG);
		}
		{
			_CD2.transform.position = _CG.transform.Find("H_2").position;
			_CD2.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD2, _CG);
		}

	}

	void build_ILE(Residue residue_cs)
	{
		sideChainLength = 4;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}

		// ILE has counterintuitive atom nomenclature - need to check DGD

		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG1 = residue_cs.sideChainList[1];
		GameObject _CG2 = residue_cs.sideChainList[2];
		GameObject _CD1 = residue_cs.sideChainList[3];

		_CB.name = "CB";
		_CG1.name = "CG1";
		_CG2.name = "CG2";
		_CD1.name = "CD1";

		{
			// Get CBeta position => R group
			Transform CB_tf = residue_cs.calpha_pf.transform.Find("tf_sidechain/R_sidechain");
			// Get CAlpha position
			Transform CA_tf = residue_cs.calpha_pf.transform;

			// Place and orient CBeta
			_CB.transform.position = CB_tf.position;
			_CB.transform.LookAt(CA_tf.position);
			AddConfigJointBond(_CB, residue_cs.calpha_pf);
		}
		{
			_CG1.transform.position = _CB.transform.Find("H_3").position;
			_CG1.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_CG1, _CB);
		}
		{
			_CG2.transform.position = _CB.transform.Find("H_2").position;
			_CG2.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_CG2, _CB);
		}
		{
			_CD1.transform.position = _CG1.transform.Find("H_3").position;
			_CD1.transform.LookAt(_CG1.transform.position);
			AddConfigJointBond(_CD1, _CG1);
		}

	}
	void AddConfigJointBond(GameObject go1, GameObject g02)
	{
		ConfigurableJoint cj = go1.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
		cj.connectedBody = g02.GetComponent<Rigidbody>();

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
