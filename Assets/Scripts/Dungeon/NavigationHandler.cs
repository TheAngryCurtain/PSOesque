using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationHandler : MonoBehaviour
{
    [SerializeField] private NavMeshSurface[] m_Surfaces;
    [SerializeField] private NavMeshLink[] m_Links;

    private void Start()
    {
        for (int i = 0; i < m_Surfaces.Length; i++)
        {
            m_Surfaces[i].BuildNavMesh();
        }

        for (int i = 0; i < m_Links.Length; i++)
        {
            m_Links[i].UpdateLink();
        }
    }
}
