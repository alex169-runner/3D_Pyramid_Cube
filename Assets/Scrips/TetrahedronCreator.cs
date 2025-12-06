using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TetrahedronCreator : MonoBehaviour
{
    float r3 = Mathf.Sqrt(3);
    float r6 = Mathf.Sqrt(6);

    [ContextMenu("Create Tetrahedron With Hard Edges")]
    void CreateWithHardEdges()
    {
        Vector3[] v = new Vector3[4]
        {
            new(0, r6 / 4, 0),
            new(0, -r6 / 12, -r3 / 3),
            new(0.5f, -r6 / 12, r3 / 6),
            new(-0.5f, -r6 / 12, r3 / 6)
        };

        // 为每个三角形面创建独立的顶点，实现硬边
        Vector3[] hardVertices = new Vector3[12]; // 4个面 × 3个顶点
        int[] hardTriangles = new int[12];
        Vector3[] hardNormals = new Vector3[12];

        // 面1: 顶点 0,2,1
        hardVertices[0] = v[0]; hardVertices[1] = v[2]; hardVertices[2] = v[1];
        hardTriangles[0] = 0; hardTriangles[1] = 1; hardTriangles[2] = 2;
        Vector3 normal1 = Vector3.Cross(v[2] - v[0], v[1] - v[0]).normalized;
        hardNormals[0] = hardNormals[1] = hardNormals[2] = normal1;

        // 面2: 顶点 0,3,2
        hardVertices[3] = v[0]; hardVertices[4] = v[3]; hardVertices[5] = v[2];
        hardTriangles[3] = 3; hardTriangles[4] = 4; hardTriangles[5] = 5;
        Vector3 normal2 = Vector3.Cross(v[3] - v[0], v[2] - v[0]).normalized;
        hardNormals[3] = hardNormals[4] = hardNormals[5] = normal2;

        // 面3: 顶点 0,1,3
        hardVertices[6] = v[0]; hardVertices[7] = v[1]; hardVertices[8] = v[3];
        hardTriangles[6] = 6; hardTriangles[7] = 7; hardTriangles[8] = 8;
        Vector3 normal3 = Vector3.Cross(v[1] - v[0], v[3] - v[0]).normalized;
        hardNormals[6] = hardNormals[7] = hardNormals[8] = normal3;

        // 面4: 顶点 1,2,3
        hardVertices[9] = v[1]; hardVertices[10] = v[2]; hardVertices[11] = v[3];
        hardTriangles[9] = 9; hardTriangles[10] = 10; hardTriangles[11] = 11;
        Vector3 normal4 = Vector3.Cross(v[2] - v[1], v[3] - v[1]).normalized;
        hardNormals[9] = hardNormals[10] = hardNormals[11] = normal4;

        Mesh mesh = new();
        mesh.name = "TetrahedronHardEdges";
        mesh.vertices = hardVertices;
        mesh.triangles = hardTriangles;
        mesh.normals = hardNormals; // 使用计算好的硬边法线

        GetComponent<MeshFilter>().sharedMesh = mesh;

        // 创建增强材质
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

        string path = "Assets/Meshes/TetrahedronMesh.asset";
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

        string matPath = "Assets/Materials/EnhancedTetrahedron.mat";
        UnityEditor.AssetDatabase.CreateAsset(enhancedMaterial, matPath);
        UnityEditor.AssetDatabase.SaveAssets();

        GetComponent<MeshRenderer>().sharedMaterial = enhancedMaterial;
#endif
    }
}