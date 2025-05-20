using UnityEngine;
using TMPro;
using System.Collections;

public class NarrationManager : MonoBehaviour
{
    public TextMeshProUGUI narrationTextUI;
    public AudioSource audioSource;               // ����� �ҽ� �ʵ�
    public float fadeDuration = 1f;

    private Coroutine currentCoroutine;

    //  �����ε� �߰�: �ؽ�Ʈ�� ���� �� ���
    public void ShowNarration(string text, float displayDuration)
    {
        ShowNarration(text, null, displayDuration);
    }

    //  ��ü ���: �ؽ�Ʈ + ����� Ŭ��
    public void ShowNarration(string text, AudioClip clip, float displayDuration)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(NarrationSequence(text, clip, displayDuration));
    }

    private IEnumerator NarrationSequence(string text, AudioClip clip, float displayDuration)
    {
        // �ؽ�Ʈ ���� ��� (�ִ� ���)
        if (!string.IsNullOrWhiteSpace(text))
        {
            narrationTextUI.text = text;
            narrationTextUI.gameObject.SetActive(true);

            yield return StartCoroutine(FadeTextAlpha(0f, 1f, fadeDuration));
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadeTextAlpha(1f, 0f, fadeDuration));

            narrationTextUI.gameObject.SetActive(false);
        }

        // �ؽ�Ʈ ���� ���� ��� (�ִ� ���)
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }
    }

    private IEnumerator FadeTextAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = narrationTextUI.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(from, to, t);
            narrationTextUI.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        narrationTextUI.color = new Color(color.r, color.g, color.b, to);
    }
}
