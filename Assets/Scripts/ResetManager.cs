using System.Collections.Generic;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    [Tooltip("Prefab of the Interactable Profile Object.")]
    [SerializeField]
    GameObject profilePrefab;

    [SerializeField] private SpellerBoard _spellerBoard;

    List<GameObject> objectsToDestroy = new List<GameObject>();

    struct InteractableProfileData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string spelling;

        // Constructor
        public InteractableProfileData(Vector3 position, Quaternion rotation, Vector3 scale, string spelling)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.spelling = spelling;
        }
    }

    List<InteractableProfileData> profilesToReset = new List<InteractableProfileData>();

    public void AddToDestroyWhenReset(GameObject gameObject)
    {
        objectsToDestroy.Add(gameObject);
    }

    public void AddToProfilesToReset(InteractableProfile existingProfile)
    {
        if (existingProfile != null)
        {
            profilesToReset.Add(new InteractableProfileData(
                existingProfile.transform.position,
                existingProfile.transform.rotation,
                existingProfile.transform.localScale,
                existingProfile.spelling
            ));
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Reset();
        }
    }
    public void Reset()
    {
        // Destroy all interaciable Alphabet Generators in the scene
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
        // Destroy all Alphabet Bricks in the scene
        foreach (AlphabetBrick bricks in FindObjectsOfType<AlphabetBrick>())
        {
            Destroy(bricks);
        }
        objectsToDestroy.Clear();

        // Generate new profiles
        foreach (InteractableProfileData data in profilesToReset)
        {
            GameObject newObj = Instantiate(profilePrefab, data.position, data.rotation);
            InteractableProfile newProfile = newObj.GetComponentInChildren<InteractableProfile>();
            newProfile.transform.localScale = data.scale;
            newProfile.spelling = data.spelling;
            newProfile.SwitchToAlphabetGenerator();
            AddToDestroyWhenReset(newObj);
        }

        _spellerBoard.ResetBoard();

        SoundManager.PlaySound("Chest");
    }
}
