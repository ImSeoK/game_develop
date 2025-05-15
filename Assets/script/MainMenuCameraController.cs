using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public Transform target;             // ������ ���� �߽��� ����
    public float rotationSpeed = 10f;    // ȸ�� �ӵ� (�ڿ������� 5~15 ���� ��õ)
    public float distance = 35f;         // Ÿ�ٰ��� �Ÿ� (�־������� ��ü ����)
    public float height = 15f;           // ī�޶� ���� (�� ������ ��������)

    void LateUpdate()
    {
        // ������ ���۵Ǹ� �� ī�޶�� �����Ƿ� �� �̻� ȸ�� X
        if (GameManager.Instance.isGameStarted || target == null) return;

        // Ÿ�ӿ� ���� ȸ�� ���� ���
        float angle = rotationSpeed * Time.time;

        // y�� ȸ������ ȸ�� ���� ����
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        // ȸ�� �������� �Ÿ���ŭ ������ ��ġ ���
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // ���� ī�޶� ��ġ ���� (���̴� ������ �������Բ�)
        transform.position = target.position + offset + new Vector3(0, height, 0);

        // ī�޶�� �׻� �߽��� �ٶ󺸰�
        transform.LookAt(target.position + Vector3.up * 5f); // �߽ɺ��� ��¦ ���� �ٶ󺸵���
    }
}