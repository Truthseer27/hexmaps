using HexagonGeometry;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexagonRenderer : MonoBehaviour
{
    private Mesh hexMesh;
    private MeshFilter hexMeshFilter;
    private MeshRenderer hexMeshRenderer;
    private List<Face> hexFaces;

    public Material hexMaterial;

    public HexCellConfig hexConfig;

    // Awake is called when this script instance (if enabled) is being loaded
    private void Awake()
    {
        hexMeshFilter = GetComponent<MeshFilter>();
        hexMeshRenderer = GetComponent<MeshRenderer>();

        hexMesh = new Mesh();
        hexMesh.name = "Hexagon";

        hexMeshFilter.mesh = hexMesh;
        hexMeshRenderer.material = hexMaterial;

        hexConfig = new HexCellConfig(outerRadius: 1f, depthAxis: HexCellDepthAxis.Y);
        Debug.Log("In Awake() where outerRadius is " + hexConfig.OuterRadius);
    }

    // Called when the object becomes enabled and active (e.g., in Play mode)
    private void OnEnable()
    {
        Debug.Log("OnEnable() is called");
        DrawMesh();
    }

    // Editor-only function that is called when the script is loaded or a value changes in the Inspector
    public void OnValidate()
    {
        Debug.Log("In OnValidate()");
        if (Application.isPlaying)
        {
            DrawMesh();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        hexFaces = new List<Face>();
        Debug.Log("In DrawFaces() where outer radius is " + hexConfig.OuterRadius);

        // Top faces
        for (int point = 0; point < HexagonMetrics.NUM_VERTICES; point++)
        {
            hexFaces.Add(CreateFace
                (hexConfig.OuterRadius,
                 hexConfig.InnerRadius,
                 point,
                 hexConfig.HeightOuter / 2f,
                 hexConfig.HeightInner / 2f,
                 hexConfig.Orientation,
                 hexConfig.DepthAxis
                ));
        }

        // Bottom faces
        //for (int point = 0; point < HexagonMetrics.NUM_VERTICES; point++)
        //{
        //    hexFaces.Add(CreateFace
        //        (hexConfig.OuterRadius,
        //         hexConfig.InnerRadius,
        //         point,
        //         -hexConfig.HeightOuter / 2f,
        //         -hexConfig.HeightInner / 2f,
        //         hexConfig.Orientation,
        //         hexConfig.DepthAxis,
        //         true
        //        ));
        //}

        // Outer faces
        //for (int point = 0; point < HexagonMetrics.NUM_VERTICES; point++)
        //{
        //    hexFaces.Add(CreateFace
        //        (hexConfig.OuterRadius,
        //         hexConfig.InnerRadius,
        //         point,
        //         hexConfig.HeightOuter / 2f,
        //         -hexConfig.HeightInner / 2f,
        //         hexConfig.Orientation,
        //         hexConfig.DepthAxis,
        //         true
        //        ));
        //}

        // Bottom faces
        //for (int point = 0; point < HexagonMetrics.NUM_VERTICES; point++)
        //{
        //    hexFaces.Add(CreateFace
        //        (hexConfig.OuterRadius,
        //         hexConfig.InnerRadius,
        //         point,
        //         hexConfig.HeightOuter / 2f,
        //         -hexConfig.HeightInner / 2f,
        //         hexConfig.Orientation,
        //         hexConfig.DepthAxis
        //        ));
        //}
    }

    private Face CreateFace
    (
        float outerRadius,
        float innerRadius,
        int currentPoint,
        float heightInner = 0f,
        float heightOuter = 0f,
        HexCellOrientation orientation = HexCellOrientation.Pointy,
        HexCellDepthAxis depthAxis = HexCellDepthAxis.Z,
        bool reverse = false
    )
    {
        Debug.Log("In CreateFace() where outer radius is " + innerRadius);
        // ensures properly connecting faces
        int filteredPoint = (currentPoint < 5) ? currentPoint + 1 : 0;
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(HexagonMetrics.GetVertex(innerRadius, currentPoint, heightInner, orientation, depthAxis));
        vertices.Add(HexagonMetrics.GetVertex(innerRadius, filteredPoint, heightInner, orientation, depthAxis));
        vertices.Add(HexagonMetrics.GetVertex(outerRadius, filteredPoint, heightOuter, orientation, depthAxis));
        vertices.Add(HexagonMetrics.GetVertex(outerRadius, currentPoint, heightOuter, orientation, depthAxis));

        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
        if (reverse) vertices.Reverse();

        return new Face(vertices, triangles, uvs);
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < hexFaces.Count; i++)
        {
            // Add the vertices
            vertices.AddRange(hexFaces[i].vertices);
            uvs.AddRange(hexFaces[i].uvs);

            // Offset the triangles
            int offset = (4 * i);
            foreach (int triangle in hexFaces[i].triangles)
            {
                triangles.Add(triangle + offset);
            }
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.uv = uvs.ToArray();
        hexMesh.RecalculateNormals();
    }
}

// define a Face for the mesh
public struct Face
{
    public List<Vector3> vertices { get; private set; }
    // topology
    public List<int> triangles { get; private set; }
    // texture coordinates
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

// dimension configuration for hexagon cells
[Serializable]
public struct HexCellConfig
{
    [field: SerializeField]
    public float OuterRadius { get; set; }
    [field: SerializeField]
    public float InnerRadius { get; set; }
    [field: SerializeField]
    public float HeightOuter { get; set; }
    [field: SerializeField]
    public float HeightInner { get; set; }
    [field: SerializeField]
    public HexCellOrientation Orientation { get; set; }
    [field: SerializeField]
    public HexCellDepthAxis DepthAxis { get; set; }

    public HexCellConfig
    (
        float outerRadius,
        float innerRadius = 0f,
        float heightOuter = 0f,
        float heightInner = 0f,
        HexCellOrientation orientation = HexCellOrientation.Pointy,
        HexCellDepthAxis depthAxis = HexCellDepthAxis.Z
    )
    {
        OuterRadius = outerRadius;
        InnerRadius = innerRadius;
        HeightOuter = heightOuter;
        HeightInner = heightInner;
        Orientation = orientation;
        DepthAxis = depthAxis;
    }
}

// Orientation of each hexagon in the grid
[Serializable]
public enum HexCellOrientation
{
    Pointy,
    Flat
}

// The depth axis for the hexagon.
// If Z, then for a pointy-topped orientation, the top point will be in the direction of the Y-axis
// If Y, then for a pointy-topped orientation, the top point will be in the direction of the Z-axis
// For a flat-topped orientation, the right point will be in the direction of the X-axis (regardless of depth axis)
[Serializable]
public enum HexCellDepthAxis
{
    Y,
    Z
}

