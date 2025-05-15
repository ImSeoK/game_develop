using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    public Transform target;             // 나선형 맵의 중심을 지정
    public float rotationSpeed = 10f;    // 회전 속도 (자연스럽게 5~15 사이 추천)
    public float distance = 35f;         // 타겟과의 거리 (멀어질수록 전체 조망)
    public float height = 15f;           // 카메라 높이 (더 위에서 내려보게)

    void LateUpdate()
    {
        // 게임이 시작되면 이 카메라는 꺼지므로 더 이상 회전 X
        if (GameManager.Instance.isGameStarted || target == null) return;

        // 타임에 따라 회전 각도 계산
        float angle = rotationSpeed * Time.time;

        // y축 회전으로 회전 벡터 생성
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        // 회전 방향으로 거리만큼 떨어진 위치 계산
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // 최종 카메라 위치 설정 (높이는 위에서 내려보게끔)
        transform.position = target.position + offset + new Vector3(0, height, 0);

        // 카메라는 항상 중심을 바라보게
        transform.LookAt(target.position + Vector3.up * 5f); // 중심보다 살짝 위를 바라보도록
    }
}