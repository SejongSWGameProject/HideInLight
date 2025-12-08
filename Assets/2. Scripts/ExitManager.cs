using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class ExitManager : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public bool isOpen = false;
    public AudioClip moveSound;
    private AudioSource audioSource;
    public float openDuration = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenExit();
        }
    }

    public void OpenExit()
    {
        StartCoroutine(OpenDoorsRoutine());
        isOpen = true;
        // 소리는 열리기 시작할 때 재생
        if (moveSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(moveSound);
        }

    }

    IEnumerator OpenDoorsRoutine()
    {
        float timer = 0f;

        // 1. 현재(닫힌) 위치 저장
        Vector3 leftStartPos = leftDoor.position;
        Vector3 rightStartPos = rightDoor.position;

        // 2. 목표(열린) 위치 계산
        // (사용하신 로직 그대로: x좌표에 +- 30)
        Vector3 leftEndPos = new Vector3(leftStartPos.x + 30, leftStartPos.y, leftStartPos.z);
        Vector3 rightEndPos = new Vector3(rightStartPos.x - 30, rightStartPos.y, rightStartPos.z);

        // 3. 시간 흐름에 따라 이동 (Lerp)
        while (timer < openDuration)
        {
            timer += Time.deltaTime;
            float t = timer / openDuration;

            // (선택사항) 더 부드러운 움직임을 원하면 아래 주석 해제 (Ease Out 효과)
            // t = Mathf.Sin(t * Mathf.PI * 0.5f);

            // A지점에서 B지점까지 t비율만큼 이동
            leftDoor.position = Vector3.Lerp(leftStartPos, leftEndPos, t);
            rightDoor.position = Vector3.Lerp(rightStartPos, rightEndPos, t);

            yield return null; // 한 프레임 대기
        }

        // 4. 루프가 끝나면 정확한 목표 위치로 고정 (오차 보정)
        leftDoor.position = leftEndPos;
        rightDoor.position = rightEndPos;
    }
}
