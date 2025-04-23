using TMPro;
using UnityEngine;

public class StatsPanelBehavior: MonoBehaviour
{
    public TextMeshProUGUI doorsText;
    public TextMeshProUGUI roomsText;
    
    void OnEnable()
    {
        var doors = Statistics.Instance.GetDoorsOpened();
        var rooms = Statistics.Instance.GetRoomsTraveled();
        
        doorsText.text = $"Doors Opened: {doors}";
        roomsText.text = $"Rooms Traveled: {rooms}";
    }
}