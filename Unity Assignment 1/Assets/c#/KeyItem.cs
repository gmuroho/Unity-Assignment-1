using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public GameProgressManager jindu_guanli;
    public float xuanzhuan_sudu = 50.0f;

    void Update()
    {
        float mei_zhen_sudu = xuanzhuan_sudu * Time.deltaTime;
        transform.Rotate(Vector3.up, mei_zhen_sudu);
    }

    void OnTriggerEnter(Collider peng_zhuang_ti)
    {
        if (peng_zhuang_ti.gameObject.tag == "Player")
        {
            if (jindu_guanli != null)
            {
                jindu_guanli.hasSpecialKey = true;

                if (jindu_guanli.logUI != null)
                {
                    jindu_guanli.logUI.text = "You found a strange key... Maybe it leads somewhere.";
                }

                Debug.Log("Special Key Picked Up! Ending C is now available.");

                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("KeyItem: GameProgressManagermeizhaodao");
            }
        }
    }
}