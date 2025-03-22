using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtonBehavior : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    
    [Header("Button Color Config")]
    public Color defaultTextColor = Color.white;
    public Color disabledTextColor = new Color(200, 200, 200);
    
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        if (_button.IsInteractable()) labelText.color = defaultTextColor;
        else labelText.color = disabledTextColor;
    }
}
