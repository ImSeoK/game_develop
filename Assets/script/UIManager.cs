using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject player;
    public GameObject mainMenuCamera;
    public GameObject inGameCamera;
    public GameObject settingsWindow;

    public void OnClickStart()
    {
        Debug.Log(" Start 버튼 클릭됨");

        GameManager.Instance.isGameStarted = true;

        mainMenuUI.SetActive(false);
        mainMenuCamera.SetActive(false);
        inGameCamera.SetActive(true);
        player.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(" 게임 시작 상태 전환 완료");
    }

    public void OnClickSettings()
    {
        mainMenuUI.SetActive(false); // 버튼들만 감추고
        settingsWindow.SetActive(true);
    }

    public void OnClickBackFromSettings()
    {
        settingsWindow.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}