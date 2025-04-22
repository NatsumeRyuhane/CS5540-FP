using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class FinalSequenceController : Singleton<FinalSequenceController>
{
    public GameObject finalSequenceAnchor;
    [FormerlySerializedAs("GameEndMask")] public Image gameEndMask;
    protected override void OnAwake()
    {
        gameObject.SetActive(false);
    }

    public void Begin()
    {
        gameObject.SetActive(true);
        LevelUIManager.Instance.DoFakeLevelCompleteSequence(3f, finalSequenceAnchor.transform.position);
    }
    
    public void DoEndGameSequence()
    {
        gameEndMask.gameObject.SetActive(true);
        PlayerController.Instance.SetAllowPlayerControl(false);
        gameEndMask.DOFade(1f, 3f).OnComplete(() =>
        {
            LevelManager.Instance.ExitToMainMenu();
        });
    }
}