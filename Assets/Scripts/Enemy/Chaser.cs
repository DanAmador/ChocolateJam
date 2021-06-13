using System;
using Character;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy {
	public class Chaser : MonoBehaviour {
		private NavMeshAgent _agent;

		public PlayerComponent player;
		public float Distance => Vector3.Distance(player.transform.position, transform.position);
		public float TimeSinceLastSample => Time.time - _lastSampleTime;

		public float distanceLimit;

		private float _lastSampleTime;

		void Start() {
			_agent = GetComponent<NavMeshAgent>();
			_agent.destination = Random.insideUnitSphere * distanceLimit;
			_lastSampleTime = Time.time;
		}

		private void OnEnable() {
			GameObject p = GameObject.Find("CharacterRoot");
			player = p.GetComponentInChildren<PlayerComponent>();
		}

		void Update() {
			if (Distance < distanceLimit) {
				_agent.destination = player.transform.position;
				_lastSampleTime = Time.time;
			}
			else {
				if (TimeSinceLastSample > 5) {
					_agent.destination = Random.insideUnitSphere * distanceLimit;
					_lastSampleTime = Time.time;
				}
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (other.gameObject.TryGetComponent(out player)) {
				player.ChangeHealth(-1);
				TrashMan.despawn(gameObject);
			}
			
		}

		private void OnDrawGizmosSelected() {
			Gizmos.DrawSphere(transform.position, distanceLimit);
		}
	}
}