using UnityEngine;

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
    private bool isInMainMenu = false; // settings가 어디서 열렸는지 추적
    private bool hasStarted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        thirdPersonCamera = inGameCamera.GetComponent<ThirdPersonCamera>();
    }

    void Update()
    {
        // ESC 키로 인게임 중 세팅창 열기/닫기
        if (GameManager.Instance.isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsWindow();
        }
    }

    public void OnClickStart()
    {
        Debug.Log("Start 버튼 클릭됨");

        GameManager.Instance.isGameStarted = true;

        mainMenuUI.SetActive(false);
        mainMenuCamera.SetActive(false);
        inGameCamera.SetActive(true);
        player.SetActive(true);

        // getting up 애니메이션 실행 (여기로 이동)
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("gettingUpTrigger");
            animator.SetBool("hasMoved", false); // 방향키 누르기 전까지 idle로 안 넘어가게
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("게임 시작 상태 전환 완료");
    }

    public void OnClickSettings()
    {
        isInMainMenu = true; // 메인 메뉴에서 열림
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
        isInMainMenu = false; // 인게임 중 ESC로 연 것

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
