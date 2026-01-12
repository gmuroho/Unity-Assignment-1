using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    
    public GameProgressManager guanliqi;

    
    public string jiejuleixing = "A";

    void OnTriggerEnter(Collider qita)
    {
        
        if (qita.CompareTag("Player"))
        {
            if (guanliqi != null)
            {
                
                if (jiejuleixing == "A" || jiejuleixing == "a")
                {
                    bool renwu_wancheng = guanliqi.AreAllTasksCompleted();
                    if (renwu_wancheng == true)
                    {
                        guanliqi.ShowEnding("A");
                    }
                    else
                    {
                        
                        Debug.Log("task not finished");
                    }
                }
                
                else if (jiejuleixing == "C" || jiejuleixing == "c")
                {
                    
                    if (guanliqi.hasSpecialKey == true)
                    {
                        guanliqi.ShowEnding("C");
                    }
                    else
                    {
                        
                        string tishi_wenzi = "It seems you need some other things to open this passage";
                        guanliqi.ShowLog(tishi_wenzi);
                        Debug.Log(tishi_wenzi);
                    }
                }
                
                else if (jiejuleixing == "B" || jiejuleixing == "b")
                {
                    guanliqi.ShowEnding("B");
                }
                else
                {
                    Debug.LogWarning("name not right" + jiejuleixing);
                }
            }
            else
            {
                
                Debug.LogError("queshaozujian");
            }
        }
    }
}