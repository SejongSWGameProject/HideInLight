using UnityEngine;

public class HeightSoundLoop : MonoBehaviour
{
    public RectTransform targetUI;       // A 오브젝트
    public AudioSource audioSource;      // AudioSource (Loop 재생 담당)
    public AudioClip loopSound;          // 반복 재생할 사운드

    private float initialHeight;
    private bool isConditionMet = false; // 현재 조건 상태

    void Start()
    {
        if (targetUI == null)
            targetUI = GetComponent<RectTransform>();

        initialHeight = targetUI.sizeDelta.y;

        // Loop는 꺼두고, Play On Awake도 비활성화
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = loopSound;
    }

    void Update()
    {
        float currentHeight = targetUI.sizeDelta.y;

        bool condition = currentHeight <= initialHeight * 0.5f;

        if (condition && !isConditionMet)
        {
            StartLoop();
        }
        else if (!condition && isConditionMet)
        {
            StopLoop();
        }
    }

    void StartLoop()
    {
        isConditionMet = true;

        if (!audioSource.isPlaying)
            audioSource.Play();   // Loop ON 이므로 자동 반복
    }

    void StopLoop()
    {
        isConditionMet = false;

        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
