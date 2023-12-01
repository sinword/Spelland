using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphabetSpeller : MonoBehaviour
{
    [SerializeField]
    float spacing = 1f;
    [SerializeField]
    string ownAlphabet;
    [SerializeField]
    GameObject effect;
    AlphabetSpeller leftAlphabetSpeller = null;
    AlphabetSpeller rightAlphabetSpeller = null;
    bool isGrabbed;

    public AlphabetSpeller GetSideAlphabetSpeller(string side)
    {
        switch (side)
        {
            case "right":
                return rightAlphabetSpeller;
            case "left":
                return leftAlphabetSpeller;
            default:
                break;
        }
        return null;
    }

    public string GetSideString(string side)
    {
        string sideString = ownAlphabet;
        AlphabetSpeller sideAlphabetSpeller = GetSideAlphabetSpeller(side);
        if (sideAlphabetSpeller != null)
        {
            switch (side)
            {
                case "right":
                    sideString = sideString + sideAlphabetSpeller.GetSideString(side);
                    break;
                case "left":
                    sideString = sideAlphabetSpeller.GetSideString(side) + sideString;
                    break;
                default:
                    break;
            }
        }
        return sideString;
    }

    public void DisableSide(string side, bool disableSelf = true)
    {
        AlphabetSpeller sideAlphabetSpeller = GetSideAlphabetSpeller(side);
        if (sideAlphabetSpeller != null)
        {
            sideAlphabetSpeller.DisableSide(side);
        }
        if (disableSelf)
        {
            gameObject.SetActive(false);
        }
    }

    void FollowSide(string side)
    {
        AlphabetSpeller sideAlphabetSpeller = GetSideAlphabetSpeller(side);
        if (sideAlphabetSpeller != null)
        {
            Transform sideTransform = sideAlphabetSpeller.transform;
            Vector3 localPosition = transform.position - sideTransform.position;
            if (Vector3.Dot(localPosition, sideTransform.right) > 0)
            {
                // this alphabet is on the side alphabet's right
                transform.position = sideTransform.position + spacing * sideTransform.right;
            }
            else
            {
                // this alphabet is on the side alphabet's left
                transform.position = sideTransform.position - spacing * sideTransform.right;
            }
            transform.rotation = sideTransform.rotation;
            SoundManager.PlaySound("Click");
        }
        UpdateRightLeftRelation();
    }

    void UpdateRightLeftRelation()
    {
        bool needToSwap = false;
        if(rightAlphabetSpeller != null)
        {
            Vector3 localPosition = rightAlphabetSpeller.transform.position - transform.position;
            if (Vector3.Dot(localPosition, transform.right) < 0)
            {
                needToSwap = true;
            }
        }
        else if(leftAlphabetSpeller != null)
        {
            Vector3 localPosition = leftAlphabetSpeller.transform.position - transform.position;
            if (Vector3.Dot(localPosition, transform.right) > 0)
            {
                needToSwap = true;
            }
        }

        if (needToSwap)
        {
            Debug.Log(ownAlphabet + ": Swap");
            AlphabetSpeller tmp = rightAlphabetSpeller;
            rightAlphabetSpeller = leftAlphabetSpeller;
            leftAlphabetSpeller = tmp;
        }
    }

    public void Grab()
    {
        if (!isGrabbed)
        {
            isGrabbed = true;
        }
    }

    public void Release()
    {
        if (GetSideAlphabetSpeller("left") != null)
        {
            FollowSide("left");
        }
        else if (GetSideAlphabetSpeller("right") != null)
        {
            FollowSide("right");
        }
        Spell();
    }

    void Spell()
    {
        string wholeString = ownAlphabet;
        if (leftAlphabetSpeller != null)
        {
            wholeString = leftAlphabetSpeller.GetSideString("left") + wholeString;
        }
        if (rightAlphabetSpeller != null)
        {
            wholeString = wholeString + rightAlphabetSpeller.GetSideString("right");
        }
        Debug.Log(ownAlphabet + " get whole string:" + wholeString);

        bool success = false;
        foreach (GameAsset.Spell spell in GameAsset.i.spells)
        {
            if (wholeString.ToLower() == spell.word.ToLower())
            {
                switch (spell.spellType)
                {
                    case GameAsset.SpellType.CREATE:
                        Instantiate(spell.createdObject, transform.position, transform.rotation);
                        //Instantiate(effect, transform.position, transform.rotation);
                        success = true;
                        SoundManager.PlaySound("Success");
                        break;
                    case GameAsset.SpellType.CREATE_AT_FIX_POINT:
                        Instantiate(spell.createdObject, spell.position, spell.roatation);
                        //Instantiate(effect, spell.position, spell.roatation);
                        success = true;
                        SoundManager.PlaySound("Success");
                        break;
                    default:
                        break;
                }
                break;
            }
        }

        if (success)
        {
            DisableSide("right", false);
            DisableSide("left", false);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Alphabet")
        {
            Vector3 localPosition = other.transform.position - transform.position;
            AlphabetSpeller sideAlphabetSpeller = other.gameObject.GetComponent<AlphabetSpeller>();
            if (Vector3.Dot(localPosition, transform.right) > 0)
            {
                // this alphabet is on the right
                rightAlphabetSpeller = sideAlphabetSpeller;
            } 
            else
            {
                // this alphabet is on the left
                leftAlphabetSpeller = sideAlphabetSpeller;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Alphabet")
        {
            AlphabetSpeller sideAlphabetSpeller = other.gameObject.GetComponent<AlphabetSpeller>();
            if (sideAlphabetSpeller == rightAlphabetSpeller)
            {
                // this alphabet is on the right
                rightAlphabetSpeller = null;
            }
            else if(sideAlphabetSpeller == leftAlphabetSpeller)
            {
                // this alphabet is on the left
                leftAlphabetSpeller = null;
            }
        }
    }
}
