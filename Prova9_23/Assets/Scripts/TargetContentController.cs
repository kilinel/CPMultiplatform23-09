using TMPro;
using UnityEngine;
using Vuforia;

public class TargetContentController : MonoBehaviour
{
    [SerializeField] ObserverBehaviour target;
    [SerializeField] GameObject contentRoot;
    [SerializeField] GameObject pokedexPanel;
    [SerializeField] TMP_Text foundText;
    [SerializeField] TMP_Text interactionText;
    void Awake()
    {
        contentRoot.SetActive(false);
       
        if (pokedexPanel != null)
            pokedexPanel.SetActive(false);

        if (foundText != null)
            foundText.gameObject.SetActive(false);

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);


        target.OnTargetStatusChanged += OnStatusChanged;


    }

    void OnDestroy()
    {
        target.OnTargetStatusChanged -= OnStatusChanged;
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool visible =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;

        contentRoot.SetActive(visible);

        if (foundText != null)
            foundText.gameObject.SetActive(visible);

        if (interactionText != null)
            interactionText.gameObject.SetActive(visible);


        if (!visible && pokedexPanel != null)
            pokedexPanel.SetActive(false);
    }
}
