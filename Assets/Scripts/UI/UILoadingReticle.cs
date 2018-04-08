using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingReticle : MonoBehaviour
{
    [SerializeField] private RectTransform m_Transform;
    [SerializeField] private Image m_Image;
    [SerializeField] private float m_FollowSpeed = 5f;
    [SerializeField] private float m_MinSqrDist = 10f;

    public System.Action OnCollected;

    private List<RectTransform> m_CollectedStars;
    private RectTransform m_Followee;

    private void Awake()
    {
        m_CollectedStars = new List<RectTransform>();
    }

    private void Update()
    {
        for (int i = 0; i < m_CollectedStars.Count; i++)
        {
            RectTransform follower = m_CollectedStars[i];
            if (i == 0)
            {
                m_Followee = m_Transform;
            }
            else
            {
                m_Followee = m_CollectedStars[i - 1];
            }

            Vector2 direction = (m_Followee.anchoredPosition - follower.anchoredPosition);
            if (direction.sqrMagnitude > m_MinSqrDist)
            {
                follower.anchoredPosition += direction * m_FollowSpeed * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI")) // probably should change this a special layer for this "mode"
        {
            RectTransform rT = collision.gameObject.GetComponent<RectTransform>();
            m_CollectedStars.Add(rT);

            Image image = rT.GetComponent<Image>();
            image.color = m_Image.color;

            CircleCollider2D collider = rT.GetComponent<CircleCollider2D>();
            Destroy(collider);

            if (OnCollected != null)
            {
                OnCollected();
            }
        }
    }
}
