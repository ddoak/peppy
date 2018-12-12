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

	public void DeleteSideChain(GameObject ppb_go, int resid)
	{
		PolyPepBuilder ppb_cs = ppb_go.GetComponent<PolyPepBuilder>();
		Residue residue_cs = ppb_cs.chainArr[resid].GetComponent<Residue>();
		if ( residue_cs.sideChainList.Count > 0)
		{
			foreach (GameObject _go in residue_cs.sideChainList)
			{
				Destroy(_go);
			}
		}
		residue_cs.sideChainList.Clear();
		//residue_cs.EnableProxySideChain();	
	}

	public void BuildSideChain(GameObject ppb_go, int resid, string type)
	{
		PolyPepBuilder ppb_cs = ppb_go.GetComponent<PolyPepBuilder>();
		Residue residue_cs = ppb_cs.chainArr[resid].GetComponent<Residue>();

		// ought to do this in residue.cs ?
		residue_cs.type = type;
		residue_cs.sidechain = new GameObject(resid + "_" + type);
		residue_cs.sidechain.transform.parent = residue_cs.transform;

		DeleteSideChain(ppb_go, resid);


		// set default atom type to sp3 - used in Awake() when instantiated
		Csp3_pf.GetComponent<Csp3>().atomType = "sp3";

		if (type == "XXX")
		{
			residue_cs.EnableProxySideChain();
		}
		else
		{
			residue_cs.DisableProxySideChain();

			switch (type)
			{
				case "ALA":
					Build_ALA(residue_cs);
					break;
				case "VAL":
					Build_VAL(residue_cs);
					break;
				case "LEU":
					Build_LEU(residue_cs);
					break;
				case "ILE":
					Build_ILE(residue_cs);
					break;
				case "MET":
					Build_MET(residue_cs);
					break;
				case "CYS":
					Build_CYS(residue_cs);
					break;
				case "SER":
					Build_SER(residue_cs);
					break;
				case "THR":
					Build_THR(residue_cs);
					break;
				case "LYS":
					Build_LYS(residue_cs);
					break;
				case "ASP":
					Build_ASP(residue_cs);
					break;
				case "GLU":
					Build_GLU(residue_cs);
					break;
				case "ASN":
					Build_ASN(residue_cs);
					break;
				case "GLN":
					Build_GLN(residue_cs);
					break;
				case "ARG":
					Build_ARG(residue_cs);
					break;
				case "PHE":
					Build_PHEorTYR(residue_cs, false);
					break;
				case "TYR":
					Build_PHEorTYR(residue_cs, true);
					break;
				case "PRO":
					Build_PRO(residue_cs);
					break;
				case "TEST":
					Build_TEST(residue_cs);
					break;
				case "DEV":
					Build_DEV(residue_cs);
					break;
				default:
					break;
			}
		}


	}

	void Build_ALA(Residue residue_cs)
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
		_CB.GetComponent<Csp3>().ConvertToCH3();
	}

	void Build_VAL(Residue residue_cs)
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

		_CB.GetComponent<Csp3>().ConvertToCH1();
		_CG1.GetComponent<Csp3>().ConvertToCH3();
		_CG2.GetComponent<Csp3>().ConvertToCH3();
	}

	void Build_LEU(Residue residue_cs)
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

		_CB.GetComponent<Csp3>().ConvertToCH2();

		_CG.GetComponent<Csp3>().ConvertToCH1();

		_CD1.GetComponent<Csp3>().ConvertToCH3();
		_CD2.GetComponent<Csp3>().ConvertToCH3();

	}

	void Build_ILE(Residue residue_cs)
	{
		sideChainLength = 4;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}

		// ILE has counterintuitive atom nomenclature - need to check DGD
		// CB is chiral - need to check DGD - checked!

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

		_CB.GetComponent<Csp3>().ConvertToCH1();

		_CG1.GetComponent<Csp3>().ConvertToCH2();

		_CG2.GetComponent<Csp3>().ConvertToCH3();
		_CD1.GetComponent<Csp3>().ConvertToCH3();

	}

	void Build_MET(Residue residue_cs)
	{
		sideChainLength = 4;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}

		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _SD = residue_cs.sideChainList[2];
		GameObject _CE = residue_cs.sideChainList[3];

		_CB.name = "CB";
		_CG.name = "CG";
		_SD.name = "SD";
		_CE.name = "CE";

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
			_SD.transform.position = _CG.transform.Find("H_3").position;
			_SD.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_SD, _CG);
		}
		{
			_CE.transform.position = _SD.transform.Find("H_3").position;
			_CE.transform.LookAt(_SD.transform.position);
			AddConfigJointBond(_CE, _SD);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertToCH2();
		_SD.GetComponent<Csp3>().ConvertToS();
		_CE.GetComponent<Csp3>().ConvertToCH3();
	}

	void Build_CYS(Residue residue_cs)
	{
		sideChainLength = 2;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _SG = residue_cs.sideChainList[1];

		_CB.name = "CB";
		_SG.name = "SG";

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
			_SG.transform.position = _CB.transform.Find("H_3").position;
			_SG.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_SG, _CB);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_SG.GetComponent<Csp3>().ConvertToSH();

	}

	void Build_SER(Residue residue_cs)
	{
		sideChainLength = 2;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _OG = residue_cs.sideChainList[1];

		_CB.name = "CB";
		_OG.name = "OG";

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
			_OG.transform.position = _CB.transform.Find("H_3").position;
			_OG.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_OG, _CB);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_OG.GetComponent<Csp3>().ConvertToOH();

	}

	void Build_THR(Residue residue_cs)
	{
		sideChainLength = 3;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}

		// CB is chiral - need to check DGD - checked!

		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _OG = residue_cs.sideChainList[2];

		_CB.name = "CB";
		_CG.name = "CG";
		_OG.name = "OG";

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
			_CG.transform.position = _CB.transform.Find("H_2").position;
			_CG.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_CG, _CB);
		}
		{
			_OG.transform.position = _CB.transform.Find("H_3").position;
			_OG.transform.LookAt(_CB.transform.position);
			AddConfigJointBond(_OG, _CB);
		}

		_CB.GetComponent<Csp3>().ConvertToCH1();
		_CG.GetComponent<Csp3>().ConvertToCH3();
		_OG.GetComponent<Csp3>().ConvertToOH();
	}

	void Build_LYS(Residue residue_cs)
	{
		sideChainLength = 5;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}

		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD = residue_cs.sideChainList[2];
		GameObject _CE = residue_cs.sideChainList[3];
		GameObject _NF = residue_cs.sideChainList[4];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD.name = "CD";
		_CE.name = "CE";
		_NF.name = "NF";

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
			_CD.transform.position = _CG.transform.Find("H_3").position;
			_CD.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD, _CG);
		}
		{
			_CE.transform.position = _CD.transform.Find("H_3").position;
			_CE.transform.LookAt(_CD.transform.position);
			AddConfigJointBond(_CE, _CD);
		}
		{
			_NF.transform.position = _CE.transform.Find("H_3").position;
			_NF.transform.LookAt(_CE.transform.position);
			AddConfigJointBond(_NF, _CE);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertToCH2();
		_CD.GetComponent<Csp3>().ConvertToCH2();
		_CE.GetComponent<Csp3>().ConvertToCH2();
		_NF.GetComponent<Csp3>().ConvertToNH3();
	}

	void Build_ASP(Residue residue_cs)
	{
		sideChainLength = 2;
		for (int i = 0; i < sideChainLength; i++)
		{
			if (i == 0)
			{
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
			if (i == 1)
			{
				Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];

		_CB.name = "CB";
		_CG.name = "CG";

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

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertSp2ToCOO();

	}

	void Build_GLU(Residue residue_cs)
	{
		sideChainLength = 3;
		for (int i = 0; i < sideChainLength; i++)
		{
			if (i == 0 || i == 1)
			{
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
			if (i == 2)
			{
				Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD = residue_cs.sideChainList[2];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD.name = "CD";

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
			_CD.transform.position = _CG.transform.Find("H_3").position;
			_CD.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD, _CG);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertToCH2();
		_CD.GetComponent<Csp3>().ConvertSp2ToCOO();

	}

	void Build_ASN(Residue residue_cs)
	{
		sideChainLength = 3;
		for (int i = 0; i < sideChainLength; i++)
		{
			if (i == 0)
			{
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
			if (i == 1 || i == 2)
			{
				Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _ND = residue_cs.sideChainList[2];

		_CB.name = "CB";
		_CG.name = "CG";
		_ND.name = "ND";

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
			_ND.transform.position = _CG.transform.Find("H_2").position; //build on H_2 position -> sp2!
			_ND.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_ND, _CG);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertSp2ToCO();
		_ND.GetComponent<Csp3>().ConvertSp2ToNH2();

	}

	void Build_GLN(Residue residue_cs)
	{
		sideChainLength = 4;
		for (int i = 0; i < sideChainLength; i++)
		{
			if (i == 0 || i == 1)
			{
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
			if (i == 2 || i == 3)
			{
				Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD = residue_cs.sideChainList[2];
		GameObject _NE = residue_cs.sideChainList[3];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD.name = "CD";
		_NE.name = "NE";

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
			_CD.transform.position = _CG.transform.Find("H_3").position;
			_CD.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD, _CG);
		}
		{
			_NE.transform.position = _CD.transform.Find("H_2").position; //build on H_2 position -> sp2!
			_NE.transform.LookAt(_CD.transform.position);
			AddConfigJointBond(_NE, _CD);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertToCH2();
		_CD.GetComponent<Csp3>().ConvertSp2ToCO();
		_NE.GetComponent<Csp3>().ConvertSp2ToNH2();

	}

	void Build_ARG(Residue residue_cs)
	{
		sideChainLength = 7;
		for (int i = 0; i < sideChainLength; i++)
		{
			if (i < 3)
			{
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
			if (i >= 3)
			{
				Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
		}

		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD = residue_cs.sideChainList[2];
		GameObject _NE = residue_cs.sideChainList[3];
		GameObject _CZ = residue_cs.sideChainList[4];
		GameObject _NH1 = residue_cs.sideChainList[5];
		GameObject _NH2 = residue_cs.sideChainList[6];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD.name = "CD";
		_NE.name = "NE";
		_CZ.name = "CZ";
		_NH1.name = "NH1";
		_NH2.name = "NH2";

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
			_CD.transform.position = _CG.transform.Find("H_3").position;
			_CD.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD, _CG);
		}
		{
			_NE.transform.position = _CD.transform.Find("H_3").position;
			_NE.transform.LookAt(_CD.transform.position);
			AddConfigJointBond(_NE, _CD);
		}
		{
			_CZ.transform.position = _NE.transform.Find("H_2").position;
			_CZ.transform.LookAt(_NE.transform.position);
			AddConfigJointBond(_CZ, _NE);
		}
		{
			_NH1.transform.position = _CZ.transform.Find("H_1").position;
			_NH1.transform.LookAt(_CZ.transform.position);
			AddConfigJointBond(_NH1, _CZ);
		}
		{
			_NH2.transform.position = _CZ.transform.Find("H_2").position;
			_NH2.transform.LookAt(_CZ.transform.position);
			AddConfigJointBond(_NH2, _CZ);
		}


		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertToCH2();
		_CD.GetComponent<Csp3>().ConvertToCH2();
		_NE.GetComponent<Csp3>().ConvertSp2ToNH();
		_CZ.GetComponent<Csp3>().ConvertSp2ToC(false);
		_NH1.GetComponent<Csp3>().ConvertSp2ToNH2();
		_NH2.GetComponent<Csp3>().ConvertSp2ToNH2();
	}

	void Build_PHEorTYR(Residue residue_cs, bool makeTYR)
	{
		sideChainLength = 7;
		for (int i = 0; i < sideChainLength; i++)
		{
			if (i == 0)
			{
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
			if (i > 0)
			{
				Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
				residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
			}
		}
		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD1 = residue_cs.sideChainList[2];
		GameObject _CD2 = residue_cs.sideChainList[3];
		GameObject _CE1 = residue_cs.sideChainList[4];
		GameObject _CE2 = residue_cs.sideChainList[5];
		GameObject _CZ = residue_cs.sideChainList[6];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD1.name = "CD1";
		_CD2.name = "CD2";
		_CE1.name = "CE1";
		_CE2.name = "CE2";
		_CZ.name = "CZ";

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

		//{
		//	_CG.transform.position = _CB.transform.Find("H_3").position;
		//	_CG.transform.LookAt(_CB.transform.position);
		//	AddConfigJointBond(_CG, _CB);
		//}

		// build ring without attaching to CB - to maintain planar geometry
		{
			_CD1.transform.position = _CG.transform.Find("H_2").position; //build on H_1 position -> sp2!
			_CD1.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD1, _CG);
		}
		{
			_CE1.transform.position = _CD1.transform.Find("H_2").position; //build on H_2 position -> sp2!
			_CE1.transform.LookAt(_CD1.transform.position);
			AddConfigJointBond(_CE1, _CD1);
		}
		{
			_CZ.transform.position = _CE1.transform.Find("H_2").position; //build on H_2 position -> sp2!
			_CZ.transform.LookAt(_CE1.transform.position);
			AddConfigJointBond(_CZ, _CE1);
		}
		{
			_CE2.transform.position = _CZ.transform.Find("H_2").position; //build on H_2 position -> sp2!
			_CE2.transform.LookAt(_CZ.transform.position);
			AddConfigJointBond(_CE2, _CZ);
		}
		{
			_CD2.transform.position = _CE2.transform.Find("H_2").position; //build on H_2 position -> sp2!
			_CD2.transform.LookAt(_CE2.transform.position);
			AddConfigJointBond(_CD2, _CE2);
			AddConfigJointBond(_CG, _CD2);
		}

		if (makeTYR)
		{
			Csp3_pf.GetComponent<Csp3>().atomType = "sp3";
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * 6 * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));

			GameObject _OH = residue_cs.sideChainList[7];
			_OH.name = "OH";

			{
				_OH.transform.position = _CZ.transform.Find("H_1").position; //build on H_1 position -> sp2!
				_OH.transform.LookAt(_CZ.transform.position);
				AddConfigJointBond(_OH, _CZ);
			}
			_OH.GetComponent<Csp3>().ConvertToOH();
		}

		{
			_CG.transform.position = _CB.transform.Find("H_3").position;
			//_CG.transform.LookAt(_CB.transform.position);

			//{
			//	Vector3 v = B - A;
			//	Quaternion q = Quaternion.FromToRotation(transform.right, v);
			//	transform.rotation = q * transform.rotation;
			//}
			{
				// this is the general solution for aligning atoms along bonds
				// took until making PHE to work it out properly

				Vector3 CGtoCB = _CB.transform.position - _CG.transform.position;
				Vector3 CGbond = _CG.transform.Find("H_1").position - _CG.transform.position;
				Quaternion q = Quaternion.FromToRotation(CGbond, CGtoCB);

				_CG.transform.rotation = q * _CG.transform.rotation; // not commutative

			}
			AddConfigJointBond(_CG, _CB);
		}

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertSp2ToC(true);
		_CD1.GetComponent<Csp3>().ConvertSp2ToCH();
		_CE1.GetComponent<Csp3>().ConvertSp2ToCH();
		if (makeTYR)
		{
			_CZ.GetComponent<Csp3>().ConvertSp2ToC(false); 
		}
		else
		{
			_CZ.GetComponent<Csp3>().ConvertSp2ToCH();
		}
		
		_CE2.GetComponent<Csp3>().ConvertSp2ToCH();
		_CD2.GetComponent<Csp3>().ConvertSp2ToCH();


	}

	void Build_PRO(Residue residue_cs)
	{
		sideChainLength = 3;
		for (int i = 0; i < sideChainLength; i++)
		{
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}

		GameObject _CB = residue_cs.sideChainList[0];
		GameObject _CG = residue_cs.sideChainList[1];
		GameObject _CD = residue_cs.sideChainList[2];

		_CB.name = "CB";
		_CG.name = "CG";
		_CD.name = "CD";
	
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
			_CD.transform.position = _CG.transform.Find("H_3").position;
			_CD.transform.LookAt(_CG.transform.position);
			AddConfigJointBond(_CD, _CG);
		}

		{
			GameObject amide = residue_cs.amide_pf;
			Transform HN = amide.transform.Find("tf_H/H_amide");

			Vector3 NtoH = HN.position - amide.transform.position;

			HN.GetComponent<Renderer>().enabled = false;
			HN.GetComponent<Collider>().enabled = false;

			_CD.transform.position = amide.transform.position + (1.6f * NtoH); 

			{
				// this is the general solution for aligning atoms along bonds
				// took until making PHE to work it out properly

				Vector3 CDtoAmideN = amide.transform.position - _CD.transform.position;
				Vector3 CDNbond = _CD.transform.Find("H_3").position - _CD.transform.position;
				Quaternion q = Quaternion.FromToRotation(CDNbond, CDtoAmideN);

				_CD.transform.rotation = q * _CD.transform.rotation; // not commutative

			}

			AddConfigJointBondSlack(_CD, residue_cs.amide_pf);

		}
		

		_CB.GetComponent<Csp3>().ConvertToCH2();
		_CG.GetComponent<Csp3>().ConvertToCH2();
		_CD.GetComponent<Csp3>().ConvertToCH2();

	}

	void Build_TEST(Residue residue_cs)
	{
		sideChainLength = 1;
		for (int i = 0; i < sideChainLength; i++)
		{
			Csp3_pf.GetComponent<Csp3>().atomType = "none"; // dev:
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
		_CB.GetComponent<Csp3>().ConvertToCH3();
	}

	void Build_DEV(Residue residue_cs)
	{
		sideChainLength = 1;
		for (int i = 0; i < sideChainLength; i++)
		{
			Csp3_pf.GetComponent<Csp3>().atomType = "sp2";
			residue_cs.sideChainList.Add(Instantiate(Csp3_pf, transform.position + (transform.right * i * 0.6f), Quaternion.identity, residue_cs.sidechain.transform));
		}
		GameObject _CB = residue_cs.sideChainList[0];
		_CB.name = "CB";

		//{
		//	// Get CBeta position => R group
		//	Transform CB_tf = residue_cs.calpha_pf.transform.Find("tf_sidechain/R_sidechain");
		//	// Get CAlpha position
		//	Transform CA_tf = residue_cs.calpha_pf.transform;

		//	// Place and orient CBeta
		//	_CB.transform.position = CB_tf.position;
		//	_CB.transform.LookAt(CA_tf.position);
		//	AddConfigJointBond(_CB, residue_cs.calpha_pf);
		//}
		//_CB.GetComponent<Csp3>().ConvertToCH3();
	}

	void AddConfigJointBond(GameObject go1, GameObject go2)
	{
		ConfigurableJoint cj = go1.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
		cj.connectedBody = go2.GetComponent<Rigidbody>();

		// NOTE
		// in PolyPepBuilder.cs anchor and connected anchor are inverted
		// incorrect - but works because cj is used only for rotation
		// and rot direction is accounted for in code


		{
			// a) was doing this initially with z aligned atoms
			// orient config joint along bond axis (z = forward)
			// => Xrot for joint is along this axis
			//cj.axis = Vector3.forward;
		}

		{
			// b) more general solution
			//cj.axis is directly between attached objects 
			Vector3 worldAxis = go1.transform.position - go2.transform.position;
			Vector3 localAxis = go1.transform.InverseTransformDirection(worldAxis);
			cj.axis = localAxis;
		}

		// can use autoconfigure because geometry has been set up correctly ?
		cj.autoConfigureConnectedAnchor = true;

		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		cj.zMotion = ConfigurableJointMotion.Locked;

		cj.angularXMotion = ConfigurableJointMotion.Free;
		cj.angularYMotion = ConfigurableJointMotion.Locked;
		cj.angularZMotion = ConfigurableJointMotion.Locked;
	}

	void AddConfigJointBondSlack(GameObject go1, GameObject go2)
	{
		ConfigurableJoint cj = go1.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
		cj.connectedBody = go2.GetComponent<Rigidbody>();

		// NOTE
		// in PolyPepBuilder.cs anchor and connected anchor are inverted
		// incorrect - but works because cj is used only for rotation
		// and rot direction is accounted for in code


		{
			// a) was doing this initially with z aligned atoms
			// orient config joint along bond axis (z = forward)
			// => Xrot for joint is along this axis
			//cj.axis = Vector3.forward;
		}

		{
			// b) more general solution
			//cj.axis is directly between attached objects 
			Vector3 worldAxis = go1.transform.position - go2.transform.position;
			Vector3 localAxis = go1.transform.InverseTransformDirection(worldAxis);
			cj.axis = localAxis;
		}

		// can use autoconfigure because geometry has been set up correctly ?
		cj.autoConfigureConnectedAnchor = true;

		cj.xMotion = ConfigurableJointMotion.Locked;
		cj.yMotion = ConfigurableJointMotion.Locked;
		cj.zMotion = ConfigurableJointMotion.Locked;

		cj.angularXMotion = ConfigurableJointMotion.Free;
		cj.angularYMotion = ConfigurableJointMotion.Free;
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
