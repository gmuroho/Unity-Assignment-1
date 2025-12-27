using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GrassMeshGenerator : MonoBehaviour
{
    [Header("单根草设置")]
    public float width = 0.05f;      // 草叶宽度
    public float height = 1.0f;      // 草叶高度
    public Color grassColor = new Color(0.2f, 0.8f, 0.2f);

    [Header("草丛生成设置")]
    public int grassCount = 12;      // 一簇里面有多少根草叶
    public float radius = 0.3f;      // 散开半径

    [ContextMenu("一键生成/更新草簇模型")]
    public void GenerateGrassCluster()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += SafeGenerate;
            return;
        }
#endif
        SafeGenerate();
    }

    private void SafeGenerate()
    {
        if (this == null) return;

        // 1. 寻找旧模型容器
        Transform modelContainer = transform.Find("Visual_Model");
        if (modelContainer != null)
        {
            DestroyImmediate(modelContainer.gameObject);
        }

        // 2. 创建新容器
        GameObject containerObj = new GameObject("Visual_Model");
        containerObj.transform.SetParent(this.transform);
        containerObj.transform.localPosition = Vector3.zero;
        containerObj.transform.localRotation = Quaternion.identity;
        containerObj.transform.localScale = Vector3.one;

        // 3. 准备材质 (解决粉紫色问题)
        // 尝试自动识别当前管线需要的着色器
        Shader grassShader = Shader.Find("Universal Render Pipeline/Lit"); // 尝试 URP
        if (grassShader == null) grassShader = Shader.Find("Standard");    // 尝试内置管线

        Material sharedMat = new Material(grassShader);
        sharedMat.color = grassColor;
        // 如果是 URP，需要设置 BaseColor
        if (sharedMat.HasProperty("_BaseColor")) sharedMat.SetColor("_BaseColor", grassColor);

        // 4. 生成草叶
        for (int i = 0; i < grassCount; i++)
        {
            GameObject singleBlade = new GameObject("Blade_" + i);
            singleBlade.transform.SetParent(containerObj.transform);

            Vector2 randomCircle = Random.insideUnitCircle * radius;
            singleBlade.transform.localPosition = new Vector3(randomCircle.x, 0, randomCircle.y);
            singleBlade.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360f), Random.Range(-5f, 15f));

            MeshFilter mf = singleBlade.AddComponent<MeshFilter>();
            MeshRenderer mr = singleBlade.AddComponent<MeshRenderer>();

            mf.sharedMesh = CreateBladeMesh();
            mr.sharedMaterial = sharedMat;

            // 设置层级
            singleBlade.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        Debug.Log($"<color=green>草簇生成完毕！</color> 材质颜色已更新。");
    }

    private Mesh CreateBladeMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-width/2, 0, -width/2),
            new Vector3(width/2, 0, -width/2),
            new Vector3(width/2, 0, width/2),
            new Vector3(-width/2, 0, width/2),
            new Vector3(0, height, 0)
        };

        int[] triangles = new int[]
        {
            0, 2, 1, 0, 3, 2,
            0, 4, 3, 3, 4, 2, 2, 4, 1, 1, 4, 0
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}