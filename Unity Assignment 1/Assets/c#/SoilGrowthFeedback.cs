using UnityEngine;
using System.Collections;

public class SoilGrowthFeedback : MonoBehaviour
{
    public Color gan_yanse = new Color(0.6f, 0.4f, 0.2f);
    public Color shi_yanse = new Color(0.2f, 0.1f, 0.05f);

    public GameObject zhiwu_moxing;

    private Renderer my_renderer;
    private bool yi_jing_shi_le = false;
    private float jiaoshui_jindu = 0f;

    void Start()
    {
        my_renderer = GetComponent<Renderer>();
        if (my_renderer != null)
        {
            my_renderer.material.color = gan_yanse;
        }

        if (zhiwu_moxing != null)
        {
            zhiwu_moxing.SetActive(false);
        }
    }

    public void ReceiveWater(float amount)
    {
        if (yi_jing_shi_le == true)
        {
            return;
        }

        jiaoshui_jindu = jiaoshui_jindu + amount;

        if (jiaoshui_jindu > 1.0f)
        {
            jiaoshui_jindu = 1.0f;
        }

        if (my_renderer != null)
        {
            float t = jiaoshui_jindu;
            my_renderer.material.color = Color.Lerp(gan_yanse, shi_yanse, t);
        }

        if (jiaoshui_jindu >= 1.0f)
        {
            if (yi_jing_shi_le == false)
            {
                yi_jing_shi_le = true;

                if (zhiwu_moxing != null)
                {
                    zhiwu_moxing.SetActive(true);
                    StartCoroutine(GrowAnimation());
                }

                GameProgressManager info_manager = Object.FindFirstObjectByType<GameProgressManager>();
                if (info_manager != null)
                {
                    info_manager.OnWatering();
                }

                Debug.Log("finish");
            }
        }
    }

    IEnumerator GrowAnimation()
    {
        float time_count = 0f;
        float total_time = 0.8f;

        Vector3 daxiao = zhiwu_moxing.transform.localScale;
        zhiwu_moxing.transform.localScale = new Vector3(0, 0, 0);

        while (time_count < total_time)
        {
            time_count = time_count + Time.deltaTime;
            float t = time_count / total_time;

            float curr_x = daxiao.x * t;
            float curr_y = daxiao.y * t;
            float curr_z = daxiao.z * t;

            zhiwu_moxing.transform.localScale = new Vector3(curr_x, curr_y, curr_z);

            yield return null;
        }

        zhiwu_moxing.transform.localScale = daxiao;
    }
}