using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(CharacterController))]
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

    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 targetMove;

    [Header("상태 이상")]
    public State currentState = State.Normal;
    public float slowAmount = 0.3f;
    public float fastAmount = 1.8f;
    public float iceFriction = 0.05f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Gravity가 항상 음수로 보장되도록
        gravity = Mathf.Abs(gravity) * -1f;

        // 초기 낙하 속도 안전하게 설정
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
        {
            direction = -direction;
        }

        if (direction.magnitude >= 0.1f)
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
        }
        else
        {
            if (currentState == State.Ice)
                targetMove = Vector3.Lerp(targetMove, Vector3.zero, Time.deltaTime / iceFriction);
            else
                targetMove = Vector3.zero;
        }

        // 점프 및 중력 처리
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            // 낙하 속도 제한 (안전 장치)
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
