using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrostaticsManager : MonoBehaviour {

	private float cycleInterval = 0.02f;

	public List<ChargedParticle> chargedParticles;
	public List<MovingChargedParticle> movingChargedParticles;

	// Use this for initialization
	void Start ()
	{
		chargedParticles = new List<ChargedParticle> (FindObjectsOfType<ChargedParticle>());
		movingChargedParticles = new List<MovingChargedParticle>(FindObjectsOfType<MovingChargedParticle>());

		foreach (MovingChargedParticle mcp in movingChargedParticles)
			StartCoroutine (Cycle (mcp));
		
	}
	
	public IEnumerator Cycle(MovingChargedParticle mcp)
	{
		while(false) // false disables ES
		{
			ApplyElectrostaticForce(mcp);
			yield return new WaitForSeconds(cycleInterval);
		}
	}

	public void RegisterMovingChargedParticle(MovingChargedParticle mcp)
	{
		chargedParticles.Add(mcp);
		movingChargedParticles.Add(mcp);
		StartCoroutine(Cycle(mcp));
	}

	public void UnRegisterMovingChargedParticle(MovingChargedParticle mcp)
	{
		movingChargedParticles.Remove(mcp);
		chargedParticles.Remove(mcp);
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
						continue;
					}

					float distance = Vector3.Distance(mcp.transform.position, cp.transform.position);
					float force = (0.1f * mcp.charge * cp.charge) / Mathf.Pow(distance, 2);

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
