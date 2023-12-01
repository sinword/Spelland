using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Blocks on the Speller Board.
/// </summary>
public class Block : MonoBehaviour
{
    SpellerBoard spellerBoard;
    AlphabetBrick alphabetInTheBlock = null;

    [Tooltip("The deafult frame.")]
    [SerializeField]
    Image frameDefault;
    [Tooltip("The frame with emission.")]
    [SerializeField]
    Image frameEmission;

    private void Start()
    {
        spellerBoard = transform.parent.GetComponent<SpellerBoard>();
        frameEmission.gameObject.SetActive(false);
    }

    public string GetAlphabetInTheBlock()
    {
        if(alphabetInTheBlock != null)
        {
            return alphabetInTheBlock.GetAlphabet();
        }
        return " ";
    }

    public void FillBlock(AlphabetBrick newAlphabet)
    {
        alphabetInTheBlock = newAlphabet;
        spellerBoard.UpdateWholeString();
    }

    public void ClearBlock()
    {
        frameEmission.gameObject.SetActive(false);
        alphabetInTheBlock = null;
        spellerBoard.UpdateWholeString();
    }

    public void ChangeFrameAlpha(float alpha)
    {
        Color tempColor = frameDefault.color;
        tempColor.a = alpha;
        frameDefault.color = tempColor;
    }

    public void ChangeColor(Color color)
    {
        frameEmission.gameObject.SetActive(true);
        frameEmission.color = color;
    }
}
