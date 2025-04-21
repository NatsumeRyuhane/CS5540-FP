using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelUIManager : Singleton<LevelUIManager>
{
    [Header("UI Elements")]
    public GameObject objectives;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI guideText;
    public TextMeshProUGUI playerMessageText;
    [FormerlySerializedAs("levelFailMask")] public Image levelEndMask;
    public GameObject levelFailUI;
    [FormerlySerializedAs("lavelWinUI")] public GameObject levelWinUI;
    public Image actionCastBar;
    public Image actionCastBarProgress;
    
    [Header("UI Prefabs")]
    public GameObject objectiveTextPrefab;
    
    private bool _isPlayerMessageDisplaying = false;

    // Store references to active tweens for interruption handling
    private Tween _playerMessageTween;
    private Tween _guideTextTween;
    private Tween _actionCastBarTween;
    private Tween _actionCastBarProgressTween;
    private Tween _levelEndMaskTween;

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
        var objectiveText = newObjective.GetComponent<TextMeshProUGUI>();
        objectiveText.text = objective;
        objectiveText.alpha = 0; // start invisible
        
        // Use DOTween to fade in
        objectiveText.DOFade(1f, 0.5f);
        
        return objectiveText;
    }
    
    public void UpdateObjective(string objective, TextMeshProUGUI objectiveText)
    {
        objectiveText.text = objective;
    }
    
    public void CompleteObjective(TextMeshProUGUI objectiveText)
    {
        // Kill any running animations on this text
        DOTween.Kill(objectiveText);
        
        // Fade in then destroy
        objectiveText.DOFade(0f, 0.5f).OnComplete(() => {
            Destroy(objectiveText.gameObject);
        });
    }
    
    public void FailObjective(TextMeshProUGUI objectiveText)
    {
        // Kill any running animations on this text
        DOTween.Kill(objectiveText);
        
        objectiveText.text = "Objective Failed: " + objectiveText.text;
        objectiveText.color = Color.red;
        
        // Fade out then destroy
        objectiveText.DOFade(0f, 0.5f).OnComplete(() => {
            Destroy(objectiveText.gameObject);
        });
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
        // Kill any running animation on guide text
        if (_guideTextTween != null)
        {
            _guideTextTween.Kill();
            _guideTextTween = null;
        }
        
        float targetAlpha = display ? 1f : 0f;
        _guideTextTween = guideText.DOFade(targetAlpha, 0.5f);
    }
    
    public void FirePlayerMessage(string message, float duration = 5f)
    {
        // Update message text
        playerMessageText.text = message;

        // Kill any existing animation sequence
        if (_playerMessageTween != null)
        {
            _playerMessageTween.Kill();
            _playerMessageTween = null;
        }

        // Create a new animation sequence
        Sequence messageSequence = DOTween.Sequence();

        // If a message is already displaying, don't fade in again
        if (!_isPlayerMessageDisplaying)
        {
            // Fade in the message
            messageSequence.Append(playerMessageText.DOFade(1f, 0.5f));
            _isPlayerMessageDisplaying = true;
        }
        else
        {
            // If already displaying, ensure it's fully visible
            playerMessageText.alpha = 1f;
        }

        // Wait for duration
        messageSequence.AppendInterval(duration);

        // Fade out
        messageSequence.Append(playerMessageText.DOFade(0f, 0.5f))
            .OnComplete(() => _isPlayerMessageDisplaying = false);

        // Store the sequence reference
        _playerMessageTween = messageSequence;
    }
    
    public void DoLevelFailSequence(float duration)
    { 
        // Kill any existing level end animation
        if (_levelEndMaskTween != null)
        {
            _levelEndMaskTween.Kill();
            _levelEndMaskTween = null;
        }
        
        // Setup the level end mask
        levelEndMask.gameObject.SetActive(true);
        levelEndMask.color = new Color(0, 0, 0, 0);
        
        // Create animation sequence
        Sequence failSequence = DOTween.Sequence();
        
        // Fade in the mask
        failSequence.Append(levelEndMask.DOFade(1f, duration));
        
        // Wait for 2 seconds
        failSequence.AppendInterval(2f);
        
        // Show the fail UI
        failSequence.AppendCallback(() => levelFailUI.SetActive(true));
        
        // Store the sequence reference
        _levelEndMaskTween = failSequence;
    }
    
    public void DoLevelCompleteSequence(float duration)
    { 
        // Kill any existing level end animation
        if (_levelEndMaskTween != null)
        {
            _levelEndMaskTween.Kill();
            _levelEndMaskTween = null;
        }
        
        // Setup the level end mask
        levelEndMask.gameObject.SetActive(true);
        levelEndMask.color = new Color(0, 0, 0, 0);
        
        // Create animation sequence
        Sequence winSequence = DOTween.Sequence();
        
        // Fade in the mask
        winSequence.Append(levelEndMask.DOFade(1f, duration));
        
        // Wait for 2 seconds
        winSequence.AppendInterval(2f);
        
        // Show the win UI
        winSequence.AppendCallback(() => levelWinUI.SetActive(true));
        
        // Store the sequence reference
        _levelEndMaskTween = winSequence;
    }
    
    public void UpdateActionCastBar(float progress)
    {
        actionCastBarProgress.fillAmount = progress;
    }
    
    public void FadeInActionCastBar()
    {
        // Kill any existing animations on action cast bar
        if (_actionCastBarTween != null)
        {
            _actionCastBarTween.Kill();
            _actionCastBarTween = null;
        }
        
        if (_actionCastBarProgressTween != null)
        {
            _actionCastBarProgressTween.Kill();
            _actionCastBarProgressTween = null;
        }
        
        // Enable the bars
        actionCastBar.enabled = true;
        actionCastBarProgress.enabled = true;
        
        // Start new fade in animations
        _actionCastBarTween = actionCastBar.DOFade(1f, 0.5f);
        _actionCastBarProgressTween = actionCastBarProgress.DOFade(1f, 0.5f);
    }
    
    public void FadeOutActionCastBar()
    {
        // Kill any existing animations on action cast bar
        if (_actionCastBarTween != null)
        {
            _actionCastBarTween.Kill();
            _actionCastBarTween = null;
        }
        
        if (_actionCastBarProgressTween != null)
        {
            _actionCastBarProgressTween.Kill();
            _actionCastBarProgressTween = null;
        }
        
        // Start new fade out animations
        _actionCastBarTween = actionCastBar.DOFade(0f, 0.5f).OnComplete(() => {
            actionCastBar.enabled = false;
        });
        
        _actionCastBarProgressTween = actionCastBarProgress.DOFade(0f, 0.5f).OnComplete(() => {
            actionCastBarProgress.enabled = false;
        });
    }
}
