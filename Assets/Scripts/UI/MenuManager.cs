using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject firstMenu;
    [SerializeField] private GameObject secondMenu;
    [SerializeField] private GameObject sharedBackground;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    
    // Canvas Group components
    private CanvasGroup firstMenuCanvasGroup;
    private CanvasGroup secondMenuCanvasGroup;
    private CanvasGroup backgroundCanvasGroup;
    
    // Track current state
    private bool isFirstMenuActive = false;
    private bool isSecondMenuActive = false;
    
    private void Awake()
    {
        // Get or add CanvasGroup components
        firstMenuCanvasGroup = GetOrAddCanvasGroup(firstMenu);
        secondMenuCanvasGroup = GetOrAddCanvasGroup(secondMenu);
        backgroundCanvasGroup = GetOrAddCanvasGroup(sharedBackground);
        
        // Initialize state
        firstMenuCanvasGroup.alpha = 0;
        secondMenuCanvasGroup.alpha = 0;
        backgroundCanvasGroup.alpha = 0;
        
        firstMenu.SetActive(false);
        secondMenu.SetActive(false);
        sharedBackground.SetActive(false);
    }
    
    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        return canvasGroup;
    }
    
    public void ShowFirstMenu()
    {
        if (isFirstMenuActive) return;
        
        if (isSecondMenuActive)
        {
            // Hide second menu first
            StartCoroutine(FadeOut(secondMenuCanvasGroup, secondMenu));
            isSecondMenuActive = false;
        }
        else
        {
            // Show background if neither menu was active
            sharedBackground.SetActive(true);
            StartCoroutine(FadeIn(backgroundCanvasGroup));
        }
        
        // Show first menu
        firstMenu.SetActive(true);
        StartCoroutine(FadeIn(firstMenuCanvasGroup));
        isFirstMenuActive = true;
    }
    
    public void ShowSecondMenu()
    {
        if (isSecondMenuActive) return;
        
        if (isFirstMenuActive)
        {
            // Hide first menu
            StartCoroutine(FadeOut(firstMenuCanvasGroup, firstMenu));
            isFirstMenuActive = false;
        }
        else
        {
            // Show background if neither menu was active
            sharedBackground.SetActive(true);
            StartCoroutine(FadeIn(backgroundCanvasGroup));
        }
        
        // Show second menu
        secondMenu.SetActive(true);
        StartCoroutine(FadeIn(secondMenuCanvasGroup));
        isSecondMenuActive = true;
    }
    
    public void HideAllMenus()
    {
        if (isFirstMenuActive)
        {
            StartCoroutine(FadeOut(firstMenuCanvasGroup, firstMenu));
            isFirstMenuActive = false;
        }
        
        if (isSecondMenuActive)
        {
            StartCoroutine(FadeOut(secondMenuCanvasGroup, secondMenu));
            isSecondMenuActive = false;
        }
        
        StartCoroutine(FadeOut(backgroundCanvasGroup, sharedBackground));
    }
    
    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0;
        canvasGroup.alpha = 0;
        
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 1;
    }
    
    private IEnumerator FadeOut(CanvasGroup canvasGroup, GameObject objectToDeactivate)
    {
        float elapsedTime = 0;
        canvasGroup.alpha = 1;
        
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 0;
        objectToDeactivate.SetActive(false);
    }
}
