using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI; // Image(밝기 조절용) 사용
using TMPro; // Dropdown 사용
using System.Collections.Generic;
using Unity.VisualScripting; // 리스트 사용

public class GameMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;   // 일시정지 메뉴 (작은 창)
    public GameObject settingsPanel;    // 설정 창 (큰 창)
    public GameObject quitConfirmPanel; // 종료 확인 창

    // ▼▼▼ [추가됨] 숨기고 싶은 게임 내 UI들 ▼▼▼
    [Header("In-Game HUD (Hide on Pause)")]
    public GameObject canvasUI;    // 배터리 UI (CanvasBattery 등)
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    [Header("Settings UI")]
    public TMP_Dropdown resolutionDropdown; // 해상도 드롭다운
    public Image brightnessOverlay;         // 밝기 조절용 검은 막(Panel)

    [Header("Audio")]
    public AudioMixer audioMixer;       // 오디오 믹서

    [Header("Puzzle UI")]
    public GameObject wirePuzzleAnchor;
    public GameObject switchPuzzleAnchor;

    [Header("Player")]
    public PlayerLight playerLight;

    private bool isPaused = false;
    Resolution[] resolutions; 

    void Start()
    {
        // 게임 시작 시 해상도 목록 초기화
        InitResolutionDropdown();
    }

    // --- [1] 해상도 설정 기능 ---
    void InitResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // --- [2] 메뉴 네비게이션 (ESC, 닫기, 열기) ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(switchPuzzleAnchor.activeSelf || wirePuzzleAnchor.activeSelf)
            {
                return;
            }
            // 설정창이나 종료확인창이 켜져 있으면 -> 일시정지 메뉴로 뒤로가기
            else if (settingsPanel.activeSelf || quitConfirmPanel.activeSelf)
            {

                BackToPauseMenu();
            }
            // 이미 일시정지 상태면 -> 게임 재개
            else if (isPaused)
            {
                ResumeGame();
            }
            // 게임 중이면 -> 일시정지
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        if(playerLight != null)
        {
            playerLight.gameObject.SetActive(false);
        }

        // ▼▼▼ [추가] 일시정지 시 HUD 숨기기 ▼▼▼
        if (canvasUI != null) canvasUI.SetActive(false);
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        Time.timeScale = 0f; // 시간 멈춤
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        if(playerLight != null)
        {
            playerLight.gameObject.SetActive(true);

        }

        // ▼▼▼ [추가] 게임 재개 시 HUD 다시 보이기 ▼▼▼
        if (canvasUI != null) canvasUI.SetActive(true);
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        Time.timeScale = 1f; // 시간 다시 흐름
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

    // --- [3] 사운드 조절 (전체, BGM, SFX) ---

    // 전체 볼륨
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    // [추가됨] 배경음악
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMParam", Mathf.Log10(volume) * 20);
    }

    // [추가됨] 효과음
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXParam", Mathf.Log10(volume) * 20);
    }

    // --- [4] 밝기 조절 (검은 막 투명도) ---
    // [추가됨] 밝기 조절 함수
    public void SetBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            // 슬라이더(0~1): 1이면 밝음(투명), 0이면 어두움(검정)
            float alpha = 1.0f - value; 
             
            // 너무 깜깜해지지 않게 최대 0.9까지만 어두워지게 제한
            alpha = Mathf.Clamp(alpha, 0f, 0.9f); 

            Color c = brightnessOverlay.color;
            c.a = alpha;
            brightnessOverlay.color = c;
        }
    }

    // --- [5] 마우스 감도 조절 (즉시 적용) ---
    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSens", sens);
        PlayerPrefs.Save();
         
        // 게임 중이므로 플레이어에게 즉시 적용하라고 명령
        PlayerMove player = FindObjectOfType<PlayerMove>();
        if (player != null) player.UpdateSensitivity();
    }
}