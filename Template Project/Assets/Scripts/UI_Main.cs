using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
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
        //m_List_PlayerBoxes.Clear();
        for (int i = 0; i < GameMain.GetGameMain().GetPlayerListSize(); ++i)
        {
            //GameObject obj = Instantiate(Prefab_UI_PlayerBox, Vector2.zero, Quaternion.identity);
            //UI_Player p = obj.GetComponent<UI_Player>();
            m_List_PlayerBoxes[i].Init(GameMain.GetGameMain().GetPlayerInfo(i), m_MainContentHolder);
            //m_List_PlayerBoxes.Add(p);
        }
        UpdateActiveGameBoxes(numPlayers);
    }

    void UpdateActiveGameBoxes(int numPlayers)
    {
        for (int i = 0; i < numPlayers; ++i)
            m_List_PlayerBoxes[i].gameObject.SetActive(true);

        for (int i = numPlayers; i < m_List_PlayerBoxes.Count; ++i)
            m_List_PlayerBoxes[i].gameObject.SetActive(false);
    }

    public void AdjustPlayerBoxesBasedOnPlayerNum(int num)
    {

    }

    public void ButtonPressed_StartGame()
    {
        GameMain.GetGameMain().GameStates_ChangeState(GAME_STATE.PRE_GAME);
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
