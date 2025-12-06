using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OctahedronCreator : MonoBehaviour
{
    [ContextMenu("Create Octahedron With Hard Edges")]
    void CreateWithHardEdges()
    {
        float scale = Mathf.Sqrt(2) / 2; // √2/2

        Vector3[] v = new Vector3[6]
        {
            new Vector3(0, 0, scale),   // 顶部顶点 (前)
            new Vector3(0, 0, -scale),  // 底部顶点 (后)
            new Vector3(0, scale, 0),   // 上顶点
            new Vector3(0, -scale, 0),  // 下顶点
            new Vector3(scale, 0, 0),   // 右顶点
            new Vector3(-scale, 0, 0)   // 左顶点
        };

        // 应用旋转欧拉角(35.264389682755, 0, 45)
        Quaternion rotation = Quaternion.Euler(35.26438968f, 0, 45f);
        for (int i = 0; i < v.Length; i++) {
            v[i] = rotation * v[i];
        }

        // 为每个三角形面创建独立的顶点，实现硬边
        // 正八面体有8个面，每个面3个顶点，共24个顶点
        Vector3[] hardVertices = new Vector3[24];
        int[] hardTriangles = new int[24];
        Vector3[] hardNormals = new Vector3[24];

        // 面1: 上三角 - 前右上 (0,4,2)
        hardVertices[0] = v[0]; hardVertices[1] = v[4]; hardVertices[2] = v[2];
        hardTriangles[0] = 0; hardTriangles[1] = 1; hardTriangles[2] = 2;
        Vector3 normal1 = Vector3.Cross(v[4] - v[0], v[2] - v[0]).normalized;
        hardNormals[0] = hardNormals[1] = hardNormals[2] = normal1;

        // 面2: 上三角 - 前左上 (0,2,5)
        hardVertices[3] = v[0]; hardVertices[4] = v[2]; hardVertices[5] = v[5];
        hardTriangles[3] = 3; hardTriangles[4] = 4; hardTriangles[5] = 5;
        Vector3 normal2 = Vector3.Cross(v[2] - v[0], v[5] - v[0]).normalized;
        hardNormals[3] = hardNormals[4] = hardNormals[5] = normal2;

        // 面3: 上三角 - 前右下 (0,3,4)
        hardVertices[6] = v[0]; hardVertices[7] = v[3]; hardVertices[8] = v[4];
        hardTriangles[6] = 6; hardTriangles[7] = 7; hardTriangles[8] = 8;
        Vector3 normal3 = Vector3.Cross(v[3] - v[0], v[4] - v[0]).normalized;
        hardNormals[6] = hardNormals[7] = hardNormals[8] = normal3;

        // 面4: 上三角 - 前左下 (0,5,3)
        hardVertices[9] = v[0]; hardVertices[10] = v[5]; hardVertices[11] = v[3];
        hardTriangles[9] = 9; hardTriangles[10] = 10; hardTriangles[11] = 11;
        Vector3 normal4 = Vector3.Cross(v[5] - v[0], v[3] - v[0]).normalized;
        hardNormals[9] = hardNormals[10] = hardNormals[11] = normal4;

        // 面5: 下三角 - 后右上 (1,2,4)
        hardVertices[12] = v[1]; hardVertices[13] = v[2]; hardVertices[14] = v[4];
        hardTriangles[12] = 12; hardTriangles[13] = 13; hardTriangles[14] = 14;
        Vector3 normal5 = Vector3.Cross(v[2] - v[1], v[4] - v[1]).normalized;
        hardNormals[12] = hardNormals[13] = hardNormals[14] = normal5;

        // 面6: 下三角 - 后左上 (1,5,2)
        hardVertices[15] = v[1]; hardVertices[16] = v[5]; hardVertices[17] = v[2];
        hardTriangles[15] = 15; hardTriangles[16] = 16; hardTriangles[17] = 17;
        Vector3 normal6 = Vector3.Cross(v[5] - v[1], v[2] - v[1]).normalized;
        hardNormals[15] = hardNormals[16] = hardNormals[17] = normal6;

        // 面7: 下三角 - 后右下 (1,4,3)
        hardVertices[18] = v[1]; hardVertices[19] = v[4]; hardVertices[20] = v[3];
        hardTriangles[18] = 18; hardTriangles[19] = 19; hardTriangles[20] = 20;
        Vector3 normal7 = Vector3.Cross(v[4] - v[1], v[3] - v[1]).normalized;
        hardNormals[18] = hardNormals[19] = hardNormals[20] = normal7;

        // 面8: 下三角 - 后左下 (1,3,5)
        hardVertices[21] = v[1]; hardVertices[22] = v[3]; hardVertices[23] = v[5];
        hardTriangles[21] = 21; hardTriangles[22] = 22; hardTriangles[23] = 23;
        Vector3 normal8 = Vector3.Cross(v[3] - v[1], v[5] - v[1]).normalized;
        hardNormals[21] = hardNormals[22] = hardNormals[23] = normal8;

        Mesh mesh = new();
        mesh.name = "OctahedronHardEdges";
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

        string path = "Assets/Meshes/OctahedronMesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Octahedron mesh saved to: " + path);
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

        if (!AssetDatabase.IsValidFolder("Assets/Materials")) {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string matPath = "Assets/Materials/EnhancedOctahedron.mat";
        AssetDatabase.CreateAsset(enhancedMaterial, matPath);
        AssetDatabase.SaveAssets();

        GetComponent<MeshRenderer>().sharedMaterial = enhancedMaterial;
        Debug.Log("Octahedron material created: " + matPath);
#endif
    }
}