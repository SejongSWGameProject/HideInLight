using UnityEngine;

public class LampController : MonoBehaviour
{
    public Light lamp;
    public Light pointLight;
    public bool isTurnedOn = false;
    public bool isBroken = false;
    public AudioClip breakSound;       // 소리 (선택)
    private AudioSource audioSource;
    public ParticleSystem sparkEffect;   // 불꽃 (선택)
    public BoxCollider darkCollider;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (lamp == null)
            lamp = GetComponentInChildren<Light>();
        if(pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }
        if (lamp.enabled)
        {
            LampManager.Instance.RegisterLamp(this);
        }
        if(darkCollider == null)
        {
            darkCollider = GetComponentInChildren<BoxCollider>();
        }
        LampManager.Instance.RegisterLamp(this, true);
    }
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();
            player.CheckDarkness(lamp.enabled);
            Debug.Log("IsInDark:" + player.getIsInDarkness());

            //isInDark == true일 때 가장 가까운 4개의 전등을 봄. 하나라도 켜져 있으면 not.
        }
    }

    public void BreakLamp()
    {
        //Debug.Log("끈다");
        if (isBroken) return;
        StartCoroutine(BreakLightRoutine());
    }

    public void TurnOn()
    {
        if (isBroken) return;
        //Debug.Log(this.name+"켜다");
        lamp.enabled = true;
        pointLight.enabled = true;
    }

    public void TurnOff()
    {
        if (isBroken) return;
        //Debug.Log(this.name + "끄다");
        lamp.enabled = false;
        pointLight.enabled = false;
    }

    public void Toggle()
    {
        if (isBroken) return;
        lamp.enabled = !lamp.enabled;
        pointLight.enabled = !pointLight.enabled;
    }

    System.Collections.IEnumerator BreakLightRoutine()
    {
        isBroken = true;

        yield return new WaitForSeconds(1f);
        // 깜박임 효과
        //for (int i = 0; i < 3; i++)
        //{
        //    lamp.enabled = false;
        //    pointLight.enabled = false;
        //    yield return new WaitForSeconds(0.1f);
        //    lamp.enabled = true;
        //    pointLight.enabled = true;
        //}

        // 완전히 꺼짐
        lamp.enabled = false;
        pointLight.enabled = false;
        //if (sparkEffect != null) sparkEffect.Play();
        if (breakSound != null) audioSource.PlayOneShot(breakSound);

    }
}
