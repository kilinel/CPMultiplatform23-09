using UnityEngine;
using Vuforia;

public class TargetContentController : MonoBehaviour
{
    [SerializeField] ObserverBehaviour target;
    [SerializeField] GameObject contentRoot;

    void Awake()
    {
        contentRoot.SetActive(false);
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
    }
}
