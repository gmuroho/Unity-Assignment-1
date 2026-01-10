using UnityEngine;
using System.Collections;

public class GrassMower : MonoBehaviour
{
    public float jiancao_r = 0.5f;
    public LayerMask cao_layer;
    public Transform doudou;

    public void PerformMow()
    {
        if (doudou == null)
        {
            doudou = transform;
        }

        Collider[] objs = Physics.OverlapSphere(doudou.position, jiancao_r, cao_layer);

        for (int i = 0; i < objs.Length; i++)
        {
            MowableGrass target = objs[i].GetComponent<MowableGrass>();
            if (target != null)
            {
                Vector3 fx = (objs[i].transform.position - doudou.position).normalized;
                fx = fx + Vector3.up * 0.5f;
                target.OnMown(fx);
            }
        }
    }
}

public class MowableGrass : MonoBehaviour
{
    public GameObject lizi;
    public GameObject moxing;

    public float fly = 5f;
    public float tor = 10f;

    bool yi_jian_guo = false;

    public void OnMown(Vector3 dir)
    {
        if (yi_jian_guo == true)
        {
            return;
        }
        yi_jian_guo = true;

        if (lizi != null)
        {
            Instantiate(lizi, transform.position, Quaternion.identity);
        }

        Rigidbody g_rb = GetComponent<Rigidbody>();
        if (g_rb == null)
        {
            g_rb = gameObject.AddComponent<Rigidbody>();
        }

        g_rb.isKinematic = false;
        g_rb.useGravity = true;

        g_rb.AddForce(dir * fly, ForceMode.Impulse);

        Vector3 sui_ji_li = new Vector3(Random.value, Random.value, Random.value);
        g_rb.AddTorque(sui_ji_li * tor, ForceMode.Impulse);

        Collider c = GetComponent<Collider>();
        if (c != null)
        {
            c.isTrigger = false;
        }

        Destroy(gameObject, 3f);
    }
}