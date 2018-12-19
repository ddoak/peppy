using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SnapshotCamera : MonoBehaviour {

	public int FileCounter = 0;

	// Use this for initialization
	void Start () {
		
	}

	public void CamCapture()
	{
		Camera Cam = GetComponent<Camera>();

		RenderTexture currentRT = RenderTexture.active;
		RenderTexture.active = Cam.targetTexture;

		Cam.Render();

		Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
		Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
		Image.Apply();
		RenderTexture.active = currentRT;

		var Bytes = Image.EncodeToPNG();
		Destroy(Image);

		string directoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/PeppySnapshots";

		//check if directory doesn't exit
		if (!Directory.Exists(directoryPath))
		{
			//if it doesn't, create it
			Directory.CreateDirectory(directoryPath);

		}
	
		//File.WriteAllBytes(Application.dataPath + "/Snapshots/" + FileCounter + ".png", Bytes);
		File.WriteAllBytes(directoryPath + "/PeppySnapshot_" + FileCounter + ".png", Bytes);

		FileCounter++;

		
	}

	// Update is called once per frame
	void Update () {
		
	}
}

// https://forum.unity.com/threads/how-to-save-manually-save-a-png-of-a-camera-view.506269/
