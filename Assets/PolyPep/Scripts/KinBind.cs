using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinBind: MonoBehaviour
{
	public bool isOccupied;
	public KinMol boundMol;
	public Transform bindingSite;
	public int typeToBind;

	// Start is called before the first frame update
	void Start()
	{
		isOccupied = false;
	}


	private void OnTriggerEnter(Collider collider)
	{
		if (!isOccupied)
		{
			KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
			if (molecule)
			{
				if (molecule.type == typeToBind)
				{
					BindMol(molecule);
					//var averagePosition = (collider.gameObject.transform.position + gameObject.transform.position) / 2f;
					//mySpawner.SpawnNewMolecule(3, averagePosition);

					//Destroy(gameObject);
					//Destroy(collider.gameObject);

				}
			}
		}

	}

	private void BindMol(KinMol molecule)
	{
		isOccupied = true;
		boundMol = molecule;
		molecule.myKinBind = this;
	}

	public void ReleaseMol()
	{
		if (boundMol)
		{
			isOccupied = false;
			boundMol.myKinBind = null;
			boundMol = null;
			
		}
		else
		{
			Debug.LogError("ReleaseMol() - boundMol NULL");
		}
	}


	// Update is called once per frame
	void Update()
	{

	}
}
