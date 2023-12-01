using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Oculus.Interaction.PoseDetection;
/// <summary>
/// Adjust Interactable Profiles with UI.
/// </summary>
public class ProfileManager : MonoBehaviour
{
    InteractableProfile currentProfile;
    List<InteractableProfile> allProfiles;
    bool isEditing = false;

    [Header("Profile")]
    [Tooltip("Prefab of the Interactable Profile Object.")]
    [SerializeField]
    GameObject profilePrefab;
    [SerializeField]
    Transform profileGeneratePoint;
    [SerializeField]
    Material profileMaterial;
    [SerializeField]
    Color profileMaterialColor;

    [Header("Panels")]
    [Tooltip("Handles generating profile, stop editing a profile.")]
    [SerializeField]
    GameObject mainButton;
    [Tooltip("Exit setup mode, hide when Edit Panel is opened.")]
    [SerializeField]
    GameObject exitButton;
    [Tooltip("The panel to edit spelling and scaling of the profile")]
    [SerializeField]
    GameObject editPanel;

    [Header("Main Button Components")]
    [SerializeField]
    [Tooltip("Switch between \"Generate Profile\" and \"Stop Editing\".")]
    TMP_Text mainButtonTitle;

    [Header("Spelling")]
    [SerializeField]
    TMP_Dropdown spellingDropdown;
    [SerializeField]
    GameObject spellingUserDefinedArea;
    [SerializeField]
    TMP_Text spellingUserDefinedText;

    [Header("Scaling")]
    [Tooltip("The x-axis scale of the current Interactable Profiles.")]
    [SerializeField]
    Slider xaxisSlider;
    [Tooltip("The y-axis scale of the current Interactable Profiles.")]
    [SerializeField]
    Slider yaxisSlider;
    [Tooltip("The z-axis scale of the current Interactable Profiles.")]
    [SerializeField]
    Slider zaxisSlider;

    [Header("Voice")]
    [SerializeField]
    VoiceInteractionHandler voiceInteractionHandler;
    [Tooltip("UI to show when listening user's voice input for custom word.")]
    [SerializeField]
    GameObject listeningIcon;

    [Header("Poses")]
    [Tooltip("Enable pose recognition after this phase")]
    [SerializeField]
    GameObject poses;

    [Header("Reset")]
    [Tooltip("The Reset Manager in this scene, will find in the scene if not assigned.")]
    [SerializeField]
    ResetManager resetManager;

    void Start()
    {
        allProfiles = new List<InteractableProfile>();

        profileMaterial.SetColor("_BaseColor", profileMaterialColor);

        if (voiceInteractionHandler == null) voiceInteractionHandler = GetComponentInChildren<VoiceInteractionHandler>();

        spellingDropdown.onValueChanged.AddListener(delegate { UpdateSpellingByDropdown(); });
        
        xaxisSlider.onValueChanged.AddListener(delegate { ScaleX(); });
        yaxisSlider.onValueChanged.AddListener(delegate { ScaleY(); });
        zaxisSlider.onValueChanged.AddListener(delegate { ScaleZ(); });

        spellingUserDefinedArea.SetActive(false);
        editPanel.SetActive(false);
        exitButton.SetActive(true);
        listeningIcon.SetActive(false);

        if (poses == null) poses = FindObjectOfType<ShapeRecognizerActiveState>().transform.parent.gameObject;
        poses.SetActive(false);

        if (resetManager == null) resetManager = FindObjectOfType<ResetManager>();
    }

    #region UpdateSpelling
    public void StartVoiceInteraction()
    {
        if (ASRManager.UseASR)
        {
            listeningIcon.SetActive(true);
            voiceInteractionHandler.ToggleActivation();
        }
    }

    public void UpdateSpellingUserDefinedText(string newText)
    {
        spellingUserDefinedText.text = newText;
    }

    public void UpdateSpellingByInput(string input)
    {
        string newSpelling = input;
        currentProfile.spelling = newSpelling;
        spellingUserDefinedText.text = newSpelling;
        listeningIcon.SetActive(false);
    }

    void UpdateSpellingByDropdown()
    {
        string newSpelling = spellingDropdown.options[spellingDropdown.value].text;

        if (newSpelling == "(User-defined)")
        {
            currentProfile.spelling = "";
            spellingUserDefinedArea.SetActive(true);
            if (!ASRManager.UseASR)
            {
                spellingUserDefinedText.text = "ASR is inactive...";
            }
            else
            {
                spellingUserDefinedText.text = "Press and speak...";
            }
        }
        else
        {
            currentProfile.spelling = newSpelling;
            spellingUserDefinedArea.SetActive(false);
        }
    }
    #endregion
    void InitEditPanel()
    {
        spellingDropdown.value = 0;
        xaxisSlider.value = 1;
        yaxisSlider.value = 1;
        zaxisSlider.value = 1;
    }
    #region SliderInteractions
    void ScaleX()
    {
        Vector3 newScale = currentProfile.transform.localScale;
        newScale.x = xaxisSlider.value;
        currentProfile.transform.localScale = newScale;
    }

    void ScaleY()
    {
        Vector3 newScale = currentProfile.transform.localScale;
        newScale.y = yaxisSlider.value;
        currentProfile.transform.localScale = newScale;
    }

    void ScaleZ()
    {
        Vector3 newScale = currentProfile.transform.localScale;
        newScale.z = zaxisSlider.value;
        currentProfile.transform.localScale = newScale;
    }
    #endregion

    #region ButtonEvents
    // To put in InteractableUnityEventWrapper through Inspector for convenience.
    public void onMainButtonReleased()
    {
        SwitchEditMode();
    }
    public void onExitButtonReleased()
    {
        ExitSetupMode();
    }
    #endregion
    
    #region EditMode
    void StartEditing()
    {
        mainButtonTitle.text = "Stop Editing";
        // Hide Exit Button
        exitButton.SetActive(false);
        // Show Edit Panel
        editPanel.SetActive(true);
    }

    void StopEditing()
    {
        mainButtonTitle.text = "Generate Profile";
        // Hide Edit Panel
        editPanel.SetActive(false);
        // Show Exit Button
        exitButton.SetActive(true);

        currentProfile = null;
    }

    void SwitchEditMode()
    {
        if (isEditing)
        {
            StopEditing();
        }
        else
        {
            GenerateProfile();
            StartEditing();
        }
        isEditing = !isEditing;
    }
    #endregion

    public void SelectProfile(InteractableProfile newProfile)
    {
        if (newProfile != currentProfile)
        {
            currentProfile = newProfile;
            if (!isEditing)
            {
                StartEditing();
            }
            GetProfileDataToPanel();
            isEditing = true;
        }
    }

    void GenerateProfile()
    {
        GameObject newProfileObject = Instantiate(profilePrefab, profileGeneratePoint.position, Quaternion.identity);
        currentProfile = newProfileObject.GetComponentInChildren<InteractableProfile>();
        resetManager.AddToDestroyWhenReset(newProfileObject);
        // lazy initialization
        currentProfile.spelling = "Teapot";
        allProfiles.Add(currentProfile);
        GetProfileDataToPanel();
    }

    void GetProfileDataToPanel()
    {
        string currentSpelling = currentProfile.spelling;
        int dropdownIndex = -1;
        for (int i = 0; i < spellingDropdown.options.Count-1; i++)
        {
            if (currentSpelling == spellingDropdown.options[i].text)
            {
                dropdownIndex = i;
            }
        }
        if (-1 == dropdownIndex)
        {
            dropdownIndex = spellingDropdown.options.Count - 1;
            spellingUserDefinedText.text = currentSpelling;
        }
        spellingDropdown.value = dropdownIndex;

        xaxisSlider.value = currentProfile.transform.localScale.x;
        yaxisSlider.value = currentProfile.transform.localScale.y;
        zaxisSlider.value = currentProfile.transform.localScale.z;
    }

    void ExitSetupMode()
    {
        foreach (InteractableProfile profile in allProfiles)
        {
            // Prepare for reset            
            resetManager.AddToProfilesToReset(profile);

            // Make the interactable objects to Alphabet Generators
            profile.SwitchToAlphabetGenerator();
        }

        // Set the color of profiles to transparent
        Color materialColor = profileMaterial.color;
        materialColor.a = 0f;
        profileMaterial.SetColor("_BaseColor", materialColor);

        // Start pose recogntion
        poses.SetActive(true);

        // Close this and UIs
        gameObject.SetActive(false);
    }
}
