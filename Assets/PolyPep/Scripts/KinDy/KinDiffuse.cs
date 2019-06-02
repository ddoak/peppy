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
	public bool isTransmembrane;
	public bool canDiffuse;

	public float speedDiffuse = 0.01f;
	public float speedZone = 0.02f;

	private void Awake()
	{

	}
	// Start is called before the first frame update
	void Start()
    {
		zoneCollider = zoneGO.GetComponent<Collider>();
		myCollider = GetComponent<Collider>();
		myRigidbody = GetComponent<Rigidbody>();
		canDiffuse = true;

	}

	private void UpdateInZone()
	{
		inZone = myCollider.bounds.Intersects(zoneCollider.bounds);
	}

	private void UpdateMovement()
	{
		if (isTransmembrane)
		{
			{
				Vector3 pushDir = - zoneCollider.transform.right;
				float delta = Vector3.Magnitude(zoneCollider.transform.position - transform.position);
				float dot = Vector3.Dot(pushDir, (zoneCollider.transform.position - transform.position));
				//Debug.Log("transmembrane" + delta);
				if (delta > 0.2f)
				{
					float force = 0.02f * delta;
					if (dot > 0f)
					{
						gameObject.GetComponent<Rigidbody>().AddForce(pushDir * force, ForceMode.Impulse);
					}
					else if (dot < 0f)
					{
						gameObject.GetComponent<Rigidbody>().AddForce(-pushDir * force, ForceMode.Impulse);
					}
				}

				transform.rotation = Quaternion.RotateTowards(transform.rotation, zoneCollider.transform.rotation * Quaternion.Euler(0,0,-90f), 5f);

				Vector2 inCircle = Random.insideUnitCircle;
				Vector3 diffuseV = new Vector3 (0f, inCircle.y, inCircle.x);
				myRigidbody.AddForce(diffuseV * speedDiffuse, ForceMode.Impulse);
			}
		}
		else
		{
			if (canDiffuse)// (age > inertTime / 2f)
			{
				if (inZone)
				{
					myRigidbody.AddForce(Random.onUnitSphere * speedDiffuse, ForceMode.Impulse);
					myRigidbody.AddTorque((0.1f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))), ForceMode.Impulse);
				}
			}
		}

	}

	private void UpdateKeepInZone()
	{
		//if (!inZone)
		//{
		//	myRigidbody.AddForce(Vector3.Normalize(zoneCollider.transform.position - transform.position) * speedZone, ForceMode.Impulse);
		//}
		if (!inZone)
		{
			Debug.DrawLine(zoneCollider.ClosestPointOnBounds(transform.position), transform.position);
			myRigidbody.AddForce((zoneCollider.ClosestPointOnBounds(transform.position) - transform.position) * 5f * speedZone, ForceMode.Impulse);
		}
	}

	private void FixedUpdate()
	{
		UpdateInZone();
		UpdateMovement();
		UpdateKeepInZone();
	}

	// Update is called once per frame
	void Update()
    {


    }
}
