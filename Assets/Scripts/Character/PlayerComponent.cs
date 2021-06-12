using System;
using NaughtyAttributes;
using Shadow;
using UnityEngine;

namespace Character {
	[RequireComponent(typeof(PathRecorder))]
	public class PlayerComponent : MonoBehaviour {
		public float speed;
		public int health;


		private PathRecorder _pr;
		[SerializeField] private ShadowPathPatrol nextSpawn;

		private void Start() {
			_pr = GetComponent<PathRecorder>();
			_pr.shadowPath = nextSpawn.pathToFollow;
			_pr.StartRecording();
		}


		void Update() { }


		[Button()]
		public void SpawnShadow() {
			Debug.Log("nigga");
			// _pr.StopRecording();
			// TODO spawn from trashcan

			// nextSpawn.enabled = true;
			// nextSpawn
		}

		public void AddHealth() {
			health = Mathf.Clamp(++health, 0, 3);
		}
	}
}