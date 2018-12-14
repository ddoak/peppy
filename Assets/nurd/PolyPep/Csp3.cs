using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Csp3 : MonoBehaviour {

	public Vector3 posA = new Vector3(0f, 0f, 0f);
	public Vector3 posB = new Vector3(0f, 0f, 0f);
	public Vector3 posC = new Vector3(0f, 0f, 0f);

	public List<Transform> HtfList = new List<Transform>();
	public List<Transform> BtfList = new List<Transform>();

	public string atomType = "sp3";

	// dev: keep prefab dev colours for atoms
	private bool keepDebugAtomMaterial;

	private void Awake()
	{
		keepDebugAtomMaterial = false;
		//Debug.Log(atomType);
		switch (atomType)
		{
			case "sp3":
				SetupSp3Atom();
				break;
			case "sp2":
				SetupSp2Atom();
				break;
			case "none":
				break;
		}


	}
	// Use this for initialization
	void Start () {

		

	}
	
	void SetupSp3Atom()
	{

		Vector3 socketPos = new Vector3(0f, 0f, 0f);

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			//  generate tetrahedral sp3 positions from cube vertices
			//Debug.Log(child);
			float _scale = 0.1192f; //  C-C bond will be (0.122475 * 0.1192) = 1.46
			bool addSocketOffset = true;
			switch (child.name)
			{
				case "H_0":
					//Debug.Log("----> Found a H_0");
					HtfList.Add(child);
					socketPos = new Vector3(1, 0, -1 / Mathf.Sqrt(2));
					// orient forward (Z) along direction of first sp3 bond using LookAt
					// essential to retain sanity later when building molecule
					transform.LookAt(transform.position + socketPos);
					break;
				case "H_1":
					HtfList.Add(child);
					socketPos = new Vector3(-1, 0, -1 / Mathf.Sqrt(2));
					break;
				case "H_2":
					HtfList.Add(child);
					socketPos = new Vector3(0, 1, 1 / Mathf.Sqrt(2));
					break;
				case "H_3":
					HtfList.Add(child);
					socketPos = new Vector3(0, -1, 1 / Mathf.Sqrt(2));
					break;

				case "tf_bond_H0":
				case "tf_bond_H1":
				case "tf_bond_H2":
				case "tf_bond_H3":
					BtfList.Add(child);
					addSocketOffset = false;
					break;

				default:
					addSocketOffset = false;
					break;
			}
			if (addSocketOffset)
			{
				child.transform.position += socketPos * _scale;
			}

		}

		
		int j = 0;
		foreach (Transform _H in HtfList)
		{
			//Debug.Log("H List ---> " + _H.name + " " + BtfList[j]);
			// use LookAt to align bonds to sp3 atom positions
		
			posA = transform.position;
			posB = _H.transform.position; //  HtfList[j].position;
			Vector3 relativePos = posB - posA;
			//Debug.Log(" dist = " + Vector3.Magnitude(relativePos));
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			BtfList[j].transform.LookAt(posB);// (HtfList[j].position);

			// point the sp3 atoms at the central atom
			_H.transform.LookAt(posA);

			j++;
		}

		// dev: set random rotation to try to avoid axis aligned bugs
		transform.rotation = Random.rotation;

		ShadowsOff();
		CollidersOff();
	}

	private void SetHAtomUnused(Transform _H)
	{
		_H.GetComponent<Renderer>().enabled = false;
		_H.GetComponent<Collider>().enabled = false;
		_H.tag = "UnusedAtom";
	}


	public void ConvertToCH3()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
					SetHAtomUnused(_H);
					break;
				case "H_1":
				case "H_2":
				case "H_3":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
				case "tf_bond_H3":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				default:
					break;
			}
		}
	}

	public void ConvertToCH2()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_3":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
				case "H_2":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H3":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				default:
					break;
			}
		}
	}

	public void ConvertToCH1()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_2":
				case "H_3":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H2":
				case "tf_bond_H3":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				case "tf_bond_H1":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				default:
					break;
			}
		}
	}

	public void ConvertToS()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_1":
				case "H_2":
				case "H_3":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
				case "tf_bond_H3":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				default:
					break;
			}
		}

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name == "C")
			{
				child.name = "S";
				child.tag = "S";
				child.GetComponent<Renderer>().material.color = Color.yellow;
				// child.localScale = new Vector3(1.1f, 1.1f, 1.1f); // will be overwriten by ScaleVDW() - doh!
			}
		}
	}

	public void ConvertToSH()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_1":
				case "H_2":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_3":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				case "tf_bond_H3":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					break;
				default:
					break;
			}
		}

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name == "C")
			{
				child.name = "S";
				child.GetComponent<Renderer>().material.color = Color.yellow;
				child.localScale = new Vector3(1.2f, 1.2f, 1.2f);
			}
		}
	}

	public void ConvertToOH()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_1":
				case "H_2":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_3":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				case "tf_bond_H3":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					break;
				default:
					break;
			}
		}

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name == "C")
			{
				child.name = "O";
				child.GetComponent<Renderer>().material.color = Color.red;
			}
		}
	}

	public void ConvertToNH3()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
				case "H_2":
				case "H_3":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
				case "tf_bond_H3":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				default:
					break;
			}
		}
		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name == "C")
			{
				child.name = "N";
				child.GetComponent<Renderer>().material.color = Color.blue;
			}
		}
	}

	public void ConvertSp2ToCOO()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
				case "H_2":
					if (_H.name == "H_1") { _H.name = "O_1"; }
					if (_H.name == "H_2") { _H.name = "O_2"; }
					_H.tag = "O";
					_H.transform.position += _H.transform.forward * 0.02f; //check COO- bond lengths
					//_H.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // will be overwriten by ScaleVDW() - doh!
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.red;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				default:
					break;
			}
		}
	}

	public void ConvertSp2ToCO()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_2":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
					_H.name = "O_1";
					_H.tag = "O";
					_H.transform.position += _H.transform.forward * 0.02f; //check COO- bond lengths
																		   //_H.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // will be overwriten by ScaleVDW() - doh!
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.red;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				case "tf_bond_H2":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				default:
					break;
			}
		}
	}

	public void ConvertSp2ToC(bool keepH1Bond)
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_1":
				case "H_2":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
					if (keepH1Bond) // phe / tyr rings
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					else
					{
						_bond.GetComponent<Renderer>().enabled = false;
						_bond.GetComponent<Collider>().enabled = false;
					}
					break;
				case "tf_bond_H2":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				default:
					break;
			}
		}
	}

	public void ConvertSp2ToNH2()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
				case "H_2":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
				case "tf_bond_H2":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				default:
					break;
			}
		}

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name == "C")
			{
				child.name = "N";
				child.tag = "N";
				child.GetComponent<Renderer>().material.color = Color.blue;
			}
		}

	}

	public void ConvertSp2ToNH()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_2":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				case "tf_bond_H2":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				default:
					break;
			}
		}

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name == "C")
			{
				child.name = "N";
				child.tag = "N";
				child.GetComponent<Renderer>().material.color = Color.blue;
			}
		}

	}

	public void ConvertSp2ToCH()
	{
		foreach (Transform _H in HtfList)
		{
			switch (_H.name)
			{
				case "H_0":
				case "H_2":
					//_H.GetComponent<Renderer>().enabled = false;
					//_H.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(_H);
					break;
				case "H_1":
					_H.transform.position += _H.transform.forward * 0.05f;
					if (!keepDebugAtomMaterial)
					{
						_H.GetComponent<Renderer>().material.color = Color.white;
					}
					break;
				default:
					break;
			}
		}
		foreach (Transform _Btf in BtfList)
		{
			Transform _bond = _Btf.GetChild(0); // only one child in pf
			switch (_Btf.name)
			{
				case "tf_bond_H0":
					if (!keepDebugAtomMaterial)
					{
						_bond.GetComponent<Renderer>().material.color = Color.grey;
					}
					break;
				case "tf_bond_H1":
					_bond.localPosition = new Vector3(_bond.localPosition.x, _bond.localPosition.y, _bond.localPosition.z - 0.25f);
					_bond.localScale = new Vector3(0.25f, 0.5f, 0.25f);
					_bond.GetComponent<Renderer>().material.color = Color.grey;
					//Debug.Log(_bond.name + " pos " + _bond.transform.localPosition.z); // = 0.5f;
					//Debug.Log("scale " + _bond.transform.localScale.y); // = 0.5f;
					break;
				case "tf_bond_H2":
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					break;
				default:
					break;
			}
		}

	}

	void SetupSp2Atom()
	{

		Vector3 socketPos = new Vector3(0f, 0f, 0f);

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			//  generate sp2 positions at 120 degree rotations
			//Debug.Log(child);
			float _scale = 0.146f; //  C-C bond will be 1.46
			bool addSocketOffset = true;
			float theta = 120.0f * Mathf.Deg2Rad;
			switch (child.name)
			{
				case "H_0":
					//Debug.Log("----> Found a H_0");
					HtfList.Add(child);
					socketPos = new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0));
					// orient forward (Z) along direction of first sp3 bond using LookAt
					// essential to retain sanity later when building molecule
					transform.LookAt(transform.position + socketPos);
					break;
				case "H_1":
					HtfList.Add(child);
					socketPos = new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
					break;
				case "H_2":
					HtfList.Add(child);
					socketPos = new Vector3(Mathf.Cos(2f * theta), 0, Mathf.Sin(2f * theta));
					break;
				case "H_3":
					//HtfList.Add(child);
					//child.GetComponent<Renderer>().enabled = false;
					//child.GetComponent<Collider>().enabled = false;
					SetHAtomUnused(child);
					addSocketOffset = false;
					break;

				case "tf_bond_H0":
				case "tf_bond_H1":
				case "tf_bond_H2":
					BtfList.Add(child);
					addSocketOffset = false;
					break;
				case "tf_bond_H3":
					//BtfList.Add(child);
					Transform _bond = child.GetChild(0); // only one child in pf
					_bond.GetComponent<Renderer>().enabled = false;
					_bond.GetComponent<Collider>().enabled = false;
					addSocketOffset = false;
					break;

				default:
					addSocketOffset = false;
					break;
			}
			if (addSocketOffset)
			{
				child.transform.position += socketPos * _scale;
			}

		}


		int j = 0;
		foreach (Transform _H in HtfList)
		{
			//Debug.Log("H List ---> " + _H.name + " " + BtfList[j]);
			// use LookAt to align bonds to sp3 atom positions

			posA = transform.position;
			posB = _H.transform.position; //  HtfList[j].position;
			Vector3 relativePos = posB - posA;
			//Debug.Log(" dist = " + Vector3.Magnitude(relativePos));
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			BtfList[j].transform.LookAt(posB);// (HtfList[j].position);

			// point the sp3 atoms at the central atom
			_H.transform.LookAt(posA);

			j++;
		}

		// dev: set random rotation to try to avoid axis aligned bugs
		//transform.rotation = Random.rotation;

		ShadowsOff();
		CollidersOff();
	}

	void ShadowsOff()
	{
		foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
		{
			Renderer myRenderer = renderer.GetComponent<Renderer>();
			myRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			myRenderer.receiveShadows = false;
		}
	}

	void CollidersOff()
	{
			foreach (Collider collider in GetComponentsInChildren<Collider>())
			{
				collider.isTrigger = true;
			}
	}



// Update is called once per frame
void Update () {
		
	}
}
