using UnityEngine;
/// <summary>
/// Handles the Usage of ASR (Automatic Speech Recognition)
/// </summary>
public class ASRManager : MonoBehaviour
{
    // Whether actually use ASR in this project
    public static bool UseASR { get; private set; }

    static ASRManager()
    {
        UseASR = false;
    }

    // Toggle the usage of ASR
    public static void ToggleUseASR()
    {
        UseASR = !UseASR;
        Debug.Log("ToggleASR: " + UseASR);
        SoundManager.PlaySound("UI_Click");
    }
}
