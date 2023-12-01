using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAsset : MonoBehaviour
{
    private static GameAsset _i;

    public static string targetWord;

    public enum SpellType
    {
        CREATE,
        CREATE_AT_FIX_POINT
    }

    public static GameAsset i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAsset>("GameAsset")).GetComponent<GameAsset>();
            return _i;
        }
    }

    public SoundAudioClip[] soundAudioClips;

    [System.Serializable]
    public class SoundAudioClip
    {
        public string soundName;
        public AudioClip audioClip;
    }

    public Spell[] spells;

    [System.Serializable]
    public class Spell
    {
        public string word;
        public SpellType spellType;
        public GameObject createdObject;
        public Vector3 position;
        public Quaternion roatation;
    }

    public Alphabet[] alphabets;

    [System.Serializable]
    public class Alphabet
    {
        public string alphbetName;
        public Mesh model;
    }
}
