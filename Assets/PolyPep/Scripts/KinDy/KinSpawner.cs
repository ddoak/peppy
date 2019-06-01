using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinSpawner : MonoBehaviour
{

	public KinMol molecule;
	public GameObject zoneGO;
	public int numMol;

	public List<Material> materials;

	public List<KinMol> kinMols;

	private int molCount;

	public Slider[] molSliders;
	public KinTxt[] molTxts;

	private int[] molCounts;

	public float pDecompose2 = 0f;

	// Start is called before the first frame update
	void Start()
    {
		
		materials.Add(Resources.Load("Materials/material01", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material02", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material03", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material04", typeof(Material)) as Material);

		molCount = 0;
		molCounts = new int[4];
		molTxts = new KinTxt[4];
		molSliders = new Slider[4];

		molSliders[0] = GameObject.Find("Slider00").GetComponent<Slider>();
		molSliders[1] = GameObject.Find("Slider01").GetComponent<Slider>();
		molSliders[2] = GameObject.Find("Slider02").GetComponent<Slider>();
		molSliders[3] = GameObject.Find("Slider03").GetComponent<Slider>();

		// set slider colors from materials
		for (int i = 0; i < 4; i++)
		{
			var fill = molSliders[i].transform.Find("Fill Area/Fill");
			Image image = fill.GetComponent<Image>();
			if (image)
			{
				Color col = materials[i].color;
				col.a = 0.5f;
				image.color = col;
			}
		}

		//Image fill = molSliders[1].GetComponentsInChildren<UnityEngine.UI.Image>().FirstOrDefault(t => t.name == "Fill"); ;

		//fill. = Color.red;// ;

		// = (slider as UnityEngine.UI.Slider).GetComponentsInChildren<UnityEngine.UI.Image>()

		molTxts[0] = GameObject.Find("molTxt00").GetComponent<KinTxt>();
		molTxts[1] = GameObject.Find("molTxt01").GetComponent<KinTxt>();
		molTxts[2] = GameObject.Find("molTxt02").GetComponent<KinTxt>();
		molTxts[3] = GameObject.Find("molTxt03").GetComponent<KinTxt>();

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
		_mol01.scale = 0.05f;
		_mol01.zoneGO = zoneGO;
		_mol01.inertTime = 0.5f;

		_mol01.GetComponent<Renderer>().material = materials[molType];

		KinDiffuse _diffuse = _mol01.gameObject.GetComponent("KinDiffuse") as KinDiffuse;

		_diffuse.zoneGO = zoneGO;

		molCount++;

		kinMols.Add(_mol01);

	}

	public void DestroyMolecule(GameObject moleculeGO)
	{
		KinMol _mol = moleculeGO.GetComponent<KinMol>() as KinMol;
		if (_mol)
		{
			kinMols.Remove(_mol);
			_mol.pendingDestruct = true;
			Destroy(moleculeGO);
		}

	}

	private void PollMol()
	{
		int _numMols = kinMols.Count;

		for (int i = 0 ; i < 4; i++)
		{
			molCounts[i] = 0;
		}

		foreach(KinMol _mol in kinMols)
		{
			if (_mol.type < 4)
			{
				molCounts[_mol.type]++;
			}
		}

		for (int i = 0; i < 4; i++)
		{
			molTxts[i].SetTxt(molCounts[i].ToString());
			molSliders[i].value = molCounts[i];
		}
	}


    // Update is called once per frame
    void Update()
    {
		PollMol();
    }
}
