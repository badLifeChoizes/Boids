using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attractor - The boids need something to flock around, this script will serve that purpose.
/// </summary>
public class Attractor : MonoBehaviour {
	//As a static variable, POS is shared by all instances of Attractor(though in this case, there will only be 1 Instance
	//When a field is static, it is scoped to the class itself rather than any instance of the class. This means POS
	//is class variable rather than an instance field so as long as both POS and the Attractor class are public,
	//any instance of any other class can access POS via Attractor.POS
	static public Vector3 POS = Vector3.zero;

	[Header("Set in Inspector")]
	public float radius = 10;
	public float xPhase = 0.5f;
	public float yPhase = 0.4f;
	public float zPhase = 0.1f;

	//FixedUpdate is called once per physics update(i,e, 50x/second)
	void FixedUpdate(){

		//Sin waves are often used for cyclical movement
		//here the various phase fields(e.g. xPhase) cause 
		//the Attractor to move around the scene with each axis(X,Y,Z) 
		//slightly out of phase with the others.
		Vector3 tPos = Vector3.zero;
		Vector3 scale = transform.localScale;
		tPos.x = Mathf.Sin (xPhase * Time.time) * radius * scale.x;
		tPos.y = Mathf.Sin (yPhase * Time.time) * radius * scale.y;
		tPos.z = Mathf.Sin (zPhase * Time.time) * radius * scale.z;

		transform.position = tPos; //assignthe transforms position to the value of tPos
		POS = tPos;
	}
}
	