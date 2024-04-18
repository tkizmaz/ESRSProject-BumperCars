using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColorController : MonoBehaviour
{
    //Defining MeshRenderer. 
    [SerializeField] private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //Calling method from BodyColorController class (singleton) called ChooseColor to choose color to given MeshRenderer.
        BodyColorController.instance.ChooseColor(meshRenderer);
    }
}
