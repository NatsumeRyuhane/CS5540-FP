using UnityEngine;


public class AnomalyDuck : AnomalyBehavior
{
    private enum States
    {
        STATIC,
        FOLLOW_PLAYER
    }

    private States _state;
    private GameObject _player;
    
    private void OnEnable()
    {
        _state = States.STATIC;
        _player = Player.gameObject;
    }

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
                Vector3 direction = _player.transform.position - transform.position;
                direction.y = 0; // Remove vertical component
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
                break;
        }
    }

    private void UpdateState()
    {
        if (_state == States.STATIC)
        {
            if (!IsThisVisibleInMainCamera())
            {
                _state = States.FOLLOW_PLAYER;
            }
        }
        else if (_state == States.FOLLOW_PLAYER)
        {
            if (IsThisVisibleInMainCamera())
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
    
}