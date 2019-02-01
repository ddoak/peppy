using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedParticle : MonoBehaviour {

	public float charge = 0f;
	public ParticleSystem myChargedParticle_ps;
	//public Rigidbody rb;

	// grotesque hack for exclusions
	public GameObject residueGO;

	// Use this for initialization
	void Start ()
	{
		//rb = gameObject.GetComponent<Rigidbody>();
	}
	

	void UpdateParticleSystem()
	{

	}

	// Update is called once per frame
	void Update () {
		
	}
}
