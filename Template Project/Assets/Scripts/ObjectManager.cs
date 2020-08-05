using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject Prefab_ObjectPlayer;
    private static ObjectManager SelfPointer;

    GameObject m_LineHolderObject;
    static public ObjectManager GetObjectManager(){ return SelfPointer; }

    List<ObjectPlayerMain> m_List_ObjectPlayerShips = new List<ObjectPlayerMain>();

    void Awake()
    {
        SelfPointer = this;
    }

    // runs from GameMain
    public void Init()
    {

    }

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

    public void SetShipsIconVisibility(bool isVisible)
    {
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].SetExtraIconsVisibility(isVisible);
    }

    public void SynchPlayerShipsToPlayerNum(int numPlayers)
    {
        CleanObjectShipListFromNull();
        CleanLineHolder();
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].Destroy();
        m_List_ObjectPlayerShips.Clear();

        for(int i = 0; i < numPlayers; ++i)
        {
            GameObject obj = Instantiate(Prefab_ObjectPlayer, Vector2.zero, Quaternion.identity);
            ObjectPlayerMain s = obj.GetComponent<ObjectPlayerMain>();
            s.Init(GameMain.GetGameMain().GetPlayerInfo(i), m_LineHolderObject);
            s.SetExtraIconsVisibility(false);
            m_List_ObjectPlayerShips.Add(s);
        }
        
    }

    // runs from GameMain
    public void InGame_Update(float deltaTime)
    {
        CleanObjectShipListFromNull();
        for(int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
             m_List_ObjectPlayerShips[i].InGame_Update(deltaTime);
    }

    // runs from GameMain
    public void PreGame_Update(float deltaTime)
    {
        CleanObjectShipListFromNull();
        for (int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
            m_List_ObjectPlayerShips[i].PreGame_Update(deltaTime);
    }

    void CleanObjectShipListFromNull()
    {
        for(int i = 0; i < m_List_ObjectPlayerShips.Count; ++i)
        {
            if (m_List_ObjectPlayerShips[i] == null)
            {
                m_List_ObjectPlayerShips.RemoveAt(i);
                --i;
            }
        }
    }

}
