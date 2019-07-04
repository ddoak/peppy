using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FrontEndMenu : MonoBehaviour
{
    
	public void LoadSceneVR()
	{
		SceneManager.LoadScene("Splash");
	}

	public void LoadSceneNonVR()
	{
		SceneManager.LoadScene("Peppy_nonVR");
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
		{
			LoadSceneVR();
		}

	}
}
