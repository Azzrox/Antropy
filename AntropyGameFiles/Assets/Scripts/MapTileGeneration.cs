using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class MapTileGeneration : MonoBehaviour
{

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    private GameObject[] decorations;

    float minTerrainHeight = float.MaxValue;
    float maxTerrainHeight = float.MinValue;

    public int resolution = 5;

    public enum TyleType { gras, water, stones, soil }
    public TyleType tyleType;
    //private bool3x3 isNeighbourWater = false;

    [Header("Decoration")]
    [SerializeField] private float ressourcesPerGrass;
    [SerializeField] private float stoneDensity;
    [SerializeField] private float size;
    [SerializeField] private GameObject defaultDecorationPrefab;
    [SerializeField] private GameObject staticDecorationPrefab;

    [Header("Colors")]

    [SerializeField] private Color[] fertilityColors;

    // Start is called before the first frame update
    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (tyleType != TyleType.water)
        {
            decorations = new GameObject[(resolution - 1) * (resolution - 1)];
        }

        CreateShape();
        UpdateMesh();
        UpdateCollider();

    }

    void UpdateCollider()
    {
        GetComponent<MeshCollider>().sharedMesh = mesh;
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

    void CreateShape()
    {
        float originX = transform.position.x;
        float originZ = transform.position.z;
        float noise = 0.3f;

        vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        colors = new Color[vertices.Length];

        int n = 0;

        GameObject decorationPrefab = null;

        if (!GameManager.Instance.showGrassMovement && staticDecorationPrefab != null)
        {
            decorationPrefab = staticDecorationPrefab;
        }
        else if (defaultDecorationPrefab != null)
        {
            decorationPrefab = defaultDecorationPrefab;
        }

        //generate the verctives of our mesh
        for (int i = 0, z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float y = Mathf.PerlinNoise((originX * resolution + x) * noise, (originZ * resolution + z) * noise) * 0.05f;

                //set the water bit lower
                if (tyleType == TyleType.water)
                {
                    int[] distanceToEdge = { x, z, resolution - x, resolution - z };
                    y -= Mathf.Sqrt((float)Mathf.Min(distanceToEdge) / resolution / 8f);
                }


                //filter vertices, that touch the border
                if (x != 0 && x != resolution && z != 0 && z != resolution)
                {
                    //add foliage
                    if (tyleType != TyleType.water)
                    {
                        //random rotation
                        Quaternion decorationRotation = Quaternion.identity;
                        if (tyleType == TyleType.gras || tyleType == TyleType.soil)
                        { decorationRotation = Quaternion.AngleAxis(UnityEngine.Random.value * 360f, Vector3.up); }
                        else if (tyleType == TyleType.stones)
                        { decorationRotation = UnityEngine.Random.rotation; }

                        if (decorationPrefab != null)
                        {
                            if (!(tyleType == TyleType.stones && stoneDensity < UnityEngine.Random.value))
                            {
                                decorations[n] = Instantiate(decorationPrefab, new Vector3((float)(x + UnityEngine.Random.value * 0.2f) / resolution + originX, y, (float)(z + UnityEngine.Random.value * 0.2f) / resolution + originZ), decorationRotation, gameObject.transform);
                                decorations[n].transform.localScale = new Vector3((1f + UnityEngine.Random.value * 0.2f) * size, (1f + UnityEngine.Random.value * 0.2f) * size, (1f + UnityEngine.Random.value * 0.2f) * size);

                            }
                            n++;
                        }
                    }
                }

                if (minTerrainHeight > y) { minTerrainHeight = y; }
                if (maxTerrainHeight < y) { maxTerrainHeight = y; }

                vertices[i] = new Vector3((float)x / resolution, y, (float)z / resolution);

                i++;
            }
        }

        //add water
        if (decorationPrefab != null)
        {
            if (tyleType == TyleType.water) { Instantiate(decorationPrefab, new Vector3(originX, transform.position.y - 0.08f, originZ), Quaternion.identity, gameObject.transform); }
        }
        //Gradient gradient = colorGradient;

        UpdateFertilityColor(0);

        triangles = new int[resolution * resolution * 6];


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

    public void RecalculateGrassDensity(float ressources)
    {

        int totalDecoration = decorations.Length;
        int neededDecoration = Mathf.FloorToInt(ressources / ressourcesPerGrass);
        neededDecoration = Mathf.Min(neededDecoration, totalDecoration);

        //if (decorations is not null)
        //{
            int activeDecoration = Array.FindAll(decorations, ob => ob.activeInHierarchy).Length;

            if (neededDecoration < activeDecoration)
            {
                GameObject[] enabledGrass = Array.FindAll(decorations, ob => ob.activeInHierarchy);
                float removalChance = ((float)activeDecoration - (float)neededDecoration) / (float)enabledGrass.Length;
                foreach (GameObject grassPrefab in enabledGrass)
                {
                    if (removalChance > UnityEngine.Random.value)
                    {
                        grassPrefab.SetActive(false);
                    }
                }

            }
            else if (neededDecoration > activeDecoration)
            {
                GameObject[] disabledGrass = Array.FindAll(decorations, ob => !ob.activeInHierarchy);
                float activateChance = ((float)neededDecoration - (float)activeDecoration) / (float)disabledGrass.Length;
                foreach (GameObject grassPrefab in disabledGrass)
                {
                    if (activateChance > UnityEngine.Random.value)
                    {
                        grassPrefab.SetActive(true);
                    }
                }
            }
        //}
    }

    public void RemoveDecorationForRoads()
    {
        float roadwidth = 0.12f;
        //foreach (GameObject grassPrefab in decorations)
        //{
        //    if (Mathf.Abs(grassPrefab.transform.localPosition.x - 0.5f) < roadwidth &&
        //        Mathf.Abs(grassPrefab.transform.localPosition.z - 0.5f) < roadwidth)
        //    {
        //        Destroy(grassPrefab);
        //    }
        //}

        var destroyDecorations = Array.FindAll(decorations, ob => (Mathf.Abs(ob.transform.localPosition.x - 0.5f) < roadwidth ||
                                                                Mathf.Abs(ob.transform.localPosition.z - 0.5f) < roadwidth));
        var keepDecorations = Array.FindAll(decorations, ob => (Mathf.Abs(ob.transform.localPosition.x - 0.5f) >= roadwidth &&
                                                                Mathf.Abs(ob.transform.localPosition.z - 0.5f) >= roadwidth));

        for (int i = 0; i < destroyDecorations.Length; i++) 
        {
            DestroyImmediate(destroyDecorations[i]);
        }

        decorations = keepDecorations;
    }

    private Color RandomizeColor(int fertilityLevel)
    {
        float hue, saturation, val;
        Color.RGBToHSV(fertilityColors[fertilityLevel], out hue, out saturation, out val);
        saturation += (UnityEngine.Random.value - 0.5f) * 0.3f;
        val += (UnityEngine.Random.value - 0.5f) * 0.3f;

        return Color.HSVToRGB(hue, saturation, val);
    }

    public void UpdateFertilityColor(int fertilityLevel)
    {
        if (fertilityLevel <= fertilityColors.Length && fertilityColors.Length > 0)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = RandomizeColor(fertilityLevel);
            }
        }
        UpdateMesh();
    }
}
