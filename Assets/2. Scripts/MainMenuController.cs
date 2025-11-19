using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI; // Image(밝기 조절용) 사용
using TMPro; // Dropdown 사용
using System.Collections.Generic; // 리스트 사용

public class MainMenuController : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject settingsPanel;        // 설정 창 패널
    public TMP_Dropdown resolutionDropdown; // 해상도 드롭다운
    public Image brightnessOverlay;         // 밝기 조절용 검은 막(Panel)

    [Header("Audio")]
    public AudioMixer audioMixer;           // 오디오 믹서

    Resolution[] resolutions; // 해상도 목록 저장 변수

    void Start()
    {
        // 시작 시 해상도 목록 초기화
        InitResolutionDropdown();
    }

    void Update()
    {
        // ESC 누르면 설정창 닫기
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel.activeSelf)
        {
            CloseOption();
        }
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

    // --- [2] 기본 메뉴 기능 ---
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

    // --- [3] 사운드 조절 기능 (Master, BGM, SFX 분리) ---
    
    // 전체 볼륨
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    // 배경음악 (BGM) - [추가됨]
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMParam", Mathf.Log10(volume) * 20);
    }

    // 효과음 (SFX) - [추가됨]
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXParam", Mathf.Log10(volume) * 20);
    }

    // --- [4] 밝기 조절 기능 (검은 막 투명도) ---
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

    // --- [5] 마우스 감도 조절 ---
    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSens", sens);
        PlayerPrefs.Save();
        Debug.Log("마우스 감도 변경됨: " + sens);
    }
}