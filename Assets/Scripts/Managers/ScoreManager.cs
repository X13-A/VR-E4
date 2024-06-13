using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using TMPro;

public class ScoreManager : MonoBehaviour, IEventHandler
{
    [SerializeField]
    TMP_Text scoresText;

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<UpdateScoreEvent>(UpdateScores);
        EventManager.Instance.AddListener<UpdateScoresTextEvent>(UpdateScoresText);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateScoreEvent>(UpdateScores);
        EventManager.Instance.RemoveListener<UpdateScoresTextEvent>(UpdateScoresText);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void UpdateScores(UpdateScoreEvent e)
    {
        int index = LevelManager.Instance.IndexLevel;
        string waveCountString = PlayerPrefs.GetString("Wave "+(index+1), "0");
        int waveCount = int.Parse(waveCountString);
        waveCount += 1;
        PlayerPrefs.SetString("Wave " + (index+1), waveCount.ToString());
        PlayerPrefs.Save();
    }

    void UpdateScoresText(UpdateScoresTextEvent e)
    {
        if (scoresText == null)
        {
            Debug.LogWarning("scoresText is null");
            return;
        }
        string text = "";
        for (int i = 0; i< LevelManager.Instance.NumberOfLevel; i++)
        {
            text += "WAVE " + (i + 1) + " : " + PlayerPrefs.GetString("Wave " + (i + 1), "0")+"\n";
        }
        scoresText.text = text;
    }
}
