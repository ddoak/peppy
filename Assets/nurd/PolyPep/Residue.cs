using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residue : MonoBehaviour {

	public GameObject amide_pf;
	public GameObject calpha_pf;
	public GameObject carbonyl_pf;

	public float phiTarget;
	public float psiTarget;

	public float phiCurrent;
	public float psiCurrent;

	public GameObject ramaPlot;
	public float ramaPlotScale = 0.0012f; //  set visually against UI

	public GameObject myCube;

	public bool residueSelected = false;

	public bool drivePhiPsiOn = false;

	Shader shaderStandard;
	Shader shaderToonOutline;

	// Use this for initialization
	private void Awake()
	{
		shaderStandard = Shader.Find("Standard");
		shaderToonOutline = Shader.Find("Toon/Basic Outline");
	}

	void Start ()
	{
		ramaPlot = GameObject.Find("RamaPlotOrigin");

		myCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myCube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		myCube.transform.rotation = ramaPlot.transform.rotation;
		myCube.transform.position = ramaPlot.transform.position;

	}
	
	void MeasurePhiPsi()
	{
		phiCurrent = 180.0f - Vector3.SignedAngle(amide_pf.transform.up, calpha_pf.transform.up, amide_pf.transform.right);
		if (phiCurrent > 180.0f)
		{
			phiCurrent -= 360.0f;
		}
		//Debug.Log(phiTarget + " " + phiCurrent);

		psiCurrent = - Vector3.SignedAngle(calpha_pf.transform.up, carbonyl_pf.transform.up, calpha_pf.transform.right);

		//Debug.Log(psiTarget + " " + psiCurrent);
	}

	void UpdatePhiPsiPlotObj()
	{ 

		Renderer _myCubeRenderer  = myCube.GetComponent<Renderer>();
		if (residueSelected)
		{
			_myCubeRenderer.material.SetColor("_Color", Color.yellow);
		}
		else
		{
			_myCubeRenderer.material.SetColor("_Color", Color.white);
		}
		myCube.transform.rotation = ramaPlot.transform.rotation;
		myCube.transform.position = ramaPlot.transform.position + (ramaPlot.transform.right * ramaPlotScale * phiCurrent) + (ramaPlot.transform.up * ramaPlotScale * psiCurrent);
	}

	// Update is called once per frame
	void Update ()
	{
		MeasurePhiPsi();
		UpdatePhiPsiPlotObj();
	}
}
