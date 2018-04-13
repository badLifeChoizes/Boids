using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner - Attached to the Main Camera. 
/// Spawner stores the fields(variables) that are shared by all Boids and instantiates all the instances of Boid prefab
/// </summary>
public class Spawner : MonoBehaviour {

	//This is a Singleton of the BoidSpawner. There is only one instance
	//of BoidSpawner, so we can store it in a static variable named S.
	static public Spawner S;
	static public List<Boid> boids;

	//These fields allow you to adjust the spawning behavior of the Boids
	[Header("Set in Inspector: Spawning")]
	public GameObject boidPrefab;
	public Transform boidAnchor;
	public int numBoids = 100;
	public float spawnRadius = 100f;
	public float spawnDelay = 0.1f;

	//These fields allow you to adjust the flocking behavior of the Boids
	[Header("Set in Inspector: Boid's Flocking Behaviour")]
	public float velocity = 30f;
	public float neighborDist = 30f;
	public float collDist = 4f;
	public float velMatching = 0.25f;
	public float flockCentering = 0.2f;
	public float collAvoid = 2f;
	public float attractPull = 2f;
	public float attractPush = 2f;
	public float attractPushDist = 5f;

	void Awake(){
		//Set the Singleton S to be this instance of BoidSpawner
		S = this;//referes to the current instance of the class
		//start instantiation of Boids
		boids = new List<Boid>();// holds a reference to all boids instantiated by Spawner
		InstantiateBoid ();
	}

	public void InstantiateBoid(){
		GameObject go = Instantiate (boidPrefab);
		Boid b = go.GetComponent<Boid> ();
		b.transform.SetParent (boidAnchor);//place all Boids as children of BoidAnchor
		boids.Add (b);
		if (boids.Count < numBoids) {
			Invoke ("InstantiateBoid", spawnDelay);//pars: (method name as String, amount of time to wait befor calling)
		}
	}
}
