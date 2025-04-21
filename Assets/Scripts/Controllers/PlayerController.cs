using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : Singleton<PlayerController>
{
    [Header("Movement Settings")] public float speed;
    public float jumpHeight;
    public AudioClip[] walkSounds;

    [Header("Interaction Settings")] public float interactionDistance = 5f;
    private CameraController _cameraController;

    private CharacterController _controller;

    private Vector3 _input;

    private GameObject _lookingAt;
    private Vector3 _moveDirection;
    
    [HideInInspector] public bool AllowPlayerControl { get; private set; }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cameraController = GetComponentInChildren<CameraController>();
        StartCoroutine(StartWalkingSound());
    }


    private void Update()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");

        _input = transform.right * moveHorizontal + transform.forward * moveVertical;
        _input.Normalize();
        _input *= speed;

        _moveDirection = _input;
        _moveDirection.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(Time.deltaTime * _moveDirection);

        // check if player is interacting with an object
        if (Input.GetKeyDown(KeyCode.E))
            if (_lookingAt != null)
                // looping over components of the object
                foreach (var component in _lookingAt.GetComponents<MonoBehaviour>())
                    // check if the object has a child of InteractableObject type
                    if (component is InteractableObject && component.enabled &&
                        Vector3.Distance(transform.position, _lookingAt.transform.position) <= interactionDistance)
                        // call the Interact method of the InteractableObject
                        (component as InteractableObject).Interact();
    }

    private void FixedUpdate()
    {
        _lookingAt = _cameraController.GetLookingAt();
    }

    private IEnumerator StartWalkingSound()
    {
        var audioSource = GetComponent<AudioSource>();
        // randomly select a walk sound
        audioSource.clip = walkSounds[Random.Range(0, walkSounds.Length)];
        audioSource.Play();
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            // if walking, play sound
            if (_input.magnitude > 0)
            {
                audioSource.clip = walkSounds[Random.Range(0, walkSounds.Length)];
                audioSource.Play();
            }
        }
    }
    
    public GameObject GetPlayerLookingAt()
    {
        return _lookingAt;
    }
    
    public void SetAllowPlayerControl(bool allow)
    {
        AllowPlayerControl = allow;
        Cursor.visible = !allow;
        Cursor.lockState = allow ? CursorLockMode.Locked : CursorLockMode.None;
    }
}