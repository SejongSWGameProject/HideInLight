using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI; // Dropdown 쓰려면 필요
using TMPro; // TextMeshPro Dropdown 쓰려면 필수!
using System.Collections.Generic; // 리스트 쓰려면 필요

public class GameMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject quitConfirmPanel;

    [Header("Settings UI")]
    public TMP_Dropdown resolutionDropdown; // 해상도 드롭다운 연결할 곳

    [Header("Audio")]
    public AudioMixer audioMixer;

    private bool isPaused = false;
    Resolution[] resolutions; // 가능한 해상도 목록 저장

    void Start()
    {
        // --- 해상도 목록 초기화 (게임 켜자마자 실행) ---
        InitResolutionDropdown();
    }

    void InitResolutionDropdown()
    {
        // 1. 모니터가 지원하는 모든 해상도 가져오기
        resolutions = Screen.resolutions;

        // 2. 드롭다운 비우기
        resolutionDropdown.ClearOptions();

        // 3. 해상도들을 문자열("1920 x 1080")로 바꿔서 리스트에 담기
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // 현재 내 화면 해상도와 똑같은지 확인 (똑같다면 그 번호를 기억)
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        // 4. 드롭다운에 리스트 추가하고, 현재 해상도로 선택해두기
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // --- 해상도 변경 함수 (드롭다운이랑 연결) ---
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        
        // 선택된 해상도로 변경 (마지막 true는 전체화면 모드)
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // ... (아래는 기존 Update 및 메뉴 관련 코드들 그대로) ...

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf || quitConfirmPanel.activeSelf)
                BackToPauseMenu();
            else if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenQuitConfirm()
    {
        pauseMenuPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSens", sens);
        PlayerPrefs.Save();
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null) player.UpdateSensitivity();
    }
}