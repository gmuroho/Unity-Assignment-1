using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameProgressManager : MonoBehaviour
{
    public int cao_total = 20;
    public int shui_total = 6;
    public int dig_total = 3;

    public TextMeshProUGUI ui1;
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI logUI;
    public GameObject mianban;
    public TextMeshProUGUI end_txt;
    public Button btn_cc;
    public Image darkOverlay;

    public GameObject men;
    public Light sunLight;
    public GameObject player;
    public AudioSource horrorAudio;

    public bool hasSpecialKey = false;

    private int c1 = 0;
    private int s2 = 0;
    private int d3 = 0;
    private bool is_over = false;
    private bool eventTriggered = false;
    private bool bEndingStarted = false;
    private bool hasShownWaterTip = false;

    public float gameTime = 15.0f;
    public float timeFlowSpeed = 0.025f;
    public float maxIntensity = 1.2f;
    public float minIntensity = 0.05f;

    public GameObject toolWaterCan;

    private Color dayColor = Color.white;
    private Color sunsetColor = new Color(1f, 0.45f, 0.2f);
    private Color nightColor = new Color(0.05f, 0.05f, 0.15f);

    // 大学生手写计时器变量
    float log_timer = 0;
    float tip_timer = 0;
    bool showing_log = false;
    bool showing_tip = false;

    void Start()
    {
        mianban.SetActive(false);
        men.SetActive(false);
        ui1.gameObject.SetActive(false);

        if (btn_cc != null)
        {
            btn_cc.gameObject.SetActive(false);
            btn_cc.onClick.AddListener(RestartGame);
        }

        if (darkOverlay != null) darkOverlay.color = new Color(0, 0, 0, 0);

        UpdateUI();

        // 开场白
        if (logUI != null)
        {
            logUI.text = "Welcome back. Please fulfill your duties.";
            logUI.gameObject.SetActive(true);
            showing_log = true;
            log_timer = 5.0f;
        }
        ui1.gameObject.SetActive(true);
    }

    public void OnGrassCut()
    {
        if (is_over == false)
        {
            c1 = c1 + 1;
            CheckProgress();
        }
    }

    public void OnWatering()
    {
        if (is_over == false)
        {
            s2 = s2 + 1;
            CheckProgress();
            if (hasShownWaterTip == false)
            {
                ShowWateringTip();
            }
        }
    }

    public void IncrementWatered()
    {
        OnWatering();
    }

    public void ShowLog(string message)
    {
        if (is_over) return;
        if (logUI != null)
        {
            logUI.gameObject.SetActive(true);
            logUI.text = message;
            showing_log = true;
            log_timer = 3.0f;
        }
    }

    public void OnDigging()
    {
        if (is_over == false)
        {
            d3 = d3 + 1;
            CheckProgress();
        }
    }

    public void ShowKeyMissingHint(string message)
    {
        ShowLog(message);
    }

    public void ShowWateringTip()
    {
        if (is_over || hasShownWaterTip) return;
        hasShownWaterTip = true;
        if (logUI != null)
        {
            logUI.gameObject.SetActive(true);
            logUI.text = "Try watering the unusual soil... something might grow.";
            showing_tip = true;
            tip_timer = 5.0f;
        }
    }

    void CheckProgress()
    {
        UpdateUI();
        if (AreAllTasksCompleted() == true && eventTriggered == false)
        {
            eventTriggered = true;
            ShowLog("Duties fulfilled. I should leave now.");
            // 这里为了稳妥，简单的任务完成后的逻辑保留协程调用，但逻辑改到外面
            StartCoroutine(AllTasksFinishedSequence());
        }
    }

    void Update()
    {
        if (is_over == true) return;

        // 检查水壶
        if (hasShownWaterTip == false && toolWaterCan != null && toolWaterCan.activeInHierarchy == false)
        {
            ShowWateringTip();
        }

        // 时间流逝
        gameTime = gameTime + Time.deltaTime * timeFlowSpeed;
        if (gameTime > 23.9f) gameTime = 23.9f;

        // 光照
        ApplyNaturalLighting();

        // 刷新UI
        UpdateUI();

        // 结局B触发
        if (gameTime >= 21.0f && AreAllTasksCompleted() == false && bEndingStarted == false)
        {
            StartCoroutine(SequenceEndingB());
        }

        // 手动处理Log消失计时器
        if (showing_log == true)
        {
            log_timer -= Time.deltaTime;
            if (log_timer <= 0)
            {
                if (logUI != null) logUI.text = "";
                showing_log = false;
            }
        }

        // 手动处理浇水提示计时器
        if (showing_tip == true)
        {
            tip_timer -= Time.deltaTime;
            if (tip_timer <= 0)
            {
                if (logUI.text.Contains("unusual soil"))
                {
                    logUI.text = "Patience is the key to growth. Keep going.";
                    tip_timer = 5.0f;
                }
                else
                {
                    logUI.text = "";
                    showing_tip = false;
                }
            }
        }
    }

    void ApplyNaturalLighting()
    {
        if (sunLight == null || bEndingStarted == true || eventTriggered == true) return;

        if (gameTime < 17.0f)
        {
            sunLight.intensity = maxIntensity;
            sunLight.color = dayColor;
        }
        else if (gameTime >= 17.0f && gameTime < 19.5f)
        {
            float t = (gameTime - 17.0f) / 2.5f;
            sunLight.intensity = Mathf.Lerp(maxIntensity, maxIntensity * 0.6f, t);
            sunLight.color = Color.Lerp(dayColor, sunsetColor, t);
        }
        else if (gameTime >= 19.5f && gameTime <= 21.0f)
        {
            float t = (gameTime - 19.5f) / 1.5f;
            sunLight.intensity = Mathf.Lerp(maxIntensity * 0.6f, minIntensity, t);
            sunLight.color = Color.Lerp(sunsetColor, nightColor, t);
        }
    }

    void UpdateUI()
    {
        if (ui1 != null)
        {
            string s1 = "Grass Removed: " + c1.ToString() + "/" + cao_total.ToString();
            string s22 = "Plants Grown: " + s2.ToString() + "/" + shui_total.ToString();
            string s3 = "Pits Dug: " + d3.ToString() + "/" + dig_total.ToString();
            ui1.text = s1 + "\n" + s22 + "\n" + s3;
        }
        if (timeUI != null)
        {
            int h = (int)gameTime;
            float m_raw = (gameTime - h) * 60;
            int m = (int)m_raw;
            string mins_str = "00";
            if (m >= 30) mins_str = "30";

            string h_str = h.ToString();
            if (h < 10) h_str = "0" + h_str;

            timeUI.text = "Time: " + h_str + ":" + mins_str;
        }
    }

    public void ObtainKey()
    {
        hasSpecialKey = true;
        ShowLog("You found the Old Key. It feels cold to the touch...");
    }

    IEnumerator AllTasksFinishedSequence()
    {
        float duration = 3.0f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (sunLight) sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0.3f, elapsed / duration);
            yield return null;
        }
        if (men != null) men.SetActive(true);
    }

    IEnumerator SequenceEndingB()
    {
        bEndingStarted = true;
        if (logUI != null) logUI.text = "Something went wrong... it's too dark.";

        for (int i = 1; i <= 4; i++)
        {
            // 震动效果
            float duration = 1.5f;
            float mag = i * 3.0f;
            Vector3 op = logUI.transform.localPosition;
            float e = 0;
            while (e < duration)
            {
                logUI.transform.localPosition = op + (Vector3)Random.insideUnitCircle * mag;
                e += Time.deltaTime;
                yield return null;
            }
            logUI.transform.localPosition = op;

            if (horrorAudio != null)
            {
                horrorAudio.volume = i * 0.25f;
                if (horrorAudio.isPlaying == false) horrorAudio.Play();
            }

            float t = 0;
            float targetAlpha = i * 0.25f;
            Color startCol = darkOverlay.color;
            Color endCol = new Color(0, 0, 0, targetAlpha);
            while (t < 2.5f)
            {
                t += Time.deltaTime;
                darkOverlay.color = Color.Lerp(startCol, endCol, t / 2.5f);
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
        }
        ShowEnding("B");
    }

    public void ShowEnding(string type)
    {
        is_over = true;
        mianban.SetActive(true);
        if (ui1) ui1.gameObject.SetActive(false);
        if (timeUI) timeUI.gameObject.SetActive(false);
        if (logUI) logUI.gameObject.SetActive(false);

        if (btn_cc != null)
        {
            btn_cc.gameObject.SetActive(true);
            btn_cc.transform.SetAsLastSibling();
        }

        if (type == "A") end_txt.text = "ENDING A: JOB WELL DONE.";
        if (type == "B")
        {
            end_txt.text = "ENDING B: TRAPPED.";
            end_txt.color = Color.red;
        }
        if (type == "C")
        {
            end_txt.text = "ENDING C: FORBIDDEN.";
            end_txt.color = Color.cyan;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool AreAllTasksCompleted()
    {
        if (c1 >= cao_total && s2 >= shui_total && d3 >= dig_total)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerEnding(string v)
    {
        if (is_over == false) ShowEnding(v);
    }
}