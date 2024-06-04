using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.SceneManagement;

public enum GAMESTATE { menu, play, pause, victory, gameover }
public delegate void afterFunction();

public class GameManager : MonoBehaviour, IEventHandler
{
    public static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }

    GAMESTATE m_State;
    public bool IsPlaying => m_State == GAMESTATE.play;

    void Start()
    {
        SetState(GAMESTATE.menu);
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(Play);
        EventManager.Instance.AddListener<ReplayButtonClickedEvent>(Replay);
        EventManager.Instance.AddListener<MenuButtonClickedEvent>(Menu);
        EventManager.Instance.AddListener<FinishAllLevelEvent>(Win);
        EventManager.Instance.AddListener<LoseEvent>(Lose);
        EventManager.Instance.AddListener<PauseEvent>(Pause);
        EventManager.Instance.AddListener<ResumeEvent>(Resume);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(Play);
        EventManager.Instance.RemoveListener<ReplayButtonClickedEvent>(Replay);
        EventManager.Instance.RemoveListener<MenuButtonClickedEvent>(Menu);
        EventManager.Instance.RemoveListener<FinishAllLevelEvent>(Win);
        EventManager.Instance.RemoveListener<LoseEvent>(Lose);
        EventManager.Instance.RemoveListener<ResumeEvent>(Resume);
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

    void Play()
    {
        SetState(GAMESTATE.play);
    }

    void Play(PlayButtonClickedEvent e)
    {
        StartCoroutine(LoadSceneThenFunction(1, Play));
    }

    void Replay(ReplayButtonClickedEvent e)
    {
        StartCoroutine(LoadSceneThenFunction(1, Play));
    }

    void Menu(MenuButtonClickedEvent e)
    {
        SetState(GAMESTATE.menu);
    }

    void Win()
    {
        SetState(GAMESTATE.victory);
    }

    void Win(FinishAllLevelEvent e)
    {
        StartCoroutine(LoadSceneThenFunction(0, Win));
    }

    void Lose()
    {
        SetState(GAMESTATE.gameover);
    }

    void Lose(LoseEvent e)
    {
        StartCoroutine(LoadSceneThenFunction(0,Lose));
    }

    void Pause(PauseEvent e)
    {
        SetState(GAMESTATE.pause);
    }

    void Resume(ResumeEvent e)
    {
        m_State = GAMESTATE.play;
        EventManager.Instance.Raise(new GameResumeEvent());

    }

    private IEnumerator LoadSceneThenFunction(int sceneIndex, afterFunction function)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        // Attendre que la scène soit chargée
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        // La scène est chargée, appeler la fonction spécifiée
        function.Invoke();
    }
}
