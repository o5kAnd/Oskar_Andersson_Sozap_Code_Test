using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLine : MonoBehaviour
{
    //This script is attached to each line segment, handles destruction of the segment and maybe on one of its neighbors
    public ObjectLine m_PreviousLine = null;
    public ObjectLine m_NextLineLine = null;

    public void DestroyLine(Vector3 colliderPos, bool checkNeighborDestruction)
    {
        if (checkNeighborDestruction == true)
            DestroyNearestNeighbour(colliderPos);
        gameObject.AddComponent<SelfDestructionScript>().InitSelfDestruction();
    }

    void DestroyNearestNeighbour(Vector3 colliderPos)
    {
        // destroy the nearest line neighbor based on the collision pos, this is done so that destruction of lines is more centered.
        //if on the edge, compare current line distance to its only neighbor and the neighbor with the colliders pos, if collider is closer then current line, destroy the neighbor, otherwise the collider is near the edge.
        // the position of a line segment is based on the mean position of its points
        if(m_PreviousLine != null && m_NextLineLine != null)
        {
            if (GetSquaredLength(colliderPos, GetLinesMeanPosition(m_PreviousLine.gameObject)) < GetSquaredLength(colliderPos, GetLinesMeanPosition(m_NextLineLine.gameObject)))
                m_PreviousLine.DestroyLine(colliderPos, false);
            else
                m_NextLineLine.DestroyLine(colliderPos, false);
        }
        else if(m_PreviousLine != null && m_NextLineLine == null)
        {
            if (GetSquaredLength(colliderPos, GetLinesMeanPosition(m_PreviousLine.gameObject)) < GetSquaredLength(GetLinesMeanPosition(gameObject), GetLinesMeanPosition(m_PreviousLine.gameObject)))
                m_PreviousLine.DestroyLine(colliderPos, false);
        }
        else if (m_PreviousLine == null && m_NextLineLine != null)
        {
            if (GetSquaredLength(colliderPos, GetLinesMeanPosition(m_NextLineLine.gameObject)) < GetSquaredLength(GetLinesMeanPosition(gameObject), GetLinesMeanPosition(m_NextLineLine.gameObject)))
                m_NextLineLine.DestroyLine(colliderPos, false);
        }
    }

    float GetSquaredLength(Vector3 pos1, Vector3 pos2)
    {
        Vector3 vec = pos1 - pos2;
        return Vector3.Dot(vec, vec);
    }

    Vector3 GetLinesMeanPosition(GameObject lineObject)
    {
        Vector3 returnVal = Vector3.zero;
        EdgeCollider2D col = lineObject.GetComponent<EdgeCollider2D>();
        if(col != null)
        {
            float f = 1.0f/(float)col.points.Length;
            Vector2 meanPos = Vector2.zero;
            for(int i = 0; i < col.points.Length; ++i)
                meanPos += col.points[i];
            returnVal = new Vector3(meanPos.x * f, meanPos.y * f, 0.0f);
        }
        return returnVal;
    }

}
