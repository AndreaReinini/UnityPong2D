using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TestLight : MonoBehaviour
{
    public Light2D light2D;
    public float targetIntensity;
    public float transitionDuration;
    public float defaultIntensity;
    public float resetDelay;

    private bool isTransitioning;
    private float initialIntensity;

    private void Start()
    {
        initialIntensity = light2D.intensity;
    }

    public void StartLightEffect()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ChangeIntensityCoroutine(targetIntensity));
        }
    }

    private System.Collections.IEnumerator ChangeIntensityCoroutine(float target)
    {
        isTransitioning = true;

        float startTime = Time.time;
        float endTime = startTime + transitionDuration;

        float startIntensity = light2D.intensity;
        float endIntensity = target;

        while (Time.time <= endTime)
        {
            float t = (Time.time - startTime) / transitionDuration;
            light2D.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            yield return null;
        }

        light2D.intensity = endIntensity;

        yield return new WaitForSeconds(resetDelay);

        startTime = Time.time;
        endTime = startTime + transitionDuration;

        startIntensity = light2D.intensity;
        endIntensity = defaultIntensity;

        while (Time.time <= endTime)
        {
            float t = (Time.time - startTime) / transitionDuration;
            light2D.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            yield return null;
        }

        light2D.intensity = endIntensity;
        isTransitioning = false;
    }
}
