using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneScripts : MonoBehaviour
{
    public void ChangeToSingleplayerScene()
    {
        SceneManager.LoadScene(1);
    }
}
