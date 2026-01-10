using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public Transform toolMountPoint;
    public Vector3 holdOffset = new Vector3(0.4f, -0.4f, 0.7f);

    GameObject currentTool;
    Transform playerCamera;

    void Start()
    {
        if (toolMountPoint != null)
        {
            playerCamera = toolMountPoint;
        }
        else
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerCamera = cam.transform;
                toolMountPoint = playerCamera;
            }
        }
    }

    void Update()
    {
        if (currentTool != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UseEquippedTool();
            }
        }
    }

    void UseEquippedTool()
    {
        Vector3 startPos = playerCamera.position;
        Vector3 direction = playerCamera.forward;

        Debug.DrawRay(startPos, direction * 3f, Color.red, 1f);

        RaycastHit hit;
        bool isHit = Physics.Raycast(startPos, direction, out hit, 3f);

        if (isHit == true)
        {
            string name = hit.collider.name;
            Debug.Log("dazhongle: " + name);

            TrimmableObject trimmable = hit.collider.GetComponentInParent<TrimmableObject>();

            if (trimmable != null)
            {
                trimmable.Trim();
            }
            else
            {
                Debug.Log("meiyou jiaoben");
            }
        }
        else
        {
            Debug.Log("missed");
        }
    }

    public void PickupAndEquipTool(GameObject toolObject)
    {
        if (currentTool != null)
        {
            // already has tool
        }
        else
        {
            currentTool = toolObject;

            Rigidbody rb = currentTool.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.detectCollisions = false;
            }

            currentTool.transform.SetParent(toolMountPoint);
            currentTool.transform.localPosition = holdOffset;

            Quaternion rot = Quaternion.identity;
            currentTool.transform.localRotation = rot;

            int layerNum = LayerMask.NameToLayer("Ignore Raycast");
            currentTool.layer = layerNum;
        }
    }

    public void DropTool()
    {
        if (currentTool != null)
        {
            Rigidbody rb = currentTool.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.detectCollisions = true;
            }

            currentTool.transform.SetParent(null);

            int layerNum2 = LayerMask.NameToLayer("Interactable");
            currentTool.layer = layerNum2;

            currentTool = null;
        }
    }
}