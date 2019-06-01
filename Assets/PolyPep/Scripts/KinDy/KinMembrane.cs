using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinMembrane : MonoBehaviour
{
    
	// Start is called before the first frame update
    void Start()
    {
        
    }

	//private void OnTriggerEnter(Collider collider)
	//{
	//	{
	//		KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
	//		if (molecule)
	//		{
	//			if (molecule.type == 3)
	//			{
	//				if ( Vector3.Dot(transform.right, (molecule.transform.position - transform.position)) > 0f)
	//				{
	//					molecule.GetComponent<Rigidbody>().AddForce(transform.right * 0.25f, ForceMode.Impulse);
	//				}
	//			}
	//		}
	//	}

	//}

	private void OnTriggerStay(Collider collider)
	{
	
		KinMol molecule = collider.gameObject.GetComponent("KinMol") as KinMol;
		if (molecule)
		{
			if (!molecule.myKinBind)
			{
				if (molecule.type == 3)
				{
					Vector3 pushDir = - transform.right;
					float dot = Vector3.Dot(pushDir, (molecule.transform.position - transform.position));
					if (dot > 0f)
					{
						molecule.GetComponent<Rigidbody>().AddForce(pushDir * 0.01f, ForceMode.Impulse);
					}
					else if (dot < 0f)
					{
						molecule.GetComponent<Rigidbody>().AddForce(-pushDir * 0.01f, ForceMode.Impulse);
					}
				}
			}
		}

		if (!molecule)
		{
			KinDiffuse diffuse = collider.gameObject.GetComponent("KinDiffuse") as KinDiffuse;
			if (diffuse)
			{
				//if (molecule.type == 3)
				{
					Vector3 pushDir = -transform.right;
					float dot = Vector3.Dot(pushDir, (diffuse.transform.position - transform.position));
					if (dot > 0f)
					{
						diffuse.GetComponent<Rigidbody>().AddForce(pushDir * 0.01f, ForceMode.Impulse);
					}
					else if (dot < 0f)
					{
						diffuse.GetComponent<Rigidbody>().AddForce(-pushDir * 0.01f, ForceMode.Impulse);
					}
				}

			}
		}
	}


	// Update is called once per frame
	void Update()
    {
        
    }
}
