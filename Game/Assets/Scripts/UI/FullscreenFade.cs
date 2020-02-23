using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void FadeComplete();
public class FullscreenFade : MonoBehaviour
{

    private Image imgFade;

    [SerializeField]
    [Range(0f, 20f)]
    private float fadeInDuration;

    [SerializeField]
    [Range(0f, 20f)]
    private float fadeOutDuration;

    [SerializeField]
    [Range(0f, 20f)]
    private float fadeOutIntroDuration;

    [SerializeField]
    private Color fadeInColor = new Color(0, 0, 0, 1);

    [SerializeField]
    private Color fadeOutColor = new Color(0, 0, 0, 0);

    private float fadingDuration;

    private float fadingTimer;

    private bool isFading = false;


    private Color targetColor;
    private Color originalColor;

    FadeComplete completeCallback;

    public void Initialize()
    {
        imgFade = GetComponentInChildren<Image>();
        imgFade.color = fadeInColor;
    }

    public void IntroFadeIn() {

    }

    public void Intro() {
        imgFade.color = fadeInColor;
        FadeOut(IntroFadeIn);
        fadingDuration = fadeOutIntroDuration;
    }

    public void FadeIn(FadeComplete fadeCompleteCallback) {
        fadingDuration = fadeInDuration;
        targetColor = fadeInColor;
        completeCallback = fadeCompleteCallback;

        StartFading();
    }

    public void FadeOut(FadeComplete fadeCompleteCallback) {
        Debug.Log("Fadeout");
        fadingDuration = fadeOutDuration;
        targetColor = fadeOutColor;
        completeCallback = fadeCompleteCallback;

        StartFading();
    }

    private void StartFading() {
        originalColor = imgFade.color;
        isFading = true;
        fadingTimer = 0f;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isFading) {
            fadingTimer += Time.unscaledDeltaTime / fadingDuration;
            imgFade.color =  Color.Lerp(originalColor, targetColor, fadingTimer);
            if (fadingTimer >= 1) {
                imgFade.color = targetColor;
                fadingTimer = 0f;
                isFading = false;
                Time.timeScale = 1f;
                completeCallback();
            }
        }
    }
}
