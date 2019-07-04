using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeam : MonoBehaviour
{
	public void DoTractorBeam(GameObject go, Vector3 position, bool attract, float scale)
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

		if (go.GetComponent<Rigidbody>())
		{
			go.GetComponent<Rigidbody>().AddForce((tractorBeam * tractorBeamScale), ForceMode.Acceleration);
		}
		
		// add scaling for 'size' of target?


	}
}
