using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Shadow;
using UnityEngine;

namespace Character {
	[RequireComponent(typeof(PathRecorder), typeof(LineRenderer))]
	public class PlayerComponent : MonoBehaviour {
		private float _currentSpeed;
		public int health;

		public float maxSpeed = 15;
		public GameObject shadowPrefab;
		private PathRecorder _pr;
		private ShadowPathPatrol _nextSpawn;
		private float _maxSpecial = 15;

		private float _currentSpecial;
		[Range(.5f, 1f)] public float boostSpeed;

		public bool isBoosting;

		public float NormalizedSpecial => _currentSpecial / _maxSpecial;
		public float SpeedWithBoost => _currentSpeed + (isBoosting ? boostSpeed : 0) * maxSpeed;

		public bool CanBoost => NormalizedSpecial > 0.1f;

		private TrailRenderer _tr;

		private void Awake() {
			_pr = GetComponent<PathRecorder>();
			_tr = GetComponent<TrailRenderer>();
		}

		private void Start() {
			_currentSpeed = maxSpeed / 2;
			_currentSpecial = _maxSpecial / 2;
			_nextSpawn = TrashMan.spawn(shadowPrefab).GetComponent<ShadowPathPatrol>();
			_nextSpawn.gameObject.SetActive(false);
			_pr.shadowPath = _nextSpawn.pathToFollow;
			_pr.StartRecording();
		}

		private void Update() {
			// Debug.Log(NormalizedSpecial);
			BoostCheck();
			
		}

		public void BoostCheck() {
			if (isBoosting) {
				_currentSpecial = Mathf.Clamp(_currentSpecial - _maxSpecial * .3f * Time.deltaTime, 0, _currentSpeed);
				isBoosting = CanBoost;
			}

			_tr.emitting = isBoosting;
			
			_currentSpecial = Mathf.Clamp(_currentSpecial + .3f * Time.deltaTime, 0, _maxSpecial);
		}

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