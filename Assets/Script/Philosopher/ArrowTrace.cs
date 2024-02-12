using UnityEngine;

public class ArrowTrace : MonoBehaviour
{
    public float accelerationRate = 4.0f;   // ���ӵ� ����
    public float maxSpeed = 30.0f;          // ȭ�� �ӵ�

    private static GameObject player;
    private float currentSpeed;

    void Start()
    {
        if (player == null)
            player = GameObject.Find("MonsterTarget");
    }

    void Update()
    {
        transform.LookAt(player.transform.position + new Vector3(0.0f, 1.0f, 0.0f));

        // ����
        if (currentSpeed < maxSpeed)
            currentSpeed += accelerationRate * Time.deltaTime;

        // �̵�
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
