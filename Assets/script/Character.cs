using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        Normal, // 기본
        Slow,   // 슬로우
        Revert, // 방향키 반전
        Ice     // 빙판
    }

    public float moveSpeed;
    public float gravity;
    public float jumpHeight;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 targetMove;

    [Space(10)]
    [Header("상태 이상")]
    public State currentState = State.Normal;
    public float slowAmount = 0.3f;
    public float iceFriction = 0.05f;

    void Start()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; // 전체화면
        Screen.fullScreen = true;

        controller = GetComponent<CharacterController>();
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
            direction.x = -horizontal;
            direction.z = -vertical;
        }


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            targetMove = moveDir.normalized * moveSpeed;

            if (currentState == State.Slow)
                targetMove *= slowAmount;
        }
        else
        {
            if (currentState == State.Ice)
                targetMove = Vector3.Lerp(targetMove,Vector3.zero,Time.deltaTime / iceFriction);
            else
                targetMove = Vector3.zero;
        }

        // Jump
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // 최종 이동 벡터 = 이동 + 중력
        Vector3 finalMove = targetMove * Time.deltaTime;
        finalMove.y = velocity.y * Time.deltaTime;
        controller.Move(finalMove);
    }

    private void Slip()
    {

    }

    // 상태 이상
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Floor_Slow")
        {
            Debug.Log("State : Slow");
            currentState = State.Slow;
        }
        else if (hit.gameObject.tag == "Floor_Revert")
        {
            Debug.Log("State : Revert");
            currentState = State.Revert;
        }
        else if (hit.gameObject.tag == "Floor_Ice")
        {
            Debug.Log("State : Ice");
            currentState = State.Ice;
        }
        else if (hit.gameObject.tag == "Floor_Normal")
        {
            Debug.Log("State : Normal");
            currentState = State.Normal;
        }
    }
}
