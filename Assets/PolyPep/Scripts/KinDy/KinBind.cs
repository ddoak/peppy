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

	public KinMol testMolecule;
	public KinMol lastBindableMolecule;

	// Start is called before the first frame update
	void Start()
	{
		isBinding = false;
	}


	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.GetComponent("KinMol") as KinMol)
		{
			testMolecule = collider.gameObject.GetComponent("KinMol") as KinMol;
			DoBindCheck(testMolecule);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent("KinMol") as KinMol == lastBindableMolecule)
		{
			lastBindableMolecule = null;
		}
	}


	private void DoBindCheck(KinMol molecule)
	{
		//if (!isBinding)
		bool canBind = false;
		{
			//KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
			if (molecule)
			{
				if (molecule.type == typeToBind)
				{
					//bool displace = Random.Range(0f, 1f) > 0.9f;
					if (!isBinding)// || displace)
					{
						// if molecule is already bound to another site - force release
						if (molecule.myKinBind)
						{
							if (affinity > molecule.myKinBind.affinity)
							{
								molecule.myKinBind.ReleaseMol();
								canBind = true;
							}
						}
						else
						{
							canBind = true;
						}

						if (canBind)
						{
							BindMol(molecule);
						}
						
					}
					else
					{
						lastBindableMolecule = testMolecule;
					}


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
			KinMol _lastBoundMolecule = boundMol;

			isBinding = false;
			boundMol.myKinBind = null;
			boundMol = null;

			if (lastBindableMolecule && (lastBindableMolecule != _lastBoundMolecule))
			{
				DoBindCheck(lastBindableMolecule);
			}
			
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

	void CheckForMissedTrigger()
	{
		if (!isBinding && lastBindableMolecule)
		{
			//Debug.Log("MISSED TRIGGER");
			//DoBindCheck(lastBindableMolecule);
		}
	}

	void CheckForMissingMolecule()
	{
		if (isBinding && !boundMol)
		{
			Debug.Log("KinBind: lost molecule");
			isBinding = false;
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckForExit();
		CheckForMissedTrigger();
		CheckForMissingMolecule();
	}
}
