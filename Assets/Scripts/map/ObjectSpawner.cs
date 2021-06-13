using System.Collections;
using System.Linq;
using Collectables;
using NaughtyAttributes;
using UnityEngine;

namespace map {
	public class ObjectSpawner : MonoBehaviour {
		private MapComponent map;
		[SerializeField, Expandable] private GameSettings gameSettings;
		private Transform _collectables;

		[SerializeField] private GameObject scorePrefab;
		[SerializeField] private GameObject chaserPrefab;
		private CollectableSpawn _scoreCollectable;
		void Awake() {
			_collectables = Instantiate(new GameObject("Collectables")).transform;
			map = GetComponentInChildren<MapComponent>();
			_scoreCollectable = new CollectableSpawn();
			_scoreCollectable.prefab = scorePrefab;
			_scoreCollectable.properties = gameSettings.collectableSpawnSettings.First().properties;
			StartCoroutine(SpawnAfterDelay());
		}

		private IEnumerator SpawnAfterDelay() {
			yield return new WaitForSeconds(3);
			SpawnCollectables();
		}

		private void SpawnCollectables() {
			foreach (CollectableSpawn collectableSpawn in gameSettings.collectableSpawnSettings) {
				for (int i = 0; i < collectableSpawn.number; i++) {
					Spawn(collectableSpawn);
				}
			}

			//Sleep deprivation, fuck loops
			SpawnScore();
			SpawnScore();
			SpawnScore();
			SpawnScore();

		}

		public void SpawnScore() {
			Spawn(_scoreCollectable);
		}

		public void SpawnChaser() {
			Spawn(chaserPrefab);
		}

		public void SpawnRandomCollectable() {
			int idx = Random.Range(0, gameSettings.collectableSpawnSettings.Count);
			Spawn(gameSettings.collectableSpawnSettings[idx]);
		}

		private GameObject Spawn(GameObject obj) {
			Vector3 pos = map.GetRandomPosition();
			Quaternion rot = new Quaternion();
			rot.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
			Transform t = TrashMan.spawn(obj).transform;
			t.position = pos;
			t.rotation = rot;
			t.name = obj.name;
			return t.gameObject;
		}

		private void Spawn(CollectableSpawn collectableSpawn) {
			GameObject spawnedCollectable = Spawn(collectableSpawn.prefab);
			BaseItem v = spawnedCollectable.GetComponent<BaseItem>();
			v.transform.parent = _collectables.transform;
			v.name = collectableSpawn.prefab.name;
			v.Init(map, collectableSpawn.properties, this);
		}
	}
}