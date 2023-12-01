using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cast ray from index finger to interact with Alphabat Generators
/// </summary>
public class RayCaster : MonoBehaviour
{
    [Tooltip("The layermask to check for collisions.")]
    [SerializeField]
    LayerMask layerAlphabetGenerator;
    [Tooltip("The max distance the ray should check for collisions.")]
    [SerializeField]
    float maxDistance = 50f;
    [Tooltip("Whether use the line renderer to show the ray.")]
    [SerializeField]
    bool useVisibleRay = true;
    [Tooltip("The time in seconds that the ray must be hitting the AlphabetGenerator before interaction begins.")]
    [SerializeField]
    float interactionDelay = 1f;
    #region LineRenderer
    LineRenderer lineRenderer;
    Color hitStart = new Color(0.3254717f, 1f, 0.7896893f);
    Color end = new Color(0.2311321f, 0.6582434f, 1f);
    Color nohitStart = new Color(1f, 0.4323243f, 0.3254902f);
    #endregion
    bool isHitting = false;
    float hitTime = 0f;
    AlphabetGenerator currentGenerator;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        int layerIndex = LayerMask.NameToLayer("Alphabet Generator");
        if (-1 == layerIndex)
        {
            Debug.LogError("LayerMask \"Alphabet Generator\" is missing.");
            gameObject.SetActive(false);
        }
        else
        {
            layerAlphabetGenerator = 1 << layerIndex;
        }

        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        ResetInteraction();
    }

    void Update()
    {
        if (useVisibleRay)
        {
            lineRenderer.SetPosition(0, transform.position);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitInfo, maxDistance, layerAlphabetGenerator))
        {
            if (!isHitting)
            {
                currentGenerator = hitInfo.transform.GetComponentInChildren<AlphabetGenerator>();
                if (currentGenerator != null)
                {
                    currentGenerator.OpenHighlight();
                    // No need to count if another interaction is happening.
                    if (!AlphabetBubble.ExistsAny()) hitTime = Time.time;
                }
                else
                {
                    Debug.LogError("The object hit by raycast is not a Alphabet Generator, Object name: " + hitInfo.transform.name);
                }
                isHitting = true;
            }
            else if (Time.time - hitTime >= interactionDelay)
            {
                currentGenerator.StartInteraction();
                ResetInteraction();
            }

            if (useVisibleRay)
            {
                lineRenderer.enabled = true;
                lineRenderer.startColor = hitStart;
                lineRenderer.SetPosition(1, hitInfo.point);
            }
        }
        else
        {
            if (isHitting)
            {
                ResetInteraction();
            }
            if (useVisibleRay)
            {
                lineRenderer.enabled = true;
                lineRenderer.startColor = nohitStart;
                Vector3 rayEndPoint = transform.position + transform.TransformDirection(Vector3.forward) * maxDistance;
                lineRenderer.SetPosition(1, rayEndPoint);
            }
        }
    }

    void ResetInteraction()
    {
        if (currentGenerator != null) currentGenerator.CloseHighlight();
        currentGenerator = null;
        hitTime = 0f;
        isHitting = false;
    }
}
