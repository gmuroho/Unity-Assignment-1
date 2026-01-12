using UnityEngine;
using System.Collections;

public class JuiceParticleEffect : MonoBehaviour
{
    public Mesh[] suoyou_moxing;
    public Material lizi_caizhi;

    public int geshu = 10;
    public float pen_lidu = 5f;
    public float huo_duochang_shijian = 1.2f;

    public float zuixiao_daxiao = 0.05f;
    public float zuida_daxiao = 0.08f;

    void Update()
    {
        if (gameObject.activeInHierarchy == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int i = 0;
                while (i < geshu)
                {
                    GameObject xiao_lizi = new GameObject("JuiceParticle");
                    xiao_lizi.transform.position = transform.position;

                    float suiji_suofang = Random.Range(zuixiao_daxiao, zuida_daxiao);
                    xiao_lizi.transform.localScale = new Vector3(suiji_suofang, suiji_suofang, suiji_suofang);

                    MeshFilter mf = xiao_lizi.AddComponent<MeshFilter>();
                    if (suoyou_moxing != null)
                    {
                        if (suoyou_moxing.Length > 0)
                        {
                            int n = Random.Range(0, suoyou_moxing.Length);
                            mf.mesh = suoyou_moxing[n];
                        }
                    }

                    MeshRenderer mr = xiao_lizi.AddComponent<MeshRenderer>();
                    mr.material = lizi_caizhi;

                    Rigidbody niudong = xiao_lizi.AddComponent<Rigidbody>();

                    Vector3 fx = transform.forward + Random.insideUnitSphere * 0.6f;
                    niudong.AddForce(fx * pen_lidu, ForceMode.Impulse);

                    Vector3 niu_li = Random.insideUnitSphere * 15f;
                    niudong.AddTorque(niu_li);

                    Destroy(xiao_lizi, huo_duochang_shijian);

                    i++;
                }
            }
        }
    }
}