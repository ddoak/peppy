using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrostaticsManager : MonoBehaviour {

	private float cycleInterval = 0.02f;

	public List<ChargedParticle> chargedParticles;
	public List<MovingChargedParticle> movingChargedParticles;

	public bool electrostaticsOn = false;
	public float electrostaticsStrength;

	// Use this for initialization
	void Start ()
	{
		chargedParticles = new List<ChargedParticle> (FindObjectsOfType<ChargedParticle>());
		movingChargedParticles = new List<MovingChargedParticle>(FindObjectsOfType<MovingChargedParticle>());

		//foreach (MovingChargedParticle mcp in movingChargedParticles)
		//	StartCoroutine (Cycle (mcp));
		
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
		if (electrostaticsOn)
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
			StopAllCoroutines();
			electrostaticsOn = false;
		}
		else
		{
			foreach (MovingChargedParticle mcp in movingChargedParticles)
				StartCoroutine(Cycle(mcp));
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

			foreach (ChargedParticle cp in chargedParticles)
			{
				//Debug.Log("cp - " + cp);
				if (cp)
				{
					if (mcp==cp)
					{
						// don't act on myself
						continue;
					}

					float distance = Vector3.Distance(mcp.transform.position, cp.transform.position);
					float force = (0.0025f * electrostaticsStrength * mcp.charge * cp.charge) / Mathf.Pow(distance, 2);

					Vector3 direction = mcp.transform.position - cp.transform.position;
					direction.Normalize();

					newForce += force * direction * cycleInterval;

					if (float.IsNaN(newForce.x))
					{
						newForce = Vector3.zero;
					}

					//Debug.Log(mcp);
					//Debug.Log(mcp.rb);
					if (mcp.rb)
					{
						mcp.rb.AddForce(newForce, ForceMode.Impulse);
						var fo = mcp.myChargedParticle_ps.forceOverLifetime;
						float _scale = 200.0f;
						fo.x = _scale * newForce.x;
						fo.y = _scale * newForce.y;
						fo.z = _scale * newForce.z;
					}
				}
			}
		}

	}
	// Update is called once per frame
	void Update () {
		
	}
}

// Implementation modelled on:
// https://youtu.be/eBVim8kEhK8
// https://github.com/Zulban/coulombic
