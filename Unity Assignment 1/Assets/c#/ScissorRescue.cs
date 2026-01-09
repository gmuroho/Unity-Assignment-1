using UnityEngine;

public class ScissorRescue : MonoBehaviour
{
    [Header("core")]
    
    public Transform leftPivot;
    public Transform rightPivot;

    [Header("audio")]
    public AudioSource cutSound;
    public AudioSource openSound;

    [Header("设置")]
    public float openAngle = 30f;
    public float animationSpeed = 12f;
    public float interactDistance = 3f;

    [Header("视角")]
    // X=0 居中, Y=-0.5 略微靠下, Z=1.2 保证能看到整个大剪刀
    public Vector3 heldPosition = new Vector3(0f, -0.5f, 1.2f);
    public Vector3 heldRotation = new Vector3(0f, 0f, 0f);

    private bool isPickedUp = false;
    private bool isCutting = false;
    private float currentAngle = 30f;
    private float targetAngle = 30f;

    void Start()
    {
        currentAngle = openAngle;
        targetAngle = openAngle;
    }

    void Update()
    {
        if (!isPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                TryPickUp();
            }
            return;
        }

        
        if (Input.GetMouseButtonDown(0) && !isCutting)
        {
            StartCut();
        }

        
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * animationSpeed);

        
        if (leftPivot) leftPivot.localRotation = Quaternion.Euler(0, currentAngle, 0);
        if (rightPivot) rightPivot.localRotation = Quaternion.Euler(0, -currentAngle, 0);

        
        if (isCutting && currentAngle < 1f)
        {
            targetAngle = openAngle;
            if (openSound) openSound.Play();
            isCutting = false;
        }
    }

    void TryPickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
            {
                PickUp();
            }
        }
    }

    void PickUp()
    {
        isPickedUp = true;

        
        transform.SetParent(Camera.main.transform);

        
        transform.localPosition = heldPosition;
        transform.localRotation = Quaternion.Euler(heldRotation);

        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Debug.Log("Scissors are ready.");
    }

    void StartCut()
    {
        isCutting = true;
        targetAngle = 0f;
        if (cutSound) cutSound.Play();
    }
}