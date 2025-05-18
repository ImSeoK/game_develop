using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void ReloadScene()
    {
        Debug.Log("버튼으로 Restart 호출됨");
        Time.timeScale = 1f; // 혹시 정지 상태에서 로드되면 안 되니까 복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
