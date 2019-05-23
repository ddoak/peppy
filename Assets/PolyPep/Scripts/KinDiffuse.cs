using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinDiffuse : MonoBehaviour
{

	public GameObject zoneGO;
	public Collider zoneCollider;
	private Collider myCollider;
	Rigidbody myRigidbody;
	public bool inZone;
	public int type;

	public float speedDiffuse;
	public float speedZone;
	public float scale = 0.05f;

	public float age;
	public float inertTime = 2.0f;

	public List<Material> materials;


	private void Awake()
	{

	}
	// Start is called before the first frame update
	void Start()
    {
		zoneCollider = zoneGO.GetComponent<Collider>();
		myCollider = GetComponent<Collider>();
		myRigidbody = GetComponent<Rigidbody>();

		speedDiffuse = 0.01f;
		speedZone = 0.02f;


	age = 0f;
	}

	private void UpdateInZone()
	{
		inZone = myCollider.bounds.Intersects(zoneCollider.bounds);
	}

	private void UpdateJiggle()
	{
		if (age > inertTime / 2f)
		{
			if (inZone)
			{
				myRigidbody.AddForce(Random.onUnitSphere * speedDiffuse, ForceMode.Impulse);
				myRigidbody.AddTorque((0.1f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))), ForceMode.Impulse);
			}
		}
	}

	private void UpdateKeepInZone()
	{
		if (!inZone)
		{
			myRigidbody.AddForce(Vector3.Normalize(zoneCollider.transform.position - transform.position) * speedZone, ForceMode.Impulse);


		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (age > inertTime)
		{
			//mol01 molecule = collider.gameObject.GetComponent("mol01") as mol01;
			//if (molecule)
			//{
			//	if ((type == 0 && molecule.type == 1))
			//	{
			//		var averagePosition = (collider.gameObject.transform.position + gameObject.transform.position) / 2f;
			//		mySpawner.SpawnNewMolecule(3, averagePosition);

			//		Destroy(gameObject);
			//		Destroy(collider.gameObject);

			//	}
			//}
		}

	}


	private void FixedUpdate()
	{
		UpdateInZone();
		UpdateJiggle();
		UpdateKeepInZone();
		age += Time.deltaTime;
	}

	// Update is called once per frame
	void Update()
    {


    }
}
