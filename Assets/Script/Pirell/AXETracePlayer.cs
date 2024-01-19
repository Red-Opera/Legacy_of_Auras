using System.Collections;
using UnityEngine;

public class AXETracePlayer : MonoBehaviour
{
    public float waitTime = 1.5f;
    public float acceleration = 10.0f;
    public float maxSpeed = 20.0f;

    private GameObject player;
    private bool isTrace = false;
    private float currentSpeed = 0.0f;

    private void Start()
    {
        player = GameObject.Find("Model");
        StartCoroutine(DestroyObject());
        StartCoroutine(WaitAXE());
    }

    private void Update()
    {
        if (isTrace)
        {
            // 최대 속도에 도달할 때까지 속도를 서서히 증가
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
            transform.position += transform.forward * currentSpeed * Time.deltaTime;

            // 플레이어 쪽을 바라보기
            transform.LookAt(player.transform);
        }
    }

    private IEnumerator WaitAXE()
    {
        yield return new WaitForSeconds(waitTime);
        isTrace = true;
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Model")
            Destroy(gameObject);
    }
}
