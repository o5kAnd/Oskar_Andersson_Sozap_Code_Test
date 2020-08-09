using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // main responsibility is handling players ships and bullets
    // Prefab section
    public GameObject Prefab_ObjectPlayer;
    public GameObject Prefab_ObjectBullet;
    // self pointer for easy access
    private static ObjectManager SelfPointer;

    // parent object for all line, easy quickly delete all of them
    GameObject m_LineHolderObject;
    static public ObjectManager GetObjectManager(){ return SelfPointer; }

    List<ObjectPlayerMain> m_List_ObjectPlayerShips = new List<ObjectPlayerMain>();
    List<ObjectBulletMain> m_List_ObjectsBullets = new List<ObjectBulletMain>();

    void Awake()
    {
        SelfPointer = this;
    }

    // --- Delete all lines -------
    public void CleanLineHolder()
    {
        if(m_LineHolderObject != null)
        {
            m_LineHolderObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
            m_LineHolderObject = null;
        }
        m_LineHolderObject = new GameObject();
        m_LineHolderObject.name = "LineHolder";
        m_LineHolderObject.transform.position = Vector3.zero;
    }


    //---- Ship specific functions ------------
    // The approach with bullets is the expectation that it might by many at the same time, 
    // so objects when created or deleted is handling so the list just adds and removes at the end, 
    public void Ship_SetIconVisibility(bool isVisible_InvinvibilityIcon, bool isVisible_ShotIcon, bool isVisible_TextIcon)
    {
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].SetExtraIconsVisibility(isVisible_InvinvibilityIcon, isVisible_ShotIcon, isVisible_TextIcon);
    }

    public void Ship_SetDrawingActivity(bool isActive)
    {
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].Lines_SetLineDrawingActivation(isActive);
    }
    // run this early in update so no attempt is made to access a null ship
    void Ship_CleanObjectListFromNull()
    {
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
        {
            if (m_List_ObjectPlayerShips[i] == null)
            {
                m_List_ObjectPlayerShips.RemoveAt(i);
                --i;
            }
        }
    }

    // This function is maybe a bit more expensive then necessary, but it is also short and multi purpose, and it does not run in "in-game" mode.
    public void Ship_SynchToPlayerNum(int numPlayers)
    {
        Ship_CleanObjectListFromNull();
        CleanLineHolder();
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].Destroy();
        m_List_ObjectPlayerShips.Clear();

        for(int i = 0; i < numPlayers; ++i)
        {
            GameObject obj = Instantiate(Prefab_ObjectPlayer, Vector2.zero, Quaternion.identity);
            ObjectPlayerMain s = obj.GetComponent<ObjectPlayerMain>();
            s.Init(GameMain.GetGameMain().GetPlayerInfo(i), m_LineHolderObject);
            s.SetExtraIconsVisibility(false, false, false);
            m_List_ObjectPlayerShips.Add(s);
        }
    }

    //---------------------------------------------------


    //---- Updates, those runs from GameMain ---------------
    public void InGame_Update(float deltaTime)
    {
        Ship_CleanObjectListFromNull();
        for(int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
             m_List_ObjectPlayerShips[i].InGame_Update(deltaTime);

        for (int i = 0; i < m_List_ObjectsBullets.Count; ++i)
            m_List_ObjectsBullets[i].InGame_Update(deltaTime);

        DestroyAllObjectsOutsideCameraView();
    }

    public void PreGame_Update(float deltaTime)
    {
        Ship_CleanObjectListFromNull();
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].PreGame_Update(deltaTime);
    }
    //---------------------------------------------------


    // ---- used for checking for a winning player -----------
    public int CheckWinner_GetNumbersOfShipsLeft()   {   return m_List_ObjectPlayerShips.Count;  }
    public int CheckWinner_GetPlayerIdFromShip(int shipId) { return m_List_ObjectPlayerShips[shipId].PlayerInfo_GetId(); }



    //---- Functions for cleaning upp objects outside the screen -----------------
    void DestroyAllObjectsOutsideCameraView()
    {
        // remove all ships outside
        for(int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
        {
            if (CheckIfPositionIsOutsideCameraView(m_List_ObjectPlayerShips[i].transform.position) == true)
                m_List_ObjectPlayerShips[i].Destroy();
        }

        // remove all bullets outside
        for (int i = 0; i < m_List_ObjectsBullets.Count; ++i)
        {
            if (CheckIfPositionIsOutsideCameraView(m_List_ObjectsBullets[i].transform.position) == true)
            {
                m_List_ObjectsBullets[i].Destroy();
                --i;
            }
        }
    }
    bool CheckIfPositionIsOutsideCameraView(Vector3 objectsWorldPosition)
    {
        float MinX = 0;
        float MinY = 0;
        float MaxX = Screen.width;
        float MaxY = Screen.height;
        Vector2 ScreenPos = Camera.main.WorldToScreenPoint(objectsWorldPosition);
        if (ScreenPos.x < MinX || ScreenPos.y < MinY || ScreenPos.x > MaxX || ScreenPos.y > MaxY)
            return true;
        return false;
    }
    //---------------------------------------------------


    //---- Bullet specific functions -----------------
    // The approach with bullets is the expectation that it might by many at the same time, 
    //  so objects when created or deleted is handling so the list just adds and removes at the end, 
    public void Bullet_RequestBulletSpawn(Vector3 spawnPos, Vector3 moveDirection, GameMain.PlayerInfo owner)
    {
        ObjectBulletMain bullet = Instantiate(Prefab_ObjectBullet, Vector3.zero, Quaternion.identity).GetComponent<ObjectBulletMain>();
        bullet.Init(spawnPos, moveDirection, m_List_ObjectsBullets.Count, owner);
        m_List_ObjectsBullets.Add(bullet);
    }

    // runs from bullets main script
    // exchange list position, so the bullet thats to be removed is last in the list 
    //(removal cost will therefore be low because the list wont have to adjust all positions after)
    public void Bullet_RemoveBulletFromList(int bulletListId)
    {
        int lastId = m_List_ObjectsBullets.Count - 1;
        m_List_ObjectsBullets[lastId].SetBulletId(bulletListId);
        m_List_ObjectsBullets[bulletListId] = m_List_ObjectsBullets[lastId];
        m_List_ObjectsBullets.RemoveAt(lastId);
    }

    public void Bullet_RemoveAllBullets()
    {
        for (int i = 0; i < m_List_ObjectsBullets.Count; ++i)
            m_List_ObjectsBullets[i].Destroy(false);
        m_List_ObjectsBullets.Clear();
    }
    //---------------------------------------------------


}
