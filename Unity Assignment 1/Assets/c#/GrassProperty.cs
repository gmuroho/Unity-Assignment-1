using UnityEngine;

public class GrassProperty : MonoBehaviour
{
    public int xue_liang = 2;
    public float bian_short = 0.4f;

    Collider pz_box;
    Vector3 yuan_lai_chicun;

    public GameObject cao_gao_model;
    public GameObject cao_di_model;

    void Awake()
    {
        pz_box = GetComponent<Collider>();
        yuan_lai_chicun = transform.localScale;
    }

    public void Cut()
    {
        if (xue_liang <= 0)
        {
            return;
        }

        xue_liang = xue_liang - 1;

        Vector3 currentS = transform.localScale;
        currentS.y = currentS.y - (yuan_lai_chicun.y * bian_short);
        transform.localScale = currentS;

        if (xue_liang == 1)
        {
            if (cao_gao_model != null) cao_gao_model.SetActive(false);
            if (cao_di_model != null) cao_di_model.SetActive(true);
        }

        if (xue_liang <= 0)
        {
            if (pz_box != null)
            {
                pz_box.enabled = false;
            }

            Vector3 finalS = transform.localScale;
            finalS.y = 0.05f;
            transform.localScale = finalS;

            if (cao_di_model != null) cao_di_model.SetActive(false);

            GameProgressManager script = Object.FindFirstObjectByType<GameProgressManager>();
            if (script != null)
            {
                script.OnGrassCut();
            }
        }
    }
}