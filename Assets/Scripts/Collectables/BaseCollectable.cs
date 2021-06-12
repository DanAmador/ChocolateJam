using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseItem : MonoBehaviour
{
    public UnityEvent onCollected;

    private MapComponent map;
    [SerializeField, Expandable] protected CollectableProperties itemProperties;
    [SerializeField] protected ItemStats itemStats;
    [Serializable]
    public struct ItemStats
    {
        public float created;
        public float age;
        public ItemState state;

    }
    public enum ItemState
    {
        PRISTINE,
        USED,
        DECAYING,
    }

    private UnityEvent itemStateChanged;
    [SerializeField]
    private float timeUntilDecay;

    void OnEnable() {
        itemStats.created = Time.time / 60f;
        itemStats.state = ItemState.PRISTINE;
        timeUntilDecay = float.MaxValue;
    }

    public virtual void Init(MapComponent map, CollectableProperties itemProperties) {
        this.map = map;
        this.itemProperties = itemProperties;
    }

    private void Start() {
        itemStateChanged = new UnityEvent();
        itemStateChanged.AddListener(OnItemStateChanged);
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
            (itemStats.state == ItemState.USED && UnityEngine.Random.Range(0f, 1f) <= itemProperties.chanceOfDecay * Time.deltaTime * 2))) {
            itemStats.state = ItemState.DECAYING;
            itemStateChanged.Invoke();
            timeUntilDecay = itemProperties.durationOfDecay;
        }

        if (itemStats.state == ItemState.DECAYING) {
            timeUntilDecay -= Time.deltaTime;
            if (timeUntilDecay <= 0) {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnItemStateChanged() { }

    protected virtual void ApplyCollectable(GameObject player) { }


    public void Collect(GameObject player) {
        ApplyCollectable(player);
        onCollected.Invoke();
    }
}
