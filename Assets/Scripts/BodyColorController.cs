using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyColorController : MonoBehaviour
{
    //To access this class by instance (Singleton Design Pattern).
    public static BodyColorController instance;
    [Header("Color Pool")]
    [Tooltip("Color Pool to set colors.")]
    [SerializeField] private List<Color> colorPool;

    //Setting instance as this object at awake.
    void Awake()
    {
        instance = this;
    }

    //Changes color of given MeshRenderer.
    public void ChooseColor(MeshRenderer bodyRenderer)
    {
        //Getting random integer from 0 to length of colorPool.
        int random = Random.Range(0, colorPool.Count);

        //Setting color of bodyRenderers'second material to choose randomly with random integer.
        bodyRenderer.materials[1].color = colorPool[random];

        //Removing selected color from List.
        colorPool.Remove(colorPool[random]);
    }
}
