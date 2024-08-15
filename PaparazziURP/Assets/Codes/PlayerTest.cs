using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [Header("移動")]
    public float speed;
    public bool canMove;
    public CharacterController playerCC;
    Vector3 moveVelocity;
    float moveVelocityY;

    [Header("第一人稱")]
    public Camera cam;
    public float mouseSensitivity;
    private float verticalLookRotation;
    // Start is called before the first frame update
    void Start()
    {
        playerCC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Look();
    }
    private void Move()
    {
        if (!canMove)
        {
            return;
        }
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveVelocity = transform.right * moveX * speed + transform.forward * moveZ * speed;

        if (playerCC.isGrounded)
        {
            moveVelocityY = 0f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float jumpSpeed = 15f;
                moveVelocityY = jumpSpeed;
            }
        }

        float gravityDownForce = -60f;
        moveVelocityY += gravityDownForce * Time.deltaTime;

        moveVelocity.y = moveVelocityY;

        playerCC.Move(moveVelocity * Time.deltaTime);
    }
    public void Look()
    {
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cam.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
    }
}
