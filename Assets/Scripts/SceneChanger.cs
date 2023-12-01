using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// A simple tool to change scene with animation.
/// </summary>
public class SceneChanger : MonoBehaviour
{
    Animator animator;
    int sceneToLoad;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            FadeToScene(0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            FadeToScene(1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            FadeToScene(2);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeToScene(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
    }
    private void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
