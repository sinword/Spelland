using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The tool to spell, turns Alphabet Bricks to interactive 3D objects.
/// </summary>
public class SpellerBoard : MonoBehaviour
{
    
    [SerializeField]
    GameObject blockPrefab;
    [SerializeField]
    Transform boardLocation;
    [SerializeField]
    float spacing = 0.1732f;
    string wholeString = "";
    [SerializeField]
    Color rightSpellColor = Color.green;
    [SerializeField]
    Color wrongSpellColor = Color.red;

    [Tooltip("The effect when a interactive 3D object shows.")]
    [SerializeField]
    GameObject effect;
    List<Block> blockList = null;

    [Tooltip("The bubble faces the camera when generated.")]
    [SerializeField]
    Transform cameraPoint;

    private List<GameObject> _createdObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (cameraPoint == null) cameraPoint = GameObject.Find("CenterEyeAnchor").transform;
        Block originBlock = GetComponentInChildren<Block>();
        blockList = new List<Block>();
        blockList.Add(originBlock);
    }

    void AddRightBlock()
    {
        GameObject newBlockObject = Instantiate(blockPrefab, transform.position + transform.right * blockList.Count * spacing, transform.rotation);
        newBlockObject.transform.SetParent(gameObject.transform);
        Block newBlock = newBlockObject.GetComponent<Block>();
        blockList.Add(newBlock);
    }

    void AddLeftBlock()
    {
        GameObject newBlockObject = Instantiate(blockPrefab, transform.position - transform.right * spacing, transform.rotation);
        newBlockObject.transform.SetParent(gameObject.transform);
        Block newBlock = newBlockObject.GetComponent<Block>();
        blockList.Insert(0, newBlock);
    }
    void RemoveBlock(int index)
    {
        GameObject targetBlockObject = blockList[index].gameObject;
        blockList.RemoveAt(index);
        Destroy(targetBlockObject);
    }

    void UpdateBoard()
    {
        float center = ((float)blockList.Count - 1f) / 2f;
        for(int i=0; i<blockList.Count; i++)
        {
            blockList[i].transform.position = transform.position + transform.right * ((float)(i) - center) * spacing;
        }
        CheckSpelling();
    }

    public void UpdateWholeString()
    {
        wholeString = "";
        for (int i = 0; i < blockList.Count; i++)
        {
            string newAlphabet = blockList[i].GetAlphabetInTheBlock();
            wholeString = wholeString + newAlphabet;
        }

        int endIdx = blockList.Count - 1;
        while (wholeString[endIdx] == ' ')
        {
            RemoveBlock(endIdx);
            endIdx--;
            if(endIdx == -1)
            {
                break;
            }
        }
        if(endIdx != -1)
        {
            int startIdx = 0;
            while (wholeString[startIdx] == ' ')
            {
                RemoveBlock(0);
                startIdx++;
            }
        }

        AddLeftBlock();
        if(blockList.Count != 1)
        {
            AddRightBlock();
        }

        wholeString = wholeString.Trim();

        UpdateBoard();

        Spell();
    }

    public void UpdateLocation()
    {
        transform.position = boardLocation.position;
        transform.LookAt(transform.position - (cameraPoint.position - transform.position), transform.up);
    }

    void Spell()
    {
        bool success = false;
        foreach (GameAsset.Spell spell in GameAsset.i.spells)
        {
            if (wholeString.ToLower() == spell.word.ToLower())
            {
                switch (spell.spellType)
                {
                    case GameAsset.SpellType.CREATE:
                        GameObject go = Instantiate(spell.createdObject, transform.position, transform.rotation);
                        _createdObjects.Add(go);
                        GameObject ef = Instantiate(effect, transform.position, transform.rotation);
                        _createdObjects.Add(ef);
                        success = true;
                        SoundManager.PlaySound("Appear");
                        break;
                    case GameAsset.SpellType.CREATE_AT_FIX_POINT:
                        GameObject go1 = Instantiate(spell.createdObject, spell.position, spell.roatation);
                        _createdObjects.Add(go1);
                        GameObject ef1 = Instantiate(effect, spell.position, spell.roatation);
                        _createdObjects.Add(ef1);
                        success = true;
                        SoundManager.PlaySound("Appear");
                        break;
                    default:
                        break;
                }
                break;
            }
        }

        if (success)
        {
            for (int i = 0; i < wholeString.Length + 1; i++)
            {
                RemoveBlock(0);
            }
            wholeString = "";
            UpdateBoard();
        }
    }

    // For hint
    void CheckSpelling()
    {
        Color newColor;
        if (GameAsset.targetWord.IndexOf(wholeString, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            newColor = rightSpellColor;
        }
        else
        {
            newColor = wrongSpellColor;
        }

        foreach (Block block in blockList)
        {
            if (" " != block.GetAlphabetInTheBlock())
            {
                block.ChangeColor(newColor);
            }
        }
    }

    public void ResetBoard()
    {
        if(!string.IsNullOrEmpty(wholeString))
        {
            for (int i = 0; i < wholeString.Length + 1; i++)
            {
                RemoveBlock(0);
            }
            wholeString = "";
            blockList[0].transform.localPosition = Vector3.zero;
        }

        for (int i = 0; i < _createdObjects.Count; i++)
        {
            Destroy(_createdObjects[i]);
        }
    }
}
