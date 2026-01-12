using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ToolSystem : MonoBehaviour
{
    public GameObject[] toolModels;
    public GameObject[] groundObjects;
    public ParticleSystem[] effects;
    public AudioClip[] pickUpSounds;
    public AudioClip[] useSounds;
    public AudioSource audioSource;
    public Image[] toolIcons;
    public TextMeshProUGUI hintText;
    public string pickUpPrefix = "pick up ";
    public string[] toolNames = { "scissors", "kettle", "shovel" };
    public float reachDistance = 3.5f;
    public string grassTag = "Grass";
    public LayerMask interactableLayers;

    private int currentToolID = -1;
    private bool isActing = false;

    void Start()
    {
        int i = 0;
        while (i < toolModels.Length)
        {
            toolModels[i].SetActive(false);
            i++;
        }

        int j = 0;
        while (j < groundObjects.Length)
        {
            groundObjects[j].SetActive(true);
            j++;
        }

        int k = 0;
        while (k < toolIcons.Length)
        {
            if (toolIcons[k] != null)
            {
                if (k == currentToolID)
                {
                    toolIcons[k].color = new Color(1, 1, 1, 1f);
                }
                else
                {
                    toolIcons[k].color = new Color(1, 1, 1, 0.3f);
                }
            }
            k++;
        }
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
            PickableItem item = hit.collider.gameObject.GetComponent<PickableItem>();
            if (item != null)
            {
                if (hintText != null)
                {
                    int id = item.toolID;
                    hintText.text = pickUpPrefix + toolNames[id];
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) || (Input.GetMouseButtonDown(0) && currentToolID == -1))
        {
            if (Physics.Raycast(ray, out hit, 3.5f))
            {
                int m = 0;
                while (m < groundObjects.Length)
                {
                    if (hit.collider.gameObject == groundObjects[m])
                    {
                        groundObjects[m].SetActive(false);
                        currentToolID = m;

                        int n = 0;
                        while (n < toolModels.Length)
                        {
                            if (n == m) toolModels[n].SetActive(true);
                            else toolModels[n].SetActive(false);
                            n++;
                        }

                        if (audioSource != null && m < pickUpSounds.Length)
                        {
                            if (pickUpSounds[m] != null)
                            {
                                audioSource.PlayOneShot(pickUpSounds[m]);
                            }
                        }

                        int u = 0;
                        while (u < toolIcons.Length)
                        {
                            if (toolIcons[u] != null)
                            {
                                if (u == currentToolID) toolIcons[u].color = new Color(1, 1, 1, 1f);
                                else toolIcons[u].color = new Color(1, 1, 1, 0.3f);
                            }
                            u++;
                        }
                        break;
                    }
                    m++;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentToolID != -1)
        {
            toolModels[currentToolID].SetActive(false);
            groundObjects[currentToolID].SetActive(true);
            groundObjects[currentToolID].transform.position = transform.position + transform.forward * 1.5f;
            currentToolID = -1;

            int u = 0;
            while (u < toolIcons.Length)
            {
                if (toolIcons[u] != null)
                {
                    if (u == currentToolID) toolIcons[u].color = new Color(1, 1, 1, 1f);
                    else toolIcons[u].color = new Color(1, 1, 1, 0.3f);
                }
                u++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTool(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchTool(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchTool(2);

        if (currentToolID != -1 && isActing == false)
        {
            if (currentToolID == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(ActionAnimation(toolModels[0].transform, Vector3.forward * 0.1f));
                    if (effects[0] != null) effects[0].Play();
                    if (audioSource != null && useSounds[0] != null) audioSource.PlayOneShot(useSounds[0]);

                    if (Physics.Raycast(ray, out hit, reachDistance, interactableLayers))
                    {
                        if (hit.collider.tag == grassTag)
                        {
                            GrassProperty grass = hit.collider.gameObject.GetComponent<GrassProperty>();
                            if (grass != null) grass.Cut();
                            else Destroy(hit.collider.gameObject);

                            GameProgressManager mgr = Object.FindFirstObjectByType<GameProgressManager>();
                            if (mgr != null) mgr.OnGrassCut();
                        }
                    }
                }
            }
            else if (currentToolID == 1)
            {
                if (Input.GetMouseButton(0))
                {
                    toolModels[1].transform.localRotation = Quaternion.Slerp(toolModels[1].transform.localRotation, Quaternion.Euler(40, 0, 0), Time.deltaTime * 5f);
                    if (Physics.Raycast(ray, out hit, reachDistance))
                    {
                        SoilGrowthFeedback soil = hit.collider.gameObject.GetComponent<SoilGrowthFeedback>();
                        if (soil != null) soil.ReceiveWater(Time.deltaTime * 0.5f);
                    }
                    if (effects[1] != null && effects[1].isPlaying == false) effects[1].Play();
                    if (audioSource != null && useSounds[1] != null && audioSource.isPlaying == false)
                    {
                        audioSource.clip = useSounds[1];
                        audioSource.Play();
                    }
                }
                else
                {
                    toolModels[1].transform.localRotation = Quaternion.Slerp(toolModels[1].transform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                    if (effects[1] != null && effects[1].isPlaying == true) effects[1].Stop();
                    if (audioSource != null && audioSource.clip == useSounds[1]) audioSource.Stop();
                }
            }
            else if (currentToolID == 2)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(ActionAnimation(toolModels[2].transform, new Vector3(0, -0.2f, 0.2f)));
                    if (effects[2] != null) effects[2].Play();
                    if (audioSource != null && useSounds[2] != null) audioSource.PlayOneShot(useSounds[2]);
                    GameProgressManager mgr = Object.FindFirstObjectByType<GameProgressManager>();
                    if (mgr != null) mgr.OnDigging();
                }
            }
        }
    }

    void SwitchTool(int id)
    {
        if (id < groundObjects.Length)
        {
            if (groundObjects[id].activeSelf == false)
            {
                currentToolID = id;
                int i = 0;
                while (i < toolModels.Length)
                {
                    if (i == id) toolModels[i].SetActive(true);
                    else toolModels[i].SetActive(false);
                    i++;
                }
                int u = 0;
                while (u < toolIcons.Length)
                {
                    if (toolIcons[u] != null)
                    {
                        if (u == currentToolID) toolIcons[u].color = new Color(1, 1, 1, 1f);
                        else toolIcons[u].color = new Color(1, 1, 1, 0.3f);
                    }
                    u++;
                }
            }
        }
    }

    IEnumerator ActionAnimation(Transform target, Vector3 offset)
    {
        isActing = true;
        Vector3 startPos = target.localPosition;
        Vector3 endPos = startPos + offset;
        float t = 0;
        while (t < 0.1f)
        {
            target.localPosition = Vector3.Lerp(startPos, endPos, t / 0.1f);
            t = t + Time.deltaTime;
            yield return null;
        }
        t = 0;
        while (t < 0.1f)
        {
            target.localPosition = Vector3.Lerp(endPos, startPos, t / 0.1f);
            t = t + Time.deltaTime;
            yield return null;
        }
        target.localPosition = startPos;
        isActing = false;
    }
}