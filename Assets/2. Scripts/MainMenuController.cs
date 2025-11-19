using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI; // UI 사용
using TMPro; // TextMeshPro Dropdown 사용 필수
using System.Collections.Generic; // 리스트 사용

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel; // 설정 창 패널
    public TMP_Dropdown resolutionDropdown; // [추가] 해상도 드롭다운 연결할 곳

    [Header("Audio")]
    public AudioMixer audioMixer;    // 오디오 믹서

    Resolution[] resolutions; // 해상도 목록 저장용 변수

    void Start()
    {
        // 게임 시작하자마자 해상도 목록을 불러와서 드롭다운에 채워넣습니다.
        InitResolutionDropdown();
    }

    // --- 해상도 초기화 함수 ---
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

    // --- 해상도 변경 함수 (드롭다운 연결용) ---
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // --- 기존 기능들 ---

    void Update()
    {
        // ESC 누르면 설정창 닫기
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel.activeSelf)
        {
            CloseOption();
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene("FirstStage");
    }

    public void OpenOption()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseOption()
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSens", sens);
        PlayerPrefs.Save();
        Debug.Log("마우스 감도 변경됨: " + sens);
    }
}