using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		transform.Rotate(1 * Time.deltaTime, 1 * Time.deltaTime, 1 * Time.deltaTime); //rotates 50 degrees per second around z axis
	}
}
