using UnityEngine;

public class EnvironmentalDistortion : MonoBehaviour
{
    public float kaishishijian = 18.0f;
    public float doudongqiangdu = 0.02f;
    public float doudongshijian = 1.0f;
    public float zuixiaojiange = 5.0f;
    public float zuidajiange = 15.0f;

    Transform xiangji;
    GameProgressManager jdglq;

    float xianzai_shijian = 0;
    float xiayici_kaishi = 0;
    bool shi_fouzai_dou = false;
    Vector3 yuanshilocation;

    void Start()
    {
        if (Camera.main != null)
        {
            xiangji = Camera.main.transform;
        }

        jdglq = Object.FindFirstObjectByType<GameProgressManager>();

        
        xiayici_kaishi = Time.time + 5.0f;
    }

    void Update()
    {
        if (jdglq == null || xiangji == null) return;

        
        if (jdglq.gameTime > kaishishijian && jdglq.AreAllTasksCompleted() == false)
        {
            
            if (shi_fouzai_dou == false && Time.time > xiayici_kaishi)
            {
                shi_fouzai_dou = true;
                xianzai_shijian = 0;
                yuanshilocation = xiangji.localPosition;
            }
        }

        
        if (shi_fouzai_dou == true)
        {
            if (xianzai_shijian < doudongshijian)
            {
                float suiji_x = Random.Range(-1f, 1f) * doudongqiangdu;
                float suiji_y = Random.Range(-1f, 1f) * doudongqiangdu;

                xiangji.localPosition = new Vector3(yuanshilocation.x + suiji_x, yuanshilocation.y + suiji_y, yuanshilocation.z);

                xianzai_shijian = xianzai_shijian + Time.deltaTime;
            }
            else
            {
                
                xiangji.localPosition = yuanshilocation;
                shi_fouzai_dou = false;

                
                float suiji_jiange = Random.Range(zuixiaojiange, zuidajiange);
                xiayici_kaishi = Time.time + suiji_jiange;
            }
        }
    }
}