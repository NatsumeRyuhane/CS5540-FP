using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    
    private Vector3 input;
    private Vector3 moveDirection;
    private float airControl = 0.5f;
    private CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        input = transform.right * moveHorizontal + transform.forward * moveVertical;
        input.Normalize();
        input *= speed;

        if (controller.isGrounded)
        {
            moveDirection = input;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            }
        }
        else
        {
            // moveDirection.y = 0.0f;
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }
        
        moveDirection.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(Time.deltaTime * moveDirection);
    }
}
