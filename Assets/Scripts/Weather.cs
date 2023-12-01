using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    public float durationTime = 30.0f;
    public Material weatherSkyBox;
    public string weatherName;
    Material originSkyBox;

    // Start is called before the first frame update
    void Start()
    {
        originSkyBox = RenderSettings.skybox;
        StartCoroutine(WeatherDuration());
    }

    IEnumerator WeatherDuration()
    {
        StartCoroutine(SoundManager.FadeInSound(weatherName, 5f, 1f));
        RenderSettings.skybox = weatherSkyBox;
        yield return new WaitForSeconds(durationTime-5f);
        StartCoroutine(SoundManager.FadeOutSound(weatherName, 5f));
        yield return new WaitForSeconds(5f);
        RenderSettings.skybox = originSkyBox;
        gameObject.SetActive(false);
    }
}
