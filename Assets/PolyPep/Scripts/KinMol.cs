using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinMol : MonoBehaviour
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

	public float decomposeProb = 0.001f;

	public float age;
	public float inertTime = 2.0f;

	public List<Material> materials;

	public KinSpawner mySpawner;

	public KinBind myKinBind;


	private void Awake()
	{

	}
	// Start is called before the first frame update
	void Start()
    {
		materials.Add(Resources.Load("Materials/material01", typeof( Material)) as Material);
		materials.Add(Resources.Load("Materials/material02", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material03", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material04", typeof(Material)) as Material);

		zoneCollider = zoneGO.GetComponent<Collider>();
		myCollider = GetComponent<Collider>();
		myRigidbody = GetComponent<Rigidbody>();

		age = 0f;

		SetRenderer();

		Vector3 myScale = new  Vector3(scale, scale, scale);
		transform.localScale = myScale;

		speedDiffuse = 0.01f;
		speedZone = 0.005f;
	}

	private void SetRenderer()
	{
		Renderer myRenderer = GetComponent<Renderer>();
		myRenderer.material = materials[type];
	}

	private void UpdateInZone()
	{
		inZone = myCollider.bounds.Intersects(zoneCollider.bounds);
	}

	private void UpdateJiggle()
	{
		if (myKinBind)
		{
			//transform.position = myKinBind.bindingSite.transform.position;
			//Vector3 targetPos = Vector3.Lerp(transform.position, myKinBind.bindingSite.transform.position, 0.2f);
			Vector3 targetPos = myKinBind.bindingSite.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.001f);
			//myRigidbody.AddForce((myKinBind.bindingSite.transform.position - transform.position) * 0.1f, ForceMode.Impulse);

			

		}
		else
		if (age > inertTime / 2f)
		{
			if (inZone)
			{
				myRigidbody.AddForce(Random.onUnitSphere * speedDiffuse, ForceMode.Impulse);
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
			KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
			if (molecule)
			{
				//Debug.Log("Trigger!");
				if ((type == 0 && molecule.type == 1))// && (myKinBind && molecule.myKinBind))
				{
					var averagePosition = (collider.gameObject.transform.position + gameObject.transform.position) / 2f;
					

					if (myKinBind)
					{
						myKinBind.ReleaseMol();
					}
					if (molecule.myKinBind)
					{
						molecule.myKinBind.ReleaseMol();
					}


					Destroy(gameObject);
					Destroy(collider.gameObject);

					mySpawner.SpawnNewMolecule(3, averagePosition);

				}
			}
		}

	}

	private void UpdateCheckDecompose()
	{

		if (type == 2 && age > inertTime)
		{
			if (Random.Range(0f, 1.0f) < decomposeProb)
			{
				Vector3 offset = (Random.onUnitSphere * transform.localScale.x);
				mySpawner.SpawnNewMolecule(0, transform.position + offset);
				mySpawner.SpawnNewMolecule(1, transform.position - offset);

				//SetType(0);
				//age = 0f;

				Destroy(gameObject);

			}
		}

	}

	private void FixedUpdate()
	{
		UpdateInZone();
		UpdateJiggle();
		UpdateKeepInZone();
		UpdateCheckDecompose();
	}

	// Update is called once per frame
	void Update()
    {
		age += Time.deltaTime;
    }
}
