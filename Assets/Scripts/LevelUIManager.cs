using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject objectives;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI guideText;
    public TextMeshProUGUI playerMessageText;
    public Image levelFailMask;
    public GameObject levelFailUI;
    public Image actionCastBar;
    public Image actionCastBarProgress;
    
    [Header("UI Prefabs")]
    public GameObject objectiveTextPrefab;

    private void Start()
    {
        playerMessageText.alpha = 0; // start invisible
        actionCastBar.enabled = false;
        actionCastBarProgress.enabled = false;
    }

    public TextMeshProUGUI CreateObjective(string objective)
    {
        // create a TMP under objectives
        var newObjective = Instantiate(objectiveTextPrefab, objectives.transform);
        newObjective.GetComponent<TextMeshProUGUI>().text = objective;
        newObjective.GetComponent<TextMeshProUGUI>().alpha = 0; // start invisible
        StartCoroutine(FadeInText(newObjective.GetComponent<TextMeshProUGUI>(), 0.5f));
        return newObjective.GetComponent<TextMeshProUGUI>();
    }
    
    public void UpdateObjective(string objective, TextMeshProUGUI objectiveText)
    {
        objectiveText.text = objective;
    }
    
    public void CompleteObjective(TextMeshProUGUI objectiveText)
    {
        StartCoroutine(FadeInText(objectiveText, 0.5f));
        Destroy(objectiveText.gameObject, 0.5f);
    }
    
    public void FailObjective(TextMeshProUGUI objectiveText)
    {
        objectiveText.text = "Objective Failed: " + objectiveText.text;
        objectiveText.color = Color.red;
        StartCoroutine(FadeInText(objectiveText, 0.5f));
        Destroy(objectiveText.gameObject, 0.5f);
    }
    
    public void UpdateTime(string time)
    {
        timeText.text = time;
    }
    
    public void UpdateLevelName(string levelName)
    {
        levelNameText.text = levelName;
    }
    
    public void UpdateGuideText(string text)
    {
        guideText.text = text;
    }
    
    public void SetGuideTextDisplay(bool display)
    {
        if (display)
        {
            StartCoroutine(FadeInText(guideText, 0.5f));
        }
        else
        {
            StartCoroutine(FadeOutText(guideText, 0.5f));
        }
    }
    
    public IEnumerator FirePlayerMessage(string message, float duration = 5f)
    {
        playerMessageText.text = message;
        StartCoroutine(FadeInText(playerMessageText, 0.5f));
        yield return new WaitForSeconds(duration);
        StartCoroutine(FadeOutText(playerMessageText, 0.5f));
    }
    
    public IEnumerator DoLevelFailSequence(float duration)
    { 
        // fade in the mask
        levelFailMask.gameObject.SetActive(true);
        levelFailMask.color = new Color(0, 0, 0, 0);
        while (levelFailMask.color.a < 1)
        {
            levelFailMask.color = new Color(0, 0, 0, levelFailMask.color.a + Time.deltaTime / duration);
            yield return null;
        }
        yield return new WaitForSeconds(2);
        levelFailUI.SetActive(true);
    }
    

    
    private IEnumerator FadeInText(TextMeshProUGUI text, float duration)
    {
        float startAlpha = text.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            text.alpha = Mathf.Lerp(startAlpha, 1, time / duration);
            yield return null;
        }
        text.alpha = 1;
    }
    
    private IEnumerator FadeOutText(TextMeshProUGUI text, float duration)
    {
        float startAlpha = text.alpha;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            text.alpha = Mathf.Lerp(startAlpha, 0, time / duration);
            yield return null;
        }
        text.alpha = 0;
    }
    
    private IEnumerator FadeInImage(Image image, float duration)
    {
        float startAlpha = image.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(startAlpha, 1, time / duration));
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }
    
    private IEnumerator FadeOutImage(Image image, float duration)
    {
        float startAlpha = image.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(startAlpha, 0, time / duration));
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
        
    
    public void UpdateActionCastBar(float progress)
    {
        actionCastBarProgress.fillAmount = progress;
    }
    
    public void FadeInActionCastBar()
    {
        actionCastBar.enabled = true;
        actionCastBarProgress.enabled = true;
        StartCoroutine(FadeInImage(actionCastBar, 0.5f));
        StartCoroutine(FadeInImage(actionCastBarProgress, 0.5f));   
    }
    
    public void FadeOutActionCastBar()
    {
        StartCoroutine(FadeOutImage(actionCastBar, 0.5f));
        StartCoroutine(FadeOutImage(actionCastBarProgress, 0.5f));
    }
}
