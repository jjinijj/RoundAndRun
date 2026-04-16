using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float maxJumpHeight = 2f; // 맥스 높이

    [SerializeField] private Transform groundCheck;   // 발 위치 빈 오브젝트
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private CapsuleCollider hitCollision;
    [SerializeField] private CameraController cameraController;

    public Animator animator;

    private CharacterController cc;
    private Vector3 velocity;
    private bool isSliding;
    public float groundY; // 착지 기준 Y
    private bool isJumping;
    private bool isDead;


    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.SetBool("IsRun", false);
        isDead = false;
    }

    void Update()
    {
        if(isDead)
            return;

        HandleJump();
        ApplyGravity();
    }

    // 맥스 높이 도달 감지 및 착지 후 애니메이션 리셋만 담당 (입력은 InputHandler에서)
    void HandleJump()
    {
        // 맥스 높이 도달하면 강제 낙하
        if (isJumping && transform.position.y >= groundY + maxJumpHeight)
        {
            isJumping = false;
            velocity.y = 0f;
        }

        if (animator.GetBool("IsJump") && IsGrounded() && velocity.y <= 0)
        {
            animator.SetBool("IsJump", false);
            if (isSliding)
                cameraController.SetSlide();
            else
                cameraController.SetRun();
        }
    }

    // --- InputHandler에서 호출하는 입력 이벤트 메서드 ---

    public void OnJump()
    {
        if (!IsGrounded()) return;

        animator.SetBool("IsJump", true);
        velocity.y = jumpForce;
        isJumping = true;
        groundY = transform.position.y;
        cameraController.SetJump();
    }

    public void OnJumpEnd()
    {
        if (!isJumping) return;

        // 키를 떼면 맥스 높이 전이라도 즉시 낙하
        isJumping = false;
        velocity.y = 0f;
    }

    public void OnSlideStart()
    {
        if (!IsGrounded() || isSliding) return;

        isSliding = true;
        hitCollision.height = 0.3f;
        hitCollision.center = new Vector3(0, 0.3f, 0);
        animator.SetBool("IsSlide", true);
        cameraController.SetSlide();
    }

    public void OnSlideEnd()
    {
        if (!isSliding) return;

        hitCollision.height = 1.5f;
        hitCollision.center = new Vector3(0, 0.75f, 0);
        animator.SetBool("IsSlide", false);
        isSliding = false;
        cameraController.SetRun();
    }

    void ApplyGravity()
    {
        if (!IsGrounded())
            velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0)
            velocity.y = -2f; // 땅에 붙이는 용도


        cc.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public void OnRun()
    {
        animator.SetBool("IsLoose", false);
        animator.SetBool("IsRun", true);
        isDead = false;
        
    }
    public void OnDead()
    {
        animator.SetBool("IsLoose", true);
        isDead = true;
    }
}

