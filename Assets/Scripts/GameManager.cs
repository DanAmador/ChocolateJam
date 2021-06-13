using NaughtyAttributes;
using Collectables;
using UnityEngine;

public class GameManager : MonoBehaviour {
	[SerializeField] private MapComponent map;
	[SerializeField, Expandable] private GameSettings gameSettings;
	private Transform _collectables;

	private CollectableSpawn _score;

	void Awake() {
		_collectables = Instantiate(new GameObject("Collectables")).transform;
		SpawnCollectables();
	}

	private void SpawnCollectables() {
		foreach (CollectableSpawn collectableSpawn in gameSettings.collectableSpawnSettings) {
			if (collectableSpawn.name == "Score") {
				_score = collectableSpawn;
			}

			for (int i = 0; i < collectableSpawn.number; i++) {
				Spawn(collectableSpawn);
			}
		}
	}


	public void SpawnScore() {
		Spawn(_score);
	}

	public void SpawnRandom() {
		int idx = Random.Range(0, gameSettings.collectableSpawnSettings.Count);
		Spawn(gameSettings.collectableSpawnSettings[idx]);
	}

	private void Spawn(CollectableSpawn collectableSpawn) {
		Vector3 pos = map.GetRandomPosition();
		Quaternion rot = new Quaternion();
		rot.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
		BaseItem v = TrashMan.spawn(collectableSpawn.prefab).GetComponent<BaseItem>();
		Transform t = v.transform;
		t.parent = _collectables.transform;
		t.position = pos;
		t.rotation = rot;
		v.name = collectableSpawn.prefab.name;
		t.localScale = Vector3.zero;
		
		v.Init(map, collectableSpawn.properties, this);
	}
}