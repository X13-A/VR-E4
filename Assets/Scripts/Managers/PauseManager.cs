using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class PauseManager : MonoBehaviour, IEventHandler
{
    bool isPause;
    [SerializeField] GameObject m_PausePanel;

    void Awake()
    {
        isPause = false;
    }

    public void SubscribeEvents()
    {
    }

    public void UnsubscribeEvents()
    {
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isPause)
        {
            Pause();
        }
    }

    void Pause()
    {
        isPause = true;
        Time.timeScale = 0.0f;
        EventManager.Instance.Raise(new PauseEvent());
        Debug.Log("Pause");
        m_PausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPause = false;
        Time.timeScale = 1.0f;
        EventManager.Instance.Raise(new ResumeEvent());
        Debug.Log("Resume");
        m_PausePanel.SetActive(false);
    }
}
