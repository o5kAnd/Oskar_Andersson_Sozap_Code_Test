using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE{STARTING, MENY, IN_GAME, MAX_NUM}
public class GameMain : MonoBehaviour
{

    int m_CurrentGameState = (int)GAME_STATE.STARTING;

    private static GameMain SelfPointer;
    static public GameMain GetGameMain() {   return SelfPointer; }

    void Awake()
    {
        SelfPointer = this;
    }
    void Start()
    {
        // Init all managers
        ObjectManager.GetObjectManager().Init();
    }

    void Update()
    {
        raycastTest();
        float deltaTime = Time.deltaTime;
        if (m_CurrentGameState == (int)GAME_STATE.IN_GAME)
            InGame_Update(deltaTime);
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
    }

    void InGame_Update(float deltaTime)
    {
        ObjectManager.GetObjectManager().InGame_Update(deltaTime);
    }


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
        float l = Mathf.Sqrt( Vector2.Dot(rayDir, rayDir) );
        rayDir = rayDir.normalized;
        RaycastHit2D raycastHit = Physics2D.Raycast(RayStart, rayDir, l);

        Debug.DrawLine(RayStart, RayEnd, Color.blue);
        Debug.DrawRay(RayStart, rayDir * l, Color.red);

        if (raycastHit.collider != null)
            Debug.Log("RAYCAST: " + raycastHit.collider.name);


        var Coll = Physics2D.Linecast(RayStart, RayEnd);
		if(Coll.collider != null)
            Debug.Log("LINECAST: " + Coll.collider.name);
    }



}
