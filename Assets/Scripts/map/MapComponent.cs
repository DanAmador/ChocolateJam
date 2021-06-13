using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponent : MonoBehaviour
{
    [Serializable]
    public struct Prop
    {
        public string name;
        public GameObject prefab;
        public int number;
    }
    public List<Prop> propSettings;
    public Bounds mapBounds;

    private Transform props;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void Awake() {
        Init();
    }

    void Init() {
        meshFilter = transform.GetComponent<MeshFilter>();
        meshRenderer = transform.GetComponent<MeshRenderer>();
        mapBounds = meshRenderer.bounds;
    }

    void Start() {
        SpawnProps();
    }

    private void SpawnProps() {
        props = Instantiate(new GameObject("props")).transform;

        foreach (Prop prop in propSettings) {
            for (int i = 0; i < prop.number; i++) {
                Vector3 pos = GetRandomPosition();
                Quaternion rot = new Quaternion();
                rot.eulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);

                Transform v = Instantiate(prop.prefab, pos, rot).transform;
                v.transform.parent = props.transform;
                v.name = prop.name + "." + i;
            }
        }
    }

    public Vector3 GetRandomPosition() {
        float x = UnityEngine.Random.Range(mapBounds.min.x, mapBounds.max.x);
        float z = UnityEngine.Random.Range(mapBounds.min.z, mapBounds.max.z);
        return new Vector3(x, 0, z);
    }
}
