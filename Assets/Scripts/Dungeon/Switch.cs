using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO this all needs to be put into it's own class

public interface ISwitchable
{
    void OnSwitchPressed();
}

public interface IInteractable
{
    void Interact();
}

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject m_SwitchableObject;

    public void Setup(Transform location)
    {
        Instantiate(m_SwitchableObject, location.position, location.rotation);
    }

    public void Interact()
    {
        m_SwitchableObject.GetComponent<ISwitchable>().OnSwitchPressed();
    }
}
