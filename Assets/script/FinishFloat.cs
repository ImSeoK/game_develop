using UnityEngine;

public class FinishFloat : MonoBehaviour
{
    public float floatSpeed = 0.3f;      // 천천히 흔들리도록
    public float floatAmount = 0.25f;    // 적당한 범위

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime * floatSpeed;

        float targetY = startPos.y + Mathf.Sin(timer) * floatAmount;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

        // Lerp 속도 조금 더 부드럽게 조절
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
    }
}
