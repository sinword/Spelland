using System;
using UnityEngine;
using Oculus.Interaction;
/// <summary>
/// This class is Alphabet Brick itself.
/// </summary>
public class AlphabetBrick : MonoBehaviour
{
    [Tooltip("The alphabet of this Alphabet Brick (e.g. A).")]
    [SerializeField]
    string ownAlphabet;

    Block targetBlock;
    bool isGrabbed = false;
    bool isInBlock = false;

    [Header("Materials")]
    [Tooltip("Default Material")]
    [SerializeField]
    Material defaultMaterial;
    [Tooltip("Material when showing hint")]
    [SerializeField]
    Material hintMaterial;
    [Tooltip("The Target Word Hint Manager in this scene, will find in the scene if not assigned.")]
    [SerializeField]
    TargetWordHintManager targetWordHintManager;

    [Header("To make this interactable with hands")]
    [SerializeField]
    GameObject grabInteractable;
    [SerializeField]
    GameObject handGrabInteractable_Left;
    [SerializeField]
    GameObject handGrabInteractable_Right;

    PhysicsGrabbable physicsGrabbable;
    bool isInTargetWord = false;

    void Start()
    {
        if (ownAlphabet != null)
        {
            SetMesh();
        }
        physicsGrabbable = GetComponent<PhysicsGrabbable>();
        targetWordHintManager = FindObjectOfType<TargetWordHintManager>();
    }

    // Set the alphabet
    public void SetOwnAlphabet(string newAlphabet)
    {
        ownAlphabet = newAlphabet;
        SetMesh();

        // If the alphabet is in the target word.
        if (GameAsset.targetWord.IndexOf(ownAlphabet, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // This Alphabet should get hint
            isInTargetWord = true;
        }
    }

    // Set the corresponding 3D model
    void SetMesh()
    {
        foreach (GameAsset.Alphabet alphabet in GameAsset.i.alphabets)
        {
            if (alphabet.alphbetName.Equals(ownAlphabet, System.StringComparison.CurrentCultureIgnoreCase))
            {
                GetComponentInChildren<MeshFilter>().mesh = alphabet.model;
            }
        }
    }

    // Getter for other game objects
    public string GetAlphabet()
    {
        return ownAlphabet;
    }

    // When grabbed
    public void Grab()
    {
        StopHint();
        if (!isGrabbed)
        {
            isGrabbed = true;
            transform.SetParent(null);
        }
    }

    // When released from hand
    public void Release()
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            if (targetBlock != null)
            {
                physicsGrabbable.enabled = false;
                GetComponent<Rigidbody>().isKinematic = true;
                SetInBlock();
            }
            else
            {
                GetComponent<Rigidbody>().isKinematic = false;
                physicsGrabbable.enabled = true;
            }
        }
    }

    // When released while close enough to a Block of the Speller Board, this Alphabet Brick sould stay in the Block.
    void SetInBlock()
    {
        isInBlock = true;
        transform.position = targetBlock.transform.position;
        transform.rotation = targetBlock.transform.rotation;
        transform.SetParent(targetBlock.transform);
        targetBlock.FillBlock(this);
        targetBlock.ChangeFrameAlpha(1f);
        SoundManager.PlaySound("UI_Click");
    }

    // When want to put into Speller Board
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Block") && targetBlock == null)
        {
            targetBlock = other.gameObject.GetComponent<Block>();
            targetBlock.ChangeFrameAlpha(0.5f);
            // if this block is filled
            if (targetBlock.GetAlphabetInTheBlock() != " ")
            {
                targetBlock = null;
            }
        }
    }

    // When taken out from Speller Board
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Block") && targetBlock != null)
        {
            isInBlock = false;
            transform.SetParent(null);
            targetBlock.ClearBlock();
            targetBlock.ChangeFrameAlpha(0.25f);
            targetBlock = null;
        }
    }

    // When dropped
    private void OnCollisionEnter(Collision collision)
    {   
        // Seems no need for this now
        // if (collision.gameObject.CompareTag("Floor"))        

        grabInteractable.SetActive(true);
        handGrabInteractable_Left.SetActive(true);
        handGrabInteractable_Right.SetActive(true);

        // Start the timer to show hint
        if (isInTargetWord)
        {
            StartCoroutine(targetWordHintManager.ShowBrickHintCountDown(this));
        }
    }

    // Show hint (emission effect) when not picked up / not in the Speller Board Block for a fixed seconds 
    public void ShowHint()
    {
        if (isGrabbed || isInBlock) return;
        GetComponentInChildren<MeshRenderer>().material = hintMaterial;
    }

    // Stop showing hint when picked up this
    void StopHint()
    {
        GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
    }
}
