using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 

public class ToolSystem : MonoBehaviour
{
    [Header("手中模型 (Camera下摆好的 0:剪刀, 1:水壶, 2:铲子)")]
    public GameObject[] toolModels;

    [Header("地面模型 (场景中平放的 0:剪刀, 1:水壶, 2:铲子)")]
    public GameObject[] groundObjects;

    [Header("粒子特效 (0:剪草/飞溅, 1:出水, 2:土块)")]
    public ParticleSystem[] effects;

    [Header("拾取音效 (对应 0,1,2 三个工具)")]
    public AudioClip[] pickUpSounds;

    [Header("使用音效 (对应 0:剪刀咔嚓, 1:水壶流声, 2:铲子铲土)")]
    public AudioClip[] useSounds;

    [Header("音频播放源 (挂载在Player或相机上)")]
    public AudioSource audioSource;

    [Header("UI图标")]
    public Image[] toolIcons;

    [Header("GRUNN 提示 UI 设置")]
    public TextMeshProUGUI hintText;  
    public string pickUpPrefix = "拾取 "; 
    public string[] toolNames = { "剪刀", "水壶", "铲子" }; 

    [Header("剪草判定设置")]
    public float reachDistance = 3.5f; 
    public string grassTag = "Grass"; 
    public LayerMask interactableLayers; 

    private int currentToolID = -1;
    private bool isActing = false;

    void Start()
    {
        
        foreach (GameObject go in toolModels) go.SetActive(false);
        foreach (GameObject go in groundObjects) go.SetActive(true);
        UpdateUI();
    }

    void Update()
    {
        
        if (hintText != null) hintText.text = "";
        HandleHintRaycast();
        

        
        if (Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonDown(0) && currentToolID == -1))
        {
            CheckPickUp();
        }

        
        if (Input.GetKeyDown(KeyCode.Q) && currentToolID != -1)
        {
            DropTool();
        }

        
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToPickedTool(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToPickedTool(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToPickedTool(2);

        
        if (currentToolID != -1 && !isActing)
        {
            HandleToolUsage();
        }
    }

    
    void HandleHintRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 3.5f))
        {
            
            PickableItem item = hit.collider.GetComponent<PickableItem>();
            if (item != null)
            {
                int id = item.toolID;
                if (hintText != null && id < toolNames.Length)
                {
                    hintText.text = pickUpPrefix + toolNames[id];
                }
            }
        }
    }

    void CheckPickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 3.5f))
        {
            
            for (int i = 0; i < groundObjects.Length; i++)
            {
                if (hit.collider.gameObject == groundObjects[i])
                {
                    PickUp(i);
                    break;
                }
            }
        }
    }

    void PickUp(int id)
    {
        groundObjects[id].SetActive(false);
        currentToolID = id;

        for (int i = 0; i < toolModels.Length; i++)
        {
            toolModels[i].SetActive(i == id);
        }

        if (audioSource && id < pickUpSounds.Length && pickUpSounds[id])
            audioSource.PlayOneShot(pickUpSounds[id]);

        UpdateUI();
    }

    void DropTool()
    {
        toolModels[currentToolID].SetActive(false);
        groundObjects[currentToolID].SetActive(true);
        groundObjects[currentToolID].transform.position = transform.position + transform.forward * 1.5f;

        currentToolID = -1;
        UpdateUI();
    }

    void SwitchToPickedTool(int id)
    {
        if (id < groundObjects.Length && !groundObjects[id].activeSelf)
        {
            currentToolID = id;
            for (int i = 0; i < toolModels.Length; i++)
            {
                toolModels[i].SetActive(i == id);
            }
            UpdateUI();
        }
    }

    void HandleToolUsage()
    {
        
        if (currentToolID == 0 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ActionAnimation(toolModels[0].transform, Vector3.forward * 0.1f));
            PlayUseEffectAndSound(0);

            
            PerformCutLogic();
        }
        
        else if (currentToolID == 1)
        {
            if (Input.GetMouseButton(0))
            {
                toolModels[1].transform.localRotation = Quaternion.Slerp(toolModels[1].transform.localRotation, Quaternion.Euler(40, 0, 0), Time.deltaTime * 5f);

                
                if (Input.GetMouseButtonDown(0))
                {
                    GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
                    if (manager != null) manager.OnWatering();
                }

                if (effects.Length > 1 && effects[1] && !effects[1].isPlaying) effects[1].Play();
                if (audioSource && useSounds.Length > 1 && useSounds[1] && !audioSource.isPlaying) { audioSource.clip = useSounds[1]; audioSource.Play(); }
            }
            else
            {
                toolModels[1].transform.localRotation = Quaternion.Slerp(toolModels[1].transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                if (effects.Length > 1 && effects[1] && effects[1].isPlaying) effects[1].Stop();
                if (audioSource && useSounds.Length > 1 && audioSource.clip == useSounds[1]) audioSource.Stop();
            }
        }
        
        else if (currentToolID == 2 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ActionAnimation(toolModels[2].transform, new Vector3(0, -0.2f, 0.2f)));
            PlayUseEffectAndSound(2);

            
            GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
            if (manager != null) manager.OnDigging();
        }
    }

    void PerformCutLogic()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance, interactableLayers))
        {
            if (hit.collider.CompareTag(grassTag))
            {
                GrassProperty grass = hit.collider.GetComponent<GrassProperty>();
                if (grass != null)
                {
                    grass.Cut();

                    
                    GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
                    if (manager != null) manager.OnGrassCut();
                }
                else
                {
                    Debug.LogWarning(hit.collider.name + " 标签是 Grass 但没挂 GrassProperty 脚本");
                    Destroy(hit.collider.gameObject);

                    GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
                    if (manager != null) manager.OnGrassCut();
                }
            }
        }
    }

    void PlayUseEffectAndSound(int id)
    {
        if (id < effects.Length && effects[id]) effects[id].Play();
        if (audioSource && id < useSounds.Length && useSounds[id]) audioSource.PlayOneShot(useSounds[id]);
    }

    IEnumerator ActionAnimation(Transform target, Vector3 offset)
    {
        isActing = true;
        Vector3 originalPos = target.localPosition;
        float dur = 0.1f;
        float elapsed = 0;
        while (elapsed < dur) { target.localPosition = Vector3.Lerp(originalPos, originalPos + offset, elapsed / dur); elapsed += Time.deltaTime; yield return null; }
        elapsed = 0;
        while (elapsed < dur) { target.localPosition = Vector3.Lerp(originalPos + offset, originalPos, elapsed / dur); elapsed += Time.deltaTime; yield return null; }
        target.localPosition = originalPos;
        isActing = false;
    }

    void UpdateUI()
    {
        for (int i = 0; i < toolIcons.Length; i++)
        {
            if (toolIcons[i]) toolIcons[i].color = (i == currentToolID) ? Color.white : new Color(1, 1, 1, 0.3f);
        }
    }
}