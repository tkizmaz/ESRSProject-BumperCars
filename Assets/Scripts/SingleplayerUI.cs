using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleplayerUI : MonoBehaviour
{
    [Header("UI Objects")]
    [Tooltip("To declare gameOverUI parent.")]
    [SerializeField] private GameObject gameOverUI;
    [Tooltip("To declare winText to SetActive when game is over.")]
    [SerializeField] private GameObject winText;
    [Tooltip("To declare loseText to SetActive when game is over.")]
    [SerializeField] private GameObject loseText;

    //To check if game is over.
    private bool isGameOver = false;

    //Performing actions by game statue, over or not by listening GameManager by checking if the player is eliminated.
    void GameOverListener(bool IsPlayerEliminated)
    {
        //Keeping car count temporarily.
        int tempCarCount = GameManager.instance.playerCount;

        //If isPlayerEliminated is true from action by GameManager class.
        if (IsPlayerEliminated && !isGameOver)
        {
            //Set gameOverUI as active.
            gameOverUI.SetActive(true);

            //Set loseText as active.
            loseText.SetActive(true);

            //Setting GameOver as true.
            isGameOver = true;
        }

        //If isPlayerEliminated is false from action by GameManager class. 
        else if (!IsPlayerEliminated && tempCarCount == 1 && !isGameOver)
        {
            //Set gameOverUI as active.
            gameOverUI.SetActive(true);

            //Set winText as active.
            winText.SetActive(true);

            //Setting GameOver as false.
            isGameOver = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //If IsPlayerEliminated action called in GameManager, call GameOverListener in this class.
        GameManager.IsPlayerEliminated += GameOverListener;
    }
}
