using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("Victory �����")]
    public Transform victoryCamTarget;      // �¸� �� �ٶ� ��ġ(�� ������Ʈ)
    public float victoryMoveSpeed = 2f;     // Victory ī�޶� �̵� �ӵ�
    public float victoryRotateSpeed = 2f;   // Victory ȸ�� �ӵ�
    public bool instantVictorySnap = false;  // ��� ��ġ�� �̵����� ����

    private bool isVictory = false;

    void LateUpdate()
    {
        if (isVictory)
        {
            if (instantVictorySnap)
            {
                transform.position = victoryCamTarget.position;
                transform.rotation = victoryCamTarget.rotation;
                isVictory = false; // �� ���� ����
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, victoryCamTarget.position, Time.deltaTime * victoryMoveSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, victoryCamTarget.rotation, Time.deltaTime * victoryRotateSpeed);
            }
        }
    }

    // �ܺο��� ȣ��
    public void TriggerVictoryCamera()
    {
        isVictory = true;
    }
}