using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject player;
    public GameObject mainMenuCamera;
    public GameObject inGameCamera;
    public GameObject settingsMenuUI;

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
        mainMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void OnClickBackFromSettings()
    {
        settingsMenuUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}