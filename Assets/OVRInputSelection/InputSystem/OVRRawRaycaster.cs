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
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ControllerSelection {
    public class OVRRawRaycaster : MonoBehaviour {
        [System.Serializable]
        public class HoverCallback : UnityEvent<Transform> { }
        [System.Serializable]
        public class SelectionCallback : UnityEvent<Transform, Ray> { }

        [Header("(Optional) Tracking space")]
        [Tooltip("Tracking space of the OVRCameraRig.\nIf tracking space is not set, the scene will be searched.\nThis search is expensive.")]
        public Transform trackingSpace = null;


        [Header("Selection")]
        [Tooltip("Primary selection button")]
        public OVRInput.Button primaryButton = OVRInput.Button.PrimaryIndexTrigger;
        [Tooltip("Secondary selection button")]
        public OVRInput.Button secondaryButton = OVRInput.Button.PrimaryTouchpad;
		[Tooltip("A selection button")]
		public OVRInput.Button aButton = OVRInput.Button.One;
		[Tooltip("B selection button")]
		public OVRInput.Button bButton = OVRInput.Button.Two;
		[Tooltip("Layers to exclude from raycast")]
        public LayerMask excludeLayers;
        [Tooltip("Maximum raycast distance")]
        public float raycastDistance = 500;

		public RawInteraction myRawInteraction;

        [Header("Hover Callbacks")]
        public OVRRawRaycaster.HoverCallback onHoverEnter;
        public OVRRawRaycaster.HoverCallback onHoverExit;
        public OVRRawRaycaster.HoverCallback onHover;

		public OVRRawRaycaster.HoverCallback onHoverADown;
		public OVRRawRaycaster.HoverCallback onHoverBDown;

		[Header("Selection Callbacks")]
        public OVRRawRaycaster.SelectionCallback onPrimarySelect;
        public OVRRawRaycaster.SelectionCallback onSecondarySelect;

		public OVRRawRaycaster.SelectionCallback onPrimarySelectDown;
		public OVRRawRaycaster.SelectionCallback onSecondarySelectDown;

		//protected Ray pointer;
		public Transform lastHit = null;
        public Transform primaryDown = null;
        public Transform secondaryDown = null;
		public Transform aDown = null;
		public Transform bDown = null;

		public Transform remoteGrab = null;
		public float remoteGrabDistance;

		private Vector3 gizmoPos1 = new Vector3(0f, 0f, 0f);
		private Vector3 gizmoPos2 = new Vector3(0f, 0f, 0f);

		//[HideInInspector]
		public OVRInput.Controller activeController = OVRInput.Controller.None;

        void Awake() {
            if (trackingSpace == null) {
                Debug.LogWarning("OVRRawRaycaster did not have a tracking space set. Looking for one");
                trackingSpace = OVRInputHelpers.FindTrackingSpace();
            }
        }

        void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (trackingSpace == null) {
                Debug.LogWarning("OVRRawRaycaster did not have a tracking space set. Looking for one");
                trackingSpace = OVRInputHelpers.FindTrackingSpace();
            }
        }

		void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(gizmoPos1, 0.04f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(gizmoPos2, 0.04f);
		}

		void Update() {
            activeController = OVRInputHelpers.GetControllerForButton(OVRInput.Button.PrimaryIndexTrigger, activeController);
            Ray pointer = OVRInputHelpers.GetSelectionRay(activeController, trackingSpace);

            RaycastHit hit; // Was anything hit?
            if (Physics.Raycast(pointer, out hit, raycastDistance, ~excludeLayers)) {
                if (lastHit != null && lastHit != hit.transform) {
                    if (onHoverExit != null) {
                        onHoverExit.Invoke(lastHit);
                    }
                    lastHit = null;
                }

                if (lastHit == null) {
                    if (onHoverEnter != null) {
                        onHoverEnter.Invoke(hit.transform);
                    }
                }

                if (onHover != null) {
                    onHover.Invoke(hit.transform);
                }

                lastHit = hit.transform;

                // Handle selection callbacks. An object is selected if the button selecting it was
                // pressed AND released while hovering over the object.

                if (activeController != OVRInput.Controller.None) {
                    if (OVRInput.GetDown(secondaryButton, activeController)) {
                        secondaryDown = lastHit;
						//Debug.Log("1");
                    }
                    else if (OVRInput.GetUp(secondaryButton, activeController)) {
                        if (secondaryDown != null && secondaryDown == lastHit) {
                            if (onSecondarySelect != null) {
                                onSecondarySelect.Invoke(secondaryDown, pointer);
								//Debug.Log("2");
							}
                        }
                    }
                    if (!OVRInput.Get(secondaryButton, activeController)) {
                        secondaryDown = null;
						//Debug.Log("3");
					}

                    if (OVRInput.GetDown(primaryButton, activeController)) {
                        primaryDown = lastHit;
						//Debug.Log("4");
					}
                    else if (OVRInput.GetUp(primaryButton, activeController)) {
                        if (primaryDown != null && primaryDown == lastHit) {
                            if (onPrimarySelect != null) {
                                onPrimarySelect.Invoke(primaryDown, pointer);
								//Debug.Log("5");
							}
                        }
                    }
                    if (!OVRInput.Get(primaryButton, activeController)) {
                        primaryDown = null;
						//Debug.Log("6");
					}
				}

				if (lastHit)
				{
					///
					if (OVRInput.Get(aButton, activeController))
					{
						aDown = lastHit;
					}
					else
					{
						aDown = null;
					}
					if (OVRInput.Get(bButton, activeController))
					{
						bDown = lastHit;
					}
					else
					{
						bDown = null;
					}
				}



				if (aDown)
				{
					//Debug.Log("A---->" + aDown);
					onHoverADown.Invoke(aDown);
				}

				if (bDown)
				{
					//Debug.Log("B---->" + bDown);
					onHoverBDown.Invoke(bDown);
				}

				if (primaryDown)
				{
					//Debug.Log(primaryDown);
					onPrimarySelectDown.Invoke(primaryDown, pointer);
				}
				if (secondaryDown)
				{
					//Debug.Log(secondaryDown);
					onSecondarySelectDown.Invoke(secondaryDown, pointer);
				}

#if UNITY_ANDROID && !UNITY_EDITOR
            // Gaze pointer fallback
            else {
                if (Input.GetMouseButtonDown(0) ) {
                    triggerDown = lastHit;
                }
                else if (Input.GetMouseButtonUp(0) ) {
                    if (triggerDown != null && triggerDown == lastHit) {
                        if (onPrimarySelect != null) {
                            onPrimarySelect.Invoke(triggerDown);
                        }
                    }
                }
                if (!Input.GetMouseButton(0)) {
                    triggerDown = null;
                }
            }
#endif

				//REMOTE GRAB
				if (!remoteGrab)
				{
					// remoteGrab not set - looking for candidate
					if (lastHit)
					{
						if (primaryDown && secondaryDown)
						{
							if (lastHit == primaryDown && lastHit == secondaryDown)
							{
								// START remote grabbing
								Debug.Log(lastHit + " is candidate for remoteGrab");
								remoteGrab = lastHit;
								remoteGrabDistance = hit.distance;
								Debug.Log("   --->" + hit.distance);
								gizmoPos1 = hit.point;
							}
						}
					}
				}
				else
				{

				}
			}
            // Nothing was hit, handle exit callback
            else if (lastHit != null) {
                if (onHoverExit != null) {
                    onHoverExit.Invoke(lastHit);
                }
                lastHit = null;
            }

			//REMOTE GRAB outside of hit test
			if (remoteGrab)
			{
				if ( (OVRInput.Get(primaryButton, activeController)) && (OVRInput.Get(secondaryButton, activeController)))
				{
					// still remote grabbing
					gizmoPos2 = (pointer.origin + (remoteGrabDistance * pointer.direction));
					myRawInteraction.RemoteGrabInteraction(primaryDown, gizmoPos2);
					
				}
				else
				{
					//END remote grabbing
					remoteGrab = null;
				}
			}
        }
    }
}