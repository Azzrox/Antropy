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
    [SerializeField] private float density;
    [SerializeField] private float size;
    [SerializeField] private GameObject decorationPrefab;

    [Header("Colors")]

    [SerializeField] private Gradient colorGradient;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        density = Mathf.Clamp(density, 0f, 1f);

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
                    if (tyleType != TyleType.water && density >= Random.value)
                    {
                        //random rotation
                        Quaternion decorationRotation = Quaternion.identity;
                        if (tyleType == TyleType.gras || tyleType == TyleType.soil)
                        { decorationRotation = Quaternion.AngleAxis(Random.value * 360f, Vector3.up); }
                        else if (tyleType == TyleType.stones)
                        { decorationRotation = Random.rotation; }


                        foliage = Instantiate(decorationPrefab, new Vector3((float)(x + Random.value * 0.2f) / resolution + originX, y, (float)(z + Random.value * 0.2f) / resolution + originZ), decorationRotation, gameObject.transform);
                        foliage.transform.localScale = new Vector3((1f + Random.value * 0.2f) * size, (1f + Random.value * 0.2f) * size, (1f + Random.value * 0.2f) * size);

                    }
                }
                if (minTerrainHeight > y) { minTerrainHeight = y; }
                if (maxTerrainHeight < y) { maxTerrainHeight = y; }

                vertices[i] = new Vector3((float) x /resolution, y, (float)z /resolution);

                i++;
            }
        }

        //add water
        if (tyleType == TyleType.water) { Instantiate(decorationPrefab, new Vector3(originX, transform.position.y - 0.08f, originZ), Quaternion.identity, gameObject.transform); }

        Gradient gradient = colorGradient;

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
