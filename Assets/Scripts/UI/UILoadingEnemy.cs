using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadingEnemy : MonoBehaviour
{
    [SerializeField] private RectTransform m_Transform;
    [SerializeField] private float m_Speed = 35f;

    public System.Action<UILoadingEnemy> OnDestroyed;
    public System.Action<UILoadingEnemy> OnCollectorReached;

    private bool m_TargetSet = false;
    private Vector2 m_TargetPosition;
    private float m_SpeedModifier = 1f;

    public void SetTargetPosition(Vector2 targetPos)
    {
        m_TargetPosition = targetPos;
        m_TargetSet = true;
    }

    private void Update()
    {
        if (m_TargetSet)
        {
            Vector3 moveDirection = (m_TargetPosition - m_Transform.anchoredPosition).normalized;
            m_Transform.Translate(moveDirection * m_Speed * m_SpeedModifier * Time.deltaTime, UnityEngine.Space.World);
            m_Transform.up = ((m_Transform.position + moveDirection) - m_Transform.position).normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            if (collision.gameObject.tag == "UILoadingCollector")
            {
                if (OnCollectorReached != null)
                {
                    OnCollectorReached(this);
                }
            }
            else if (collision.gameObject.tag == "UILoadingProjectile")
            {
                Destroy(collision.gameObject);

                if (OnDestroyed != null)
                {
                    OnDestroyed(this);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            m_SpeedModifier = 0.1f;
        }
    }
}
