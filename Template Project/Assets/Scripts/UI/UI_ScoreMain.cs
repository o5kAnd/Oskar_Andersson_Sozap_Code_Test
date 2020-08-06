using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_ScoreMain : MonoBehaviour
{
    public Text m_Text_NextStateButtonText;
    public List<UI_ScorePlayer> m_List_PlayerScores = new List<UI_ScorePlayer>();
    public List<GameObject> m_List_RankingObjects = new List<GameObject>();

    GAME_STATE m_GameStat_NextState;
    public void Init(int maxScore, int playerAmount)
    {
        GameMain G = GameMain.GetGameMain();
        for(int i = 0; i < m_List_PlayerScores.Count; ++i)
        {
            m_List_PlayerScores[i].Init(G.GetPlayerInfo(i), maxScore);
        }

        if (CheckIfPlayerReachedMaxPoints() == true)
        {
            m_GameStat_NextState = GAME_STATE.PLAYERS_PICK_MENU;
            m_Text_NextStateButtonText.text = "To Menu";
        }
        else
        {
            m_GameStat_NextState = GAME_STATE.PRE_GAME;
            m_Text_NextStateButtonText.text = "Next Round";
        }

        SortPlayerRanking();

        UpdateActivePlayerScoreBoxes(playerAmount);

    }


    void UpdateActivePlayerScoreBoxes(int numPlayers)
    {
        for (int i = 0; i < numPlayers; ++i)
            m_List_PlayerScores[i].gameObject.SetActive(true);

        for (int i = numPlayers; i < m_List_PlayerScores.Count; ++i)
            m_List_PlayerScores[i].gameObject.SetActive(false);
    }


    void SortPlayerRanking()
    {
        List<int> idRankList = new List<int>();

        // create ranking
        for (int i = 0; i < m_List_PlayerScores.Count; ++i)
            idRankList.Add(i);

        // sort ranking
        for (int i = 0; i < idRankList.Count; ++i)
        {
            int highest = m_List_PlayerScores[idRankList[i]].GetScore();
            int highestId = idRankList[i];

            for (int j = i; j < idRankList.Count; ++j)
            {
                // switch place if score is higher or if score is the same, prioritase player order
                if (m_List_PlayerScores[idRankList[j]].GetScore() > highest || (m_List_PlayerScores[idRankList[j]].GetScore() == highest && idRankList[i] > idRankList[j]))
                {
                    highest = m_List_PlayerScores[idRankList[j]].GetScore();
                    int temp = idRankList[i];
                    idRankList[i] = idRankList[j];
                    idRankList[j] = temp;
                }
            }
        }

        // set correct parent
        for (int i = 0; i < m_List_RankingObjects.Count; ++i)
            m_List_PlayerScores[idRankList[i]].transform.SetParent((m_List_RankingObjects[i].transform));

    }

    bool CheckIfPlayerReachedMaxPoints()
    {
        for(int i = 0; i < m_List_PlayerScores.Count; ++i)
        {
            if (m_List_PlayerScores[i].GetIfMaxScore() == true)
                return true;
        }
        return false;
    }


    public void Button_NextState()
    {
        GameMain.GetGameMain().GameStates_ChangeState(m_GameStat_NextState);
        this.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
    }

}
