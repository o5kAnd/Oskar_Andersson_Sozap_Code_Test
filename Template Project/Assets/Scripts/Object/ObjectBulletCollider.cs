using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBulletCollider : MonoBehaviour
{
    // This Class handle the bullets collision and mostly destruction related functions

    public ObjectBulletMain m_MainScript;

    // The main collision check currently used, in linecast hit, check tag and proceed to specific collision function 
    public void LinecastCheck(Vector3 direction, Vector3 deltaMove)
    {
        Vector3 pos1 = transform.position;
        Vector3 pos2 = pos1 + deltaMove + (direction * 0.6f);

        var Coll = Physics2D.Linecast(pos2, pos1);
        if (Coll.collider != null)
        {
            GameObject obj = Coll.collider.gameObject;
            if (obj.tag == "Player")
                PlayerColliding(obj);
            else if (obj.tag == "Line")
                LineColliding(obj);
            else if (obj.tag == "Bullet")
                BulletColliding(obj);
        }
    }
    
    void PlayerColliding(GameObject player)
    {
        ObjectPlayerCollider playerScript = player.GetComponent<ObjectPlayerCollider>();
        if (playerScript != null)
        {
            if (playerScript.BulletColliding(m_MainScript.PlayerInfo_GetId()) == true)
            {
                m_MainScript.PlayerInfo_AddScore(GameSettings.BULLET_SCORE_AMOUNT_PER_SHIP_HIT);
                m_MainScript.Destroy();
            }  
        } 
    }
    void LineColliding(GameObject line)
    {
        if(GameSettings.BULLET_CAN_DESTROY_LINES == true)
        {
            ObjectLine lineScript = line.GetComponent<ObjectLine>();
            lineScript.DestroyLine(transform.position, true);
        }
        m_MainScript.Destroy();
    }

    void BulletColliding(GameObject bullet)
    {
        ObjectBulletCollider bulletScript = bullet.GetComponent<ObjectBulletCollider>();
        if(bulletScript != null)
        {
            if (bulletScript.m_MainScript.PlayerInfo_GetId() != m_MainScript.PlayerInfo_GetId())
            {
                m_MainScript.Destroy();
                bulletScript.m_MainScript.Destroy();
            }
        }
    }
}
