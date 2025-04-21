using System;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public float mouseSensitivity = 100f;
    Transform _playerBody;

    [Header("Camera Pitch")] private float _pitch = 0f;
    private float _pitchMin = -90f;
    private float _pitchMax = 90f;

    [Header("Raycast")] public float raycastRange = 100f;
    private GameObject _lookingAt;
    


    void Start()
    {
        _playerBody = transform.parent.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        TargetUpdate();
        if (!PlayerController.Instance.AllowPlayerControl) return;

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

    void TargetUpdate()
    {
        _lookingAt = Physics.Raycast(transform.position, transform.forward, out var hit, raycastRange) ? hit.collider.gameObject : null;
    }

    public GameObject GetLookingAt()
    {
        return _lookingAt;
    }
}