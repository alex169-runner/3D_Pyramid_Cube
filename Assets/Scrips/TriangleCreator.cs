using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleCreator : MonoBehaviour
{
    [ContextMenu("Create Triangle With Hard Edges")]
    void CreateWithHardEdges()
    {
        Vector3[] v = new Vector3[3]
        {
            new(-0.5f, 0, 0),
            new(0.5f, 0, 0),
            new(0, Mathf.Sqrt(3) / 2, 0)
        };

        Vector3[] hardVertices = new Vector3[3];
        int[] hardTriangles = new int[3];
        Vector3[] hardNormals = new Vector3[3];

        hardVertices[0] = v[0]; hardVertices[1] = v[2]; hardVertices[2] = v[1];
        hardTriangles[0] = 0; hardTriangles[1] = 1; hardTriangles[2] = 2;
        Vector3 normal1 = Vector3.Cross(v[2] - v[0], v[1] - v[0]).normalized;
        hardNormals[0] = hardNormals[1] = hardNormals[2] = normal1;

        Mesh mesh = new();
        mesh.name = "TriangleHardEdges";
        mesh.vertices = hardVertices;
        mesh.triangles = hardTriangles;
        mesh.normals = hardNormals;

        GetComponent<MeshFilter>().sharedMesh = mesh;

        CreateEnhancedMaterial();

#if UNITY_EDITOR
        SaveMeshAsAsset(mesh);
#endif
    }

    void SaveMeshAsAsset(Mesh mesh)
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder("Assets/Meshes")) {
            AssetDatabase.CreateFolder("Assets", "Meshes");
        }

        string path = "Assets/Meshes/TriangleMesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
#endif
    }

    void CreateEnhancedMaterial()
    {
#if UNITY_EDITOR
        Material enhancedMaterial = new Material(Shader.Find("Standard"));

        enhancedMaterial.color = Color.white;
        enhancedMaterial.SetFloat("_Metallic", 0.2f); // 少量金属感
        enhancedMaterial.SetFloat("_Glossiness", 0.1f); // 低光滑度
        enhancedMaterial.EnableKeyword("_EMISSION"); // 启用自发光

        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Materials")) {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string matPath = "Assets/Materials/EnhancedTriangle.mat";
        UnityEditor.AssetDatabase.CreateAsset(enhancedMaterial, matPath);
        UnityEditor.AssetDatabase.SaveAssets();

        GetComponent<MeshRenderer>().sharedMaterial = enhancedMaterial;
#endif
    }
}