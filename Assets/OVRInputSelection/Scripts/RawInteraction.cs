/************************************************************************************

Copyright   :   Copyright 2017-Present Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;


public class RawInteraction : MonoBehaviour {
    protected Material oldHoverMat;
    public Material yellowMat;
    public Material backIdle;
    public Material backACtive;
    public UnityEngine.UI.Text outText;

    public void OnHoverEnter(Transform t) {
        if (t.gameObject.name == "BackButton") {
            t.gameObject.GetComponent<Renderer>().material = backACtive;
        }
        else {
			//Debug.Log("---> " + t);
			GameObject go =  t.gameObject;
			BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
			if (bu != null)
			{
				//Debug.Log("      --> script");
				bu.SetBackboneUnitControllerHover(true);
			}

			//oldHoverMat = t.gameObject.GetComponent<Renderer>().material;
            //t.gameObject.GetComponent<Renderer>().material = yellowMat;

        }
        if (outText != null) {
            outText.text = "<b>Last Interaction:</b>\nHover Enter:" + t.gameObject.name;
        }
    }

    public void OnHoverExit(Transform t) {
        if (t.gameObject.name == "BackButton") {
            t.gameObject.GetComponent<Renderer>().material = backIdle;
        }
        else {
			//Debug.Log("---> " + t);
			GameObject go = t.gameObject;
			BackboneUnit bu = (go.GetComponent("BackboneUnit") as BackboneUnit);
			if (bu != null)
			{
				//Debug.Log("      --> script");
				bu.SetBackboneUnitControllerHover(false);
			}
			//t.gameObject.GetComponent<Renderer>().material = oldHoverMat;
		}
        if (outText != null) {
            outText.text = "<b>Last Interaction:</b>\nHover Exit:" + t.gameObject.name;
        }
    }

    public void OnPrimarySelected(Transform t) {
        if (t.gameObject.name == "BackButton") {
            SceneManager.LoadScene("main", LoadSceneMode.Single);
        }
        //Debug.Log("Clicked on " + t.gameObject.name);
        if (outText != null) {
            outText.text = "<b>Last Interaction:</b>\nClicked On:" + t.gameObject.name;
        }
    }

	public void OnSecondarySelected(Transform t)
	{
		if (t.gameObject.name == "BackButton")
		{
			SceneManager.LoadScene("main", LoadSceneMode.Single);
		}
		//Debug.Log("Secondary Clicked on " + t.gameObject.name);
		if (outText != null)
		{
			outText.text = "<b>Last Interaction:</b>\nClicked On:" + t.gameObject.name;
		}
	}

	public void OnPrimarySelectedButtonDown(Transform t)
	{
		Debug.Log("Primary Select Button Down" + t.gameObject.name);
	}

	public void OnSecondarySelectedButtonDown(Transform t)
	{
		Debug.Log("Secondary Select Button Down" + t.gameObject.name);
	}

}
