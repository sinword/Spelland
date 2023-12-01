using Facebook.WitAi;
using Facebook.WitAi.Lib;
using Oculus.Voice;
using UnityEngine;
/// <summary>
/// Handle speech to text, almost same as the sample in a Meta's example scene.
/// </summary>
public class VoiceInteractionHandler : MonoBehaviour
{
    string lastResponse;

    [Header("Voice")]
    [SerializeField]
    AppVoiceExperience appVoiceExperience;
    
    enum UsageFor
    {
        AlphabetBubble,
        ProfileManager
    }

    [Header("Usage")]
    [Tooltip("Where does this interaction happens.")]
    [SerializeField]
    UsageFor usageFor;
    [SerializeField]
    AlphabetBubble alphabetBubble;
    [SerializeField]
    ProfileManager profileManager;


    // Whether voice is activated
    public bool IsActive => _active;
    private bool _active = false;

    private void Start()
    {
        // If not assigned
        if (appVoiceExperience == null) appVoiceExperience = GetComponentInChildren<AppVoiceExperience>();
        if (usageFor == UsageFor.ProfileManager & profileManager == null) profileManager = FindObjectOfType<ProfileManager>();
    }

    // Add delegates
    private void OnEnable()
    {
        appVoiceExperience.events.OnRequestCreated.AddListener(OnRequestStarted);
        appVoiceExperience.events.OnPartialTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.events.OnFullTranscription.AddListener(OnRequestTranscript);
        appVoiceExperience.events.OnStartListening.AddListener(OnListenStart);
        appVoiceExperience.events.OnStoppedListening.AddListener(OnListenStop);
        appVoiceExperience.events.OnStoppedListeningDueToDeactivation.AddListener(OnListenForcedStop);
        appVoiceExperience.events.OnStoppedListeningDueToInactivity.AddListener(OnListenForcedStop);
        appVoiceExperience.events.OnResponse.AddListener(OnRequestResponse);
        appVoiceExperience.events.OnError.AddListener(OnRequestError);
    }
    // Remove delegates
    private void OnDisable()
    {
        appVoiceExperience.events.OnRequestCreated.RemoveListener(OnRequestStarted);
        appVoiceExperience.events.OnPartialTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.events.OnFullTranscription.RemoveListener(OnRequestTranscript);
        appVoiceExperience.events.OnStartListening.RemoveListener(OnListenStart);
        appVoiceExperience.events.OnStoppedListening.RemoveListener(OnListenStop);
        appVoiceExperience.events.OnStoppedListeningDueToDeactivation.RemoveListener(OnListenForcedStop);
        appVoiceExperience.events.OnStoppedListeningDueToInactivity.RemoveListener(OnListenForcedStop);
        appVoiceExperience.events.OnResponse.RemoveListener(OnRequestResponse);
        appVoiceExperience.events.OnError.RemoveListener(OnRequestError);
    }

    // Request began
    private void OnRequestStarted(WitRequest r)
    {
        Debug.Log("OnRequestStarted(WitRequest r): "+r);
        r.onRawResponse = (response) => Debug.Log(response);
        // Begin
        _active = true;
        lastResponse = null;
    }
    // Request transcript
    private void OnRequestTranscript(string transcript)
    {
        // Not used in current case
    }
    // Listen start
    private void OnListenStart()
    {
        Debug.Log("OnListenStart()");
        switch (usageFor)
        {
            case UsageFor.AlphabetBubble:
                alphabetBubble.UpdateVoiceLog("Listening...");
                break;
            case UsageFor.ProfileManager:
                profileManager.UpdateSpellingUserDefinedText("Listening...");
                break;
            default:
                break;
        }
    }
    // Listen stop
    private void OnListenStop()
    {
        Debug.Log("OnListenStop()");
        switch (usageFor)
        {
            case UsageFor.AlphabetBubble:
                alphabetBubble.UpdateVoiceLog("Processing...");
                break;
            case UsageFor.ProfileManager:
                profileManager.UpdateSpellingUserDefinedText("Processing...");
                break;
            default:
                break;
        }
    }
    // Listen stop
    private void OnListenForcedStop()
    {
        Debug.Log("OnListenForcedStop()");
        OnRequestComplete();
    }
    // Request response
    private void OnRequestResponse(WitResponseNode response)
    {
        Debug.Log("OnRequestResponse(WitResponseNode response)");
        if (!string.IsNullOrEmpty(response["text"]))
        {
            lastResponse = response["text"];
        }
        else
        {
            lastResponse = "Didn't hear.";
        }
        OnRequestComplete();
    }
    // Request error
    private void OnRequestError(string error, string message)
    {
        Debug.LogError($"<color=\"red\">Error: {error}\n{message}</color>");
        switch (usageFor)
        {
            case UsageFor.AlphabetBubble:
                alphabetBubble.UpdateVoiceLog($"<color=\"red\">Error: {error}\n{message}</color>");
                break;
            case UsageFor.ProfileManager:
                profileManager.UpdateSpellingByInput($"<color=\"red\">Error: {error}\n{message}</color>");
                break;
            default:
                break;
        }
        OnRequestComplete();
    }
    // Deactivate
    private void OnRequestComplete()
    {
        Debug.Log("OnRequestComplete()");
        _active = false;
        switch (usageFor)
        {
            case UsageFor.AlphabetBubble:
                alphabetBubble.CheckAnswer(lastResponse);
                break;
            case UsageFor.ProfileManager:
                profileManager.UpdateSpellingByInput(lastResponse);
                break;
            default:
                break;
        }
    }

    // Toggle activation
    public void ToggleActivation()
    {
        SetActivation(!_active);
    }
    // Set activation
    public void SetActivation(bool toActivated)
    {
        if (_active != toActivated)
        {
            _active = toActivated;
            if (_active)
            {
                appVoiceExperience.Activate();
            }
            else
            {
                appVoiceExperience.Deactivate();
            }
        }
    }
}
