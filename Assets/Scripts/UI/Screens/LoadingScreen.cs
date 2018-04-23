using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.UI;
using Rewired;

public class LoadingScreen : UIBaseScreen
{
    [SerializeField] private Transform m_ReticleTransform;
    [SerializeField] private float m_MoveSpeed = 8f;

    private UI.Enums.ScreenId m_PostLoadScreenId;
    private float m_Horizontal;
    private float m_Vertical;

    private float m_MinLoadTime = 1f;
    private float m_LoadStartTime = 0f;
    private float m_TotalLoadTime = 0f;

    public override void Initialize(object[] screenParams)
    {
        base.Initialize(screenParams);
        m_PostLoadScreenId = (UI.Enums.ScreenId)screenParams[0];

        if (screenParams.Length > 1)
        {
            // check for an scene to load while in this screen
            Enums.eScene scene = (Enums.eScene)screenParams[1];

            VSEventManager.Instance.AddListener<UIEvents.AsyncSceneLoadProgressEvent>(OnLoadProgress);

            m_LoadStartTime = Time.time;
            SceneLoader.Instance.RequestSceneLoadAsync(scene);
        }
    }

    private void OnLoadProgress(UIEvents.AsyncSceneLoadProgressEvent e)
    {
        if (e.Progress >= 0.9f)
        {
            m_TotalLoadTime = Time.time - m_LoadStartTime;
            if (m_TotalLoadTime < m_MinLoadTime)
            {
                float deltaTime = m_MinLoadTime - m_TotalLoadTime;
                Invoke("Advance", deltaTime);
            }
            else
            {
                Advance();
            }
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();

        VSEventManager.Instance.RemoveListener<UIEvents.AsyncSceneLoadProgressEvent>(OnLoadProgress);
    }

    protected override void OnInputUpdate(InputActionEventData data)
	{
		if (ScreenInputLocked()) return; // can be controlled by any player

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
        }

        Vector3 moveDirection = (Vector3.up * m_Vertical + Vector3.right * m_Horizontal);
        if (moveDirection != Vector3.zero)
        {
            m_ReticleTransform.Translate(moveDirection * m_MoveSpeed * Time.deltaTime, UnityEngine.Space.World);
        }

        // pass to base
        if (!handled)
		{
			base.OnInputUpdate(data);
		}
	}

    private void Advance()
    {
        if (m_PostLoadScreenId != UI.Enums.ScreenId.None)
        {
            UIManager.Instance.TransitionToScreen(m_PostLoadScreenId);
        }
    }
}
