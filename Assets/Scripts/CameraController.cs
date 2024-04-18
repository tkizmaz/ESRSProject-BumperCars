using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 originalPosition;
    private void Start()
    {
        //Keeping original position to return.
        originalPosition = transform.localPosition;
    }

    public IEnumerator CameraShake(float durationOfShake, float magnitudeOfShake)
    {
        //Elapsed time to continue the shake.
        float elapsedTime = 0.0f;

        //Head back time, at the point of the crash, the head should go forwards and it should come back slowly.
        float headBackTime = 15f;

        //While elapsed time is not finished.
        while (elapsedTime < durationOfShake)
        {
            //Getting random in range -1 and 1 to shake the camera on x and y.
            float x = Random.Range(-1f, 1f) * magnitudeOfShake;
            float y = Random.Range(-1f, 1f) * magnitudeOfShake;
            //Getting headback and multiplying with magnitude of shake and crash.
            float z = headBackTime * magnitudeOfShake;

            //Shaking the camera.
            transform.localPosition = new Vector3(x, y, z);

            //Adding time to elapsedTime.
            elapsedTime += Time.deltaTime;

            //Decreasing headback time for head to come slowly to original position.
            headBackTime -= 0.5f;

            //Checking if the position becomes 0, will not let it go below 0.
            if (headBackTime <= 0)
            {
                //Limit it to go below 0.
                headBackTime = 0;
            }

            yield return null;
        }

        //Returning original position.
        transform.localPosition = originalPosition;
    }
}
