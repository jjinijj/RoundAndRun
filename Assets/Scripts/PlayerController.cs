using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float maxJumpHeight = 2f; // 맥스 높이

    [SerializeField] private Transform groundCheck;   // 발 위치 빈 오브젝트
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    public Animator animator;

    private CharacterController cc;
    private Vector3 velocity;
    private bool isSliding;
    private float groundY; // 착지 기준 Y
    private bool isJumping;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.SetBool("IsRun", true);
    }

    void Update()
    {
        HandleJump();
        HandleSlide();
        ApplyGravity();
    }


    void HandleJump()
    {
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsJump", true);
            velocity.y = jumpForce;
            isJumping = true;
            groundY = transform.position.y;
        }

        // 맥스 높이 도달하면 키 눌러도 강제 낙하
        if (isJumping && transform.position.y >= groundY + maxJumpHeight)
        {
            isJumping = false;
            velocity.y = 0f; // 상승 멈추고 중력에 맡김
        }

        // 키 떼면 상승 멈춤 (맥스 전에 떼도 바로 낙하)
        if (isJumping && Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            velocity.y = 0f;
        }

        if (animator.GetBool("IsJump") && IsGrounded() && velocity.y <= 0)
        {
            animator.SetBool("IsJump", false);
        }
    }   

    void HandleSlide()
    {
        if (IsGrounded() && !isSliding && Input.GetKeyDown(KeyCode.DownArrow))
        {
            isSliding = true;
            // 슬라이드 중 히트박스 줄이기 (선택)
            cc.height = 0.5f;
            cc.center = new Vector3(0,  0.5f, 0);
            animator.SetBool("IsSlide", true);
        }

        if(isSliding && Input.GetKeyUp(KeyCode.DownArrow))
        {
            cc.center = new Vector3(0,  0.75f, 0);
            cc.height = 1.5f;
            animator.SetBool("IsSlide", false);
            isSliding = false;
        }
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
}

