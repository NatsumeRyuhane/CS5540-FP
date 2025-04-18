using UnityEngine;

public class MainMenuBehavior : MonoBehaviour
{
    public static void NewGame()
    {
        // Load the game's first level using the LoadingManager
        LoadingManager.instance.LoadScene("Level2");
    }

    public static void Continue()
    {
        throw new System.NotImplementedException();
    }
    
    public static void OpenSettings()
    {
        throw new System.NotImplementedException();
    }
}