using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum GAMESTATE { menu, play, pause, victory, gameover, introduction }
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
        EventManager.Instance.AddListener<PlayIntroductionButtonClickedEvent>(PlayIntroduction);
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
        EventManager.Instance.RemoveListener<PlayIntroductionButtonClickedEvent>(PlayIntroduction);
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
                EventManager.Instance.Raise(new PlaySoundEvent() { eNameClip = "menu", eLoop = true });
                break;
            case GAMESTATE.play:
                EventManager.Instance.Raise(new GamePlayEvent());
                EventManager.Instance.Raise(new StopSoundAllEvent());
                EventManager.Instance.Raise(new PlaySoundEvent() { eNameClip = "ambient", eLoop = true });
                break;
            case GAMESTATE.victory:
                EventManager.Instance.Raise(new GameWinEvent());
                EventManager.Instance.Raise(new StopSoundAllEvent());
                EventManager.Instance.Raise(new PlaySoundEvent() { eNameClip = "winGame", eLoop = false });
                break;
            case GAMESTATE.gameover:
                EventManager.Instance.Raise(new GameLoseEvent());
                EventManager.Instance.Raise(new StopSoundAllEvent());
                EventManager.Instance.Raise(new PlaySoundEvent() { eNameClip = "loseGame", eLoop = false });
                break;
            case GAMESTATE.pause:
                EventManager.Instance.Raise(new GamePauseEvent());
                EventManager.Instance.Raise(new SoundMixAllEvent() { eGameplayVolume = 0 });
                break;
            case GAMESTATE.introduction:
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

    void Introduction()
    {
        SetState(GAMESTATE.introduction);
    }

    void Play(PlayButtonClickedEvent e)
    {
        StartCoroutine(LoadSceneThenFunction(1, Play));
    }

    void PlayIntroduction(PlayIntroductionButtonClickedEvent e)
    {
        StartCoroutine(LoadSceneThenFunction(3, Introduction));
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
        EventManager.Instance.Raise(new SoundMixAllEvent() { eGameplayVolume = 1 });
    }

    private IEnumerator LoadSceneThenFunction(int? sceneIndex, afterFunction function)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)sceneIndex);
        // Attendre que la scène soit chargée
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        // La scène est chargée, appeler la fonction spécifiée
        function.Invoke();
    }
}
