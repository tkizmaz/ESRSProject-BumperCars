using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody carBody;
    private BoxCollider carCollider;
    private float moveInput;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float rotationSpeed = 2f;
    private float isGoingForward;
    [SerializeField]
    private GameObject steeringWheel;

    // Start is called before the first frame update
    void Start()
    {
        carBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveCar();
    }

    void MoveCar()
    {
        float currentSpeed = Input.GetAxis("Vertical");
        float rotationSpeedInput = Input.GetAxis("Horizontal");

        Vector3 tempCarVelocity = carBody.velocity;
        isGoingForward = transform.InverseTransformDirection(tempCarVelocity).z;


        int direction = currentSpeed >= 0 ? 1 : 1;
        carBody.AddRelativeForce(Vector3.forward * speed * currentSpeed);
        carBody.AddTorque((Vector3.up * isGoingForward) * rotationSpeed * rotationSpeedInput);

        steeringWheel.transform.Rotate(0f, 0f,10f * -rotationSpeedInput);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Box")
        {
        }
    }
}
