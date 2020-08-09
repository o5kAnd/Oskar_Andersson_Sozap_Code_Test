using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_ScorePlayer : MonoBehaviour
{
    // This script is for handling the individual players score, it also manage the visuals like filling the scorebar

    // Parts of the UI that is assign in unity editor --------
    public Text m_Text_RankNumber;
    public Text m_Text_Points;
    public Text m_Text_PlayerName;
    public Text m_Text_Winner;
    public Image m_Image_ColorIndicator;
    //----------------------------------------------
    public Image m_Image_ScoreBar;
    public RectTransform m_RectTransform_ScoreBar;

    GameMain.PlayerInfo m_PlayerInfo_ScoreHolder;
    int m_MaxScore;
    int m_CurrentScore;
    int m_PrevScore;

    float m_ScoreBarMaxWidth;
    public void Init(GameMain.PlayerInfo playerInfoToSynchWith, int maxScore)
    {
        SetWinnerTextActivation(false);
        m_PlayerInfo_ScoreHolder = playerInfoToSynchWith;
        m_Text_PlayerName.text = m_PlayerInfo_ScoreHolder.name;
        m_MaxScore = maxScore;

        m_Image_ColorIndicator.color = m_PlayerInfo_ScoreHolder.color * 0.5f;
        m_Image_ScoreBar.color = m_PlayerInfo_ScoreHolder.color;

        m_ScoreBarMaxWidth = m_RectTransform_ScoreBar.sizeDelta.x;

        SetScore(m_PlayerInfo_ScoreHolder.GetScore(), m_PlayerInfo_ScoreHolder.GetScore_PreviousRound());
    }


    public bool GetIfMaxScore()
    {
        if (m_CurrentScore >= m_MaxScore)
        {
            SetWinner();
            return true;
        }  
        return false;
    }

    public int GetScore() { return m_CurrentScore; }

    public void SetScore(int currentScore, int previousScore)
    {
        m_FillScorebar_CurrentTime = 0.0f;
        m_CurrentScore = currentScore;
        m_PrevScore = previousScore;
        m_Text_Points.text = "" + m_CurrentScore;

        m_RectTransform_ScoreBar.sizeDelta = new Vector2(GetScoreBarWidth(m_PrevScore, m_MaxScore), m_RectTransform_ScoreBar.sizeDelta.y);
    }

    void SetWinnerTextActivation(bool active)
    {
        m_Text_Winner.gameObject.SetActive(active);
    }

    bool activateWinnerAnimation = false;
    void SetWinner()
    {
        SetWinnerTextActivation(true);
        activateWinnerAnimation = true;
    }
    void PlayWinnerAnimation()
    {
        if(activateWinnerAnimation == true)
        {
            activateWinnerAnimation = false;
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator != null)
                animator.SetBool("isWinner", true);
        }
    }

    float GetScoreBarWidth(float currentScore, float maxScore)
    {
        if (currentScore > maxScore)
            currentScore = maxScore;
        float scoreFactor = (float)currentScore / (float)maxScore;
        return scoreFactor * m_ScoreBarMaxWidth;
    }

    public void SetRankNumber(int num)
    {
        m_Text_RankNumber.text = "" + num; 
    }

    float m_FillScorebar_CurrentTime = 0.0f;
    const float FILL_SCORE_BAR_TIME_UNTIL_START = 0.5f;
    void Update_FillScoreBar(float deltaTime, float timeInSecUntilFilled)
    {
        if (m_FillScorebar_CurrentTime > FILL_SCORE_BAR_TIME_UNTIL_START)
        {
            float time = m_FillScorebar_CurrentTime - FILL_SCORE_BAR_TIME_UNTIL_START;
            if (time > 1.0f)
            {
                PlayWinnerAnimation();
                time = 1.0f;
            }
                

            float prev = GetScoreBarWidth(m_PrevScore, m_MaxScore);
            float current = GetScoreBarWidth(m_CurrentScore, m_MaxScore);

            float size = (time * current) + ((1.0f - time) * prev);
            m_RectTransform_ScoreBar.sizeDelta = new Vector2(size, m_RectTransform_ScoreBar.sizeDelta.y);

            m_FillScorebar_CurrentTime += (deltaTime / timeInSecUntilFilled);
        }
        else
            m_FillScorebar_CurrentTime += deltaTime;
    }


    void Update()
    {
        Update_FillScoreBar(Time.deltaTime, 1.0f);
    }
}
