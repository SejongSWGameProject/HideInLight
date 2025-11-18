using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; 

public class GameMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;   // 1. 작은 메뉴 (설정, 나가기 버튼 있는 곳)
    public GameObject settingsPanel;    // 2. 큰 설정창 (볼륨 조절)
    public GameObject quitConfirmPanel; // 3. 종료 확인창 (진짜 나갈거냐?)

    [Header("Audio")]
    public AudioMixer audioMixer;

    private bool isPaused = false; 

    void Update()
    {
        // ESC 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 만약 설정창이나 종료확인창이 켜져 있다면 -> 뒤로가기(일시정지 메뉴로)
            if (settingsPanel.activeSelf || quitConfirmPanel.activeSelf)
            {
                BackToPauseMenu();
            }
            // 이미 일시정지 메뉴가 켜져 있다면 -> 게임 재개
            else if (isPaused)
            {
                ResumeGame();
            }
            // 게임 중이라면 -> 일시정지 메뉴 열기
            else
            {
                PauseGame();
            }
        }
    }

    // 1. 게임 일시정지 (ESC 누르면 가장 먼저 뜨는 창)
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // 작은 메뉴 켜기
        settingsPanel.SetActive(false); // 혹시 모르니 다른 건 끄기
        quitConfirmPanel.SetActive(false);

        Time.timeScale = 0f; // 시간 멈춤
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 2. 게임 재개 (메뉴 닫고 게임으로)
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);

        Time.timeScale = 1f; // 시간 다시 흐름
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 3. 설정 창 열기 (PauseMenu -> Settings)
    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false); // 작은 메뉴 숨기고
        settingsPanel.SetActive(true);   // 큰 설정창 보여줌
    }

    // 4. 종료 확인 창 열기 (PauseMenu -> QuitConfirm)
    public void OpenQuitConfirm()
    {
        pauseMenuPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    // 5. 뒤로 가기 (Settings/QuitConfirm -> PauseMenu)
    // X 버튼이나 '아니오' 버튼에 연결하세요
    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        pauseMenuPanel.SetActive(true); // 다시 작은 메뉴를 켜줌
    }

    // 6. 진짜 게임 종료 (Yes 버튼)
    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }

    // --- 기존 설정 기능 ---
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSens", sens);
        PlayerPrefs.Save();

        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null)
        {
            player.UpdateSensitivity(); 
        }
    }
}