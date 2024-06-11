using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class LevelManager : MonoBehaviour, IEventHandler
{
    [SerializeField] List<Level> m_Levels;
    private Level m_CurrentLevel;
    private int m_IndexLevel;
    [SerializeField] float m_WaitTimeBetweenLevel;


    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        m_CurrentLevel = null;
        m_IndexLevel = 0;
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<StartBlinkingFinishedEvent>(InitializeLevel);
        EventManager.Instance.AddListener<AllEnemyDeadEvent>(FinishLevel);
        EventManager.Instance.AddListener<GameLoseEvent>(LoseLevel);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<StartBlinkingFinishedEvent>(InitializeLevel);
        EventManager.Instance.RemoveListener<AllEnemyDeadEvent>(FinishLevel);
        EventManager.Instance.RemoveListener<GameLoseEvent>(LoseLevel);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void LoadLevel(Level level)
    {
        Debug.Log("Load Level " + m_IndexLevel);
        m_CurrentLevel = level;
        EventManager.Instance.Raise(new LoadLevelEvent { level = m_CurrentLevel});
    }

    void InitializeLevel(StartBlinkingFinishedEvent e)
    {
        if (m_Levels.Count > 0)
        {
            LoadLevel(m_Levels[0]);
        }
    }

    void FinishLevel(AllEnemyDeadEvent e)
    {
        if (m_IndexLevel < m_Levels.Count - 1) {
            EventManager.Instance.Raise(new StopSoundAllEvent());
            EventManager.Instance.Raise(new PlaySoundEvent() { eNameClip = "finishedWave", eLoop = false });
            StartCoroutine(WaitNextLevel());
        }
        else
        {
            Initialize();
            EventManager.Instance.Raise(new FinishAllLevelEvent());
        }
    }

    void LoseLevel(GameLoseEvent e)
    {
        Initialize();
    }

    IEnumerator WaitNextLevel()
    {
        yield return new WaitForSeconds(m_WaitTimeBetweenLevel);
        m_IndexLevel++;
        LoadLevel(m_Levels[m_IndexLevel]);
        EventManager.Instance.Raise(new StopSoundAllEvent());
        EventManager.Instance.Raise(new PlaySoundEvent() { eNameClip = "ambient", eLoop = true });
    }

}
