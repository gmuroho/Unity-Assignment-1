using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScissorsPivotController : MonoBehaviour
{
    [Header("cengjiyinyong")]
    [Tooltip("tuoru group left")]
    public Transform groupLeft;
    [Tooltip("tuoru group right")]
    public Transform groupRight;


[Header("kaihejiaodu")]
    [Range(0, 60)] public float openAngle = 30f;   // zhangkai
    [Range(-10, 10)] public float closeAngle = 0f; // helong

    [Header("donghua")]
    public float cutSpeed = 0.1f;    
    public float recoverSpeed = 0.2f; 

    private bool isCutting = false;

    void Start()
    {
        // check
        if (groupLeft == null || groupRight == null)
        {
            Debug.LogError("请在 Models_Offsets 的脚本槽位中手动拖入 group left 和 group right！");
            return;
        }

        // original
        SetScissorsAngle(openAngle);
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && !isCutting)
        {
            StartCoroutine(PerformCut());
        }
    }

    IEnumerator PerformCut()
    {
        isCutting = true;

        
        float elapsed = 0;
        while (elapsed < cutSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cutSpeed;
            
            float current = Mathf.Lerp(openAngle, closeAngle, t);
            SetScissorsAngle(current);
            yield return null;
        }
        SetScissorsAngle(closeAngle);

        
        yield return new WaitForSeconds(0.02f);

        
        elapsed = 0;
        while (elapsed < recoverSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoverSpeed;
            float current = Mathf.Lerp(closeAngle, openAngle, t);
            SetScissorsAngle(current);
            yield return null;
        }
        SetScissorsAngle(openAngle);

        isCutting = false;
    }

    
    
    
    void SetScissorsAngle(float angle)
    {
        
        
        
        groupLeft.localRotation = Quaternion.Euler(0, 0, angle);
        groupRight.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    
    void OnValidate()
    {
        if (!Application.isPlaying && groupLeft != null && groupRight != null)
        {
            SetScissorsAngle(openAngle);
        }
    }



}