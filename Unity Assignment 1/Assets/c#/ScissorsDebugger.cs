using UnityEngine;

public class ScissorsDebugger : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;
    Vector3 startSize;

    void Awake()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        startSize = transform.localScale;

        Debug.Log("weizhi jilu le");
    }

    void Update()
    {
        MeshRenderer mian = GetComponent<MeshRenderer>();
        if (mian != null)
        {
            if (mian.enabled == false)
            {
                mian.enabled = true;
                Debug.Log("kaiqi xianshi");
            }
        }

        float juli = Vector3.Distance(transform.localPosition, startPos);
        if (juli > 0.01f)
        {
            transform.localPosition = startPos;
            transform.localRotation = startRot;
            transform.localScale = startSize;
            Debug.Log("weizhi cuole, huanyuan");
        }

        int ceng = gameObject.layer;
        // zheli jiancha cengji
    }
}