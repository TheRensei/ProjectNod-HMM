using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureCapture : MonoBehaviour
{
    public float timerDuration = 2f;

    public void StartReading(int input)
    {
        GestureType gT = (GestureType)input;
        //Start a coroutine which will read the angular velocity, add it to the list
        //and after given time the data will be saved to a .json file
        StartCoroutine(RecordGesture(gT));
    }

    private IEnumerator RecordGesture(GestureType gT)
    {
        List<Vector3> angVelList = new List<Vector3>();

        //Timer Duration
        float duration = timerDuration;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            angVelList.Add(OVRManager.display.angularVelocity);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        //Convert list to an array
        Vector3[] angVelArr = angVelList.ToArray();

        //Pass gesture type and angular velocity array to be saved
        GestureSerializer.SaveGestureDataRaw(gT, angVelArr);
    }
}
