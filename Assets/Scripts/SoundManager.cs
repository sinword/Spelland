using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager{

    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;
    private static Dictionary<string, GameObject> playingSound = new Dictionary<string, GameObject>();
    private static Dictionary<string, int> fadingingSound = new Dictionary<string, int>();
    // none : 0, fade in : 1, fade out : 2;

    //public static void PlaySound(Sound sound, Vector3 position)
    //{
    //    GameObject soundGameObject = new GameObject("Sound");
    //    soundGameObject.transform.position = position;
    //    AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
    //    audioSource.clip = GetAudioClip(sound);
    //    audioSource.maxDistance = 100f;
    //    audioSource.spatialBlend = 1f;
    //    audioSource.rolloffMode = AudioRolloffMode.Linear;
    //    audioSource.dopplerLevel = 0f;
    //    audioSource.Play();

    //    Object.Destroy(soundGameObject, audioSource.clip.length);
    //}

    public static void StartPlayingSound(string soundName, float volume = 1)
    {
        AudioSource audioSource = null;
        if (!playingSound.ContainsKey(soundName))
        {
            GameObject soundGameObject = new GameObject(soundName);
            audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(soundName);
            playingSound.Add(soundName, soundGameObject);
        }
        else
        {
            GameObject soundGameObject = playingSound[soundName];
            audioSource = soundGameObject.GetComponent<AudioSource>();
        }

        if (!audioSource.isPlaying)
        {
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    public static void StopPlayingSound(string soundName)
    {
        AudioSource audioSource = null;
        if (playingSound.ContainsKey(soundName))
        {
            GameObject soundGameObject = playingSound[soundName];
            audioSource = soundGameObject.GetComponent<AudioSource>();
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public static IEnumerator FadeInSound(string soundName, float duration, float volume)
    {
        AudioSource audioSource = null;
        if (!playingSound.ContainsKey(soundName))
        {
            GameObject soundGameObject = new GameObject(soundName);
            audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(soundName);
            playingSound.Add(soundName, soundGameObject);
        }
        else
        {
            GameObject soundGameObject = playingSound[soundName];
            audioSource = soundGameObject.GetComponent<AudioSource>();
        }
        fadingingSound[soundName] = 1;
        audioSource.volume = 0;
        audioSource.Play();
        float currentTime = 0;
        while (currentTime < duration && fadingingSound[soundName] == 1)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, volume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public static IEnumerator FadeOutSound(string soundName, float duration)
    {
        AudioSource audioSource = null;
        if (playingSound.ContainsKey(soundName))
        {
            GameObject soundGameObject = playingSound[soundName];
            audioSource = soundGameObject.GetComponent<AudioSource>();
            if (audioSource.isPlaying)
            {
                fadingingSound[soundName] = 2;
                float currentTime = 0;
                float start = audioSource.volume;
                while (currentTime < duration && fadingingSound[soundName] == 2)
                {
                    currentTime += Time.deltaTime;
                    audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
                    yield return null;
                }
                if(fadingingSound[soundName] == 2)
                {
                    audioSource.Stop();
                }
            }
        }
        yield break;
    }

    public static void PlaySound(string soundName, float volume = 1)
    {
        if(oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject(soundName);
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }
        if(GetAudioClip(soundName) != null)
        {
            oneShotAudioSource.PlayOneShot(GetAudioClip(soundName), volume);
        }
    }

    private static AudioClip GetAudioClip(string soundName)
    {
        foreach(GameAsset.SoundAudioClip soundAudioClip in GameAsset.i.soundAudioClips)
        {
            if (soundAudioClip.soundName == soundName)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + soundName + " not found");
        return null;
    }
}
