using System.Collections;
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
		if (residueGO)
		{
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


		ParticleSystem.MainModule _psMain = myChargedParticle_ps.main;
		Debug.Log(gameObject + " " + charge);
		if (charge < 0)
		{
			_psMain.startColor = new Color(0.7f, 0.3f, 0.3f, 0.8f); //Color.red;
		}
		else if (charge > 0)
		{
			_psMain.startColor = new Color(0.4f, 0.4f, 1.0f, 0.8f); //Color.blue;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
