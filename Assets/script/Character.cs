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
        Fast,
        Fly,
        Dark
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

    public float flySpeedMultiplier = 2.5f;

    [Header("상태 이상")]
    public State currentState = State.Normal;
    public float slowAmount = 0.3f;
    public float fastAmount = 1.8f;
    public float iceFriction = 0.05f;
    public GameObject darkOverlay;

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
        darkOverlay.SetActive(false);

        gravity = Mathf.Abs(gravity) * -1f;
        velocity = Vector3.zero;

        animator.applyRootMotion = false;
    }

    void Update()
    {
        HandleModeToggle(); // 여기서 G 키 누르는지 계속 확인 중

        if (currentState == State.Fly)
            FlyMove();  // 비행 모드 이동 처리
        else
            Move();     // 일반 이동 처리
    }

    void HandleModeToggle()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentState != State.Fly)
                currentState = State.Fly;
            else
                currentState = State.Normal;
        }
    }

    void FlyMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float upward = 0f;
        if (Input.GetKey(KeyCode.Space)) upward = 1f;
        if (Input.GetKey(KeyCode.LeftControl)) upward = -1f;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        bool isMoving = direction.magnitude >= 0.1f || upward != 0f;

        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isJumping", false);

        Vector3 moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // 상승 및 하강 방향 포함
        moveDir += Vector3.up * upward;

        // 비행 속도 적용
        float speed = moveSpeed * flySpeedMultiplier;

        Vector3 finalMove = moveDir.normalized * speed * Time.deltaTime;

        velocity.y = 0f; // 중력 제거
        controller.Move(finalMove);
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

            // 발소리 재생 처리
            if (IsGrounded())
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    // 현재 타임스케일 기준으로 사운드 선택
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

        bool grounded = IsGrounded();

        // 1. 점프 입력 처리 (착지한 상태에서만 가능)
        if (grounded)
        {
            // 낙하/점프 상태 초기화
            if (animator.GetBool("isJumping") || animator.GetBool("isFalling"))
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
            }

            // 스페이스바 입력 시 점프
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("isJumping", true);

                if (jumpSound != null)
                    audioSource.PlayOneShot(jumpSound);
            }

            // 가벼운 착지 보정
            if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            // 공중에 있음 → 낙하 감지
            if (velocity.y < -0.1f && !animator.GetBool("isFalling"))
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false); // 점프 상태 해제
            }

            // 최대 낙하 속도 제한
            if (velocity.y < -50f)
                velocity.y = -50f;
        }

        // 2. 중력 적용
        velocity.y += gravity * Time.deltaTime;

        // 3. 실제 이동 처리
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trigger_Dark")
        {
            currentState = State.Dark;
            darkOverlay.SetActive(true);
            Debug.Log("Dark");
        }
    }

    private void OnTriggerExit(Collider other)
    {
            currentState = State.Normal;
            darkOverlay.SetActive(false);
            Debug.Log("DarkExit");
    }


}
