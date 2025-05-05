using UnityEngine;
using UnityEngine.EventSystems;

public class ClimbGrab : MonoBehaviour
{
    public float rayDistance = 2f;
    public float climbSpeed = 3f;
    public LayerMask climbableMask;
    public float climbOffsetHeight = 1.5f;
    public float climbTimeout = 1.5f;

    private bool isClimbing = false;
    private float climbTimer = 0f;
    private CharacterController controller;
    private Vector3 targetPosition;
    private Vector3 originalPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 클라이밍이 아닐 때 입력 처리
        if (!isClimbing)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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

        // Ray 디버그용
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, climbableMask))
        {
            // Ray가 제대로 닿았는지 로그 확인
            Debug.Log($"Hit {hit.collider.name} at {hit.point}");

            // 목표 위치 설정 (hit 위치 + 적당한 높이 보정)
            targetPosition = new Vector3(hit.point.x, hit.point.y + climbOffsetHeight, hit.point.z);
            originalPosition = transform.position;

            isClimbing = true;
            climbTimer = 0f;

            // 이동/점프 비활성화
            if (TryGetComponent<PlayerMovement>(out var move))
            {
                move.enabled = false;
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
        Debug.Log("Climb Finished");

        // 이동 기능 다시 활성화
        if (TryGetComponent<PlayerMovement>(out var move))
        {
            move.enabled = true;
        }
    }
}
