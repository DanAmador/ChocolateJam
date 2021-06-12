using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private MapComponent map;
	[SerializeField, Expandable] private GameSettings gameSettings;
	private Transform collectables;

	void Start() {
		SpawnCollectables();
	}

	private void SpawnCollectables() {
		collectables = Instantiate(new GameObject("collectables")).transform;

		foreach (CollectableSpawn collectableSpawn in gameSettings.collectableSpawnSettings) {
			for (int i = 0; i < collectableSpawn.number; i++) {
				Vector3 pos = map.GetRandomPosition();
				Quaternion rot = new Quaternion();
				rot.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

				BaseItem v = Instantiate(collectableSpawn.prefab, pos, rot).GetComponent<BaseItem>();
				v.transform.parent = collectables.transform;
				v.name = collectableSpawn.name + "." + i;
				v.Init(map, collectableSpawn.properties);
			}
		}
	}
}
