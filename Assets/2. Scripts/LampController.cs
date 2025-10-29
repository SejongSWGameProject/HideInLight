using UnityEngine;

public class LampController : MonoBehaviour
{
    public Light lamp;
    private bool isBroken = false;
    public AudioSource breakSound;       // �Ҹ� (����)
    public ParticleSystem sparkEffect;   // �Ҳ� (����)
    void Start()
    {
        if (lamp == null)
            lamp = GetComponentInChildren<Light>();
        LampManager.Instance.RegisterLamp(this);
    }
    void Update()
    {
        
    }

    public void BreakLamp()
    {
        Debug.Log("����");
        if (isBroken) return;
        StartCoroutine(BreakLightRoutine());
    }

    System.Collections.IEnumerator BreakLightRoutine()
    {
        isBroken = true;

        // ������ ȿ��
        for (int i = 0; i < 3; i++)
        {
            lamp.enabled = false;
            yield return new WaitForSeconds(0.1f);
            lamp.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        // ������ ����
        lamp.enabled = false;
        //if (sparkEffect != null) sparkEffect.Play();
        //if (breakSound != null) breakSound.Play();

    }
}
