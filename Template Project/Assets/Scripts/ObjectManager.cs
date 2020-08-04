using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject Prefab_ObjectPlayer;
    private static ObjectManager SelfPointer;
    static public ObjectManager GetObjectManager(){ return SelfPointer; }

    void Awake()
    {
        SelfPointer = this;
    }

    // runs from GameMain
    public void Init()
    {

    }

    // runs from GameMain
    public void InGame_Update(float deltaTime)
    {
        
    }
}
