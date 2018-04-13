using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BOID - Attached to the boid prefab, its job is to handle the movement of each individual Boid.
/// Because this is OOP each boid will think for itself and react to its own individual understanding of the world
/// </summary>
public class Boid : MonoBehaviour {

	// the GetComponent reference is a bit time consuming, so for performance, it's important to cache a
	//reference to the Rigidbody component. the rigid field allows us to avoid calling GetCompnent every frame.
	[Header("Set Dynamically")]
	public Rigidbody rigid; 

	private Neighborhood neighborhood; //reference to the Neighborhood class

	//Use this for initialization
	void Awake(){
		neighborhood = GetComponent<Neighborhood> ();
		rigid = GetComponent<Rigidbody> (); 

		//set a random initial position
		//  insideUnitSphere static property generates a random vector3 inside a sphere within a radius of 1 unit
		// multiply by SpawnRadius field of the spawner. The resultant Vector3 is assigned to the pos property
		pos = Random.insideUnitSphere * Spawner.S.spawnRadius; 

		//set a random initial velocity
		//onUnitSphere generates a random vector3 on the surface of a unitSphere
		//multiply by velocity field set on the spawner and assign result to the velocit of the boids rigidbody component.
		Vector3 vel = Random.onUnitSphere * Spawner.S.velocity; //c
		rigid.velocity = vel;

		LookAhead ();//orient the boid to face in the direction of its rigid.velocity

		//Give the Boid a random color, but make sure its not too dark
		Color randColor = Color.black;
		while (randColor.r + randColor.g + randColor.b < 1.0f) {
			randColor = new Color (Random.value, Random.value, Random.value);
		}
		//returns an array of all the rendere components attached to this Boid GameObject and its children(Fuselage/Wings).
		Renderer[] rends = gameObject.GetComponentsInChildren<Renderer> (); 
		foreach (Renderer r in rends) {
			r.material.color = randColor;
		}
		TrailRenderer tRend = GetComponent<TrailRenderer> ();
		tRend.material.SetColor ("_TintColor", randColor);
	}

	void LookAhead(){					//d
		//orients the Boid to look at the direction it's flying
		transform.LookAt(pos + rigid.velocity);
	}

	public Vector3 pos{			//b
		get{ return transform.position; }
		set{ transform.position = value;}
	}

	//called oncer per physics update(i.e., 50x/second)
	void FixedUpdate(){
		Vector3 vel = rigid.velocity;
		Spawner spn = Spawner.S;

		//COLLLISION AVOIDANCE - Avoid neighbors ho are too close
		Vector3 velAvoid = Vector3.zero;
		Vector3 tooClosePos = neighborhood.avgClosePos;
		//If the response is Vector3.zero, then no need to react
		if (tooClosePos != Vector3.zero) {
			velAvoid = pos - tooClosePos;
			velAvoid.Normalize ();
			velAvoid *= spn.velocity;
		}

		//VELOCITY MATCHING - Try to match velocity with neighbors
		Vector3 velAlign = neighborhood.avgVel;
		//Only do more if the velAlign is not Vector3.zer
		if (velAlign != Vector3.zero) {
			//We're really interested in direction, so normalize he velocit
			velAlign.Normalize();
			//set it to the chosen speed
			velAlign *= spn.velocity;
		}

		//FLOCK CENTERING - Move towards the enter of local neighbors
		Vector3 velCenter = neighborhood.avgPos;
		if (velCenter != Vector3.zero) {
			velCenter -= transform.position;
			velCenter.Normalize ();
			velCenter *= spn.velocity;
		}

		//ATTRATION - Move towards the attractor
		//delta = Attractor position = Boid position
		Vector3 delta = Attractor.POS - pos;
		//check wether we're attracted or avoiding the Attractor
		bool attracted = (delta.magnitude > spn.attractPushDist);
		Vector3 velAttract = delta.normalized * spn.velocity;

		//Apply all the velocities
		float fdt = Time.fixedDeltaTime;
		if (velAvoid != Vector3.zero) {
			vel = Vector3.Lerp (vel, velAvoid, spn.collAvoid * fdt);
		} else {
			if (velAlign != Vector3.zero) {
				vel = Vector3.Lerp (vel, velAlign, spn.velMatching * fdt);
			}
			if (velCenter != Vector3.zero) {
				vel = Vector3.Lerp (vel, velAlign, spn.flockCentering * fdt);
			}
			if (velAttract != Vector3.zero) {
				if (attracted) {   //if the boids position is far enough away from the attractor to be attracted to it
					vel = Vector3.Lerp (vel, velAttract, spn.attractPull * fdt);
				} else {		//boid is too close to attractor then vel will interplate toward the opposite direction of velAttract
					vel = Vector3.Lerp (vel, -velAttract, spn.attractPush * fdt);
				}
			}
		}

		//set vel to the velocity set on the Spawner singleton
		vel = vel.normalized * spn.velocity;
		//Finally assign this to the Rigidbody
		rigid.velocity = vel;
		//Look in the direction of the new velocity
		LookAhead();
	}
}
	
