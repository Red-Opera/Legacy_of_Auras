using UnityEngine;

public class ArrowTrace : MonoBehaviour
{
    public float accelerationRate = 4.0f;   // 가속도 비율
    public float maxSpeed = 30.0f;          // 화살 속도

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

        // 가속
        if (currentSpeed < maxSpeed)
            currentSpeed += accelerationRate * Time.deltaTime;

        // 이동
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }
}
