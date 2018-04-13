using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Neighborhood - Attached to the Boid prefab, it keeps track of which other Boids are nearby.
/// Key to eah Boids individual understanding of the world is its knowledge of which other Boids are close enough to worry about
/// tracks which other boids are near this one and give infor about them, including average position an average velocity of all
/// nearby boids and which boids are too close
/// </summary>
public class Neighborhood : MonoBehaviour {

	[Header("Set Dynamically")]
	public List<Boid> neighbors;
	private SphereCollider coll;

	// Use this for initialization
	void Start () {
		neighbors = new List<Boid> ();
		//get reference of sphere collider
		coll = GetComponent<SphereCollider> ();
		//set the radius of sphere collider to half of the spawner singletons neighborDist. 
		//It is half because the neghborDist is the disstance at which we want these GameObjects to
		//see each other, so a radius of 1/2 the distance will just barely touch at exatly the neighbor dist
		coll.radius = Spawner.S.neighborDist / 2;
	}

	//chek is neighborDist has changed , if so change the radius of the Sphere Collider
	void FixedUpdate(){
		if (coll.radius != Spawner.S.neighborDist / 2) {
			coll.radius = Spawner.S.neighborDist / 2;
		}
	}

	//called when somehting else enters this sphereCollider trigger(a collider that allows other things to pass through it)
	//other boids should be the only things that have colliders on them, but to be sure, we GetComponent<Boid>() on the other
	//collider and only continue if the resullt is null. At that point if boid has moved within the neighborhood and is not yet in
	//neighbors list add it. If another boid is no longer touching the trigger when OnTriggerExit called remove boid from list
	void OnTriggerEnter(Collider other){
		Boid b = other.GetComponent<Boid> ();
		if (b != null) {
			if (neighbors.IndexOf (b) != -1) {
				neighbors.Remove (b);
			}
		}
	}

	//returns the average position of all boids in the neighbors list
	public Vector3 avgPos{
		get{
			Vector3 avg = Vector3.zero;
			if (neighbors.Count == 0)
				return avg;

			for(int i = 0; i < neighbors.Count; i++){
				avg += neighbors[i].pos;
			}
			avg /= neighbors.Count;

			return avg;
		}
	}

	//returns the average velocity of all neighbor boids
	public Vector3 avgVel{
		get{
			Vector3 avg = Vector3.zero;
			if (neighbors.Count == 0)
				return avg;

			for (int i = 0; i < neighbors.Count; i++) {
				avg += neighbors [i].rigid.velocity;
			}
			avg /= neighbors.Count;

			return avg;
		}
	}

	//returns the average position of all neighbor boids that are within the collisionDist(from the Spawner singleton)
	public Vector3 avgClosePos{
		get{
			Vector3 avg = Vector3.zero;
			Vector3 delta;
			int nearCount = 0;
			for (int i = 0; i < neighbors.Count; i++) {
				delta = neighbors [i].pos - transform.position;
				if (delta.magnitude <= Spawner.S.collDist) {
					avg += neighbors [i].pos;
					nearCount++;
				}
			}
			//If there were no neighbors too close, return Vector3.zer
			if(nearCount == 0) return avg;

			//Otherwise, average their locations
			avg /= nearCount;
			return avg;
		}
	}
}
