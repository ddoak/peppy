using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
	public GameObject water;
	//public GameObject zoneGO;
	public int numMol;
	public List<GameObject> waters;
	public PolyPepManager myPolyPepManager;

	// electrostatics
	public ElectrostaticsManager myElectrostaticsManager;

	private int molCount;

	// Start is called before the first frame update
	void Start()
    {
		myPolyPepManager = GameObject.Find("PolyPepManager").GetComponent<PolyPepManager>();
		myElectrostaticsManager = GameObject.Find("ElectrostaticsManager").GetComponent<ElectrostaticsManager>();

		DoStartingSpawn(numMol, 1);
    }

	void DoStartingSpawn(int number, int type)
	{
		Bounds bounds = this.GetComponent<Collider>().bounds;

		for (int i = 0; i < number; i++)
		{
			Vector3 randomPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
											Random.Range(bounds.min.y, bounds.max.y),
											Random.Range(bounds.min.z, bounds.max.z));

			//Random.Range(0, 2);

			SpawnNewMolecule(type, randomPos);
		}
	}


	public void SpawnNewMolecule(int molType, Vector3 position)
	{

		GameObject _water = Instantiate(water, position, Quaternion.identity); //, zoneGO.transform);

		_water.gameObject.name = "water_" + molCount;

		
		molCount++;

		waters.Add(_water);

		myElectrostaticsManager.RegisterMovingChargedParticle(_water.transform.Find("O").GetComponent<MovingChargedParticle>());
		myElectrostaticsManager.RegisterMovingChargedParticle(_water.transform.Find("H1").GetComponent<MovingChargedParticle>());
		myElectrostaticsManager.RegisterMovingChargedParticle(_water.transform.Find("tf_H/H2").GetComponent<MovingChargedParticle>());
	}

	private void DoJiggle()
	{
		if (myPolyPepManager.jiggleStrength > 0.0f)
		{
			foreach (GameObject _water  in waters)
			{
				JiggleRb(_water.GetComponent<Rigidbody>());
			}
		}

	}

	private void JiggleRb(Rigidbody rb)
	{
		rb.AddForce(UnityEngine.Random.onUnitSphere * 0.01f * myPolyPepManager.jiggleStrength, ForceMode.Impulse);
		rb.AddTorque((0.001f * myPolyPepManager.jiggleStrength * new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))), ForceMode.Impulse);
	}

	// Update is called once per frame
	void Update()
    {
		DoJiggle();
    }
}
