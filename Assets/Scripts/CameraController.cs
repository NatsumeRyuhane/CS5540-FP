using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    Transform _playerBody;
    
    private float _pitch = 0f;
    private float _pitchMin = -90f;
    private float _pitchMax = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerBody = transform.parent.transform;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
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
}