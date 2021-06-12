using System;
using NaughtyAttributes;
using Shadow;
using UnityEngine;

namespace Character {
	[RequireComponent(typeof(PathRecorder))]
	public class PlayerComponent : MonoBehaviour {
		public float speed;
		public int health;

		public GameObject shadowPrefab;
		private PathRecorder _pr;
		private ShadowPathPatrol _nextSpawn;

		
		
		private void Start() {
			_pr = GetComponent<PathRecorder>();
			_nextSpawn = TrashMan.spawn(shadowPrefab).GetComponent<ShadowPathPatrol>();
			_nextSpawn.gameObject.SetActive(false);
			_pr.shadowPath = _nextSpawn.pathToFollow;
			_pr.StartRecording();
		}


		void Update() { }


		[Button()]
		public void SpawnShadow() {
			_pr.StopRecording();

			_nextSpawn.Spawn();
			_nextSpawn = TrashMan.spawn(shadowPrefab).GetComponent<ShadowPathPatrol>();
			_nextSpawn.gameObject.SetActive(false);
			_pr.shadowPath = _nextSpawn.pathToFollow;
			_pr.StartRecording();
		}

		public void AddHealth() {
			health = Mathf.Clamp(++health, 0, 3);
		}
	}
}