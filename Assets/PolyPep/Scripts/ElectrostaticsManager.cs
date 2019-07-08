using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrostaticsManager : MonoBehaviour {

	private float cycleInterval = 0.1f; 

	public List<ChargedParticle> chargedParticles;
	public List<MovingChargedParticle> movingChargedParticles;

	public bool electrostaticsOn = false;
	public float electrostaticsStrength;
	public bool showElectrostatics;

	public PolyPepManager myPolyPepManager;

	// Use this for initialization
	void Start ()
	{
		myPolyPepManager =  GameObject.Find("PolyPepManager").GetComponent<PolyPepManager>();

		chargedParticles = new List<ChargedParticle> (FindObjectsOfType<ChargedParticle>());
		movingChargedParticles = new List<MovingChargedParticle>(FindObjectsOfType<MovingChargedParticle>());

		foreach (MovingChargedParticle mcp in movingChargedParticles)
			StartCoroutine(Cycle(mcp));

	}
	
	public IEnumerator Cycle(MovingChargedParticle mcp)
	{
		while(true) // false disables ES
		{
			ApplyElectrostaticForce(mcp);
			yield return new WaitForSeconds(cycleInterval);
		}
	}

	public void RegisterMovingChargedParticle(MovingChargedParticle mcp)
	{
		chargedParticles.Add(mcp);
		movingChargedParticles.Add(mcp);
		//if (electrostaticsOn)
		{
			StartCoroutine(Cycle(mcp));
		}

	}

	public void UnRegisterMovingChargedParticle(MovingChargedParticle mcp)
	{
		movingChargedParticles.Remove(mcp);
		chargedParticles.Remove(mcp);
	}

	public void SwitchElectrostatics()
	{
		if (electrostaticsOn)
		{
			//StopAllCoroutines();
			electrostaticsOn = false;
		}
		else
		{
			//foreach (MovingChargedParticle mcp in movingChargedParticles)
			//	StartCoroutine(Cycle(mcp));
			electrostaticsOn = true;
		}
	}

	private void ApplyElectrostaticForce(MovingChargedParticle mcp)
	{
		Vector3 newForce = Vector3.zero;

		// null checks - seem to be necessary when sidechain mcp are deleted 

		if (mcp)
		{
			//Debug.Log("mcp - " + mcp);
			//Debug.Log("mcp.rb - " + mcp.rb);

			foreach (MovingChargedParticle mcp2 in movingChargedParticles)
			{
				//Debug.Log("cp - " + cp);
				if (mcp2)
				{
					// Exclusions

					// 1. don't act on myself
					if (mcp==mcp2)
					{
						continue;
					}

					// 2. don't act on mcp in same residue
					if (mcp.residueGO && mcp2.residueGO)
					{
						if (mcp.residueGO == mcp2.residueGO)
						{ 	
							continue;
						}
					}

					// 3. if backbone HN / OC don't act on adjacent residues
					// (performance - not sure if paralleled in real MD sim)
					if (mcp.rb && mcp2.rb)
					{
						if ((mcp.isBBAmide && mcp2.isBBCarbonyl) || (mcp2.isBBAmide && mcp.isBBCarbonyl))
						{
							if (mcp.myPPBChain == mcp2.myPPBChain)
							{
								if ((mcp.resid == mcp2.resid + 1) || (mcp.resid == mcp2.resid - 1))
								{
									continue;
								}
							}
						}
					}

					// 4. Water - if both mcp have same rb
					if (mcp.rb == mcp2.rb)
					{
						continue;
					}


					float distance = Vector3.Distance(mcp.transform.position, mcp2.transform.position);
					float force = (5.0f * 0.0025f * electrostaticsStrength * mcp.charge * mcp2.charge) / Mathf.Pow(distance, 2);

					Vector3 direction = mcp.transform.position - mcp2.transform.position;
					direction.Normalize();

					newForce += force * direction * cycleInterval;
					
					if (float.IsNaN(newForce.x))
					{
						newForce = Vector3.zero;
					}

				}
			}
			//Debug.Log(mcp);
			//Debug.Log(mcp.rb);
			if (mcp.rb)
			{
				mcp.rb.AddForce(newForce, ForceMode.Impulse);
			}

			// particle system update
			if (mcp.myChargedParticle_ps)
			{

				if (showElectrostatics)
				{
					if (!mcp.myChargedParticle_ps.isPlaying)
					{
						mcp.myChargedParticle_ps.Play();
					}

					// 
					var fo = mcp.myChargedParticle_ps.forceOverLifetime;
					var main = mcp.myChargedParticle_ps.main;
					var shape = mcp.myChargedParticle_ps.shape;

					var em = mcp.myChargedParticle_ps.emission;
					em.rate = Mathf.Abs(mcp.charge) * Mathf.Abs(mcp.charge) * 5000;


					float scaleParticleForce = 7.5f * (1 / cycleInterval); // 1000.0f;
					fo.x = scaleParticleForce * newForce.x;
					fo.y = scaleParticleForce * newForce.y;
					fo.z = scaleParticleForce * newForce.z;

					//
					main.startLifetimeMultiplier = Mathf.Clamp((1.0f-(0.5f*Vector3.Magnitude(newForce))), 0.1f, 1.0f);

					//main.startSize = Mathf.Lerp(0.4f, 1.0f, (electrostaticsStrength / 100.0f));

					main.startSize = Mathf.Lerp(0.4f, 2.0f, Vector3.Magnitude(newForce));

					// scale shape radius to keep particles visible
					// 2f is base radius in particle effect
					shape.radius = 2f * myPolyPepManager.vdwScale;

				}
				else
				{
					if (mcp.myChargedParticle_ps.isPlaying)
					{
						mcp.myChargedParticle_ps.Stop();
					}
				}
			}
		}

	}

	void UpdateCheckForSwitchOnOff()
	{
		if (electrostaticsOn && electrostaticsStrength == 0)
		{
				//Debug.Log("ES switched OFF");
				SwitchElectrostatics();
		}
		else if (!electrostaticsOn && electrostaticsStrength != 0)
		{
				//Debug.Log("ES switched ON");
				SwitchElectrostatics();
		}
	}

	// Update is called once per frame
	void Update ()
	{
		UpdateCheckForSwitchOnOff();
	}
}

// Implementation modelled on:
// https://youtu.be/eBVim8kEhK8
// https://github.com/Zulban/coulombic
