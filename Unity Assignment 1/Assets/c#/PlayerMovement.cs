using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movement Settings")]
    public float moveSpeed = 7f;         
    public float lookSensitivity = 2f;   

    [Header("Jump Settings")]
    public float jumpForce = 5f;         
    public float groundCheckDistance = 0.2f; 

    
    private Rigidbody rb;
    private Camera playerCamera;

    
    private Vector3 movementInput;

    
    private float rotationX = 0f;

    

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the player!");
            return;
        }

        
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("Camera component not found in children!");
        }

        
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
        

        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        
        movementInput = new Vector3(x, 0f, z).normalized;

        

        
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        
        transform.Rotate(Vector3.up * mouseX);

        
        rotationX -= mouseY;
        
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        if (playerCamera != null)
        {
            
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    
    void FixedUpdate()
    {
        
        Vector3 worldMoveDir = transform.TransformDirection(movementInput);

        
        Vector3 targetVelocity = worldMoveDir * moveSpeed;

        
        Vector3 velocityChange = targetVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        
    }

    
    bool IsGrounded()
    {
        
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}
