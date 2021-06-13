using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MapComponent : MonoBehaviour
{
    [Serializable]
    public struct Prop
    {
        public string name;
        public GameObject prefab;
        public float minMapHeight;
        public float maxMapHeight;
        public int number;
    }
    [SerializeField] private List<Prop> propSettings;
    [SerializeField] private Bounds mapBounds;
    [SerializeField] private bool autoUpdate;

	private NavMeshSurface _surface;
    [Serializable]
    public class MapSettings
    {
        public int mapWidth;
        public int mapHeight;
        public int tesselation;
        public int size;
        public Gradient mapColorGradient;
        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public float noiseThreshhold;
        public int offsetSeed;
        [Range(1, 10)] public int octaves;
        [Range(-100.0f, 100.0f)] public float fixedOffsetX;
        [Range(-100.0f, 100.0f)] public float fixedOffsetY;
        [Min(0.0001f)] public int scale;
        [Range(0f, 1f)] public float persistance;
        [Min(1f)] public float lacunarity;
        [Range(0.0f, 10.0f)] public float frequency;
        [Range(-10.0f, 10.0f)] public float ridgeOffset;
        [Range(0.0f, 2.0f)] public float gain;
    }
    [SerializeField, OnValueChanged("OnSettingsChanged")] private MapSettings mapSettings;

    private Vector2[] octaveOffsets;

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
        octaveOffsets = GetOctaveOffsets(mapSettings.octaves, mapSettings.offsetSeed, mapSettings.fixedOffsetX, mapSettings.fixedOffsetY);
    }

    void Start() {
        GenerateMap();
        SpawnProps();
        _surface = GetComponent<NavMeshSurface>();
		_surface.BuildNavMesh();
    }

    public void OnSettingsChanged() {
        if (autoUpdate) GenerateMap();
    }

    public void DrawMap(Texture2D texture, Mesh mesh) {
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshFilter.mesh = mesh;
    }

    public void GenerateMap() {
        Init();
        Mesh mesh = GenerateTerrainMesh();
        Texture2D texture = GenerateTerrainTexture();
        // Tesselate();
        DrawMap(texture, mesh);
    }

    public void TransformUVtoWorld(float u, float v, ref Vector3 p) {
        p = transform.TransformPoint((u - 0.5f) * mapBounds.size.x, 0, (v - 0.5f) * mapBounds.size.z);
    }

    public static Vector2[] GetOctaveOffsets(int octaves, int offsetSeed, float fixedOffsetX, float fixedOffsetY) {
        System.Random prng = new System.Random(offsetSeed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + fixedOffsetX;
            float offsetY = prng.Next(-100000, 100000) + fixedOffsetY;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    public float Perlin(float x, float y) {
        float amplitude = 1;
        float frequency = 1;
        float noiseValue = 0;
        x = x + mapSettings.fixedOffsetX;
        y = y + mapSettings.fixedOffsetY;

        for (int i = 0; i < mapSettings.octaves; i++) {
            // higher frequency -> further apart sample points -> height values change faster
            float sampleX = (x - (mapSettings.mapWidth / 2)) / mapSettings.scale * frequency + octaveOffsets[i].x;
            float sampleY = (y - (mapSettings.mapHeight / 2)) / mapSettings.scale * frequency + octaveOffsets[i].y;

            // adjusts perlin noise range to -1 to 1 so that the noise height can decrease
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            // increase noise height by perlin value of each octave
            noiseValue += perlinValue * amplitude;

            // decreases every octave
            amplitude *= mapSettings.persistance;
            // increases every octave
            frequency *= mapSettings.lacunarity;
        }
        return noiseValue;
    }

    private float Ridgef(float h) {
        h = mapSettings.ridgeOffset - Mathf.Abs(h);
        return (h * h);
    }

    public float RMF(float x, float y) {
        float sum = 0.0f;
        float max = 0.0f;
        float prev = 1.0f;
        float amplitude = 0.5f;
        float maxo = Mathf.Max(Ridgef(0.0f), Ridgef(1.0f));
        float f = mapSettings.frequency;
        x = x + mapSettings.fixedOffsetX;
        y = y + mapSettings.fixedOffsetY;

        for (int i = 0; i < mapSettings.octaves; i++) {

            float n = Ridgef(Mathf.PerlinNoise(x * f, y * f) - 0.5f);
            float multiplier = amplitude * prev;
            sum += n * multiplier;
            max += maxo * multiplier;
            prev = n;
            f *= mapSettings.lacunarity;
            amplitude *= mapSettings.gain;
        }
        return (2.0f * sum / max) - 1.0f;
    }

    public Texture2D GenerateTerrainTexture() {
        Texture2D mapTexture = new Texture2D(mapSettings.size, mapSettings.size);
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

                color = mapSettings.mapColorGradient.Evaluate(Perlin(p.x, p.z));
                mapTexture.SetPixel(x, z, color);
            }
        }

        mapTexture.Apply();
        return mapTexture;
    }

    public Vector3 GetMeshPosition(Vector3 targetPosition) {
        Vector3 closestPoint = mapBounds.ClosestPoint(targetPosition);
        Vector3 meshPosition = new Vector3(closestPoint.x, GetMeshHeight(closestPoint), closestPoint.z);
        return meshPosition;
    }

    public float GetMeshHeight(Vector3 targetPosition) {
        float height = mapSettings.meshHeightMultiplier * mapSettings.meshHeightCurve.Evaluate(Perlin(targetPosition.x, targetPosition.z));
        return height;
    }

    public Vector3 GetRandomPosition() {
        float x = UnityEngine.Random.Range(mapBounds.min.x, mapBounds.max.x);
        float z = UnityEngine.Random.Range(mapBounds.min.z, mapBounds.max.z);
        return new Vector3(x, 0, z);
    }

    public Vector3 GetPositionWithMinHeight(float minHeight, float maxHeight) {
        Vector3 randomPos = GetMeshPosition(GetRandomPosition());
        if (randomPos.y >= minHeight && randomPos.y <= maxHeight) {
            return randomPos;
        } else {
            return GetPositionWithMinHeight(minHeight, maxHeight);
        }
    }

    private Mesh GenerateTerrainMesh() {
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        for (var i = 0; i < vertices.Length; i++)
            vertices[i].y = GetMeshHeight(transform.TransformPoint(vertices[i]));

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private void Tesselate() {
        Vector3 p = new Vector3();
        Vector2 uv = new Vector2();

        Mesh terrainMesh = meshFilter.sharedMesh;
        //mesh.Clear(false);
        Vector3[] vertices = new Vector3[(mapSettings.tesselation + 1) * (mapSettings.tesselation + 1)];
        Vector2[] uvs = new Vector2[(mapSettings.tesselation + 1) * (mapSettings.tesselation + 1)];

        int i = 0;
        float scale = mapSettings.size / mapSettings.tesselation;
        float offW = -mapSettings.size / 2f;
        float offD = -mapSettings.size / 2f;
        for (int d = 0; d <= mapSettings.tesselation; d++) {
            uv.y = d / (float)mapSettings.tesselation;

            for (int w = 0; w <= mapSettings.tesselation; w++) {
                float x = scale * w + offW;
                float z = scale * d + offD;
                float y = 0;

                uv.x = w / (float)mapSettings.tesselation;

                uvs[i] = uv;

                p.Set(x, y, z);
                vertices[i] = p; // new Vector3(w, 0, d) - new Vector3(width / 2f, 0, depth / 2f);

                i++;
            }
        }

        int[] triangles = new int[mapSettings.tesselation * mapSettings.tesselation * 2 * 3]; // 2 - polygon per quad, 3 - corners per polygon

        for (int d = 0; d < mapSettings.tesselation; d++) {
            for (int w = 0; w < mapSettings.tesselation; w++) {
                // quad triangles index.
                int ti = (d * (mapSettings.tesselation) + w) * 6; // 6 - polygons per quad * corners per polygon

                // First tringle
                triangles[ti] = (d * (mapSettings.tesselation + 1)) + w;
                triangles[ti + 1] = ((d + 1) * (mapSettings.tesselation + 1)) + w;
                triangles[ti + 2] = ((d + 1) * (mapSettings.tesselation + 1)) + w + 1;

                // Second triangle
                triangles[ti + 3] = (d * (mapSettings.tesselation + 1)) + w;
                triangles[ti + 4] = ((d + 1) * (mapSettings.tesselation + 1)) + w + 1;
                triangles[ti + 5] = (d * (mapSettings.tesselation + 1)) + w + 1;
            }
        }

        // Assigning vertices, triangles and UV to the mesh.
        //mesh.vertices = vertices;
        terrainMesh.triangles = triangles;


        for (i = 0; i < vertices.Length; i++) {
            float height = GetMeshHeight(transform.TransformPoint(vertices[i]));
            vertices[i].y = height;
        }

        terrainMesh.vertices = vertices;
        terrainMesh.uv = uvs;
        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateBounds();

        Debug.Log("tessi");
    }

    private void SpawnProps() {
        props = Instantiate(new GameObject("props")).transform;

        foreach (Prop prop in propSettings) {
            for (int i = 0; i < prop.number; i++) {
                Vector3 pos = GetPositionWithMinHeight(prop.minMapHeight, prop.maxMapHeight);
                Quaternion rot = new Quaternion();
                rot.eulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);

                Transform v = Instantiate(prop.prefab, pos, rot).transform;
                v.transform.parent = props.transform;
                v.name = prop.name + "." + i;
            }
        }
    }
}
