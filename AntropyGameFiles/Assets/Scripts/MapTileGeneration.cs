using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MapTileGeneration : MonoBehaviour
{

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    float minTerrainHeight = float.MaxValue;
    float maxTerrainHeight = float.MinValue;

    public int resolution = 5;

    public enum TyleType {gras, water, stones, soil}
    public TyleType tyleType;

    [Header("Decoration")]
    [SerializeField] private float grassDensity;
    [SerializeField] private float stoneDensity;
    [SerializeField] private float soilDensity;
    [SerializeField] private float size;
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject stonePrefab;

    [Header("Colors")]

    [SerializeField] private Gradient grasGradient;
    [SerializeField] private Gradient waterGradient;
    [SerializeField] private Gradient stonesGradient;
    [SerializeField] private Gradient soilGradient;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        grassDensity = Mathf.Clamp(grassDensity, 0f, 1f);
        stoneDensity = Mathf.Clamp(stoneDensity, 0f, 1f);

        CreateShape();
        UpdateMesh();

    }

    //void Update()
    //{
    //    Gradient gradient = new Gradient { };
    //    //get the correct color per type
    //    switch (tyleType)
    //    {
    //        case TyleType.gras:
    //            gradient = grasGradient; break;
    //        case TyleType.water:
    //            gradient = waterGradient; break;
    //        case TyleType.stones:
    //            gradient = stonesGradient; break;
    //        case TyleType.soil:
    //            gradient = soilGradient; break;
    //    }

    //    //color the vertices
    //    for (int i = 0, z = 0; z <= resolution; z++)
    //    {
    //        for (int x = 0; x <= resolution; x++)
    //        {
    //            float grade = vertices[i].y / (maxTerrainHeight - minTerrainHeight);
    //            colors[i] = gradient.Evaluate(grade);

    //            i++;
    //        }
    //    }

    //    mesh.colors = colors;
    //}

    void CreateShape ()
    {
        float originX = transform.position.x;
        float originZ = transform.position.z;
        float noise = 0.3f;

        vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        colors = new Color[vertices.Length];

        GameObject foliage;

        //generate the verctives of our mesh
        for (int i = 0, z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float y = Mathf.PerlinNoise((originX*resolution + x) * noise, (originZ*resolution + z) * noise) * 0.05f;

                //filter vertices, that touch the border
                if (x != 0 && x != resolution && z != 0 && z != resolution)
                {
                    //set the water bit lower
                    if (tyleType == TyleType.water) { y -= 0.2f; }

                    //add foliage
                    if ((tyleType == TyleType.gras && grassDensity >= Random.value)
                        || (tyleType == TyleType.soil && soilDensity >= Random.value)) 
                    {
                        foliage = Instantiate(grassPrefab, new Vector3((float)(x + Random.value*0.2f) / resolution + originX, y, (float)(z + Random.value*0.2f) / resolution + originZ), Quaternion.AngleAxis(Random.value*360f,Vector3.up), gameObject.transform);
                        foliage.transform.localScale = new Vector3((1f + Random.value*0.2f)*size, (1f + Random.value * 0.2f) * size, (1f + Random.value * 0.2f)*size);
                    }
                    //add rocks and stones
                    else if (tyleType == TyleType.stones && stoneDensity >= Random.value)
                    {
                        foliage = Instantiate(stonePrefab, new Vector3((float)(x + Random.value * 0.2f) / resolution + originX, y + 0.025f, (float)(z + Random.value * 0.2f) / resolution + originZ), Random.rotation, gameObject.transform);
                        foliage.transform.localScale = new Vector3((1f + Random.value * 0.2f) * size, (1f + Random.value * 0.2f) * size, (1f + Random.value * 0.2f) * size);
                    }

                }

                if (minTerrainHeight > y) { minTerrainHeight = y; }
                if (maxTerrainHeight < y) { maxTerrainHeight = y; }

                vertices[i] = new Vector3((float) x /resolution, y, (float)z /resolution);

                i++;
            }
        }

        Gradient gradient = new Gradient{};
        //get the correct color per type
        switch (tyleType)
        {
            case TyleType.gras:
                gradient = grasGradient; break;
            case TyleType.water:
                gradient = waterGradient; break;
            case TyleType.stones:
                gradient = stonesGradient; break;
            case TyleType.soil:
                gradient = soilGradient; break;
        }

        //color the vertices
        for (int i = 0, z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float grade = vertices[i].y / (maxTerrainHeight - minTerrainHeight);
                colors[i] = gradient.Evaluate(grade);

                i++;
            }
        }


        triangles = new int[resolution*resolution*6];


        int vert = 0;
        int tris = 0;

        //connect the vertices with faces
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + resolution + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + resolution + 1;
                triangles[tris + 5] = vert + resolution + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();

    }

}
