using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GrassMeshGenerator : MonoBehaviour
{
    public float kuan = 0.05f;
    public float gao = 1.0f;
    public Color cao_color = new Color(0.2f, 0.8f, 0.2f);

    public int count = 12;
    public float r = 0.3f;

    [ContextMenu("Update Grass")]
    public void GenerateGrassCluster()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += DoWork;
            return;
        }
#endif
        DoWork();
    }

    void DoWork()
    {
        if (this == null) return;

        Transform t = transform.Find("Visual_Model");
        if (t != null)
        {
            DestroyImmediate(t.gameObject);
        }

        GameObject obj1 = new GameObject("Visual_Model");
        obj1.transform.SetParent(this.transform);
        obj1.transform.localPosition = Vector3.zero;
        obj1.transform.localRotation = Quaternion.identity;
        obj1.transform.localScale = Vector3.one;

        Shader s1 = Shader.Find("Universal Render Pipeline/Lit");
        if (s1 == null) s1 = Shader.Find("Standard");

        Material mat = new Material(s1);
        mat.color = cao_color;

        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", cao_color);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject b = new GameObject("cao_" + i);
            b.transform.SetParent(obj1.transform);

            Vector2 p = Random.insideUnitCircle * r;
            b.transform.localPosition = new Vector3(p.x, 0, p.y);
            b.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360f), Random.Range(-5f, 15f));

            MeshFilter mf = b.AddComponent<MeshFilter>();
            MeshRenderer mr = b.AddComponent<MeshRenderer>();

            Mesh m = new Mesh();
            Vector3[] dian = new Vector3[5];
            dian[0] = new Vector3(-kuan / 2, 0, -kuan / 2);
            dian[1] = new Vector3(kuan / 2, 0, -kuan / 2);
            dian[2] = new Vector3(kuan / 2, 0, kuan / 2);
            dian[3] = new Vector3(-kuan / 2, 0, kuan / 2);
            dian[4] = new Vector3(0, gao, 0);

            int[] sanjiaoxing = new int[]
            {
                0, 2, 1, 0, 3, 2,
                0, 4, 3, 3, 4, 2, 2, 4, 1, 1, 4, 0
            };

            m.vertices = dian;
            m.triangles = sanjiaoxing;
            m.RecalculateNormals();

            mf.sharedMesh = m;
            mr.sharedMaterial = mat;

            b.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        Debug.Log("grass is fine with right color");
    }
}