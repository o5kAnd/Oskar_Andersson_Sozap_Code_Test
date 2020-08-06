using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameMain : MonoBehaviour
{
    public GameObject Prefab_UI_Main;
    public GameObject Prefab_UI_ScoreHolder;
    public GameObject Prefab_UI_CountDownText;

    public UI_Main m_UI_Main;

    private static GameMain SelfPointer;
    static public GameMain GetGameMain() {   return SelfPointer; }

    public class PlayerInfo
    {
        public PlayerInfo(string name, int listId, Vector2 spawnPosition, Color color, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey)
        {
            this.name = name;
            this.listId = listId;
            this.score = 0;
            this.score_PrevRound = 0;
            this.position = spawnPosition;
            this.color = color;
            this.upKey = upKey;
            this.downKey = downKey;
            this.leftKey = leftKey;
            this.rightKey = rightKey;
        }



        public string name;
        public int listId;

        //--- Scores ----
        int score;
        public int GetScore() { return score; }
        int score_PrevRound;
        public int GetScore_PreviousRound() { return score_PrevRound; }
        public void AddScore(int scoreNum)
        {
            UpdatePreviousMatchScore();
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


        public Vector2 position;
        public Color color;
        public KeyCode upKey;
        public KeyCode downKey;
        public KeyCode leftKey;
        public KeyCode rightKey;
    }

    List<PlayerInfo> m_List_Players = new List<PlayerInfo>();

    const int PLAYER_AMOUNT_MAX = 4;
    const int PLAYER_AMOUNT_MIN = 2;
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
            ObjectManager.GetObjectManager().SynchPlayerShipsToPlayerNum(num);

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
        m_List_Players.Add(new PlayerInfo("Player 1", m_List_Players.Count, new Vector2(p.x, p.y), Color.red, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
        m_List_Players.Add(new PlayerInfo("Player 2", m_List_Players.Count, new Vector2(p.x, p.y), Color.green, KeyCode.Y, KeyCode.H, KeyCode.G, KeyCode.J));
        m_List_Players.Add(new PlayerInfo("Player 3", m_List_Players.Count, new Vector2(p.x, p.y), Color.yellow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow));
        m_List_Players.Add(new PlayerInfo("Player 4", m_List_Players.Count, new Vector2(p.x, p.y), Color.blue, KeyCode.Keypad8, KeyCode.Keypad5, KeyCode.Keypad4, KeyCode.Keypad6));
    }

    public PlayerInfo GetPlayerInfo(int playerId)
    {
        return m_List_Players[playerId];
    }
    public int GetPlayerListSize()
    {
        return m_List_Players.Count;
    }


    void Awake()
    {
        SelfPointer = this;
        RenderSettings.ambientLight = Color.white;
        InitAllPlayers();
    }
    void Start()
    {
        // Init all managers
        ObjectManager.GetObjectManager().Init();
        GameStates_InitState();
        GameStates_ChangeState(GAME_STATE.PLAYERS_PICK_MENU);
    }

    void Update()
    {
        //raycastTest();

        float deltaTime = Time.deltaTime;
        m_Array_GameStates[m_CurrentGameState].UpdateState(deltaTime);
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
    }


    int CountDown_Round = 0;
    bool CountDown_FirstMessageShowed = false;
    int CountDown_NumCountsUntilStart = 4;
    int CountDown_CurrentCount = 0;
    float CountDown_Timer = 0.0f;

    void CountDown_Reset()
    {
        CountDown_CurrentCount = CountDown_NumCountsUntilStart;
        CountDown_Timer = 0.0f;
        CountDown_FirstMessageShowed = false;
    }

    void CountDown_Update(float deltaTime)
    {
        if (CountDown_Timer >= 1.0f)
        {
            CountDown_CurrentCount--;
            CountDown_Timer = 0.0f;
            if (CountDown_FirstMessageShowed == false)
            {
                CountDown_FirstMessageShowed = true;
                if (CountDown_Round <= 1)
                    SpawnCountDownUI("First to " + SCORE_AMOUNT_FOR_END +" points");
                else
                    SpawnCountDownUI("Round: " + CountDown_Round);
            }
            else if (CountDown_CurrentCount > 0)
                SpawnCountDownUI("" + CountDown_CurrentCount);
            else if(CountDown_CurrentCount == 0)
                SpawnCountDownUI("Start");


        }
            
        CountDown_Timer += deltaTime;
    }

    bool CountDown_CheckIfFinished()
    {
        if (CountDown_CurrentCount < 0)
            return true;
        return false;
    }





    const int SCORE_AMOUNT_PER_WIN = 30;
    const int SCORE_AMOUNT_FOR_END = 60;
    public void CheckWinner()
    {
        ObjectManager o = ObjectManager.GetObjectManager();
        int numShipsLeft = o.CheckWinner_GetNumbersOfShipsLeft();
        if(numShipsLeft <= 1)
        {
            for (int i = 0; i < m_List_Players.Count; ++i)
                m_List_Players[i].UpdatePreviousMatchScore();

            if (numShipsLeft == 1)
            {
                int winnerId = o.CheckWinner_GetPlayerIdFromShip(0);
                m_List_Players[winnerId].AddScore(SCORE_AMOUNT_PER_WIN);
            }
            GameStates_ChangeState(GAME_STATE.SHOW_SCORE);
        }
    }



    public void SpawnCountDownUI(string countDownText)
    {
        GameObject obj = Instantiate(Prefab_UI_CountDownText, Vector2.zero, Quaternion.identity);
        UI_CountDownText m_UI_Score = obj.GetComponent<UI_CountDownText>();

        m_UI_Score.Init(countDownText, 0.75f);
    }





    //-----------------------------------------------------------------------------
    //---------- STATE SECTION -------------------------------

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
        ObjectManager.GetObjectManager().SynchPlayerShipsToPlayerNum(numberOfPlayersActive);
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
        ObjectManager.GetObjectManager().SetShipsIconVisibility(true);
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

        m_UI_Score.Init(SCORE_AMOUNT_FOR_END, numberOfPlayersActive);
    }
    public void State_ShowScore_ExitState()
    {
        ObjectManager.GetObjectManager().Bullet_RemoveAllBullets();
        ObjectManager.GetObjectManager().SynchPlayerShipsToPlayerNum(numberOfPlayersActive);
    }
    public void State_ShowScore_Update(float deltaTime)
    {

    }

    //-----------------------------------------------------------------------------













    Vector2 RayStart = Vector2.zero;
    Vector2 RayEnd = Vector2.zero;
    void raycastTest()
    {
        if (Input.GetMouseButton(0))
        {
            RayStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetMouseButton(1))
        {
            RayEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        Vector2 rayDir = RayEnd - RayStart;
        float l = Mathf.Sqrt(Vector2.Dot(rayDir, rayDir));
        rayDir = rayDir.normalized;
        RaycastHit2D raycastHit = Physics2D.Raycast(RayStart, rayDir, l);

        Debug.DrawLine(RayStart, RayEnd, Color.blue);
        Debug.DrawRay(RayStart, rayDir * l, Color.red);

        if (raycastHit.collider != null)
            Debug.Log("RAYCAST: " + raycastHit.collider.name);


        var Coll = Physics2D.Linecast(RayStart, RayEnd);
        if (Coll.collider != null)
            Debug.Log("LINECAST: " + Coll.collider.name);
    }



}
