using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouvement : MonoBehaviour
{
    [SerializeField] private Transform camera;
    float mouseSensitivity = 2f;
    private float upLimit = -10f;
    private float downLimit = 30f;
    [SerializeField] private float speed = 3f;
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }
    void Move()
    {
        //float horizontalMove = Input.GetAxis("Vertical");
        //float verticalMove = Input.GetAxis("Horizontal");

        //Vector3 move = transform.forward * verticalMove + transform.right * (-horizontalMove);

        //transform.position += move * Time.deltaTime;

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            GetComponent<Animator>().SetBool("walk", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("walk", false);
        }

        moveDirection = new Vector3(-Input.GetAxis("Vertical"), 0, 0 );
        moveDirection = transform.TransformDirection(moveDirection);

        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * Time.deltaTime * speed * 30);

        transform.position += moveDirection * Time.deltaTime * speed;
    }

    void Rotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else if(Input.GetMouseButton(1))
        {
            float horizontalRotation = Input.GetAxis("Mouse X");
            float verticalRotation = Input.GetAxis("Mouse Y");

            transform.Rotate(0, horizontalRotation * mouseSensitivity, 0);
            camera.Rotate(-verticalRotation * mouseSensitivity, 0,0);

            Vector3 currentRotation = camera.localEulerAngles;
            if (currentRotation.x > 180)
                currentRotation.x -= 360;
            currentRotation.x = Mathf.Clamp(currentRotation.x, upLimit, downLimit);
            camera.localRotation = Quaternion.Euler(currentRotation);
        }
        
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
