using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelBehavior: MonoBehaviour
{
    public Toggle fastModeToggle;
    private string fastModeKeyName = "FastModeEnabled";
    
    void OnEnable()
    {
        // Load the saved fast mode setting from PlayerPrefs
        bool isFastModeEnabled = PlayerPrefs.GetInt(fastModeKeyName, 0) == 1;
        fastModeToggle.isOn = isFastModeEnabled;
        
        // Subscribe to the toggle's onValueChanged event
        fastModeToggle.onValueChanged.AddListener(OnFastModeToggleValueChanged);
    }
    
    void OnDisable()
    {
        // Unsubscribe from the toggle's onValueChanged event
        if (fastModeToggle != null)
            fastModeToggle.onValueChanged.RemoveListener(OnFastModeToggleValueChanged);
    }
    
    private void OnFastModeToggleValueChanged(bool isOn)
    {
        // This method will be called whenever the toggle's value changes
        // The isOn parameter contains the new value of the toggle
        SetFastMode(isOn);
        
        // You can do additional things with the toggle value here
        Debug.Log("Fast mode is now: " + (isOn ? "enabled" : "disabled"));
    }
    
    public static void SetFastMode(bool isEnabled)
    {
        // Save the fast mode setting to PlayerPrefs
        PlayerPrefs.SetInt("FastModeEnabled", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
