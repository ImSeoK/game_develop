using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float mouseSensitivity = 100f;
    public float distance = 4f;
    public float verticalRotationLimit = 80f;

    float yaw = 0f;
    float pitch = 0f;

    void Start()
    {
        if (GameManager.Instance.isGameStarted)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        if (!GameManager.Instance.isGameStarted || target == null) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -verticalRotationLimit, verticalRotationLimit);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position - rotation * Vector3.forward * distance + Vector3.up * offset.y;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // 상체 기준으로 보기
    }
}
