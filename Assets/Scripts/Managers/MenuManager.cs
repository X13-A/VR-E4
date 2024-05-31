using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class MenuManager : MonoBehaviour, IEventHandler
{
    [SerializeField] GameObject m_MenuPanel;
    [SerializeField] GameObject m_WinPanel;
    [SerializeField] GameObject m_LosePanel;
    [SerializeField] GameObject m_ScorePanel;

    List<GameObject> m_Panels;

    void OpenPanel(GameObject panel)
    {
        m_Panels.ForEach(item => item.SetActive(panel == item));
    }

    private void Awake()
    {
        m_Panels = new List<GameObject>(
            new GameObject[] { m_MenuPanel, m_WinPanel, m_LosePanel, m_ScorePanel });
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<GameMenuEvent>(GameMenu);
        EventManager.Instance.AddListener<GamePlayEvent>(GamePlay);
        EventManager.Instance.AddListener<GameWinEvent>(GameWin);
        EventManager.Instance.AddListener<GameLoseEvent>(GameLose);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<GameMenuEvent>(GameMenu);
        EventManager.Instance.RemoveListener<GamePlayEvent>(GamePlay);
        EventManager.Instance.RemoveListener<GameWinEvent>(GameWin);
        EventManager.Instance.RemoveListener<GameLoseEvent>(GameLose);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void PlayButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new PlayButtonClickedEvent());
    }

    public void ScoreButtonHasBeenClicked()
    {
        Score();
    }

    public void QuitButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new QuitButtonClickedEvent());
    }

    void GameMenu(GameMenuEvent e)
    {
        OpenPanel(m_MenuPanel);
    }

    void GamePlay(GamePlayEvent e)
    {
        OpenPanel(null);
    }

    void GameLose(GameLoseEvent e)
    {
        OpenPanel(m_LosePanel);
    }

    void GameWin(GameWinEvent e)
    {
        OpenPanel(m_WinPanel);
    }

    void Score()
    {
        OpenPanel(m_ScorePanel);
    }
}
