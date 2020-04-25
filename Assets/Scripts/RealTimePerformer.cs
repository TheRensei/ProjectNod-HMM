using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RealTimePerformer : MonoBehaviour
{
    public Text label;
    public Text timer;

    RealTimeRecognitionModule recognition;

    bool found = false;

    private void Start()
    {
        recognition = new RealTimeRecognitionModule(9, 7, 2, 10);

        StartCoroutine(Answer());
    }

    IEnumerator Answer()
    {
        float duration = 20f;
        float normalizedTime = 0;

        while (normalizedTime <= 1f && !found)
        {
            GestureType currentGesture = recognition.Decide();
            if (currentGesture == GestureType.Nodding || currentGesture == GestureType.Shaking)
            {
                label.text = currentGesture == GestureType.Nodding ? "Shaking" : "Nodding";

                found = true;
                Debug.LogWarning("Recognized " + currentGesture.ToString());
            }

            normalizedTime += Time.deltaTime / duration;
            timer.text = (normalizedTime * duration).ToString();
            yield return null;
        }

        yield return new WaitForSeconds(10f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

    public void StartTest()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.EnterPlaymode();
#endif
    }
}