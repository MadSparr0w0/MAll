using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    public float rotationSpeed = 10f;
    public float dragSlowdown = 0.5f;

    private CharacterController controller;
    private Animator anim;
    private Vector3 moveDirection = Vector3.zero;
    private Transform cameraTransform;
    private bool isDragging;
    private float originalSpeed;

    private float previousY; 
    private bool isJumping;  

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        cameraTransform = Camera.main.transform;
        originalSpeed = moveSpeed;
        previousY = 0f;
        isJumping = false;
    }

    void Update()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputDirection = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")).normalized;

        moveSpeed = isDragging ? originalSpeed * dragSlowdown : originalSpeed;

        if (inputDirection.magnitude > 0.1f)
        {
            anim.SetInteger("AnimationPar", 1);

            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            moveDirection.x = inputDirection.x * moveSpeed;
            moveDirection.z = inputDirection.z * moveSpeed;
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
            moveDirection.x = 0;
            moveDirection.z = 0;
        }

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isDragging)
            {
                moveDirection.y = jumpForce;
                anim.Play("Jump_start"); 
                isJumping = true;
            }
            else if (isJumping)
            {
 
                anim.Play("Jump_end");
                isJumping = false;
            }
            else
            {
                moveDirection.y = 0f;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;

            if (Mathf.Abs(moveDirection.y - previousY) > 0.5f && !Input.GetKey(KeyCode.Space))
            {
                anim.Play("Jump_loop"); 
            }
        }

        previousY = moveDirection.y;

        controller.Move(moveDirection * Time.deltaTime);
    }


    public Vector3 GetCurrentVelocity()
    {
        return moveDirection;
    }

    public void SetDragging(bool state)
    {
        isDragging = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("Player") && other.CompareTag("Finish"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
