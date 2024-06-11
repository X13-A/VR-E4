using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> texts;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] protected InputActionAsset inputActionAsset;

    private int currentIndex = 0;
    private bool isSkipping = false;

    protected InputAction skipText;

    private void Awake()
    {
        skipText = inputActionAsset.FindActionMap("XRI RightHand Interaction").FindAction("Shoot");
    }

    protected void OnEnable()
    {
        skipText.performed += OnSkipPerformed;
        skipText.Enable();
    }

    protected void OnDisable()
    {
        skipText.performed -= OnSkipPerformed;
        skipText.Disable();
    }

    private void OnSkipPerformed(InputAction.CallbackContext context)
    {
        isSkipping = true;
    }

    void Start()
    {
        foreach (TextMeshProUGUI text in texts)
        {
            text.alpha = 0f;
        }

        StartCoroutine(DisplayTexts());
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
