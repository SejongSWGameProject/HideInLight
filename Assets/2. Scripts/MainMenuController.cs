using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio; // 오디오 믹서 쓰려면 이거 필수!

public class MainMenuController : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 창 패널
    public AudioMixer audioMixer;    // 오디오 믹서

    // Update 함수는 매 프레임마다 실행됩니다.
    // 여기서 키 입력을 감시합니다.
    void Update()
    {
        // 만약 ESC 키가 눌렸고(GetKeyDown), 설정 패널이 현재 켜져 있다면(activeSelf)
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel.activeSelf)
        {
            CloseOption(); // 창 닫기 함수 실행
        }
    }

    // 1. 게임 시작
    public void GameStart()
    {
        SceneManager.LoadScene("FirstStage");
    }

    // 2. 환경설정 열기
    public void OpenOption()
    {
        settingsPanel.SetActive(true); // 패널 켜기
    }

    // 3. 환경설정 닫기
    public void CloseOption()
    {
        settingsPanel.SetActive(false); // 패널 끄기
    }

    // 4. 게임 종료
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }

    // 5. 볼륨 조절 (슬라이더랑 연결될 함수)
    public void SetVolume(float volume)
    {
        // 슬라이더 값(0~1)을 데시벨(-80~0)로 변환하는 공식
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    // 6. 마우스 감도 조절 (감도 슬라이더랑 연결될 함수)
    public void SetSensitivity(float sens)
    {
        // "MouseSens"라는 이름표를 붙여서 컴퓨터에 영구 저장합니다.
        // 게임을 껐다 켜도, 다른 씬으로 넘어가도 이 값은 유지됩니다.
        PlayerPrefs.SetFloat("MouseSens", sens);
        PlayerPrefs.Save(); 
        
        // 값이 잘 바뀌나 콘솔창에서 확인용 (나중에 지워도 됨)
        Debug.Log("마우스 감도 변경됨: " + sens);
    }
}