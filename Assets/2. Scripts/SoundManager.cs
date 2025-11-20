using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Wire Puzzle")]
    public AudioClip wirePuzzleSuccess;
    public AudioClip wirePuzzleFail;
    public AudioClip wireDrag;

    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WirePuzzleSuccessSound()
    {
        audioSource.PlayOneShot(wirePuzzleSuccess);
    }
    public void WirePuzzleFailSound()
    {
        audioSource.PlayOneShot(wirePuzzleFail);
    }
    public void WireDragSound()
    {
        audioSource.PlayOneShot(wirePuzzleFail);

    }
    public void StartConnectingSound()
    {
        // 이미 재생 중이 아니라면 (중복 재생 방지)
        if (!audioSource.isPlaying)
        {
            audioSource.Play(); // Loop가 켜져 있으므로 멈출 때까지 계속 재생됨
        }
    }

    // [전선 잇기 종료]할 때 이 함수를 호출
    public void StopConnectingSound()
    {
        // 재생 중일 때만 멈춤
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
