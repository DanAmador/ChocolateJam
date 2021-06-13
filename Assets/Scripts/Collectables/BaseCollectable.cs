using System;
using Character;
using map;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Collectables {
	[Serializable]
	public struct ItemStats {
		public float created;
		public float age;
		public ItemState state;
	}

	public enum ItemState {
		PRISTINE,
		DECAYING,
	}

	[RequireComponent(typeof(BoxCollider))]
	public  class BaseItem : MonoBehaviour {
		public UnityEvent onCollected;

		private MeshRenderer _mr;
		protected ObjectSpawner _gm;
		private MapComponent _map;
		[SerializeField, Expandable] protected CollectableProperties itemProperties;
		[SerializeField] protected ItemStats itemStats;


		private UnityEvent itemStateChanged;
		[SerializeField] private float timeUntilDecay;

		private ParticleSystem _pr;

		void OnEnable() {
			itemStats.created = Time.time / 60f;
			itemStats.state = ItemState.PRISTINE;
			timeUntilDecay = float.MaxValue;
		}

		public void Init(MapComponent map, CollectableProperties itemProperties, ObjectSpawner objectSpawner) {
			_map = map;
			_gm = objectSpawner;
			this.itemProperties = itemProperties;
			_mr.enabled = true;
			transform.localScale = Vector3.zero;

			LeanTween.scale(gameObject, Vector3.one, .5f)
				.setOnComplete(() => GetComponent<BoxCollider>().enabled = true);
		}

		private void Awake() {
			_pr = GetComponent<ParticleSystem>();
			_mr = GetComponentInChildren<MeshRenderer>();
			_mr.enabled = true;
			itemStateChanged = new UnityEvent();
			onCollected = new UnityEvent();
		}

		void Update() {
			Age();
		}

		private void Age() {
			itemStats.age = Time.realtimeSinceStartup / 60 - itemStats.created;

			if (itemStats.state != ItemState.DECAYING &&
			    (itemStats.age >= itemProperties.maxLifetime - itemProperties.durationOfDecay ||
			     UnityEngine.Random.Range(0f, 1f) <= itemProperties.chanceOfDecay * Time.deltaTime ||
			     (UnityEngine.Random.Range(0f, 1f) <=
			      itemProperties.chanceOfDecay * Time.deltaTime * 2))) {
				itemStats.state = ItemState.DECAYING;
				itemStateChanged.Invoke();
				timeUntilDecay = itemProperties.durationOfDecay;
			}

			if (itemStats.state == ItemState.DECAYING) {
				timeUntilDecay -= Time.deltaTime;
				if (timeUntilDecay <= 0) {
					TrashMan.despawn(gameObject);
				}
			}
		}


		protected virtual void ApplyCollectable(PlayerComponent player) {
			_gm.SpawnRandomCollectable();
		}

		private void OnTriggerEnter(Collider other) {
			PlayerComponent player;

			if (other.gameObject.TryGetComponent(out player)) {
				ApplyCollectable(player);
				onCollected.Invoke();
				_mr.enabled = false;
				TrashMan.despawnAfterDelay(gameObject, .5f);
			}
		}
	}
}