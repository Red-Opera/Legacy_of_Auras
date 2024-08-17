using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowTrace : MonoBehaviour
{
    [HideInInspector] public MonsterState state;

    [SerializeField] private float accelerationRate = 4.0f;   // ���ӵ� ����
    [SerializeField] private float maxSpeed = 30.0f;          // ȭ�� �ӵ�

    private static GameObject player;
    private float currentSpeed;

    private void Start()
    {
        if (player == null)
            player = GameObject.Find("MonsterTarget");

        if (SceneManager.GetActiveScene().name == "Final")
            transform.localScale *= 3.0f;
    }

    private void FixedUpdate()
    {
        transform.LookAt(player.transform.position + new Vector3(0.0f, 1.0f, 0.0f));

        // ����
        if (currentSpeed < maxSpeed)
            currentSpeed += accelerationRate * Time.deltaTime;

        // �̵�
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Model")
        {
            PlayerHPBar.SetDamage(state.attack);
            Destroy(gameObject);
        }
    }
}
