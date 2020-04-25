using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Slider timerBarOnScreen;
    public Slider timerBarWorldSpace;
    public GestureCapture gCap;
    public List<Button> buttonList;
    public Text txt;
    float timerDuration = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        timerDuration = gCap.timerDuration;
    }

    public void StartTimer()
    {
        StartCoroutine(counter());
    }

    IEnumerator counter()
    {
        DisableButtons();
        //Timer Duration
        float duration = timerDuration;
        float normalizedTime = 0;

        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            timerBarOnScreen.value = normalizedTime;
            timerBarWorldSpace.value = timerBarOnScreen.value;
            yield return null;
        }

        //Enable buttons after starting reading
        txt.text = " ";
        timerBarOnScreen.value = 0;
        timerBarWorldSpace.value = 0;
        EnableButtons();
    }

    public void DisableButtons()
    {
        foreach(Button a in buttonList)
        {
            a.interactable = false;
        }
    }

    public void EnableButtons()
    {
        foreach (Button a in buttonList)
        {
            a.interactable = true;
        }
    }

}
