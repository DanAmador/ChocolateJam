using System;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using Shadow;
using UnityEngine;
using UnityEngine.Events;

namespace Character {
	[RequireComponent(typeof(PathRecorder))]
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
		public UnityEvent onDieEvent = new UnityEvent();
		[SerializeField] private CinemachineVirtualCamera _camera;

		public float maxFov, defaultFov, fovInterpolation;
		[SerializeField] private ParticleManager particleManager;

		private void Awake() {
			_pr = GetComponent<PathRecorder>();
			fovInterpolation = 0;
			defaultFov = _camera.m_Lens.FieldOfView;
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


			_camera.m_Lens.FieldOfView = Mathf.Lerp(defaultFov, maxFov, fovInterpolation);
			fovInterpolation = Mathf.Clamp01(fovInterpolation + (isBoosting ? Time.deltaTime : -Time.deltaTime));

			particleManager.SetTrail(isBoosting);

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

		public void ChangeHealth(int amount) {
			if (amount < 0 ) {
				CinemachineShake.Instance.ShakeCamera(4,.1f);
			}
			health = Mathf.Clamp(amount + health, 0, 3);

			if (health == 0) onDieEvent.Invoke();
		}

		public void AddSpeed(float val) {
			_currentSpecial = Mathf.Clamp(_currentSpecial + val, 0.1f, _maxSpecial);
		}
	}
}