using Unity.Android.Gradle.Manifest;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    private const float GRAVITY_CONST = -9.81f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpHeight = 1.5f; 

    [Header("Look Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 1f;
    private float minPitch = -89f;
    private float maxPitch = 89f;
    private float cameraPitch = 0f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistance = .3f;
    [SerializeField] private LayerMask groundLayerMask;
    private bool isGrounded;

    private Vector3 velocity;
    private CharacterController characterController; 


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();   
    }


    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.DayInProgress) return;
        if (TabMenuUI.Instance != null && TabMenuUI.Instance.IsOpen) return;

        ApplyGravity(); 
        HandleLook(); 
        HandleMovement();

    }


    private void HandleLook()
    {
        Vector2 inputVector = GameInput.Instance.GetLookVectorDelta();
        float currentTargetYaw = inputVector.x * mouseSensitivity;

        transform.Rotate(Vector3.up * currentTargetYaw);

        cameraPitch -= inputVector.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDirec = (transform.forward * inputVector.y) + (transform.right * inputVector.x);
        
        characterController.Move(moveDirec * moveSpeed * Time.deltaTime); 
  
    }

    private void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckDistance, groundLayerMask); 

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(isGrounded && GameInput.Instance.JumpPressed())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY_CONST);
        }


        velocity.y += GRAVITY_CONST * Time.deltaTime; 
        characterController.Move(velocity * Time.deltaTime); 
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckPoint.position, groundCheckDistance); 
    }

}
