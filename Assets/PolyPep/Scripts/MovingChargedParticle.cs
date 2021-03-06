﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingChargedParticle : ChargedParticle {


	public Rigidbody rb;

	// reference to residue for exclusions
	public GameObject residueGO;
	public bool isBBAmide = false;
	public bool isBBCarbonyl = false;
	public PolyPepBuilder myPPBChain = null;
	public int resid = -1;

	// Use this for initialization

	void Start ()
	{
		if (!rb)
		{
			rb = gameObject.GetComponent<Rigidbody>();
		}
		// 
		myPPBChain = residueGO.GetComponent<Residue>().myPolyPepBuilder;
		resid = residueGO.GetComponent<Residue>().resid;
		if (rb.tag == "amide")
		{
			isBBAmide = true;
		}
		if (rb.tag == "carbonyl")
		{
			isBBCarbonyl = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
