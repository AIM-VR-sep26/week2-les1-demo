using UnityEngine;
using UnityEngine.Events;

/// <summary>Fires an event while something heavy enough rests on the plate.</summary>
[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Tooltip("Objects lighter than this are ignored.")]
    [SerializeField] private float requiredMass = 1f;
    [SerializeField] private float pressDepth = 0.02f;
    [SerializeField] private AudioClip clickSound;

    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private Vector3 restPosition;
    private AudioSource speaker;
    private int load;

    private void Awake()
    {
        restPosition = transform.localPosition;
        speaker = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector3 target = load > 0
            ? restPosition + new Vector3(0f, -pressDepth, 0f)
            : restPosition;
        transform.localPosition = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Counts(other)) return;
        load++;
        if (load == 1) Press();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Counts(other)) return;
        load--;
        if (load == 0) onReleased.Invoke();
    }

    private bool Counts(Collider other)
    {
        return other.TryGetComponent(out Rigidbody body)
               && body.mass >= requiredMass;
    }

    private void Press()
    {
        onPressed.Invoke();
        speaker.PlayOneShot(clickSound, Random.Range(0.9f, 1f));
    }
}
