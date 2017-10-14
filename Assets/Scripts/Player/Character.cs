﻿using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    //[SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] private Transform m_Transform;
    [SerializeField] private Rigidbody m_Rigidbody;

    [SerializeField] private float m_MoveSpeed = 3f;


    protected virtual void Awake()
    {
       
    }

    protected virtual void OnDestroy()
    {
        
    }

    //protected virtual void SetDestination(Vector3 position)
    //{
    //    m_Agent.isStopped = false;
    //    m_Agent.SetDestination(position);

    //    //if (m_Agent.pathStatus == NavMeshPathStatus.PathPartial)
    //    //{
            
    //    //}
    //}

    //protected virtual void Stop()
    //{
    //    m_Agent.isStopped = true;
    //}

    protected virtual void Move(Vector3 direction)
    {
        m_Rigidbody.MovePosition(m_Transform.position + direction * m_MoveSpeed * Time.fixedDeltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation, rotation, Time.fixedDeltaTime * 10f));
        }
    }
}
