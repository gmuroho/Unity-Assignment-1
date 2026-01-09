using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GrassMeshGenerator : MonoBehaviour
{
    [Header("单根草")]
    public float width = 0.05f;      
    public float height = 1.0f;      
    public Color grassColor = new Color(0.2f, 0.8f, 0.2f);

    [Header("草丛生")]
    public int grassCount = 12;      
    public float radius = 0.3f;      

    [ContextMenu("更新草簇模型")]
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

        
        Transform modelContainer = transform.Find("Visual_Model");
        if (modelContainer != null)
        {
            DestroyImmediate(modelContainer.gameObject);
        }

        
        GameObject containerObj = new GameObject("Visual_Model");
        containerObj.transform.SetParent(this.transform);
        containerObj.transform.localPosition = Vector3.zero;
        containerObj.transform.localRotation = Quaternion.identity;
        containerObj.transform.localScale = Vector3.one;

        
        
        Shader grassShader = Shader.Find("Universal Render Pipeline/Lit"); 
        if (grassShader == null) grassShader = Shader.Find("Standard");    

        Material sharedMat = new Material(grassShader);
        sharedMat.color = grassColor;
        
        if (sharedMat.HasProperty("_BaseColor")) sharedMat.SetColor("_BaseColor", grassColor);

        
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

            
            singleBlade.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        Debug.Log($"<color=green>草簇生成完l</color> 材质颜色已更新。");
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