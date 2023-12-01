using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Horizontal layout tool for Alphabet Bricks.
/// </summary>
public class AlphabetLayoutGroup : MonoBehaviour
{
    [Tooltip("The prefab of the Alphabet object.")]
    [SerializeField]
    GameObject alphabetPrefab;
    [Tooltip("Spacing between the Alphabets")]
    [SerializeField]
    float spacing = 0.12f;
    [Tooltip("The alphabets objects in this spelling.")]
    [SerializeField]
    List<GameObject> alphabets;

    [Header("Reset")]
    [Tooltip("The Reset Manager in this scene, will find in the scene if not assigned.")]
    [SerializeField]
    ResetManager resetManager;

    void Start()
    {
        if (alphabets.Count > 0) ArrangeLayout();

        if (resetManager == null) resetManager = FindObjectOfType<ResetManager>();
    }

    public void AddAlphbet(GameObject newAlphabet)
    {
        alphabets.Add(newAlphabet);
        ArrangeLayout();
    }
    
    public void ArrangeLayout()
    {
        float numOfLetters = alphabets.Count;
        float midNum = ((float) (alphabets.Count + 1)) / 2;

        for (int i = 0; i < numOfLetters; i++)
        {
            alphabets[i].transform.localPosition = (new Vector3((midNum-i-1) * spacing, 0, 0));
        }
    }
    
    public void SetUp(string spelling)
    {
        // Generate Alphabets
        for (int i = 0; i < spelling.Length; i++)
        {
            GameObject newAlphabetObject = Instantiate(
                alphabetPrefab,
                transform
            );
            newAlphabetObject.transform.Rotate(new Vector3(0, 180, 0));
            newAlphabetObject.GetComponentInChildren<AlphabetBrick>().SetOwnAlphabet(spelling[i].ToString());
            // Put into Alphabet Layout Group
            newAlphabetObject.transform.SetParent(transform);
            AddAlphbet(newAlphabetObject);
        }
        ArrangeLayout();
    }

    // When bubble is popped
    public void Dismiss()
    {
        for (int i = 0; i < alphabets.Count; i++)
        {
            alphabets[i].transform.SetParent(null);
            alphabets[i].GetComponent<Rigidbody>().isKinematic = false;
            resetManager.AddToDestroyWhenReset(alphabets[i]);
        }
    }
}
