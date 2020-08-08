using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayerCollider : MonoBehaviour
{

    public ObjectPlayerMain m_MainScript;

    public void LinecastCheck(Vector3 direction, Vector3 deltaMove)
    {
        Vector3 extraMargin = direction * 0.5f;

        Vector3 pos1 = transform.position + (direction * 0.5f);
        Vector3 pos2 = pos1 + deltaMove + extraMargin;

        var Coll = Physics2D.Linecast(pos2, pos1);
        if (Coll.collider != null)
        {
            GameObject obj = Coll.collider.gameObject;
            //Debug.Log("LINECAST from Player: " + Coll.collider.name + ", tag: " + obj.tag);
            if (obj.tag == "Player")
                ShipColliding(obj);
            else if (obj.tag == "Line")
                LineColliding(obj, Coll.point);
        }    
    }

    /*
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            GameObject obj = collider.gameObject;
            if (obj != null)
            {
                //Debug.Log("tag " + obj.tag);
                if (obj.tag == "Line")
                    LineColliding(obj);
            }   
        }
    }
    */
    // Return true if colliding
    public bool BulletColliding(int bulletsPlayersId)
    {
        if (m_MainScript.PlayerInfo_MatchIds(bulletsPlayersId) == false && m_MainScript.Invincibility_GetIfInvincible() == false)
        {
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
