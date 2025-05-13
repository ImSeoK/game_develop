using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))] // AudioSource 필수
public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        Normal,
        Slow,
        Revert,
        Ice,
        Fast
    }

    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public Transform cameraTransform;
    public LayerMask groundLayers;

    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 targetMove;
    private Animator animator;

    [Header("상태 이상")]
    public State currentState = State.Normal;
    public float slowAmount = 0.3f;
    public float fastAmount = 1.8f;
    public float iceFriction = 0.05f;

    [Header("사운드")]
    public AudioClip runSound;
    public AudioClip slowRunSound;
    public AudioClip jumpSound;
    private AudioSource audioSource;
    private float footstepTimer = 0f;
    public float footstepInterval = 0.4f;

    bool IsGrounded()
    {
        LayerMask groundLayers = LayerMask.GetMask("Grabbable");
        return Physics.Raycast(transform.position, Vector3.down, 0.2f, groundLayers);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gravity = Mathf.Abs(gravity) * -1f;
        velocity = Vector3.zero;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (currentState == State.Revert)
            direction = -direction;

        bool isMoving = direction.magnitude >= 0.1f;
        animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            targetMove = moveDir * moveSpeed;

            if (currentState == State.Slow)
                targetMove *= slowAmount;
            else if (currentState == State.Fast)
                targetMove *= fastAmount;

            // ✅ 발소리 재생 처리
            if (IsGrounded())
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    // ✅ 현재 타임스케일 기준으로 사운드 선택
                    AudioClip selectedClip = (Time.timeScale < 1f && slowRunSound != null) ? slowRunSound : runSound;

                    if (selectedClip != null)
                    {
                        audioSource.PlayOneShot(selectedClip);
                    }

                    footstepTimer = footstepInterval;
                }
            }
        }
        else
        {
            footstepTimer = 0f;

            if (currentState == State.Ice)
                targetMove = Vector3.Lerp(targetMove, Vector3.zero, Time.deltaTime / iceFriction);
            else
                targetMove = Vector3.zero;
        }

        // 점프 및 중력 처리
        if (IsGrounded())
        {
            if (animator.GetBool("isJumping"))
            {
                animator.SetBool("isJumping", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("isJumping", true);

                // ✅ 점프 사운드 재생
                if (jumpSound != null)
                {
                    audioSource.PlayOneShot(jumpSound);
                }
            }

            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            if (velocity.y < -50f)
                velocity.y = -50f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = targetMove * Time.deltaTime;
        finalMove.y = velocity.y * Time.deltaTime;
        controller.Move(finalMove);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Floor_Slow":
                currentState = State.Slow;
                break;
            case "Floor_Revert":
                currentState = State.Revert;
                break;
            case "Floor_Ice":
                currentState = State.Ice;
                break;
            case "Floor_Fast":
                currentState = State.Fast;
                break;
            case "Floor_Normal":
                currentState = State.Normal;
                break;
        }
    }
}
