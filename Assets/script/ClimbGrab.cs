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
        // Ŭ���̹��� �ƴ� �� �Է� ó��
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

        // Ray ����׿�
        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, climbableMask))
        {
            // Ray�� ����� ��Ҵ��� �α� Ȯ��
            Debug.Log($"Hit {hit.collider.name} at {hit.point}");

            // ��ǥ ��ġ ���� (hit ��ġ + ������ ���� ����)
            targetPosition = new Vector3(hit.point.x, hit.point.y + climbOffsetHeight, hit.point.z);
            originalPosition = transform.position;

            isClimbing = true;
            climbTimer = 0f;

            // �̵�/���� ��Ȱ��ȭ
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

        // �̵� ��� �ٽ� Ȱ��ȭ
        if (TryGetComponent<PlayerMovement>(out var move))
        {
            move.enabled = true;
        }
    }
}
