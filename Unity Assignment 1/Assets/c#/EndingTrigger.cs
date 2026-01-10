using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public GameProgressManager guanjia;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            guanjia.ShowEnding();
        }
    }
}