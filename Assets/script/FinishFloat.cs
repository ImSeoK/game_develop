using UnityEngine;

public class FinishFloat : MonoBehaviour
{
    public float floatSpeed = 0.3f;      // õõ�� ��鸮����
    public float floatAmount = 0.25f;    // ������ ����

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

        // Lerp �ӵ� ���� �� �ε巴�� ����
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
    }
}
