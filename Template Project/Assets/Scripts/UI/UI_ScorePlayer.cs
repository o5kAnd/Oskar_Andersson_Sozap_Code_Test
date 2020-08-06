using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_ScorePlayer : MonoBehaviour
{
    public Text m_Text_RankNumber;
    public Text m_Text_Points;
    public Text m_Text_PlayerName;
    public Image m_Image_ColorIndicator;

    public Image m_Image_ScoreBar;
    public RectTransform m_RectTransform_ScoreBar;

    GameMain.PlayerInfo m_PlayerInfo_ScoreHolder;
    int m_MaxScore;
    int m_CurrentScore;
    int m_PrevScore;

    float m_ScoreBarMaxWidth;
    public void Init(GameMain.PlayerInfo playerInfoToSynchWith, int maxScore)
    {
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
            return true;
        return false;
    }

    public int GetScore() { return m_CurrentScore; }

    public void SetScore(int currentScore, int previousScore)
    {
        m_CurrentScore = currentScore > m_MaxScore ? m_MaxScore : currentScore;
        m_PrevScore = previousScore > m_MaxScore ? m_MaxScore : previousScore;
        m_Text_Points.text = "" + m_CurrentScore;

        float scoreFactor = (float)m_CurrentScore/(float)m_MaxScore;
        m_RectTransform_ScoreBar.sizeDelta = new Vector2(scoreFactor * m_ScoreBarMaxWidth, m_RectTransform_ScoreBar.sizeDelta.y);
    }

    public void SetRankNumber(int num)
    {
        m_Text_RankNumber.text = "" + num; 
    }



    void Update()
    {
        
    }
}
