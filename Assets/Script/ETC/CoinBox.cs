using UnityEngine;

public class CoinBox : MonoBehaviour
{
    public float upPower = 5000.0f;     // ������ �� ���� �ö󰡴� ��
    public float rotationSpeed = 25.0f; // ȸ�� �ӵ�
    public PlayerState playerState;     // �÷��̾� ����

    private Rigidbody rigidbody;
    private int dropMoney;              // �� ��� �ڽ��� ��

    private Alert alert;                // �÷��̾� �˸� UI

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null, "Error (Null Reference) : Rigidbody�� �������� �ʽ��ϴ�.");

        rigidbody.AddForce(new Vector3(0, upPower, 0));
    }

    public void Update()
    {
        // ������Ʈ�� õõ�� ȸ����Ŵ
        float rotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);
    }

    public void SetDropGold(int gold) { dropMoney = gold; }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (alert == null)
                alert = GameObject.Find("UI").GetComponent<Alert>();

            playerState.money += dropMoney;

            alert.PushAlert(dropMoney.ToString() + "G�� ȹ�� �߽��ϴ�. (���� ��� : " + playerState.money.ToString() + ")");
            Destroy(transform.parent.gameObject);
        }
    }
}
