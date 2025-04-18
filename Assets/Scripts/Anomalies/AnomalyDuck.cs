using UnityEngine;


public class AnomalyDuck : AnomalyBehavior
{
    private enum States
    {
        STATIC,
        FOLLOW_PLAYER
    }

    private States _state;

    private void Update()
    {
        UpdateState();

        switch (_state)
        {
            case States.STATIC:
                // Implement static behavior here
                break;
            case States.FOLLOW_PLAYER:
                // turn to look at player
                transform.LookAt(_player.transform.position);
                break;
        }
    }

    private void UpdateState()
    {
        if (_state == States.STATIC)
        {
            if (!IsThisVisibleInCamera())
            {
                _state = States.FOLLOW_PLAYER;
            }
        }
        else if (_state == States.FOLLOW_PLAYER)
        {
            if (IsThisVisibleInCamera())
            {
                _state = States.STATIC;
            }
        }
    }

    public override void Interact()
    {
        return;
    }

    protected override void InteractEffect()
    {
        return;
    }

    protected override void OnAwake()
    {
        return;
    }

    protected override void OnAnomalyEffectEnd()
    {
        return;
    }
}