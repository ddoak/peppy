using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PolyPepManager : MonoBehaviour {


	public GameObject polyPepBuilder_pf;
	public List<PolyPepBuilder> allPolyPepBuilders = new List<PolyPepBuilder>();

	public bool collidersOn = false;


	// Use this for initialization
	void Start()
	{
		GameObject pp = Instantiate(polyPepBuilder_pf);
		PolyPepBuilder pp_cs = pp.GetComponent<PolyPepBuilder>();
		pp_cs.numResidues = 10;

		GameObject pp2 = Instantiate(polyPepBuilder_pf);
		PolyPepBuilder pp2_cs = pp2.GetComponent<PolyPepBuilder>();
		pp2_cs.numResidues = 8;

		GameObject pp3 = Instantiate(polyPepBuilder_pf);
		PolyPepBuilder pp3_cs = pp3.GetComponent<PolyPepBuilder>();
		pp2_cs.numResidues = 6;

		foreach (PolyPepBuilder polyPep in FindObjectsOfType<PolyPepBuilder>() )
		{
			Debug.Log("------------------->" + polyPep);
			allPolyPepBuilders.Add(polyPep);

		}
	
	}
	

	public void UpdateVDWScalesFromUI(float scaleVDWx10)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		foreach (PolyPepBuilder _pp in allPolyPepBuilders)
		{
			_pp.ScaleVDW(scaleVDWx10 / 10.0f);
		}
	}

	public void UpdateCollidersFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		foreach (PolyPepBuilder _pp in allPolyPepBuilders)
		{
			_pp.SetAllCollidersIsTrigger(!value);
		}
	}








	// Update is called once per frame
	void Update () {
		
	}
}
