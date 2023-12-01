using System.Collections;
using UnityEngine;
using TMPro;
using Facebook.WitAi.TTS.Utilities;

public class AlphabetBubble : MonoBehaviour
{
    [Header("Initialization")]
    [Tooltip("The spelling of the object this bubble is from.")]
    public string spelling;
    [Tooltip("The initial position")]
    [SerializeField]
    Transform generatePoint;
    [Tooltip("The bubble faces the camera when generated, will find in the scene if not assigned.")]
    [SerializeField]
    Transform cameraPoint;
    [Tooltip("The distance from camera for the bubble to fly to when its to far.")]
    [SerializeField]
    float endPointDistanceFromCamera = 1f;
    [Tooltip("The distance from camera less than this has no need to fly.")]
    [SerializeField]
    float maxNoFlyDistance = 1f;

    [Header("Alphabet")]
    [Tooltip("Alphabet objects will be put under this.")]
    [SerializeField]
    AlphabetLayoutGroup alphabetLayoutGroup;

    [Header("Animations")]
    [Tooltip("The animator of UI contents.")]
    [SerializeField]
    Animator UIAnimator;
    [Tooltip("The animator of breaking the bubble.")]
    [SerializeField]
    Animator DoneAnimator;
    [Tooltip("The time for bubble to move towards the player.")]
    [SerializeField]
    float bubbleMoveDuration = 2f;

    [Header("Voice")]
    [SerializeField]
    VoiceInteractionHandler voiceInteractionHandler;
    [SerializeField]
    bool showVoiceLog = false;
    [SerializeField]
    TMP_Text voiceLogText;
    [SerializeField]
    TTSSpeaker ttsspeaker;

    // Whether there is a bubble in the scene now.
    static bool bubbleExists;

    // initialize the static bool variable bubbleExists
    static AlphabetBubble()
    {
        bubbleExists = false;
    }

    void Start()
    {
        if (spelling == null) Debug.LogError("AlphabetBubble's spelling is not set correctly!");

        if (cameraPoint == null) cameraPoint = GameObject.Find("CenterEyeAnchor").transform;
        if (voiceInteractionHandler == null) voiceInteractionHandler = GetComponentInChildren<VoiceInteractionHandler>();
        if (ttsspeaker == null) ttsspeaker = FindObjectOfType<TTSSpeaker>();

        // Initialize Transform
        transform.position = generatePoint.position;
        transform.LookAt(cameraPoint, Vector3.up);

        voiceLogText.transform.parent.gameObject.SetActive(showVoiceLog);

        SoundManager.PlaySound("Bubble_Appear", 0.9f);

        bubbleExists = true;

        StartInteraction();
    }

    void OnDisable()
    {
        ResetState();
    }

    public void SetSpelling(string newSpelling)
    {
        spelling = newSpelling;
        alphabetLayoutGroup.SetUp(newSpelling);
    }

    // Start the interaction (Listen&Speak) with the player
    public void StartInteraction()
    {
        Debug.Log(spelling+"<color=red> StartInteraction()</color>");
        StartCoroutine(InteractionPipeline());
    }

    // TTS show the example promouciation
    void SaySpelling()
    {
        try
        {
            if (ttsspeaker.IsLoading || ttsspeaker.IsSpeaking)
            {
                ttsspeaker.Stop();
            }
            ttsspeaker.Speak(spelling);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            UpdateVoiceLog(e.ToString());
        }
    }

    // The main pipeline of the interaction
    IEnumerator InteractionPipeline()
    {
        
        yield return new WaitForSeconds(1);
        if (gameObject.activeSelf)
        {
            UIAnimator.SetTrigger("Listen");
            yield return new WaitForSeconds(1.5f);
            SaySpelling();
        }
        else
        {
            // Stop the interaction if the buuble is not active (not pointed) after 1 second
            ForcedCLose();
            yield break;
        }
        yield return new WaitForSeconds(4);
        
        // Whether actually use ASR
        if (ASRManager.UseASR)
        {
            // Speech Recognition
            if (gameObject.activeSelf)
            {
                Debug.Log(spelling +" voiceInteractionHandler.ToggleActivation();");
                voiceInteractionHandler.ToggleActivation();
            }
            else
            {
                ForcedCLose();
                yield break;
            }
        }
        else
        {
            // Forced Correct Answer
            yield return new WaitForSeconds(4);
            CheckAnswer(spelling);
            yield break;
        }
    }

    public void CheckAnswer(string answer)
    {
        Debug.Log($"Answer: <color=blue>{answer}</color>");
        if (showVoiceLog)
        {
            voiceLogText.text = answer;
        }
        if (spelling.Equals(answer))
        {
            UIAnimator.SetTrigger("Correct");
            SoundManager.PlaySound("Spell_Correct", 0.45f);
            Vector3 cameraPosSnapShot = cameraPoint.position;
            Vector3 heading = transform.position - cameraPosSnapShot;
            float dist = heading.magnitude;
            if (dist > maxNoFlyDistance)
            {
                Vector3 endPoint = cameraPosSnapShot + (heading / dist) * endPointDistanceFromCamera;
                StartCoroutine(SendBubbleAndPop(endPoint));
            }
            else
            {
                PopBubble();
            }
        }
        else
        {
            // Wrong answer
            voiceLogText.text += " (Wrong)";
            SoundManager.PlaySound("Spell_Wrong", 0.9f);
            RestartInteraction();
        }
    }

    IEnumerator SendBubbleAndPop(Vector3 alphabetBubbleEndPointSnapShot)
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(SendBubbleToPlayer(alphabetBubbleEndPointSnapShot));
        yield return new WaitForSeconds(bubbleMoveDuration);
        PopBubble();
    }
    IEnumerator SendBubbleToPlayer(Vector3 alphabetBubbleEndPointSnapShot)
    {
        // Simple moving toward, should improve.
        Vector3 startPosition = transform.position;
        float timeElapsed = 0f;
        while (timeElapsed < bubbleMoveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, alphabetBubbleEndPointSnapShot, timeElapsed / bubbleMoveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = alphabetBubbleEndPointSnapShot;
    }

    // The end of a successful interaction
    void PopBubble()
    {
        DoneAnimator.SetTrigger("Done");
        alphabetLayoutGroup.Dismiss();
        SoundManager.PlaySound("Bubble_Pop", 0.85f);
        ForcedCLose();
    }

    // Reset Animators
    void ResetState()
    {
        UIAnimator.ResetTrigger("Listen");
        UIAnimator.ResetTrigger("Correct");
        DoneAnimator.ResetTrigger("Done");
        UIAnimator.SetTrigger("Restart");
    }

    void RestartInteraction()
    {
        Debug.Log(spelling + "<color=red> RestartInteraction()</color>");
        // TBD: Show "Try again" hint.
        voiceLogText.text += ", Try Again!";
        ResetState();
        StartCoroutine(InteractionPipeline());
    }

    public void UpdateVoiceLog(string newLog)
    {
        if (showVoiceLog)
        {
            voiceLogText.text = newLog;
        }
    }

    // Close this bubble forcibly
    public void ForcedCLose()
    {
        if (gameObject.activeSelf)
        {
            bubbleExists = false;
            gameObject.SetActive(false);
        }
    }

    // For other classes to check
    public static bool ExistsAny()
    {
        return bubbleExists;
    }
}
