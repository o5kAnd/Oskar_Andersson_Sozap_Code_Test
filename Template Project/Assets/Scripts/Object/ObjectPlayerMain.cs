using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ObjectPlayerMain : MonoBehaviour
{
    public GameObject Prefab_Line;


    Color m_ShipsColor;
    public SpriteRenderer m_Renderer_ShipsColorPart;
    public SpriteRenderer m_Renderer_ShipsMainPart;
    public ObjectPlayerCollider m_ColliderScript;

    public SpriteRenderer m_Renderer_InvincibilityIcon;
    public SpriteRenderer m_Renderer_ShotIcon;
    public Text m_Text_ScoreIcon;

    public GameObject m_GameObj_RotateBody;
    public GameObject m_LineHolderObject;

    Vector3 m_CurrentDir = new Vector3(1.0f, 0.0f, 0.0f);

    const float SHIP_SPEED_PER_SEC_FARWARD = 10.05f;

    GameMain.PlayerInfo m_PlayerInfo_ShipOwner;
    public int PlayerInfo_GetId() { return m_PlayerInfo_ShipOwner.listId; }
    public bool PlayerInfo_MatchIds(int listId) { return m_PlayerInfo_ShipOwner.listId == listId ? true : false; }

    public void Init(GameMain.PlayerInfo playerInfo, GameObject lineObjectHolder)
    {
        this.gameObject.name = playerInfo.name + "'s Ship";
        m_LineHolderObject = lineObjectHolder;
        m_PlayerInfo_ShipOwner = playerInfo;
        Keys_SetRepresentation(m_PlayerInfo_ShipOwner.upKey, m_PlayerInfo_ShipOwner.downKey, m_PlayerInfo_ShipOwner.leftKey, m_PlayerInfo_ShipOwner.rightKey);

        m_ShipsColor = m_PlayerInfo_ShipOwner.color;
        m_Renderer_ShipsColorPart.color = m_ShipsColor;
        transform.position = m_PlayerInfo_ShipOwner.position;
        Lines_NewCreateLine();
    }
    
    public void SetExtraIconsVisibility(bool isVisible)
    {
        m_Renderer_InvincibilityIcon.gameObject.SetActive(isVisible);
        m_Renderer_ShotIcon.gameObject.SetActive(isVisible);
        m_Text_ScoreIcon.transform.parent.gameObject.SetActive(isVisible);
    }

    // removal of gameobject should go through this function
    public void Destroy(float timeUntilDestruction = 0.0f)
    {
        this.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction(timeUntilDestruction);
    }


    public void PreGame_Update(float deltaTime)
    {
        Rotation_Update(deltaTime);
    }

    public void InGame_Update(float deltaTime)
    {
        Lines_CheckLineTimers();
        Lines_UpdateTimers(deltaTime);

        Vector3 deltaMove = m_CurrentDir * deltaTime * SHIP_SPEED_PER_SEC_FARWARD;
        m_ColliderScript.LinecastCheck(m_CurrentDir, deltaMove);
        transform.position += deltaMove;
        Rotation_Update(deltaTime);

        Invincibility_Update(deltaTime);
        Blinking_UpdateBlinking(deltaTime);

        BulletSpawn_Update(deltaTime);
    }






    //---------------------------------------------------------------
    // ----------- ROTATION PART -----------

    const float ROTATION_MAX_SPEED_PER_SEC = 200.0f;
    const float ROTATION_INCREASE_SPEED_PER_SEC = 300.0f;
    const float ROTATION_DECREASE_SPEED_PER_SEC = -400.0f;

    float Rotation_LeftRotationForce = 0.0f;
    float Rotation_RightRotationForce = 0.0f;

    float Rotation_PreviousValue = 0;
    float Rotation_UpdateRotationSpeedOnSpecific(float currentRotationForce, float increaseInRotationForce)
    {
        currentRotationForce += increaseInRotationForce;
        if (currentRotationForce > ROTATION_MAX_SPEED_PER_SEC)
            currentRotationForce = ROTATION_MAX_SPEED_PER_SEC;
        else if (currentRotationForce < 0.0f)
            currentRotationForce = 0.0f;
        return currentRotationForce;
    }

    void Rotation_Update(float deltaTime)
    {
        float rotation = 0.0f;
        if (Keys_GetKeyHeld(KEY_REPRESENTATION.RIGHT))
            Rotation_RightRotationForce = Rotation_UpdateRotationSpeedOnSpecific(Rotation_RightRotationForce, ROTATION_INCREASE_SPEED_PER_SEC * deltaTime);
        else
            Rotation_RightRotationForce = Rotation_UpdateRotationSpeedOnSpecific(Rotation_RightRotationForce, ROTATION_DECREASE_SPEED_PER_SEC * deltaTime);

        if (Keys_GetKeyHeld(KEY_REPRESENTATION.LEFT))
            Rotation_LeftRotationForce = Rotation_UpdateRotationSpeedOnSpecific(Rotation_LeftRotationForce, ROTATION_INCREASE_SPEED_PER_SEC * deltaTime);
        else
            Rotation_LeftRotationForce = Rotation_UpdateRotationSpeedOnSpecific(Rotation_LeftRotationForce, ROTATION_DECREASE_SPEED_PER_SEC * deltaTime);

        rotation = (Rotation_LeftRotationForce - Rotation_RightRotationForce) * deltaTime;

        Rotation_PreviousValue = m_GameObj_RotateBody.transform.rotation.z;
        if (rotation != 0.0f)
        {
            m_CurrentDir = Rotated_GetVector(m_CurrentDir, rotation);
            m_GameObj_RotateBody.transform.Rotate(0.0f, 0.0f, rotation);
        }
    }

    const float DEGREE_TO_RADIANS = Mathf.PI / 180.0f;
    public static Vector2 Rotated_GetVector(Vector2 vec, float rotateDegree)
    {
        vec.Normalize();

        float rotateRadians = DEGREE_TO_RADIANS * rotateDegree;

        float tmpx = vec.x;
        float tmpy = vec.y;

        float x = Mathf.Cos(rotateRadians) * tmpx - Mathf.Sin(rotateRadians) * tmpy;
        float y = Mathf.Sin(rotateRadians) * tmpx + Mathf.Cos(rotateRadians) * tmpy;

        return new Vector2(x, y);
    }

    // used to detect if ship is heading in a straight course or is rotating
    bool Rotation_CheckIfSameAngleSinceLastUpdate() { return (Rotation_PreviousValue == m_GameObj_RotateBody.transform.rotation.z ? true : false); }

    //---------------------------------------------------------------




    //---------------------------------------------------------------
    // ----------- KEY PART -----------
    enum KEY_REPRESENTATION { UP, DOWN, LEFT, RIGHT, MAX_NUM}
    KeyCode[] m_Arr_KeyCodes = new KeyCode[(int)KEY_REPRESENTATION.MAX_NUM];
    //int[] m_Arr_KeyStates = new int[(int)KEY_REPRESENTATION.MAX_NUM];
    //int[] m_Arr_KeyStates_Prev = new int[(int)KEY_REPRESENTATION.MAX_NUM];

    bool Keys_GetKeyHeld(KEY_REPRESENTATION key)    {   return Input.GetKey(m_Arr_KeyCodes[(int)key]);   }
    bool Keys_GetKeyPressed(KEY_REPRESENTATION key) { return Input.GetKeyDown(m_Arr_KeyCodes[(int)key]); }
    bool Keys_GetKeyReleased(KEY_REPRESENTATION key) { return Input.GetKeyUp(m_Arr_KeyCodes[(int)key]); }
    void Keys_SetRepresentation(KeyCode upKey, KeyCode downKey, KeyCode leftKey,KeyCode rightKey)
    {
        m_Arr_KeyCodes[(int)KEY_REPRESENTATION.UP] = upKey;
        m_Arr_KeyCodes[(int)KEY_REPRESENTATION.DOWN] = downKey;
        m_Arr_KeyCodes[(int)KEY_REPRESENTATION.LEFT] = leftKey;
        m_Arr_KeyCodes[(int)KEY_REPRESENTATION.RIGHT] = rightKey;
    }

    //---------------------------------------------------------------






    //---------------------------------------------------------------
    // ----------- BULLET SECTION -----------
    const float BULLET_SPAWN_COOLDOWN = 2.0f;
    float m_BulletSpawn_Timer = BULLET_SPAWN_COOLDOWN;
    void BulletSpawn_Update(float deltaTime)
    {
        if (m_BulletSpawn_Timer >= BULLET_SPAWN_COOLDOWN)
        {
            m_Renderer_ShotIcon.gameObject.SetActive(true);
            if (Keys_GetKeyPressed(KEY_REPRESENTATION.UP))
            {
                m_BulletSpawn_Timer = 0.0f;
                // bullets spawn pos is ships pos + bullet radius + the position the ship will be in next frame
                Vector3 BulletSpawnPos = transform.position + (m_CurrentDir * 0.5f) + (m_CurrentDir * deltaTime * SHIP_SPEED_PER_SEC_FARWARD);
                ObjectManager.GetObjectManager().Bullet_RequestBulletSpawn(transform.position, m_CurrentDir, m_PlayerInfo_ShipOwner);
            }
        }
        else
        {
            m_Renderer_ShotIcon.gameObject.SetActive(false);
            m_BulletSpawn_Timer += deltaTime;
        }
    }

    //---------------------------------------------------------------







    //---------------------------------------------------------------
    // ----------- INVINVIBILITY SECTION -----------
    const float INVINVIBILITY_TIME = 3.0f;
    const float INVINVIBILITY_COOLDOWN = INVINVIBILITY_TIME + 1.0f;
    bool m_Invincibility_IsInvincible = false;
    float m_Invincibility_Timer = INVINVIBILITY_COOLDOWN;
    void Invincibility_Update(float deltaTime)
    {
        if (m_Invincibility_Timer >= INVINVIBILITY_COOLDOWN)
        {
            m_Renderer_InvincibilityIcon.gameObject.SetActive(true);
            if(Keys_GetKeyPressed(KEY_REPRESENTATION.DOWN) == true)
            {
                m_Invincibility_Timer = 0.0f;
                m_Invincibility_IsInvincible = true;

                Blinking_StartBlinking(INVINVIBILITY_TIME, 2, 0.4f);
            }
        }
        else
        {
            m_Renderer_InvincibilityIcon.gameObject.SetActive(false);
            m_Invincibility_Timer += deltaTime;
            if (m_Invincibility_Timer >= INVINVIBILITY_TIME)
            {
                m_Invincibility_IsInvincible = false;
            }
        }
    }

    public bool Invincibility_GetIfInvincible() { return m_Invincibility_IsInvincible; }

    //---------------------------------------------------------------






    //---------------------------------------------------------------
    // ----------- BLINKING SECTION -----------
    bool m_Blinking_IsBlinking = false;
    float m_Blinking_TotalTime = 0.0f;
    float m_Blinking_Intensity = 0.0f;
    float m_Blinking_Speed = 0.0f;
    float m_Blinking_CurrentTime = 0.0f;

    float m_Blinking_BlinkTime = 0.0f;
    float m_Blinking_BlinkTimeFactor = 1.0f;
    void Blinking_StartBlinking(float blinkingTotalTime, int numberOfBlinks, float blinkingIntensity)
    {
        Blinking_ResetValues();
        m_Blinking_IsBlinking = true;
        m_Blinking_TotalTime = blinkingTotalTime;
        m_Blinking_Intensity = blinkingIntensity;

        m_Blinking_Speed = (float)(numberOfBlinks * 2) / blinkingTotalTime;
    }

    void Blinking_ResetValues()
    {
        m_Blinking_IsBlinking = false;
        m_Blinking_TotalTime = 0.0f;
        m_Blinking_Intensity = 0.0f;
        m_Blinking_Speed = 0.0f;
        m_Blinking_CurrentTime = 0.0f;

        m_Blinking_BlinkTime = 0.0f;
        m_Blinking_BlinkTimeFactor = 1.0f;
        Blinking_SetAlphaOnShipParts(1.0f);
    }

    void Blinking_UpdateBlinking(float deltaTime)
    {
        if(m_Blinking_IsBlinking == true)
        {
            if (m_Blinking_CurrentTime >= m_Blinking_TotalTime)
                Blinking_ResetValues();
            else
            {
                m_Blinking_CurrentTime += deltaTime;
                Blinking_SetAlphaOnShipParts(1.0f - (m_Blinking_BlinkTime * m_Blinking_Intensity));

                m_Blinking_BlinkTime += deltaTime * m_Blinking_Speed * m_Blinking_BlinkTimeFactor;
                if (m_Blinking_BlinkTime <= 0.0f)
                {
                    m_Blinking_BlinkTime = 0.0f;
                    m_Blinking_BlinkTimeFactor = 1.0f;
                }
                else if (m_Blinking_BlinkTime >= 1.0f)
                {
                    m_Blinking_BlinkTime = 1.0f;
                    m_Blinking_BlinkTimeFactor = -1.0f;
                }
            }
        }
    }

    void Blinking_SetAlphaOnShipParts(float alphaVal)
    {
        m_Renderer_ShipsColorPart.color = GetColorWithNewAlpha(m_Renderer_ShipsColorPart.color, alphaVal);
        m_Renderer_ShipsMainPart.color = GetColorWithNewAlpha(m_Renderer_ShipsMainPart.color, alphaVal);
    }

    //---------------------------------------------------------------


    Color GetColorWithNewAlpha(Color color, float alphaVal) {   return new Color(color.r, color.g, color.b, alphaVal); }





    void Lines_CheckLineTimers()
    {
        if (lineRenderer == null)
            Lines_NewCreateLine();
        
        // add points if timer <= 0, but only if the ship is rotating
        if (m_LineTimer_Current_UpdateLine <= 0.0f && Rotation_CheckIfSameAngleSinceLastUpdate() == false)
        {
            Lines_UpdateLine_AddNewPoint();
        }
        
        Lines_UpdateLine_UpdateLatestPoint();

        if (m_LineTimer_Current_NewLine <= 0.0f)
        {
            //Lines_UpdateLine_AddNewPoint();
            Lines_NewCreateLine();
        }

    }

    

    const float LINE_TIMER_MAX = 1.0f;
    const float LINE_UPDATE_INTERVAL = 1.0f/30.0f;
    float m_LineTimer_Current_NewLine = 0.0f;
    float m_LineTimer_Current_UpdateLine = 0.0f;


    public GameObject currentLine;

    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    public List<Vector2> m_List_objectPositions;

    Vector3 Lines_GetLineDrawPosition()
    {
        return m_GameObj_RotateBody.transform.position;
    }
    void Lines_UpdateTimers(float deltaTime)
    {
        m_LineTimer_Current_NewLine -= deltaTime;
        m_LineTimer_Current_UpdateLine -= deltaTime;
    }

    /*void UpdateObjectPosition()
    {
        m_List_objectPositions.Add(m_GameObj_RotateBody.transform.position);
    }*/
    void Lines_NewCreateLine()
    {
        m_LineTimer_Current_NewLine = LINE_TIMER_MAX;

        // get last point from previous line if it exists, it will be a good starting point, otherwise, use the ships rotating body
        Vector3 lineStartPos = Lines_GetLineDrawPosition();
        if (m_List_objectPositions.Count > 0)
            lineStartPos = m_List_objectPositions[m_List_objectPositions.Count - 1];
        m_List_objectPositions.Clear();
        m_List_objectPositions.Add(lineStartPos);
        m_List_objectPositions.Add(Lines_GetLineDrawPosition());

        currentLine = Instantiate(Prefab_Line, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        
        lineRenderer.SetPosition(0, m_List_objectPositions[0]);
        lineRenderer.SetPosition(1, m_List_objectPositions[1]);
        edgeCollider.points = m_List_objectPositions.ToArray();

        lineRenderer.startColor = m_ShipsColor;
        lineRenderer.endColor = m_ShipsColor;
        lineRenderer.material.color = m_ShipsColor;

        currentLine.gameObject.transform.SetParent(m_LineHolderObject.transform);
    }

    void Lines_UpdateLine_AddNewPoint()
    {
        m_LineTimer_Current_UpdateLine = LINE_UPDATE_INTERVAL;

        m_List_objectPositions.Add(Lines_GetLineDrawPosition());
        lineRenderer.positionCount++;
        //lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
        edgeCollider.points = m_List_objectPositions.ToArray();
    }

    void Lines_UpdateLine_UpdateLatestPoint()
    {
        m_List_objectPositions[m_List_objectPositions.Count - 1] = Lines_GetLineDrawPosition();

        lineRenderer.SetPosition(lineRenderer.positionCount - 1, m_List_objectPositions[m_List_objectPositions.Count - 1]);
        edgeCollider.points = m_List_objectPositions.ToArray();
        
    }







}
