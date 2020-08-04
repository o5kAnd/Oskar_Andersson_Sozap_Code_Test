using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlayerCollider : MonoBehaviour
{

    public ObjectPlayerMain m_MainScript;



    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            GameObject obj = collider.gameObject;
            if (obj != null)
            {
                Debug.Log("tag " + obj.tag);
                if (obj.tag == "Line")
                    DestroyMainObjectAndCollider(obj);
            }
                
        }

    }

    void DestroyMainObjectAndCollider(GameObject colliderObject)
    {
        colliderObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
        m_MainScript.gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
    }


    /*void OnTriggerEnter2D(Collision2D col)
    {
        if (col == null)
            Debug.Log("Trigger null");
        else if (col.collider == null)
            Debug.Log("Trigger collider null");
        else
            Debug.Log("Trigger with: " + col.collider.name);

    }*/

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collide with: " + col.collider.name);

    }

}
