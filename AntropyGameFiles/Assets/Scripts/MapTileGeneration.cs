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
    public int resolution = 5;

    public enum TyleType {gras, water, stones, soil}
    public TyleType tyleType;

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

        CreateShape();
        UpdateMesh();

    }

    void CreateShape ()
    {
        float originX = transform.position.x;
        float originZ = transform.position.z;
        float noise = 0.3f;

        vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        colors = new Color[vertices.Length];

        float minTerrainHeight = float.MaxValue;
        float maxTerrainHeight = float.MinValue;

        //generate the verctives of our mesh
        for (int i = 0, z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float y = Mathf.PerlinNoise((originX*resolution + x) * noise, (originZ*resolution + z) * noise) * 0.15f;

                //set the water bit lower
                if (tyleType == TyleType.water && x != 0 && x != resolution && z != 0 && z != resolution) { y -= 0.2f; }

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
