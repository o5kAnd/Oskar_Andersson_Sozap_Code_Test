using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE { STARTING, PLAYERS_PICK_MENU, PRE_GAME, IN_GAME, SHOW_SCORE, MAX_NUM }
public partial class GameMain : MonoBehaviour
{
    //---- game state management
    class GameState_Virtual
    {
        protected GameMain m_MainRef;
        public void Init(GameMain mainRef)
        {
            m_MainRef = mainRef;
        }
        public virtual void UpdateState(float deltaTime)
        {

        }
        public virtual void EnterState()
        {

        }
        public virtual void ExitState()
        {

        }
    }

    class GameState_PlayersPickMenu : GameState_Virtual
    {
        public override void UpdateState(float deltaTime)
        {

        }
        public override void EnterState()
        {
            m_MainRef.State_PlayerPickMenu_EnterState();
        }
        public override void ExitState()
        {
            m_MainRef.State_PlayerPickMenu_ExitState();
        }
    }

    class GameState_PreGame : GameState_Virtual
    {
        public override void UpdateState(float deltaTime)
        {
            m_MainRef.State_PreGame_Update(deltaTime);
        }
        public override void EnterState()
        {
            m_MainRef.State_PreGame_EnterState();
        }
        public override void ExitState()
        {
            m_MainRef.State_PreGame_ExitState();
        }
    }

    class GameState_InGame : GameState_Virtual
    {
        public override void UpdateState(float deltaTime)
        {
            m_MainRef.State_InGame_Update(deltaTime);
        }
        public override void EnterState()
        {
            m_MainRef.State_InGame_EnterState();
        }
        public override void ExitState()
        {
            m_MainRef.State_InGame_ExitState();
        }
    }

    class GameState_ShowScore : GameState_Virtual
    {
        public override void UpdateState(float deltaTime)
        {
            m_MainRef.State_ShowScore_Update(deltaTime);
        }
        public override void EnterState()
        {
            m_MainRef.State_ShowScore_EnterState();
        }
        public override void ExitState()
        {
            m_MainRef.State_ShowScore_ExitState();
        }
    }

    GameState_Virtual[] m_Array_GameStates = new GameState_Virtual[(int)GAME_STATE.MAX_NUM];

    void GameStates_InitState()
    {
        m_Array_GameStates[(int)GAME_STATE.STARTING] = new GameState_Virtual();
        m_Array_GameStates[(int)GAME_STATE.PLAYERS_PICK_MENU] = new GameState_PlayersPickMenu();
        m_Array_GameStates[(int)GAME_STATE.PRE_GAME] = new GameState_PreGame();
        m_Array_GameStates[(int)GAME_STATE.IN_GAME] = new GameState_InGame();
        m_Array_GameStates[(int)GAME_STATE.SHOW_SCORE] = new GameState_ShowScore();

        for (int i = 0; i < (int)GAME_STATE.MAX_NUM; ++i)
            m_Array_GameStates[i].Init(this);
    }

    public void GameStates_ChangeState(GAME_STATE newState)
    {
        m_Array_GameStates[m_CurrentGameState].ExitState();
        m_CurrentGameState = (int)newState;
        m_Array_GameStates[m_CurrentGameState].EnterState();
    }
    int m_CurrentGameState = (int)GAME_STATE.STARTING;
}
