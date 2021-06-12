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
    public Gradient mapColorGradient;

    private Transform props;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private void OnEnable() {
        Init();
    }

    void Init() {
        meshFilter = transform.GetComponent<MeshFilter>();
        meshRenderer = transform.GetComponent<MeshRenderer>();
        mapBounds = meshRenderer.bounds;
    }

    void Start() {
        GenerateMap();
        SpawnProps();
    }

    public void DrawMap(Texture2D texture) {
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void GenerateMap() {
        Texture2D terrainTexture = GenerateTerrainTexture();
        DrawMap(terrainTexture);
    }

    public void TransformUVtoWorld(float u, float v, ref Vector3 p) {
        p = transform.TransformPoint((u - 0.5f) * mapBounds.size.x, 0, (v - 0.5f) * mapBounds.size.z);
    }

    private float Ridgef(float h) {
        h = UnityEngine.Random.Range(-10f, 10f) - Mathf.Abs(h);
        return (h * h);
    }

    public float GetValueAtPositionWithRMFNoise(float x, float y) {
        float sum = 0.0f;
        float max = 0.0f;
        float prev = 1.0f;
        float amplitude = 0.5f;
        float maxo = Mathf.Max(Ridgef(0.0f), Ridgef(1.0f));
        float f = UnityEngine.Random.Range(0f, 10f);
        x = x + UnityEngine.Random.Range(0, 100);
        y = y + UnityEngine.Random.Range(0, 100);

        int octaves = 4;
        for (int i = 0; i < octaves; i++) {

            float n = Ridgef(Mathf.PerlinNoise(x * f, y * f) - 0.5f);
            float multiplier = amplitude * prev;
            sum += n * multiplier;
            max += maxo * multiplier;
            prev = n;
            f *= UnityEngine.Random.Range(1f, 5f);
            amplitude *= UnityEngine.Random.Range(0f, 2f);
        }
        return (2.0f * sum / max) - 1.0f;
    }

    public Texture2D GenerateTerrainTexture() {
        Texture2D mapTexture = new Texture2D(256, 265);
        mapTexture.name = "MapTexture";
        mapTexture.wrapMode = TextureWrapMode.Repeat;

        Color color = new Color();

        Vector3 p = new Vector3(0, 0, 0);
        float u, v;
        float invh = 1.0f / mapTexture.height;
        float invw = 1.0f / mapTexture.width;

        for (int z = 0; z < mapTexture.height; z++) {
            v = z * invh;

            for (int x = 0; x < mapTexture.width; x++) {
                u = x * invw;
                TransformUVtoWorld(u, v, ref p);

                color = mapColorGradient.Evaluate(GetValueAtPositionWithRMFNoise(u, v));
                mapTexture.SetPixel(x, z, color);
            }
        }

        mapTexture.Apply();
        return mapTexture;
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
