using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // a class filled with constants that should be accessible from everywhere, 
    //the purpose of this class is an easy access to game play modifications such as ships speed and bullet destructibility.


    //---- Bullet settings
    public const bool BULLET_CAN_DESTROY_SHIPS = true;
    public const bool BULLET_CAN_DESTROY_LINES = true;
    public const int BULLET_SCORE_AMOUNT_PER_SHIP_HIT = 10;
    public const float BULLET_SPEED_PER_SEC = 25.0f;


    //---- Ship settings
    public const float SHIP_SPEED_PER_SEC_FARWARD = 10.0f;
    public const float SHIP_BULLET_SHOOTING_COOLDOWN_IN_SEC = 2.0f;
    public const float SHIP_INVINCIBILITY_TIME = 1.5f;
    public const float SHIP_INVINCIBILITY_COOLDOWN = SHIP_INVINCIBILITY_TIME + 4.0f;
    public const float SHIP_ROTATION_MAX_SPEED_PER_SEC = 200.0f;
    public const float SHIP_ROTATION_INCREASE_SPEED_PER_SEC = 400.0f;
    public const float SHIP_ROTATION_DECREASE_SPEED_PER_SEC = -400.0f;


    //---- General game play settings
    public const int NUMBER_OF_COUNT_DOWN_UNTIL_GAME_START = 5;
    public const float END_MATCH_TIME_BEFORE_SHOWING_SCORE = 0.3f;
    public const int SCORE_AMOUNT_PER_MATCH_ROUND_WIN = 30;
    public const int SCORE_AMOUNT_FOR_TOTAL_VICTORY = 150;


    //---- Player settings
    static Color PLAYER_ONE_COLOR = Color.red;      public static Color GetPlayerOneColor() { return PLAYER_ONE_COLOR; }
    public const KeyCode PLAYER_ONE_KEY_UP = KeyCode.W;
    public const KeyCode PLAYER_ONE_KEY_DOWN = KeyCode.S;
    public const KeyCode PLAYER_ONE_KEY_LEFT = KeyCode.A;
    public const KeyCode PLAYER_ONE_KEY_RIGHT = KeyCode.D;
    static Color PLAYER_TWO_COLOR = Color.green;    public static Color GetPlayerTwoColor() { return PLAYER_TWO_COLOR; }
    public const KeyCode PLAYER_TWO_KEY_UP = KeyCode.Y;
    public const KeyCode PLAYER_TWO_KEY_DOWN = KeyCode.H;
    public const KeyCode PLAYER_TWO_KEY_LEFT = KeyCode.G;
    public const KeyCode PLAYER_TWO_KEY_RIGHT = KeyCode.J;
    static Color PLAYER_THREE_COLOR = Color.yellow; public static Color GetPlayerThreeColor() { return PLAYER_THREE_COLOR; }
    public const KeyCode PLAYER_THREE_KEY_UP = KeyCode.UpArrow;
    public const KeyCode PLAYER_THREE_KEY_DOWN = KeyCode.DownArrow;
    public const KeyCode PLAYER_THREE_KEY_LEFT = KeyCode.LeftArrow;
    public const KeyCode PLAYER_THREE_KEY_RIGHT = KeyCode.RightArrow;
    static Color PLAYER_FOUR_COLOR = Color.blue;    public static Color GetPlayerFourColor() { return PLAYER_FOUR_COLOR; }
    public const KeyCode PLAYER_FOUR_KEY_UP = KeyCode.Keypad8;
    public const KeyCode PLAYER_FOUR_KEY_DOWN = KeyCode.Keypad5;
    public const KeyCode PLAYER_FOUR_KEY_LEFT = KeyCode.Keypad4;
    public const KeyCode PLAYER_FOUR_KEY_RIGHT = KeyCode.Keypad6;


}
