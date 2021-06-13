using System;
using System.Linq;
using Character;
using NaughtyAttributes;
using UnityEngine;

namespace Shadow {
	[RequireComponent(typeof(ShadowPath))]
	public class ShadowPathPatrol : MonoBehaviour {
		public ShadowPath pathToFollow;


		public ShadowPathPatrol child;

		public int refreshRate;
		public float lastCollisionTime;
		private int _refreshIdx;


		private SphereCollider _sphereCollider;
		private Collider[] _hitColliders;
		[SerializeField] private GameObject shadowPrefab;
		public ShadowPathPatrol parent1, parent2;

		private ShadowPathPatrol _other;
		private bool CanCollide => Time.time - lastCollisionTime > pathToFollow.Duration / 2f;

		public float speed = 5;
		public float recursionDepth = 1;

		private void Start() {
			pathToFollow = GetComponent<ShadowPath>();
			_sphereCollider = GetComponent<SphereCollider>();
			_hitColliders = new Collider[10];
		}


		private void Update() {
			_refreshIdx++;

			if (_refreshIdx > refreshRate && CanCollide) {
				CheckCollision();
				_refreshIdx = 0;
			}
		}

		private void CheckCollision() {
			int numColliders =
				Physics.OverlapSphereNonAlloc(transform.position, _sphereCollider.radius * 1f, _hitColliders);
			for (int i = 0; i < numColliders; i++) {
				if (_hitColliders[i] != null) {
					if (_hitColliders[i].TryGetComponent(out _other)) {
						if (_other != this) {
							SpawnChild(this, _other, (this.transform.position + _other.transform.position) / 2);
							break;
						}
						else {
							_other = null;
						}
					}
				}
			}
		}

		private void CalculateParentsMix() {
			Vector3[] remainingPoints1 = parent1.pathToFollow.RemainingOffsets();
			Vector3[] remainingPoints2 = parent2.pathToFollow.RemainingOffsets();
			float remainingTimeAvg = (parent1.pathToFollow.RemainingTime() + parent2.pathToFollow.RemainingTime()) / 2;
			int amountOfElements = Math.Max(remainingPoints1.Length, remainingPoints2.Length);

			Vector3[] newOffsets = new Vector3[amountOfElements];
			float totalDistance = 0;
			Vector3 offset;
			for (int i = 0; i < amountOfElements; i++) {
				offset = remainingPoints1.ElementAtOrDefault(i) + remainingPoints2.ElementAtOrDefault(i);
				totalDistance += offset.magnitude;
				newOffsets[i] = offset;
			}

			
			float time = totalDistance / (speed * recursionDepth);
			pathToFollow.UpdatePath(newOffsets, time);
		}

		[Button()]
		public void UpdatePathFromParents() {
			if (parent1 != null && parent2 != null) {
				CalculateParentsMix();
			}
		}

		public void Spawn() {
			gameObject.SetActive(true);
			;
			// pathToFollow.ClosePath();
		}

		private void SpawnChild(ShadowPathPatrol p1, ShadowPathPatrol p2, Vector3 transformPosition) {
			if (child == null) {
				child = TrashMan.spawn(shadowPrefab).GetComponent<ShadowPathPatrol>();
				child.parent1 = p1;
				child.parent2 = p2;
				child.recursionDepth = Mathf.CeilToInt((p1.recursionDepth + p2.recursionDepth) / 2) + 1;
				child.transform.position = transformPosition;
				child.UpdatePathFromParents();
				p1.child = child;
				p2.child = child;
				p1.pathToFollow.splineDescription.pause();
				p2.pathToFollow.splineDescription.pause();
				p1.gameObject.SetActive(false);
				p2.gameObject.SetActive(false);
				child.pathToFollow.onCompoundPathLoop.AddListener(child.ActivateAndFlip);
			}
		}

		private void ActivateAndFlip(ShadowPathPatrol p) {
			p.child = null;
			p.pathToFollow.splineDescription.direction *= -1;
			p.pathToFollow.splineDescription.resume();
			p.lastCollisionTime = Time.time;
			p.gameObject.SetActive(true);
		}

//Callback for compound paths that reactivates the parents and flips the direction
		public void ActivateAndFlip() {
			ActivateAndFlip(parent2);
			ActivateAndFlip(parent1);
			parent1 = null;
			parent2 = null;

			gameObject.SetActive(false);
			TrashMan.despawn(gameObject);
		}
	}
}