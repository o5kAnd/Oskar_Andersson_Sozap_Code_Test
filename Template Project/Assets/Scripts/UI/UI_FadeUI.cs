using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_FadeUI : MonoBehaviour// should be assigned to the main holder parent
{


    Text[] m_Array_Texts;
    List<float> m_List_TextsDefaultAlpha = new List<float>();
    Image[] m_Array_Images;
    List<float> m_List_ImagesDefaultAlpha = new List<float>();


    public void Init(float fadeInTime, float fadeOutTime)
    {
        m_List_TextsDefaultAlpha.Clear();
        m_Array_Texts = gameObject.GetComponentsInChildren<Text>();
        for (int i = 0; i < m_Array_Texts.Length; ++i)
            m_List_TextsDefaultAlpha.Add(m_Array_Texts[i].color.a);

        m_List_ImagesDefaultAlpha.Clear();
        m_Array_Images = gameObject.GetComponentsInChildren<Image>();
        for (int i = 0; i < m_Array_Images.Length; ++i)
            m_List_ImagesDefaultAlpha.Add(m_Array_Images[i].color.a);

        m_FadeIn_CurrentTime = 0.0f;
        m_FadeIn_TotalTime = fadeInTime;

        m_FadeOut_CurrentTime = 0.0f;
        m_FadeOut_TotalTime = fadeOutTime;

        SetAlpha(0);
    }



    bool m_FadeIn_Active = false;
    float m_FadeIn_TotalTime = 0.0f;
    float m_FadeIn_CurrentTime = 0.0f;

    public void ActivateFadeIn()
    {
        if(m_FadeIn_TotalTime > 0.0f)
        {
            m_FadeIn_Active = true;
            SetAlpha(0);
        }
        else
            SetAlpha(1.0f);
    }

    void Update_FadeIn(float deltaTime)
    {
        if (m_FadeIn_Active == true)
        {
            if (m_FadeIn_CurrentTime < m_FadeIn_TotalTime)
            {
                float f = m_FadeIn_CurrentTime / m_FadeIn_TotalTime;
                SetAlpha(f);
                m_FadeIn_CurrentTime += deltaTime;
            }
            else
            {
                m_FadeIn_Active = false;
                SetAlpha(1.0f);
            }   
        }
    }


    bool m_FadeOut_Active = false;
    float m_FadeOut_TotalTime = 0.0f;
    float m_FadeOut_CurrentTime = 0.0f;
    float m_FadeOut_AlphaFromUnfinishedFadeIn = 1.0f;

    public void ActivateFadeOut()
    {
        if (m_FadeOut_TotalTime > 0.0f)
        {
            m_FadeOut_Active = true;
            SetAlpha(1.0f);
            if (m_FadeIn_Active == true)
            {
                m_FadeIn_Active = false;
                m_FadeOut_AlphaFromUnfinishedFadeIn = m_FadeIn_CurrentTime / m_FadeIn_TotalTime;
            }
            else
                m_FadeOut_AlphaFromUnfinishedFadeIn = 1.0f;
        }
        else
            SetAlpha(0.0f);
    }

    void Update_FadeOut(float deltaTime)
    {
        if (m_FadeOut_Active == true)
        {
            if (m_FadeOut_CurrentTime < m_FadeOut_TotalTime)
            {
                float f = m_FadeOut_CurrentTime / m_FadeOut_TotalTime;
                SetAlpha((1.0f - f) * m_FadeOut_AlphaFromUnfinishedFadeIn);
                m_FadeOut_CurrentTime += deltaTime;
            }
            else
            {
                m_FadeOut_Active = false;
                SetAlpha(0.0f);
            }
                
        }

    }

    void SetAlpha(float alphaVal)
    {
        //Debug.Log("ALPHA VAL: " + alphaVal);
        for (int i = 0; i < m_Array_Texts.Length; ++i)
            m_Array_Texts[i].color = GetColorWithNewAlpha(m_Array_Texts[i].color, m_List_TextsDefaultAlpha[i] * alphaVal);

        for (int i = 0; i < m_Array_Images.Length; ++i)
            m_Array_Images[i].color = GetColorWithNewAlpha(m_Array_Images[i].color, m_List_ImagesDefaultAlpha[i] * alphaVal);
    }

    Color GetColorWithNewAlpha(Color color, float newAlpha)
    {
        return new Color(color.r, color.g, color.b, newAlpha);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        Update_FadeIn(deltaTime);
        Update_FadeOut(deltaTime);
    }
}
