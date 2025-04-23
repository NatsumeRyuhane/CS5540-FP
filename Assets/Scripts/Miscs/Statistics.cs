using UnityEngine;

public class Statistics : Singleton<Statistics>
{
    // this class logs how many rooms the player has traveled 
    // and save it to the player prefs
    private int _roomsTraveled;
    private const string RoomsTraveledKey = "RoomsTraveled";
    
    private int _doorsOpened;
    private const string DoorsOpenedKey = "DoorsOpened";
    
    private void Awake()
    {
        // Load the saved statistics from PlayerPrefs
        _roomsTraveled = PlayerPrefs.GetInt(RoomsTraveledKey, 0);
        _doorsOpened = PlayerPrefs.GetInt(DoorsOpenedKey, 0);
    }
    
    public void IncrementRoomCount()
    {
        _roomsTraveled++;
        PlayerPrefs.SetInt(RoomsTraveledKey, _roomsTraveled);
        PlayerPrefs.Save();
    }
    
    public int GetRoomsTraveled()
    {
        return _roomsTraveled;
    }
    
    public void IncrementDoorsOpened()
    {
        _doorsOpened++;
        PlayerPrefs.SetInt(DoorsOpenedKey, _doorsOpened);
        PlayerPrefs.Save();
    }
    
    public int GetDoorsOpened()
    {
        return PlayerPrefs.GetInt(DoorsOpenedKey, 0);
    }
}