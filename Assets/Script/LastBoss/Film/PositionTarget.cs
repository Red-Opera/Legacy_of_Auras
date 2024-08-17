using UnityEngine;

public class PositionTarget : MonoBehaviour
{
    [SerializeField] private Transform toTarget;
    [SerializeField] private string toTargetString;

    private void Start()
    {
        if (toTarget == null)
            toTarget = GameObject.Find(toTargetString).transform;
    }

    private void FixedUpdate()
    {
        if (toTarget != null)
        {
            transform.position = toTarget.position;
            transform.rotation = toTarget.rotation;
        }
    }
}
