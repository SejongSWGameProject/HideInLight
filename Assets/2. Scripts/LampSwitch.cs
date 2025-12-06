using System.Collections.Generic; // List를 사용할 경우 필요 (지금은 배열 사용)
using UnityEngine;

public class LampSwitch : MonoBehaviour
{
    // [Tooltip("제어할 모든 전등 오브젝트를 여기에 연결하세요.")]
    [Header("연결할 전등 (여러 개)")]
    public LampController[] targetLights; // ★★★ 변경점: 단일 Light에서 Light[] 배열로 변경!

    public bool isLightOn = false;
    public AudioClip switchOn;
    public AudioClip switchOff;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        LampManager.Instance.RegisterLampSwitch(this);
        // 1. 배열이 비어있는지, 또는 첫 번째 항목이 비어있는지 확인합니다.
        if (targetLights == null || targetLights.Length == 0 || targetLights[0] == null)
        {
            Debug.Log("LightSwitch 스크립트에 'Target Lights'가 하나 이상 연결되어야 합니다!");
        }
        else
        {
            // 2. 첫 번째 전등의 상태를 기준으로 초기 상태(isLightOn)를 정합니다.
            // (모든 전등이 처음에 같은 상태라고 가정)
            foreach(LampController l in targetLights)
            {
                if (l == null) continue;
                else if (isLightOn)
                {
                    l.TurnOn();
                    LampManager.Instance.RegisterLamp(l);
                }
                else
                {
                    l.TurnOff();
                }
            }
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 연결된 '모든' 전등의 상태를 반전시킵니다.
    /// </summary>
    public void ToggleLights()
    {
        // 1. 배열 자체에 문제가 없는지 다시 한번 확인
        if (targetLights == null || targetLights.Length == 0) return;

        // 2. 마스터 전등 상태를 반전시킵니다.
        isLightOn = !isLightOn;
        // 3. ★★★ 변경점: foreach 반복문으로 배열의 '모든' 전등을 순회합니다.
        foreach (LampController light in targetLights)
        {
            // (혹시 배열 중간에 비어있는(null) 항목이 있을 경우를 대비)
            if (light != null && !light.isBroken)
            {
                light.Toggle();
                if (isLightOn)
                {
                    //Debug.Log(this.name+light.name+"Toggle켜짐");
                    LampManager.Instance.RegisterLamp(light);
                    audioSource.PlayOneShot(switchOn);
                }
                else if (!isLightOn)
                {
                    //Debug.Log(this.name + light.name+"Toggle끔");
                    LampManager.Instance.QuitLamp(light);
                    audioSource.PlayOneShot(switchOff);

                }
            }   
        }

        LampManager.Instance.UpdateActiveLampCount();
    }
}