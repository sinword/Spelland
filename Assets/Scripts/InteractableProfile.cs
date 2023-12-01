using UnityEngine;
using Oculus.Interaction;

public class InteractableProfile : MonoBehaviour
{
    public string spelling;
    ProfileManager profileManager;
    [SerializeField]
    AlphabetGenerator alphabetGenerator;

    void Start()
    {
        // Get from scene
        profileManager = FindObjectOfType<ProfileManager>();
    }

    public void onSelect()
    {
        profileManager.SelectProfile(this);
    }

    public void SwitchToAlphabetGenerator()
    {
        // This actually doesn't work as expected :(
        GetComponent<Grabbable>().enabled = false;

        alphabetGenerator.spelling = spelling;
        alphabetGenerator.enabled = true;
    }
}
