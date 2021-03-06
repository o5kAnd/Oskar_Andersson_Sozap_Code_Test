﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBulletMain : MonoBehaviour
{
    //--- This main class for Bullets, contains most of the bullet related functions except collision
    public ObjectBulletCollider m_ColliderScript;

    GameMain.PlayerInfo m_PlayerInfo_BulletOwner;
    Vector3 m_MoveDirection;
    Vector3 m_PrevPosition;
    int m_BulletsListId;

    // match if list ID is equal to that of bullets playerInfo
    public int PlayerInfo_GetId() { return m_PlayerInfo_BulletOwner.listId; }
    public void PlayerInfo_AddScore(int scoreAmount) { m_PlayerInfo_BulletOwner.AddScore(scoreAmount); }
    public bool PlayerInfo_MatchIds(int listId) { return m_PlayerInfo_BulletOwner.listId == listId ? true : false; }

    public void SetBulletId(int id) { m_BulletsListId = id; }
    public void Init(Vector3 spawnPos, Vector3 moveDirection, int listId, GameMain.PlayerInfo owner)
    {
        m_BulletsListId = listId;
        transform.position = spawnPos;
        m_MoveDirection = moveDirection;
        m_PlayerInfo_BulletOwner = owner;
    }

    // Runs from ObjectManager
    public void InGame_Update(float deltaTime)
    {
        UpdateMovementAndPerformCollisionCheck(deltaTime);
    }

    void UpdateMovementAndPerformCollisionCheck(float deltaTime)
    {
        Vector3 deltaMove = m_MoveDirection * deltaTime * GameSettings.BULLET_SPEED_PER_SEC;
        m_ColliderScript.LinecastCheck(m_MoveDirection, deltaMove);
        m_PrevPosition = transform.position;
        transform.position += deltaMove;
    }

    // removal of game object should go through this function
    // "removeBulletFromBulletList" should be true except then you manually is handling the Bullet list in ObjectManager
    public void Destroy(bool removeBulletFromBulletList = true)
    {
        if (removeBulletFromBulletList == true)
            ObjectManager.GetObjectManager().Bullet_RemoveBulletFromList(m_BulletsListId);
        this.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
    }



}
