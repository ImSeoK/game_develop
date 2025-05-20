using UnityEngine;

public class FallSoundTrigger : MonoBehaviour
{
    public Transform player;
    public AudioSource audioSource;
    public AudioClip fallSound;
    public float fallDistanceThreshold = 10f;

    private float lastSoundY; // 마지막으로 사운드가 재생된 Y값

    void Start()
    {
        lastSoundY = player.position.y; // 시작 시 위치 기준
    }

    void Update()
    {
        float currentY = player.position.y;

        // 현재 위치가 마지막 사운드 재생 위치보다 일정 거리만큼 더 아래면
        if (currentY < lastSoundY - fallDistanceThreshold)
        {
            audioSource.PlayOneShot(fallSound);
            lastSoundY = currentY; // 현재 위치를 새로운 기준점으로 갱신
        }

        // 올라갈 경우 lastSoundY도 다시 따라가게 (선택 사항)
        if (currentY > lastSoundY)
        {
            lastSoundY = currentY;
        }
    }
}
