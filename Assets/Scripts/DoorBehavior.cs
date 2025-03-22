using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorBehavior : InteractableObject
{
    private bool _isOpen = false;
    private GameObject _door;
    private Animator _animator;
    public bool allowInteract = true;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        Close();
    }
    
    public override void Interact()
    {
        if (!allowInteract) return;
        
        _isOpen = !_isOpen;
        
        if (_isOpen) Open();
        else Close();
    }

    public void Open()
    {
        _animator.SetBool("isOpen", true);
    }
    
    public void Close()
    {
        _animator.SetBool("isOpen", false);
    }
    
}