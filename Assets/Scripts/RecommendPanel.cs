using UnityEngine;
using TMPro;
using Oculus.Interaction.PoseDetection;
/// <summary>
/// The first thing shown in the game, providing the facilitator to choose target word and recommended the corresponding source words.
/// </summary>
public class RecommendPanel : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The panel to adjust the transform of interactive 3D objects, will open after this phase.")]
    TargetManager targetManager;
    [Tooltip("Dropdown of the choosable targets.")]
    [SerializeField]
    TMP_Dropdown targetDropdown;
    [Tooltip("Recommended source words for the first target word in the drop down.")]
    [SerializeField]
    GameObject recommeded1;
    [Tooltip("Recommended source words for the second target word in the drop down.")]
    [SerializeField]
    GameObject recommeded2;

    [Header("Poses")]
    [Tooltip("Disable pose recognition while setup phase, will find in the scene if not assigned.")]
    [SerializeField]
    GameObject poses;

    [Header("Target Word Hint Manager")]
    [Tooltip("The Target Word Hint Manager in this scene, the hint can be set up once the target word is decided, will find in the scene if not assigned.")]
    [SerializeField]
    TargetWordHintManager targetWordHintManager;

    void Start()
    {
        targetDropdown.onValueChanged.AddListener(delegate { UpdateRecommendedByDropdown(); });

        if (poses == null) poses = FindObjectOfType<ShapeRecognizerActiveState>().transform.parent.gameObject;
        poses.SetActive(false);

        if(targetWordHintManager == null) targetWordHintManager = FindObjectOfType<TargetWordHintManager>();
    }

    void UpdateRecommendedByDropdown()
    {
        switch (targetDropdown.value)
        {
            case 0:
                recommeded1.SetActive(true);
                recommeded2.SetActive(false);
                break;
            case 1:
                recommeded1.SetActive(false);
                recommeded2.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ClosePanel()
    {
        // Update the target word for this game.
        GameAsset.targetWord = targetDropdown.options[targetDropdown.value].text;

        targetWordHintManager.SetUpHintOnHand();

        targetManager.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
