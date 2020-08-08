using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    // The main script for the player select UI

    public GameObject Prefab_UI_PlayerBox;

    public GameObject m_MainContentHolder;

    public Text m_Text_NumPlayers; 

    public List<UI_Player> m_List_PlayerBoxes = new List<UI_Player>();

    public void Init(int numPlayers)
    {
        InitAllPlayerBoxes(numPlayers);
    }
    public void InitAllPlayerBoxes(int numPlayers)
    {
        for (int i = 0; i < GameMain.GetGameMain().GetPlayerListSize(); ++i)
            m_List_PlayerBoxes[i].Init(GameMain.GetGameMain().GetPlayerInfo(i), m_MainContentHolder);

        UpdateActiveGameBoxes(numPlayers);
        m_Text_NumPlayers.text = "" + numPlayers;
    }

    void UpdateActiveGameBoxes(int numPlayers)
    {
        for (int i = 0; i < numPlayers; ++i)
            m_List_PlayerBoxes[i].gameObject.SetActive(true);

        for (int i = numPlayers; i < m_List_PlayerBoxes.Count; ++i)
            m_List_PlayerBoxes[i].gameObject.SetActive(false);
    }

    public void ButtonPressed_StartGame()
    {
        GameMain.GetGameMain().GameStates_ChangeState(GAME_STATE.PRE_GAME, 0.5f);
        m_FadeScript.ActivateFadeOut();
        this.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction(0.6f);
    }

    public void ButtonPressed_IncreaseNumPlayers()
    {
        GameMain.GetGameMain().IncreaseNumberOfPlayers();
        int numPlayers = GameMain.GetGameMain().GetNumberOfActivePlayers();
        m_Text_NumPlayers.text = "" + numPlayers;
        UpdateActiveGameBoxes(numPlayers);
    }

    public void ButtonPressed_DecreaseNumPlayers()
    {
        GameMain.GetGameMain().DecreaseNumberOfPlayers();
        int numPlayers = GameMain.GetGameMain().GetNumberOfActivePlayers();
        m_Text_NumPlayers.text = "" + numPlayers;
        UpdateActiveGameBoxes(numPlayers);
    }

    UI_FadeUI m_FadeScript;
    void Awake()
    {
        m_FadeScript = this.gameObject.AddComponent<UI_FadeUI>();
        m_FadeScript.Init(0.5f, 0.5f);
        m_FadeScript.ActivateFadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
