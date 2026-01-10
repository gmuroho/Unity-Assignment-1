using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ToolSystem : MonoBehaviour
{
    [Header("model in hand")]
    public GameObject[] toolModels;

    [Header("model on the ground")]
    public GameObject[] groundObjects;

    [Header("particle effect (0:jiancao, 1:chushui, 2:tukuai)")]
    public ParticleSystem[] effects;

    [Header("shiquyinxiao duiying 012)")]
    public AudioClip[] pickUpSounds;

    [Header("using audio (tongshang)")]
    public AudioClip[] useSounds;

    [Header("yinpingbofangyuan (guazai playerorcamera shang)")]
    public AudioSource audioSource;

    [Header("ui icon")]
    public Image[] toolIcons;

    [Header("tishi ui setting")]
    public TextMeshProUGUI hintText;
    public string pickUpPrefix = "pick up ";
    public string[] toolNames = { "scissors", "kettle", "shovel" };

    [Header("jiancaopandingshezhi")]
    public float reachDistance = 3.5f;
    public string grassTag = "Grass";
    public LayerMask interactableLayers;

    private int currentToolID = -1;
    private bool isActing = false;

    void Start()
    {
        for (int i = 0; i < toolModels.Length; i++)
        {
            toolModels[i].SetActive(false);
        }
        for (int j = 0; j < groundObjects.Length; j++)
        {
            groundObjects[j].SetActive(true);
        }
        UpdateUI();
    }

    void Update()
    {
        if (hintText != null)
        {
            hintText.text = "";
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3.5f))
        {
            PickableItem item = hit.collider.GetComponent<PickableItem>();
            if (item != null)
            {
                int id = item.toolID;
                if (hintText != null)
                {
                    if (id < toolNames.Length)
                    {
                        hintText.text = pickUpPrefix + toolNames[id];
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckPickUp();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (currentToolID == -1)
            {
                CheckPickUp();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentToolID != -1)
            {
                DropTool();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToPickedTool(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToPickedTool(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToPickedTool(2);

        if (currentToolID != -1)
        {
            if (isActing == false)
            {
                HandleToolUsage();
            }
        }
    }

    void CheckPickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3.5f))
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
            if (i == id)
            {
                toolModels[i].SetActive(true);
            }
            else
            {
                toolModels[i].SetActive(false);
            }
        }

        if (audioSource != null)
        {
            if (id < pickUpSounds.Length)
            {
                if (pickUpSounds[id] != null)
                {
                    audioSource.PlayOneShot(pickUpSounds[id]);
                }
            }
        }

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
        if (id < groundObjects.Length)
        {
            if (groundObjects[id].activeSelf == false)
            {
                currentToolID = id;
                for (int i = 0; i < toolModels.Length; i++)
                {
                    if (i == id)
                    {
                        toolModels[i].SetActive(true);
                    }
                    else
                    {
                        toolModels[i].SetActive(false);
                    }
                }
                UpdateUI();
            }
        }
    }

    void HandleToolUsage()
    {
        if (currentToolID == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(ActionAnimation(toolModels[0].transform, Vector3.forward * 0.1f));
                PlayUseEffectAndSound(0);

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
                            Destroy(hit.collider.gameObject);
                            GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
                            if (manager != null) manager.OnGrassCut();
                        }
                    }
                }
            }
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

                if (effects.Length > 1)
                {
                    if (effects[1] != null && effects[1].isPlaying == false)
                    {
                        effects[1].Play();
                    }
                }
                if (audioSource != null && useSounds.Length > 1)
                {
                    if (useSounds[1] != null && audioSource.isPlaying == false)
                    {
                        audioSource.clip = useSounds[1];
                        audioSource.Play();
                    }
                }
            }
            else
            {
                toolModels[1].transform.localRotation = Quaternion.Slerp(toolModels[1].transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                if (effects.Length > 1 && effects[1] != null)
                {
                    if (effects[1].isPlaying == true) effects[1].Stop();
                }
                if (audioSource != null && useSounds.Length > 1)
                {
                    if (audioSource.clip == useSounds[1]) audioSource.Stop();
                }
            }
        }
        else if (currentToolID == 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(ActionAnimation(toolModels[2].transform, new Vector3(0, -0.2f, 0.2f)));
                PlayUseEffectAndSound(2);

                GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
                if (manager != null) manager.OnDigging();
            }
        }
    }

    void PlayUseEffectAndSound(int id)
    {
        if (id < effects.Length)
        {
            if (effects[id] != null) effects[id].Play();
        }
        if (audioSource != null)
        {
            if (id < useSounds.Length)
            {
                if (useSounds[id] != null) audioSource.PlayOneShot(useSounds[id]);
            }
        }
    }

    IEnumerator ActionAnimation(Transform target, Vector3 offset)
    {
        isActing = true;
        Vector3 originalPos = target.localPosition;
        float dur = 0.1f;
        float elapsed = 0;
        while (elapsed < dur)
        {
            target.localPosition = Vector3.Lerp(originalPos, originalPos + offset, elapsed / dur);
            elapsed = elapsed + Time.deltaTime;
            yield return null;
        }
        elapsed = 0;
        while (elapsed < dur)
        {
            target.localPosition = Vector3.Lerp(originalPos + offset, originalPos, elapsed / dur);
            elapsed = elapsed + Time.deltaTime;
            yield return null;
        }
        target.localPosition = originalPos;
        isActing = false;
    }

    void UpdateUI()
    {
        for (int i = 0; i < toolIcons.Length; i++)
        {
            if (toolIcons[i] != null)
            {
                if (i == currentToolID)
                {
                    toolIcons[i].color = new Color(1, 1, 1, 1);
                }
                else
                {
                    toolIcons[i].color = new Color(1, 1, 1, 0.3f);
                }
            }
        }
    }
}