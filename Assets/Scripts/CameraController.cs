using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    Transform _playerBody;
    
    [Header("Camera Pitch")]
    private float _pitch = 0f;
    private float _pitchMin = -90f;
    private float _pitchMax = 90f;
    
    [Header("Raycast")]
    public float raycastRange = 100f;
    private GameObject _lookingAt;
    
    private LevelManager _levelManager;
    

    void Start()
    {
        _playerBody = transform.parent.transform;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        _levelManager = FindFirstObjectByType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_levelManager.AllowPlayerControl) return;
        
        float moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (_playerBody)
        {
            // yaw rotation
            _playerBody.Rotate(Vector3.up * moveX);
            
            // pitch rotation
            _pitch -= moveY;
            _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
            transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        TargetUpdate();
    }

    void TargetUpdate()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastRange))
        {
            _lookingAt = hit.collider.gameObject;
            
        }
        else _lookingAt = null;
    }
    
    public GameObject GetLookingAt()
    {
        return _lookingAt;
    }
}