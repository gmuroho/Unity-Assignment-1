using UnityEngine;
using System.Collections;

public class WaterPourEffect : MonoBehaviour
{
    public Mesh waterMesh;
    public Material waterMaterial;

    public float shuidishuliang = 20f;
    public float chushisudu = 3f;
    public float cunhuoshijian = 1.0f;

    public float minScale = 0.02f;
    public float maxScale = 0.04f;

    private float miao = 0f;

    void Update()
    {
        if (gameObject.activeInHierarchy == true)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= miao)
                {
                    miao = Time.time + 1f / shuidishuliang;

                    GameObject xiao_shuidi = new GameObject("WaterDrop");
                    xiao_shuidi.transform.position = transform.position;

                    float suiji_daxiao = Random.Range(minScale, maxScale);
                    xiao_shuidi.transform.localScale = new Vector3(suiji_daxiao, suiji_daxiao, suiji_daxiao);

                    MeshFilter filter = xiao_shuidi.AddComponent<MeshFilter>();
                    filter.mesh = waterMesh;

                    MeshRenderer renderer = xiao_shuidi.AddComponent<MeshRenderer>();
                    renderer.material = waterMaterial;

                    Rigidbody gangti = xiao_shuidi.AddComponent<Rigidbody>();
                    gangti.mass = 0.1f;

                    Vector3 fangxiang = transform.forward + (Random.insideUnitSphere * 0.1f);
                    gangti.AddForce(fangxiang * chushisudu, ForceMode.Impulse);

                    WaterDropLogic luoji = xiao_shuidi.AddComponent<WaterDropLogic>();
                    luoji.max_time = cunhuoshijian;
                    luoji.kaishi_scale = xiao_shuidi.transform.localScale;
                }
            }
        }
    }
}

public class WaterDropLogic : MonoBehaviour
{
    public float max_time;
    public Vector3 kaishi_scale;
    private float jishiqi = 0;

    void Update()
    {
        jishiqi = jishiqi + Time.deltaTime;

        if (jishiqi < max_time)
        {
            float baifenbi = jishiqi / max_time;
            transform.localScale = Vector3.Lerp(kaishi_scale, Vector3.zero, baifenbi);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}