using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make interactable objects to generate alphabets of its spelling
/// </summary>
public class AlphabetGenerator : MonoBehaviour
{
    [Tooltip("The spelling of this object.")]
    public string spelling;

    AlphabetBubble currentAlphabetBubble;
    [Header("Pop-up Alphabet Bubble")]
    [Tooltip("The bubble covering the alphabets.")]
    [SerializeField]
    AlphabetBubble alphabetBubble;

    bool dontCloseBubble = false;

    [Header("Highlight")]
    [Tooltip("The Highlight effect (Outline) when pointed")]
    [SerializeField]
    Outline outlineEffect;

    void OnEnable()
    {
        // Set LayerMask
        int layerIndex = LayerMask.NameToLayer("Alphabet Generator");
        if (-1 == layerIndex)
        {
            Debug.LogError("LayerMask \"Alphabet Generator\" is missing.");
        }
        else
        {
            gameObject.layer = layerIndex;
        }

        // Close some components
        if (outlineEffect == null) outlineEffect = GetComponent<Outline>();
        outlineEffect.enabled = false;

        alphabetBubble.gameObject.SetActive(false);
    }

    void Start()
    {
        alphabetBubble.SetSpelling(spelling);
    }

    #region InteractionsWithPlayer
    public void StartInteraction()
    {
        StartCoroutine(HighlightAndBubble());
    }
    IEnumerator HighlightAndBubble()
    {
        if (AlphabetBubble.ExistsAny()) yield break;
        ShowBubble();
    }
    public void ForcedStopInteraction()
    {
        StopCoroutine(HighlightAndBubble());
        CloseHighlight();
        if (!dontCloseBubble)
        {
            CloseBubble();
        }
    }
    #endregion

    #region InteractionProcesses
    // Highlight when pointed
    public void OpenHighlight()
    {
        // Show Highlight effect
        outlineEffect.enabled = true;
    }
    public void CloseHighlight()
    {
        // Close Highlight effect
        outlineEffect.enabled = false;
    }

    void ShowBubble()
    {
        // No more interaction with this AlphbetGenerator
        GetComponent<Collider>().enabled = false;
        CloseHighlight();        

        alphabetBubble.gameObject.SetActive(true);
    }

    // When stop being pointed while bubble is shown
    void CloseBubble()
    {
        alphabetBubble.ForcedCLose();
    }

    #endregion
}
