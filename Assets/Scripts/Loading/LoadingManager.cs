using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
{
    private Image _progressBar;
    private TextMeshProUGUI _loadingText;
    private string _sceneName;
    // Call this method to load a new scene with the loading screen
    public void LoadScene(string sceneName)
    {
        _sceneName = sceneName;
        
        // offload current scene and switch to loading screen
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // Load the loading scene first
        yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Loading"));
        
        // Find the progress bar and loading text in the scene
        _progressBar = GameObject.Find("Progress").GetComponent<Image>();
        _loadingText = GameObject.Find("LoadingText").GetComponent<TextMeshProUGUI>();
        
        // Reset progress bar
        _progressBar.fillAmount = 0f;
        
        // Start loading the scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneName);
        
        // Don't allow the scene to activate until we're ready
        operation.allowSceneActivation = false;
        
        // Track loading progress
        while (!operation.isDone)
        {
            // Unity's AsyncOperation ranges from 0 to 0.9 during loading
            // We convert it to a 0 to 1 range for our progress bar
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            // Update the progress bar
            _progressBar.fillAmount = progress;
            
            // Allow scene activation when loading is at 90%
            if (operation.progress >= 0.9f)
            {
                _loadingText.text = "PRESS ANY KEY TO CONTINUE";
                if (Input.anyKeyDown)
                {
                    // Wait a brief moment to ensure UI updates
                    yield return new WaitForSeconds(0.2f);
                
                    // Allow the scene to activate
                    operation.allowSceneActivation = true;
                
                    // Hide loading screen after a short delay to ensure smooth transition
                    yield return new WaitForSeconds(0.1f);
                }
                
            }
            
            yield return null;
        }
    }
}