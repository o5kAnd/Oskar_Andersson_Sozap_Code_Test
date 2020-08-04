using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayerMain : MonoBehaviour
{
    public GameObject Prefab_ObjectBullet;
    public GameObject Prefab_Line;


    Color m_ShipsColor;
    public SpriteRenderer m_Renderer_ShipsColorPart;
    public SpriteRenderer m_Renderer_ShipsMainPart;
    public ObjectPlayerCollider m_ColliderScript;

    public SpriteRenderer m_Renderer_InvincibilityIcon;
    public SpriteRenderer m_Renderer_ShotIcon;
    public TextMesh m_Text_ScoreIcon;

    public GameObject m_GameObj_RotateBody;

    Vector3 CurrentDir = new Vector3(1.0f, 0.0f, 0.0f);


    public void Init(Color shipsColor, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey)
    {
        Keys_SetRepresentation(upKey, downKey, leftKey, rightKey);

        m_ShipsColor = shipsColor;
        m_Renderer_ShipsColorPart.color = m_ShipsColor;
        Lines_NewCreateLine();
    }




    public void IngameUpdate(float deltaTime)
    {
        Lines_CheckLineTimers();
        Lines_UpdateTimers(deltaTime);

        transform.position += CurrentDir * deltaTime * SHIP_SPEED_PER_SEC_FARWARD;
        UpdateRotation(deltaTime);

    }






    void UpdateRotation(float deltaTime)
    {
        float rotation = 0.0f;
        if (Keys_GetKeyHeld(KEY_REPRESENTATION.RIGHT))
            rotation = -deltaTime * SHIP_SPEED_PER_SEC_ROTATE;
        else if (Keys_GetKeyHeld(KEY_REPRESENTATION.LEFT))
            rotation = deltaTime * SHIP_SPEED_PER_SEC_ROTATE;

        if (rotation != 0.0f)
        {
            CurrentDir = GetRotatedVector(CurrentDir, rotation);
            m_GameObj_RotateBody.transform.Rotate(0.0f, 0.0f, rotation);
        }
    }

    const float DEGREE_TO_RADIANS = Mathf.PI / 180.0f;
    public static Vector2 GetRotatedVector(Vector2 vec, float rotateDegree)
    {
        vec.Normalize();

        float rotateRadians = DEGREE_TO_RADIANS * rotateDegree;

        float tmpx = vec.x;
        float tmpy = vec.y;

        float x = Mathf.Cos(rotateRadians) * tmpx - Mathf.Sin(rotateRadians) * tmpy;
        float y = Mathf.Sin(rotateRadians) * tmpx + Mathf.Cos(rotateRadians) * tmpy;

        return new Vector2(x, y);
    }





    // Key Part
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




    //Invincibility
    const float INVINVIBILITY_TIME = 2.0f;
    const float INVINVIBILITY_COOLDOWN = INVINVIBILITY_TIME + 2.0f;
    bool Invincibility_IsInvincible = false;
    float Invincibility_Timer = 0.0f;
    void Invincibility_Update(float deltaTime)
    {
        if(Invincibility_Timer >= INVINVIBILITY_COOLDOWN)
        {
            if(Keys_GetKeyPressed(KEY_REPRESENTATION.DOWN) == true)
            {
                Invincibility_Timer = 0.0f;
                Invincibility_IsInvincible = true;
            }
        }
        else
        {
            Invincibility_Timer += deltaTime;
            if(Invincibility_Timer >= INVINVIBILITY_TIME)
            {
                Invincibility_IsInvincible = false;
            }
        }

    }

    public bool Invincibility_GetIfInvincible() { return Invincibility_IsInvincible; }






    void Lines_CheckLineTimers()
    {
        
        if (m_LineTimer_Current_UpdateLine <= 0.0f)
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

    const float SHIP_SPEED_PER_SEC_FARWARD = 2.0f;
    const float SHIP_SPEED_PER_SEC_ROTATE = 300.0f;
    const float LINE_TIMER_MAX = 1.0f;
    const float LINE_UPDATE_INTERVAL = 0.25f;
    float m_LineTimer_Current_NewLine = 0.0f;
    float m_LineTimer_Current_UpdateLine = 0.0f;



    public GameObject currentLine;

    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    public List<Vector2> m_List_objectPositions;


    void Lines_UpdateTimers(float deltaTime)
    {
        m_LineTimer_Current_NewLine -= deltaTime;
        m_LineTimer_Current_UpdateLine -= deltaTime;
    }

    void UpdateObjectPosition()
    {
        m_List_objectPositions.Add(m_GameObj_RotateBody.transform.position);
    }
    void Lines_NewCreateLine()
    {
        m_LineTimer_Current_NewLine = LINE_TIMER_MAX;

        currentLine = Instantiate(Prefab_Line, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        m_List_objectPositions.Clear();
        UpdateObjectPosition();
        UpdateObjectPosition();
        lineRenderer.SetPosition(0, m_List_objectPositions[0]);
        lineRenderer.SetPosition(1, m_List_objectPositions[1]);
        edgeCollider.points = m_List_objectPositions.ToArray();

        lineRenderer.startColor = m_ShipsColor;
        lineRenderer.endColor = m_ShipsColor;
        lineRenderer.material.color = m_ShipsColor;

    }

    void Lines_UpdateLine_AddNewPoint()
    {
        m_LineTimer_Current_UpdateLine = LINE_UPDATE_INTERVAL;

        UpdateObjectPosition();
        lineRenderer.positionCount++;
        //lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);
        edgeCollider.points = m_List_objectPositions.ToArray();
    }

    void Lines_UpdateLine_UpdateLatestPoint()
    {
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, m_GameObj_RotateBody.transform.position);
        
    }


    void Start()
    {
        Init(Color.red, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);
    }

    void Update()
    {
        IngameUpdate(Time.deltaTime);
    }





}
