using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{

	public Transform lastHit = null;
	public Vector3 hitPoint;

	public GameObject myMouseProxy;

	public bool grabbing;
	public bool startGrab;
	public GameObject grabObj;
	public float grabObjDist;
	

	// Start is called before the first frame update
	void Start()
	{
		Camera.main.stereoTargetEye = StereoTargetEyeMask.None;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateMouseRaycast();
		//UpdateMouseButtons();
		UpdatePollKeyboard();
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

		//myMouseProxy.transform.position = ray.GetPoint(1.0f);

		//Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red);
		//Debug.DrawRay(ray.origin, ray.direction * 100, Color.cyan);

		if (Physics.Raycast(ray, out hit, 200.0f))
		{
			if (hit.transform)
			{
				hitPoint = hit.point;
				myMouseProxy.transform.position = hitPoint;

				// raycast is hitting something

				GameObject go = hit.transform.gameObject;
				//Debug.Log(hit.transform.gameObject.name);

				if (hit.transform != lastHit)
				{
					// something is not what I hit (or didn't hit) last frame
					if (lastHit != null)
					{
						// was hitting something last frame - but have exited it
						HoverExit();
					}

					// have entered something new
					HoverEnter(go);

				}

				// store current hit as lasthit
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

		// Remote Grab

		if (Input.GetMouseButtonDown(2) && lastHit && !grabbing)
		{
			startGrab = true;
			grabbing = true;
			grabObj = lastHit.gameObject;
			grabObjDist = Vector3.Magnitude(hitPoint - Camera.main.transform.position);
		}

		if (Input.GetMouseButtonUp(2))
		{
			grabbing = false;
			grabObj = null;
		}

		if (grabbing)
		{
			Vector3 remoteGrabTargetPosition = ray.GetPoint(grabObjDist);
			RemoteGrabInteraction(grabObj.transform, remoteGrabTargetPosition);
		}

	}

	void HoverExit()
	{
		GameObject go = lastHit.gameObject;
		//Debug.Log(" Hover Exiting " + go.name + " GameObject");
		BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
		if (bu != null)
		{
			bu.SetBackboneUnitControllerHover(false);
		}
	}

	void HoverEnter(GameObject go)
	{
		// Debug.Log(" Hover Entering " + go.name + " GameObject");
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
			//Debug.Log(" Mouse Down");
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

	private void UpdatePollKeyboard()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			//Debug.Log(" Mouse Down");
			if (lastHit != null)
			{
				GameObject go = lastHit.gameObject;
				BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
				if (bu != null)
				{
					bu.SetMyResidueSelect(true);

				}

			}

		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			//Debug.Log(" Mouse Down");
			if (lastHit != null)
			{
				GameObject go = lastHit.gameObject;
				BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
				if (bu != null)
				{
					bu.SetMyResidueSelect(false);

				}

			}

		}
	}

	public void RemoteGrabInteraction(Transform t, Vector3 destination)
	{
		//Debug.Log("do  RemoteGrabInteraction!");

		GameObject go = t.gameObject;
		//BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
		//if (bu != null)
		{
			//Debug.Log("      --> script");
			//bu.TractorBeam(destination, true, 3.0f);
			TractorBeam(go, destination, true, 5.0f);
		}


	}

	public void TractorBeam(GameObject go, Vector3 position, bool attract, float scale)
	{

		//Debug.Log("tractor beam me!");

		float tractorBeamAttractionFactor = scale * 100.0f;
		float tractorBeamMin = scale * 100.0f;
		float tractorBeamDistanceRatio = 400f / scale; // larger = weaker


		Vector3 tractorBeam = position - go.transform.position;
		float tractorBeamMagnitude = Vector3.Magnitude(tractorBeam);
		//tractorBeamMagnitude = Mathf.Min(1.0f, tractorBeamMagnitude);

		if (!attract)
		{
			// repel
			tractorBeam = go.transform.position - position;
		}
		float tractorBeamScale = Mathf.Max(tractorBeamMin, (tractorBeamAttractionFactor * tractorBeamMagnitude / tractorBeamDistanceRatio));


		go.GetComponent<Rigidbody>().AddForce((tractorBeam * tractorBeamScale), ForceMode.Acceleration);
		// add scaling for 'size' of target?


	}


}
