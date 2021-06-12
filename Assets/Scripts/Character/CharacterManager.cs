using System;
using NaughtyAttributes;
using Shadow;
using UnityEngine;

namespace Character {
	[RequireComponent(typeof(PathRecorder))]
	public class CharacterManager : MonoBehaviour {
		public float speed;
		public int health;


		private PathRecorder pr;
		[SerializeField]
		private ShadowPathPatrol nextSpawn;

		private void Start() {
			pr = GetComponent<PathRecorder>();
			pr.shadowPath = nextSpawn.pathToFollow;
			pr.StartRecording();
		}


		void Update() { }


		[Button()]
		private void SpawnShadow() {
			pr.StopRecording();
			//TODO spawn from trashcan

			nextSpawn.enabled = true;
			// nextSpawn
		}
	}
}