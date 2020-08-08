using UnityEngine;
using System.Collections;

public class SelfDestructionScript : MonoBehaviour
{
    // This function is inspired from my project, it adds an self destruct to an object, 
    //so it make sure that the object will actually be destroyed whenever it can,
    // it does also have a time function that lets one decide how long until destruction.
    bool m_SelfDestructionInited = false;
    float m_TimeUntilSelfDestruction = 0.0f;
    float m_CurrentTime = 0.0f;

    public void InitSelfDestruction(float timeUntilDestruction = 0.0f)
    {
        m_SelfDestructionInited = true;
        m_TimeUntilSelfDestruction = timeUntilDestruction;
        CheckSelfDestruction(0.0f);
    }

    void Update()
    {
        CheckSelfDestruction(Time.deltaTime);
    }

    void CheckSelfDestruction(float DeltaTime)
    {
        if (m_SelfDestructionInited == true)
        {
            if (m_CurrentTime >= m_TimeUntilSelfDestruction)
                Destroy(this.gameObject);
            else
                m_CurrentTime += DeltaTime;
        }
    }
}
