using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameMain : MonoBehaviour
{
    // The main script, it controls the game states and fundamental functions
    public GameObject Prefab_UI_Main;
    public GameObject Prefab_UI_ScoreHolder;
    public GameObject Prefab_UI_CountDownText;

    public UI_Main m_UI_Main;

    private static GameMain SelfPointer;
    static public GameMain GetGameMain() {   return SelfPointer; }

    //------------- PLAYER INFO SECTION -----------------------
    // This class contains all important player information, 
    // reference's of this will be passed to different objects and used to check information such as ownership, 
    // also to adjust some data like adding scores
    public class PlayerInfo
    {
        public PlayerInfo(string name, int listId, Vector2 spawnPosition, Color color, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey)
        {
            this.name = name;
            this.listId = listId;
            this.score = 0;
            this.score_PrevRound = 0;
            this.spawnPosition = spawnPosition;
            this.color = color;
            this.upKey = upKey;
            this.downKey = downKey;
            this.leftKey = leftKey;
            this.rightKey = rightKey;
        }

        public string name;
        public int listId;
        public Vector2 spawnPosition;
        public Color color;
        public KeyCode upKey;
        public KeyCode downKey;
        public KeyCode leftKey;
        public KeyCode rightKey;

        //--- Scores ----
        int score;
        public int GetScore() { return score; }
        int score_PrevRound;
        public int GetScore_PreviousRound() { return score_PrevRound; }
        public void AddScore(int scoreNum)
        {
            score += scoreNum;
        }

        public void UpdatePreviousMatchScore()
        {
            score_PrevRound = score;
        }

        public void ResetScore()
        {
            score = 0;
            score_PrevRound = 0;
        }
        //---
    }

    const int PLAYER_AMOUNT_MAX = 4;
    const int PLAYER_AMOUNT_MIN = 2;

    List<PlayerInfo> m_List_Players = new List<PlayerInfo>();
    public void IncreaseNumberOfPlayers()
    {
        SetNumberOfPlayers(numberOfPlayersActive + 1);        
    }

    public void DecreaseNumberOfPlayers()
    {
        SetNumberOfPlayers(numberOfPlayersActive - 1);        
    }
    public void SetNumberOfPlayers(int num)
    {
        if (num > PLAYER_AMOUNT_MAX)
            num = PLAYER_AMOUNT_MAX;
        else if (num < PLAYER_AMOUNT_MIN)
            num = PLAYER_AMOUNT_MIN;

        if (numberOfPlayersActive != num)
            ObjectManager.GetObjectManager().Ship_SynchToPlayerNum(num);

        numberOfPlayersActive = num;
    }

    int numberOfPlayersActive = PLAYER_AMOUNT_MIN;
    public int GetNumberOfActivePlayers() { return numberOfPlayersActive; }


    Vector2 m_Position_MidPointPlayerSpawnOffset = new Vector2(2.0f, 2.0f);
    void ResetPlayerScores()
    {
        for (int i = 0; i < m_List_Players.Count; ++i)
            m_List_Players[i].ResetScore();
    }
    void InitAllPlayers()
    {
        Vector2 p = m_Position_MidPointPlayerSpawnOffset;
        m_List_Players.Clear();
        m_List_Players.Add(new PlayerInfo("Player 1", m_List_Players.Count, new Vector2(p.x, p.y), GameSettings.GetPlayerOneColor(), GameSettings.PLAYER_ONE_KEY_UP, GameSettings.PLAYER_ONE_KEY_DOWN, GameSettings.PLAYER_ONE_KEY_LEFT, GameSettings.PLAYER_ONE_KEY_RIGHT));
        m_List_Players.Add(new PlayerInfo("Player 2", m_List_Players.Count, new Vector2(p.x, p.y), GameSettings.GetPlayerTwoColor(), GameSettings.PLAYER_TWO_KEY_UP, GameSettings.PLAYER_TWO_KEY_DOWN, GameSettings.PLAYER_TWO_KEY_LEFT, GameSettings.PLAYER_TWO_KEY_RIGHT));
        m_List_Players.Add(new PlayerInfo("Player 3", m_List_Players.Count, new Vector2(p.x, p.y), GameSettings.GetPlayerThreeColor(), GameSettings.PLAYER_THREE_KEY_UP, GameSettings.PLAYER_THREE_KEY_DOWN, GameSettings.PLAYER_THREE_KEY_LEFT, GameSettings.PLAYER_THREE_KEY_RIGHT));
        m_List_Players.Add(new PlayerInfo("Player 4", m_List_Players.Count, new Vector2(p.x, p.y), GameSettings.GetPlayerFourColor(), GameSettings.PLAYER_FOUR_KEY_UP, GameSettings.PLAYER_FOUR_KEY_DOWN, GameSettings.PLAYER_FOUR_KEY_LEFT, GameSettings.PLAYER_FOUR_KEY_RIGHT));
    }

    public PlayerInfo GetPlayerInfo(int playerId)
    {
        return m_List_Players[playerId];
    }
    public int GetPlayerListSize()
    {
        return m_List_Players.Count;
    }

    //--------------------------------------------------------------------------------



    //---------------------------------------------------------------
    // ----------- COUNT DOWN SECTION -----------
    // Count down is used in pre-inGame state and consist of the period defined by the constant below
    // This will also show some information such as which round it is or how much points needed for victory

    int CountDown_Round = 0;
    bool CountDown_FirstMessageShowed = false;
    int CountDown_CurrentCount = 0;
    float CountDown_Timer = 0.0f;

    void CountDown_Reset()
    {
        CountDown_CurrentCount = GameSettings.NUMBER_OF_COUNT_DOWN_UNTIL_GAME_START;
        CountDown_Timer = 0.0f;
        CountDown_FirstMessageShowed = false;
    }

    float m_CountDownSizeDisplayFactor = 0.0f;
    void CountDown_Update(float deltaTime)
    {
        if (CountDown_Timer >= 1.0f)
        {
            CountDown_CurrentCount--;
            CountDown_Timer = 0.0f;
            if (CountDown_FirstMessageShowed == false)
            {
                m_CountDownSizeDisplayFactor = 0.0f;
                CountDown_FirstMessageShowed = true;
                if (CountDown_Round <= 1)
                    SpawnCountDownUI("First to " + GameSettings.SCORE_AMOUNT_FOR_TOTAL_VICTORY + " points", 1.0f, m_CountDownSizeDisplayFactor, 0.0f);
                else
                    SpawnCountDownUI("Round: " + CountDown_Round, 1.0f, m_CountDownSizeDisplayFactor, 0.0f);
            }
            else if (CountDown_CurrentCount > 0)
            {
                SpawnCountDownUI("" + CountDown_CurrentCount, 1.0f, m_CountDownSizeDisplayFactor, 0.7f);
                m_CountDownSizeDisplayFactor = 0.7f;
            }
            else if(CountDown_CurrentCount == 0)
                SpawnCountDownUI("Start", 1.1f, m_CountDownSizeDisplayFactor, 0.0f);
        } 
        CountDown_Timer += deltaTime;
    }

    bool CountDown_CheckIfFinished()
    {
        if (CountDown_CurrentCount < 0)
            return true;
        return false;
    }

    // size factors is 0-1 val and adjusts how large the text is in relation to its standard size at the beginning and end
    public void SpawnCountDownUI(string countDownText, float displayTime, float sizeFactorStart, float sizeFactorEnd)
    {
        GameObject obj = Instantiate(Prefab_UI_CountDownText, Vector2.zero, Quaternion.identity);
        UI_CountDownText m_UI_Score = obj.GetComponent<UI_CountDownText>();

        m_UI_Score.Init(countDownText, displayTime);
        m_UI_Score.SetIntroductionSettings(sizeFactorStart, 1.0f, displayTime * 0.6f, 0.3f);
        m_UI_Score.SetEndSettings(sizeFactorEnd, 1.0f, displayTime * 0.4f, 0.3f);
    }

    //--------------------------------------------------------------------------------


    //---------------------------------------------------------------
    // ----------- SCORE AND MATCH END SECTION -----------
    // This section is used to check if the game ends and should transition to show score state
    // its start by checking the winner or draw, which is done in the ingame state update, and then proceeds to the EndMatch function.

    bool endMatch_Inited = false;
    public void CheckWinner()
    {
        if(endMatch_Inited == false)
        {
            ObjectManager o = ObjectManager.GetObjectManager();
            int numShipsLeft = o.CheckWinner_GetNumbersOfShipsLeft();
            if (numShipsLeft <= 1)
            {
                o.Ship_SetDrawingActivity(false);
                endMatch_Inited = true;
                if (GameSettings.END_MATCH_TIME_BEFORE_SHOWING_SCORE > 0.0f)
                    Invoke("EndMatch", GameSettings.END_MATCH_TIME_BEFORE_SHOWING_SCORE);
                else
                    EndMatch();
            }
        }
    }

    public void EndMatch()
    {
        if (endMatch_Inited == true)
        {
            endMatch_Inited = false;
            ObjectManager o = ObjectManager.GetObjectManager();
            int numShipsLeft = o.CheckWinner_GetNumbersOfShipsLeft();
            if (numShipsLeft == 1)
            {
                int winnerId = o.CheckWinner_GetPlayerIdFromShip(0);
                m_List_Players[winnerId].AddScore(GameSettings.SCORE_AMOUNT_PER_MATCH_ROUND_WIN);
            }
            GameStates_ChangeState(GAME_STATE.SHOW_SCORE);
        }
    }


    //--------------------------------------------------------------------------------




    //-----------------------------------------------------------------------------
    //---------- STATE SECTION -------------------------------
    // State functions for each state contains exactly one "Enter", "Exit" and "Update" function
    // The states are a good way to handle contained Create, Update and Destroy for parts that should only be active in specific game sections
    // Although practically, many of the functions currently is empty but is there for consistency and potential uses in later version.

    //---- PLAYER PICK MENU STATE ----------------
    public void State_PlayerPickMenu_EnterState()
    {
        if (m_UI_Main == null)
        {
            GameObject obj = Instantiate(Prefab_UI_Main, Vector2.zero, Quaternion.identity);
            m_UI_Main = obj.GetComponent<UI_Main>();
        }
        m_UI_Main.Init(numberOfPlayersActive);
        SetNumberOfPlayers(numberOfPlayersActive);
        ResetPlayerScores();
        ObjectManager.GetObjectManager().Ship_SynchToPlayerNum(numberOfPlayersActive);
    }
    public void State_PlayerPickMenu_ExitState()
    {

    }
    public void State_PlayerPickMenu_Update(float deltaTime)
    {

    }

    //---- PRE GAME STATE ----------------
    public void State_PreGame_EnterState()
    {
        CountDown_Reset();
        ObjectManager.GetObjectManager().Ship_SetIconVisibility(false, false, true);
    }
    public void State_PreGame_ExitState()
    {

    }
    public void State_PreGame_Update(float deltaTime)
    {
        ObjectManager.GetObjectManager().PreGame_Update(deltaTime);
        if (CountDown_CheckIfFinished() == true)
            GameStates_ChangeState(GAME_STATE.IN_GAME);
        else
            CountDown_Update(deltaTime);

    }

    //---- IN GAME STATE ----------------
    public void State_InGame_EnterState()
    {
        ObjectManager.GetObjectManager().Ship_SetIconVisibility(true, true, true);
        ObjectManager.GetObjectManager().Ship_SetDrawingActivity(true);
        for (int i = 0; i < m_List_Players.Count; ++i)
            m_List_Players[i].UpdatePreviousMatchScore();
    }
    public void State_InGame_ExitState()
    {

    }
    public void State_InGame_Update(float deltaTime)
    {
        ObjectManager.GetObjectManager().InGame_Update(deltaTime);
        CheckWinner();
    }

    //---- SHOW SCORE STATE ----------------
    public void State_ShowScore_EnterState()
    {
        GameObject obj = Instantiate(Prefab_UI_ScoreHolder, Vector2.zero, Quaternion.identity);
        UI_ScoreMain m_UI_Score = obj.GetComponent<UI_ScoreMain>();

        m_UI_Score.Init(GameSettings.SCORE_AMOUNT_FOR_TOTAL_VICTORY, numberOfPlayersActive);
    }
    public void State_ShowScore_ExitState()
    {
        ObjectManager.GetObjectManager().Bullet_RemoveAllBullets();
        ObjectManager.GetObjectManager().Ship_SynchToPlayerNum(numberOfPlayersActive);
    }
    public void State_ShowScore_Update(float deltaTime)
    {

    }

    //-----------------------------------------------------------------------------






    void Awake()
    {
        SelfPointer = this;
        RenderSettings.ambientLight = Color.white;
        InitAllPlayers();
    }
    void Start()
    {
        // Init all managers
        GameStates_InitState();
        GameStates_ChangeState(GAME_STATE.PLAYERS_PICK_MENU);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        m_Array_GameStates[m_CurrentGameState].UpdateState(deltaTime);
    }


}
