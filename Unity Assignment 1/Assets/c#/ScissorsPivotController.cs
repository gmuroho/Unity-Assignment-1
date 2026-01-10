using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorsPivotController : MonoBehaviour
{
    public Transform zuo;
    public Transform you;

    [Range(0, 60)] public float kai = 30f;
    [Range(-10, 10)] public float guan = 0f;

    public float s1 = 0.1f;
    public float s2 = 0.2f;

    bool isMoving = false;

    void Start()
    {
        if (zuo == null || you == null)
        {
            Debug.Log("dongxi mei tuo jin lai!");
            return;
        }

        SetAngle(kai);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isMoving == false)
        {
            StartCoroutine(DoCut());
        }
    }

    IEnumerator DoCut()
    {
        isMoving = true;

        float t1 = 0;
        while (t1 < s1)
        {
            t1 += Time.deltaTime;
            float val = Mathf.Lerp(kai, guan, t1 / s1);
            SetAngle(val);
            yield return null;
        }
        SetAngle(guan);

        yield return new WaitForSeconds(0.02f);

        float t2 = 0;
        while (t2 < s2)
        {
            t2 += Time.deltaTime;
            float val = Mathf.Lerp(guan, kai, t2 / s2);
            SetAngle(val);
            yield return null;
        }
        SetAngle(kai);

        isMoving = false;
    }

    void SetAngle(float a)
    {
        zuo.localRotation = Quaternion.Euler(0, 0, a);
        you.localRotation = Quaternion.Euler(0, 0, -a);
    }

    void OnValidate()
    {
        if (!Application.isPlaying && zuo != null && you != null)
        {
            SetAngle(kai);
        }
    }
}