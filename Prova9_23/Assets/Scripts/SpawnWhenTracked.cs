using UnityEngine;
using Vuforia;

public class SpawnWhenTracked : MonoBehaviour
{
    [SerializeField] ObserverBehaviour target;
    [SerializeField] GameObject prefab;
    [SerializeField] Transform anchor;

    GameObject instance;

    void Awake()
    {
        target.OnTargetStatusChanged += OnStatusChanged;
    }

    void OnDestroy()
    {
        target.OnTargetStatusChanged -= OnStatusChanged;
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool tracked =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;

        if (tracked && instance == null)
            instance = Instantiate(prefab, anchor.position, anchor.rotation, anchor);

        if (!tracked && instance != null)
            Destroy(instance);
    }
}
