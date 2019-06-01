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

	public float scale;

	public float decomposeProb = 0.001f;

	public float age;
	public float inertTime = 2.0f;
	public bool pendingDestruct;

	public List<Material> materials;

	public KinSpawner mySpawner;

	public KinBind myKinBind;

	private Vector3 leftZoneOffset;



	private void Awake()
	{

	}
	// Start is called before the first frame update
	void Start()
    {
		//Debug.Log("New Molecule!");

		materials.Add(Resources.Load("Materials/material01", typeof( Material)) as Material);
		materials.Add(Resources.Load("Materials/material02", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material03", typeof(Material)) as Material);
		materials.Add(Resources.Load("Materials/material04", typeof(Material)) as Material);

		//zoneCollider = zoneGO.GetComponent<Collider>();
		myCollider = GetComponent<Collider>();
		myRigidbody = GetComponent<Rigidbody>();

		age = 0f;
		pendingDestruct = false;

		//SetRenderer();


		Vector3 myScale = new  Vector3(scale, scale, scale);
		transform.localScale = myScale;

	}

	private void SetRenderer()
	{
		Renderer myRenderer = GetComponent<Renderer>();
		myRenderer.material = materials[type];
	}


	private void UpdateBind()
	{
		if (myKinBind)
		{
			Vector3 targetPos = myKinBind.bindingSite.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPos, myKinBind.affinity * 0.001f);
		}

	}

	// REACT
	private void OnTriggerEnter(Collider collider)
	{

		if (age > inertTime)
		{
			KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
			if (molecule)
			{
				//Debug.Log("Collided with another molecule");
				if ((type == 0 && molecule.type == 1) && (!pendingDestruct && !molecule.pendingDestruct))// && (myKinBind && molecule.myKinBind))
				{
					var averagePosition = (collider.gameObject.transform.position + gameObject.transform.position) / 2f;

					{
						// deal with enzyme bound
						if (myKinBind)
						{
							myKinBind.ReleaseMol();
						}
						if (molecule.myKinBind)
						{
							molecule.myKinBind.ReleaseMol();
						}
					}

					// Destroy reactant A + B
					if (collider.gameObject) // ?
					{
						mySpawner.DestroyMolecule(gameObject);
						mySpawner.DestroyMolecule(collider.gameObject);
					}

					//Destroy(gameObject);
					//Destroy(collider.gameObject);

					// Create product C
					mySpawner.SpawnNewMolecule(2, averagePosition);

				}
			}
		}

	}

	private void UpdateCheckDecompose()
	{

		if (type == 2 && age > inertTime)
		{
			if (Random.Range(0f, 1.0f) <  mySpawner.pDecompose2)// decomposeProb)
			{
				Vector3 offset = (Random.onUnitSphere * transform.localScale.x);
				// Create products A + B
				mySpawner.SpawnNewMolecule(0, transform.position + offset);
				mySpawner.SpawnNewMolecule(1, transform.position - offset);
				// Destroy reactant C
				mySpawner.DestroyMolecule(gameObject);
				//Destroy(gameObject);

			}
		}

	}

	private void FixedUpdate()
	{
		UpdateBind();
		UpdateCheckDecompose();
	}

	// Update is called once per frame
	void Update()
    {
		age += Time.deltaTime;
    }
}
