using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_Player : MonoBehaviour
{
    // The individual player element in player select

    // Parts of the UI that is assign in unity editor --------
    public Text m_Text_PlayerName_Display;
    
    public InputField m_InputField_PlayerName;

    public Text m_ButtonText_Up;
    public Image m_ButtonIcon_Up;

    public Text m_ButtonText_Down;
    public Image m_ButtonIcon_Down;

    public Text m_ButtonText_Left;
    public Image m_ButtonIcon_Left;

    public Text m_ButtonText_Right;
    public Image m_ButtonIcon_Right;
    // ----------

    static Vector3 SHIP_POSITION_OFFSET = new Vector3(-1.0f, 2.5f, 0.0f);

    GameMain.PlayerInfo m_PlayerInfo;
    public void Init(GameMain.PlayerInfo playerInfo, GameObject parentObj)
    {
        m_PlayerInfo = playerInfo;
        m_Text_PlayerName_Display.gameObject.SetActive(true);
        m_Text_PlayerName_Display.text = m_PlayerInfo.name;
        m_InputField_PlayerName.gameObject.SetActive(false);

        playerInfo.spawnPosition = Camera.main.ScreenToWorldPoint(transform.position) + SHIP_POSITION_OFFSET;
        //transform.position = Camera.main.ScreenToWorldPoint(new Vector3(playerInfo.position.x, playerInfo.position.y, 0.0f));
        //transform.SetParent(parentObj.transform);

        AssignKeys(playerInfo.upKey, playerInfo.downKey, playerInfo.leftKey, playerInfo.rightKey);
    }

    // Handles the visual representation of the keys each player uses, some keys will be translated to symbols.
    // If the key is not display as a symbol, it should max have one char, which in this case is defined as the last char in the string.
    public void AssignKeys(KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey)
    {
        if(upKey == KeyCode.UpArrow)
        {
            m_ButtonIcon_Up.gameObject.SetActive(true);
            m_ButtonText_Up.gameObject.SetActive(false);
        }
        else
        {
            m_ButtonText_Up.gameObject.SetActive(true);
            m_ButtonText_Up.text = GetLastCharFromString(upKey.ToString());
            m_ButtonIcon_Up.gameObject.SetActive(false);
        }


        if (downKey == KeyCode.DownArrow)
        {
            m_ButtonIcon_Down.gameObject.SetActive(true);
            m_ButtonText_Down.gameObject.SetActive(false);
        }
        else
        {
            m_ButtonText_Down.gameObject.SetActive(true);
            m_ButtonText_Down.text = GetLastCharFromString(downKey.ToString());
            m_ButtonIcon_Down.gameObject.SetActive(false);
        }

        if (leftKey == KeyCode.LeftArrow)
        {
            m_ButtonIcon_Left.gameObject.SetActive(true);
            m_ButtonText_Left.gameObject.SetActive(false);
        }
        else
        {
            m_ButtonText_Left.gameObject.SetActive(true);
            m_ButtonText_Left.text = GetLastCharFromString(leftKey.ToString());
            m_ButtonIcon_Left.gameObject.SetActive(false);
        }

        if (rightKey == KeyCode.RightArrow)
        {
            m_ButtonIcon_Right.gameObject.SetActive(true);
            m_ButtonText_Right.gameObject.SetActive(false);
        }
        else
        {
            m_ButtonText_Right.gameObject.SetActive(true);
            m_ButtonText_Right.text = GetLastCharFromString(rightKey.ToString());
            m_ButtonIcon_Right.gameObject.SetActive(false);
        }
    }

    string GetLastCharFromString(string text)
    {
        char[] arr = text.ToCharArray();
        if (arr.Length > 0)
            return "" + arr[arr.Length - 1];
        return "";
    }


    //---------------------------------------------------------------
    // ----------- BUTTON PRESS AND FIELD FUNCTIONS -----------
    public void ButtonPressed_ChangeName()
    {
        m_Text_PlayerName_Display.gameObject.SetActive(false);
        m_InputField_PlayerName.gameObject.SetActive(true);
        m_InputField_PlayerName.Select();
    }

    public void OnEndEdit_FieldDeselect(string edited)
    {
        m_Text_PlayerName_Display.text = edited;
        m_PlayerInfo.name = edited;
        m_Text_PlayerName_Display.gameObject.SetActive(true);
        m_InputField_PlayerName.gameObject.SetActive(false);
    }

}
