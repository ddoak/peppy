using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingChargedParticle : ChargedParticle {


	public Rigidbody rb;


	// Use this for initialization

	void Start ()
	{
		if (!rb)
		{
			rb = gameObject.GetComponent<Rigidbody>();
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
