using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayerCollider : MonoBehaviour
{
    // This Class handle the Ships collision and mostly destruction related functions
    public ObjectPlayerMain m_MainScript;

    // The main collision check currently used, in linecast hit, check tag and proceed to specific collision function
    public void LinecastCheck(Vector3 direction, Vector3 deltaMove)
    {
        Vector3 extraMargin = direction * 0.5f;

        Vector3 pos1 = transform.position + (direction * 0.5f);
        Vector3 pos2 = pos1 + deltaMove + extraMargin;

        var Coll = Physics2D.Linecast(pos2, pos1);
        if (Coll.collider != null)
        {
            GameObject obj = Coll.collider.gameObject;
            if (obj.tag == "Player")
                ShipColliding(obj);
            else if (obj.tag == "Line")
                LineColliding(obj, Coll.point);
        }    
    }

    // currently used in the bullets collision class, returns true if colliding
    public bool BulletColliding(int bulletsPlayersId)
    {
        if (m_MainScript.PlayerInfo_MatchIds(bulletsPlayersId) == false && m_MainScript.Invincibility_GetIfInvincible() == false)
        {
            if(GameSettings.BULLET_CAN_DESTROY_SHIPS == true)
                m_MainScript.Destroy();
            return true;
        }
        return false;
    }

    void LineColliding(GameObject line, Vector2 collisionPosition)
    {
        if(m_MainScript.Invincibility_GetIfInvincible() == false)
        {
            ObjectLine lineScript = line.GetComponent<ObjectLine>();
            lineScript.DestroyLine(new Vector3(collisionPosition.x, collisionPosition.y, 0.0f), true);
            m_MainScript.Destroy();
        }
    }

    void ShipColliding(GameObject ship)
    {
        if (m_MainScript.Invincibility_GetIfInvincible() == false)
        {
            ObjectPlayerCollider shipcScript = ship.GetComponent<ObjectPlayerCollider>();
            if (shipcScript != null)
            {
                if (shipcScript.m_MainScript.PlayerInfo_GetId() != m_MainScript.PlayerInfo_GetId())
                {
                    shipcScript.m_MainScript.Destroy();
                    m_MainScript.Destroy();
                }
            }
        }
    }



}
