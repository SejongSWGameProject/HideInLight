using UnityEngine;

public class MonsterIncapsule : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip capsuleBreakClip;
    private AudioSource audioSource;
    public float threshold = 0.1f;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) return;

        if (capsuleBreakClip != null)
        {
            AudioSource.PlayClipAtPoint(capsuleBreakClip, transform.position, threshold);
        }
    }
}
