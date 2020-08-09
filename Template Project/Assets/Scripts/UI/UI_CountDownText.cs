using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_CountDownText : MonoBehaviour
{
    // a Count down specifically designed for UI text manipulations.
    // Manipulation includes 2 main parts (introduction/start and end), where one can decide how the text change in size and transparency over time.

    // The script is used separately from the rest, after initiation, it will do its job and then destroy itself and the game object.
    // The functions that should be run from outside the script is Init and SetIntro/EndSettings functions 

    // Parts of the UI that is assign in unity editor --------
    public Text m_Text;
    //-------------------------------

    float m_TimeDisplayed;
    float m_CurrentTimer;
    public void Init(string countDownMessage, float timeDisplayed)
    {
        m_Text.text = countDownMessage;
        m_TimeDisplayed = timeDisplayed;
        SetTextScale(0.0f);
        SetTextAlpha(0.0f);
    }

    
    void Update()
    {
        float deltaTime = Time.deltaTime;
        if(Intro_Update(m_CurrentTimer) == false)
            End_Update(m_CurrentTimer, m_TimeDisplayed);

        if (m_CurrentTimer >= m_TimeDisplayed)
            this.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
        else
            m_CurrentTimer += deltaTime;
    }





    //----- Start/intro time functions -----------

    float m_StartSizeFactor = 1.0f;
    float m_StartTime = 0.0f;
    float m_StartCurve = 1.0f;
    float m_StartAlpha = 1.0f;
    //float m_Start_CurrentTime = 0.0f;

    // function that will be active at the beginning of texts display time
    // startSizeFactor & fadeFromAlphaVal = start value from 0 - 1 of what size or alpha the text have
    // time = how long the startTime is (should be <= total time)
    // curve = how the changes take place, 1 == linier, < 1 == fast change first, then slower, > 1 == slow change first, then faster
    public void SetIntroductionSettings(float startSizeFactor, float fadeFromAlphaVal, float time, float curve)
    {
        m_StartSizeFactor = startSizeFactor;
        m_StartTime = time;
        m_StartCurve = curve;
        m_StartAlpha = fadeFromAlphaVal;
        //m_Start_CurrentTime = 0.0f;
    }

    bool Intro_Update(float currentTime) // returns true if modifications is run, this is so End_Update wont run until this is finished
    {
        if (currentTime < m_StartTime)
        {
            float modifyFactor = GetSizeFactor(currentTime, m_StartTime, m_StartCurve);

            float s = (1.0f - m_StartSizeFactor) * modifyFactor;
            float a = (1.0f - m_StartAlpha) * modifyFactor;

            SetTextScale(m_StartSizeFactor + s);
            SetTextAlpha(m_StartAlpha + a);

            return true;
        }
        else
        {
            SetTextScale(1.0f);
            SetTextAlpha(1.0f);

            return false;
        }
    }


    //----- end time functions -----------

    float m_EndSizeFactor = 1.0f;
    float m_EndTime = 0.0f;
    float m_EndCurve = 1.0f;
    float m_EndAlpha = 1.0f;
    //float m_EndCurrentTime = 0.0f;

    // the same principle as "SetIntroductionSettings"
    public void SetEndSettings(float endSizeFactor, float fadeToAlphaVal, float time, float curve)
    {
        m_EndSizeFactor = endSizeFactor;
        m_EndTime = time;
        m_EndCurve = curve;
        m_EndAlpha = fadeToAlphaVal;
        //m_EndCurrentTime = 0.0f;
    }

    void End_Update(float currentTime, float maxTime)
    {
        float endTimeBegin = maxTime - m_EndTime;
        if (currentTime > endTimeBegin)
        {
            float currentEndTime = currentTime - endTimeBegin;
            float currentEndTimeRev = m_EndTime - currentEndTime;

            if (currentEndTimeRev < 0.0f)
                currentEndTimeRev = 0.0f;

            float modifyFactor = GetSizeFactor(currentEndTimeRev, m_EndTime, m_EndCurve);

            float s = (1.0f - m_EndSizeFactor) * modifyFactor;
            float a = (1.0f - m_EndAlpha) * modifyFactor;

            SetTextScale(m_EndSizeFactor + s);
            SetTextAlpha(m_EndAlpha + a);
        }
        else
        {
            SetTextScale(1.0f);
            SetTextAlpha(1.0f);
        }
    }



    //----- General functions -----------

    float GetSizeFactor(float currentVal, float maxVal, float curve)
    {
        float f = currentVal / maxVal;
        return Mathf.Pow(f, curve);
    }

    void SetTextAlpha(float alphaVal)
    {
        m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, alphaVal);
    }

    void SetTextScale(float scale)
    {
        m_Text.transform.localScale = new Vector3(scale, scale, 1.0f);
    }
}
