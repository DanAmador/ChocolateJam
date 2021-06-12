using System;
using System.Collections;
using System.Collections.Generic;
using Collectables;
using UnityEngine;

[Serializable]
public struct CollectableSpawn
{
    public string name;
    public CollectableProperties properties;
    public BaseItem prefab;
    public int number;
}

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public List<CollectableSpawn> collectableSpawnSettings;
}
