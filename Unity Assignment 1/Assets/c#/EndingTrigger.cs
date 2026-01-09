using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
   
    [Header("引用设置")]
    
    public GameProgressManager progressManager;

    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if (progressManager != null)
            {
                
                progressManager.ShowEnding();
            }
            else
            {
                Debug.LogError("EndingTrigger 脚本上的 Progress Manager 槽位空");
            }
        }
    }
}