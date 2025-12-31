using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // 引入 TMPro 命名空间

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

    // --- 修改点：使用 TextMeshProUGUI 替换 Text ---
    [Header("GRUNN 提示 UI 设置")]
    public TextMeshProUGUI hintText;  // 替换为 TextMeshProUGUI 组件
    public string pickUpPrefix = "拾取 "; // 提示前缀
    public string[] toolNames = { "剪刀", "水壶", "铲子" }; // 对应 ID 的名称显示
    // ----------------------------

    private int currentToolID = -1;
    private bool isActing = false;

    void Start()
    {
        // 初始：手部模型全关，地面模型全开
        foreach (GameObject go in toolModels) go.SetActive(false);
        foreach (GameObject go in groundObjects) go.SetActive(true);
        UpdateUI();
    }

    void Update()
    {
        // --- 每帧清理提示文字并检测指向 ---
        if (hintText != null) hintText.text = "";
        HandleHintRaycast();
        // ------------------------------------

        // 1. 拾取：射线检测地面上的工具
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            CheckPickUp();
        }

        // 2. 放下：手上消失，地面重新出现
        if (Input.GetKeyDown(KeyCode.Q) && currentToolID != -1)
        {
            DropTool();
        }

        // 3. 切换快捷键
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToPickedTool(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToPickedTool(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToPickedTool(2);

        // 4. 使用逻辑
        if (currentToolID != -1 && !isActing)
        {
            HandleToolUsage();
        }
    }

    // --- 负责“指向时显示文字”的逻辑 ---
    void HandleHintRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 3.5f))
        {
            // 尝试获取物体上的 PickableItem 脚本
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
            // 遍历 groundObjects 数组对比
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
        // 逻辑核心：地面消失，手上出现
        groundObjects[id].SetActive(false);
        currentToolID = id;

        for (int i = 0; i < toolModels.Length; i++)
        {
            toolModels[i].SetActive(i == id);
        }

        // 播放拾取音效
        if (audioSource && id < pickUpSounds.Length && pickUpSounds[id])
            audioSource.PlayOneShot(pickUpSounds[id]);

        UpdateUI();
    }

    void DropTool()
    {
        // 手上消失
        toolModels[currentToolID].SetActive(false);

        // 地面出现
        groundObjects[currentToolID].SetActive(true);
        groundObjects[currentToolID].transform.position = transform.position + transform.forward * 1.5f;

        currentToolID = -1;
        UpdateUI();
    }

    void SwitchToPickedTool(int id)
    {
        // 如果地面物体是隐藏的，说明已被拾取，可以切换
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
        // 剪刀(0)
        if (currentToolID == 0 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ActionAnimation(toolModels[0].transform, Vector3.forward * 0.1f));
            PlayUseEffectAndSound(0);
        }
        // 水壶(1) - 持续动作
        else if (currentToolID == 1)
        {
            if (Input.GetMouseButton(0))
            {
                toolModels[1].transform.localRotation = Quaternion.Slerp(toolModels[1].transform.localRotation, Quaternion.Euler(40, 0, 0), Time.deltaTime * 5f);
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
        // 铲子(2)
        else if (currentToolID == 2 && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ActionAnimation(toolModels[2].transform, new Vector3(0, -0.2f, 0.2f)));
            PlayUseEffectAndSound(2);
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