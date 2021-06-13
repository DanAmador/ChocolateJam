using System;
using UnityEngine;

namespace Character {
	public class ParticleManager : MonoBehaviour {
		public TrailRenderer top, bottom;
		public ParticleSystem particles;
		public float maxTime = 5;

		private float _interpolation;
		public void Start() {
			SetTrail(false);
		}

		public void SetTrail(bool isBoosting) {
			float b = Mathf.Lerp(0, maxTime, _interpolation);
			top.time = b;
			bottom.time = b;
			_interpolation= Mathf.Clamp01(_interpolation+ (isBoosting ? Time.deltaTime : -Time.deltaTime));

			ParticleSystem.EmissionModule particlesEmission = particles.emission;
			particlesEmission.enabled = isBoosting;
		}
	}
}