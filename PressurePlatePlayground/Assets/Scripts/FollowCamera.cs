using UnityEngine;

public sealed class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -8f);

    public void SetTarget(Transform value)
    {
        target = value;
    }

    private void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 0.5f);
    }
}
