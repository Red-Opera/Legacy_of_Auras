using UnityEngine;

public class PositionTarget : MonoBehaviour
{
    [SerializeField] private Transform toTarget;
    [SerializeField] private string toTargetString;

    void Start()
    {
        if (toTarget == null)
            toTarget = GameObject.Find(toTargetString).transform;
    }

    void Update()
    {
        transform.position = toTarget.position;
        transform.rotation = toTarget.rotation;
    }
}
