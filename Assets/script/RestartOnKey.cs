using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
                // 현재 활성화된 씬 이름 또는 인덱스를 통해 다시 로드
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
