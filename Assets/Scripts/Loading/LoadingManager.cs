using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    private Image _progressBar;
    private TextMeshProUGUI _loadingText;
    private string _sceneName;

    // Call this method to load a new scene with the loading screen
    public void LoadScene(string sceneName)
    {
        _sceneName = sceneName;

        // Validate if the scene exists
        if (!Application.CanStreamedLevelBeLoaded(_sceneName))
        {
            Debug.LogError($"Scene '{_sceneName}' does not exist or cannot be loaded.");
            StartCoroutine(DisplayErrorMessage($"ERROR: Scene '{_sceneName}' not found. Please check the level name."));
            return;
        }

        // Offload current scene and switch to loading screen
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // Load the loading scene first
        var loadLoadingScene = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single);
        loadLoadingScene.allowSceneActivation = true;
        while (!loadLoadingScene.isDone)
        {
            yield return null;
        }

        Debug.Log("Loading scene: " + _sceneName);

        // Reset references since we're in a new scene
        FindUIReferences();

        // Reset progress bar
        if (_progressBar != null)
            _progressBar.fillAmount = 0f;

        // Start loading the scene in the background
        var asyncLoad = SceneManager.LoadSceneAsync(_sceneName);
        if (asyncLoad == null)
        {
            Debug.LogError($"Failed to load scene '{_sceneName}'.");
            yield break;
        }

        asyncLoad.allowSceneActivation = true;

        // Track loading progress
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress);
            if (_progressBar != null)
                _progressBar.fillAmount = progress;

            yield return null;
        }
    }

    private void FindUIReferences()
    {
        GameObject progressBarObject = GameObject.Find("Progress");
        GameObject loadingTextObject = GameObject.Find("LoadingText");

        if (progressBarObject == null || loadingTextObject == null)
        {
            Debug.LogError("Loading UI components not found in the scene.");
            return;
        }

        _progressBar = progressBarObject.GetComponent<Image>();
        _loadingText = loadingTextObject.GetComponent<TextMeshProUGUI>();
    }

    private IEnumerator DisplayErrorMessage(string message)
    {
        // Load the loading scene first
        yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Loading"));

        GameObject loadingTextObject = GameObject.Find("LoadingText");

        if (loadingTextObject == null)
        {
            Debug.LogError("LoadingText UI component not found in the scene.");
            yield break;
        }

        _loadingText = loadingTextObject.GetComponent<TextMeshProUGUI>();
        _loadingText.text = message;

        yield return new WaitForSeconds(5f);
    }
}