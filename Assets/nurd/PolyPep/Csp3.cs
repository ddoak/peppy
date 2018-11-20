using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Csp3 : MonoBehaviour {

	public Vector3 posA = new Vector3(0f, 0f, 0f);
	public Vector3 posB = new Vector3(0f, 0f, 0f);
	public Vector3 posC = new Vector3(0f, 0f, 0f);

	public List<Transform> HtfList = new List<Transform>();
	public List<Transform> BtfList = new List<Transform>();

	private void Awake()
	{
		SetupSp3Atom();
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
			float _scale = 0.1f; // 0.075f;
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
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			BtfList[j].transform.LookAt(posB);// (HtfList[j].position);

			j++;
		}

		// dev: set random rotation to try to avoid axis aligned bugs
		transform.rotation = Random.rotation;

		CollidersOff();
	}

	void CollidersOff()
	{
			foreach (var col in GetComponentsInChildren<Collider>())
			{
				col.isTrigger = true;
			}
	}



// Update is called once per frame
void Update () {
		
	}
}
