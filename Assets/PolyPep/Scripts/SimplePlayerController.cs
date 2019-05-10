using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    public float speed = 3.0F;
    public float gravity = 20.0F;

    private Vector3 moveDirection = Vector3.zero;
    public CharacterController controller;

	private Vector2 mouseDirection = Vector2.zero;
	private Vector2 mouseLook = Vector2.zero;
	private Vector2 smoothV = Vector2.zero;
	public float sensitivity;
	public float smoothing;

	public MouseInteraction myMI;
	public Transform lastHit;
	public Vector3 hitPoint;

	void Start()
    {
        // Store reference to attached component
        controller = GetComponent<CharacterController>();

		myMI = GameObject.Find("MouseInteraction").GetComponent<MouseInteraction>();
		
	}

	void UpdateDataFromMouseInteraction()
	{
		

		if (Input.GetMouseButtonDown(2))
		{
			lastHit = myMI.lastHit;
			hitPoint = myMI.hitPoint;
		}


	}

	void UpdateMovement()
	{
		// Character is on ground (built-in functionality of Character Controller)
		if (controller.isGrounded)
		{
			// Use input up and down for direction, multiplied by speed
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		}
		// Apply gravity manually.
		moveDirection.y -= gravity * Time.deltaTime;
		//// Move Character Controller
		controller.Move(moveDirection * Time.deltaTime);



		{
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			mouseDirection = new Vector2(mouseX,mouseY);

			//mouseDirection = Vector2.Scale(mouseDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
			mouseDirection = Vector2.Scale(mouseDirection, new Vector2(sensitivity, sensitivity));
			smoothV.x = Mathf.Lerp(smoothV.x, mouseDirection.x, 1f / smoothing);
			smoothV.y = Mathf.Lerp(smoothV.y, mouseDirection.y, 1f / smoothing);

			mouseLook += smoothV;
			mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

			//Debug.Log(mouseDirection);

			Camera.main.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);

			if (lastHit && Input.GetMouseButton(2))
			{
				Transform target = lastHit;
				//Debug.Log("Mouse1");
				this.transform.LookAt(hitPoint);

			}
			else
			{
				this.transform.rotation = Quaternion.AngleAxis(mouseLook.x, Vector3.up);
			}
				
		}

	}

    void Update()
    {
		
		UpdateMovement();
		UpdateDataFromMouseInteraction();
	}
}


// Ref:
// https://joshuawinn.com/unity-player-controller-top-down-c-sharp-simple-basic-bare-bones/
// https://youtu.be/blO039OzUZc

// https://stackoverflow.com/questions/34117591/c-sharp-with-unity-3d-how-do-i-make-a-camera-move-around-an-object-when-user-mo/48997101#48997101

// Vector3 deltaPointer = Vector3.Project(((pointer.origin + playerControllerDelta) - prevPointer.origin), pointer.direction);