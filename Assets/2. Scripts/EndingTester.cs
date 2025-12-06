using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingTester : MonoBehaviour
{
    void Update()
    {
        // 숫자 1키 : 노말 엔딩 테스트
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestEnding("Normal");
        }
        // 숫자 2키 : 해피 엔딩 테스트
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestEnding("Happy");
        }
        // 숫자 3키 : 배드 엔딩 테스트
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestEnding("Bad");
        }
    }

    void TestEnding(string endingType)
    {
        Debug.Log($"[테스트 모드] {endingType} 엔딩으로 설정하고 재시작합니다.");

        // 1. 엔딩 데이터 강제 저장
        PlayerPrefs.SetString("EndingResult", endingType);
        PlayerPrefs.Save(); // 저장 확정

        // 2. 현재 씬 재시작 (새로고침)
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}