using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{

    public Transform lastHit = null;

	public GameObject myMouseProxy;

    // Start is called before the first frame update
    void Start()
    {
		Camera.main.stereoTargetEye = StereoTargetEyeMask.None;
	}

    // Update is called once per frame
    void Update()
    {
        UpdateMouseRaycast();
        UpdateMouseButtons();
    }


	void OnDrawGizmos()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(ray.GetPoint(10.0f), 1.0f);
	}


	void UpdateMouseRaycast()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//Debug.Log(Input.mousePosition);

		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward) * 10;
		//Debug.DrawRay(transform.position, forward, Color.green);

		Debug.DrawRay(ray.origin, ray.GetPoint(50.0f), Color.green);

		Camera.main.ScreenToViewportPoint(Input.mousePosition);

		myMouseProxy.transform.position = ray.GetPoint(1.0f);

		//Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red);
		//Debug.DrawRay(ray.origin, ray.direction * 100, Color.cyan);

		if (Physics.Raycast(ray, out hit, 200.0f))
        {
            if (hit.transform)
            {
                GameObject go = hit.transform.gameObject;
                //Debug.Log(hit.transform.gameObject.name);

                if (hit.transform != lastHit)
                {
                    if (lastHit != null)
                    {
                        HoverExit();
                    }

                    HoverEnter(go);

                }
                lastHit = hit.transform;


            }
            else
            {
 
            }

        }
        else
        {
            if (lastHit != null)
            {
                HoverExit();
                lastHit = null;
            }
        }

    }

    void HoverExit()
    {
        GameObject go = lastHit.gameObject;
        Debug.Log(" Hover Exiting " + go.name + " GameObject");
        BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
        if (bu != null)
        {
            bu.SetBackboneUnitControllerHover(false);
        }
    }

    void HoverEnter(GameObject go)
    {
        Debug.Log(" Hover Entering " + go.name + " GameObject");
        BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
        if (bu != null)
        {
            //Debug.Log("      --> script");
            bu.SetBackboneUnitControllerHover(true);
        }
    }

    private void UpdateMouseButtons()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(" Mouse Down");
            if (lastHit != null)
            {
                GameObject go = lastHit.gameObject;
                BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
                if (bu != null)
                {
                    bu.SetMyResidueSelect(!bu.myResidue.residueSelected);

                }
            }
        }

    }

}
