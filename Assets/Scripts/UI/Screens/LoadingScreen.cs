using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.UI;
using Rewired;

public class LoadingScreen : UIBaseScreen
{
	[SerializeField] private RectTransform m_LoadingIcon;
    [SerializeField] private UILoadingReticle m_Reticle;
    [SerializeField] private GameObject m_CollectiblePrefab;
    [SerializeField] private float m_ReticleSpeed = 5f;
    [SerializeField] private RectTransform m_StarContainer;
    [SerializeField] private Text m_CollectedLabel;

    private float m_Horizontal;
    private float m_Vertical;

    private string m_ScoreKey = "";
    private int m_BestScore;
    private int m_TotalCollected = 0;

    public override void Initialize()
    {
        base.Initialize();

        m_Reticle.OnCollected += OnCollectibleCollected;

        LoadBestScore();
        ScatterStars();
    }

    private void LoadBestScore()
    {
        if (PlayerPrefs.HasKey(m_ScoreKey))
        {
            m_BestScore = PlayerPrefs.GetInt(m_ScoreKey);
        }
        else
        {
            m_BestScore = 0;
        }
    }

    public override void Shutdown()
    {
        base.Shutdown();

        m_Reticle.OnCollected -= OnCollectibleCollected;
    }

    private void OnCollectibleCollected()
    {
        m_TotalCollected += 1;
        m_CollectedLabel.text = m_TotalCollected.ToString();

        if (m_TotalCollected > m_BestScore)
        {
            m_BestScore = m_TotalCollected;
            PlayerPrefs.SetInt(m_ScoreKey, m_BestScore);

            m_CollectedLabel.color = m_LoadingIcon.GetComponent<Image>().color;
        }

        SpawnCollectible();
    }

    private void SpawnCollectible()
    {
        GameObject collectible = (GameObject)Instantiate(m_CollectiblePrefab, m_StarContainer);
        RectTransform rT = collectible.GetComponent<RectTransform>();
        if (rT != null)
        {
            float randX = UnityEngine.Random.Range(-400, 400);
            float randY = UnityEngine.Random.Range(-225, 225);
            rT.anchoredPosition = new Vector2(randX, randY);
        }
    }

    private void ScatterStars()
    {
        int defaultCount = 25;
        for (int i = 0; i < defaultCount; i++)
        {
            SpawnCollectible();
        }
    }

    protected override void OnInputUpdate(InputActionEventData data)
	{
		if (InputLocked()) return;

		bool handled = false;

        switch (data.actionId)
		{
		    case RewiredConsts.Action.Navigate_Horizontal:
			    m_Horizontal = data.GetAxis();
			    break;

		    case RewiredConsts.Action.Navigate_Vertical:
			    m_Vertical = data.GetAxis();
			    break;
		}

		m_LoadingIcon.Translate((Vector3.up * m_Vertical + Vector3.right * m_Horizontal) * m_ReticleSpeed * Time.deltaTime, UnityEngine.Space.World);

		// pass to base
		if (!handled)
		{
			base.OnInputUpdate(data);
		}
	}
}
