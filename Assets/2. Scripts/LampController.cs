using UnityEngine;

public class LampController : MonoBehaviour
{
    public Light lamp;
    public bool isTurnedOn = false;
    public bool isBroken = false;
    public AudioSource breakSound;       // 소리 (선택)
    public ParticleSystem sparkEffect;   // 불꽃 (선택)
    void Start()
    {
        if (lamp == null)
            lamp = GetComponentInChildren<Light>();
        if (lamp.enabled)
        {
            LampManager.Instance.RegisterLamp(this);
        }
    }
    void Update()
    {
        
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
    }

    public void TurnOff()
    {
        if (isBroken) return;
        //Debug.Log(this.name + "끄다");
        lamp.enabled = false;

    }

    public void Toggle()
    {
        if (isBroken) return;
        lamp.enabled = !lamp.enabled;

    }

    System.Collections.IEnumerator BreakLightRoutine()
    {
        isBroken = true;

        // 깜박임 효과
        for (int i = 0; i < 3; i++)
        {
            lamp.enabled = false;
            yield return new WaitForSeconds(0.1f);
            lamp.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        // 완전히 꺼짐
        lamp.enabled = false;
        //if (sparkEffect != null) sparkEffect.Play();
        //if (breakSound != null) breakSound.Play();

    }
}
