using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRVRGate : MonoBehaviour
{
    void Start()
    {
        Open();
    }

    void Open()
    {
        SoundManager.PlaySound("Gateopen");
        StartCoroutine(LoadToVR());
    }

    IEnumerator LoadToVR()
    {
        yield return new WaitForSeconds(5);
        FindObjectOfType<SceneChanger>().FadeToScene(1);
    }
}
