using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles hint on hand & Alphabet Bricks twinkle hint to spell the target word.
/// </summary>
public class TargetWordHintManager : MonoBehaviour
{
    [Header("Hint on hand")]
    [Tooltip("The main object, containg the 3D object & its Alphabets.")]
    [SerializeField]
    GameObject hintOnHand;
    [Tooltip("[For Demo] Miniature of Tree.")]
    [SerializeField]
    GameObject treeMiniature;
    [Tooltip("[For Demo] Miniature of Gate.")]
    [SerializeField]
    GameObject gateMiniature;

    GameObject targetWordMiniature;

    [Tooltip("The text of the target word.")]
    [SerializeField]
    AlphabetLayoutGroup alphabetLayoutGroup;

    [Header("Alphabet Bricks twinkle hint")]
    [Tooltip("Material when showing hint")]
    [SerializeField]
    Material hintMaterial;
    [Tooltip("The emission color of hint material")]
    [SerializeField]
    Color emissionColorEnd = new(1f, 115f / 255f, 115f / 255f);
    Color emissionColorStart = Color.black;
    [Tooltip("Show hint after not interacting for this seconds")]
    [SerializeField]
    float showHintAfter = 15f;
    [Tooltip("The duration for the material to change from dark to shiny and the opposite")]
    [SerializeField]
    float hintDuration = 1.0f;
    
    bool hintActive = false;

    #region HintOnHand

    void Start()
    {
        treeMiniature.SetActive(false);
        gateMiniature.SetActive(false);
        hintOnHand.SetActive(false);
    }

    public void SetUpHintOnHand()
    {
        alphabetLayoutGroup.SetUp(GameAsset.targetWord);
        switch (GameAsset.targetWord)
        {
            case "Tree":
                targetWordMiniature = treeMiniature;
                break;
            case "Gate":
                targetWordMiniature = gateMiniature;
                break;
            default:
                break;
        }
        targetWordMiniature.SetActive(true);
    }

    public void ShowHintOnHand()
    {
        hintOnHand.SetActive(true);
    }
    public void CloseHintOnHand()
    {
        hintOnHand.SetActive(false);
    }
    #endregion

    #region TwinkleBricks
    // The Twinkle effect
    IEnumerator AnimateHintColor()
    {
        float time = 0.0f;

        while (true)
        {
            float t = time / hintDuration;
            Color hintColor = Color.Lerp(emissionColorStart, emissionColorEnd, t);
            hintMaterial.SetColor("_EmissionColor", hintColor);

            time += Time.deltaTime;

            if (time >= hintDuration)
            {
                time = 0.0f;
                Color tempColor = emissionColorStart;
                emissionColorStart = emissionColorEnd;
                emissionColorEnd = tempColor;
            }

            yield return null;
        }
    }

    // Show hint of the Alphabet Brick seperately after showHintAfter seconds
    // Start hint twinkle animation when the first after the first showHintAfter seconds
    public IEnumerator ShowBrickHintCountDown(AlphabetBrick brick)
    {
        yield return new WaitForSeconds(showHintAfter);
        brick.ShowHint();
        if (!hintActive)
        {
            StartCoroutine(AnimateHintColor());
            hintActive = true;
        }
    }

    #endregion
}
