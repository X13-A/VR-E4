using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.SceneManagement;

public enum GAMESTATE { menu, play, pause, victory, gameover }

public class GameManager : MonoBehaviour, IEventHandler
{
    public static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }

    GAMESTATE m_State;
    public bool IsPlaying => m_State == GAMESTATE.play;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(Play);
        EventManager.Instance.AddListener<MenuButtonClickedEvent>(Menu);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(Play);
        EventManager.Instance.AddListener<MenuButtonClickedEvent>(Menu);
    }

    void SetState(GAMESTATE newState)
    {
        m_State = newState;

        switch (m_State)
        {
            case GAMESTATE.menu:
                EventManager.Instance.Raise(new GameMenuEvent());
                break;
            case GAMESTATE.play:
                EventManager.Instance.Raise(new GamePlayEvent());
                break;
            case GAMESTATE.victory:
                EventManager.Instance.Raise(new GameWinEvent());
                break;
            case GAMESTATE.gameover:
                EventManager.Instance.Raise(new GameLoseEvent());
                break;
        }
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void Play(PlayButtonClickedEvent e)
    {
        SetState(GAMESTATE.play);
        SceneManager.LoadScene(1);
    }

    void Menu(MenuButtonClickedEvent e)
    {
        SetState(GAMESTATE.menu);
    }

}
