using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleplayerControllers : MonoBehaviour
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
    [SerializeField]
    private float crashMagnitudeDivider;
    [SerializeField]
    private float forceMultiplier;
    [Header("Camera")]
    [Tooltip("To access camera class.")]
    [SerializeField] private CameraController cameraShake;

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

        steeringWheel.transform.Rotate(0f, 0f, 10f * -rotationSpeedInput);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float crashPower = collision.relativeVelocity.magnitude / crashMagnitudeDivider;
        //Starting coroutine of camera shaking with 0.1 duration and our crashPower.
        StartCoroutine(Camera.main.gameObject.GetComponent<CameraController>().CameraShake(0.4f, crashPower));
        if (collision.rigidbody)
        {

             Vector3 force = transform.forward * carBody.velocity.magnitude * forceMultiplier;
             collision.rigidbody.AddForce(force, ForceMode.Impulse);


        }
    }

}
