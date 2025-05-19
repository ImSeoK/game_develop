using UnityEngine;
using UnityEngine.Playables;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject player;
    public GameObject mainMenuCamera;
    public GameObject inGameCamera;
    public GameObject settingsWindow;

    public static UIManager Instance;

    private ThirdPersonCamera thirdPersonCamera;
    private bool isSettingsOpen = false;
    private bool isInMainMenu = false; // settings�� ��� ���ȴ��� ����
    private bool hasStarted = false;

    [Header("Cutscene")]
    public GameObject cutsceneCamera;
    public PlayableDirector cutSceneDirector;
    public GameObject cutsceneCharacter;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        thirdPersonCamera = inGameCamera.GetComponent<ThirdPersonCamera>();
    }

    void Update()
    {
        // ESC Ű�� �ΰ��� �� ����â ����/�ݱ�
        if (GameManager.Instance.isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsWindow();
        }
    }

    public void OnClickStart()
    {
        Debug.Log("Start ��ư Ŭ����");

        GameManager.Instance.isGameStarted = true;

        mainMenuUI.SetActive(false);
        mainMenuCamera.SetActive(false);

        // �ƾ� ī�޶� �Ѱ�, ������ ���α�
        cutsceneCamera.SetActive(true);
        inGameCamera.SetActive(false);
        player.SetActive(false);

        // �ƾ� ���
        cutSceneDirector.stopped += OnCutsceneEnded;
        cutSceneDirector.Play();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("�ƾ� ��� ����");
    }

    private void OnCutsceneEnded(PlayableDirector director)
    {
        cutsceneCamera.SetActive(false);
        cutsceneCharacter.SetActive(false);

        inGameCamera.SetActive(true);
        player.SetActive(true);

        // gettingUp�� Entry �� gettingUp���� �ִϸ����Ϳ��� �ٷ� ���۵ǹǷ� Ʈ���� ���� ����
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("hasMoved", false); // idle ���� �������� ���
        }

        cutSceneDirector.stopped -= OnCutsceneEnded;

        Debug.Log("���� ���� ���� ��ȯ �Ϸ�");
    }

    public void OnClickSettings()
    {
        isInMainMenu = true; // ���� �޴����� ����
        mainMenuUI.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void ToggleSettingsWindow()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsWindow.SetActive(isSettingsOpen);

        Cursor.visible = isSettingsOpen;
        Cursor.lockState = isSettingsOpen ? CursorLockMode.None : CursorLockMode.Locked;

        Time.timeScale = isSettingsOpen ? 0f : 1f;
        isInMainMenu = false; // �ΰ��� �� ESC�� �� ��

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.isCameraControlEnabled = !isSettingsOpen;
        }
    }

    public void OnClickResume()
    {
        settingsWindow.SetActive(false);
        Time.timeScale = 1f;
        isSettingsOpen = false;

        if (isInMainMenu)
        {
            mainMenuUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.isCameraControlEnabled = true;
        }
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
