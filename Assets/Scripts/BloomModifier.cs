using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class BloomModifier : MonoBehaviour
{
    private Bloom bloomEffect;
    public PostProcessVolume postProcessVolume;

    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out bloomEffect);
    }

    public void SetBloomIntensity(float value)
    {
        bloomEffect.intensity.value = value;
        StartCoroutine(ExampleCoroutine());
    }
    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 0.1 seconds.
        yield return new WaitForSeconds(0.1f);
        bloomEffect.intensity.value = 0;
        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}