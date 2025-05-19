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

    private bool isJumped = false;
    private float jumpTimeCounter = 0f;
    private float maxJumpGraceTime = 1.5f;

    float jumpStartTime = -1f;
    float fallStartTime = -1f;

    private bool wasGrounded = true;
    private float fallTimer = 0f;
    private float fallTimeThreshold = 1f;

    public float flySpeedMultiplier = 2.5f;

    private bool hasTriggeredMove = false;

    public bool isClimbing = false;

    private bool isGameCleared = false;
    public bool isVictory = false;

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
    public AudioClip victoryClip;
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

        if (!hasTriggeredMove && (
        Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
    {
        hasTriggeredMove = true;
        animator.SetBool("hasMoved", true);
    }
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
        if (isVictory) return;

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

            if (IsGrounded())
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    AudioClip selectedClip = (Time.timeScale < 1f && slowRunSound != null) ? slowRunSound : runSound;
                    if (selectedClip != null)
                        audioSource.PlayOneShot(selectedClip);

                    footstepTimer = footstepInterval;
                }
            }
        }
        else
        {
            footstepTimer = 0f;
            targetMove = (currentState == State.Ice)
                ? Vector3.Lerp(targetMove, Vector3.zero, Time.deltaTime / iceFriction)
                : Vector3.zero;
        }

        bool grounded = IsGrounded();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Climbing"))
        {
            jumpStartTime = -1f;
            fallStartTime = -1f;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            return;
        }

        if (!isClimbing)
        {
            if (grounded)
            {
                jumpStartTime = -1f;
                fallStartTime = -1f;

                if (animator.GetBool("isJumping") || animator.GetBool("isFalling"))
                {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isFalling", false);
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    animator.SetBool("isJumping", true);
                    if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
                }

                if (velocity.y < 0)
                    velocity.y = -2f;
            }
            else
            {
                if (animator.GetBool("isJumping"))
                {
                    if (jumpStartTime < 0f)
                        jumpStartTime = Time.time;

                    if (Time.time - jumpStartTime >= 6f)
                    {
                        animator.SetBool("isFalling", true);
                        animator.SetBool("isJumping", false);
                        jumpStartTime = -1f;
                    }
                }
                else
                {
                    if (fallStartTime < 0f)
                        fallStartTime = Time.time;

                    if (Time.time - fallStartTime >= 2f)
                    {
                        animator.SetBool("isFalling", true);
                    }
                }

                if (velocity.y < -50f)
                    velocity.y = -50f;
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = targetMove * Time.deltaTime;
        finalMove.y = velocity.y * Time.deltaTime;
        controller.Move(finalMove);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trigger_Dark")
        {
            currentState = State.Dark;
            darkOverlay.SetActive(true);
            Debug.Log("Dark");
        }

        if (other.gameObject.tag == "Floor_Normal")
        {
            currentState = State.Normal;
        } 

        // 게임 클리어 트리거
        if (!isGameCleared && other.CompareTag("Finish")) // Finish 태그 사용
        {
            isGameCleared = true;
            TriggerVictory();
        }
    }

    private void OnTriggerExit(Collider other)
    {
            currentState = State.Normal;
            darkOverlay.SetActive(false);
            Debug.Log("DarkExit");
    }

    private void TriggerVictory()
    {
        animator.SetTrigger("Victory");

        // 카메라 전환 전에 기존 카메라 제어 끄기
        var cam = Camera.main;

        // ThirdPersonCamera 비활성화
        ThirdPersonCamera tpc = cam.GetComponent<ThirdPersonCamera>();
        if (tpc != null)
        {
            tpc.isCameraControlEnabled = false;
        }

        // CameraFollow 실행
        CameraFollow camFollow = cam.GetComponent<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.TriggerVictoryCamera();
        }

        PlayerMovement move = FindObjectOfType<PlayerMovement>();
        if (move != null)
        {
            move.isVictory = true;
        }

        // Victory 사운드 재생
        if (victoryClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(victoryClip);
        }

        Debug.Log("Victory!");
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
