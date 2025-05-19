using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("Victory 연출용")]
    public Transform victoryCamTarget;      // 승리 시 바라볼 위치(빈 오브젝트)
    public float victoryMoveSpeed = 2f;     // Victory 카메라 이동 속도
    public float victoryRotateSpeed = 2f;   // Victory 회전 속도
    public bool instantVictorySnap = false;  // 즉시 위치로 이동할지 여부

    private bool isVictory = false;

    void LateUpdate()
    {
        if (isVictory)
        {
            if (instantVictorySnap)
            {
                transform.position = victoryCamTarget.position;
                transform.rotation = victoryCamTarget.rotation;
                isVictory = false; // 한 번만 수행
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, victoryCamTarget.position, Time.deltaTime * victoryMoveSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, victoryCamTarget.rotation, Time.deltaTime * victoryRotateSpeed);
            }
        }
    }

    // 외부에서 호출
    public void TriggerVictoryCamera()
    {
        isVictory = true;
    }
}