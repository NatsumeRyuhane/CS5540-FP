using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ManuManager : Singleton<ManuManager>
{
    public GameObject stats;
    public GameObject panel;
    public GameObject settings;
    private Tweener _statsTween;
    private Tweener _settingsTween;
    private Tweener _panelTween;
    
    private void Start()
    {
        // Ensure we have CanvasGroup components on each object
        EnsureCanvasGroup(stats);
        EnsureCanvasGroup(panel);
        EnsureCanvasGroup(settings);
        
        // Initialize state
        stats.SetActive(false);
        settings.SetActive(false);
        panel.SetActive(false);
        
        // reset time scale to normal
        Time.timeScale = 1f;
    }
    
    private CanvasGroup EnsureCanvasGroup(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        return canvasGroup;
    }
    
    public static void NewGame()
    {
        // Load the game's first level using the LoadingManager
        LoadingManager.Instance.LoadScene("Level1");
    }
    
    public static void ToggleStats()
    {
        Instance.ToggleStatsNonStatic();
    }

    public static void ToggleSettings()
    {
        Instance.ToggleSettingsNonStatic();
    }

    private void setPanel(bool visible)
    {
        if (visible)
        {
            // do nothing if the panel is already visible
            if (panel.activeSelf) return;
            
            panel.SetActive(true);
            CanvasGroup canvasGroup = EnsureCanvasGroup(panel);
            canvasGroup.alpha = 0;
            _panelTween?.Kill();
            _panelTween = canvasGroup.DOFade(1, 0.5f);
        }
        else
        {
            // do nothing if the panel is already hidden
            if (!panel.activeSelf) return;
            
            CanvasGroup canvasGroup = EnsureCanvasGroup(panel);
            _panelTween?.Kill();
            _panelTween = canvasGroup.DOFade(0, 0.5f).OnComplete(() => panel.SetActive(false));
        }
    }

    private void ToggleStatsNonStatic()
    {
       if (stats.activeSelf)
       {
           // Hide stats
           CanvasGroup canvasGroup = EnsureCanvasGroup(stats);
           _statsTween?.Kill();
           _statsTween = canvasGroup.DOFade(0, 0.5f).OnComplete(() => {
               stats.SetActive(false);
               
               // Hide panel if both menus are now hidden
               if (!settings.activeSelf) {
                   setPanel(false);
               }
           });
       }
       else
       {
           // Show panel first
           setPanel(true);
           
           // Hide settings if it's visible
           if (settings.activeSelf)
           {
               CanvasGroup settingsCanvasGroup = EnsureCanvasGroup(settings);
               _settingsTween?.Kill();
               _settingsTween = settingsCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
                   settings.SetActive(false);
               });
           }
           
           // Show stats
           stats.SetActive(true);
           CanvasGroup statsCanvasGroup = EnsureCanvasGroup(stats);
           statsCanvasGroup.alpha = 0;
           _statsTween?.Kill();
           _statsTween = statsCanvasGroup.DOFade(1, 0.5f);
       }
    }

    private void ToggleSettingsNonStatic()
    {
        if (settings.activeSelf)
        {
            // Hide settings
            CanvasGroup canvasGroup = EnsureCanvasGroup(settings);
            _settingsTween?.Kill();
            _settingsTween = canvasGroup.DOFade(0, 0.5f).OnComplete(() => {
                settings.SetActive(false);
                
                // Hide panel if both menus are now hidden
                if (!stats.activeSelf) {
                    setPanel(false);
                }
            });
        }
        else
        {
            // Show panel first
            setPanel(true);
            
            // Hide stats if it's visible
            if (stats.activeSelf)
            {
                CanvasGroup statsCanvasGroup = EnsureCanvasGroup(stats);
                _statsTween?.Kill();
                _statsTween = statsCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
                    stats.SetActive(false);
                });
            }
            
            // Show settings
            settings.SetActive(true);
            CanvasGroup settingsCanvasGroup = EnsureCanvasGroup(settings);
            settingsCanvasGroup.alpha = 0;
            _settingsTween?.Kill();
            _settingsTween = settingsCanvasGroup.DOFade(1, 0.5f);
        }
    }
}
