using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameProgressManager : MonoBehaviour
{
    [Header("Mission Settings")]
    public int cao_total = 20;
    public int shui_total = 5;
    public int dig_total = 3;

    [Header("UI References")]
    public TextMeshProUGUI ui1;
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI logUI;
    public GameObject mianban;          
    public TextMeshProUGUI end_txt;
    public Button btn_cc;               
    public Image darkOverlay;           

    [Header("Environment & Player")]
    public GameObject men;
    public Light sunLight;
    public GameObject player;
    public AudioSource horrorAudio;

    [Header("Ending Logic")]
    public bool hasSpecialKey = false;

    private int c1 = 0;
    private int s2 = 0;
    private int d3 = 0;
    private bool is_over = false;
    private bool eventTriggered = false;
    private bool bEndingStarted = false;

    [Header("Time & Lighting System")]
    public float gameTime = 15.0f;
    public float timeFlowSpeed = 0.04f; 
    public float maxIntensity = 1.2f;
    public float minIntensity = 0.05f;

    private Color dayColor = Color.white;
    private Color sunsetColor = new Color(1f, 0.45f, 0.2f);
    private Color nightColor = new Color(0.05f, 0.05f, 0.15f);

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
        StartCoroutine(StartGameSequence());
    }

    public void OnGrassCut() { if (!is_over) { c1++; CheckProgress(); } }
    public void OnWatering() { if (!is_over) { s2++; CheckProgress(); } }
    public void OnDigging() { if (!is_over) { d3++; CheckProgress(); } }

    void CheckProgress()
    {
        UpdateUI();
        if (AreAllTasksCompleted() && !eventTriggered)
        {
            StartCoroutine(AllTasksFinishedSequence());
        }
    }

    void Update()
    {
        if (is_over) return;

        gameTime += Time.deltaTime * timeFlowSpeed;
        if (gameTime > 23.9f) gameTime = 23.9f;

        ApplyNaturalLighting();
        UpdateUI();

        if (gameTime >= 21.0f && !AreAllTasksCompleted() && !bEndingStarted)
        {
            StartCoroutine(SequenceEndingB());
        }
    }

    void ApplyNaturalLighting()
    {
        if (sunLight == null || bEndingStarted || eventTriggered) return;

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
            ui1.text = "Grass Removed: " + c1 + "/" + cao_total + "\n" +
                        "Water Applied: " + s2 + "/" + shui_total + "\n" +
                        "Pits Dug: " + d3 + "/" + dig_total;
        }

        if (timeUI != null)
        {
            int hours = Mathf.FloorToInt(gameTime);
            int mins = Mathf.FloorToInt((gameTime - hours) * 60);

            
            string displayMins = (mins >= 30) ? "30" : "00";
            timeUI.text = string.Format("Time: {0:00}:{1}", hours, displayMins);
        }
    }

    public void ObtainKey()
    {
        hasSpecialKey = true;
        StopCoroutine("KeyMessageRoutine");
        StartCoroutine(KeyMessageRoutine());
    }

    IEnumerator KeyMessageRoutine()
    {
        if (logUI != null)
        {
            logUI.text = "You have found this key. It might lead you to another place...";
            logUI.gameObject.SetActive(true);
            yield return new WaitForSeconds(7.0f);
            logUI.text = "";
        }
    }

    IEnumerator StartGameSequence()
    {
        if (logUI != null)
        {
            logUI.text = "Welcome back. Please fulfill your duties.";
            logUI.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(5.0f);
        if (logUI != null) logUI.text = "";
        ui1.gameObject.SetActive(true);
    }

    IEnumerator AllTasksFinishedSequence()
    {
        eventTriggered = true;
        if (logUI != null) logUI.text = "Duties fulfilled. I should leave now.";

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
        if (logUI != null)
        {
            logUI.text = "Something went wrong... it's too dark.";
            logUI.gameObject.SetActive(true);
        }

        for (int i = 1; i <= 4; i++)
        {
            yield return StartCoroutine(ShakeUIEffect(1.5f, i * 3.0f));

            if (horrorAudio != null)
            {
                horrorAudio.volume = i * 0.25f;
                if (!horrorAudio.isPlaying) horrorAudio.Play();
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

    IEnumerator ShakeUIEffect(float duration, float magnitude)
    {
        Vector3 originalPos = logUI.transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            logUI.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        logUI.transform.localPosition = originalPos;
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
            
            mianban.transform.SetAsLastSibling();
        }

        if (type == "A")
        {
            end_txt.text = "ENDING A: JOB WELL DONE.\nYou escaped the estate safely.";
        }
        else if (type == "B")
        {
            end_txt.text = "ENDING B: SOMETHING WENT WRONG.\nYou are trapped here forever.";
            end_txt.color = Color.red;
            
            if (darkOverlay != null) darkOverlay.color = new Color(0, 0, 0, 0.85f);
        }
        else if (type == "C")
        {
            end_txt.text = "ENDING C: THE FORBIDDEN PATH.\nYou found what was hidden.";
            end_txt.color = Color.cyan;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool AreAllTasksCompleted() { return c1 >= cao_total && s2 >= shui_total && d3 >= dig_total; }
    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerEnding(string v) { if (!is_over) ShowEnding(v); }
}