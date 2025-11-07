using UnityEngine;

public class LampController : MonoBehaviour
{
    public Light lamp;
    public bool isTurnedOn = false;
    public bool isBroken = false;
    public AudioSource breakSound;       // ¼Ò¸® (¼±ÅÃ)
    public ParticleSystem sparkEffect;   // ºÒ²É (¼±ÅÃ)
    void Start()
    {
        if (lamp == null)
            lamp = GetComponentInChildren<Light>();
        lamp.enabled = isTurnedOn;
        LampManager.Instance.RegisterLamp(this);
    }
    void Update()
    {
        
    }

    public void BreakLamp()
    {
        //Debug.Log("²ö´Ù");
        if (isBroken) return;
        StartCoroutine(BreakLightRoutine());
    }

    public void TurnOn()
    {
        if (isBroken) return;
        lamp.enabled = true;
    }

    public void TurnOff()
    {
        if (isBroken) return;
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

        // ±ô¹ÚÀÓ È¿°ú
        for (int i = 0; i < 3; i++)
        {
            lamp.enabled = false;
            yield return new WaitForSeconds(0.1f);
            lamp.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        // ¿ÏÀüÈ÷ ²¨Áü
        lamp.enabled = false;
        //if (sparkEffect != null) sparkEffect.Play();
        //if (breakSound != null) breakSound.Play();

    }
}
