using System.Collections;
using UnityEngine;

public class LastBossFlyAXE : MonoBehaviour
{
    [SerializeField] private float rotationTime = 10.0f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float maxSpeedTime = 2.0f;

    private float currentSpeed = 0.0f;
    private Vector3 rotationAxis;

    private void Start()
    {
        // 각 오브젝트의 이름에 따라 회전 축을 설정합니다.
        if (gameObject.name == "FlyAXE")
            rotationAxis = Vector3.right;

        else if (gameObject.name == "FlyAXE (1)")
            rotationAxis = Vector3.forward;

        StartCoroutine(LookAtMonster());
        StartCoroutine(AddSpeed());
    }

    private void Update()
    {
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    private IEnumerator LookAtMonster()
    {
        while (true)
        {
            Vector3 lookAtTransform = transform.parent.position - transform.position + Vector3.up * 7.5f;
            Quaternion lookAt = Quaternion.LookRotation(Quaternion.AngleAxis(45, rotationAxis) * lookAtTransform);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookAt, rotationTime * Time.deltaTime);

            yield return null;
        }
    }

    private IEnumerator AddSpeed()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, maxSpeedTime * Time.deltaTime);
            yield return null;
        }
    }
}
