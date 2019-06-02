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

	public KinMol possRxMol;
	public float rxTime;

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
		rxTime = 0f;

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
			GetComponent<KinDiffuse>().canDiffuse = false;
			Vector3 targetPos = myKinBind.bindingSite.transform.position;
			transform.position = Vector3.MoveTowards(transform.position, targetPos, myKinBind.affinity * 0.001f);
		}
		else
		{
			GetComponent<KinDiffuse>().canDiffuse = true;
		}

	}


	private void OnTriggerEnter(Collider other)
	{

		//if (age > inertTime)
		{
			KinMol molecule = other.gameObject.GetComponent("KinMol") as KinMol;
			if (molecule)
			{
				//Debug.Log("Collided with another molecule");
				if ((type == 0 && molecule.type == 1) && (!pendingDestruct && !molecule.pendingDestruct))
				{
					if (!possRxMol)
					{
						possRxMol = molecule;
						rxTime = 0f;
					}
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		KinMol molecule = other.gameObject.GetComponent("KinMol") as KinMol;
		if (molecule)
		{
			if (molecule = possRxMol)
			{
				possRxMol = null;
				rxTime = 0f;
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		KinMol molecule = other.gameObject.GetComponent("KinMol") as KinMol;
		if (molecule)
		{
			if (molecule = possRxMol)
			{
				rxTime += Time.deltaTime;
			}
			if (rxTime > mySpawner.tRx01)
			{
				DoReaction();
			}
		}
	}

	private void DoReaction()
	{
		// DO REACTION
		{
			// deal with enzyme bound
			if (myKinBind)
			{
				myKinBind.ReleaseMol();
			}
			if (possRxMol.myKinBind)
			{
				possRxMol.myKinBind.ReleaseMol();
			}
		}


		var averagePosition = (possRxMol.gameObject.transform.position + gameObject.transform.position) / 2f;

		// Destroy reactant A + B

		mySpawner.DestroyMolecule(gameObject);
			mySpawner.DestroyMolecule(possRxMol.gameObject);


		//Destroy(gameObject);
		//Destroy(collider.gameObject);

		// Create product C
		mySpawner.SpawnNewMolecule(3, averagePosition);
	}

	// CHECK REACT
	private void LegacyOnTriggerEnter(Collider collider)
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

					float dotA = Vector3.Dot(Vector3.Normalize(gameObject.GetComponent<Rigidbody>().velocity), Vector3.Normalize(gameObject.transform.position - collider.gameObject.transform.position));

					float dotB = Vector3.Dot(Vector3.Normalize(collider.gameObject.GetComponent<Rigidbody>().velocity), Vector3.Normalize(collider.gameObject.transform.position - gameObject.transform.position));

					//Debug.Log(" velocity = " + gameObject.GetComponent<Rigidbody>().velocity);

					//Debug.Log(" dotA = " + dotA);
					//Debug.Log(" dotB = " + dotB);

					bool enzymeInvolved = (myKinBind || molecule.myKinBind);

					if ((dotA + dotB) > 0.1f)
					{
						Debug.Log(" dotA + dotB = " + (dotA + dotB) + " " + enzymeInvolved);

						// DO REACTION
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

	}

	private void UpdateCheckDecompose()
	{

		if (type == 2 && age > inertTime)
		{
			if (Random.Range(0f, 1.0f) <  mySpawner.pDecompose2)
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

		if (type == 3 && age > inertTime)
		{
			if (Random.Range(0f, 1.0f) < mySpawner.pDecompose3)
			{
				// Create product
				mySpawner.SpawnNewMolecule(2, transform.position);
				// Destroy reactant
				mySpawner.DestroyMolecule(gameObject);
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
