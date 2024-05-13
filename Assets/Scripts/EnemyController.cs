using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //Target to follow and hit.
    private GameObject target;
    //Target list to create target pool.
    private List<GameObject> targets;
    //Rigidbody of enemy.
    private Rigidbody agentBody;
    [Header("Speed Variables")]
    [Tooltip("To declare speed of Enemy.")]
    [SerializeField] private float speedOfEnemy;
    [Tooltip("To declare faceTurnSpeed of Enemy to turn slowly.")]
    [SerializeField] private float faceTurnSpeed = 5f;
    [Header("Duration Variable")]
    [Tooltip("To declare stayDuration at collision.")]
    [SerializeField] private float stayDuration;



    //Random is to give a little position difference in x axis.
    private float randomX;

    // Start is called before the first frame update
    void Start()
    {
        //Setting Rigidbody.
        agentBody = GetComponent<Rigidbody>();
        //Setting random at the start.
        ChangeRandom();
    }

    //Check the area for targets.
    List<GameObject> CheckAreaForTargets()
    {
        //Getting all objects with the tag Car as an array.
        var tempTargets = GameObject.FindGameObjectsWithTag("Car");
        //Changing array of game objects to List.
        targets = new List<GameObject>(tempTargets);
        //Removing this game object from the list to avoid self target.
        targets.Remove(this.gameObject);
        //Returning target list.
        return targets;

    }

    //Locks the target.
    void LockTarget()
    {
        //If there are enemies left in area.
        if(CheckAreaForTargets().Count > 0)
        {
            //Lock randomly.
            target = targets[Random.Range(0, targets.Count)];
        }      
    }

    private void FixedUpdate()
    { 
        //If there is a target.
        if (target)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x + randomX, target.transform.position.y, target.transform.position.z);
            //Calculating the position to move from enemy's position to target position with the speed.
            Vector3 positionToMove = Vector3.MoveTowards(transform.position, targetPosition, speedOfEnemy * Time.fixedDeltaTime);
            //It will move to the position.
            agentBody.MovePosition(positionToMove);
            //Declaring rotation to look at.
            var rotation = Quaternion.LookRotation(targetPosition - transform.position);
            //Enemy will rotate towards target with a speed.
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * faceTurnSpeed);
        }

        //If target is not specified.
        else
        {
            //Since the target is not selected, find target.
            LockTarget();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If collided object has rigidbody.
        if (collision.rigidbody)
        {
            //Adding impulse to it with velocity of the car.
            collision.rigidbody.AddForce(transform.forward * agentBody.velocity.x * 2);
        }

        //If collided object has rigidbody and velocity magnitude is greater than 2;
        if(collision.rigidbody && collision.rigidbody.velocity.magnitude> 2)
        {
            //Changing random to select an enemy.
            ChangeRandom();
        }
        
    }

    //If collision stayed too long between to objects.
    private void OnCollisionStay(Collision collision)
    {
        //If collided object has tag "Car".
        if (collision.gameObject.tag == "Car")
        {
            //Timer will start.
            if (stayDuration > 0)
            {
                stayDuration -= Time.deltaTime;
            }

            //If timer has ended for a collision that stayed too long, find another target.
            else
            {
                //Find another target.
                LockTarget();
                //Reset the duration.
                stayDuration = 5f;
            }  
        }
    }

    private void ChangeRandom()
    {
        //Changing random in the range -2.5f and 2.5f. This random is for enemy to move towards with a little error to simulate reality. Not literally locked.
        randomX = Random.Range(-2.5f, 2.5f);
    }

}