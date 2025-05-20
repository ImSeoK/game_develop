using UnityEngine;
using TMPro;

public class NarrationTrigger : MonoBehaviour
{
    public string narrationText = "이곳은 오래된 신전의 입구다...";
    public float displayDuration = 4f;
    public AudioClip narrationAudio; //  오디오 클립 추가

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            NarrationManager narrationManager = FindObjectOfType<NarrationManager>();
            if (narrationManager != null)
            {
                //  텍스트 + 오디오 클립 전달
                narrationManager.ShowNarration(narrationText, narrationAudio, displayDuration);
            }
        }
    }
}