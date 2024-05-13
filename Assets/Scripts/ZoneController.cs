using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    //Creating action DestroyCollision with the parameter type string, that will trigger by listener of this action.
    public static System.Action<string> DestroyCollision;

    //If there is a collision.
    private void OnCollisionEnter(Collision collision)
    {

        //If the tag of the collided object is Car.
        if (collision.gameObject.tag == "Car")
        {
            Debug.Log("girdi");
            //Trigger the action with the collided object's name.
            DestroyCollision(collision.gameObject.name);
            //Destroy the object.
            Destroy(collision.gameObject);

        }
    }
}
