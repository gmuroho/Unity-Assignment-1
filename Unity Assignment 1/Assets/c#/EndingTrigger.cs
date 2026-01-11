using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    [Header("References")]
    public GameProgressManager progressManager; // 引用进度管理器

    [Header("Trigger Settings")]
    [Tooltip("设置此触发器对应的结局类型：A (正常完成), B (失败/死亡), C (秘密钥匙)")]
    public string endingType = "A";

    private void OnTriggerEnter(Collider other)
    {
        // 确保只有玩家能触发结局
        if (other.CompareTag("Player"))
        {
            if (progressManager == null)
            {
                Debug.LogError("EndingTrigger: 未在 Inspector 中指定 GameProgressManager！");
                return;
            }

            // 根据设置的结局类型进行逻辑判定
            switch (endingType.ToUpper())
            {
                case "A":
                    // 结局A：只有当任务全部完成时才允许触发
                    if (progressManager.AreAllTasksCompleted())
                    {
                        progressManager.ShowEnding("A");
                    }
                    else
                    {
                        // 如果任务没完成，可以根据需要在这里给玩家一个简单的提示，或者保持静默
                        Debug.Log("任务未完成，无法从此处离开。");
                    }
                    break;

                case "C":
                    // 结局C：只有当玩家拾取了特殊钥匙 (hasSpecialKey) 时才允许触发
                    if (progressManager.hasSpecialKey)
                    {
                        progressManager.ShowEnding("C");
                    }
                    else
                    {
                        Debug.Log("你似乎缺少一把特定的钥匙来开启这条路。");
                    }
                    break;

                case "B":
                    // 结局B：通常是由时间耗尽自动触发，但如果场景中有危险区域导致死亡结局，也可以手动触发
                    progressManager.ShowEnding("B");
                    break;

                default:
                    Debug.LogWarning("EndingTrigger: 未知的结局类型 " + endingType);
                    break;
            }
        }
    }
}