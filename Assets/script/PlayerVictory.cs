using UnityEngine;

public class PlayerVictory : MonoBehaviour
{
    public GameObject victoryTextUI; // VICTORY �ؽ�Ʈ ������Ʈ�� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            victoryTextUI.SetActive(true); // �ؽ�Ʈ�� ������
        }
    }
}