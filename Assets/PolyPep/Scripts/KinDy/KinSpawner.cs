using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinSpawner : MonoBehaviour
{

	public KinMol molecule;
	public GameObject zoneGO;
	public int numMol = 20;

	public List<Material> materials;

	private int molCount = 0;

	// Start is called before the first frame update
	void Start()
    {
		
		materials.Add(Resources.Load("Materials/material01", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material02", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material03", typeof(Material)) as Material);

		DoStartingSpawn();
    }

	void DoStartingSpawn()
	{ 
		Bounds bounds = zoneGO.GetComponent<Collider>().bounds;

		for (int i = 0; i < numMol; i++)
		{
			Vector3 randomPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
											Random.Range(bounds.min.y, bounds.max.y),
											Random.Range(bounds.min.z, bounds.max.z));

			//Random.Range(0, 2);

			SpawnNewMolecule(2, randomPos);
		}
	}


	public void SpawnNewMolecule(int molType, Vector3 position)
	{

		KinMol _mol01 = Instantiate(molecule, position, Quaternion.identity); //, zoneGO.transform);

		_mol01.gameObject.name = "mol_" + molCount;
		_mol01.type = molType;
		_mol01.mySpawner = this;
		_mol01.scale = 0.04f;
		_mol01.zoneGO = zoneGO;

		KinDiffuse _diffuse = _mol01.gameObject.GetComponent("KinDiffuse") as KinDiffuse;

		_diffuse.zoneGO = zoneGO;

		molCount++;

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
