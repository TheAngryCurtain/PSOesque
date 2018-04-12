using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.UI;
using Rewired;

public class LoadingScreen : UIBaseScreen
{
    [SerializeField] private UILoadingReticle m_Reticle;
    [SerializeField] private GameObject m_EnemyPrefab;
    [SerializeField] private GameObject m_LoadingContainer;
    [SerializeField] private GameObject m_CollectiblePrefab;

    [SerializeField] private Text m_CollectedLabel;
    [SerializeField] private Image m_RadialTimerFill;
    [SerializeField] private List<UIPromptInfo> m_SkipPrompts;

    private string m_ScoreKey = "0xff0000";
    private int m_BestScore;
    private int m_TotalCollected = 0;

    private bool m_CanSkip = false;
    private bool m_PlayerControlEnabled = true;

    // should make these serializable for editing
    private float m_EnemySpawnInterval = 3.5f;
    private float m_MinEnemySpawnInterval = 0.5f;
    private int m_EnemySpawnCount = 0;
    private float m_SpawnTime = 0f;
    private int m_MaxEnemyPerInterval = 6;
    private float m_PlayerRespawnDelay = 1f;
    private float m_MaxGameTime = 90f; // seconds
    private float m_CurrentGameTime = 0f;

    public override void Initialize()
    {
        base.Initialize();

        m_Reticle.OnCollected += OnCollectibleCollected;
        m_Reticle.OnDelivered += OnCollectiableDelivered;
        m_Reticle.OnKilled += OnPlayerKilled;

        Invoke("SkipLoading", 2f);
        InvokeRepeating("ClockTick", 1f, 1f);

        LoadBestScore();
        m_CurrentGameTime = m_MaxGameTime;
    }

    private void ClockTick()
    {
        // update game timer
        if (m_CurrentGameTime > 0f)
        {
            m_CurrentGameTime -= 1f;
        }
        else
        {
            m_CurrentGameTime = 0;

            // end game
            PlayerPrefs.SetInt(m_ScoreKey, m_BestScore);
            Advance();
        }

        StartCoroutine(SetRadialValue(m_RadialTimerFill, m_CurrentGameTime / m_MaxGameTime));
    }

    public override void Update()
    {
        if (Time.time > m_SpawnTime + m_EnemySpawnInterval)
        {
            m_SpawnTime = Time.time;
            SpawnEnemy();

            m_EnemySpawnCount += 1;

            if (m_EnemySpawnCount == m_MaxEnemyPerInterval)
            {
                if (m_EnemySpawnInterval > m_MinEnemySpawnInterval)
                {
                    m_EnemySpawnInterval -= 0.5f;
                }
                else
                {
                    m_EnemySpawnInterval = m_MinEnemySpawnInterval;
                }

                m_EnemySpawnCount = 0;
            }
        }
    }

    private void LoadBestScore()
    {
        if (PlayerPrefs.HasKey(m_ScoreKey))
        {
            m_BestScore = PlayerPrefs.GetInt(m_ScoreKey);
            Debug.LogFormat("Best Score: {0}", m_BestScore);
        }
        else
        {
            m_BestScore = 0;
#if UNITY_EDITOR
            PlayerPrefs.DeleteKey(m_ScoreKey); // clear score
#endif
        }
    }

    private void SkipLoading()
    {
        UIManager.Instance.RefreshPrompts(m_SkipPrompts);
        m_CanSkip = true;

        m_LoadingContainer.SetActive(false);
    }

    private void SpawnEnemy()
    {
        Vector2 randPosition = UnityEngine.Random.insideUnitCircle;
        GameObject enemyObj = (GameObject)Instantiate(m_EnemyPrefab, this.transform);
        RectTransform rTransform = enemyObj.GetComponent<RectTransform>();

        UILoadingEnemy enemy = enemyObj.GetComponent<UILoadingEnemy>();
        if (enemy != null)
        {
            enemy.OnDestroyed += OnEnemyDestroyed;
            enemy.OnCollectorReached += OnCollectorReached;

            enemy.SetTargetPosition(Vector2.zero);
        }
        else
        {
            Debug.LogErrorFormat("Enemy doesn't have the UILoadingEnemy component attached");
        }

        rTransform.anchoredPosition = randPosition.normalized * 500f; // define constant for radius
    }

    public override void Shutdown()
    {
        m_Reticle.OnCollected -= OnCollectibleCollected;
        m_Reticle.OnDelivered -= OnCollectiableDelivered;
        m_Reticle.OnKilled -= OnPlayerKilled;

        base.Shutdown();
    }

    private void OnCollectibleCollected()
    {
        // TODO
        // make the home base thing pulse or glow or something
    }

    private void SetCollected(int amount)
    {
        m_TotalCollected = amount;
        m_CollectedLabel.text = m_TotalCollected.ToString();
    }

    private void OnCollectiableDelivered()
    {
        SetCollected(m_TotalCollected + 1);
        
        if (m_TotalCollected > m_BestScore)
        {
            m_BestScore = m_TotalCollected;
        }
    }

    private void OnPlayerKilled()
    {
        // TODO
        // particles?
        // sound effect?
        m_PlayerControlEnabled = false;
        m_Reticle.gameObject.SetActive(false);

        RectTransform rT = m_Reticle.GetComponent<RectTransform>();
        rT.anchoredPosition = new Vector2(0f, 50f);

        Invoke("ReEnablePlayer", m_PlayerRespawnDelay);
    }

    private void ReEnablePlayer()
    {
        m_Reticle.gameObject.SetActive(true);
        m_PlayerControlEnabled = true;
    }

    private void OnEnemyDestroyed(UILoadingEnemy enemy)
    {
        // TODO
        // particles?
        // sound effect?

        RectTransform enemyTransform = enemy.GetComponent<RectTransform>();
        SpawnCollectibleAtPosition(enemyTransform.anchoredPosition);

        enemy.OnDestroyed -= OnEnemyDestroyed;
        enemy.OnCollectorReached -= OnCollectorReached;

        Destroy(enemy.gameObject);
    }

    private void OnCollectorReached(UILoadingEnemy enemy)
    {
        // TODO
        // particles?
        // sound effect?
        if (m_TotalCollected > 0)
        {
            SetCollected(m_TotalCollected - 1);
        }

        Destroy(enemy.gameObject);
    }

    private IEnumerator SetRadialValue(Image radial, float desired)
    {
        float decrement = 0.01f;
        while (radial.fillAmount > desired)
        {
            radial.fillAmount -= decrement;

            yield return null;
        }

        radial.fillAmount = desired;
    }

    private void SpawnCollectibleAtPosition(Vector2 position)
    {
        GameObject collectible = (GameObject)Instantiate(m_CollectiblePrefab, this.transform);

        RectTransform rT = collectible.GetComponent<RectTransform>();
        rT.anchoredPosition = position;
    }

    protected override void OnInputUpdate(InputActionEventData data)
	{
		if (InputLocked()) return;

		bool handled = false;

        switch (data.actionId)
		{
            case RewiredConsts.Action.Confirm: // change this to START, once you set that up you lazy fool
                if (m_CanSkip && data.GetButtonDown())
                {
                    Advance();
                    handled = true;
                }
                break;

            default:
                if (m_PlayerControlEnabled)
                {
                    handled = m_Reticle.OnInputUpdate(data);
                }
                break;
		}

		// pass to base
		if (!handled)
		{
			base.OnInputUpdate(data);
		}
	}

    private void Advance()
    {
        UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Lobby);
    }
}
