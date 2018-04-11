using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class UILoadingReticle : MonoBehaviour
{
    [SerializeField] private RectTransform m_Transform;
    [SerializeField] private GameObject m_ProjectilePrefab;
    [SerializeField] private GameObject m_Weapon;
    [SerializeField] private Image m_Image;
    [SerializeField] private float m_ReticleSpeed = 25f;
    [SerializeField] private float m_FollowSpeed = 5f;
    [SerializeField] private float m_MinSqrDist = 10f;
    [SerializeField] private float m_LaunchForce = 200f;
    [SerializeField] private int m_MaxTrailLength = 1;

    [SerializeField] private GameObject[] m_AmmoIcons;

    public System.Action OnCollected;
    public System.Action OnDelivered;
    public System.Action OnKilled;

    private List<RectTransform> m_CollectedObjects;
    private RectTransform m_Followee;

    private float m_Horizontal;
    private float m_Vertical;
    private int m_AmmoRemaining;
    private int m_MaxAmmo = 3;

    private void Awake()
    {
        m_CollectedObjects = new List<RectTransform>();

        m_AmmoRemaining = m_MaxAmmo;
    }

    private void Update()
    {
        for (int i = 0; i < m_CollectedObjects.Count; i++)
        {
            RectTransform follower = m_CollectedObjects[i];
            if (i == 0)
            {
                m_Followee = m_Transform;
            }
            else
            {
                m_Followee = m_CollectedObjects[i - 1];
            }

            Vector2 direction = (m_Followee.anchoredPosition - follower.anchoredPosition);
            if (direction.sqrMagnitude > m_MinSqrDist)
            {
                follower.anchoredPosition += direction * m_FollowSpeed * Time.deltaTime;
            }
        }
    }

    public bool OnInputUpdate(InputActionEventData data)
    {
        bool handled = false;
        switch (data.actionId)
        {
            case RewiredConsts.Action.Navigate_Horizontal:
                m_Horizontal = data.GetAxis();
                handled = true;
                break;

            case RewiredConsts.Action.Navigate_Vertical:
                m_Vertical = data.GetAxis();
                handled = true;
                break;

            case RewiredConsts.Action.Attack:
                if (m_CollectedObjects.Count == 0 && data.GetButtonDown())
                {
                    if (m_AmmoRemaining > 0)
                    {
                        Vector2 moveDir = new Vector2(m_Transform.up.x, m_Transform.up.y);
                        GameObject projObj = (GameObject)Instantiate(m_ProjectilePrefab, m_Transform.parent);
                        RectTransform projTransform = projObj.GetComponent<RectTransform>();
                        Vector2 launchPosition = m_Transform.anchoredPosition + moveDir * 15f;
                        projTransform.anchoredPosition = launchPosition;

                        // destroy after delay
                        Destroy(projObj, 5f);

                        Rigidbody2D projBody = projObj.GetComponent<Rigidbody2D>();
                        if (projBody != null)
                        {
                            projBody.AddForce(moveDir * m_LaunchForce, ForceMode2D.Impulse);
                        }
                        else
                        {
                            Debug.LogErrorFormat("Projectile has no rigidbody2D");
                        }

                        m_AmmoRemaining -= 1;
                        UpdateAmmoIcons();

                        handled = true;
                    }
                    else
                    {
                        // TODO
                        // sound effect?
                        Debug.LogFormat("> Out of Ammo");
                    }
                }
                break;
        }

        if (handled)
        {
            Vector3 moveDirection = (Vector3.up * m_Vertical + Vector3.right * m_Horizontal);
            if (moveDirection != Vector3.zero)
            {
                m_Transform.Translate(moveDirection * m_ReticleSpeed * Time.deltaTime, UnityEngine.Space.World);
                m_Transform.up = ((m_Transform.position + moveDirection) - m_Transform.position).normalized;
            }
        }

        return handled;
    }

    private void UpdateAmmoIcons()
    {
        for (int i = 0; i < m_MaxAmmo; i++)
        {
            m_AmmoIcons[i].SetActive(m_AmmoRemaining > i);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            if (collision.gameObject.tag == "UILoadingCollectible")
            {
                if (m_CollectedObjects.Count < m_MaxTrailLength)
                {
                    RectTransform rT = collision.gameObject.GetComponent<RectTransform>();
                    m_CollectedObjects.Add(rT);

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
            else if (collision.gameObject.tag == "UILoadingCollector")
            {
                m_AmmoRemaining = m_MaxAmmo;
                UpdateAmmoIcons();

                if (m_CollectedObjects.Count > 0)
                {
                    for (int i = m_CollectedObjects.Count - 1; i >= 0 ; i--)
                    {
                        // should change this so that it 'follows' to the middle of the base before getting destroyed
                        RectTransform rT = m_CollectedObjects[i];
                        m_CollectedObjects.Remove(rT);

                        Destroy(rT.gameObject);

                        if (OnDelivered != null)
                        {
                            OnDelivered();
                        }
                    }
                }
            }
            else if (collision.gameObject.tag == "UILoadingEnemy")
            {
                m_AmmoRemaining = m_MaxAmmo;
                UpdateAmmoIcons();

                for (int i = m_CollectedObjects.Count - 1; i >= 0; i--)
                {
                    RectTransform rT = m_CollectedObjects[i];
                    m_CollectedObjects.Remove(rT);

                    Destroy(rT.gameObject);
                }

                if (OnKilled != null)
                {
                    OnKilled();
                }
            }
        }
    }
}
