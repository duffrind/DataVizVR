using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	public float heightMovementFactor = 2.5f;
	public float mouseSensitivy = 1.5f;

	// Update is called once per frame
	void Update () 
	{
		float heightWeight = heightMovementFactor * transform.position.y;

		Vector3 movement = Vector3.zero;

		// Forward / camera movement.
		movement += Input.GetAxis ("Mouse ScrollWheel") * Vector3.forward * heightWeight;

		// X/Z movement
		if( Input.GetMouseButton(0) ){
			movement += -Input.GetAxis ("Mouse X") * Vector3.right * heightWeight * mouseSensitivy;
			movement += -Input.GetAxis ("Mouse Y") * Vector3.up * heightWeight * mouseSensitivy;
		}

		transform.Translate (movement);
	}
}
