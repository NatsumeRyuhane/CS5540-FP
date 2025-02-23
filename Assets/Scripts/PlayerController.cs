using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    
    private Vector3 _input;
    private Vector3 _moveDirection;

    private CharacterController _controller;
    private CameraController _cameraController;
    
    private GameObject _lookingAt;
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cameraController = GetComponentInChildren<CameraController>();
    }


    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        _input = transform.right * moveHorizontal + transform.forward * moveVertical;
        _input.Normalize();
        _input *= speed;
        
        _moveDirection = _input;
        _moveDirection.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(Time.deltaTime * _moveDirection);
        
        // check if player is interacting with an object
        if (Input.GetKeyDown(KeyCode.E))
        {
            // looping over components of the object
            foreach (var component in _lookingAt.GetComponents<MonoBehaviour>())
            {
                // check if the object has a child of InteractableObject type
                if (component is InteractableObject)
                {
                    // call the Interact method of the InteractableObject
                    (component as InteractableObject).Interact();
                }
            }
        }
    }
    
    private void FixedUpdate()
    {
        _lookingAt = _cameraController.GetLookingAt();
    }
}
