using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceCallout : MonoBehaviour
{
    [SerializeField] private Image m_CalloutIcon;
    [SerializeField] private Text m_TextLabel;

    [SerializeField] private Transform m_Transform;

    private Transform m_CameraTransform;

    private void Awake()
    {
        m_CameraTransform = Camera.main.transform;
    }

    public void Setup(Sprite icon, string text)
    {
        m_CalloutIcon.sprite = icon;
        m_TextLabel.text = text;

        // hide it initially
        Show(false);
    }

    public void Show(bool show)
    {
        m_Transform.gameObject.SetActive(show);
    }

    private void LateUpdate()
    {
        if (m_Transform != null && m_Transform.gameObject.activeInHierarchy)
        {
            m_Transform.LookAt(m_Transform.position + m_CameraTransform.rotation * Vector3.forward, m_CameraTransform.rotation * Vector3.up);
        }
    }
}
