using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    public List<TextMeshProUGUI> texts;
    public float fadeDuration = 1.0f;
    public float displayDuration = 2.0f;
    public KeyCode skipKey = KeyCode.Space;

    private int currentIndex = 0;
    private bool isSkipping = false;

    void Start()
    {
        foreach (TextMeshProUGUI text in texts)
        {
            text.alpha = 0f;
        }

        StartCoroutine(DisplayTexts());
    }

    void Update()
    {
        if (Input.GetKeyDown(skipKey))
        {
            isSkipping = true;
        }
    }

    IEnumerator DisplayTexts()
    {
        while (currentIndex < texts.Count)
        {
            yield return StartCoroutine(FadeIn(texts[currentIndex]));
            float elapsedTime = 0f;
            while (elapsedTime < displayDuration)
            {
                if (isSkipping)
                {
                    isSkipping = false;
                    break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return StartCoroutine(FadeOut(texts[currentIndex]));
            currentIndex++;
        }

        EventManager.Instance.Raise(new PlayButtonClickedEvent());
    }

    IEnumerator FadeIn(TextMeshProUGUI text)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (isSkipping)
            {
                text.alpha = 1f;
                isSkipping = false;
                yield break;
            }
            text.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.alpha = 1f;
    }

    IEnumerator FadeOut(TextMeshProUGUI text)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (isSkipping)
            {
                text.alpha = 0f;
                isSkipping = false;
                yield break;
            }
            text.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.alpha = 0f;
    }
}
