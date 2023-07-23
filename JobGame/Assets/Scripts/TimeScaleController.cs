using UnityEngine;
using System.Collections;

public class TimeScaleController : MonoBehaviour
{
    private float targetTimeScale = 3f;
    private float timeScaleIncrement = 0.03f;

    public void StartIncreasing()
    {
        StartCoroutine(IncreaseTimeScale());
    }
    public void StopIncreasing()
    {
        StopCoroutine(IncreaseTimeScale());
    }

    IEnumerator IncreaseTimeScale()
    {
        while (Time.timeScale < targetTimeScale)
        {
            Time.timeScale += timeScaleIncrement;

            yield return new WaitForSeconds(2f);
        }
    }
}
