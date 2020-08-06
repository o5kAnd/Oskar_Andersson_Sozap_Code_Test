using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_CountDownText : MonoBehaviour
{
    public Text m_Text;
    float m_TimeDisplayed;
    float m_CurrentTimer;
    public void Init(string countDownMessage, float timeDisplayed)
    {
        m_Text.text = countDownMessage;
        m_TimeDisplayed = timeDisplayed;

    }

    float m_StartSizeFactor = 1.0f;
    float m_StartTime = 0.0f;
    float m_StartCurve = 1.0f;
    public void SetIntroductionSettings(float startSizeFactor, float time, float curve)
    {
        m_StartSizeFactor = startSizeFactor;

    }

    
    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (m_CurrentTimer >= m_TimeDisplayed)
            this.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
        else
            m_CurrentTimer += deltaTime;
    }
}
