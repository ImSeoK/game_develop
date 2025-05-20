using UnityEngine;
using TMPro;

public class NarrationTrigger : MonoBehaviour
{
    public string narrationText = "�̰��� ������ ������ �Ա���...";
    public float displayDuration = 4f;
    public AudioClip narrationAudio; //  ����� Ŭ�� �߰�

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
                //  �ؽ�Ʈ + ����� Ŭ�� ����
                narrationManager.ShowNarration(narrationText, narrationAudio, displayDuration);
            }
        }
    }
}