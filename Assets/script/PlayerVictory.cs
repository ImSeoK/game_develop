using UnityEngine;

public class PlayerVictory : MonoBehaviour
{
    public GameObject victoryTextUI; // VICTORY 텍스트 오브젝트만 연결

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            victoryTextUI.SetActive(true); // 텍스트만 보여줌
        }
    }
}