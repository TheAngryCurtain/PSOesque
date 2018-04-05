using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO replace with shader
public class TextureScroll : MonoBehaviour
{
    [SerializeField] private Renderer m_Renderer;

    public bool ScrollY = true;
    public bool ScrollX = false;
    public float ScrollRate = 1f;

    private Material m_Material;
    private Vector2 m_TextureOffset = Vector2.zero;

    private void Awake()
    {
        m_Material = m_Renderer.material;
    }

    private void LateUpdate()
    {
        if (m_Material)
        {
            if (ScrollY)
            {
                m_TextureOffset.y += Time.deltaTime * ScrollRate;
            }

            if (ScrollX)
            {
                m_TextureOffset.x += Time.deltaTime * ScrollRate;
            }

            m_Material.mainTextureOffset = m_TextureOffset;
        }
    }
}
