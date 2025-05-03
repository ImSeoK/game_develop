using UnityEngine;

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
        Cursor.lockState = CursorLockMode.Locked; // ���콺�� ȭ�� �߾ӿ� ����
        Cursor.visible = false;                  // ���콺 Ŀ�� ����
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -verticalRotationLimit, verticalRotationLimit);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position - rotation * Vector3.forward * distance + Vector3.up * offset.y;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // ��ü �������� ����
    }
}
