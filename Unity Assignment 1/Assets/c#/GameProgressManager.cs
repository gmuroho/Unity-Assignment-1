using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameProgressManager : MonoBehaviour
{
    public int cao_total = 20;
    public int shui_total = 5;
    public int dig_total = 3;

    public TextMeshProUGUI ui1;
    public GameObject mianban;
    public TextMeshProUGUI end_txt;
    public Button btn_cc;

    public GameObject men;

    int c1 = 0;
    int s2 = 0;
    int d3 = 0;

    bool is_over = false;

    void Start()
    {
        mianban.SetActive(false);
        men.SetActive(false);
        btn_cc.onClick.AddListener(RestartGame);
        UpdateUI();
    }

    public void OnGrassCut()
    {
        if (c1 < cao_total)
        {
            c1++;
            UpdateUI();
            check();
        }
    }

    public void OnWatering()
    {
        if (s2 < shui_total)
        {
            s2++;
            UpdateUI();
            check();
        }
    }

    public void OnDigging()
    {
        if (d3 < dig_total)
        {
            d3++;
            UpdateUI();
            check();
        }
    }

    void UpdateUI()
    {
        ui1.text = "Grass: " + c1 + "/" + cao_total + "\n" +
                   "Water: " + s2 + "/" + shui_total + "\n" +
                   "Dig: " + d3 + "/" + dig_total;
    }

    void check()
    {
        if (AreAllTasksCompleted())
        {
            if (!men.activeSelf)
            {
                men.SetActive(true);
            }
        }
    }

    public void TriggerEnding()
    {
        if (is_over || !AreAllTasksCompleted())
        {
            return;
        }

        is_over = true;
        mianban.SetActive(true);
        end_txt.text = "You finished your work, but you feel something stayed here forever.";
        ui1.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool AreAllTasksCompleted()
    {
        return c1 >= cao_total && s2 >= shui_total && d3 >= dig_total;
    }

    public void ShowEnding()
    {
        TriggerEnding();
    }
}