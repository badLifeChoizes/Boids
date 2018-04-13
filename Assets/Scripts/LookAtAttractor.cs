using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LookAtAttractor - Attached to the Main Camera, this script causes the camera to turn and look at the Attractor each frame.
/// </summary>
public class LookAtAttractor : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.LookAt (Attractor.POS);
	}
}
