using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionHandler : MonoBehaviour
{
    [SerializeField] private Space m_ParentSpace;

    private void OnCollisionEnter(Collision collision)
    {
        m_ParentSpace.OnFloorCollsionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        m_ParentSpace.OnFloorCollisionExit(collision);
    }
}
