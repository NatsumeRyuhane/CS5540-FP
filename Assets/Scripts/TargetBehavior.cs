using UnityEngine;

public class TargetBehavior : InteractableObject
{
    public Color completedColor;
    [HideInInspector] public bool Completed { get; private set; }
    
    public void Start()
    {
        Completed = false;
    }
    
    public override void Interact()
    {
        Completed = true;
        GetComponent<Renderer>().material.color = completedColor;
    }
}
