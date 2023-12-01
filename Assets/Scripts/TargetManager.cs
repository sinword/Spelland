using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// The panel to set the transform of interactive 3D objects, by using ghost objects.
/// </summary>
public class TargetManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The panel and tool to set everyday objects with Interactable Profiles, will open after this phase.")]
    ProfileManager profileManager;

    [Header("Target object")]
    [Tooltip("[For Demo] the ghost of example target word: Tree.")]
    [SerializeField]    
    GameObject treeGhost;
    [Tooltip("[For Demo] the ghost of example target word: Gate.")]
    [SerializeField]
    GameObject gateGhost;
    [Tooltip("The current target word, shown on the panel.")]
    [SerializeField]
    TMP_Text titleText;
    
    GameObject currentObject;

    [Header("Transform Sliders")]
    [Tooltip("The x cooridnate of the ghost.")]
    [SerializeField]
    Slider xSlider;
    [Tooltip("The z cooridnate of the ghost.")]
    [SerializeField]
    Slider zSlider;
    [Tooltip("The rotation of the ghost.")]
    [SerializeField]
    Slider rotateSlider;

    [Header("Buttons")]
    [SerializeField]
    GameObject okButton;

    void Start()
    {
        switch (GameAsset.targetWord)
        {
            case "Tree":
                currentObject = treeGhost;
                break;
            case "Gate":
                currentObject = gateGhost;
                break;
            default:
                break;
        }
        // Show the ghost and move in front to user.
        currentObject.SetActive(true);
        currentObject.transform.position = new Vector3(0, 0, 2);

        xSlider.onValueChanged.AddListener(delegate { PosX(); });
        zSlider.onValueChanged.AddListener(delegate { PosZ(); });
        rotateSlider.onValueChanged.AddListener(delegate { RotateY(); });
        GetDataToPanel();
    }

    #region SliderInteractions
    void PosX()
    {
        Vector3 newPos = currentObject.transform.position;
        newPos.x = xSlider.value;
        currentObject.transform.position = newPos;
    }

    void PosZ()
    {
        Vector3 newPos = currentObject.transform.position;
        newPos.z = zSlider.value;
        currentObject.transform.position = newPos;
    }

    void RotateY()
    {
        Quaternion target = Quaternion.Euler(0, rotateSlider.value, 0);
        currentObject.transform.rotation = target;
    }
    #endregion

    void GetDataToPanel()
    {
        xSlider.value = currentObject.transform.position.x;
        zSlider.value = currentObject.transform.position.z;
        rotateSlider.value = currentObject.transform.rotation.y;
    }

    // LEGACY
    /* 
    public void NextObject()
    {
        tree.SetActive(false);
        title.text = "Gate";
        gate.transform.position = new Vector3(0, 0, 2);
        gate.SetActive(true);
        currentObject = gate;
        GetDataToPanel();
        nextButton.SetActive(false);
        okButton.SetActive(true);
    }*/

    // Record the transform data and go to Profile Manager, called when the OK button is pressed.
    public void Ok()
    {
        currentObject.SetActive(false);

        foreach (GameAsset.Spell spell in GameAsset.i.spells)
        {            
            if (spell.word.Equals(GameAsset.targetWord, System.StringComparison.CurrentCultureIgnoreCase))
            {
                spell.position = currentObject.transform.position;
                spell.roatation = currentObject.transform.rotation;
            }
        }

        profileManager.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
