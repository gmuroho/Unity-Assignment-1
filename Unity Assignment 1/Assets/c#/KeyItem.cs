using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("References")]
    public GameProgressManager progressManager; // 拖入场景中的 GameProgressManager

    [Header("Visual Effects")]
    public float rotationSpeed = 50.0f; // 钥匙自转速度，方便玩家发现

    void Update()
    {
        // 让钥匙在空中自转，增加视觉引导
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测碰撞物体是否为玩家
        if (other.CompareTag("Player"))
        {
            if (progressManager != null)
            {
                // 1. 修改全局逻辑变量，允许触发结局 C
                progressManager.hasSpecialKey = true;

                // 2. 在屏幕日志中显示提示（如果 logUI 已分配）
                if (progressManager.logUI != null)
                {
                    progressManager.logUI.text = "You found a strange key... Maybe it leads somewhere.";
                }

                Debug.Log("Special Key Picked Up! Ending C is now available.");

                // 3. 销毁或隐藏钥匙物体，防止重复触发
                gameObject.SetActive(false);
                // 或者使用 Destroy(gameObject);
            }
            else
            {
                Debug.LogError("KeyItem: GameProgressManager 未在 Inspector 面板中分配！");
            }
        }
    }
}