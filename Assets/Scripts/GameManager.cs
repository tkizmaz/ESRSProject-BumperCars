using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //To access this class by instance (Singleton Design Pattern).
    public static GameManager instance;
    //Creating action for UIController to listen and if game is over, perform UI actions.
    public static System.Action<bool> IsPlayerEliminated;
    //Declaring player count.
    public int playerCount;
    // Start is called before the first frame update

    //Method to listen actions in ZoneController.
    void DestroyCollisionListener(string carName)
    {
        //If player count is equal to one return.
        if(playerCount == 1)
        {
            return;
        }

        //If there is a collision in Zone, decrease playerCount.
        playerCount--;

        //If the Player is collided with the Zone, meaning game over.
        if (carName == "Player")
        {

            //Calling action created in this class and passing true.
            IsPlayerEliminated(true);
        }

        else
        {
            //Calling action created in this class and passing false.
            IsPlayerEliminated(false);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //If there is a collision detected in ZoneController class, call DestroyCollisionListener.
        ZoneController.DestroyCollision += DestroyCollisionListener;
    }

    //Setting instance as this object at awake.
    private void Awake()
    {
        instance = this;
    }
}
