using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void ReloadScene()
    {
        Debug.Log("��ư���� Restart ȣ���");
        Time.timeScale = 1f; // Ȥ�� ���� ���¿��� �ε�Ǹ� �� �Ǵϱ� ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
