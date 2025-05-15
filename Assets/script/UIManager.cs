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
        Debug.Log(" Start ��ư Ŭ����");

        GameManager.Instance.isGameStarted = true;

        mainMenuUI.SetActive(false);
        mainMenuCamera.SetActive(false);
        inGameCamera.SetActive(true);
        player.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(" ���� ���� ���� ��ȯ �Ϸ�");
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