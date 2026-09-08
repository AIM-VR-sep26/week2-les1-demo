using UnityEngine;

public sealed class DoorLock : MonoBehaviour
{
    [SerializeField] private Light lockLight;
    [SerializeField] private Vector3 inwardOffset;
    [SerializeField] private float slideSpeed = 2.5f;

    private Vector3 outwardPosition;
    private bool active;

    private void Awake()
    {
        outwardPosition = transform.localPosition;
        lockLight.gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector3 target = active ? outwardPosition + inwardOffset : outwardPosition;
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            target,
            slideSpeed * Time.deltaTime);
    }

    public void Activate()
    {
        active = true;
        lockLight.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        active = false;
        lockLight.gameObject.SetActive(false);
    }
}
