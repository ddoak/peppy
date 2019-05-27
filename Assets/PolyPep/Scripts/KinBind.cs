using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinBind: MonoBehaviour
{
	public bool isBinding;
	public KinMol boundMol;
	public Transform bindingSite;
	public int typeToBind;
	public float affinity;
	public bool isReleaseSite;

	// Start is called before the first frame update
	void Start()
	{
		isBinding = false;
	}


	private void OnTriggerEnter(Collider collider)
	{
		if (!isBinding)
		{
			KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
			if (molecule)
			{
				if (molecule.type == typeToBind)
				{
					// if already bound to another site - force release
					if (molecule.myKinBind)
					{
						molecule.myKinBind.ReleaseMol();
					}

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
		isBinding = true;
		boundMol = molecule;
		molecule.myKinBind = this;
	}

	public void ReleaseMol()
	{
		if (boundMol)
		{
			isBinding = false;
			boundMol.myKinBind = null;
			boundMol = null;
			
		}
		else
		{
			Debug.LogError("ReleaseMol() - boundMol NULL");
		}
	}

	void CheckForExit()
	{
		if (isReleaseSite && isBinding)
		{
			if (bindingSite.GetComponent<Collider>().bounds.Intersects(boundMol.GetComponent<Collider>().bounds))
			{
				ReleaseMol();
			}

		}
	}


	// Update is called once per frame
	void Update()
	{
		CheckForExit();
	}
}
