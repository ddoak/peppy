using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PolyPepManager : MonoBehaviour {


	public GameObject polyPepBuilder_pf;
	public List<PolyPepBuilder> allPolyPepBuilders = new List<PolyPepBuilder>();

	public bool collidersOn = false;

	public int UIDefinedSecondaryStructure { get; set; }

	public float phiTarget = 0f;
	public float psiTarget = 0f;

	private Slider phiSliderUI;
	private Slider psiSliderUI;
	private Slider vdwSliderUI;
	private Slider scaleSliderUI;
	private Slider hbondSliderUI;

	// Use this for initialization
	void Start()
	{
		GameObject pp = Instantiate(polyPepBuilder_pf, new Vector3(0f,1f,0f), Quaternion.identity);
		PolyPepBuilder pp_cs = pp.GetComponent<PolyPepBuilder>();
		pp_cs.numResidues = 10;

		GameObject pp2 = Instantiate(polyPepBuilder_pf, new Vector3(0f, 2f, 0f), Quaternion.identity);
		PolyPepBuilder pp2_cs = pp2.GetComponent<PolyPepBuilder>();
		pp2_cs.numResidues = 10;

		GameObject pp3 = Instantiate(polyPepBuilder_pf, new Vector3(0f, 3f, 0f), Quaternion.identity);
		PolyPepBuilder pp3_cs = pp3.GetComponent<PolyPepBuilder>();
		pp3_cs.numResidues = 10;

		foreach (PolyPepBuilder polyPep in FindObjectsOfType<PolyPepBuilder>() )
		{
			Debug.Log("------------------->" + polyPep);
			allPolyPepBuilders.Add(polyPep);

		}

		{
			//UI
			// initialise phi psi slider values (hacky?)

			GameObject temp = GameObject.Find("Slider_Phi");


			phiSliderUI = temp.GetComponent<Slider>();
			phiSliderUI.value = 0;

			temp = GameObject.Find("Slider_Psi");

			psiSliderUI = temp.GetComponent<Slider>();
			psiSliderUI.value = 0;

			temp = GameObject.Find("Slider_Vdw");

			vdwSliderUI = temp.GetComponent<Slider>();
			vdwSliderUI.value = 10;


			temp = GameObject.Find("Slider_HbondStrength");

			hbondSliderUI = temp.GetComponent<Slider>();
			hbondSliderUI.value = 2000;

			//temp = GameObject.Find("Slider_ResStart");

			//resStartSliderUI = temp.GetComponent<Slider>();
			//resStartSliderUI.maxValue = numResidues;
			//resStartSliderUI.value = 1;

			//temp = GameObject.Find("Slider_ResEnd");

			//Assert.IsNotNull(temp);

			//resEndSliderUI = temp.GetComponent<Slider>();
			//resEndSliderUI.maxValue = numResidues;
			//resEndSliderUI.value = 3; // numResidues; // initial value (+1)

			//temp = GameObject.Find("Slider_Scale");

			//scaleSliderUI = temp.GetComponent<Slider>();
			//scaleSliderUI.value = 10;

		}

	}
	

	public void UpdateVDWScalesFromUI(float scaleVDWx10)
	{

		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.ScaleVDW(scaleVDWx10 / 10.0f);
		}
	}

	public void UpdateCollidersFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetAllColliderIsTrigger(!value);
		}
	}

	public void UpdateHbondOnFromUI(bool value)
	{
		//Debug.Log("hello from the manager! ---> " + scaleVDWx10);
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.ActiveHbondSpringConstraints = value;
		}
	}

	public void UpdateHbondStrengthFromUI(float hbondStrength)
	{

		//Debug.Log("hello Hbond Strength from the manager! ---> " + hbondStrength);
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.hbondStrength = hbondStrength;
		}
	}

	public void UpdateDefinedSecondaryStructureFromUI()
	{
		float phi = phiTarget;
		float psi = psiTarget;
		switch (UIDefinedSecondaryStructure)
		{
			case 0:     
				// not defined
				phi = phiTarget;
				psi = psiTarget;
				break;

			case 1:     
				//alpha helix (right handed) (phi + ps ~ -105)
				phi = -57.0f;
				psi = -47.0f;
				break;

			case 2:     
				//310 helix (phi + psi ~ -75)
				phi = -49.0f;// -74.0f;
				psi = -26.0f;// -4.0f;
				break;

			case 3:     
				//anti beta sheet
				phi = -139.0f;
				psi = 135.0f;
				break;

			case 4:     
				//parallel beta sheet
				phi = -119.0f;
				psi = 113.0f;
				break;

			case 5:     
				//pi helix (phi + ps ~ -125)
				phi = -55.0f;
				psi = -70.0f;
				break;

			case 6:     
				//alpha helix (left handed)
				phi = 47.0f;
				psi = 57.0f;
				break;
		}
		psiTarget = psi;
		phiTarget = phi;
		UpdatePhiPsiForPolyPeptides();
	}

	public void UpdatePhiFromUI(float phi)
	{
		Debug.Log("hello from the manager! ---> " + phi);
		phiTarget = phi;
		UpdatePhiPsiForPolyPeptides();
	}

	public void UpdatePsiFromUI(float psi)
	{
		Debug.Log("hello from the manager! ---> " + psi);
		psiTarget = psi;
		UpdatePhiPsiForPolyPeptides();
	}

	private void UpdatePhiPsiForPolyPeptides()
	{
		foreach (PolyPepBuilder _ppb in allPolyPepBuilders)
		{
			_ppb.SetPhiPsiForSelection(phiTarget, psiTarget);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
