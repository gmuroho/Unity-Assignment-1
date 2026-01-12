using UnityEngine;
using UnityEngine.UI;

public class WaterCanTrigger : MonoBehaviour
{
    public GameObject plantPrefab;
    public Transform spawnPoint;
    public float requiredWaterTime = 3.0f;
    public Image fillUI;

    private float currentWaterTime = 0;
    private bool isWatered = false;
    private bool hasShownPatienceTip = false;

    private GameProgressManager progressManager;

    [System.Obsolete]
    void Start()
    {
        progressManager = (GameProgressManager)GameObject.FindObjectOfType(typeof(GameProgressManager));

        if (fillUI != null)
        {
            fillUI.fillAmount = 0;
        }
    }

    void OnMouseOver()
    {
        if (isWatered == true)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (hasShownPatienceTip == false)
            {
                if (progressManager != null)
                {
                    progressManager.ShowLog("You need to be patient... see what happens.");
                }
                hasShownPatienceTip = true;
            }

            currentWaterTime = currentWaterTime + Time.deltaTime;

            if (fillUI != null)
            {
                fillUI.fillAmount = currentWaterTime / requiredWaterTime;
            }

            if (currentWaterTime >= requiredWaterTime)
            {
                isWatered = true;

                if (fillUI != null)
                {
                    fillUI.gameObject.SetActive(false);
                }

                if (plantPrefab != null)
                {
                    if (spawnPoint != null)
                    {
                        Instantiate(plantPrefab, spawnPoint.position, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(plantPrefab, transform.position, Quaternion.identity);
                    }
                }

                if (progressManager != null)
                {
                    progressManager.IncrementWatered();
                }

                this.enabled = false;
            }
        }
        else
        {
            currentWaterTime = currentWaterTime - Time.deltaTime;

            if (currentWaterTime < 0)
            {
                currentWaterTime = 0;
            }

            if (fillUI != null)
            {
                fillUI.fillAmount = currentWaterTime / requiredWaterTime;
            }
        }
    }
}