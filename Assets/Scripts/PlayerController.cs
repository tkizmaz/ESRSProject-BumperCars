using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
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
    [SerializeField]
    private GameObject cameraHolder;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        if (!IsOwner) return;
        randomNumber.Value = Random.Range(0, 1000);

    }
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        Camera.main.gameObject.AddComponent<CameraController>();
        Transform cameraTransform = Camera.main.gameObject.transform;
        cameraTransform.SetParent(cameraHolder.transform);
        cameraTransform.position = cameraHolder.transform.position;
        cameraTransform.rotation = cameraHolder.transform.rotation;
        carBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
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
        Debug.Log(this.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        float crashPower = collision.relativeVelocity.magnitude / crashMagnitudeDivider;
        //Starting coroutine of camera shaking with 0.1 duration and our crashPower.
        StartCoroutine(Camera.main.gameObject.GetComponent<CameraController>().CameraShake(0.4f, crashPower));
        if (collision.rigidbody)
        {
            if (!IsOwner) return;
            if (collision.rigidbody)
            {
                Debug.Log(collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                Vector3 force = transform.forward * carBody.velocity.magnitude * forceMultiplier;
                ApplyForceToOtherPlayer(collision.rigidbody, force);
            }
        }
    }


    private void ApplyForceToOtherPlayer(Rigidbody otherRigidbody, Vector3 force)
    {

        if (IsServer)
        {
            ApplyForceClientRpc(otherRigidbody.GetComponent<NetworkObject>().NetworkObjectId, force);
        }
        else
        {
            ApplyForceServerRpc(otherRigidbody.GetComponent<NetworkObject>().NetworkObjectId, force);
        }
    }

    [ClientRpc]
    private void ApplyForceClientRpc(ulong networkId, Vector3 force)
    {

        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkId];
        if (networkObject != null)
        {
            Rigidbody otherRigidbody = networkObject.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    [ServerRpc]
    private void ApplyForceServerRpc(ulong networkId, Vector3 force)
    {
        ApplyForceClientRpc(networkId, force);
    }

}
