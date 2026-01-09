using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameProgressManager : MonoBehaviour
{
    [Header("Mission Targets")]
    public int totalGrass = 20;    
    public int totalWatering = 5;  
    public int totalDigging = 3;   

    [Header("UI References")]
    public TextMeshProUGUI taskStatusText; 
    public GameObject resultPanel;         
    public TextMeshProUGUI endingText;      
    public Button restartButton;           

    [Header("Exit Settings")]
    public GameObject exitDoor;            

    
    private int currentGrass = 0;
    private int currentWater = 0;
    private int currentDig = 0;

    private bool isGameEnded = false;

    void Start()
    {
        
        if (resultPanel != null)
        {
            resultPanel.SetActive(false); 
        }

        // 2. 初始化出口状态
        if (exitDoor != null)
        {
            exitDoor.SetActive(false); 
        }

        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        UpdateUI();
    }

    

    public void OnGrassCut()
    {
        if (currentGrass < totalGrass)
        {
            currentGrass++;
            UpdateUI();
            CheckAllTasksComplete();
        }
    }

    public void OnWatering()
    {
        if (currentWater < totalWatering)
        {
            currentWater++;
            UpdateUI();
            CheckAllTasksComplete();
        }
    }

    public void OnDigging()
    {
        if (currentDig < totalDigging)
        {
            currentDig++;
            UpdateUI();
            CheckAllTasksComplete();
        }
    }

    

    void UpdateUI()
    {
        if (taskStatusText != null)
        {
            
            taskStatusText.text = $"Grass: {currentGrass}/{totalGrass}\n" +
                                 $"Water: {currentWater}/{totalWatering}\n" +
                                 $"Dig: {currentDig}/{totalDigging}";
        }
    }

    void CheckAllTasksComplete()
    {
        
        if (AreAllTasksCompleted())
        {
            OpenExit();
        }
    }

    void OpenExit()
    {
        if (exitDoor != null && !exitDoor.activeSelf)
        {
            exitDoor.SetActive(true); 
            Debug.Log("Work finished. The gate is open.");
        }
    }

    

    
    public void TriggerEnding()
    {
        
        
        if (isGameEnded || !AreAllTasksCompleted())
        {
            if (!AreAllTasksCompleted()) Debug.Log("Attempted to trigger ending, but tasks are incomplete.");
            return;
        }

        isGameEnded = true;

        if (resultPanel != null)
        {
            resultPanel.SetActive(true); 

            if (endingText != null)
            {
                
                endingText.text = "You finished your work, but you feel something stayed here forever.";
            }

            
            if (taskStatusText != null) taskStatusText.gameObject.SetActive(false);

            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    
    public void RestartGame()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    public bool AreAllTasksCompleted()
    {
        return currentGrass >= totalGrass && currentWater >= totalWatering && currentDig >= totalDigging;
    }

    
    internal void ShowEnding()
    {
        TriggerEnding();
    }
}