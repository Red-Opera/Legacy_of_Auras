using System.Collections;
using UnityEngine;

public class AXETracePlayer : MonoBehaviour
{
    public float waitTime = 1.5f;
    public float acceleration = 10.0f;
    public float maxSpeed = 20.0f;

    private static GameObject player;
    private static SkinnedMeshRenderer playerCenter;
    private bool isTrace = false;
    private float currentSpeed = 0.0f;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Model");
            playerCenter = player.transform.Find("HuntressCoat_L.001_HuntressClothesUnwrap1:Mesh.003").GetComponent<SkinnedMeshRenderer>();
        }

        StartCoroutine(DestroyObject());
        StartCoroutine(WaitAXE());
    }

    private void Update()
    {
        if (isTrace)
        {
            // �ִ� �ӵ��� ������ ������ �ӵ��� ������ ����
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
            transform.position += transform.forward * currentSpeed * Time.deltaTime;

            // �÷��̾� ���� �ٶ󺸱�
            transform.LookAt(playerCenter.bounds.center);
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
