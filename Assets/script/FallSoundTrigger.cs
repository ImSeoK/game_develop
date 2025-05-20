using UnityEngine;

public class FallSoundTrigger : MonoBehaviour
{
    public Transform player;
    public AudioSource audioSource;
    public AudioClip fallSound;
    public float fallDistanceThreshold = 10f;

    private float lastSoundY; // ���������� ���尡 ����� Y��

    void Start()
    {
        lastSoundY = player.position.y; // ���� �� ��ġ ����
    }

    void Update()
    {
        float currentY = player.position.y;

        // ���� ��ġ�� ������ ���� ��� ��ġ���� ���� �Ÿ���ŭ �� �Ʒ���
        if (currentY < lastSoundY - fallDistanceThreshold)
        {
            audioSource.PlayOneShot(fallSound);
            lastSoundY = currentY; // ���� ��ġ�� ���ο� ���������� ����
        }

        // �ö� ��� lastSoundY�� �ٽ� ���󰡰� (���� ����)
        if (currentY > lastSoundY)
        {
            lastSoundY = currentY;
        }
    }
}
