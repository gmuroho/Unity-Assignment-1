using UnityEngine;

public class GrassSetupPro : MonoBehaviour
{
    public string miao_zhun_ceng = "Grass";
    public bool yao_peng_zhuang = true;
    public bool qing_li_jiu_de = true;

    [ContextMenu("piont me start create grass")]
    public void Setup()
    {
        int ceng_index = LayerMask.NameToLayer(miao_zhun_ceng);
        if (ceng_index == -1)
        {
            Debug.LogError("wrong cant find layer: " + miao_zhun_ceng + " zaiLayers add one");
            return;
        }

        int jishu = 0;

        Transform[] all_er_zi = GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < all_er_zi.Length; i++)
        {
            Transform dang_qian = all_er_zi[i];
            MeshFilter wg = dang_qian.GetComponent<MeshFilter>();

            if (wg != null)
            {
                dang_qian.gameObject.layer = ceng_index;

                if (qing_li_jiu_de)
                {
                    Collider[] jiu_de = dang_qian.GetComponents<Collider>();
                    for (int j = 0; j < jiu_de.Length; j++)
                    {
                        if (!(jiu_de[j] is MeshCollider))
                        {
                            DestroyImmediate(jiu_de[j]);
                        }
                    }
                }

                if (yao_peng_zhuang)
                {
                    if (dang_qian.GetComponent<MeshCollider>() == null)
                    {
                        dang_qian.gameObject.AddComponent<MeshCollider>();
                    }
                }

                if (dang_qian.GetComponent<TrimmableObject>() == null)
                {
                    dang_qian.gameObject.AddComponent<TrimmableObject>();
                }

                jishu++;
            }
        }

        Debug.Log("over, " + jishu + " gecaoyiwancheng");
    }
}