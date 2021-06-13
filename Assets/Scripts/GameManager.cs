using System;
using NaughtyAttributes;
using Collectables;
using map;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ObjectSpawner))]
public class GameManager : MonoBehaviour {

	private ObjectSpawner _objectSpawner;


	public float timeToSpawnChaser = 20;

	private float _lastChaserSpawn;
	public float TimeSinceLastChaser => Time.time - _lastChaserSpawn;
	private NavMeshSurface _surface;
	void Awake() {
		_objectSpawner = GetComponent<ObjectSpawner>();
		_lastChaserSpawn = Time.time;
	}

	private void Start() {
		_objectSpawner.SpawnChaser();
		_surface = GetComponentInChildren<NavMeshSurface>();
		_surface.BuildNavMesh();

	}

	public void Update() {
		if (TimeSinceLastChaser > timeToSpawnChaser) {
			_lastChaserSpawn = Time.time;
			_objectSpawner.SpawnChaser();
		}	
	}
}