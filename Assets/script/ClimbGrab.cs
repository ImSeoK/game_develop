using UnityEngine;
using UnityEngine.EventSystems;

public class ClimbGrab : MonoBehaviour
{
    public float rayDistance = 2f;
    public float climbSpeed = 3f;
    public LayerMask climbableMask;
    public float climbOffsetHeight = 1.5f;
    public float climbTimeout = 1.5f;
    public float climbCooldown = 1.0f;

    private bool isClimbing = false;
    private float climbTimer = 0f;
    private float cooldownTimer = 0f;
    private CharacterController controller;
    private Vector3 targetPosition;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>(); // Animator 가져오기
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isClimbing)
        {
            if (cooldownTimer <= 0f && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                TryClimb();
            }
        }
        else
        {
            Climb();
        }
    }

    void TryClimb()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, climbableMask))
        {
            Debug.Log("Hit " + hit.collider.name + " at " + hit.point);

            targetPosition = new Vector3(hit.point.x, hit.point.y + climbOffsetHeight, hit.point.z);

            isClimbing = true;
            climbTimer = 0f;

            if (TryGetComponent<PlayerMovement>(out var move))
            {
                move.enabled = false;
                move.isClimbing = true;
            }

            // 🔽 클라이밍 애니메이션 재생
            if (animator)
            {
                animator.SetTrigger("Climbing");
            }
        }
    }

    void Climb()
    {
        climbTimer += Time.deltaTime;

        Vector3 moveDirection = (targetPosition - transform.position);
        float distanceToTarget = moveDirection.magnitude;

        if (distanceToTarget > 0.05f && climbTimer < climbTimeout)
        {
            controller.Move(moveDirection.normalized * climbSpeed * Time.deltaTime);
        }
        else
        {
            FinishClimb();
        }
    }

    void FinishClimb()
    {
        isClimbing = false;
        cooldownTimer = climbCooldown;
        Debug.Log("Climb Finished");

        if (TryGetComponent<PlayerMovement>(out var move))
        {
            move.enabled = true;
            move.isClimbing = false;
        }

        // 🔽 클라이밍 종료 시 Idle 상태 등으로 전환되도록 비워두거나 필요 시 전환 애니메이션 설정
        if (animator)
        {
            animator.Play("Idle"); // Idle 상태가 없다면 제거 가능
        }
    }
}
